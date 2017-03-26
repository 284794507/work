using System;
using System.Collections.Generic;

namespace LFCDal.Model
{
    public class tPTUChLampCfg
    {
        public int PTUNo { get; set; } // int, not null

        public byte PTUChNo { get; set; } // tinyint, not null

        public int LampNo { get; set; } // int, not null

        public DateTime? RecDateTime { get; set; } // datetime, null

        public string CTUID { get; set; } // char(40), not null
    }
}
