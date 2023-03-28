/*The System.Drawing namespace has functionalities that are only available on Windows, which leads to
  the CA1416 warning being raised. We have decided to disable it since the Silly Music Player App
  will be made for the Windows platform only. */
#pragma warning disable CA1416 

using Server;
using System.Drawing;
using System.Drawing.Imaging;
using System.Text;
using System.Xml.Linq;

namespace Shared.Tests
{
    [TestClass]
    public class AccountTests
    {
        private const string _Password = "passwordPlaceholder";
        private const string _Username = "usernamePlaceholder";
        
        [TestMethod]
        public void ACCSHARED001_GetPassword_placeholder_returnsPlaceholder()
        {
            // Arrange
            Account account = new Account();
            account.setPassword(_Password);

            // Act
            string actual = account.getPassword();

            // Assert
            Assert.AreEqual(_Password, actual);
        }

        [TestMethod]
        public void ACCSHARED002_GetUsername_placeholder_returnsPlaceholder()
        {
            // Arrange
            Account account = new Account();
            account.setUsername(_Username);

            // Act
            string actual = account.getUsername();

            // Assert
            Assert.AreEqual(_Username, actual);
        }

        [TestMethod]
        public void ACCSHARED003_SetPassword_placeholder_Assigned()
        {
            // Arrange
            Account account = new Account();

            // Act
            account.setPassword(_Password);
            string actual = account.getPassword();

            // Assert
            Assert.AreEqual(_Password, actual);
        }

        [TestMethod]
        public void ACCSHARED004_SetUsername_placeholder_Assigned()
        {
            // Arrange
            Account account = new Account();

            // Act
            account.setUsername(_Username);
            string actual = account.getUsername();

            // Assert
            Assert.AreEqual(_Username, actual);
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
            Account account = new(_Username, _Password);

            // Assert
            Assert.AreEqual(_Username, account.getUsername(), "Username not set to placeholder");
            Assert.AreEqual(_Password, account.getPassword(), "Password not set to placeholder");
        }
    }

    [TestClass]
    public class PacketTests
    {
        [TestMethod]
        public void PACKETUNIT001_SerializeHeader_CorrectBytesAllocated()
        {
            // [ACCOUNT] [SONG] | [SIGNUP] [LOGIN] | [SYNC] [MEDIA] [DOWNLOAD] [LIST]
            byte expected = 0b10100000;

            PacketHeader pHeader = new PacketHeader(PacketHeader.AccountAction.SignUp);

            Assert.AreEqual(expected, pHeader.Serialize());
        }

