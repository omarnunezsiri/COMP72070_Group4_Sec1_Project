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
        private const string _ARTISTNAME = "deborah george";

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

            // Act

            // Assert
            Assert.Fail();
        }

        [TestMethod]
        public void ARTISTUNIT003_DeleteArtist_ArtistNotInCollection()
        {
            // Arrange

            // Act

            // Assert
            Assert.Fail();
        }

        [TestMethod]
        public void ARTISTUNIT004_UpdateArtist_Name_ArtistUpdated()
        {
            // Arrange

            // Act

            // Assert
            Assert.Fail();
        }

        [TestMethod]
        public void ARTISTUNIT005_UpdateArtist_Image_ArtistUpdated()
        {
            // Arrange

            // Act

            // Assert
            Assert.Fail();
        }

        [TestMethod]
        public void ARTISTUNIT006_AddArtist_ArtistExists_NotAdded()
        {
            // Arrange

            // Act

            // Assert
            Assert.Fail();
        }

        [TestMethod]
        public void ARTISTUNIT007_DeleteArtist_ArtistDoesntExist_NothingHappens()
        {
            // Arrange

            // Act

            // Assert
            Assert.Fail();
        }

        [TestMethod]
        public void ARTISTUNIT008_FindArtist_ArtistReturned()
        {
            // Arrange

            // Act

            // Assert
            Assert.Fail();
        }

        [TestMethod]
        public void ARTISTUNIT009_FindUnexistingArtist_ExceptionThrown()
        {
            // Arrange

            // Act

            // Assert
            Assert.Fail();
        }
    }
}