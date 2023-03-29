/*The System.Drawing namespace has functionalities that are only available on Windows, which leads to
  the CA1416 warning being raised. We have decided to disable it since the Silly Music Player App
  will be made for the Windows platform only. */
#pragma warning disable CA1416 

using System.Diagnostics;
using System.Drawing;

namespace Server.Tests
{
    [TestClass]
    public class ArtistControllerTests
    {
        private Bitmap _artistImage = (Bitmap)Image.FromFile("default.png");
        private Bitmap _artistImage2 = (Bitmap)Image.FromFile("second.jpg");
        private const string _ArtistName = "deborah george";
        private const string _ArtistName2 = "justin bieber";
        private const string _UnexistingHash = "unexisting";

        [TestMethod]
        public void ARTISTUNIT001_AddArtist_ArtistAdded()
        {
            // Arrange
            ArtistController artistController = new ArtistController();

            // Act
            artistController.AddArtist(_ArtistName, _artistImage);
            Artist artist = artistController.FindArtist(_ArtistName); // assumes that find works

            // Assert
            Assert.IsNotNull(artist);
            Assert.AreEqual(_ArtistName, artist.GetName(), "Name is not as expected");
            Assert.AreEqual(_artistImage, artist.GetImage(), "Image is not as expected");
        }

        [TestMethod]
        public void ARTISTUNIT002_ViewArtists_DictionaryWithArtistsGiven()
        {
            // Arrange
            const int ExpectedSize = 1;
            ArtistController artistController = new();
            artistController.AddArtist(_ArtistName, _artistImage);

            // Act
            Dictionary<string, Artist> artists = artistController.ViewArtists();

            // Assert
            Assert.AreEqual(ExpectedSize, artists.Count);
            Assert.AreEqual(_ArtistName, artists[_ArtistName].GetName());
            Assert.AreEqual(_artistImage, artists[_ArtistName].GetImage());
        }

        [TestMethod]
        public void ARTISTUNIT003_DeleteArtist_ArtistNotInCollection()
        {
            // Arrange
            ArtistController artistController = new();
            artistController.AddArtist(_ArtistName, _artistImage);

            // Act
            artistController.DeleteArtist(_ArtistName);

            // Assert
            Exception ex = Assert.ThrowsException<KeyNotFoundException>(() => artistController.FindArtist(_ArtistName));
            Assert.AreEqual("Artist not found.", ex.Message);
        }

        [TestMethod]
        public void ARTISTUNIT004_UpdateArtist_Name_ArtistUpdated()
        {
            // Arrange
            ArtistController artistsController = new();
            artistsController.AddArtist(_ArtistName, _artistImage);

            // Act
            artistsController.UpdateArtist(_ArtistName, "name", _ArtistName2);

            // Assert
            Assert.IsNotNull(artistsController.FindArtist(_ArtistName2));
        }

        [TestMethod]
        public void ARTISTUNIT005_UpdateArtist_Image_ArtistUpdated()
        {
            // Arrange
            ArtistController artistsController = new();
            artistsController.AddArtist(_ArtistName, _artistImage);

            // Act
            artistsController.UpdateArtist(_ArtistName, "image", _artistImage2);

            // Assert
            Assert.IsTrue(Utils.CompareBitmaps(artistsController.FindArtist(_ArtistName).GetImage(), _artistImage2));
        }

        [TestMethod]
        public void ARTISTUNIT006_AddArtist_ArtistExists_NotAdded()
        {
            // Arrange
            ArtistController artistController = new();
            artistController.AddArtist(_ArtistName, _artistImage);

            // Act
            bool added = artistController.AddArtist(_ArtistName, _artistImage);

            // Assert
            Assert.IsFalse(added);
        }

        [TestMethod]
        public void ARTISTUNIT007_DeleteArtist_ArtistDoesntExist_NothingHappens()
        {
            // Arrange
            ArtistController artistController = new();

            // Act
            bool deleted = artistController.DeleteArtist(_ArtistName);

            // Assert
            Assert.IsFalse(deleted);
        }

        [TestMethod]
        public void ARTISTUNIT008_FindArtist_ArtistReturned()
        {
            // Arrange
            ArtistController artistController = new();
            artistController.AddArtist(_ArtistName, _artistImage);

            // Act
            Artist artist = artistController.FindArtist(_ArtistName);

            // Assert
            Assert.IsNotNull(artist);
            Assert.AreEqual(_ArtistName, artist.GetName(), "Name is not as expected");
            Assert.AreEqual(_artistImage, artist.GetImage(), "Image is not as expected");
        }

        [TestMethod]
        public void ARTISTUNIT009_FindUnexistingArtist_ExceptionThrown()
        {
            // Arrange
            ArtistController artistController = new();

            // Act and Assert
            Exception ex = Assert.ThrowsException<KeyNotFoundException>(() => artistController.FindArtist(_UnexistingHash));
            Assert.AreEqual("Artist not found.", ex.Message);
        }

        [TestMethod]
        public void ARTISTUNIT010_UpdateUnexistingArtist_ThrowsException()
        {
            // Arrange
            ArtistController artistController = new();

            // Act and Assert
            Exception ex = Assert.ThrowsException<KeyNotFoundException>(() => artistController.UpdateArtist(_ArtistName, "name", new object()));
            Assert.AreEqual("Artist not found.", ex.Message);
        }

