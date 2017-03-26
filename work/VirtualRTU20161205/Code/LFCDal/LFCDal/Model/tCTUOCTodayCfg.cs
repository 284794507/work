using System;
using System.Collections.Generic;

namespace LFCDal.Model
{
    public class tCTUOCTodayCfg
    {
        public byte CfgNo { get; set; } // tinyint, not null

        public DateTime PlanOptDateTime { get; set; } // datetime, not null

        public string CTUChNoByte { get; set; } // char(8), not null

        public byte OptValue { get; set; } // tinyint, not null

        public byte? CtrlStatus { get; set; } // tinyint, null

        public DateTime? OptDateTime { get; set; } // datetime, null

        public string Memo { get; set; } // nvarchar(200), null

        public string CTUID { get; set; } // char(40), not null
    }
}
