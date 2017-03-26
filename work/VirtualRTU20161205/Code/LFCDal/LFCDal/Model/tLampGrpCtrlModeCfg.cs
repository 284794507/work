using System;
using System.Collections.Generic;

namespace LFCDal.Model
{
    public class tLampGrpCtrlModeCfg
    {
        public int? GrpNo { get; set; } // int, null

        public byte? OptNo { get; set; } // tinyint, null

        public byte? OptMode { get; set; } // tinyint, null

        public byte? OptCtrlType { get; set; } // tinyint, null

        public byte? OptPwrSitchPos { get; set; } // tinyint, null

        public byte? OptRelTime { get; set; } // tinyint, null

        public string OptAbsTime { get; set; } // char(24), null

        public string CTUID { get; set; } // char(40), not null
    }
}
