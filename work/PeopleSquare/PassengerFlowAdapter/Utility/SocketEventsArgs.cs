﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Utility
{
    public class SocketEventsArgs:EventArgs
    {
        public string ObjID { get; set; }

        public byte[] Buffer { get; set; }
    }
}
