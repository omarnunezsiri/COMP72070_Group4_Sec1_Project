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

        /*public Packet()
         {
             this.header = new PacketHeader();
             //
         }*/

        public Packet(byte[] serialized)
        {
            this.header = new PacketHeader(serialized[0]);
            byte[] bodyS = new byte[serialized.Length - 1];

            Array.Copy(serialized, 1, bodyS, 0, bodyS.Length);

            // Check what type of packet this is. Options are [Account] and [Song]
            if (header.GetPacketType() == PacketHeader.Type.Account)
            {
                this.body = new Account(bodyS);
            }
            else // Type.Song
            {
                switch(header.GetSongAction())
                {
                    case PacketHeader.SongAction.Download:
                        this.body = new DownloadBody(bodyS);
                        break;
                    case PacketHeader.SongAction.Media:
                        this.body = new MediaControlBody(bodyS);
                        break;
                    case PacketHeader.SongAction.Sync:
                        this.body = new SyncBody(bodyS);
                        break;
                    case PacketHeader.SongAction.List:
                        this.body = new SearchBody(bodyS);
                        break;
                    case PacketHeader.SongAction.NotApplicable:
                        // This header has no information... ignore this packet
                        break;
                }
            }
        }

        public byte[] Serialize()
        {
            // Serialize header and body
            byte h = header.Serialize();
            byte[] b = body.Serialize();

            // Join header and body into new buffer
            byte[] join = new byte[b.Length + 1];
            join[0] = h;
            Array.Copy(b, 0, join, 1, b.Length);

            // Return joined buffer
            return join;

        }
    }
}
