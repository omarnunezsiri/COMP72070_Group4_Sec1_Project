/*The System.Drawing namespace has functionalities that are only available on Windows, which leads to
  the CA1416 warning being raised. We have decided to disable it since the Silly Music Player App
  will be made for the Windows platform only. */
#pragma warning disable CA1416 

using System.Drawing;

namespace Server.Tests
{
    [TestClass]
    public class ArtistControllerTests
    {
        private Bitmap _artistImage = (Bitmap)Image.FromFile("default.png");
        private Bitmap _artistImage2 = (Bitmap)Image.FromFile("second.jpg");
        private const string _ARTISTNAME = "deborah george";
        private const string _ARTISTNAME2 = "justin bieber";
        private const string _UNEXISTING = "unexisting";

        [TestMethod]
        public void ARTISTUNIT001_AddArtist_ArtistAdded()
        {
            // Arrange
            ArtistController artistController = new ArtistController();

            // Act
            artistController.AddArtist(_ARTISTNAME, _artistImage);
            Artist artist = artistController.FindArtist(_ARTISTNAME); // assumes that find works

            // Assert
            Assert.IsNotNull(artist);
            Assert.AreEqual(_ARTISTNAME, artist.GetName(), "Name is not as expected");
            Assert.AreEqual(_artistImage, artist.GetImage(), "Image is not as expected");
        }

        [TestMethod]
        public void ARTISTUNIT002_ViewArtists_DictionaryWithArtistsGiven()
        {
            // Arrange
            const int ExpectedSize = 1;
            ArtistController artistController = new();
            artistController.AddArtist(_ARTISTNAME, _artistImage);

            // Act
            Dictionary<string, Artist> artists = artistController.ViewArtists();

            // Assert
            Assert.AreEqual(ExpectedSize, artists.Count);
            Assert.AreEqual(_ARTISTNAME, artists[_ARTISTNAME].GetName());
            Assert.AreEqual(_artistImage, artists[_ARTISTNAME].GetImage());
        }

        [TestMethod]
        public void ARTISTUNIT003_DeleteArtist_ArtistNotInCollection()
        {
            // Arrange
            ArtistController artistController = new();
            artistController.AddArtist(_ARTISTNAME, _artistImage);

            // Act
            artistController.DeleteArtist(_ARTISTNAME);

            // Assert
            Exception ex = Assert.ThrowsException<KeyNotFoundException>(() => artistController.FindArtist(_ARTISTNAME));
            Assert.AreEqual("Artist not found.", ex.Message);
        }

        [TestMethod]
        public void ARTISTUNIT004_UpdateArtist_Name_ArtistUpdated()
        {
            // Arrange
            ArtistController artistsController = new();
            artistsController.AddArtist(_ARTISTNAME, _artistImage);

            // Act
            artistsController.UpdateArtist(_ARTISTNAME, "name", _ARTISTNAME2);

            // Assert
            Assert.IsNotNull(artistsController.FindArtist(_ARTISTNAME2));
        }

        [TestMethod]
        public void ARTISTUNIT005_UpdateArtist_Image_ArtistUpdated()
        {
            // Arrange
            ArtistController artistsController = new();
            artistsController.AddArtist(_ARTISTNAME, _artistImage);

            // Act
            artistsController.UpdateArtist(_ARTISTNAME, "image", _artistImage2);

            // Assert
            Assert.IsTrue(Utils.CompareBitmaps(artistsController.FindArtist(_ARTISTNAME).GetImage(), _artistImage2));
        }

        [TestMethod]
        public void ARTISTUNIT006_AddArtist_ArtistExists_NotAdded()
        {
            // Arrange
            ArtistController artistController = new();
            artistController.AddArtist(_ARTISTNAME, _artistImage);

            // Act
            bool added = artistController.AddArtist(_ARTISTNAME, _artistImage);

            // Assert
            Assert.IsFalse(added);
        }

        [TestMethod]
        public void ARTISTUNIT007_DeleteArtist_ArtistDoesntExist_NothingHappens()
        {
            // Arrange
            ArtistController artistController = new();

            // Act
            bool deleted = artistController.DeleteArtist(_ARTISTNAME);

            // Assert
            Assert.IsFalse(deleted);
        }

