
using Server;
using System.Drawing;

namespace Client.Tests
{
    [TestClass]
    public class SearchTests
    {
        [TestMethod]
        public void CLIENTSEARCH002_PopulateSearchResults_NonEmptyResponse_SongsAdded()
        {
            // Arrange
            const int ExpectedSize = 2;
            bool rightSize = false;
            bool firstSongFound = false;
            bool secondSongFound = false;

            List<Song> songs = new();
            SongController songController = new();
            AlbumController albumController = new();

            FileHandler.ReadSongs(songController, "SearchTextFiles/" + Constants.SongsFile);
            FileHandler.ReadAlbums(albumController, "SearchTextFiles/" + Constants.AlbumsFile, "");

            /* Prep for byte array */
            Song song1 = songController.FindSong("testSong");
            Song song2 = songController.FindSong("testSong2");
            Album album1 = albumController.FindAlbum(song1.GetAlbum());
            Album album2 = albumController.FindAlbum(song2.GetAlbum());

            byte[] song1Bytes = song1.Serialize();
            byte[] song2Bytes = song2.Serialize();
            byte[] album1CoverBytes = Utils.GetBitmapBytes(album1.GetImage());
            byte[] cover1LengthBytes = BitConverter.GetBytes(album1CoverBytes.Length);
            byte[] album2CoverBytes = Utils.GetBitmapBytes(album2.GetImage());
            byte[] cover2LengthBytes = BitConverter.GetBytes(album2CoverBytes.Length);

            int offset = 0;
            byte[] bytes = new byte[song1Bytes.Length + song2Bytes.Length + album1CoverBytes.Length + cover1LengthBytes.Length + album2CoverBytes.Length + cover2LengthBytes.Length];
            song1Bytes.CopyTo(bytes, offset);

            offset += song1Bytes.Length;

            cover1LengthBytes.CopyTo(bytes, offset);
            offset += sizeof(int);

            album1CoverBytes.CopyTo(bytes, offset);
            offset += album1CoverBytes.Length;

            song2Bytes.CopyTo(bytes, offset);
            offset += song2Bytes.Length;

            cover2LengthBytes.CopyTo(bytes, offset);
            offset += sizeof(int);

            album2CoverBytes.CopyTo(bytes, offset);

            // Act
            Utils.PopulateSearchResults(bytes, songs, "");

            if (songs.Count == ExpectedSize)
                rightSize = true;

            if (songs[0].GetName() == "testSong")
                firstSongFound = true;

            if (songs[1].GetName() == "testSong2")
                secondSongFound = true;

            // Assert
            Assert.IsTrue(rightSize & firstSongFound & secondSongFound); // bitwise AND flags
        }

        [TestMethod]
        public void CLIENTSEARCH003_PopulateSearchResults_EmptyResponse_ListRemainsEmpty()
        {
            // Arrange
            const int ExpectedSize = 0;

            List<Song> songs = new();

            byte[] bytes = new byte[0];

            // Act
            Utils.PopulateSearchResults(bytes, songs, "");

            // Assert
            Assert.AreEqual(ExpectedSize, songs.Count); // bitwise AND flags
        }

        [TestMethod]
        public void CLIENTSEARCH004_PopulateSearchResults_NonEmptyResponse_CoversWrittenToFiles()
        {
            // Arrange
            bool firstSongFound = false;
            bool secondSongFound = false;

            List<Song> songs = new();
            SongController songController = new();
            AlbumController albumController = new();

            FileHandler.ReadSongs(songController, "SearchTextFiles/" + Constants.SongsFile);
            FileHandler.ReadAlbums(albumController, "SearchTextFiles/" + Constants.AlbumsFile, "");

            /* Prep for byte array */
            Song song1 = songController.FindSong("testSong");
            Song song2 = songController.FindSong("testSong2");
            Album album1 = albumController.FindAlbum(song1.GetAlbum());
            Album album2 = albumController.FindAlbum(song2.GetAlbum());

            byte[] song1Bytes = song1.Serialize();
            byte[] song2Bytes = song2.Serialize();
            byte[] album1CoverBytes = Utils.GetBitmapBytes(album1.GetImage());
            byte[] cover1LengthBytes = BitConverter.GetBytes(album1CoverBytes.Length);
            byte[] album2CoverBytes = Utils.GetBitmapBytes(album2.GetImage());
            byte[] cover2LengthBytes = BitConverter.GetBytes(album2CoverBytes.Length);

            int offset = 0;
            byte[] bytes = new byte[song1Bytes.Length + song2Bytes.Length + album1CoverBytes.Length + cover1LengthBytes.Length + album2CoverBytes.Length + cover2LengthBytes.Length];
            song1Bytes.CopyTo(bytes, offset);

            offset += song1Bytes.Length;

            cover1LengthBytes.CopyTo(bytes, offset);
            offset += sizeof(int);

            album1CoverBytes.CopyTo(bytes, offset);
            offset += album1CoverBytes.Length;

            song2Bytes.CopyTo(bytes, offset);
            offset += song2Bytes.Length;

            cover2LengthBytes.CopyTo(bytes, offset);
            offset += sizeof(int);

            album2CoverBytes.CopyTo(bytes, offset);

            // Act
            Utils.PopulateSearchResults(bytes, songs, "");

            Bitmap album1Bmp = FileHandler.readImageBytes("testSong.jpg");
            Bitmap album2Bmp = FileHandler.readImageBytes("testSong2.jpg");

            if (Utils.CompareBitmaps(album1Bmp, album1.GetImage()))
                firstSongFound = true;

            if (Utils.CompareBitmaps(album2Bmp, album2.GetImage()))
                secondSongFound = true;

            // Assert
            Assert.IsTrue(firstSongFound & secondSongFound); // bitwise AND flags
        }
    }

    [TestClass]
    public class LogTests
    {

    }
}