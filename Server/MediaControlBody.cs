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
        bool play;
        bool pause;
        bool previous;
        bool skip;
        bool getState;

        // Current media state response
        bool playing;
        bool paused;
        bool idle;

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
        override public byte[] SerializeData()
        {
            throw new NotImplementedException();
        }
    }
}
