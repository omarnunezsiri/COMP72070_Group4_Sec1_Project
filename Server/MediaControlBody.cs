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
        // TODO: Only one true from each section

        // [PLAY] [PAUSE] [PREVIOUS] [SKIP/NEXT] [GETSTATE] [-] [-] [-] | [PLAYING] [PAUSED] [IDLE] [-] [-] [-] [-] [-]

        // Action commands
        public enum Action
        {
            Play,
            Pause,
            Previous,
            Skip,
            GetState
        };

        private Action action;


        // Current media state response
        public enum State
        {
            Playing,
            Paused,
            Idle
        }

        private State state;

        // Constructor for sending a client request
        public MediaControlBody(Action _action)
        {
            this.role = Role.Client;

            this.action = _action;
        }

        // Constructor for building a server response from scratch
        public MediaControlBody(Action _action, State _state)
            : this(_action)
        {
            this.appendServerResponse(_state);
        }

        // Method for allowing the server to respond to a received packet
        public void appendServerResponse(State _state)
        {
            this.role = Role.Server;

            this.state = _state;
        }

        // Construct from serialized input
        public MediaControlBody(byte serialized)
        {

        }

        // Serialize data
        override public byte[] Serialize()
        {
            byte[] serialized = new byte[2]; // maximum of two bytes

            serialized[0] = base.SerializeBit(serialized[0], this.action == Action.Play);
            serialized[0] = base.SerializeBit(serialized[0], this.action == Action.Pause);
            serialized[0] = base.SerializeBit(serialized[0], this.action == Action.Previous);
            serialized[0] = base.SerializeBit(serialized[0], this.action == Action.Skip);
            serialized[0] = base.SerializeBit(serialized[0], this.action == Action.GetState);
            serialized[0] <<= 3; // Shift the remaining 3 bits (align to MSB)

            if (base.role == Role.Server)
            {
                serialized[1] = base.SerializeBit(serialized[1], this.state == State.Playing);
                serialized[1] = base.SerializeBit(serialized[1], this.state == State.Paused);
                serialized[1] = base.SerializeBit(serialized[1], this.state == State.Idle);
                serialized[1] <<= 5; // Shift the remaining 5 bits (align to MSB)
            }

            return serialized;
        }

        public void SetAction(Action _action)
        {
            this.action = _action;
        }

        public void SetState(State _state)
        {
            this.state = _state;
        }

        public Action GetAction()
        {
            return this.action;
        }

        public State GetState()
        {
            return this.state;
        }
        
    }
}
