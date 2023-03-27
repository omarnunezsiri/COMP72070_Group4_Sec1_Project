using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Server
{
    public abstract class PacketBody : ISerializable
    {
        internal enum Role
        {
            Server,
            Client
        }

        internal Role role;

        private PacketHeader header;

        protected byte SerializeBit(byte _serialized, bool bit)
        {
            byte serialized = _serialized;

            byte one = 1;
            byte zero = 0;

            serialized <<= 1;
            serialized += bit ? one : zero;

            return serialized;
        }

        public PacketBody()
        { }
        

        public PacketHeader getHeader()
        {
            return this.header;
        }



        public abstract byte[] Serialize();
    }
}
