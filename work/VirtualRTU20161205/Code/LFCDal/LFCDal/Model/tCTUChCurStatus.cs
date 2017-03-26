using System;
using System.Collections.Generic;

namespace LFCDal.Model
{
    public class tCTUChCurStatus
    {
        public byte CTUChNo { get; set; } // tinyint, not null

        public byte ChaStatus { get; set; } // tinyint, not null

        public string DOValue { get; set; } // char(4), null

        public string DIValue { get; set; } // char(8), null

        public byte? EnergyEfficient { get; set; } // tinyint, null

        public DateTime? RecDateTime { get; set; } // datetime, null
    }
}