        [TestMethod]
        public void ARTISTUNIT008_FindArtist_ArtistReturned()
        {
            // Arrange
            ArtistController artistController = new();
            artistController.AddArtist(_ARTISTNAME, _artistImage);

            // Act
            Artist artist = artistController.FindArtist(_ARTISTNAME);

            // Assert
            Assert.IsNotNull(artist);
            Assert.AreEqual(_ARTISTNAME, artist.GetName(), "Name is not as expected");
            Assert.AreEqual(_artistImage, artist.GetImage(), "Image is not as expected");
        }

        [TestMethod]
        public void ARTISTUNIT009_FindUnexistingArtist_ExceptionThrown()
        {
            // Arrange
            ArtistController artistController = new();

            // Act and Assert
            Exception ex = Assert.ThrowsException<KeyNotFoundException>(() => artistController.FindArtist(_UNEXISTING));
            Assert.AreEqual("Artist not found.", ex.Message);
        }

        [TestMethod]
        public void ARTISTUNIT010_UpdateUnexistingArtist_ThrowsException()
        {
            // Arrange
            ArtistController artistController = new();

            // Act and Assert
            Exception ex = Assert.ThrowsException<KeyNotFoundException>(() => artistController.UpdateArtist(_ARTISTNAME, "name", new object()));
            Assert.AreEqual("Artist not found.", ex.Message);
        }

        [TestMethod]
        public void ARTISTUNIT011_UpdateToCloneHash_ReturnsFalse()
        {
            // Arrange
            ArtistController artistController = new();
            artistController.AddArtist(_ARTISTNAME, _artistImage);
            artistController.AddArtist(_ARTISTNAME2, _artistImage2);

            // Act
            bool updated = artistController.UpdateArtist(_ARTISTNAME, "name", _ARTISTNAME2); // cloning hash

            // Assert
            Assert.IsFalse(updated);
        }
    }

    [TestClass]
    public class AlbumControllerTests
    {
        private Bitmap _image = (Bitmap)Image.FromFile("default.png");
        private Bitmap _anotherImage = (Bitmap)Image.FromFile("second.jpg");
        private const string _ALBUMNAME = "album";
        private const string _ALBUMNAME2 = "album2";
        private const string _ARTISTNAME = "artist";
        private const string _ARTISTNAME2 = "anotherartist";
        
        [TestMethod]
        public void ALBUMUNIT001_AddAlbum_AlbumAdded()
        {
            // Arrange
            AlbumController albumController = new();

            // Act
            albumController.AddAlbum(_ALBUMNAME, _ARTISTNAME, _image);
            Album album = albumController.FindAlbum(_ALBUMNAME);

            // Assert
            Assert.AreEqual(_ALBUMNAME, album.GetName());
            Assert.AreEqual(_ARTISTNAME, album.GetArtist());
            Assert.IsTrue(Utils.CompareBitmaps(_image, album.GetImage()));
        }

        [TestMethod]
        public void ALBUMUNIT002_ViewAlbums_CollectionReturned()
        {
            // Arrange
            const int ExpectedSize = 1;
            AlbumController albumController = new();
            albumController.AddAlbum(_ALBUMNAME, _ARTISTNAME, _image);

            // Act
            var collection = albumController.ViewAlbums();

            // Assert
            Assert.IsTrue(collection.Count == ExpectedSize);
            Assert.AreEqual(_ALBUMNAME, collection[_ALBUMNAME].GetName());
            Assert.AreEqual(_ARTISTNAME, collection[_ALBUMNAME].GetArtist());
            Assert.AreEqual(_image, collection[_ALBUMNAME].GetImage());
        }

        [TestMethod]
        public void ALBUMUNIT003_DeleteAlbum_AlbumNotFound()
        {
            // Arrange
            AlbumController albumController = new();
            albumController.AddAlbum(_ALBUMNAME, _ARTISTNAME, _image);

            // Act
            albumController.DeleteAlbum(_ALBUMNAME);

            // Arrange
            Exception ex = Assert.ThrowsException<KeyNotFoundException>(() => albumController.FindAlbum(_ALBUMNAME));
            Assert.AreEqual("Album not found.", ex.Message);
        }


