/*The System.Drawing namespace has functionalities that are only available on Windows, which leads to
  the CA1416 warning being raised. We have decided to disable it since the Silly Music Player App
  will be made for the Windows platform only. */
#pragma warning disable CA1416 

using System.Drawing;

namespace Shared.Tests
{
    [TestClass]
    public class AccountTests
    {
        private const string _PASSWORD = "passwordPlaceholder";
        private const string _USERNAME = "usernamePlaceholder";
        
        [TestMethod]
        public void ACCSHARED001_GetPassword_placeholder_returnsPlaceholder()
        {
            // Arrange
            Account account = new Account();
            account.setPassword(_PASSWORD);

            // Act
            string actual = account.getPassword();

            // Assert
            Assert.AreEqual(_PASSWORD, actual);
        }

        [TestMethod]
        public void ACCSHARED002_GetUsername_placeholder_returnsPlaceholder()
        {
            // Arrange
            Account account = new Account();
            account.setUsername(_USERNAME);

            // Act
            string actual = account.getUsername();

            // Assert
            Assert.AreEqual(_USERNAME, actual);
        }

        [TestMethod]
        public void ACCSHARED003_SetPassword_placeholder_Assigned()
        {
            // Arrange
            Account account = new Account();

            // Act
            account.setPassword(_PASSWORD);
            string actual = account.getPassword();

            // Assert
            Assert.AreEqual(_PASSWORD, actual);
        }

        [TestMethod]
        public void ACCSHARED004_SetUsername_placeholder_Assigned()
        {
            // Arrange
            Account account = new Account();

            // Act
            account.setUsername(_USERNAME);
            string actual = account.getUsername();

            // Assert
            Assert.AreEqual(_USERNAME, actual);
        }
    }

    [TestClass]
    public class PacketTests
    {
        [TestMethod]
        public void PACKETUNIT001_SerializeHeader_CorrectBytesAllocated()
        {
            // [ACCOUNT] [SONG] | [SIGNUP] [LOGIN] | [SYNC] [MEDIA] [DOWNLOAD] [LIST]
            byte expected = 0b10100110;


            PacketHeader pHeader = new PacketHeader(false, false, false, false, false, false, false, false);

            Assert.AreEqual(expected, pHeader.SerializeData());
        }

        [TestMethod]
        public void PACKETUNIT002_SerializeDownloadBody_CorrectBytesAllocated()
        {

        }

        [TestMethod]
        public void PACKETUNIT003_SerializeMediaBody_CorrectBytesAllocated()
        {

        }

        [TestMethod]
        public void PACKETUNIT004_SerializeSyncBody_CorrectBytesAllocated()
        {


        }

        [TestMethod]
        public void PACKETUNIT005_SerializeAccountBody_CorrectBytesAllocated()
        {

        }

        [TestMethod]
        public void PACKETUNIT006_SerializeSearchBody_CorrectBytesAllocated()
        {

        }

        [TestMethod]
        public void PACKETUNIT007_SerializeSongData_CorrectBytesAllocated()
        {

        }

        [TestMethod]
        public void PACKETUNIT008_SerializeImageData_CorrectBytesAllocated()
        {

        }

        [TestMethod]
        public void PACKETUNIT009_DeserializeHeader_CorrectMembersAssigned()
        {

        }

        [TestMethod]
        public void PACKETUNIT010_DeserializeDownloadBody_CorrectMembersAssigned()
        {

        }

        [TestMethod]
        public void PACKETUNIT011_DeserializeMediaBody_CorrectMembersAssigned()
        {

        }

        [TestMethod]
        public void PACKETUNIT012_DeserializeSyncBody_CorrectMembersAssigned()
        {

        }

        [TestMethod]
        public void PACKETUNIT013_DeserializeAccountBody_CorrectMembersAssigned()
        {

        }

        [TestMethod]
        public void PACKETUNIT014_DeserializeSearchBody_CorrectMembersAssigned()
        {

        }

        [TestMethod]
        public void PACKETUNIT015_DeserializeSongData_CorrectMembersAssigned()
        {

        }

        [TestMethod]
        public void PACKETUNIT016_DeserializeImageData_CorrectMembersAssigned()
        {

        }
    }


    [TestClass]
    public class SongTests
    {
        private const string _SONGNAME = "songNamePlaceholder";
        private const string _ARTISTNAME = "artistPlaceholder";
        private const string _ALBUMNAME = "albumPlaceholder";
        private const float _DURATION = 1.0f;

        [TestMethod]
        public void SONGSHARED001_SetName_placeholder_placeholderSet()
        {
            // Arrange
            Song song = new();

            // Act
            song.SetName(_SONGNAME);
            string actual = song.GetName();

            // Assert
            Assert.AreEqual(_SONGNAME, actual);
        }

        [TestMethod]
        public void SONGSHARED002_SetArtist_placeholder_placeholderSet()
        {
            // Arrange
            Song song = new();

            // Act
            song.SetArtist(_ARTISTNAME);
            string actual = song.GetArtist();

            // Assert
            Assert.AreEqual(_ARTISTNAME, actual);
        }

        [TestMethod]
        public void SONGSHARED003_SetDuration_placeholder_placeholderSet()
        {
            // Arrange
            Song song = new();

            // Act
            song.SetDuration(_DURATION);
            float actual = song.GetDuration();

            // Assert
            Assert.AreEqual(_DURATION, actual);
        }

