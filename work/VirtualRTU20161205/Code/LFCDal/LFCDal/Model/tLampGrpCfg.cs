using System;
using System.Collections.Generic;

namespace LFCDal.Model
{
    public class tLampGrpCfg
    {
        public int GrpNo { get; set; } // int, not null

        public int LampNo { get; set; } // int, not null

        public DateTime? OptDatetime { get; set; } // datetime, null

        public byte? OptType { get; set; } // tinyint, null

        public byte? DownloadTick { get; set; } // tinyint, null

        public byte? DLFailedTimes { get; set; } // tinyint, null

        public DateTime? DLDateTime { get; set; } // datetime, null

        public string CTUID { get; set; } // char(40), not null
    }
}
