﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Server
{
    public class Account
    {
        private string _password;
        private string _username;

        public string getPassword() { return _password;  }
        public void setPassword(string pw) { _password = pw;  }

        public string getUsername() { return null; }
        public void setUsername(string username) { }
    }
}
