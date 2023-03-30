using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Numerics;
using System.Reflection.Metadata;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;


// [FILTER_LEN byte] [FILTER byte[]] [CONTEXT_HASH 8bytes] | [DATA_LEN 2byte] [DATA byte[]]

namespace Server
{
    public class SearchBody : PacketBody
    {
        UInt64 context;
        String filter;

        // Response information
        byte[] response;
        

        // Constructor for sending a client request
        public SearchBody(UInt64 context, String filter)
        {
            this.role = PacketBody.Role.Client;

            this.context = context;
            this.filter = filter;
            
            this.response = Encoding.ASCII.GetBytes("init");
        }

        // Constructor for building a server response from scratch
        public SearchBody(UInt64 context, String filter, byte[] songData) :
            this(context, filter)
        {
            this.appendServerResponse(songData);
        }

        // Method for allowing the server to respond to a received packet
        public void appendServerResponse(byte[] songData)
        {
            this.role = PacketBody.Role.Server;

            this.response = songData;
        }

        // Construct from serialized input
        public SearchBody(byte[] serialized)
        {
            // TODO Set State and timecode from serialized data
            role = Role.Client;
            int pointer = 0;

            // Deserialize filter length
            int len = serialized[pointer++];

            // Deserialize filter
            this.filter = Encoding.ASCII.GetString(serialized, pointer, len);
            pointer += len;

            // Deserialize the hash
            for (int i = 1; i <= sizeof(UInt64); i++)
            {
                this.context <<= 8;
                this.context += serialized[pointer++];
            }

            if (serialized.Length > pointer)
            {
                // Deserialize data length
                int dataLen = (serialized[pointer++] << 8) + serialized[pointer++];

                // Deserialize data
                this.response = new byte[dataLen];
                Array.Copy(serialized, pointer, this.response, 0, dataLen);
                role = Role.Server;
            }
        }

        // Serialize data
        override public byte[] Serialize()
        {
            int dataLen = role == Role.Client ? 0 : response.Length + sizeof(UInt16);
            byte[] serialized = new byte[this.filter.Length + dataLen + 1 + sizeof(UInt64)]; // maximum of 2 bytes

            int pointer = 0;

            // serialize the length of the filter
            serialized[pointer++] = (byte)this.filter.Length;

            // Serialize the filter
            byte[] filterBytes = Encoding.ASCII.GetBytes(this.filter);
            filterBytes.CopyTo(serialized, pointer);
            pointer += filterBytes.Length;

            // Serialize the hash
            for (int i = 1; i <= sizeof(UInt64); i++)
            {
                serialized[pointer++] = (byte)((byte)(this.context >> (sizeof(UInt64) - i) * 8) & 0xFF);
            }

            if (this.role == Role.Server)
            {
                // Serialize the data length
                for (int i = 1; i <= sizeof(UInt16); i++)
                {
                    serialized[pointer++] = (byte)((byte)((UInt16)this.response.Length >> (sizeof(UInt16) - i) * 8) & 0xFF);
                }

                // Serialize the response
                this.response.CopyTo(serialized, pointer);
                pointer += this.response.Length;
            }

            return serialized;
        }

        public UInt64 GetContext()
        {
            return this.context;
        }

        public String GetFilter()
        {
            return this.filter;
        }

        public byte[] GetResponse()
        {
            return this.response;
        }
    }
}
