
using Server;
using System.Drawing;
using System.Net.Sockets;

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

            FileHandler.ReadSongs(songController, "SearchTextFiles/" + Constants.SongsFile);

            /* Prep for byte array */
            Song song1 = songController.FindSong("testSong");
            Song song2 = songController.FindSong("testSong2");

            byte[] song1Bytes = song1.Serialize();
            byte[] song2Bytes = song2.Serialize();

            int offset = 0;
            byte[] bytes = new byte[song1Bytes.Length + song2Bytes.Length];
            song1Bytes.CopyTo(bytes, offset);

            offset += song1Bytes.Length;

            song2Bytes.CopyTo(bytes, offset);

            // Act
            Utils.PopulateSearchResults(bytes, songs);

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
            Utils.PopulateSearchResults(bytes, songs);

            // Assert
            Assert.AreEqual(ExpectedSize, songs.Count); // bitwise AND flags
        }
    }

    [TestClass]
    public class LogTests
    {
        private Logger logger = Logger.Instance; // singleton logger instance

        [TestMethod]
        public void CLOGUNIT001_LogSentLogIn_LoggedPacket()
        {
            // Arrange
            File.Delete(Constants.ClientLogsFile);
            Logger.SetFileName(Constants.ClientLogsFile);
            PacketHeader packetHeader = new(PacketHeader.AccountAction.LogIn);
            Account account = new Account("uname", "passw");

            Packet packet = new Packet(packetHeader, account);
            string expected = $"{DateTime.Now.ToString("dddd, dd MMMM yyyy hh:mm tt")} - Sent Log In Request to Server for username (uname), password (passw)\r\n";

            // Act
            logger.Log(packet, true);

            string actual = File.ReadAllText(Constants.ClientLogsFile);

            // Assert
            Assert.AreEqual(expected, actual);
        }

        [TestMethod]
        public void CLOGUNIT002_LogSentSignUp_LoggedPacket()
        {
            File.Delete(Constants.ClientLogsFile);
            Logger.SetFileName(Constants.ClientLogsFile);
            PacketHeader packetHeader = new(PacketHeader.AccountAction.SignUp);
            Account account = new Account("uname", "passw");

            Packet packet = new Packet(packetHeader, account);
            string expected = $"{DateTime.Now.ToString("dddd, dd MMMM yyyy hh:mm tt")} - Sent Sign Up Request to Server for username (uname), password (passw)\r\n";

            // Act
            logger.Log(packet, true);

            string actual = File.ReadAllText(Constants.ClientLogsFile);

            // Assert
            Assert.AreEqual(expected, actual);
        }

        [TestMethod]
        public void CLOGUNIT003_LogSentSearch_LoggedPacket()
        {
            // Arrange
            File.Delete(Constants.ClientLogsFile);
            Logger.SetFileName(Constants.ClientLogsFile);
            PacketHeader packetHeader = new(PacketHeader.SongAction.List);
            SearchBody searchBody = new(0x00000000, "filter");

            Packet packet = new Packet(packetHeader, searchBody);
            string expected = $"{DateTime.Now.ToString("dddd, dd MMMM yyyy hh:mm tt")} - Sent List/Search/Filter Request to Server: Filter (filter), Context (0)\r\n";
            
            // Act
            logger.Log(packet, true);

            string actual = File.ReadAllText(Constants.ClientLogsFile);

            // Assert
            Assert.AreEqual(expected, actual);
        }

        [TestMethod]
        public void CLOGUNIT004_LogSentDownload_LoggedPacket()
        {
            // Arrange
            File.Delete(Constants.ClientLogsFile);
            Logger.SetFileName(Constants.ClientLogsFile);
            PacketHeader packetHeader = new(PacketHeader.SongAction.Download);
            DownloadBody db = new(DownloadBody.Type.SongFile, "4660");

            Packet packet = new(packetHeader, db);
            string expected = $"{DateTime.Now.ToString("dddd, dd MMMM yyyy hh:mm tt")} - Sent Download Request to Server: Type (SongFile), Hash (4660)\r\n";
            
            // Act
            logger.Log(packet, true);

            string actual = File.ReadAllText(Constants.ClientLogsFile);

            // Assert
            Assert.AreEqual(expected, actual);
        }

        [TestMethod]
        public void CLOGUNIT006_LogSentMediaControl_LoggedPacket()
        {
            // Arrange
            File.Delete(Constants.ClientLogsFile);
            Logger.SetFileName(Constants.ClientLogsFile);
            PacketHeader packetHeader = new PacketHeader(PacketHeader.SongAction.Media);
            MediaControlBody mcb = new(MediaControlBody.Action.Previous);

            Packet packet = new(packetHeader, mcb);
            string expected = $"{DateTime.Now.ToString("dddd, dd MMMM yyyy hh:mm tt")} - Sent Media Request to Server: Action (Previous)\r\n";

            // Act
            logger.Log(packet, true);

            string actual = File.ReadAllText(Constants.ClientLogsFile);

            // Assert
            Assert.AreEqual(expected, actual);
        }

        [TestMethod]
        public void CLOGUNIT007_LogReceivedLogIn_LoggedPacket()
        {
            File.Delete(Constants.ClientLogsFile);
            Logger.SetFileName(Constants.ClientLogsFile);
            PacketHeader packetHeader = new(PacketHeader.AccountAction.LogIn);
            Account account = new Account("uname", "passw");
            account.setStatus(Account.Status.Success);

            Packet serverPacket = new Packet(packetHeader, account);

            Packet packet = new(serverPacket.Serialize());
            string expected = $"{DateTime.Now.ToString("dddd, dd MMMM yyyy hh:mm tt")} - Received Log In Response from Server for username (uname), password (passw) (Status: Success)\r\n";

            // Act
            logger.Log(packet, false);

            string actual = File.ReadAllText(Constants.ClientLogsFile);

            // Assert
            Assert.AreEqual(expected, actual);
        }

        [TestMethod]
        public void CLOGUNIT008_LogReceivedSignUp_LoggedPacket()
        {
            // Arrange
            File.Delete(Constants.ClientLogsFile);
            Logger.SetFileName(Constants.ClientLogsFile);
            PacketHeader packetHeader = new(PacketHeader.AccountAction.SignUp);
            Account account = new Account("uname", "passw");
            account.setStatus(Account.Status.Success);

            Packet serverPacket = new Packet(packetHeader, account);

            Packet packet = new(serverPacket.Serialize());
            string expected = $"{DateTime.Now.ToString("dddd, dd MMMM yyyy hh:mm tt")} - Received Sign Up Response from Server for username (uname), password (passw) (Status: Success)\r\n";

            // Act
            logger.Log(packet, false);

            string actual = File.ReadAllText(Constants.ClientLogsFile);

            // Assert
            Assert.AreEqual(expected, actual);
        }

        [TestMethod]
        public void CLOGUNIT009_LogReceivedSearch_LoggedPacket()
        {
            // Arrange
            File.Delete(Constants.ClientLogsFile);
            Logger.SetFileName(Constants.ClientLogsFile);
            PacketHeader packetHeader = new(PacketHeader.SongAction.List);
            SearchBody searchBody = new(0x00000000, "filter", new byte[420]);

            Packet serverPacket = new Packet(packetHeader, searchBody);
            Packet packet = new(serverPacket.Serialize());
            string expected = $"{DateTime.Now.ToString("dddd, dd MMMM yyyy hh:mm tt")} - Received List/Search/Filter Response from Server: Filter (filter), Context (0) Data Byte Count (420)\r\n";

            // Act
            logger.Log(packet, false);

            string actual = File.ReadAllText(Constants.ClientLogsFile);

            // Assert
            Assert.AreEqual(expected, actual);
        }

        [TestMethod]
        public void CLOGUNIT010_LogReceivedDownload_LoggedPacket()
        {
            // Arrange
            File.Delete(Constants.ClientLogsFile);
            Logger.SetFileName(Constants.ClientLogsFile);
            PacketHeader packetHeader = new(PacketHeader.SongAction.Download);
            DownloadBody db = new(DownloadBody.Type.SongFile, "4660");
            db.appendServerResponse(12, 24, 1234, new byte[1234]);

            Packet serverPacket = new(packetHeader, db);
            Packet packet = new(serverPacket.Serialize());
            string expected = $"{DateTime.Now.ToString("dddd, dd MMMM yyyy hh:mm tt")} - Received Download Response from Server: Type (SongFile), Hash (4660) Block Index (12), Total Blocks (24), Data Byte Count (1234)\r\n";

            // Act
            logger.Log(packet, false);

            string actual = File.ReadAllText(Constants.ClientLogsFile);

            // Assert
            Assert.AreEqual(expected, actual);
        }

        [TestMethod]
        public void CLOGUNIT011_LogReceivedSync_LoggedPacket()
        {
            // Arrange
            File.Delete(Constants.ClientLogsFile);
            Logger.SetFileName(Constants.ClientLogsFile);
            PacketHeader packetHeader = new PacketHeader(PacketHeader.SongAction.Sync);
            SyncBody sb = new(9831721, SyncBody.State.Paused);

            Packet serverPacket = new(packetHeader, sb);
            Packet packet = new(serverPacket.Serialize());
            string expected = $"{DateTime.Now.ToString("dddd, dd MMMM yyyy hh:mm tt")} - Received Sync Response from Server: Current play time (9831721), Stream state (Paused)\r\n";

            // Act
            logger.Log(packet, false);

            string actual = File.ReadAllText(Constants.ClientLogsFile);

            // Assert
            Assert.AreEqual(expected, actual);
        }

        [TestMethod]
        public void CLOGUNIT012_LogReceivedMediaControl_LoggedPacket()
        {
            // Arrange
            File.Delete(Constants.ClientLogsFile);
            Logger.SetFileName(Constants.ClientLogsFile);
            PacketHeader packetHeader = new PacketHeader(PacketHeader.SongAction.Media);
            MediaControlBody mcb = new(MediaControlBody.Action.Previous, MediaControlBody.State.Playing);

            Packet serverPacket = new(packetHeader, mcb);
            Packet packet = new(serverPacket.Serialize());
            string expected = $"{DateTime.Now.ToString("dddd, dd MMMM yyyy hh:mm tt")} - Received Media Response from Server: Action (Previous) Current Media State (Playing)\r\n";

            // Act
            logger.Log(packet, false);

            string actual = File.ReadAllText(Constants.ClientLogsFile);

            // Assert
            Assert.AreEqual(expected, actual);
        }

        [TestMethod]
        public void CLOGUNIT013_LogTimeSentPacket_LogContainsCurrentTime()
        {
            // Arrange
            File.Delete(Constants.ClientLogsFile);
            Logger.SetFileName(Constants.ClientLogsFile);
            PacketHeader packetHeader = new PacketHeader(PacketHeader.SongAction.Media);
            MediaControlBody mcb = new(MediaControlBody.Action.Previous);

            Packet packet = new(packetHeader, mcb);

            // Act
            logger.Log(packet, true);

            string actual = File.ReadAllText(Constants.ClientLogsFile);

            // Assert
            Assert.IsTrue(actual.Contains(DateTime.Now.ToString("dddd, dd MMMM yyyy hh:mm tt")));
        }

        [TestMethod]
        public void CLOGUNIT014_LogTimeReceivedPacket_LogContainsCurrentTime()
        {
            // Arrange
            File.Delete(Constants.ClientLogsFile);
            Logger.SetFileName(Constants.ClientLogsFile);
            PacketHeader packetHeader = new PacketHeader(PacketHeader.SongAction.Sync);
            SyncBody sb = new(9831721, SyncBody.State.Paused);

            Packet serverPacket = new(packetHeader, sb);
            Packet packet = new(serverPacket.Serialize());

            // Act
            logger.Log(packet, false);

            string actual = File.ReadAllText(Constants.ClientLogsFile);

            // Assert
            Assert.IsTrue(actual.Contains(DateTime.Now.ToString("dddd, dd MMMM yyyy hh:mm tt")));
        }

        [TestMethod]
        public void CLOGUNIT015_LogSentForgotPassword_LoggedPacket()
        {
            File.Delete(Constants.ClientLogsFile);
            Logger.SetFileName(Constants.ClientLogsFile);
            PacketHeader packetHeader = new(PacketHeader.AccountAction.LogIn);
            Account account = new Account("uname", "");

            Packet packet = new Packet(packetHeader, account);
            string expected = $"{DateTime.Now.ToString("dddd, dd MMMM yyyy hh:mm tt")} - Sent Forgot Password Request to Server for username (uname), password ()\r\n";

            // Act
            logger.Log(packet, true);

            string actual = File.ReadAllText(Constants.ClientLogsFile);

            // Assert
            Assert.AreEqual(expected, actual);
        }

        [TestMethod]
        public void CLOGUNIT016_LogReceivedForgotPassword_LoggedPacket()
        {
            File.Delete(Constants.ClientLogsFile);
            Logger.SetFileName(Constants.ClientLogsFile);
            PacketHeader packetHeader = new(PacketHeader.AccountAction.LogIn);
            Account account = new Account("uname", "");
            account.setStatus(Account.Status.Success);

            Packet serverPacket = new Packet(packetHeader, account);

            Packet packet = new(serverPacket.Serialize());
            string expected = $"{DateTime.Now.ToString("dddd, dd MMMM yyyy hh:mm tt")} - Received Forgot Password Response from Server for username (uname), password () (Status: Success)\r\n";

            // Act
            logger.Log(packet, false);

            string actual = File.ReadAllText(Constants.ClientLogsFile);

            // Assert
            Assert.AreEqual(expected, actual);
        }

        [TestMethod]
        public void CLOGUNIT017_LogSentResetPassword_LoggedPacket()
        {
            File.Delete(Constants.ClientLogsFile);
            Logger.SetFileName(Constants.ClientLogsFile);
            PacketHeader packetHeader = new(PacketHeader.AccountAction.NotApplicable);
            Account account = new Account("uname", "newpassword");

            Packet packet = new Packet(packetHeader, account);
            string expected = $"{DateTime.Now.ToString("dddd, dd MMMM yyyy hh:mm tt")} - Sent Reset Password Request to Server for username (uname), password (newpassword)\r\n";

            // Act
            logger.Log(packet, true);

            string actual = File.ReadAllText(Constants.ClientLogsFile);

            // Assert
            Assert.AreEqual(expected, actual);
        }

        [TestMethod]
        public void CLOGUNIT018_LogReceivedResetPassword_LoggedPacket()
        {
            File.Delete(Constants.ClientLogsFile);
            Logger.SetFileName(Constants.ClientLogsFile);
            PacketHeader packetHeader = new(PacketHeader.AccountAction.NotApplicable);
            Account account = new Account("uname", "newpassword");
            account.setStatus(Account.Status.Success);

            Packet serverPacket = new Packet(packetHeader, account);

            Packet packet = new(serverPacket.Serialize());
            string expected = $"{DateTime.Now.ToString("dddd, dd MMMM yyyy hh:mm tt")} - Received Reset Password Response from Server for username (uname), password (newpassword) (Status: Success)\r\n";

            // Act
            logger.Log(packet, false);

            string actual = File.ReadAllText(Constants.ClientLogsFile);

            // Assert
            Assert.AreEqual(expected, actual);
        }
    }
}