        [TestMethod]
        public void ALBUMUNIT004_UpdateAlbumName_AlbumUpdated()
        {
            // Arrange
            bool updated = false;
            AlbumController albumController = new();
            albumController.AddAlbum(_ALBUMNAME, _ARTISTNAME, _image);

            // Act
            albumController.UpdateAlbum(_ALBUMNAME, "name", _ALBUMNAME2);

            try
            {
                Album album = albumController.FindAlbum(_ALBUMNAME2); // hash updated
                updated = true;
            }
            catch (Exception)
            {
                throw;
            }

            // Assert
            Assert.IsTrue(updated);
        }

        [TestMethod]
        public void ALBUMUNIT005_UpdateAlbumArtist_AlbumUpdated()
        {
            bool updated = false;
            AlbumController albumController = new();
            albumController.AddAlbum(_ALBUMNAME, _ARTISTNAME, _image);

            // Act
            albumController.UpdateAlbum(_ALBUMNAME, "artist", _ARTISTNAME2);

            Album album = albumController.FindAlbum(_ALBUMNAME);

            if (album.GetArtist() == _ARTISTNAME2)
                updated = true;

            // Assert
            Assert.IsTrue(updated);
        }

        [TestMethod]
        public void ALBUMUNIT006_UpdateAlbumImage_AlbumUpdated()
        {
            // Arrange
            bool updated = false;
            AlbumController albumController = new();
            albumController.AddAlbum(_ALBUMNAME, _ARTISTNAME, _image);

            // Act
            albumController.UpdateAlbum(_ALBUMNAME, "image", _anotherImage);

            Album album = albumController.FindAlbum(_ALBUMNAME);

            updated = Utils.CompareBitmaps(album.GetImage(), _anotherImage);

            // Assert
            Assert.IsTrue(updated);
        }

        [TestMethod]
        public void ALBUMUNIT007_AddExistingAlbum_ReturnsFalse()
        {
            // Arrange
            AlbumController albumController = new();
            albumController.AddAlbum(_ALBUMNAME, _ARTISTNAME, _image);

            // Act
            bool added = albumController.AddAlbum(_ALBUMNAME, _ARTISTNAME, _image);

            // Assert
            Assert.IsFalse(added);
        }

        [TestMethod]
        public void ALBUMUNIT008_DeleteUnexistingAlbum_ReturnsFalse()
        {
            // Arrange
            AlbumController albumController = new();

            // Act
            bool deleted = albumController.DeleteAlbum(_ALBUMNAME);

            // Assert
            Assert.IsFalse(deleted);
        }

        [TestMethod]
        public void ALBUMUNIT009_FindAlbum_AlbumReturned()
        {
            // Arrange
            AlbumController albumController = new();
            albumController.AddAlbum(_ALBUMNAME, _ARTISTNAME, _image);

            // Act
            Album album = albumController.FindAlbum(_ALBUMNAME);

            // Assert
            Assert.AreEqual(_ALBUMNAME, album.GetName());
            Assert.AreEqual(_ARTISTNAME, album.GetArtist());
            Assert.IsTrue(Utils.CompareBitmaps(_image, album.GetImage()));
        }


        [TestMethod]
        public void ALBUMUNIT010_FindUnexistingAlbum_ExceptionThrown()
        {
            // Arrange
            AlbumController albumController = new();

            // Act and Assert
            Exception ex = Assert.ThrowsException<KeyNotFoundException>(() => albumController.FindAlbum(_ALBUMNAME));
            Assert.AreEqual("Album not found.", ex.Message);
        }

        [TestMethod]
        public void ALBUMUNIT011_UpdateUnexistingAlbum_ExceptionThrown()
        {
            // Arrange
            AlbumController albumController = new();

            // Act and Assert
            Exception ex = Assert.ThrowsException<KeyNotFoundException>(() => albumController.UpdateAlbum(_ALBUMNAME, "name", new object()));
            Assert.AreEqual("Album not found.", ex.Message);
        }

        [TestMethod]
        public void ALBUMUNIT012_UpdateHashToDuplicate_ReturnsFalse()
        {
            // Arrange
            AlbumController albumController = new();
            albumController.AddAlbum(_ALBUMNAME, _ARTISTNAME, _image);
            albumController.AddAlbum(_ALBUMNAME2, _ARTISTNAME2, _anotherImage);

            // Act
            bool updated = albumController.UpdateAlbum(_ALBUMNAME, "name", _ALBUMNAME2); // duplicate hash

            // Assert
            Assert.IsFalse(updated);
        }
    }

