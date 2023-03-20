/*The System.Drawing namespace has functionalities that are only available on Windows, which leads to
  the CA1416 warning being raised. We have decided to disable it since the Silly Music Player App
  will be made for the Windows platform only. */
#pragma warning disable CA1416 

using Server;
using System.Drawing;
using System.Xml.Linq;

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

        [TestMethod]
        public void ACCSHARED005_DefaultConstructor_ObjectInSafeState()
        {
            // Arrange and Act
            Account account = new();

            // Assert
            Assert.AreEqual(string.Empty, account.getUsername(), "Username not set to safe state");
            Assert.AreEqual(string.Empty, account.getPassword(), "Password not set to safe state");
        }

        [TestMethod]
        public void ACCSHARED006_ParameterizedConstructor_placeholders_Assigned()
        {
            // Arrange and Act
            Account account = new(_USERNAME, _PASSWORD);

            // Assert
            Assert.AreEqual(_USERNAME, account.getUsername(), "Username not set to placeholder");
            Assert.AreEqual(_PASSWORD, account.getPassword(), "Password not set to placeholder");
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

            PacketHeader pHeader = new PacketHeader(true, false, true, false, false, true, true, false);

            Assert.AreEqual(expected, pHeader.Serialize());
        }

        [TestMethod]
        public void PACKETUNIT101_SerializeHeader_CorrectBytesAllocated()
        {
            // [ACCOUNT] [SONG] | [SIGNUP] [LOGIN] | [SYNC] [MEDIA] [DOWNLOAD] [LIST]
            byte expected = 0b01011001;

            PacketHeader pHeader = new PacketHeader(false, true, false, true, true, false, false, true);

            Assert.AreEqual(expected, pHeader.Serialize());
        }

        [TestMethod]
        public void PACKETUNIT002_SerializeDownloadBody_ResponseSerialize()
        {
            // [ISALBUMCOVER] [ISSONGFILE] [-] [-] [-] [-] [-] [-] | [ITEM HASH 8Bytes] [Block index 2bytes] [Total Block count 2bytes] [Data Bytecount 4bytes] [Data nbytes]

            byte[] data = new byte[] { 10, 20, 30, 40, 50, 60, 70, 80, 90, 100 };
            byte[] expected = new byte[17] { 0b10000000, 1, 2, 3, 4, 5, 6, 7, 8, 0x11, 0x12, 0x21, 0x22, 0x31, 0x32, 0x33, 0x34 };

            byte[] fullExpected = new byte[data.Length + expected.Length];
            expected.CopyTo(fullExpected, 0);
            data.CopyTo(fullExpected, expected.Length);

            UInt64 hash = 0x0102030405060708;
            DownloadBody mBody = new DownloadBody(DownloadBody.Type.AlbumCover, hash, 0x1112, 0x2122, 0x31323334, data );

            byte[] serialized = mBody.Serialize();

            Assert.IsTrue(Enumerable.SequenceEqual(fullExpected, serialized));
        }

        [TestMethod]
        public void PACKETUNIT102_SerializeDownloadBody_RequestSerialize()
        {
            // [ISALBUMCOVER] [ISSONGFILE] [-] [-] [-] [-] [-] [-] | [ITEM HASH 8Bytes] [Block index 2bytes] [Total Block count 2bytes] [Data Bytecount 4bytes] [Data nbytes]

            byte[] expected = new byte[17] { 0b10000000, 1, 2, 3, 4, 5, 6, 7, 8,   0, 0, 0, 0, 0, 0, 0, 0 };

            UInt64 hash = 0x0102030405060708;
            DownloadBody mBody = new DownloadBody(DownloadBody.Type.AlbumCover, hash);

            byte[] serialized = mBody.Serialize();

            Assert.IsTrue(Enumerable.SequenceEqual(expected, serialized));
        }

        [TestMethod]
        public void PACKETUNIT003_SerializeMediaBody_ServerSerialize()
        {
            // [PLAY] [PAUSE] [PREVIOUS] [SKIP/NEXT] [GETSTATE] [-] [-] [-] | [PLAYING] [PAUSED] [IDLE] [-] [-] [-] [-] [-]
            byte[] expected = new byte[2] { 0b00111000, 0b11000000 }; // 56 192

            MediaControlBody mBody = new MediaControlBody(false, false, true, true, true, true, true, false);

            byte[] serialized = mBody.Serialize();

            Assert.IsTrue(Enumerable.SequenceEqual(expected, serialized));
        }

        [TestMethod]
        public void PACKETUNIT103_SerializeMediaBody_ClientSerialize()
        {
            // [PLAY] [PAUSE] [PREVIOUS] [SKIP/NEXT] [GETSTATE] [-] [-] [-] | [PLAYING] [PAUSED] [IDLE] [-] [-] [-] [-] [-]
            byte[] expected = new byte[2] { 0b10011000, 0b00000000 }; // 152 0

            MediaControlBody mBody = new MediaControlBody(true, false, false, true, true);

            byte[] serialized = mBody.Serialize();

            Assert.IsTrue(Enumerable.SequenceEqual(expected, serialized));
        }

        [TestMethod]
        public void PACKETUNIT004_SerializeSyncBody_CorrectBytesAllocated()
        {
            Assert.Fail();

        }

        [TestMethod]
        public void PACKETUNIT005_SerializeAccountBody_CorrectBytesAllocated()
        {
            Assert.Fail();
        }

        [TestMethod]
        public void PACKETUNIT006_SerializeSearchBody_CorrectBytesAllocated()
        {
            Assert.Fail();
        }

        [TestMethod]
        public void PACKETUNIT007_SerializeSongData_CorrectBytesAllocated()
        {
            Assert.Fail();
        }

        [TestMethod]
        public void PACKETUNIT008_SerializeImageData_CorrectBytesAllocated()
        {
            Assert.Fail();
        }

        [TestMethod]
        public void PACKETUNIT009_DeserializeHeader_CorrectMembersAssigned()
        {
            Assert.Fail();
        }

        [TestMethod]
        public void PACKETUNIT010_DeserializeDownloadBody_CorrectMembersAssigned()
        {
            Assert.Fail();
        }

        [TestMethod]
        public void PACKETUNIT011_DeserializeMediaBody_CorrectMembersAssigned()
        {
            Assert.Fail();
        }

        [TestMethod]
        public void PACKETUNIT012_DeserializeSyncBody_CorrectMembersAssigned()
        {
            Assert.Fail();
        }

        [TestMethod]
        public void PACKETUNIT013_DeserializeAccountBody_CorrectMembersAssigned()
        {
            Assert.Fail();
        }

        [TestMethod]
        public void PACKETUNIT014_DeserializeSearchBody_CorrectMembersAssigned()
        {
            Assert.Fail();
        }

        [TestMethod]
        public void PACKETUNIT015_DeserializeSongData_CorrectMembersAssigned()
        {
            Assert.Fail();
        }

        [TestMethod]
        public void PACKETUNIT016_DeserializeImageData_CorrectMembersAssigned()
        {
            Assert.Fail();
        }
    }

    [TestClass]
    public class UtilsTests
    {
        private const int _X = 15;
        private const int _Y = 15;
        private const int _ANOTHERY = 20;

        [TestMethod]
        public void UTILSSHARED001_CompareBitmaps_DifferentSize_ReturnsFalse()
        {
            // Arrange
            Bitmap a = new(_X, _Y);
            Bitmap b = new(_X, _ANOTHERY);

            const bool EXPECTED = false;

            // Act
            bool sameBitmap = Utils.CompareBitmaps(a, b);

            // Assert
            Assert.AreEqual(EXPECTED, sameBitmap);
        }

        [TestMethod]
        public void UTILSSHARED002_CompareBitmaps_DifferentPixels_ReturnsFalse()
        {
            // Arrange
            Bitmap a = (Bitmap)Image.FromFile("firstpng.png");
            Bitmap b = (Bitmap)Image.FromFile("second.jpg");

            const bool EXPECTED = false;

            // Act
            bool sameBitmap = Utils.CompareBitmaps(a, b);

            // Assert
            Assert.AreEqual(EXPECTED, sameBitmap);
        }

        [TestMethod]
        public void UTILSSHARED003_CompareBitmaps_SameImage_ReturnsTrue()
        {
            // Arrange
            Bitmap a = (Bitmap)Image.FromFile("ubuntu.png");
            Bitmap b = (Bitmap)Image.FromFile("ubuntu.png");

            const bool EXPECTED = true;

            // Act
            bool sameBitmap = Utils.CompareBitmaps(a, b);

            // Assert
            Assert.AreEqual(EXPECTED, sameBitmap);
        }

        [TestMethod]
        public void UTILSSHARED004_GetBitmapBytes_Bitmap_CanParseBitmap()
        {
            // Arrange
            Bitmap bmp = (Bitmap)Image.FromFile("ubuntu.png");

            // Act
            byte[] bitmapBytes = Utils.GetBitmapBytes(bmp);

            Bitmap anotherBmp = Utils.GetBitmapFromBytes(bitmapBytes); // assumes that method works properly

            // Assert
            Assert.IsTrue(Utils.CompareBitmaps(bmp, anotherBmp));
        }

        [TestMethod]
        public void UTILSSHARED005_GetBitmapFromBytes_byteArray_BitmapParsed()
        {
            // Arrange
            Bitmap bmp = (Bitmap)Image.FromFile("second.jpg");

            byte[] bitmapBytes = Utils.GetBitmapBytes(bmp); // assumes that method works properly

            // Act
            Bitmap anotherBmp = Utils.GetBitmapFromBytes(bitmapBytes);

            // Assert
            Assert.IsTrue(Utils.CompareBitmaps(anotherBmp, bmp));
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

        [TestMethod]
        public void SONGSHARED009_DefaultConstructor_ObjectInSafeState()
        {
            // Arrange and Act
            Song song = new();

            // Assert
            Assert.AreEqual(string.Empty, song.GetName(), "Song name not set to safe state");
            Assert.AreEqual(string.Empty, song.GetAlbum(), "Song album not set to safe state");
            Assert.AreEqual(string.Empty, song.GetArtist(), "Song artist not set to safe state");
            Assert.AreEqual(0.0f, song.GetDuration(), "Song duration not set to safe state");
        }

        [TestMethod]
        public void SONGSHARED010_ParameterizedConstructor_placeholders_Assigned()
        {
            // Arrange and Act
            Song song = new(_SONGNAME, _ALBUMNAME, _ARTISTNAME, _DURATION);

            // Assert
            Assert.AreEqual(_SONGNAME, song.GetName(), "Song name not set to placeholder");
            Assert.AreEqual(_ALBUMNAME, song.GetAlbum(), "Song album not set to placeholder");
            Assert.AreEqual(_ARTISTNAME, song.GetArtist(), "Song artist not set to placeholder");
            Assert.AreEqual(_DURATION, song.GetDuration(), "Song duration not set to placeholder");
        }
    }

    [TestClass]
    public class ArtistTests
    {
        private Bitmap _bitmap = (Bitmap)Image.FromFile("placeholder.png");
        private Bitmap _defaultBitmap = (Bitmap)Image.FromFile("default.png");
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

        [TestMethod]
        public void ARTISTSHARED005_DefaultConstructor_ObjectInSafeState()
        {
            // Arrange and Act
            Artist artist = new();

            bool imagesAreSame = Utils.CompareBitmaps(_defaultBitmap, artist.GetImage());

            // Assert
            Assert.AreEqual(string.Empty, artist.GetName(), "Artist name not set to safe state");
            Assert.IsTrue(imagesAreSame, "Artist image not set to safe state");
        }

        [TestMethod]
        public void ARTISTSHARED006_ParameterizedConstructor_placeholders_Assigned()
        {
            // Arrange and Act
            Artist artist = new(_ARTISTNAME, _bitmap);

            // Assert
            Assert.AreEqual(_ARTISTNAME, artist.GetName(), "Artist name not set to placeholder");
            Assert.AreEqual(_bitmap, artist.GetImage(), "Artist image not set to placeholder");
        }
    }

    [TestClass]
    public class AlbumTests
    {
        private const string _ALBUMNAME = "albumPlaceholder";
        private const string _ARTISTNAME = "artistPlaceholder";
        private Bitmap _bitmap = (Bitmap)Image.FromFile("placeholder.png");
        private Bitmap _defaultBitmap = (Bitmap)Image.FromFile("default.png");

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

        [TestMethod]
        public void ALBUMSHARED007_DefaultConstructor_ObjectInSafeState()
        {
            // Arrange and Act
            Album album = new();

            bool imagesAreSame = Utils.CompareBitmaps(_defaultBitmap, album.GetImage());

            // Assert
            Assert.AreEqual(string.Empty, album.GetName(), "Album name not set to safe state");
            Assert.AreEqual(string.Empty, album.GetArtist(), "Artist name not set to safe state");
            Assert.IsTrue(imagesAreSame, "Album image not set to safe state");
        }

        [TestMethod]
        public void ALBUMSHARED008_ParameterizedConstructor_placeholders_Assigned()
        {
            // Arrange and Act
            Album album = new(_ALBUMNAME, _ARTISTNAME, _bitmap);

            // Assert
            Assert.AreEqual(_ALBUMNAME, album.GetName(), "Album name not set to placeholder");
            Assert.AreEqual(_ARTISTNAME, album.GetArtist(), "Artist name not set to placeholder");
            Assert.AreEqual(_bitmap, album.GetImage(), "Album image not set to placeholder");
        }
    }

    [TestClass]
    public class FileHandlerTests
    {
        private string _MP3FILENAME = "mymp3.mp3";
        private string _IMAGENAME = "ubuntu.png";
        private string _OUTPUTMP3 = "output.mp3";
        private string _OUTPUTIMAGE = "output.png";
        private string _UNEXISTINGMP3 = "hello.mp3";
        private string _UNEXISTINGIMAGE = "wow.png";

        [TestMethod]
        public void FHSHARED001_writeMp3Bytes_byteArray_bytesWritten()
        {
            // Arrange
            byte[] mp3bytes = FileHandler.readMp3Bytes(_MP3FILENAME); // assumes that readMp3Bytes works correctly
            File.Delete(_OUTPUTMP3); // removes the file from dir if previous existence

            // Act
            bool bytesWritten = FileHandler.writeMp3Bytes(_OUTPUTMP3, mp3bytes);

            // Assert
            Assert.IsTrue(bytesWritten);
        }

        [TestMethod]
        public void FHSHARED002_readMp3Bytes_byteArrayCreated()
        {
            // Arrange
            Type expectedType = typeof(byte[]);

            // Act
            var mp3bytes = FileHandler.readMp3Bytes(_MP3FILENAME);

            // Assert
            Assert.AreEqual(expectedType, mp3bytes.GetType());
        }

        [TestMethod]
        public void FHSHARED003_writeImage_Bitmap_imageWritten()
        {
            // Arrange
            Bitmap original = FileHandler.readImageBytes(_IMAGENAME); // assumes that readImageBytes works
            File.Delete(_OUTPUTIMAGE); // removes the file from dir if previous existence

            // Act
            bool imageWritten = FileHandler.writeImageBytes(_OUTPUTIMAGE, original);

            // Assert
            Assert.IsTrue(imageWritten);
        }

        [TestMethod]
        public void FHSHARED004_readImage_existingImage_imageCreated()
        {
            // Arrange
            Type expectedType = typeof(Bitmap);

            // Act
            var image = FileHandler.readImageBytes(_IMAGENAME);

            // Assert
            Assert.AreEqual(expectedType, image.GetType());

        }

        [TestMethod]
        public void FHSHARED005_readMp3Bytes_unexistingFile_throwsException()
        {
            // Arrange
            const bool EXPECTED = true;

            // Act

            bool notFoundThrown = false;

            try
            {
                var mp3bytes = FileHandler.readMp3Bytes(_UNEXISTINGMP3);
            }
            catch (FileNotFoundException)
            {
                notFoundThrown = true;
            }

            // Assert
            Assert.AreEqual(EXPECTED, notFoundThrown, "FileNotFoundException wasn't thrown.");
        }

        [TestMethod]
        public void FHSHARED006_writeMp3Bytes_emptyByteArray_returnsFalse()
        {
            // Arrange
            byte[] mp3bytes = new byte[0]; // assumes that readMp3Bytes works correctly

            // Act
            bool bytesWritten = FileHandler.writeMp3Bytes(_OUTPUTMP3, mp3bytes);

            // Assert
            Assert.IsFalse(bytesWritten);
        }

        [TestMethod]
        public void FHSHARED007_readImage_unexistingImage_throwsException()
        {
            // Arrange
            bool exceptionThrown = false;

            // Act

            try
            {
                FileHandler.readImageBytes(_UNEXISTINGIMAGE);
            }
            catch (FileNotFoundException)
            {
                exceptionThrown = true;
            }

            // Assert
            Assert.IsTrue(exceptionThrown);
        }


        [TestMethod]
        public void FHSHARED008_writeMp3Bytes_byteArray_fileExists()
        {
            // Arrange
            byte[] mp3bytes = FileHandler.readMp3Bytes(_MP3FILENAME); // assumes that readMp3Bytes works correctly
            File.Delete(_OUTPUTMP3); // removes the file from dir if previous existence

            // Act
            FileHandler.writeMp3Bytes(_OUTPUTMP3, mp3bytes);

            // Assert
            Assert.IsTrue(File.Exists(_OUTPUTMP3));
        }

        [TestMethod]
        public void FHSHARED009_readMp3Bytes_expectedLength()
        {
            // Arrange
            const long EXPECTEDLENGTH = 2505799;

            // Act
            var mp3bytes = FileHandler.readMp3Bytes(_MP3FILENAME);

            // Assert
            Assert.AreEqual(EXPECTEDLENGTH, mp3bytes.Length, "Length of byte array is not the same as expected");
        }

        [TestMethod]
        public void FHSHARED010_writeMp3Bytes_byteArray_contentWritten()
        {
            // Arrange
            byte[] mp3bytes = FileHandler.readMp3Bytes(_MP3FILENAME); // assumes that readMp3Bytes works correctly
            File.Delete(_OUTPUTMP3); // removes the file from dir if previous existence

            // Act
            FileHandler.writeMp3Bytes(_OUTPUTMP3, mp3bytes);

            byte[] anotherbytes = FileHandler.readMp3Bytes(_OUTPUTMP3);

            // Assert
            Assert.IsTrue(mp3bytes.SequenceEqual(anotherbytes));
        }

        [TestMethod]
        public void FHSHARED011_writeImage_Bitmap_contentWritten()
        {
            // Arrange
            Bitmap bitmap = FileHandler.readImageBytes(_IMAGENAME); // assumes that readImageBytes works correctly
            File.Delete(_OUTPUTIMAGE); // removes the file from dir if previous existence

            // Act
            FileHandler.writeImageBytes(_OUTPUTIMAGE, bitmap);

            Bitmap anotherBitmap = FileHandler.readImageBytes(_OUTPUTIMAGE);

            // Assert
            Assert.IsTrue(Utils.CompareBitmaps(bitmap, anotherBitmap));
        }

        [TestMethod]
        public void FHSHARED012_writeMp3Bytes_nullByteArray_returnsFalse()
        {
            // Arrange and Act
            bool bytesWritten = FileHandler.writeMp3Bytes(_OUTPUTMP3, null);

            // Assert
            Assert.IsFalse(bytesWritten);
        }
    }

    [TestClass]
    public class SerializableTests
    {
        private const string _USERNAME = "user";
        private const string _PASSWORD = "pass";
        private const string _SONGNAME = "name";
        private const string _SONGALBUM = "album";
        private const string _SONGARTIST = "artist";
        private const float _SONGDURATION = 3.12f;

        [TestMethod]
        public void ACCSHARED007_Serialize_AccountObject_byteArrayReturned()
        {
            // Arrange
            Account account = new Account(_USERNAME, _PASSWORD);

            // [USERNAMELENGTH 1byte] [USERNAME lengthBytes] | [PASSWORDLENGTH 1byte] [PASSWORD lengthBytes]
            byte[] expected = new byte[] {4, 117, 115, 101, 114, 4, 112, 97, 115, 115};

            // Act
            byte[] serialized = account.Serialize();

            // Assert
            Assert.IsTrue(Enumerable.SequenceEqual(expected, serialized));
        }

        [TestMethod]
        public void ACCSHARED008_Deserialize_byteArray_AccountObjectCreated()
        {
            // Arrange

            // [USERNAMELENGTH 1byte] [USERNAME lengthBytes] | [PASSWORDLENGTH 1byte] [PASSWORD lengthBytes]
            byte[] serialized = new byte[] { 4, 117, 115, 101, 114, 4, 112, 97, 115, 115 };

            // Act
            Account account = new Account(serialized);

            // Assert
            Assert.AreEqual(_USERNAME, account.getUsername(), "Username wasn't parsed properly");
            Assert.AreEqual(_PASSWORD, account.getPassword(), "Password wasn't parsed properly");
        }

        [TestMethod]
        public void SONGSHARED011_Serialize_SongObject_byteArrayReturned()
        {
            // Arrange
            Song song = new Song(_SONGNAME, _SONGALBUM, _SONGARTIST, _SONGDURATION);

            // [NAMELENGTH 1byte] [NAME nBytes] | [ARTISTLENGTH 1byte] [ARTIST nBytes] | [DURATION 4bytes] | [ALBUMLENGTH 1byte] [ALBUM nBytes]
            byte[] expected = new byte[] { 4, 110, 97, 109, 101, 6, 97, 114, 116, 105, 115, 116, 20, 174, 71, 64, 5, 97, 108, 98, 117, 109 };

            // Act
            byte[] serialized = song.Serialize();

            // Assert
            Assert.IsTrue(Enumerable.SequenceEqual(expected, serialized));
        }

        [TestMethod]
        public void SONGSHARED012_Deserialize_byteArray_SongObjectCreated()
        {
            // Arrange

            // [NAMELENGTH 1byte][NAME nBytes] | [ARTISTLENGTH 1byte][ARTIST nBytes] | [DURATION 4bytes] | [ALBUMLENGTH 1byte][ALBUM nBytes]
            byte[] serialized = new byte[] { 4, 110, 97, 109, 101, 6, 97, 114, 116, 105, 115, 116, 20, 174, 71, 64, 5, 97, 108, 98, 117, 109 };

            // Act
            Song song = new Song(serialized);

            // Assert
            Assert.AreEqual(_SONGNAME, song.GetName(), "Name wasn't parsed properly");
            Assert.AreEqual(_SONGARTIST, song.GetArtist(), "Artist wasn't parsed properly");
            Assert.AreEqual(_SONGALBUM, song.GetAlbum(), "Album wasn't parsed properly");
            Assert.AreEqual(_SONGDURATION, song.GetDuration(), "Duration wasn't parsed properly");
        }

        [TestMethod]
        public void ALBUMSHARED009_Serialize_AlbumObject_byteArrayReturned()
        {
            Assert.Fail();
        }

        [TestMethod]
        public void ALBUMSHARED010_Deserialize_byteArray_AlbumCreated()
        {
            Assert.Fail();
        }

        [TestMethod]
        public void ARTISTSHARED007_Serialize_Artist_byteArrayReturned()
        {
            Assert.Fail();
        }

        [TestMethod]
        public void ARTISTSHARED008_Deserialize_byteArray_ArtistCreated()
        {
            Assert.Fail();
        }
    }
}