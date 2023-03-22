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
            artistController.AddArtist(_ARTISTNAME, _artistImage);

            // Act
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
}