    [TestClass]
    public class AccountControllerTests
    {
        private const string _USERNAME = "username";
        private const string _PASSWORD = "password";
        private const string _USERNAME2 = "username2";
        private const string _PASSWORD2 = "password2";

        [TestMethod]
        public void ACCUNIT001_AddAccount_AccountAdded()
        {
            // Arrange
            AccountController accountController = new();

            // Act
            accountController.AddAccount(_USERNAME, _PASSWORD);
            Account account = accountController.FindAccount(_USERNAME);

            // Assert
            Assert.AreEqual(_USERNAME, account.getUsername());
            Assert.AreEqual(_PASSWORD, account.getPassword());
        }

        [TestMethod]
        public void ACCUNIT002_ViewAccounts_CollectionReturned()
        {
            // Arrange
            const int ExpectedSize = 1;
            AccountController accountController = new();
            accountController.AddAccount(_USERNAME, _PASSWORD);

            // Act
            var collection = accountController.ViewAccounts();

            // Assert
            Assert.AreEqual(ExpectedSize, collection.Count);
            Assert.AreEqual(_USERNAME, collection[_USERNAME].getUsername());
            Assert.AreEqual(_PASSWORD, collection[_USERNAME].getPassword());
        }

        [TestMethod]
        public void ACCUNIT003_DeleteAccounts_AccountNotFound()
        {
            // Arrange
            AccountController accountController = new();
            accountController.AddAccount(_USERNAME, _PASSWORD);

            // Act
            accountController.DeleteAccount(_USERNAME);

            // Assert
            Exception ex = Assert.ThrowsException<KeyNotFoundException>(() => accountController.FindAccount(_USERNAME));
            Assert.AreEqual("Account not found.", ex.Message);
        }

        [TestMethod]
        public void ACCUNIT004_AccountAuth_Success()
        {
            // Arrange
            AccountController controller = new();
            controller.AddAccount(_USERNAME, _PASSWORD);

            // Act
            bool auth = controller.AuthAccount(_USERNAME, _PASSWORD);

            // Assert
            Assert.IsTrue(auth);
        }

        [TestMethod]
        public void ACCUNIT005_AccountAuth_IncorrectHash_Fail()
        {
            // Arrange
            AccountController controller = new();
            controller.AddAccount(_USERNAME, _PASSWORD);

            // Act
            bool auth = controller.AuthAccount(_USERNAME2, _PASSWORD);

            // Assert
            Assert.IsFalse(auth);
        }

        [TestMethod]
        public void ACCUNIT006_UpdateAccountUsername_AccountUpdated()
        {
            // Arrange
            AccountController accountController = new();
            accountController.AddAccount(_USERNAME, _PASSWORD);

            // Act
            accountController.UpdateAccount(_USERNAME, "username", _USERNAME2);

            // Assert
            Assert.AreEqual(_USERNAME2, accountController.FindAccount(_USERNAME2).getUsername());
        }

        [TestMethod]
        public void ACCUNIT007_UpdateAccountPassword_AccountUpdated()
        {
            // Arrange
            AccountController accountController = new();
            accountController.AddAccount(_USERNAME, _PASSWORD);

            // Act
            accountController.UpdateAccount(_USERNAME, "password", _PASSWORD2);

            // Assert
            Assert.AreEqual(_PASSWORD2, accountController.FindAccount(_USERNAME).getPassword());
        }

        [TestMethod]
        public void ACCUNIT008_AddExistingAccount_ReturnsFalse()
        {
            // Arrange
            AccountController accountController = new();
            accountController.AddAccount(_USERNAME, _PASSWORD);

            // Act
            bool added = accountController.AddAccount(_USERNAME, _PASSWORD);

            // Assert
            Assert.IsFalse(added);
        }

        [TestMethod]
        public void ACCUNIT009_DeleteUnexistingAccount_ReturnsFalse()
        {
            // Arrange
            AccountController accountController = new();

            // Act
            bool deleted = accountController.DeleteAccount(_USERNAME);

            // Assert
            Assert.IsFalse(deleted);
        }

