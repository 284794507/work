using System;
using System.Collections.Generic;

namespace LFCDal.Model
{
    public class tCTUTmpCtrlCfg
    {
        public byte CfgNo { get; set; } // tinyint, not null

        public DateTime? PreCtrlTime { get; set; } // datetime, null

        public byte? OptType { get; set; } // tinyint, null

        public byte? OptValue { get; set; } // tinyint, null

        public byte? CTUChNo { get; set; } // tinyint, null

        public byte? ValidFlag { get; set; } // tinyint, null

        public byte? IsOpenTimeFlag { get; set; } // tinyint, null

        public string CTUID { get; set; } // char(40), not null
    }
}
