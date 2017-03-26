using System;
using System.Collections.Generic;

namespace LFCDal.Model
{
    public class tPTUOptRec
    {
        public int PTUNo { get; set; } // int, not null

        public byte PTUChNo { get; set; } // tinyint, not null

        public byte OptValue { get; set; } // tinyint, not null

        public byte? isLocked { get; set; } // tinyint, null

        public DateTime? AddOptDateTime { get; set; } // datetime, null

        public byte? OptResult { get; set; } // tinyint, null

        public byte? OptTimes { get; set; } // tinyint, null

        public byte? OptCommValue { get; set; } // tinyint, null

        public DateTime? OptDateTime { get; set; } // datetime, null

        public string CTUID { get; set; } // char(40), not null
    }
}