        [TestMethod]
        public void ACCUNIT010_FindAccountInCollection_AccountReturned()
        {
            // Arrange
            AccountController accountController = new();
            accountController.AddAccount(_USERNAME, _PASSWORD);

            // Act
            Account account = accountController.FindAccount(_USERNAME);

            // Assert
            Assert.AreEqual(_USERNAME, account.getUsername());
            Assert.AreEqual(_PASSWORD, account.getPassword());
        }

        [TestMethod]
        public void ACCUNIT011_FindUnexistingAccount_ExceptionThrown()
        {
            // Arrange
            AccountController accountController = new();

            // Act and Assert
            Exception ex = Assert.ThrowsException<KeyNotFoundException>(() => accountController.FindAccount(_USERNAME));
            Assert.AreEqual("Account not found.", ex.Message);
        }

        [TestMethod]
        public void ACCUNIT012_UpdateUnexistingAccount_ExceptionThrown()
        {
            // Arrange
            AccountController accountController = new();

            // Act and Assert
            Exception ex = Assert.ThrowsException<KeyNotFoundException>(() => accountController.UpdateAccount(_USERNAME, "username", new object()));
            Assert.AreEqual("Account not found.", ex.Message);
        }

        [TestMethod]
        public void ACCUNIT013_UpdateHashToDuplicate_ReturnsFalse()
        {
            // Arrange
            AccountController accountController = new();
            accountController.AddAccount(_USERNAME, _PASSWORD);
            accountController.AddAccount(_USERNAME2, _PASSWORD2);

            // Act
            bool updated = accountController.UpdateAccount(_USERNAME, "username", _USERNAME2);

            // Assert
            Assert.IsFalse(updated);
        }

        [TestMethod]
        public void ACCUNIT014_AccountAuth_IncorrectPassword_Fail()
        {
            // Arrange
            AccountController controller = new();
            controller.AddAccount(_USERNAME, _PASSWORD);

            // Act
            bool auth = controller.AuthAccount(_USERNAME, _PASSWORD2);

            // Assert
            Assert.IsFalse(auth);
        }
    }

    [TestClass]
    public class SongControllerTests
    {
        private const string _NAME = "name";
        private const string _ARTIST = "artist";
        private const string _ALBUM = "album";
        private const string _NAME2 = "anotherName";
        private const string _ARTIST2 = "artist2";
        private const string _ALBUM2 = "album2";
        private const float _DURATION = 3.12f;
        private const float _DURATION2 = 4.5f;

        [TestMethod]
        public void SONGUNIT001_AddSong_SongAdded()
        {
            // Arrange
            SongController controller = new();

            // Act
            controller.AddSong(_NAME, _ALBUM, _ARTIST, _DURATION);
            Song song = controller.FindSong(_NAME);

            // Assert
            Assert.AreEqual(_NAME, song.GetName());
            Assert.AreEqual(_ALBUM, song.GetAlbum());
            Assert.AreEqual(_ARTIST, song.GetArtist());
            Assert.AreEqual(_DURATION, song.GetDuration());
        }


        [TestMethod]
        public void SONGUNIT002_ViewSongs_CollectionReturned()
        {
            // Arrange
            const int ExpectedSize = 1;
            SongController controller = new();
            controller.AddSong(_NAME, _ALBUM, _ARTIST, _DURATION);

            // Act
            var collection = controller.ViewSongs();

            // Assert
            Assert.IsTrue(collection.Count == ExpectedSize);
        }

        [TestMethod]
        public void SONGUNIT003_DeleteSong_SongNotFound()
        {
            // Arrange
            SongController controller = new();
            controller.AddSong(_NAME, _ALBUM, _ARTIST, _DURATION);

            // Act
            controller.DeleteSong(_NAME);

            // Assert
            Exception ex = Assert.ThrowsException<KeyNotFoundException>(() => controller.FindSong(_NAME));
            Assert.AreEqual("Song not found.", ex.Message);
        }

        [TestMethod]
        public void SONGUNIT004_UpdateSong_Name_NameUpdated()
        {
            // Arrange
            SongController controller = new();
            controller.AddSong(_NAME, _ALBUM, _ARTIST, _DURATION);

            // Act
            controller.UpdateSong(_NAME, "name", _NAME2);

            // Assert
            Assert.IsNotNull(controller.FindSong(_NAME2));
        }


