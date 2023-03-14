using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Server
{
    public class PacketHeader
    {
        // What type of packet?
        // Section 1 - Packet type (Section 2 or 3)
        private bool account;
        private bool song;

        // Section 2 - Account Packet Type
        private bool signUp;
        private bool logIn;

        // Section 3 - Song Packet Type
        private bool sync;
        private bool media;
        private bool download;
        private bool list;

        public PacketHeader()
        { }

        public PacketHeader(bool account, bool song, bool signup, bool login, bool sync, bool media, bool download, bool list)
        {
            this.SetAccountBit(account);
            this.SetSongBit(song);
            this.SetSignUpBit(signup);
            this.SetLogInBit(login);
            this.SetSyncBit(sync);
            this.SetMediaBit(media);
            this.SetDownloadBit(download);
            this.SetListBit(list);
        }

        public PacketHeader(byte serialized)
        { }

        // Serialize
        public byte SerializeData()
        {
            byte serialized = new byte();

            byte one = 1;
            byte zero = 0;

            serialized += this.GetAccountBit() ? one : zero;
            serialized <<= 1;

            serialized += this.GetSongBit() ? one : zero;
            serialized <<= 1;

            serialized += this.GetSignUpBit() ? one : zero;
            serialized <<= 1;

            serialized += this.GetLogInBit() ? one : zero;
            serialized <<= 1;

            serialized += this.GetSyncBit() ? one : zero;
            serialized <<= 1;

            serialized += this.GetMediaBit() ? one : zero;
            serialized <<= 1;

            serialized += this.GetDownloadBit() ? one : zero;
            serialized <<= 1;

            serialized += this.GetListBit() ? one : zero;
            //serialized <<= 1;

            return serialized;
        }

        // Setters
        public void SetAccountBit(bool bit)
        {
            this.account = bit;
        }

        public void SetSongBit(bool bit)
        {
            this.song = bit;
        }

        public void SetSignUpBit(bool bit)
        {
            this.signUp = bit;
        }

        public void SetLogInBit(bool bit)
        {
            this.logIn = bit;
        }

        public void SetSyncBit(bool bit)
        {
            this.sync = bit;
        }

        public void SetMediaBit(bool bit)
        {
            this.media = bit;
        }

        public void SetDownloadBit(bool bit)
        {
            this.download = bit;
        }

        public void SetListBit(bool bit)
        {
            this.list = bit;
        }

        // Getters
        public bool GetAccountBit()
        {
            return this.account;
        }

        public bool GetSongBit()
        {
            return this.song;
        }

        public bool GetSignUpBit()
        {
            return this.signUp;
        }
        public bool GetLogInBit()
        {
            return this.logIn;
        }

        public bool GetSyncBit()
        {
            return this.sync;
        }

        public bool GetMediaBit()
        {
            return this.media;
        }

        public bool GetDownloadBit()
        {
            return this.download;
        }

        public bool GetListBit()
        {
            return this.list;
        }
    }
}