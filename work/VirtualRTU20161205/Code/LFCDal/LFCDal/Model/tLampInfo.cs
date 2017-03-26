using System;
using System.Collections.Generic;

namespace LFCDal.Model
{
    public class tLampInfo
    {
        public int LampNo { get; set; } // int, not null

        public string LampName { get; set; } // nvarchar(40), null

        public int? LampPower { get; set; } // int, null

        public byte? ElecPhase { get; set; } // tinyint, null

        public DateTime? RecDateTime { get; set; } // datetime, null

        public byte? CTUChNO { get; set; } // tinyint, null

        public byte? CTUChExtNo { get; set; } // tinyint, null

        public byte? HavePTUStatus { get; set; } // tinyint, null

        public byte? LampInstStatus { get; set; } // tinyint, null

        public string Memo { get; set; } // nvarchar(200), null

        public int? LampInitBrightness { get; set; } // int, null

        public byte? LampType { get; set; } // tinyint, null

        public byte? LampStatus { get; set; } // tinyint, null

        public byte? LampInitBrightnessStatus { get; set; } // tinyint, null

        public byte? LampEnableFlag { get; set; } // tinyint, null

        public byte? LampOutChNo { get; set; } // tinyint, null

        public byte? LampPTUChNo { get; set; } // tinyint, null

        public int? LampPTUNo { get; set; } // int, null

        public byte? LampCfgEnFlag { get; set; } // tinyint, null

        public string CTUID { get; set; } // char(40), not null
    }
}
