﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CTM_Route_Utility
{
    public class SocketEventsArgs : EventArgs
    {
        public string DeviceID { get; set; }

        public byte[] Buffer { get; set; }
    }
}
