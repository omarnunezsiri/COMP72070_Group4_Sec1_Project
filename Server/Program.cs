// See https://aka.ms/new-console-template for more information


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