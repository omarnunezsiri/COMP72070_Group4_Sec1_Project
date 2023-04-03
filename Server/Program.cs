#define UseSockets

using System.Net.Sockets;
using System.Net;
using Server;
using System.Drawing;

/* Defaults to the Server project directory instead of the Debug (which is ignored by our .gitignore file). */
Directory.SetCurrentDirectory("../../../");

Logger logger = Logger.Instance;
Logger.SetFileName(Constants.TextDirectory + Constants.ServerLogsFile);

//init databases
AccountController accountController = new AccountController();
SongController songController = new SongController();
AlbumController albumController = new AlbumController();
ArtistController artistController = new ArtistController();

//get some safe file handling up in here
FileHandler.ReadSongs(songController, Constants.TextDirectory + Constants.SongsFile);
FileHandler.ReadAlbums(albumController, Constants.TextDirectory + Constants.AlbumsFile, Constants.ImagesDirectory);
FileHandler.ReadArtists(artistController, Constants.TextDirectory + Constants.ArtistsFile, Constants.ImagesDirectory);
FileHandler.ReadAccounts(accountController, Constants.TextDirectory + Constants.AccountsFile);


#if (UseSockets)
IPEndPoint ipEndPoint = new(IPAddress.Any, Constants.PortNumber);
TcpListener listener = new(ipEndPoint);

try
{
    Console.WriteLine("---- Silly Music Player Running on SSP (3.5.2) ----\n");
    listener.Start();

    // keep listening for connections until interrupted
    while (true)
    {
        Console.WriteLine("Waiting for Client Connection...");
        using TcpClient handler = await listener.AcceptTcpClientAsync();
        Console.WriteLine("Connection Established! Waiting for login/signup...\n");

        await using NetworkStream stream = handler.GetStream();

        Packet rx;
        byte[] buffer = new byte[1024];
        int receivedBytes;

        // Connection will run until Client disconnects, then goes back to listening
        try
        {
            bool individualSend;
            while ((receivedBytes = stream.Read(buffer)) > 0)
            {
                individualSend = true;

                byte[] receivedBuffer = new byte[receivedBytes];
                Array.Copy(buffer, receivedBuffer, receivedBytes);
                rx = new Packet(receivedBuffer);

                logger.Log(rx, false);
                //check for packet type and handle accordingly
                switch (rx.header.GetPacketType())
                {
                    case PacketHeader.Type.Account:

                        Account accountReceived = (Account)rx.body;

                        switch (rx.header.GetAccountAction())
                        {
                            case PacketHeader.AccountAction.NotApplicable:
                                /* Work around to reset password after user was found */
                                accountController.FindAccount(accountReceived.getUsername()).setPassword(accountReceived.getPassword());
                                accountReceived.setStatus(Account.Status.Success);
                                break;
                            case PacketHeader.AccountAction.SignUp:
                                /* If we receive an empty password, the user is attempting to create a username that doesn't exist */
                                if (accountReceived.getPassword() == string.Empty)
                                {
                                    try
                                    {
                                        Account account = accountController.FindAccount(accountReceived.getUsername());
                                        accountReceived.setStatus(Account.Status.Failure);
                                    }
                                    catch (Exception)
                                    {
                                        accountReceived.setStatus(Account.Status.Success);
                                    }
                                } /* If we can add the account to the Dictionary, response is success. Failure otherwise. */
                                else if (accountController.AddAccount(accountReceived.getUsername(), accountReceived.getPassword()))
                                    accountReceived.setStatus(Account.Status.Success);
                                else
                                    accountReceived.setStatus(Account.Status.Failure);
                                break;
                            case PacketHeader.AccountAction.LogIn:
                                bool success = false;
                                try
                                {
                                    Account foundAccount = accountController.FindAccount(accountReceived.getUsername());

                                    /* If password is empty, user is trying to reset their password */
                                    if ((accountReceived.getPassword() == string.Empty))
                                        success = true;
                                    else
                                    {
                                        /* If password is not empty, user is trying to log in */
                                        if (accountReceived.getPassword() == foundAccount.getPassword())
                                            success = true;
                                    }
                                }
                                catch (KeyNotFoundException)
                                {

                                }

                                if (success)
                                    accountReceived.setStatus(Account.Status.Success);
                                else
                                    accountReceived.setStatus(Account.Status.Failure);
                                break;
                        }
                        break;

                    case PacketHeader.Type.Song:
                        switch (rx.header.GetSongAction())
                        {
                            case PacketHeader.SongAction.NotApplicable:

                                throw new NotImplementedException();

                                break;
                            case PacketHeader.SongAction.Sync:

                                throw new NotImplementedException();

                                break;
                            case PacketHeader.SongAction.Media:

                                throw new NotImplementedException();

                                break;
                            case PacketHeader.SongAction.Download:

                                DownloadBody db = (DownloadBody)rx.body;
                                string hash = db.GetHash();
                                switch (db.GetType())
                                {
                                    case DownloadBody.Type.AlbumCover:
                                        try
                                        {
                                            Album album = albumController.FindAlbum(hash);
                                            byte[] coverBytes = Utils.GetBitmapBytes(album.GetImage());
                                            await TransmitDownloadData(rx, stream, coverBytes);
                                            individualSend = false;
                                        }
                                        catch (Exception)
                                        {

                                        }
                                        break;
                                    case DownloadBody.Type.SongFile:
                                        try
                                        {
                                            byte[] mp3Bytes = FileHandler.readMp3Bytes(Constants.Mp3sDirectory + $"{hash}.mp3");
                                            await TransmitDownloadData(rx, stream, mp3Bytes);
                                            individualSend = false;
                                        }
                                        catch (Exception)
                                        {

                                        }
                                        break;
                                }

                                break;
                            case PacketHeader.SongAction.List:

                                SearchBody sb = (SearchBody)rx.body;
                                List<Song> results = Utils.SearchSong(songController, sb.GetFilter());

                                //create the packet after here
                                sb.appendServerResponse(Utils.GenerateServerSearchResponse(results));
                                break;
                        }
                        break;
                }

                if(individualSend)
                {
                    logger.Log(rx, true);

                    byte[] TxBuffer = rx.Serialize();
                    stream.Write(TxBuffer);
                }

            }
        }
        catch (IOException)
        {

        }
        Console.WriteLine("\n Closing current Client Connection \n");
    }

}
finally
{
    listener.Stop();
}
#endif

static async Task TransmitDownloadData(Packet rx, NetworkStream stream, byte[] bytes)
{
    Logger instance = Logger.Instance;

    ushort blockIndex;
    ushort totalBlocks = (ushort)(bytes.Length / Constants.BufferMax);
    uint toTake = 0;
    int offset = 0;

    DownloadBody db = (DownloadBody)rx.body;

    for (blockIndex = 0; blockIndex < totalBlocks; blockIndex++)
    {
        if (offset < (bytes.Length - Constants.BufferMax))
            toTake = Constants.BufferMax;
        else
            toTake = (uint)(bytes.Length - Constants.BufferMax);

        byte[] dataBytes = new byte[toTake];
        Array.Copy(bytes, offset, dataBytes, 0, toTake);
        offset += (int)toTake;

        db.appendServerResponse(blockIndex, totalBlocks, (uint)dataBytes.Length, dataBytes);

        byte[] serialized = rx.Serialize();
        await stream.WriteAsync(serialized);

        instance.Log(rx, true);
    }
}