        [TestMethod]
        public void SONGSHARED004_SetAlbum_placeholder_placeholderSet()
        {
            // Arrange
            Song song = new();

            // Act
            song.SetAlbum(_ALBUMNAME);
            string actual = song.GetAlbum();

            // Assert
            Assert.AreEqual(_ALBUMNAME, actual);
        }

        [TestMethod]
        public void SONGSHARED005_GetName_placeholder_returnsPlaceholder()
        {
            // Arrange
            Song song = new();
            song.SetName(_SONGNAME);

            // Act
            string actual = song.GetName();

            // Assert
            Assert.AreEqual(_SONGNAME, actual);
        }

        [TestMethod]
        public void SONGSHARED006_GetArtist_placeholder_returnsPlaceholder()
        {
            // Arrange
            Song song = new();
            song.SetArtist(_ARTISTNAME);

            // Act
            string actual = song.GetArtist();

            // Assert
            Assert.AreEqual(_ARTISTNAME, actual);
        }

        [TestMethod]
        public void SONGSHARED007_GetDuration_placeholder_returnsPlaceholder()
        {
            // Arrange
            Song song = new();
            song.SetDuration(_DURATION);

            // Act
            float actual = song.GetDuration();

            // Assert
            Assert.AreEqual(_DURATION, actual);
        }

        [TestMethod]
        public void SONGSHARED008_GetAlbum_placeholder_returnsPlaceholder()
        {
            // Arrange
            Song song = new();
            song.SetAlbum(_ALBUMNAME);

            // Act
            string actual = song.GetAlbum();

            // Assert
            Assert.AreEqual(_ALBUMNAME, actual);
        }
    }

    [TestClass]
    public class ArtistTests
    {
        private Bitmap _bitmap = (Bitmap)Image.FromFile("placeholder.png");
        private const string _ARTISTNAME = "artistPlaceholder";

        [TestMethod]
        public void ARTISTSHARED001_SetName_placeholder_placeholderSet()
        {
            // Arrange
            Artist artist = new();

            // Act
            artist.SetName(_ARTISTNAME);
            string actual = artist.GetName();

            // Assert
            Assert.AreEqual(_ARTISTNAME, actual);
        }

        [TestMethod]
        public void ARTISTSHARED002_SetImage_placeholder_placeholderSet()
        {
            // Arrange
            Artist artist = new();

            // Act
            artist.SetImage(_bitmap);
            Bitmap actual = artist.GetImage();

            // Assert
            Assert.AreEqual(_bitmap, actual);
        }

        [TestMethod]
        public void ARTISTSHARED003_GetName_placeholder_returnsPlaceholder()
        {
            // Arrange
            Artist artist = new();
            artist.SetName(_ARTISTNAME);

            // Act
            string actual = artist.GetName();

            // Assert
            Assert.AreEqual(_ARTISTNAME, actual);
        }

        [TestMethod]
        public void ARTISTSHARED004_GetImage_placeholder_returnsPlaceholder()
        {
            // Arrange
            Artist artist = new();
            artist.SetImage(_bitmap);

            // Act
            Bitmap actual = artist.GetImage();

            // Assert
            Assert.AreEqual(_bitmap, actual);
        }
    }

    [TestClass]
    public class AlbumTests
    {
        private const string _ALBUMNAME = "albumPlaceholder";
        private const string _ARTISTNAME = "artistPlaceholder";
        private Bitmap _bitmap = (Bitmap)Image.FromFile("placeholder.png");

        [TestMethod]
        public void ALBUMSHARED001_SetName_placeholder_placeholderSet()
        {
            // Arrange
            Album album = new();

            // Act
            album.SetName(_ALBUMNAME);
            string actual = album.GetName();

            // Assert
            Assert.AreEqual(_ALBUMNAME, actual);
        }

        [TestMethod]
        public void ALBUMSHARED002_SetArtist_placeholder_placeholderSet()
        {
            // Arrange
            Album album = new();

            // Act
            album.SetArtist(_ARTISTNAME);
            string actual = album.GetArtist();

            // Assert
            Assert.AreEqual(_ARTISTNAME, actual);
        }

        [TestMethod]
        public void ALBUMSHARED003_SetImage_placeholder_placeholderSet()
        {
            // Arrange
            Album album = new();

            // Act
            album.SetImage(_bitmap);
            Bitmap actual = album.GetImage();

            // Assert
            Assert.AreEqual(_bitmap, actual);
        }

        [TestMethod]
        public void ALBUMSHARED004_GetName_placeholder_returnsPlaceholder()
        {
            // Arrange
            Album album = new();
            album.SetName(_ALBUMNAME);

            // Act
            string actual = album.GetName();

            // Assert
            Assert.AreEqual(_ALBUMNAME, actual);
        }

        [TestMethod]
        public void ALBUMSHARED005_GetImage_placeholder_returnsPlaceholder()
        {
            // Arrange
            Album album = new();
            album.SetImage(_bitmap);

            // Act
            Bitmap actual = album.GetImage();

            // Assert
            Assert.AreEqual(_bitmap, actual);
        }

        [TestMethod]
        public void ALBUMSHARED006_GetArtist_placeholder_returnsPlaceholder()
        {
            // Arrange
            Album album = new();
            album.SetArtist(_ARTISTNAME);

            // Act
            string actual = album.GetArtist();

            // Assert
            Assert.AreEqual(_ARTISTNAME, actual);
        }
    }
}