        [TestMethod]
        public void SONGUNIT005_UpdateSong_Artist_ArtistUpdated()
        {
            // Arrange
            SongController controller = new();
            controller.AddSong(_NAME, _ALBUM, _ARTIST, _DURATION);

            // Act
            controller.UpdateSong(_NAME, "artist", _ARTIST2);

            // Assert
            Assert.AreEqual(_ARTIST2, controller.ViewSongs()[_NAME].GetArtist());
        }


        [TestMethod]
        public void SONGUNIT006_UpdateSong_Duration_DurationUpdated()
        {
            // Arrange
            SongController controller = new();
            controller.AddSong(_NAME, _ALBUM, _ARTIST, _DURATION);

            // Act
            controller.UpdateSong(_NAME, "duration", _DURATION2);

            // Assert
            Assert.AreEqual(_DURATION2, controller.ViewSongs()[_NAME].GetDuration());
        }

        [TestMethod]
        public void SONGUNIT007_UpdateSong_Album_AlbumUpdated()
        {
            // Arrange
            SongController controller = new();
            controller.AddSong(_NAME, _ALBUM, _ARTIST, _DURATION);

            // Act
            controller.UpdateSong(_NAME, "album", _ALBUM2);

            // Assert
            Assert.AreEqual(_ALBUM2, controller.ViewSongs()[_NAME].GetAlbum());
        }

        [TestMethod]
        public void SONGUNIT008_AddExistingSong_ReturnsFalse()
        {
            // Arrange
            SongController controller = new();
            controller.AddSong(_NAME, _ALBUM, _ARTIST, _DURATION);

            // Act
            bool added = controller.AddSong(_NAME, _ALBUM, _ARTIST, _DURATION);

            // Assert
            Assert.IsFalse(added);
        }

        [TestMethod]
        public void SONGUNIT009_DeleteUnexistingSong_ReturnsFalse()
        {
            // Arrange
            SongController controller = new();

            // Act
            bool deleted = controller.DeleteSong(_NAME);

            // Assert
            Assert.IsFalse(deleted);
        }

        [TestMethod]
        public void SONGUNIT010_FindSong_SongReturned()
        {
            // Arrange
            SongController controller = new();
            controller.AddSong(_NAME, _ALBUM, _ARTIST, _DURATION);

            // Act
            Song song = controller.FindSong(_NAME);

            // Assert
            Assert.AreEqual(_NAME, song.GetName());
            Assert.AreEqual(_ALBUM, song.GetAlbum());
            Assert.AreEqual(_ARTIST, song.GetArtist());
            Assert.AreEqual(_DURATION, song.GetDuration());
        }

        [TestMethod]
        public void SONGUNIT011_FindUnexistingSong_ExceptionThrown()
        {
            // Arrange
            SongController controller = new();

            // Act and Assert
            Exception ex = Assert.ThrowsException<KeyNotFoundException>(() => controller.FindSong(_NAME));
            Assert.AreEqual("Song not found.", ex.Message);
        }

        [TestMethod]
        public void SONGUNIT012_UpdateUnexistingSong_ExceptionThrown()
        {
            // Arrange
            SongController controller = new();

            // Act and Assert
            Exception ex = Assert.ThrowsException<KeyNotFoundException>(() => controller.UpdateSong(_NAME, "name", new object()));
            Assert.AreEqual("Song not found.", ex.Message);
        }

        [TestMethod]
        public void SONGUNIT013_UpdateHashToDuplicate_ReturnsFalse()
        {
            // Arrange
            SongController controller = new();
            controller.AddSong(_NAME, _ALBUM, _ARTIST, _DURATION);
            controller.AddSong(_NAME2, _ALBUM2, _ARTIST2, _DURATION2);

            // Act
            bool updated = controller.UpdateSong(_NAME, "name", _NAME2); // duplicate hash

            // Assert
            Assert.IsFalse(updated);
        }
    }

    [TestClass]
    public class FileHandlerTests
    {
        /* Account members */
        private const string _USERNAME = "username";
        private const string _PASSWORD = "password";
        private const string _USERNAME2 = "username2";

        /* Song members*/
        private const string _SONGNAME = "song";
        private const string _SONGNAME2 = "song2";
        private const float _DURATION = 3.12f;

        /* Album members*/
        private const string _ALBUMNAME = "album";
        private const string _ALBUMNAME2 = "album2";
        private Bitmap _albumImg = (Bitmap)Image.FromFile("album.png");
        private Bitmap _albumImg2 = (Bitmap)Image.FromFile("album2.png");

