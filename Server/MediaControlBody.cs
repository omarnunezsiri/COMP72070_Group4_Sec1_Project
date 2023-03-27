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
            NotApplicable,
            Playing,
            Paused,
            Idle
        }

        private State state;

        // Constructor for sending a client request
        public MediaControlBody(Action _action)
        {
            this.role = Role.Client;

            this.SetAction(_action);
            this.SetState(State.NotApplicable);
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

            this.SetState(_state);
        }

        // Construct from serialized input
        public MediaControlBody(byte[] serialized)
        {
            this.role = Role.Client;
            byte actionFlags = serialized[0];

            // Deserialize Action
            int mask = 0b01000000;
            this.action = Action.Play; // short form for 0b10000000 being Action.Play
            this.action = ((actionFlags & mask) != 0) ? Action.Pause : this.action; 
            mask >>= 1;
            this.action = ((actionFlags & mask) != 0) ? Action.Previous : this.action; 
            mask >>= 1;
            this.action = ((actionFlags & mask) != 0) ? Action.Skip : this.action;
            mask >>= 1;
            this.action = ((actionFlags & mask) != 0) ? Action.GetState : this.action;
            
            if (serialized.Length > 1)
            {
                this.role = Role.Server;
                byte stateFlags = serialized[1];

                // Deserialize State
                mask = 0b01000000; // reuse mask variable
                this.state = State.Playing; // short form for 0b10000000 being State.Playing
                this.state = ((stateFlags & mask) != 0) ? State.Paused : this.state;
                mask >>= 1;
                this.state = ((stateFlags & mask) != 0) ? State.Idle : this.state;
            }
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

        private void SetAction(Action _action)
        {
            this.action = _action;
        }

        private void SetState(State _state)
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
