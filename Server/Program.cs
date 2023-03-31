//#define UseSockets

using System.Net.Sockets;
using System.Net;
using Server;

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


//client simulation for search
//SearchBody body = new SearchBody(0, "testSong");
//PacketHeader head = new PacketHeader(PacketHeader.SongAction.List);

//Packet pk = new Packet(head, body);
Packet pk = Utils.GenerateClientSearchPacket("test");
byte[] tx = pk.Serialize();

//accountController.AddAccount("user", "password");
//PacketHeader packetHeader = new(PacketHeader.AccountAction.SignUp);
//PacketBody packetBody = new Account("user", "");
//Packet packet = new(packetHeader, packetBody);

//byte[] bytes = packet.Serialize();

//server stuff

//Unpack incoming packet
Packet rx = new Packet(tx);
logger.Log(rx, false);

//check for packet type and handle accordingly
switch(rx.header.GetPacketType())
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
                if(accountReceived.getPassword() == string.Empty)
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
                
                throw new NotImplementedException();

                break;
            case PacketHeader.SongAction.List:

                SearchBody sb = (SearchBody)rx.body;
                List<Song> results = Utils.SearchSong(songController, sb.GetFilter());

                //create the packet after here
                sb.appendServerResponse(Utils.GenerateServerSearchResponse(results, albumController));
                break;
        }
        break;
}


logger.Log(rx, true);

Song testSong = songController.FindSong("testSong");

Console.WriteLine(testSong.GetArtist());

//PacketHeader packetHeader = new(PacketHeader.AccountAction.SignUp);
//PacketBody packetBody = new Account("user", "password");
//Packet packet = new(packetHeader, packetBody);

//byte[] bytes = packet.Serialize();

//Console.WriteLine("\npacket info when received from client: \n");
//Packet anotherPacket = new(bytes);
//Console.WriteLine(anotherPacket.header.GetPacketType());
//Console.WriteLine(anotherPacket.header.GetAccountAction());

//Account account = (Account)anotherPacket.body;
//Console.WriteLine(account.getUsername());
//Console.WriteLine(account.getPassword());
//Console.WriteLine(account.getStatus());

//account.setStatus(Account.Status.Success);

//byte[] anotherBytes = anotherPacket.Serialize();
//Packet clientPacket = new(anotherBytes);

//Console.WriteLine("\npacket info when client receives it:\n");
//Console.WriteLine(clientPacket.header.GetPacketType());
//Console.WriteLine(clientPacket.header.GetAccountAction());

//Account account2 = (Account)clientPacket.body;
//Console.WriteLine(account2.getUsername());
//Console.WriteLine(account2.getPassword());
//Console.WriteLine(account2.getStatus());


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

        Packet rxPacket;
        byte[] buffer = new byte[1024];
        int receivedBytes;
        int offset;

        // Connection will run until Client disconnects, then goes back to listening
        while ((receivedBytes = stream.Read(buffer, 0, buffer.Length)) > 0)
        {
            rxPacket = new Packet(buffer);
            switch (rxPacket.header.GetPacketType())
            {
                case PacketHeader.Type.Account:

                    throw new NotImplementedException();

                    break;

                case PacketHeader.Type.Song:
                    switch (rxPacket.header.GetSongAction())
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

                            throw new NotImplementedException();

                            break;
                        case PacketHeader.SongAction.List:

                            throw new NotImplementedException();
                            break;
                    }
                    break;
            }

        }

        Console.WriteLine("\n Closing current Client Connection \n");
    }

}
finally
{
    listener.Stop();
}
#endif