        /* Artist members*/
        private const string _ARTISTNAME = "deborah";
        private const string _ARTISTNAME2 = "justin";
        private Bitmap _image = (Bitmap)Image.FromFile("deborah.png");
        private Bitmap _image2 = (Bitmap)Image.FromFile("justin.jpeg");

        [TestMethod]
        public void FHUNIT001_WriteSongs_CollectionInDataFile()
        {
            // Arrange
            string[] Expected = new string[] {"song,album,deborah,3.12", "song2,album,deborah,3.12"};

            // Act
            SongController songController = new();
            songController.AddSong(_SONGNAME, _ALBUMNAME, _ARTISTNAME, _DURATION);
            songController.AddSong(_SONGNAME2, _ALBUMNAME, _ARTISTNAME, _DURATION);

            FileHandler.WriteSongs(songController, Constants.SongsFile);
            string[] actual = File.ReadAllLines(Constants.SongsFile);

            // Assert
            Assert.IsTrue(Expected.SequenceEqual(actual));
        }

        [TestMethod]
        public void FHUNIT002_WriteAccounts_CollectionInDataFile()
        {
            // Arrange
            string[] Expected = new string[] {"username,password", "username2,password"};

            AccountController accountController = new AccountController();
            accountController.AddAccount(_USERNAME, _PASSWORD);
            accountController.AddAccount(_USERNAME2, _PASSWORD);

            // Act
            FileHandler.WriteAccounts(accountController, Constants.AccountsFile);
            string[] actual = File.ReadAllLines(Constants.AccountsFile);

            // Assert
            Assert.IsTrue(Expected.SequenceEqual(actual));
        }

        [TestMethod]
        public void FHUNIT003_WriteAlbums_CollectionInDataFile()
        {
            // Arrange
            string[] Expected = new string[] {"Png,album,deborah", "Png,album2,deborah"};

            AlbumController albumController = new();
            albumController.AddAlbum(_ALBUMNAME, _ARTISTNAME, _image);
            albumController.AddAlbum(_ALBUMNAME2, _ARTISTNAME, _image);

            // Act
            FileHandler.WriteAlbums(albumController, Constants.AlbumsFile);
            string[] actual = File.ReadAllLines(Constants.AlbumsFile);

            // Assert
            Assert.IsTrue(Expected.SequenceEqual(actual));
        }

        [TestMethod]
        public void FHUNIT004_WriteArtists_CollectionInDataFile()
        {
            // Arrange
            string[] Expected = new string[] {"Png,deborah", "Jpeg,justin"};

            ArtistController artistController = new();
            artistController.AddArtist(_ARTISTNAME, _image);
            artistController.AddArtist(_ARTISTNAME2, _image2);

            // Act
            FileHandler.WriteArtists(artistController, Constants.ArtistsFile);
            string[] actual = File.ReadAllLines(Constants.ArtistsFile);

            // Assert
            Assert.IsTrue(Expected.SequenceEqual(actual));
        }

        [TestMethod]
        public void FHUNIT005_ReadSongs_ControllerPopulated()
        {
            // Arrange
            bool parsedFirst = false;
            bool parsedSecond = false;

            string[] data = new string[] { "song,album,deborah,3.12", "song2,album,deborah,3.12" };
            File.WriteAllLines(Constants.SongsFile, data);

            // Act
            SongController songController = new();
            FileHandler.ReadSongs(songController, Constants.SongsFile);

            Song song1 = songController.FindSong(_SONGNAME);
            Song song2 = songController.FindSong(_SONGNAME2);

            if (song1.GetAlbum() == _ALBUMNAME && song1.GetArtist() == _ARTISTNAME && song1.GetDuration() == _DURATION)
                parsedFirst = true;

            if (song2.GetAlbum() == _ALBUMNAME && song2.GetArtist() == _ARTISTNAME && song2.GetDuration() == _DURATION)
                parsedSecond = true;

            // Assert
            Assert.IsTrue(parsedFirst & parsedSecond); // bitwise AND flags
        }


