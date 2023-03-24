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

            // Act

            // Assert
            Assert.Fail();
        }

        [TestMethod]
        public void ACCUNIT005_AccountAuth_Fail()
        {
            // Arrange

            // Act

            // Assert
            Assert.Fail();
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
    }
}