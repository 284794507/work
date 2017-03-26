using System;
using System.Collections.Generic;

namespace LFCDal.Model
{
    public class tPTUGrpCfg
    {
        public int PTUGrpNo { get; set; } // int, not null

        public string PTUGrpName { get; set; } // varchar(40), null

        public int PTUNo { get; set; } // int, not null

        public string PTUChByte { get; set; } // char(4), not null

        public DateTime CfgDateTime { get; set; } // datetime, not null

        public int Downloadtimes { get; set; } // int, not null

        public byte DownloadStatus { get; set; } // tinyint, not null

        public DateTime? DownloadDTime { get; set; } // datetime, null

        public string CTUID { get; set; } // char(40), not null
    }
}