        [TestMethod]
        public void FHUNIT006_ReadAccounts_ControllerPopulated()
        {
            // Arrange
            bool parsedFirst = false;
            bool parsedSecond = false;

            string[] data = new string[] { "username,password", "username2,password" };
            File.WriteAllLines(Constants.AccountsFile, data);

            // Act
            AccountController accountController = new();
            FileHandler.ReadAccounts(accountController, Constants.AccountsFile);

            Account account1 = accountController.FindAccount(_USERNAME);
            Account account2 = accountController.FindAccount(_USERNAME2);

            if (account1.getPassword() == _PASSWORD)
                parsedFirst = true;

            if(account2.getPassword() == _PASSWORD)
                parsedSecond = true;

            // Assert
            Assert.IsTrue(parsedFirst & parsedSecond); // bitwise AND flags 
        }


        [TestMethod]
        public void FHUNIT007_ReadAlbums_ControllerPopulated()
        {
            // Arrange
            bool parsedFirst = false;
            bool parsedSecond = false;

            string[] data = new string[] { "Png,album,deborah", "Png,album2,deborah" };
            File.WriteAllLines(Constants.AlbumsFile, data);

            // Act
            AlbumController albumController = new();
            FileHandler.ReadAlbums(albumController, Constants.AlbumsFile);

            Album album1 = albumController.FindAlbum(_ALBUMNAME);
            Album album2 = albumController.FindAlbum(_ALBUMNAME2);

            if (album1.GetArtist() == _ARTISTNAME && Utils.CompareBitmaps(_albumImg, album1.GetImage()))
                parsedFirst = true;

            if (album2.GetArtist() == _ARTISTNAME && Utils.CompareBitmaps(_albumImg2, album2.GetImage()))
                parsedSecond = true;

            // Assert
            Assert.IsTrue(parsedFirst & parsedSecond); // bitwise AND flags
        }

        [TestMethod]
        public void FHUNIT008_ReadArtists_ControllerPopulated()
        {
            // Arrange
            bool parsedFirst = false;
            bool parsedSecond = false;

            string[] data = new string[] { "Png,deborah", "Jpeg,justin" };
            File.WriteAllLines(Constants.ArtistsFile, data);

            // Act
            ArtistController artistController = new();
            FileHandler.ReadArtists(artistController, Constants.ArtistsFile);

            Artist artist1 = artistController.FindArtist(_ARTISTNAME);
            Artist artist2 = artistController.FindArtist(_ARTISTNAME2);

            if (Utils.CompareBitmaps(artist1.GetImage(), _image))
                parsedFirst = true;

            if (Utils.CompareBitmaps(artist2.GetImage(), _image2))
                parsedSecond = true;
            
            // Assert
            Assert.IsTrue(parsedFirst & parsedSecond); // bitwise AND flags
        }

        [TestMethod]
        public void FHUNIT009_ReadAccounts_FileDoesntExist_ExceptionThrown()
        {
            // Arrange
            File.Delete(Constants.AccountsFile); // ensures that the file doesn't exist

            AccountController controller = new();

            // Act and Assert
            Assert.ThrowsException<FileNotFoundException>(() => FileHandler.ReadAccounts(controller, Constants.AccountsFile));
        }

        [TestMethod]
        public void FHUNIT010_ReadSongs_FileDoesntExist_ExceptionThrown()
        {
            // Arrange
            File.Delete(Constants.SongsFile); // ensures that the file doesn't exist

            SongController controller = new();

            // Act and Assert
            Assert.ThrowsException<FileNotFoundException>(() => FileHandler.ReadSongs(controller, Constants.SongsFile));
        }

        [TestMethod]
        public void FHUNIT011_ReadArtists_FileDoesntExist_ExceptionThrown()
        {
            // Arrange
            File.Delete(Constants.ArtistsFile); // ensures that the file doesn't exist

            ArtistController controller = new();

            // Act and Assert
            Assert.ThrowsException<FileNotFoundException>(() => FileHandler.ReadArtists(controller, Constants.ArtistsFile));
        }


        [TestMethod]
        public void FHUNIT012_ReadAlbums_FileDoesntExist_ExceptionThrown()
        {
            // Arrange
            File.Delete(Constants.AlbumsFile); // ensures that the file doesn't exist

            AlbumController controller = new();

            // Act and Assert
            Assert.ThrowsException<FileNotFoundException>(() => FileHandler.ReadAlbums(controller, Constants.AlbumsFile));
        }
    }
}