        [TestMethod]
        public void PACKETUNIT101_SerializeHeader_CorrectBytesAllocated()
        {
            // [ACCOUNT] [SONG] | [SIGNUP] [LOGIN] | [SYNC] [MEDIA] [DOWNLOAD] [LIST]
            byte expected = 0b01000010;

            PacketHeader pHeader = new PacketHeader(PacketHeader.SongAction.Download);

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
        public void PACKETUNIT003_SerializeMediaBody_PlayPaused()
        {
            // [PLAY] [PAUSE] [PREVIOUS] [SKIP/NEXT] [GETSTATE] [-] [-] [-] | [PLAYING] [PAUSED] [IDLE] [-] [-] [-] [-] [-]
            byte[] expected = new byte[2] { 0b10000000, 0b01000000 }; // 56 192

            MediaControlBody mBody = new MediaControlBody(MediaControlBody.Action.Play, MediaControlBody.State.Paused);

            byte[] serialized = mBody.Serialize();

            Assert.IsTrue(Enumerable.SequenceEqual(expected, serialized));
        }

        [TestMethod]
        public void PACKETUNIT103_SerializeMediaBody_PausePlaying()
        {
            // [PLAY] [PAUSE] [PREVIOUS] [SKIP/NEXT] [GETSTATE] [-] [-] [-] | [PLAYING] [PAUSED] [IDLE] [-] [-] [-] [-] [-]
            byte[] expected = new byte[2] { 0b01000000, 0b10000000 }; // 56 192

            MediaControlBody mBody = new MediaControlBody(MediaControlBody.Action.Pause, MediaControlBody.State.Playing);

            byte[] serialized = mBody.Serialize();

            Assert.IsTrue(Enumerable.SequenceEqual(expected, serialized));
        }

        [TestMethod]
        public void PACKETUNIT203_SerializeMediaBody_PreviousIdle()
        {
            // [PLAY] [PAUSE] [PREVIOUS] [SKIP/NEXT] [GETSTATE] [-] [-] [-] | [PLAYING] [PAUSED] [IDLE] [-] [-] [-] [-] [-]
            byte[] expected = new byte[2] { 0b00100000, 0b00100000 }; // 56 192

            MediaControlBody mBody = new MediaControlBody(MediaControlBody.Action.Previous, MediaControlBody.State.Idle);

            byte[] serialized = mBody.Serialize();

            Assert.IsTrue(Enumerable.SequenceEqual(expected, serialized));
        }

        [TestMethod]
        public void PACKETUNIT303_SerializeMediaBody_SkipPaused()
        {
            // [PLAY] [PAUSE] [PREVIOUS] [SKIP/NEXT] [GETSTATE] [-] [-] [-] | [PLAYING] [PAUSED] [IDLE] [-] [-] [-] [-] [-]
            byte[] expected = new byte[2] { 0b00010000, 0b01000000 }; // 56 192

            MediaControlBody mBody = new MediaControlBody(MediaControlBody.Action.Skip, MediaControlBody.State.Paused);

            byte[] serialized = mBody.Serialize();

            Assert.IsTrue(Enumerable.SequenceEqual(expected, serialized));
        }

        [TestMethod]
        public void PACKETUNIT403_SerializeMediaBody_GetStatePlaying()
        {
            // [PLAY] [PAUSE] [PREVIOUS] [SKIP/NEXT] [GETSTATE] [-] [-] [-] | [PLAYING] [PAUSED] [IDLE] [-] [-] [-] [-] [-]
            byte[] expected = new byte[2] { 0b00001000, 0b10000000 }; // 56 192

            MediaControlBody mBody = new MediaControlBody(MediaControlBody.Action.GetState, MediaControlBody.State.Playing);

            byte[] serialized = mBody.Serialize();

            Assert.IsTrue(Enumerable.SequenceEqual(expected, serialized));
        }

        [TestMethod]
        public void PACKETUNIT004_SerializeSyncBody_PlayingSerialize()
        {
            // [TIMECODE 8bytes] | [PLAYING] [PAUSED] [IDLE] [-] [-] [-] [-] [-]
            byte[] expected = new byte[9] { 0b00000000, 0b00000000, 0b00000000, 0b00000000, 0b00000000, 0b00000000, 0b00000000, 0b00000010, 0b10000000};

            SyncBody sBody = new SyncBody(2, SyncBody.State.Playing);

            byte[] serialized = sBody.Serialize();

            Assert.IsTrue(Enumerable.SequenceEqual(expected, serialized));
        }

        [TestMethod]
        public void PACKETUNIT104_SerializeSyncBody_PauseSerialize()
        {
            // [TIMECODE 8bytes] | [PLAYING] [PAUSED] [IDLE] [-] [-] [-] [-] [-]
            byte[] expected = new byte[9] { 0b00110000, 0b00001100, 0b00100000, 0b11000101, 0b00111110, 0b10001000, 0b11101000, 0b00111000, 0b01000000 };

            SyncBody sBody = new SyncBody(0x300C20C53E88E838, SyncBody.State.Paused);

            byte[] serialized = sBody.Serialize();

            Assert.IsTrue(Enumerable.SequenceEqual(expected, serialized));
        }

        [TestMethod]
        public void PACKETUNIT204_SerializeSyncBody_IdleSerialize()
        {
            // [TIMECODE 8bytes] | [PLAYING] [PAUSED] [IDLE] [-] [-] [-] [-] [-]
            byte[] expected = new byte[9] { 0b00110000, 0b00001100, 0b00100000, 0b11000101, 0b00111110, 0b10001000, 0b11101000, 0b00111000, 0b00100000 };

            SyncBody sBody = new SyncBody(0x300C20C53E88E838, SyncBody.State.Idle);

            byte[] serialized = sBody.Serialize();

            Assert.IsTrue(Enumerable.SequenceEqual(expected, serialized));
        }

        [TestMethod]
        public void PACKETUNIT005_SerializeAccountBody_CorrectUsernameLength()
        {
            // BYTE[USERNAME LENGTH] LENGTH[USERNAME] | BYTE[PASSWORD LENGTH] LENGTH[PASSWORD]
            String username = "myUsername";
            String password = "someWeirdPassword";

            Account acc = new Account(username, password);

            byte[] bytes = acc.Serialize();

            Assert.AreEqual(username.Length, bytes[0]);
        }

        [TestMethod]
        public void PACKETUNIT105_SerializeAccountBody_CorrectPasswordLength()
        {
            // BYTE[USERNAME LENGTH] LENGTH[USERNAME] | BYTE[PASSWORD LENGTH] LENGTH[PASSWORD]
            String username = "mySillyUsernameIsThisThingRightHere";
            String password = "someWeirdPassword";

            Account acc = new Account(username, password);

            byte[] bytes = acc.Serialize();

            Assert.AreEqual(password.Length, bytes[username.Length+1]);
        }

        [TestMethod]
        public void PACKETUNIT205_SerializeAccountBody_CorrectUsername()
        {
            // BYTE[USERNAME LENGTH] LENGTH[USERNAME] | BYTE[PASSWORD LENGTH] LENGTH[PASSWORD]
            String username = "myUsername";
            String password = "someWeirdPassword";

            Account acc = new Account(username, password);

            byte[] bytes = acc.Serialize();

            String str = Encoding.ASCII.GetString(bytes, 1, username.Length);

            Assert.AreEqual(username, str);
        }

        [TestMethod]
        public void PACKETUNIT305_SerializeAccountBody_CorrectPassword()
        {
            // BYTE[USERNAME LENGTH] LENGTH[USERNAME] | BYTE[PASSWORD LENGTH] LENGTH[PASSWORD]
            String username = "myUsername";
            String password = "someWeirdPassword";

            Account acc = new Account(username, password);

            byte[] bytes = acc.Serialize();

            String str = Encoding.ASCII.GetString(bytes, 2 + username.Length, password.Length);

            Assert.AreEqual(password, str);
        }

        [TestMethod]
        public void PACKETUNIT006_SerializeSearchBody_FilterMatch()
        {
            // [FILTER_LEN byte] [FILTER byte[]] [CONTEXT_HASH 8bytes] | [DATA_LEN 2byte] [DATA byte[]]
            String filter = "cool song please";
            UInt64 context = 0x0030120340210120;

            SearchBody sBody = new SearchBody(context, filter);

            byte[] serial = sBody.Serialize();

            Assert.AreEqual(filter.Length, serial[0]);
            Assert.AreEqual(filter, Encoding.ASCII.GetString(serial, 1, filter.Length));
        }

        [TestMethod]
        public void PACKETUNIT106_SerializeSearchBody_ContextHash()
        {
            // [FILTER_LEN byte] [FILTER byte[]] [CONTEXT_HASH 8bytes] | [DATA_LEN 2byte] [DATA byte[]]
            String filter = "cool song please";
            UInt64 context = 0x123456789ABCDEF0;
            /*
                        byte[] res = Encoding.ASCII.GetBytes("here is my response");*/

            SearchBody sBody = new SearchBody(context, filter);

            byte[] serial = sBody.Serialize();

            UInt64 result = 0;
            for (int i = 1; i <= 8; i++)
            {
                byte b = serial[filter.Length + i];
                result <<= 8;

                Console.WriteLine("shifted 0x{0:X2}", result);
                result += b;


                Console.WriteLine("      + 0x{0:X2}", b);
                Console.WriteLine("      = 0x{0:X2}\n", result);
            }


            Assert.AreEqual(context, result);
        }

        [TestMethod]
        public void PACKETUNIT206_SerializeSearchBody_Response()
        {
            // [FILTER_LEN byte] [FILTER byte[]] [CONTEXT_HASH 8bytes] | [DATA_LEN 2byte] [DATA byte[]]
            String filter = "cool song please";
            String serverResponse = "Cool_song.mp3";
            UInt64 context = 0x123456789ABCDEF0;

            byte[] res = Encoding.ASCII.GetBytes(serverResponse);

            SearchBody sBody = new SearchBody(context, filter, res);

            byte[] serial = sBody.Serialize();

            int position = 1 + serial[0] + sizeof(UInt64);

            // Deserialized variables
            int length = (serial[position++] << 8) + serial[position++];

            Assert.AreEqual(serverResponse.Length, length);// + serial[position]);
            Assert.AreEqual(serverResponse, Encoding.ASCII.GetString(serial, position, length));
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
            // Known header 
            PacketHeader pHeader = new PacketHeader(PacketHeader.AccountAction.SignUp);

            // Create from serialize
            PacketHeader newPHeader = new PacketHeader(pHeader.Serialize());


            Assert.AreEqual(pHeader.GetPacketType(), newPHeader.GetPacketType());
            Assert.AreEqual(pHeader.GetAccountAction(), newPHeader.GetAccountAction());
            Assert.AreEqual(pHeader.GetSongAction(), newPHeader.GetSongAction());
        }

        [TestMethod]
        public void PACKETUNIT010_DeserializeDownloadBody_RequestDeserialize()
        {
            UInt64 hash = 0x0102030405060708;
            DownloadBody.Type type = DownloadBody.Type.SongFile;
            DownloadBody dBody = new DownloadBody(type, hash);

            DownloadBody deserializedDBody = new DownloadBody(dBody.Serialize());

            Assert.AreEqual(type, deserializedDBody.GetType());
            Assert.AreEqual(hash, deserializedDBody.GetHash());
        }

        [TestMethod]
        public void PACKETUNIT110_DeserializeDownloadBody_ResponseDeserialize()
        {
            UInt64 hash = 0x0102030405060708;
            UInt16 blockIndex = 34;
            UInt16 totalBlocks = 4737;
            UInt32 dataByteCount = 3484727;
            byte[] data = Encoding.ASCII.GetBytes("Hello there! this is song data :)");
            DownloadBody dBody = new DownloadBody(DownloadBody.Type.SongFile, hash, blockIndex, totalBlocks, dataByteCount, data);

            DownloadBody deserializedDBody = new DownloadBody(dBody.Serialize());

            Assert.AreEqual(DownloadBody.Type.SongFile, deserializedDBody.GetType());
            Assert.AreEqual(hash, deserializedDBody.GetHash());
            Assert.AreEqual(blockIndex, deserializedDBody.GetBlockIndex());
            Assert.AreEqual(totalBlocks, deserializedDBody.GetTotalBlocks());
            Assert.IsTrue(Enumerable.SequenceEqual(data, deserializedDBody.GetData()));
        }

        [TestMethod]
        public void PACKETUNIT011_DeserializeMediaBody_Request()
        {
            MediaControlBody.Action action = MediaControlBody.Action.Skip;
            MediaControlBody.State state = MediaControlBody.State.NotApplicable;
            MediaControlBody mBody = new MediaControlBody(action); // Just give it action

            MediaControlBody dMBody = new MediaControlBody(mBody.Serialize());

            Assert.AreEqual(action, dMBody.GetAction());
            Assert.AreEqual(state, dMBody.GetState());
        }

        [TestMethod]
        public void PACKETUNIT111_DeserializeMediaBody_Response()
        {
            MediaControlBody.Action action = MediaControlBody.Action.GetState;
            MediaControlBody.State state = MediaControlBody.State.Playing;
            MediaControlBody mBody = new MediaControlBody(action, state);

            MediaControlBody dMBody = new MediaControlBody(mBody.Serialize());

            Assert.AreEqual(action, dMBody.GetAction());
            Assert.AreEqual(state, dMBody.GetState());
        }

        [TestMethod]
        public void PACKETUNIT012_DeserializeSyncBody_CorrectMembersAssigned()
        {
            UInt64 timecode = 0x300C20C53E88E838;
            SyncBody.State state = SyncBody.State.Playing;
            SyncBody sBody = new SyncBody(timecode, state);

            SyncBody dSBody = new SyncBody(sBody.Serialize());

            Assert.AreEqual(timecode, dSBody.GetTimecode());
            Assert.AreEqual(state, dSBody.GetState());
        }

        [TestMethod]
        public void PACKETUNIT013_DeserializeAccountBody_CorrectMembersAssigned()
        {
            String username = "myUsername";
            String password = "someWeirdPassword";
            Account acc = new Account(username, password);

            Account dAcc = new Account(acc.Serialize());

            Assert.AreEqual(username, dAcc.getUsername());
            Assert.AreEqual(password, dAcc.getPassword());
        }

        [TestMethod]
        public void PACKETUNIT014_DeserializeSearchBody_Request()
        {
            // [FILTER_LEN byte] [FILTER byte[]] [CONTEXT_HASH 8bytes] | [DATA_LEN 2byte] [DATA byte[]]
            String filter = "cool song please";
            UInt64 context = 0x123456789ABCDEF0;
            SearchBody sBody = new SearchBody(context, filter);

            SearchBody dSBody = new SearchBody(sBody.Serialize());

            Assert.AreEqual(filter, dSBody.GetFilter());
            Assert.AreEqual(context, dSBody.GetContext());
        }

        [TestMethod]
        public void PACKETUNIT114_DeserializeSearchBody_Response()
        {
            // [FILTER_LEN byte] [FILTER byte[]] [CONTEXT_HASH 8bytes] | [DATA_LEN 2byte] [DATA byte[]]
            String filter = "cool song please";
            byte[] serverResponse = Encoding.ASCII.GetBytes("Cool_song.mp3, potato_joe.mp3, dragon_song.mp3");
            UInt64 context = 0x123456789ABCDEF0;
            SearchBody sBody = new SearchBody(context, filter, serverResponse);

            SearchBody dSBody = new SearchBody(sBody.Serialize());

            Assert.AreEqual(filter, dSBody.GetFilter());
            Assert.AreEqual(context, dSBody.GetContext());
            Assert.IsTrue(Enumerable.SequenceEqual(serverResponse, dSBody.GetResponse()));
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
        private const int _AnotherY = 20;

        [TestMethod]
        public void UTILSSHARED001_CompareBitmaps_DifferentSize_ReturnsFalse()
        {
            // Arrange
            Bitmap a = new(_X, _Y);
            Bitmap b = new(_X, _AnotherY);

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
        private const string _SongName = "songNamePlaceholder";
        private const string _ArtistName = "artistPlaceholder";
        private const string _AlbumName = "albumPlaceholder";
        private const float _Duration = 1.0f;

        [TestMethod]
        public void SONGSHARED001_SetName_placeholder_placeholderSet()
        {
            // Arrange
            Song song = new();

            // Act
            song.SetName(_SongName);
            string actual = song.GetName();

            // Assert
            Assert.AreEqual(_SongName, actual);
        }

        [TestMethod]
        public void SONGSHARED002_SetArtist_placeholder_placeholderSet()
        {
            // Arrange
            Song song = new();

            // Act
            song.SetArtist(_ArtistName);
            string actual = song.GetArtist();

            // Assert
            Assert.AreEqual(_ArtistName, actual);
        }

        [TestMethod]
        public void SONGSHARED003_SetDuration_placeholder_placeholderSet()
        {
            // Arrange
            Song song = new();

            // Act
            song.SetDuration(_Duration);
            float actual = song.GetDuration();

            // Assert
            Assert.AreEqual(_Duration, actual);
        }

        [TestMethod]
        public void SONGSHARED004_SetAlbum_placeholder_placeholderSet()
        {
            // Arrange
            Song song = new();

            // Act
            song.SetAlbum(_AlbumName);
            string actual = song.GetAlbum();

            // Assert
            Assert.AreEqual(_AlbumName, actual);
        }

        [TestMethod]
        public void SONGSHARED005_GetName_placeholder_returnsPlaceholder()
        {
            // Arrange
            Song song = new();
            song.SetName(_SongName);

            // Act
            string actual = song.GetName();

            // Assert
            Assert.AreEqual(_SongName, actual);
        }

        [TestMethod]
        public void SONGSHARED006_GetArtist_placeholder_returnsPlaceholder()
        {
            // Arrange
            Song song = new();
            song.SetArtist(_ArtistName);

            // Act
            string actual = song.GetArtist();

            // Assert
            Assert.AreEqual(_ArtistName, actual);
        }

        [TestMethod]
        public void SONGSHARED007_GetDuration_placeholder_returnsPlaceholder()
        {
            // Arrange
            Song song = new();
            song.SetDuration(_Duration);

            // Act
            float actual = song.GetDuration();

            // Assert
            Assert.AreEqual(_Duration, actual);
        }

        [TestMethod]
        public void SONGSHARED008_GetAlbum_placeholder_returnsPlaceholder()
        {
            // Arrange
            Song song = new();
            song.SetAlbum(_AlbumName);

            // Act
            string actual = song.GetAlbum();

            // Assert
            Assert.AreEqual(_AlbumName, actual);
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
            Song song = new(_SongName, _AlbumName, _ArtistName, _Duration);

            // Assert
            Assert.AreEqual(_SongName, song.GetName(), "Song name not set to placeholder");
            Assert.AreEqual(_AlbumName, song.GetAlbum(), "Song album not set to placeholder");
            Assert.AreEqual(_ArtistName, song.GetArtist(), "Song artist not set to placeholder");
            Assert.AreEqual(_Duration, song.GetDuration(), "Song duration not set to placeholder");
        }
    }

    [TestClass]
    public class ArtistTests
    {
        private Bitmap _bitmap = (Bitmap)Image.FromFile("placeholder.png");
        private Bitmap _defaultBitmap = (Bitmap)Image.FromFile("default.png");
        private const string _ArtistName = "artistPlaceholder";

        [TestMethod]
        public void ARTISTSHARED001_SetName_placeholder_placeholderSet()
        {
            // Arrange
            Artist artist = new();

            // Act
            artist.SetName(_ArtistName);
            string actual = artist.GetName();

            // Assert
            Assert.AreEqual(_ArtistName, actual);
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
            artist.SetName(_ArtistName);

            // Act
            string actual = artist.GetName();

            // Assert
            Assert.AreEqual(_ArtistName, actual);
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
            Artist artist = new(_ArtistName, _bitmap);

            // Assert
            Assert.AreEqual(_ArtistName, artist.GetName(), "Artist name not set to placeholder");
            Assert.AreEqual(_bitmap, artist.GetImage(), "Artist image not set to placeholder");
        }
    }

    [TestClass]
    public class AlbumTests
    {
        private const string _AlbumName = "albumPlaceholder";
        private const string _ArtistName = "artistPlaceholder";
        private Bitmap _bitmap = (Bitmap)Image.FromFile("placeholder.png");
        private Bitmap _defaultBitmap = (Bitmap)Image.FromFile("default.png");

        [TestMethod]
        public void ALBUMSHARED001_SetName_placeholder_placeholderSet()
        {
            // Arrange
            Album album = new();

            // Act
            album.SetName(_AlbumName);
            string actual = album.GetName();

            // Assert
            Assert.AreEqual(_AlbumName, actual);
        }

        [TestMethod]
        public void ALBUMSHARED002_SetArtist_placeholder_placeholderSet()
        {
            // Arrange
            Album album = new();

            // Act
            album.SetArtist(_ArtistName);
            string actual = album.GetArtist();

            // Assert
            Assert.AreEqual(_ArtistName, actual);
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
            album.SetName(_AlbumName);

            // Act
            string actual = album.GetName();

            // Assert
            Assert.AreEqual(_AlbumName, actual);
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
            album.SetArtist(_ArtistName);

            // Act
            string actual = album.GetArtist();

            // Assert
            Assert.AreEqual(_ArtistName, actual);
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
            Album album = new(_AlbumName, _ArtistName, _bitmap);

            // Assert
            Assert.AreEqual(_AlbumName, album.GetName(), "Album name not set to placeholder");
            Assert.AreEqual(_ArtistName, album.GetArtist(), "Artist name not set to placeholder");
            Assert.AreEqual(_bitmap, album.GetImage(), "Album image not set to placeholder");
        }
    }

    [TestClass]
    public class FileHandlerTests
    {
        private string _Mp3FileName = "mymp3.mp3";
        private string _ImageName = "ubuntu.png";
        private string _OutputMp3 = "output.mp3";
        private string _OutputImage = "output.png";
        private string _UnexistingMp3 = "hello.mp3";
        private string _UnexistingImage = "wow.png";

        [TestMethod]
        public void FHSHARED001_writeMp3Bytes_byteArray_bytesWritten()
        {
            // Arrange
            byte[] mp3bytes = FileHandler.readMp3Bytes(_Mp3FileName); // assumes that readMp3Bytes works correctly
            File.Delete(_OutputMp3); // removes the file from dir if previous existence

            // Act
            bool bytesWritten = FileHandler.writeMp3Bytes(_OutputMp3, mp3bytes);

            // Assert
            Assert.IsTrue(bytesWritten);
        }

        [TestMethod]
        public void FHSHARED002_readMp3Bytes_byteArrayCreated()
        {
            // Arrange
            Type expectedType = typeof(byte[]);

            // Act
            var mp3bytes = FileHandler.readMp3Bytes(_Mp3FileName);

            // Assert
            Assert.AreEqual(expectedType, mp3bytes.GetType());
        }

        [TestMethod]
        public void FHSHARED003_writeImage_Bitmap_imageWritten()
        {
            // Arrange
            Bitmap original = FileHandler.readImageBytes(_ImageName); // assumes that readImageBytes works
            File.Delete(_OutputImage); // removes the file from dir if previous existence

            // Act
            bool imageWritten = FileHandler.writeImageBytes(_OutputImage, original);

            // Assert
            Assert.IsTrue(imageWritten);
        }

        [TestMethod]
        public void FHSHARED004_readImage_existingImage_imageCreated()
        {
            // Arrange
            Type expectedType = typeof(Bitmap);

            // Act
            var image = FileHandler.readImageBytes(_ImageName);

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
                var mp3bytes = FileHandler.readMp3Bytes(_UnexistingMp3);
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
            bool bytesWritten = FileHandler.writeMp3Bytes(_OutputMp3, mp3bytes);

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
                FileHandler.readImageBytes(_UnexistingImage);
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
            byte[] mp3bytes = FileHandler.readMp3Bytes(_Mp3FileName); // assumes that readMp3Bytes works correctly
            File.Delete(_OutputMp3); // removes the file from dir if previous existence

            // Act
            FileHandler.writeMp3Bytes(_OutputMp3, mp3bytes);

            // Assert
            Assert.IsTrue(File.Exists(_OutputMp3));
        }

        [TestMethod]
        public void FHSHARED009_readMp3Bytes_expectedLength()
        {
            // Arrange
            const long EXPECTEDLENGTH = 2505799;

            // Act
            var mp3bytes = FileHandler.readMp3Bytes(_Mp3FileName);

            // Assert
            Assert.AreEqual(EXPECTEDLENGTH, mp3bytes.Length, "Length of byte array is not the same as expected");
        }

        [TestMethod]
        public void FHSHARED010_writeMp3Bytes_byteArray_contentWritten()
        {
            // Arrange
            byte[] mp3bytes = FileHandler.readMp3Bytes(_Mp3FileName); // assumes that readMp3Bytes works correctly
            File.Delete(_OutputMp3); // removes the file from dir if previous existence

            // Act
            FileHandler.writeMp3Bytes(_OutputMp3, mp3bytes);

            byte[] anotherbytes = FileHandler.readMp3Bytes(_OutputMp3);

            // Assert
            Assert.IsTrue(mp3bytes.SequenceEqual(anotherbytes));
        }

        [TestMethod]
        public void FHSHARED011_writeImage_Bitmap_contentWritten()
        {
            // Arrange
            Bitmap bitmap = FileHandler.readImageBytes(_ImageName); // assumes that readImageBytes works correctly
            File.Delete(_OutputImage); // removes the file from dir if previous existence

            // Act
            FileHandler.writeImageBytes(_OutputImage, bitmap);

            Bitmap anotherBitmap = FileHandler.readImageBytes(_OutputImage);

            // Assert
            Assert.IsTrue(Utils.CompareBitmaps(bitmap, anotherBitmap));
        }

        [TestMethod]
        public void FHSHARED012_writeMp3Bytes_nullByteArray_returnsFalse()
        {
            // Arrange and Act
            bool bytesWritten = FileHandler.writeMp3Bytes(_OutputMp3, null);

            // Assert
            Assert.IsFalse(bytesWritten);
        }
    }

    [TestClass]
    public class SerializableTests
    {
        private const string _Username = "user";
        private const string _Password = "pass";
        private const string _SongName = "name";
        private const string _SongAlbum = "album";
        private const string _SongArtist = "artist";
        private const string _ArtistName = "aname";
        private const string _AlbumName = "album";
        private const string _AlbumArtist = "deborah";
        private Bitmap _artistImage = (Bitmap)Image.FromFile("second.jpg");
        private Bitmap _albumImage = (Bitmap)Image.FromFile("default.png");
        private const float _SongDuration = 3.12f;

        [TestMethod]
        public void ACCSHARED007_Serialize_AccountObject_byteArrayReturned()
        {
            // Arrange
            Account account = new Account(_Username, _Password);

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
            Assert.AreEqual(_Username, account.getUsername(), "Username wasn't parsed properly");
            Assert.AreEqual(_Password, account.getPassword(), "Password wasn't parsed properly");
        }

        [TestMethod]
        public void SONGSHARED011_Serialize_SongObject_byteArrayReturned()
        {
            // Arrange
            Song song = new Song(_SongName, _SongAlbum, _SongArtist, _SongDuration);

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
            Assert.AreEqual(_SongName, song.GetName(), "Name wasn't parsed properly");
            Assert.AreEqual(_SongArtist, song.GetArtist(), "Artist wasn't parsed properly");
            Assert.AreEqual(_SongAlbum, song.GetAlbum(), "Album wasn't parsed properly");
            Assert.AreEqual(_SongDuration, song.GetDuration(), "Duration wasn't parsed properly");
        }

        [TestMethod]
        public void ALBUMSHARED009_Serialize_AlbumObject_byteArrayReturned()
        {
            // Arrange
            Album album = new Album(_AlbumName, _AlbumArtist, _albumImage);

            byte[] nameBytes = new byte[] { 5, 97, 108, 98, 117, 109 };
            byte[] artistBytes = new byte[] { 7, 100, 101, 98, 111, 114, 97, 104 };
            byte[] imageBytes = Utils.GetBitmapBytes(album.GetImage());
            byte[] imageLengthBytes = BitConverter.GetBytes(imageBytes.Length);

            // [NAMELENGTH 1byte] [NAME nBytes] | [ARTISTLENGTH 1byte] [ARTIST nBytes] | [BITMAPLENGTH 4bytes] [BITMAP nBytes]
            byte[] expected = new byte[nameBytes.Length + artistBytes.Length + sizeof(int) + imageBytes.Length];
            nameBytes.CopyTo(expected, 0);
            artistBytes.CopyTo(expected, nameBytes.Length);
            imageLengthBytes.CopyTo(expected, nameBytes.Length + artistBytes.Length);
            imageBytes.CopyTo(expected, nameBytes.Length + artistBytes.Length + sizeof(int));

            // Act
            byte[] serialized = album.Serialize();

            // Assert
            Assert.IsTrue(Enumerable.SequenceEqual(expected, serialized));
        }

        [TestMethod]
        public void ALBUMSHARED010_Deserialize_byteArray_AlbumCreated()
        {
            // Arrange
            byte[] nameBytes = new byte[] { 5, 97, 108, 98, 117, 109 };
            byte[] artistBytes = new byte[] { 7, 100, 101, 98, 111, 114, 97, 104 };
            byte[] imageBytes = Utils.GetBitmapBytes(_albumImage);
            byte[] imageLengthBytes = BitConverter.GetBytes(imageBytes.Length);

            // [NAMELENGTH 1byte] [NAME nBytes] | [ARTISTLENGTH 1byte] [ARTIST nBytes] | [BITMAPLENGTH 4bytes] [BITMAP nBytes]
            byte[] serialized = new byte[nameBytes.Length + artistBytes.Length + sizeof(int) + imageBytes.Length];
            nameBytes.CopyTo(serialized, 0);
            artistBytes.CopyTo(serialized, nameBytes.Length);
            imageLengthBytes.CopyTo(serialized, nameBytes.Length + artistBytes.Length);
            imageBytes.CopyTo(serialized, nameBytes.Length + artistBytes.Length + sizeof(int));

            // Act
            Album album = new(serialized);

            // Assert
            Assert.AreEqual(_AlbumName, album.GetName(), "Name not parsed properly");
            Assert.AreEqual(_AlbumArtist, album.GetArtist(), "Artist not parsed properly");
            Assert.IsTrue(Utils.CompareBitmaps(_albumImage, album.GetImage()), "Image not parsed properly");
        }

        [TestMethod]
        public void ARTISTSHARED007_Serialize_Artist_byteArrayReturned()
        {
            // Arrange
            Artist artist = new Artist(_ArtistName, _artistImage);

            byte[] nameBytes = new byte[] { 5, 97, 110, 97, 109, 101 };
            byte[] bitmapBytes = Utils.GetBitmapBytes(artist.GetImage());
            byte[] bitmapLength = BitConverter.GetBytes(bitmapBytes.Length);

            // [NAMELENGTH 1byte] [NAME nBytes] | [BITMAPLENGTH 4bytes] [BITMAP nBytes]
            byte[] expected = new byte[nameBytes.Length + sizeof(int) + bitmapBytes.Length];
            nameBytes.CopyTo(expected, 0);
            bitmapLength.CopyTo(expected, nameBytes.Length);
            bitmapBytes.CopyTo(expected, nameBytes.Length + sizeof(int));

            // Act
            byte[] serialized = artist.Serialize();

            // Assert
            Assert.IsTrue(Enumerable.SequenceEqual(expected, serialized));
        }

        [TestMethod]
        public void ARTISTSHARED008_Deserialize_byteArray_ArtistCreated()
        {
            // Arrange
            byte[] nameBytes = new byte[] { 5, 97, 110, 97, 109, 101 };
            byte[] bitmapBytes = Utils.GetBitmapBytes(_artistImage);
            byte[] bitmapLengthBytes = BitConverter.GetBytes(bitmapBytes.Length);

            // [NAMELENGTH 1byte] [NAME nBytes] | [BITMAPLENGTH 4bytes] [BITMAP nBytes]
            byte[] serialized = new byte[nameBytes.Length + sizeof(int) + bitmapBytes.Length];
            nameBytes.CopyTo(serialized, 0);
            bitmapLengthBytes.CopyTo(serialized, nameBytes.Length);
            bitmapBytes.CopyTo(serialized, nameBytes.Length + sizeof(int));

            // Act
            Artist artist = new(serialized);

            // Assert
            Assert.AreEqual(_ArtistName, artist.GetName(), "Name not parsed properly");
            Assert.IsTrue(Utils.CompareBitmaps(_artistImage, artist.GetImage()), "Image not parsed properly");
        }
    }
}