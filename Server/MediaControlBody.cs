using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Server
{
    public class MediaControlBody : PacketBody
    {
        // Action commands
        private bool play;
        private bool pause;
        private bool previous;
        private bool skip;
        private bool getState;

        // Current media state response
        private bool playing;
        private bool paused;
        private bool idle;

        private byte SerializeBit(byte _serialized, bool bit)
        {
            byte serialized = _serialized;

            byte one = 1;
            byte zero = 0;

            serialized <<= 1;
            serialized += bit ? one : zero;

            return serialized;
        }


        // Constructor for sending a client request
        public MediaControlBody(bool _play, bool _pause, bool _previous, bool _skip, bool _getState)
        {
            this.role = Role.Client;

            this.play = _play;
            this.pause = _pause;
            this.previous = _previous;
            this.skip = _skip;
            this.getState = _getState;
        }

        // Constructor for building a server response from scratch
        public MediaControlBody(bool _play, bool _pause, bool _previous, bool _skip, bool _getState, bool _playing, bool _paused, bool _idle)
            : this(_play, _pause, _previous, _skip, _getState)
        {
            this.appendServerResponse(_playing, _paused, _idle);
        }

        // Method for allowing the server to respond to a received packet
        public void appendServerResponse(bool _playing, bool _paused, bool _idle)
        {
            this.role = Role.Server;

            this.playing = _playing;
            this.paused = _paused;
            this.idle = _idle;
        }

        // Construct from serialized input
        public MediaControlBody(byte serialized)
        {

        }

        // Serialize data
        override public byte[] Serialize()
        {
            byte[] serialized = new byte[2]; // maximum of two bytes

            serialized[0] = this.SerializeBit(serialized[0], this.play);
            serialized[0] = this.SerializeBit(serialized[0], this.pause);
            serialized[0] = this.SerializeBit(serialized[0], this.previous);
            serialized[0] = this.SerializeBit(serialized[0], this.skip);
            serialized[0] = this.SerializeBit(serialized[0], this.getState);
            serialized[0] <<= 3; // Shift the remaining 3 bits (align to MSB)

            if (base.role == Role.Server)
            {
                serialized[1] = this.SerializeBit(serialized[1], this.playing);
                serialized[1] = this.SerializeBit(serialized[1], this.paused);
                serialized[1] = this.SerializeBit(serialized[1], this.idle);
                serialized[1] <<= 5; // Shift the remaining 5 bits (align to MSB)
            }

            return serialized;
        }
    }
}
