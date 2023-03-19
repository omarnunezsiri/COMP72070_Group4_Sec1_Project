using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Server
{
    public class Account : ISerializable
    {
        private string _password;
        private string _username;

        public Account()
        {
            _password = string.Empty;
            _username = string.Empty;
        }

        public Account(string username, string password)
        {
            _password = password;
            _username = username;
        }

        public Account(byte[] accountBytes)
        {
            int offset = 0;
            byte len = accountBytes[offset++];

            _username = BitConverter.ToString(accountBytes, offset, len);
            offset += len;

            len = accountBytes[offset++];
            _password = BitConverter.ToString(accountBytes, offset, len);
        }

        public string getPassword() { return _password;  }
        public void setPassword(string pw) { _password = pw;  }

        public string getUsername() { return _username; }
        public void setUsername(string username) { _username = username; }

        public byte[] Serialize()
        {
            int offset = 0;

            // BYTE[USERNAME LENGTH] LENGTH[USERNAME] | BYTE[PASSWORD LENGTH] LENGTH[PASSWORD]
            byte[] accountBytes = new byte[2 * (sizeof(byte)) + _username.Length + _password.Length];

            byte len = Convert.ToByte(_username.Length);
            accountBytes[offset++] = len;

            byte[] usernameBytes = Encoding.ASCII.GetBytes(_username);
            usernameBytes.CopyTo(accountBytes, offset);

            offset += len;

            len = Convert.ToByte(_password.Length);
            accountBytes[offset++] = len;

            byte[] passwordBytes = Encoding.ASCII.GetBytes(_password);
            passwordBytes.CopyTo(accountBytes, offset);

            return accountBytes;
        }
    }
}
