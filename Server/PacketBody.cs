using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Server
{
    public abstract class PacketBody
    {
        internal enum Role
        {
            Server,
            Client
        }

        internal Role role;

        private PacketHeader header;

        public PacketBody()
        { }

        public PacketBody(PacketHeader header)
        { }

        public void useHeader(PacketHeader header)
        {
            this.header = header;
        }

        public PacketHeader getHeader()
        {
            return this.header;
        }

        public abstract byte[] SerializeData();
    }
}