        [TestMethod]
        public void ARTISTUNIT011_UpdateToCloneHash_ReturnsFalse()
        {
            // Arrange
            ArtistController artistController = new();
            artistController.AddArtist(_ArtistName, _artistImage);
            artistController.AddArtist(_ArtistName2, _artistImage2);

            // Act
            bool updated = artistController.UpdateArtist(_ArtistName, "name", _ArtistName2); // cloning hash

            // Assert
            Assert.IsFalse(updated);
        }
    }

    [TestClass]
    public class AlbumControllerTests
    {
        private Bitmap _image = (Bitmap)Image.FromFile("default.png");
        private Bitmap _anotherImage = (Bitmap)Image.FromFile("second.jpg");
        private const string _AlbumName = "album";
        private const string _AlbumName2 = "album2";
        private const string _ArtistName = "artist";
        private const string _ArtistName2 = "anotherartist";
        
        [TestMethod]
        public void ALBUMUNIT001_AddAlbum_AlbumAdded()
        {
            // Arrange
            AlbumController albumController = new();

            // Act
            albumController.AddAlbum(_AlbumName, _ArtistName, _image);
            Album album = albumController.FindAlbum(_AlbumName);

            // Assert
            Assert.AreEqual(_AlbumName, album.GetName());
            Assert.AreEqual(_ArtistName, album.GetArtist());
            Assert.IsTrue(Utils.CompareBitmaps(_image, album.GetImage()));
        }

        [TestMethod]
        public void ALBUMUNIT002_ViewAlbums_CollectionReturned()
        {
            // Arrange
            const int ExpectedSize = 1;
            AlbumController albumController = new();
            albumController.AddAlbum(_AlbumName, _ArtistName, _image);

            // Act
            var collection = albumController.ViewAlbums();

            // Assert
            Assert.IsTrue(collection.Count == ExpectedSize);
            Assert.AreEqual(_AlbumName, collection[_AlbumName].GetName());
            Assert.AreEqual(_ArtistName, collection[_AlbumName].GetArtist());
            Assert.AreEqual(_image, collection[_AlbumName].GetImage());
        }

        [TestMethod]
        public void ALBUMUNIT003_DeleteAlbum_AlbumNotFound()
        {
            // Arrange
            AlbumController albumController = new();
            albumController.AddAlbum(_AlbumName, _ArtistName, _image);

            // Act
            albumController.DeleteAlbum(_AlbumName);

            // Arrange
            Exception ex = Assert.ThrowsException<KeyNotFoundException>(() => albumController.FindAlbum(_AlbumName));
            Assert.AreEqual("Album not found.", ex.Message);
        }


        [TestMethod]
        public void ALBUMUNIT004_UpdateAlbumName_AlbumUpdated()
        {
            // Arrange
            bool updated = false;
            AlbumController albumController = new();
            albumController.AddAlbum(_AlbumName, _ArtistName, _image);

            // Act
            albumController.UpdateAlbum(_AlbumName, "name", _AlbumName2);

            try
            {
                Album album = albumController.FindAlbum(_AlbumName2); // hash updated
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
            albumController.AddAlbum(_AlbumName, _ArtistName, _image);

            // Act
            albumController.UpdateAlbum(_AlbumName, "artist", _ArtistName2);

            Album album = albumController.FindAlbum(_AlbumName);

            if (album.GetArtist() == _ArtistName2)
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
            albumController.AddAlbum(_AlbumName, _ArtistName, _image);

            // Act
            albumController.UpdateAlbum(_AlbumName, "image", _anotherImage);

            Album album = albumController.FindAlbum(_AlbumName);

            updated = Utils.CompareBitmaps(album.GetImage(), _anotherImage);

            // Assert
            Assert.IsTrue(updated);
        }

        [TestMethod]
        public void ALBUMUNIT007_AddExistingAlbum_ReturnsFalse()
        {
            // Arrange
            AlbumController albumController = new();
            albumController.AddAlbum(_AlbumName, _ArtistName, _image);

            // Act
            bool added = albumController.AddAlbum(_AlbumName, _ArtistName, _image);

            // Assert
            Assert.IsFalse(added);
        }

        [TestMethod]
        public void ALBUMUNIT008_DeleteUnexistingAlbum_ReturnsFalse()
        {
            // Arrange
            AlbumController albumController = new();

            // Act
            bool deleted = albumController.DeleteAlbum(_AlbumName);

            // Assert
            Assert.IsFalse(deleted);
        }

        [TestMethod]
        public void ALBUMUNIT009_FindAlbum_AlbumReturned()
        {
            // Arrange
            AlbumController albumController = new();
            albumController.AddAlbum(_AlbumName, _ArtistName, _image);

            // Act
            Album album = albumController.FindAlbum(_AlbumName);

            // Assert
            Assert.AreEqual(_AlbumName, album.GetName());
            Assert.AreEqual(_ArtistName, album.GetArtist());
            Assert.IsTrue(Utils.CompareBitmaps(_image, album.GetImage()));
        }


        [TestMethod]
        public void ALBUMUNIT010_FindUnexistingAlbum_ExceptionThrown()
        {
            // Arrange
            AlbumController albumController = new();

            // Act and Assert
            Exception ex = Assert.ThrowsException<KeyNotFoundException>(() => albumController.FindAlbum(_AlbumName));
            Assert.AreEqual("Album not found.", ex.Message);
        }

        [TestMethod]
        public void ALBUMUNIT011_UpdateUnexistingAlbum_ExceptionThrown()
        {
            // Arrange
            AlbumController albumController = new();

            // Act and Assert
            Exception ex = Assert.ThrowsException<KeyNotFoundException>(() => albumController.UpdateAlbum(_AlbumName, "name", new object()));
            Assert.AreEqual("Album not found.", ex.Message);
        }

        [TestMethod]
        public void ALBUMUNIT012_UpdateHashToDuplicate_ReturnsFalse()
        {
            // Arrange
            AlbumController albumController = new();
            albumController.AddAlbum(_AlbumName, _ArtistName, _image);
            albumController.AddAlbum(_AlbumName2, _ArtistName2, _anotherImage);

            // Act
            bool updated = albumController.UpdateAlbum(_AlbumName, "name", _AlbumName2); // duplicate hash

            // Assert
            Assert.IsFalse(updated);
        }
    }

