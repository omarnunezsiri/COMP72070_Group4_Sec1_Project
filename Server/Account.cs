using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Server
{
    public class Account : PacketBody
    {
        private string _password;
        private string _username;
        
        public enum Status
        {
            NotApplicable, //0b00000000

            Success, // 0b10000000
            Failure // 0b01000000
        }

        Status _status;

        public Account()
        {
            _password = string.Empty;
            _username = string.Empty;
            _status = Status.NotApplicable;
        }

        public Account(string username, string password)
        {
            this.role = Role.Client;

            _password = password;
            _username = username;
            _status = Status.NotApplicable;
        }
        public Account(string username, string password, Status status)
        {
            this.role = Role.Server;

            _password = password;
            _username = username;
            _status = status;
        }

        public Account(byte[] accountBytes)
        {
            this.role = Role.Client;

            int offset = 0;
            byte len = accountBytes[offset++];

            _username = Encoding.ASCII.GetString(accountBytes, offset, len);
            offset += len;

            len = accountBytes[offset++];
            _password = Encoding.ASCII.GetString(accountBytes, offset, len);
            offset += len;

            if (accountBytes.Length > offset)
            {
                int mask = 0b10000000;
                _status = Status.NotApplicable; // 0b00000000
                _status = ((accountBytes[offset] & mask) != 0) ? Status.Success : _status; mask >>= 1;
                _status = ((accountBytes[offset] & mask) != 0) ? Status.Failure : _status; mask >>= 1;

                this.role = Role.Server;
            }
            
        }

        public string getPassword() { return _password;  }
        public void setPassword(string pw) { _password = pw;  }

        public string getUsername() { return _username; }
        public void setUsername(string username) { _username = username; }

        public Status getStatus() { return _status; }
        public void setStatus(Status status) {
            this.role = Role.Server;
            _status = status;
        }

        override public byte[] Serialize()
        {
            int offset = 0;

            // BYTE[USERNAME LENGTH] LENGTH[USERNAME] | BYTE[PASSWORD LENGTH] LENGTH[PASSWORD]
            int isServer = (this.role == Role.Server ? 1 : 0);
            byte[] accountBytes = new byte[Constants.AccountIndividualBytes * (sizeof(byte)) + _username.Length + _password.Length + isServer];

            byte len = Convert.ToByte(_username.Length);
            accountBytes[offset++] = len;

            byte[] usernameBytes = Encoding.ASCII.GetBytes(_username);
            usernameBytes.CopyTo(accountBytes, offset);

            offset += len;

            len = Convert.ToByte(_password.Length);
            accountBytes[offset++] = len;

            byte[] passwordBytes = Encoding.ASCII.GetBytes(_password);
            passwordBytes.CopyTo(accountBytes, offset);

            if (isServer == 1)
            {
                int flags = 0b11111111;
                flags = (_status == Status.NotApplicable ? 0b00000000 : flags);
                flags = (_status == Status.Success ? 0b10000000 : flags);
                flags = (_status == Status.Failure ? 0b01000000 : flags);

                accountBytes[accountBytes.Length - 1] = (byte)flags;
            }

            return accountBytes;
        }
    }
}
