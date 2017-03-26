using System;
using System.Collections.Generic;

namespace LFCDal.Model
{
    public class tLampStatus
    {
        public int LampNo { get; set; } // int, not null

        public byte? LampStatus { get; set; } // tinyint, null

        public byte? LampRunStatus { get; set; } // tinyint, null

        public long? LampVolt { get; set; } // bigint, null

        public string CTUID { get; set; } // char(40), not null
    }
}
