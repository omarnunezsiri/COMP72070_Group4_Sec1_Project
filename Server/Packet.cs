using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace Server
{
    public class Packet
    {
        private PacketHeader header;
        private PacketBody body;

        public Packet(PacketHeader header, PacketBody body)
        {
            this.header = header;
            this.body = body;
        }

        public Packet()
        {
            this.header = new PacketHeader();
            //
        }
    }
}
