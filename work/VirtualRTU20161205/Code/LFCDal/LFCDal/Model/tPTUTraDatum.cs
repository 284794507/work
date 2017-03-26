using System;
using System.Collections.Generic;

namespace LFCDal.Model
{
    public class tPTUTraDatum
    {
        public byte PTUNo { get; set; } // tinyint, not null

        public byte? DC1 { get; set; } // tinyint, null

        public byte? DC2 { get; set; } // tinyint, null

        public byte? DC3 { get; set; } // tinyint, null

        public byte? DC4 { get; set; } // tinyint, null

        public byte? DC5 { get; set; } // tinyint, null

        public byte? DC6 { get; set; } // tinyint, null

        public byte? DC7 { get; set; } // tinyint, null

        public byte? DC8 { get; set; } // tinyint, null

        public byte? DC9 { get; set; } // tinyint, null

        public byte? DC10 { get; set; } // tinyint, null

        public byte? DC11 { get; set; } // tinyint, null

        public byte? DC12 { get; set; } // tinyint, null

        public DateTime? UpdateTime { get; set; } // datetime, null
        
        public string CTUID { get; set; } // char(40), not null
    }
}
