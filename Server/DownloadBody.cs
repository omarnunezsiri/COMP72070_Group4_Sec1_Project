using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Numerics;
using System.Reflection.Metadata;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;

namespace Server
{
    public class DownloadBody : PacketBody
    {
        // Deciding not to use enums for greater flexibility in testing

        // [ISALBUMCOVER] [ISSONGFILE] [-] [-] [-] [-] [-] [-] | [ITEM HASH 8Bytes] [Block index 2bytes] [Total Block count 2bytes] [Data Bytecount 4bytes] [Data nbytes]

        public enum Type
        {
            AlbumCover,
            SongFile
        }

        // Query information
        Type type;
        private UInt64 hash;

        // Server Response
        private UInt16 blockIndex;
        private UInt16 totalBlocks;
        private UInt32 dataByteCount;
        private byte[] data; 

        // Constructor for sending a client request
        public DownloadBody(Type _type, UInt64 _hash)
        {
            this.role = PacketBody.Role.Client;

            this.type = _type;
            this.hash = _hash;
        }

        // Constructor for building a server response from scratch
        public DownloadBody(Type _type, UInt64 _hash, UInt16 _blockIndex, UInt16 _totalBlocks, UInt32 _dataByteCount, byte[] _data)
            : this(_type, _hash)
        {
            this.appendServerResponse(_blockIndex, _totalBlocks, _dataByteCount, _data);
        }

        // Method for allowing the server to respond to a received packet
        public void appendServerResponse(UInt16 _blockIndex, UInt16 _totalBlocks, UInt32 _dataByteCount, byte[] _data)
        {
            this.role = PacketBody.Role.Server;

            this.blockIndex = _blockIndex;
            this.totalBlocks = _totalBlocks;
            this.dataByteCount = _dataByteCount;
            this.data = _data;
        }

        // Construct from serialized input
        public DownloadBody(byte serialized)
        {

        }

        // Serialize data
        override public byte[] Serialize()
        {
            int dataLen = this.data == null ? 0 : this.data.Length;
            byte[] serialized = new byte[17 + dataLen]; // maximum of 17 bytes (And then data bytes)

            int pointer = 0;

            // Serialize the flags
            serialized[pointer] = base.SerializeBit(serialized[pointer], this.type == Type.AlbumCover);
            serialized[pointer] = base.SerializeBit(serialized[pointer], this.type == Type.SongFile);
            serialized[pointer++] <<= 6; // Shift the remaining 6 bits (align to MSB)

            // Serialize the hash
            for (int i = 0; i < sizeof(UInt64); i++)
            {
                serialized[pointer++] = (byte)((byte)(this.hash >> (7 - i) * 8) & 0xFF);
            }
            
            if (this.role == Role.Server)
            {
                // Serialize block index
                serialized[pointer++] = (byte)((byte)(this.blockIndex >> 1 * 8) & 0xFF);
                serialized[pointer++] = (byte)((byte)(this.blockIndex >> 0 * 8) & 0xFF);

                // Serialize total block count
                serialized[pointer++] = (byte)((byte)(this.totalBlocks >> 1 * 8) & 0xFF);
                serialized[pointer++] = (byte)((byte)(this.totalBlocks >> 0 * 8) & 0xFF);

                // Serialize data byte count
                serialized[pointer++] = (byte)((byte)(this.dataByteCount >> 3 * 8) & 0xFF);
                serialized[pointer++] = (byte)((byte)(this.dataByteCount >> 2 * 8) & 0xFF);
                serialized[pointer++] = (byte)((byte)(this.dataByteCount >> 1 * 8) & 0xFF);
                serialized[pointer++] = (byte)((byte)(this.dataByteCount >> 0 * 8) & 0xFF);

                // serialize data
                this.data.CopyTo(serialized, pointer);
            }

            return serialized;
        }

       
    }
}