    [TestClass]
    public class AccountControllerTests
    {
        private const string _Username = "username";
        private const string _Password = "password";
        private const string _Username2 = "username2";
        private const string _Password2 = "password2";

        [TestMethod]
        public void ACCUNIT001_AddAccount_AccountAdded()
        {
            // Arrange
            AccountController accountController = new();

            // Act
            accountController.AddAccount(_Username, _Password);
            Account account = accountController.FindAccount(_Username);

            // Assert
            Assert.AreEqual(_Username, account.getUsername());
            Assert.AreEqual(_Password, account.getPassword());
        }

        [TestMethod]
        public void ACCUNIT002_ViewAccounts_CollectionReturned()
        {
            // Arrange
            const int ExpectedSize = 1;
            AccountController accountController = new();
            accountController.AddAccount(_Username, _Password);

            // Act
            var collection = accountController.ViewAccounts();

            // Assert
            Assert.AreEqual(ExpectedSize, collection.Count);
            Assert.AreEqual(_Username, collection[_Username].getUsername());
            Assert.AreEqual(_Password, collection[_Username].getPassword());
        }

        [TestMethod]
        public void ACCUNIT003_DeleteAccounts_AccountNotFound()
        {
            // Arrange
            AccountController accountController = new();
            accountController.AddAccount(_Username, _Password);

            // Act
            accountController.DeleteAccount(_Username);

            // Assert
            Exception ex = Assert.ThrowsException<KeyNotFoundException>(() => accountController.FindAccount(_Username));
            Assert.AreEqual("Account not found.", ex.Message);
        }

        [TestMethod]
        public void ACCUNIT004_AccountAuth_Success()
        {
            // Arrange
            AccountController controller = new();
            controller.AddAccount(_Username, _Password);

            // Act
            bool auth = controller.AuthAccount(_Username, _Password);

            // Assert
            Assert.IsTrue(auth);
        }

        [TestMethod]
        public void ACCUNIT005_AccountAuth_IncorrectHash_Fail()
        {
            // Arrange
            AccountController controller = new();
            controller.AddAccount(_Username, _Password);

            // Act
            bool auth = controller.AuthAccount(_Username2, _Password);

            // Assert
            Assert.IsFalse(auth);
        }

        [TestMethod]
        public void ACCUNIT006_UpdateAccountUsername_AccountUpdated()
        {
            // Arrange
            AccountController accountController = new();
            accountController.AddAccount(_Username, _Password);

            // Act
            accountController.UpdateAccount(_Username, "username", _Username2);

            // Assert
            Assert.AreEqual(_Username2, accountController.FindAccount(_Username2).getUsername());
        }

        [TestMethod]
        public void ACCUNIT007_UpdateAccountPassword_AccountUpdated()
        {
            // Arrange
            AccountController accountController = new();
            accountController.AddAccount(_Username, _Password);

            // Act
            accountController.UpdateAccount(_Username, "password", _Password2);

            // Assert
            Assert.AreEqual(_Password2, accountController.FindAccount(_Username).getPassword());
        }

        [TestMethod]
        public void ACCUNIT008_AddExistingAccount_ReturnsFalse()
        {
            // Arrange
            AccountController accountController = new();
            accountController.AddAccount(_Username, _Password);

            // Act
            bool added = accountController.AddAccount(_Username, _Password);

            // Assert
            Assert.IsFalse(added);
        }

        [TestMethod]
        public void ACCUNIT009_DeleteUnexistingAccount_ReturnsFalse()
        {
            // Arrange
            AccountController accountController = new();

            // Act
            bool deleted = accountController.DeleteAccount(_Username);

            // Assert
            Assert.IsFalse(deleted);
        }

