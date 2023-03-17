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
    public class SyncBody : PacketBody
    {
        // Deciding not to use enums for greater flexibility in testing

        // [TIMECODE 8bytes] | [PLAYING] [PAUSED] [IDLE] [-] [-] [-] [-] [-]

        public enum State
        {
            Playing,
            Paused,
            Idle
        }

        // Response information
        UInt64 timecode; // milliseconds since the start
        State state;

        // Constructor for sending a client request
        public SyncBody()
        {
            this.role = PacketBody.Role.Client;
        }

        // Constructor for building a server response from scratch
        public SyncBody(UInt64 _timecode, State _state)
        {
            this.appendServerResponse(_timecode, _state);
        }

        // Method for allowing the server to respond to a received packet
        public void appendServerResponse(UInt64 _timecode, State _state)
        {
            this.role = PacketBody.Role.Server;

            this.state = _state;
            this.timecode = _timecode;
        }

        // Construct from serialized input
        public SyncBody(byte[] serialized)
        {

        }

        // Serialize data
        override public byte[] Serialize()
        {
            byte[] serialized = new byte[sizeof(UInt64) + 1]; // maximum of 2 bytes

            int pointer = 0;


            if (this.role == Role.Server)
            {
                // Serialize the hash
                for (int i = 0; i < sizeof(UInt64); i++)
                {
                    serialized[pointer++] = (byte)((byte)(this.timecode >> (sizeof(UInt64) - 1 - i) * 8) & 0xFF);
                }

                // Serialize the flags
                serialized[pointer] = base.SerializeBit(serialized[pointer], this.state == State.Playing);
                serialized[pointer] = base.SerializeBit(serialized[pointer], this.state == State.Paused);
                serialized[pointer] = base.SerializeBit(serialized[pointer], this.state == State.Idle);
                serialized[pointer++] <<= 5; // Shift the remaining 5 bits (align to MSB)
            }

            return serialized;
        }


    }
}
