using System;
using System.Collections.Generic;

namespace LFCDal.Model
{
    public class tCTUChOptChangeHis
    {
        public byte CTUChNo { get; set; } // tinyint, not null

        public byte? ChaStatus { get; set; } // tinyint, null

        public string DIValue { get; set; } // char(8), not null

        public string DOValue { get; set; } // char(8), not null

        public byte EnergyEfficient { get; set; } // tinyint, not null

        public DateTime RecDateTime { get; set; } // datetime, not null
    }
}