        [TestMethod]
        public void ACCUNIT010_FindAccountInCollection_AccountReturned()
        {
            // Arrange
            AccountController accountController = new();
            accountController.AddAccount(_Username, _Password);

            // Act
            Account account = accountController.FindAccount(_Username);

            // Assert
            Assert.AreEqual(_Username, account.getUsername());
            Assert.AreEqual(_Password, account.getPassword());
        }

        [TestMethod]
        public void ACCUNIT011_FindUnexistingAccount_ExceptionThrown()
        {
            // Arrange
            AccountController accountController = new();

            // Act and Assert
            Exception ex = Assert.ThrowsException<KeyNotFoundException>(() => accountController.FindAccount(_Username));
            Assert.AreEqual("Account not found.", ex.Message);
        }

        [TestMethod]
        public void ACCUNIT012_UpdateUnexistingAccount_ExceptionThrown()
        {
            // Arrange
            AccountController accountController = new();

            // Act and Assert
            Exception ex = Assert.ThrowsException<KeyNotFoundException>(() => accountController.UpdateAccount(_Username, "username", new object()));
            Assert.AreEqual("Account not found.", ex.Message);
        }

        [TestMethod]
        public void ACCUNIT013_UpdateHashToDuplicate_ReturnsFalse()
        {
            // Arrange
            AccountController accountController = new();
            accountController.AddAccount(_Username, _Password);
            accountController.AddAccount(_Username2, _Password2);

            // Act
            bool updated = accountController.UpdateAccount(_Username, "username", _Username2);

            // Assert
            Assert.IsFalse(updated);
        }

        [TestMethod]
        public void ACCUNIT014_AccountAuth_IncorrectPassword_Fail()
        {
            // Arrange
            AccountController controller = new();
            controller.AddAccount(_Username, _Password);

            // Act
            bool auth = controller.AuthAccount(_Username, _Password2);

            // Assert
            Assert.IsFalse(auth);
        }
    }

    [TestClass]
    public class SongControllerTests
    {
        private const string _SongName = "name";
        private const string _ArtistName = "artist";
        private const string _AlbumName = "album";
        private const string _SongName2 = "anotherName";
        private const string _ArtistName2 = "artist2";
        private const string _AlbumName2 = "album2";
        private const float _Duration = 3.12f;
        private const float _Duration2 = 4.5f;

        [TestMethod]
        public void SONGUNIT001_AddSong_SongAdded()
        {
            // Arrange
            SongController controller = new();

            // Act
            controller.AddSong(_SongName, _AlbumName, _ArtistName, _Duration);
            Song song = controller.FindSong(_SongName);

            // Assert
            Assert.AreEqual(_SongName, song.GetName());
            Assert.AreEqual(_AlbumName, song.GetAlbum());
            Assert.AreEqual(_ArtistName, song.GetArtist());
            Assert.AreEqual(_Duration, song.GetDuration());
        }


        [TestMethod]
        public void SONGUNIT002_ViewSongs_CollectionReturned()
        {
            // Arrange
            const int ExpectedSize = 1;
            SongController controller = new();
            controller.AddSong(_SongName, _AlbumName, _ArtistName, _Duration);

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
            controller.AddSong(_SongName, _AlbumName, _ArtistName, _Duration);

            // Act
            controller.DeleteSong(_SongName);

            // Assert
            Exception ex = Assert.ThrowsException<KeyNotFoundException>(() => controller.FindSong(_SongName));
            Assert.AreEqual("Song not found.", ex.Message);
        }

        [TestMethod]
        public void SONGUNIT004_UpdateSong_Name_NameUpdated()
        {
            // Arrange
            SongController controller = new();
            controller.AddSong(_SongName, _AlbumName, _ArtistName, _Duration);

            // Act
            controller.UpdateSong(_SongName, "name", _SongName2);

            // Assert
            Assert.IsNotNull(controller.FindSong(_SongName2));
        }


        [TestMethod]
        public void SONGUNIT005_UpdateSong_Artist_ArtistUpdated()
        {
            // Arrange
            SongController controller = new();
            controller.AddSong(_SongName, _AlbumName, _ArtistName, _Duration);

            // Act
            controller.UpdateSong(_SongName, "artist", _ArtistName2);

            // Assert
            Assert.AreEqual(_ArtistName2, controller.ViewSongs()[_SongName].GetArtist());
        }


        [TestMethod]
        public void SONGUNIT006_UpdateSong_Duration_DurationUpdated()
        {
            // Arrange
            SongController controller = new();
            controller.AddSong(_SongName, _AlbumName, _ArtistName, _Duration);

            // Act
            controller.UpdateSong(_SongName, "duration", _Duration2);

            // Assert
            Assert.AreEqual(_Duration2, controller.ViewSongs()[_SongName].GetDuration());
        }

        [TestMethod]
        public void SONGUNIT007_UpdateSong_Album_AlbumUpdated()
        {
            // Arrange
            SongController controller = new();
            controller.AddSong(_SongName, _AlbumName, _ArtistName, _Duration);

            // Act
            controller.UpdateSong(_SongName, "album", _AlbumName2);

            // Assert
            Assert.AreEqual(_AlbumName2, controller.ViewSongs()[_SongName].GetAlbum());
        }

