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
}