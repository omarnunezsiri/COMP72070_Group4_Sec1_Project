using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;

namespace Server
{
    public class PacketHeader
    {
        // What type of packet?
        // Section 1 - Packet type (Section 2 or 3)
        public enum Type
        {
            Account,
            Song
        }

        private Type _packetType;

        // Section 2 - Account Packet Type
        public enum  AccountAction {
            NotApplicable,
            SignUp,
            LogIn
        }

        private AccountAction _accountAction;

        // Section 3 - Song Packet Type
        public enum SongAction
        {
            NotApplicable,
            Sync,
            Media,
            Download,
            List
        }

        private SongAction _songAction;

        public PacketHeader(SongAction songAction)
        {
            this._packetType = Type.Song;
            this._accountAction = AccountAction.NotApplicable;

            this._songAction = songAction;
        }
        public PacketHeader(AccountAction accountAction)
        {
            this._packetType = Type.Account;
            this._songAction = SongAction.NotApplicable;

            this._accountAction = accountAction;
        }



        public PacketHeader(byte serialized)
        {
            int mask = 0b10000000;

            _packetType = ((serialized & mask) != 0) ? Type.Account : Type.Song;


            mask >>= 2;
            _accountAction = AccountAction.NotApplicable;
            _accountAction = ((serialized & mask) != 0) ? AccountAction.SignUp : _accountAction;
            mask >>= 1;
            _accountAction = ((serialized & mask) != 0) ? AccountAction.LogIn : _accountAction;
           
            mask >>= 1;
            _songAction = SongAction.NotApplicable;
            _songAction = ((serialized & mask) != 0) ? SongAction.Sync : _songAction;
            mask >>= 1;
            _songAction = ((serialized & mask) != 0) ? SongAction.Media : _songAction;
            mask >>= 1;
            _songAction = ((serialized & mask) != 0) ? SongAction.Download : _songAction;
            mask >>= 1;
            _songAction = ((serialized & mask) != 0) ? SongAction.List : _songAction;
        }

        // Serialize
        public byte Serialize()
        {
            byte serialized = new byte();

            byte one = 1;
            byte zero = 0;

            serialized += (this.GetPacketType() == Type.Account) ? one : zero;
            serialized <<= 1;

            serialized += (this.GetPacketType() == Type.Song) ? one : zero;
            serialized <<= 1;

            serialized += (this.GetAccountAction() == AccountAction.SignUp) ? one : zero;
            serialized <<= 1;

            serialized += (this.GetAccountAction() == AccountAction.LogIn) ? one : zero;
            serialized <<= 1;

            serialized += (this.GetSongAction() == SongAction.Sync) ? one : zero;
            serialized <<= 1;

            serialized += (this.GetSongAction() == SongAction.Media) ? one : zero;
            serialized <<= 1;

            serialized += (this.GetSongAction() == SongAction.Download) ? one : zero;
            serialized <<= 1;

            serialized += (this.GetSongAction() == SongAction.List) ? one : zero;
            //serialized <<= 1;

            return serialized;
        }

        // Setters
        public PacketHeader SetPacketType(Type packetType)
        {
            this._packetType = packetType;

            return this;
        }
        
        public PacketHeader SetAccountAction(AccountAction accountAction)
        {
            this._accountAction = accountAction;

            return this;
        }

        public PacketHeader SetSongAction(SongAction songAction)
        {
            this._songAction = songAction;

            return this;
        }

        // Getters
        public Type GetPacketType()
        {
            return this._packetType;
        }

        public AccountAction GetAccountAction()
        {
            return this._accountAction;
        }

        public SongAction GetSongAction()
        {
            return this._songAction;
        }
    }
}