        [TestMethod]
        public void SONGUNIT008_AddExistingSong_ReturnsFalse()
        {
            // Arrange
            SongController controller = new();
            controller.AddSong(_SongName, _AlbumName, _ArtistName, _Duration);

            // Act
            bool added = controller.AddSong(_SongName, _AlbumName, _ArtistName, _Duration);

            // Assert
            Assert.IsFalse(added);
        }

        [TestMethod]
        public void SONGUNIT009_DeleteUnexistingSong_ReturnsFalse()
        {
            // Arrange
            SongController controller = new();

            // Act
            bool deleted = controller.DeleteSong(_SongName);

            // Assert
            Assert.IsFalse(deleted);
        }

        [TestMethod]
        public void SONGUNIT010_FindSong_SongReturned()
        {
            // Arrange
            SongController controller = new();
            controller.AddSong(_SongName, _AlbumName, _ArtistName, _Duration);

            // Act
            Song song = controller.FindSong(_SongName);

            // Assert
            Assert.AreEqual(_SongName, song.GetName());
            Assert.AreEqual(_AlbumName, song.GetAlbum());
            Assert.AreEqual(_ArtistName, song.GetArtist());
            Assert.AreEqual(_Duration, song.GetDuration());
        }

        [TestMethod]
        public void SONGUNIT011_FindUnexistingSong_ExceptionThrown()
        {
            // Arrange
            SongController controller = new();

            // Act and Assert
            Exception ex = Assert.ThrowsException<KeyNotFoundException>(() => controller.FindSong(_SongName));
            Assert.AreEqual("Song not found.", ex.Message);
        }

        [TestMethod]
        public void SONGUNIT012_UpdateUnexistingSong_ExceptionThrown()
        {
            // Arrange
            SongController controller = new();

            // Act and Assert
            Exception ex = Assert.ThrowsException<KeyNotFoundException>(() => controller.UpdateSong(_SongName, "name", new object()));
            Assert.AreEqual("Song not found.", ex.Message);
        }

        [TestMethod]
        public void SONGUNIT013_UpdateHashToDuplicate_ReturnsFalse()
        {
            // Arrange
            SongController controller = new();
            controller.AddSong(_SongName, _AlbumName, _ArtistName, _Duration);
            controller.AddSong(_SongName2, _AlbumName2, _ArtistName2, _Duration2);

            // Act
            bool updated = controller.UpdateSong(_SongName, "name", _SongName2); // duplicate hash

            // Assert
            Assert.IsFalse(updated);
        }
    }

    [TestClass]
    public class FileHandlerTests
    {
        /* Account members */
        private const string _Username = "username";
        private const string _Password = "password";
        private const string _Username2 = "username2";

        /* Song members*/
        private const string _SongName = "song";
        private const string _SongName2 = "song2";
        private const float _Duration = 3.12f;

        /* Album members*/
        private const string _AlbumName = "album";
        private const string _AlbumName2 = "album2";
        private Bitmap _albumImg = (Bitmap)Image.FromFile("album.png");
        private Bitmap _albumImg2 = (Bitmap)Image.FromFile("album2.png");

        /* Artist members*/
        private const string _ArtistName = "deborah";
        private const string _ArtistName2 = "justin";
        private Bitmap _artistImage = (Bitmap)Image.FromFile("deborah.png");
        private Bitmap _artistImage2 = (Bitmap)Image.FromFile("justin.jpeg");

