﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Server
{
    public abstract class PacketBody
    {
        public PacketBody()
        { }

        public PacketBody(PacketHeader header)
        { }

        public abstract void UseHeader(PacketHeader header);
    }
}