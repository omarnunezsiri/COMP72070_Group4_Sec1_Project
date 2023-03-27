using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Numerics;
using System.Reflection;
using System.Reflection.Metadata;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;
using static Server.PacketHeader;

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

            this.data = new byte[0];
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
        public DownloadBody(byte[] serialized)
        {
            // TODO : Deserialize
            int pointer = 0;

            // Deserialize flags
            byte flags = serialized[pointer++];

            int mask = 0b01000000;
            this.type = Type.AlbumCover; // short form for 0b10000000 being Type.AlbumCOver
            this.type = ((flags & mask) != 0) ? Type.SongFile : this.type;


            // Deserialize the hash
            for (int i = 1; i <= sizeof(UInt64); i++)
            {
                this.hash <<= 8;
                this.hash += serialized[pointer++];
            }

            this.data = new byte[0];
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
            for (int i = 1; i <= sizeof(UInt64); i++)
            {
                serialized[pointer++] = (byte)((byte)(this.hash >> (sizeof(UInt64) - i) * 8) & 0xFF);
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

        public void SetType(Type _type)
        {
            this.type = _type;
        }

        public void SetHash(UInt64 _hash)
        {
            this.hash = _hash;
        }

        public void SetBlockIndex(UInt16 _blockIndex)
        {
            this.blockIndex = _blockIndex;
        }

        public void SetTotalBlocks(UInt16 _totalBlocks)
        {
            this.totalBlocks = _totalBlocks;
        }

        public void SetDataByteCount(UInt32 _dataByteCount)
        {
            this.dataByteCount = _dataByteCount;
        }

        public void SetData(byte[] _data)
        {
            this.data = _data;
        }

        public Type GetType()
        {
            return this.type;
        }

        public UInt64 GetHash()
        {
            return this.hash;
        }

        public UInt16 GetBlockIndex()
        {
            return this.blockIndex;
        }

        public UInt16 GetTotalBlocks()
        {
            return this.totalBlocks;
        }

        public UInt32 GetDataByteCount()
        {
            return this.dataByteCount;
        }

        public byte[] GetData()
        {
            return this.data;
        }
    }
}