        [TestMethod]
        public void FHUNIT001_WriteSongs_CollectionInDataFile()
        {
            // Arrange
            string[] Expected = new string[] {"song,album,deborah,3.12", "song2,album,deborah,3.12"};

            // Act
            SongController songController = new();
            songController.AddSong(_SongName, _AlbumName, _ArtistName, _Duration);
            songController.AddSong(_SongName2, _AlbumName, _ArtistName, _Duration);

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
            accountController.AddAccount(_Username, _Password);
            accountController.AddAccount(_Username2, _Password);

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
            albumController.AddAlbum(_AlbumName, _ArtistName, _artistImage);
            albumController.AddAlbum(_AlbumName2, _ArtistName, _artistImage);

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
            artistController.AddArtist(_ArtistName, _artistImage);
            artistController.AddArtist(_ArtistName2, _artistImage2);

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

            Song song1 = songController.FindSong(_SongName);
            Song song2 = songController.FindSong(_SongName2);

            if (song1.GetAlbum() == _AlbumName && song1.GetArtist() == _ArtistName && song1.GetDuration() == _Duration)
                parsedFirst = true;

            if (song2.GetAlbum() == _AlbumName && song2.GetArtist() == _ArtistName && song2.GetDuration() == _Duration)
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

            Account account1 = accountController.FindAccount(_Username);
            Account account2 = accountController.FindAccount(_Username2);

            if (account1.getPassword() == _Password)
                parsedFirst = true;

            if(account2.getPassword() == _Password)
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

            Album album1 = albumController.FindAlbum(_AlbumName);
            Album album2 = albumController.FindAlbum(_AlbumName2);

            if (album1.GetArtist() == _ArtistName && Utils.CompareBitmaps(_albumImg, album1.GetImage()))
                parsedFirst = true;

            if (album2.GetArtist() == _ArtistName && Utils.CompareBitmaps(_albumImg2, album2.GetImage()))
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

            Artist artist1 = artistController.FindArtist(_ArtistName);
            Artist artist2 = artistController.FindArtist(_ArtistName2);

            if (Utils.CompareBitmaps(artist1.GetImage(), _artistImage))
                parsedFirst = true;

            if (Utils.CompareBitmaps(artist2.GetImage(), _artistImage2))
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

    [TestClass]
    public class LoggerTests
    {
        /* Singleton logger instance */
        Logger logger = Logger.Instance;

        /* Account members */
        private const string _Username = "username";
        private const string _Password = "password";

        [TestMethod]
        public void SLOGUNIT001_Log_SentLogIn_LoggedPacket()
        {
            // Arrange
            File.Delete(Constants.ServerLogsFile);
            Logger.SetFileName(Constants.ServerLogsFile);

            string expected = $"{DateTime.Now.ToString("dddd, dd MMMM yyyy hh:mm tt")} - Sent Log In Response to Client for username (username), password (password) (Status: Failure)\r\n";
            PacketHeader packetHeader = new(PacketHeader.AccountAction.LogIn);

            /* Simulates Server appending LogIn response */
            Account accountBody = new Account(_Username, _Password);
            accountBody.setStatus(Account.Status.Failure);

            PacketBody packetBody = accountBody;

            Packet packet = new Packet(packetHeader, packetBody);

            // Act
            logger.Log(packet, true); // logs the current Packet being sent to *Client*

            string actual = File.ReadAllText(Constants.ServerLogsFile);
            // Arrange
            
            Assert.AreEqual(expected, actual);
        }

        [TestMethod]
        public void SLOGUNIT002_Log_SentSignUp_LoggedPacket()
        {
            // Arrange
            File.Delete(Constants.ServerLogsFile);
            Logger.SetFileName(Constants.ServerLogsFile);

            string expected = $"{DateTime.Now.ToString("dddd, dd MMMM yyyy hh:mm tt")} - Sent Sign Up Response to Client for username (username), password (password) (Status: Success)\r\n";
            PacketHeader packetHeader = new(PacketHeader.AccountAction.SignUp);

            /* Simulates Server appending Sign Up response */
            Account accountBody = new Account(_Username, _Password);
            accountBody.setStatus(Account.Status.Success);

            PacketBody packetBody = accountBody;

            Packet packet = new Packet(packetHeader, packetBody);

            // Act
            logger.Log(packet, true); // logs the current Packet being sent to *Client*

            string actual = File.ReadAllText(Constants.ServerLogsFile);
            // Arrange

            Assert.AreEqual(expected, actual);
        }


        [TestMethod]
        public void SLOGUNIT003_Log_SentSearch_LoggedPacket()
        {
            // Arrange
            File.Delete(Constants.ServerLogsFile);
            Logger.SetFileName(Constants.ServerLogsFile);

            string expected = $"{DateTime.Now.ToString("dddd, dd MMMM yyyy hh:mm tt")} - Sent List/Search/Filter Response to Client: Filter (filter), Context (953128) Data Byte Count (1230)\r\n";
            PacketHeader packetHeader = new (PacketHeader.SongAction.List);

            // Simulates Appending server response
            SearchBody searchBody = new(953128, "filter", new byte[1230]);
            PacketBody packetBody = searchBody;

            Packet packet = new Packet(packetHeader, packetBody);

            // Act
            logger.Log(packet, true); // logs the current Packet being sent to *Client*

            string actual = File.ReadAllText(Constants.ServerLogsFile);

            // Assert
            Assert.AreEqual(expected , actual);
        }

        [TestMethod]
        public void SLOGUNIT004_Log_SentDownload_LoggedPacket()
        {
            // Arrange
            File.Delete(Constants.ServerLogsFile);
            Logger.SetFileName(Constants.ServerLogsFile);

            string expected = $"{DateTime.Now.ToString("dddd, dd MMMM yyyy hh:mm tt")} - Sent Download Response to Client: Type (AlbumCover), Hash (953128) Block Index (12), Total Blocks (16), Data Byte Count (15)\r\n";
            PacketHeader packetHeader = new PacketHeader(PacketHeader.SongAction.Download);

            // Simulates appending server response
            DownloadBody downloadBody = new DownloadBody(DownloadBody.Type.AlbumCover, 953128, 12, 16, 15, new byte[15]);
            PacketBody packetBody = downloadBody;

            Packet packet = new Packet(packetHeader, packetBody);

            // Act
            logger.Log(packet, true);

            string actual = File.ReadAllText(Constants.ServerLogsFile);

            // Assert
            Assert.AreEqual(expected, actual);
        }

        [TestMethod]
        public void SLOGUNIT005_LogSentSync_LoggedPacket()
        {
            // Arrange
            File.Delete(Constants.ServerLogsFile);
            Logger.SetFileName(Constants.ServerLogsFile);

            string expected = $"{DateTime.Now.ToString("dddd, dd MMMM yyyy hh:mm tt")} - Sent Sync Response to Client: Current play time (1234555), Stream state (Playing)\r\n";
            PacketHeader packetHeader = new PacketHeader(PacketHeader.SongAction.Sync);

            // Simulates appending server response
            SyncBody syncBody = new SyncBody(1234555, SyncBody.State.Playing);
            PacketBody packetBody = syncBody;

            Packet packet = new Packet(packetHeader, packetBody);

            // Act
            logger.Log(packet, true);

            string actual = File.ReadAllText(Constants.ServerLogsFile);

            // Assert
            Assert.AreEqual(expected, actual);
        }
        
        [TestMethod]
        public void SLOGUNIT006_LogSentMediaControl_LoggedPacket()
        {
            // Arrange
            File.Delete(Constants.ServerLogsFile);
            Logger.SetFileName(Constants.ServerLogsFile);

            string expected = $"{DateTime.Now.ToString("dddd, dd MMMM yyyy hh:mm tt")} - Sent Media Response to Client: Action (Previous) Current Media State (Playing)\r\n";
            PacketHeader packetHeader = new(PacketHeader.SongAction.Media);

            // Simulates adding server response
            MediaControlBody mediaControlBody = new(MediaControlBody.Action.Previous, MediaControlBody.State.Playing);

            PacketBody packetBody = mediaControlBody;

            Packet packet = new Packet(packetHeader, packetBody);

            // Act
            logger.Log(packet, true);

            string actual = File.ReadAllText(Constants.ServerLogsFile);

            // Assert
            Assert.AreEqual(expected, actual);
        }

        [TestMethod]
        public void SLOGUNIT007_LogReceivedLogIn_LoggedPacket()
        {
            // Arrange
            File.Delete(Constants.ServerLogsFile);
            Logger.SetFileName(Constants.ServerLogsFile);

            string expected = $"{DateTime.Now.ToString("dddd, dd MMMM yyyy hh:mm tt")} - Received Log In Request from Client for username (username), password (password)\r\n";

            PacketHeader packetHeader = new(PacketHeader.AccountAction.LogIn);
            Account accountBody = new Account(_Username, _Password);
            PacketBody packetBody = accountBody;
            Packet packet = new Packet(packetHeader, packetBody);

            // Simulates server receiving packet
            Packet serverPacket = new Packet(packet.Serialize());

            // Act
            logger.Log(serverPacket, false); // logs the current Packet being received from Client

            string actual = File.ReadAllText(Constants.ServerLogsFile);

            // Assert

            Assert.AreEqual(expected, actual);
        }

        [TestMethod]
        public void SLOGUNIT008_LogReceivedSignUp_LoggedPacket()
        {
            // Arrange
            File.Delete(Constants.ServerLogsFile);
            Logger.SetFileName(Constants.ServerLogsFile);

            string expected = $"{DateTime.Now.ToString("dddd, dd MMMM yyyy hh:mm tt")} - Received Sign Up Request from Client for username (username), password (password)\r\n";

            PacketHeader packetHeader = new(PacketHeader.AccountAction.SignUp);
            Account accountBody = new Account(_Username, _Password);
            PacketBody packetBody = accountBody;
            Packet packet = new Packet(packetHeader, packetBody);

            // Simulates server receiving packet
            Packet serverPacket = new Packet(packet.Serialize());

            // Act
            logger.Log(serverPacket, false); // logs the current Packet being received from Client

            string actual = File.ReadAllText(Constants.ServerLogsFile);

            // Assert

            Assert.AreEqual(expected, actual);

        }

        [TestMethod]
        public void SLOGUNIT009_LogReceivedSearch_LoggedPacket()
        {
            // Arrange
            File.Delete(Constants.ServerLogsFile);
            Logger.SetFileName(Constants.ServerLogsFile);

            string expected = $"{DateTime.Now.ToString("dddd, dd MMMM yyyy hh:mm tt")} - Received List/Search/Filter Request from Client: Filter (filter), Context (12345)\r\n";

            PacketHeader packetHeader = new(PacketHeader.SongAction.List);
            SearchBody searchBody = new(12345, "filter");
            PacketBody packetBody = searchBody;
            Packet packet = new Packet(packetHeader, packetBody);

            // Simulates server receiving packet
            Packet serverPacket = new Packet(packet.Serialize());

            // Act
            logger.Log(serverPacket, false); // logs the current Packet being received from Client

            string actual = File.ReadAllText(Constants.ServerLogsFile);

            // Assert

            Assert.AreEqual(expected, actual);

        }

        [TestMethod]
        public void SLOGUNIT010_LogReceivedDownload_LoggedPacket()
        {
            // Arrange
            File.Delete(Constants.ServerLogsFile);
            Logger.SetFileName(Constants.ServerLogsFile);

            string expected = $"{DateTime.Now.ToString("dddd, dd MMMM yyyy hh:mm tt")} - Received Download Request from Client: Type (AlbumCover), Hash (55555)\r\n";

            PacketHeader packetHeader = new(PacketHeader.SongAction.Download);
            DownloadBody downloadBody = new DownloadBody(DownloadBody.Type.AlbumCover, 55555);
            PacketBody packetBody = downloadBody;
            Packet packet = new Packet(packetHeader, packetBody);

            // Simulates server receiving packet
            Packet serverPacket = new Packet(packet.Serialize());

            // Act
            logger.Log(serverPacket, false); // logs the current Packet being received from Client

            string actual = File.ReadAllText(Constants.ServerLogsFile);

            // Assert

            Assert.AreEqual(expected, actual);

        }

        [TestMethod]
        public void SLOGUNIT012_LogReceivedMediaControl_LoggedPacket()
        {
            // Arrange
            File.Delete(Constants.ServerLogsFile);
            Logger.SetFileName(Constants.ServerLogsFile);

            string expected = $"{DateTime.Now.ToString("dddd, dd MMMM yyyy hh:mm tt")} - Received Media Request from Client: Action (Play)\r\n";

            PacketHeader packetHeader = new(PacketHeader.SongAction.Media);
            MediaControlBody mediaControlBody = new(MediaControlBody.Action.Play);
            PacketBody packetBody = mediaControlBody;
            Packet packet = new Packet(packetHeader, packetBody);

            // Simulates server receiving packet
            Packet serverPacket = new Packet(packet.Serialize());

            // Act
            logger.Log(serverPacket, false); // logs the current Packet being received from Client

            string actual = File.ReadAllText(Constants.ServerLogsFile);

            // Assert

            Assert.AreEqual(expected, actual);

        }

        [TestMethod]
        public void SLOGUNIT013_LogTimeSent_LoggedPacket()
        {
            // Arrange
            File.Delete(Constants.ServerLogsFile);
            Logger.SetFileName(Constants.ServerLogsFile);

            string dtString = DateTime.Now.ToString("dddd, dd MMMM yyyy hh:mm tt");
            PacketHeader packetHeader = new(PacketHeader.SongAction.Media);

            // Simulates adding server response
            MediaControlBody mediaControlBody = new(MediaControlBody.Action.Previous, MediaControlBody.State.Playing);

            PacketBody packetBody = mediaControlBody;

            Packet packet = new Packet(packetHeader, packetBody);

            // Act
            logger.Log(packet, true);

            string actual = File.ReadAllText(Constants.ServerLogsFile);

            // Assert
            Assert.IsTrue(actual.Contains(dtString));
        }

        [TestMethod]
        public void SLOGUNIT014_LogTimeReceived_LoggedPacket()
        {
            // Arrange
            File.Delete(Constants.ServerLogsFile);
            Logger.SetFileName(Constants.ServerLogsFile);

            string dtString = DateTime.Now.ToString("dddd, dd MMMM yyyy hh:mm tt");

            PacketHeader packetHeader = new(PacketHeader.SongAction.Media);
            MediaControlBody mediaControlBody = new(MediaControlBody.Action.Play);
            PacketBody packetBody = mediaControlBody;
            Packet packet = new Packet(packetHeader, packetBody);

            // Simulates server receiving packet
            Packet serverPacket = new Packet(packet.Serialize());

            // Act
            logger.Log(serverPacket, false); // logs the current Packet being received from Client

            string actual = File.ReadAllText(Constants.ServerLogsFile);

            // Assert
            Assert.IsTrue(actual.Contains(dtString));
        }

        [TestMethod]
        public void SLOGUNIT015_LogSentForgotPassword_LoggedPacket()
        {
            // Arrange
            File.Delete(Constants.ServerLogsFile);
            Logger.SetFileName(Constants.ServerLogsFile);

            string expected = $"{DateTime.Now.ToString("dddd, dd MMMM yyyy hh:mm tt")} - Sent Forgot Password Response to Client for username (username), password () (Status: Failure)\r\n";
            PacketHeader packetHeader = new(PacketHeader.AccountAction.LogIn);

            /* Simulates Server appending ForgotPassword response */
            Account accountBody = new Account();
            accountBody.setUsername(_Username);
            accountBody.setStatus(Account.Status.Failure);

            PacketBody packetBody = accountBody;

            Packet packet = new Packet(packetHeader, packetBody);

            // Act
            logger.Log(packet, true); // logs the current Packet being sent to *Client*

            string actual = File.ReadAllText(Constants.ServerLogsFile);

            // Arrange

            Assert.AreEqual(expected, actual);
        }


        [TestMethod]
        public void SLOGUNIT016_LogsReceivedForgotPassword_LoggedPacket()
        {
            // Arrange
            File.Delete(Constants.ServerLogsFile);
            Logger.SetFileName(Constants.ServerLogsFile);

            string expected = $"{DateTime.Now.ToString("dddd, dd MMMM yyyy hh:mm tt")} - Received Forgot Password Request from Client for username (username), password ()\r\n";

            PacketHeader packetHeader = new(PacketHeader.AccountAction.LogIn);
            Account accountBody = new Account(_Username, "");
            PacketBody packetBody = accountBody;
            Packet packet = new Packet(packetHeader, packetBody);

            // Simulates server receiving packet
            Packet serverPacket = new Packet(packet.Serialize());

            // Act
            logger.Log(serverPacket, false); // logs the current Packet being received from Client

            string actual = File.ReadAllText(Constants.ServerLogsFile);

            // Assert

            Assert.AreEqual(expected, actual);
        }
    }
}