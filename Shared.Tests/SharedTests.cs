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
        [TestMethod]
        public void SONGSHARED001_SetName_placeholder_placeholderSet()
        {

        }

        [TestMethod]
        public void SONGSHARED002_SetArtist_placeholder_placeholderSet()
        {

        }

        [TestMethod]
        public void SONGSHARED003_SetDuration_placeholder_placeholderSet()
        {

        }

        [TestMethod]
        public void SONGSHARED004_SetAlbum_placeholder_placeholderSet()
        {

        }

        [TestMethod]
        public void SONGSHARED005_GetName_placeholder_returnsPlaceholder()
        {

        }

        [TestMethod]
        public void SONGSHARED006_GetArtist_placeholder_returnsPlaceholder()
        {

        }

        [TestMethod]
        public void SONGSHARED007_GetDuration_placeholder_returnsPlaceholder()
        {

        }

        [TestMethod]
        public void SONGSHARED008_GetAlbum_placeholder_returnsPlaceholder()
        {

        }
    }
}