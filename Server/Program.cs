using System.Net.Sockets;
using System.Net;
using Server;


Directory.SetCurrentDirectory("../../../");

//init databases
SongController songController = new SongController();
AlbumController albumController = new AlbumController();
ArtistController artistController = new ArtistController();

//get some safe file handling up in here
FileHandler.ReadSongs(songController, "songs.txt");
//FileHandler.ReadArtists(artistController, "artists.txt");
//FileHandler.ReadAlbums(albumController, "albums.txt");


//client simulation for search
//SearchBody body = new SearchBody(0, "testSong");
//PacketHeader head = new PacketHeader(PacketHeader.SongAction.List);

//Packet pk = new Packet(head, body);
Packet pk = Utils.generateClientSearchPacket("test");
byte[] tx = pk.Serialize();

//server stuff

//Unpack incoming packet
Packet rx = new Packet(tx);

//check for packet type and handle accordingly
switch(rx.header.GetPacketType())
{
    case PacketHeader.Type.Account:
        
        throw new NotImplementedException();

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
                String filter = "test"; //sb.GetFilter();
                List<Song> results = Utils.searchSong(songController, filter);

                //create the packet after here
                Packet responsePk = Utils.generateServerSearchResponse(results);
                break;
        }
        break;
}



Song testSong = songController.FindSong("testSong");

Console.WriteLine(testSong.GetArtist());

PacketHeader packetHeader = new(PacketHeader.AccountAction.SignUp);
PacketBody packetBody = new Account("user", "password");
Packet packet = new(packetHeader, packetBody);

byte[] bytes = packet.Serialize();

Console.WriteLine("\npacket info when received from client: \n");
Packet anotherPacket = new(bytes);
Console.WriteLine(anotherPacket.header.GetPacketType());
Console.WriteLine(anotherPacket.header.GetAccountAction());

Account account = (Account)anotherPacket.body;
Console.WriteLine(account.getUsername());
Console.WriteLine(account.getPassword());
Console.WriteLine(account.getStatus());

account.setStatus(Account.Status.Success);

byte[] anotherBytes = anotherPacket.Serialize();
Packet clientPacket = new(anotherBytes);

Console.WriteLine("\npacket info when client receives it:\n");
Console.WriteLine(clientPacket.header.GetPacketType());
Console.WriteLine(clientPacket.header.GetAccountAction());

Account account2 = (Account)clientPacket.body;
Console.WriteLine(account2.getUsername());
Console.WriteLine(account2.getPassword());
Console.WriteLine(account2.getStatus());

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

        byte[] buffer = new byte[1024];
        int receivedBytes;
        int offset;

        // Connection will run until Client disconnects, then goes back to listening
        while ((receivedBytes = stream.Read(buffer, 0, buffer.Length)) > 0)
        {
            
            //offset = 0;
            //PacketHeader packetHeader = new(buffer[offset++]); // header located in the first byte of the packet

            //PacketHeader.Type packetType = packetHeader.GetPacketType();

            //if(packetType == PacketHeader.Type.Account)
            //{
            //    PacketHeader.AccountAction accountAction = packetHeader.GetAccountAction();

            //    if (accountAction is not 0)
            //    {
            //        switch (accountAction)
            //        {
            //            case PacketHeader.AccountAction.SignUp:

            //                break;
            //            case PacketHeader.AccountAction.LogIn:
            //                break;
            //        }
            //    }
            //}
            //else if(packetType == PacketHeader.Type.Song)
            //{

            //}

        }

        Console.WriteLine("\n Closing current Client Connection \n");
    }

}
finally
{
    listener.Stop();
}