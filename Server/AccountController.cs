using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Server
{
    public class AccountController
    {
        Dictionary<string, Account> accounts;

        public AccountController()
        {
            accounts = new();
        }

        public bool AddAccount(string username, string password)
        {
            return accounts.TryAdd(username, new Account(username, password));
        }

        public Account FindAccount(string hash)
        {
            bool found = accounts.ContainsKey(hash);

            if (!found) { throw new KeyNotFoundException("Account not found."); }

            return accounts[hash];
        }

        public Dictionary<string, Account> ViewAccounts() { return accounts; }

        public bool DeleteAccount(string hash) { return accounts.Remove(hash); }

        public bool UpdateAccount(string hash, string criteria, object o)
        {
            bool actionDone = false;

            Account account = FindAccount(hash); // can throw a KeyNotFoundException exception

            if (criteria == "username")
            {
                account.setUsername((string)o);
                DeleteAccount(hash);

                if (AddAccount(account.getUsername(), account.getPassword())) // no duplicates
                    actionDone = true;
            }
            else if (criteria == "password")
            {
                account.setPassword((string)o);
                actionDone = true;
            }

            return actionDone;
        }

       public bool AuthAccount(string username, string password) 
       {
            bool auth;

            try
            {
                Account account = FindAccount(username);
                if (account.getPassword() == password)
                    auth = true;
                else
                    auth = false;
            }
            catch (KeyNotFoundException)
            {
                auth = false;
            }

            return auth;
       }
    }
}
