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
            throw new NotImplementedException();
        }

        public string getPassword() { return _password;  }
        public void setPassword(string pw) { _password = pw;  }

        public string getUsername() { return _username; }
        public void setUsername(string username) { _username = username; }

        public byte[] Serialize()
        {
            throw new NotImplementedException();
        }
    }
}
