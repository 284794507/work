using System;
using System.Collections.Generic;

namespace LFCDal.Model
{
    public class tLampAutoRunCfg
    {
        public int LampNo { get; set; } // int, not null

        public byte? CfgNo { get; set; } // tinyint, null

        public string PTUChByte { get; set; } // char(4), null

        public short? OptTime { get; set; } // smallint, null

        public byte? OptValue { get; set; } // tinyint, null

        public DateTime? CDateTime { get; set; } // datetime, null

        public byte? CmdType { get; set; } // tinyint, null

        public byte? DownloadTick { get; set; } // tinyint, null

        public byte? DLFailedTimes { get; set; } // tinyint, null

        public DateTime? DLDateTime { get; set; } // datetime, null

        public byte? CurRunTick { get; set; } // tinyint, null

        public DateTime? ShouldOptDateTime { get; set; } // datetime, null

        public string CTUID { get; set; } // char(40), not null
    }
}
