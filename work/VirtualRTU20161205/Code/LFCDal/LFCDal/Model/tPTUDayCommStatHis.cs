using System;
using System.Collections.Generic;

namespace LFCDal.Model
{
    public class tPTUDayCommStatHis
    {
        public DateTime theDate { get; set; } // datetime, not null

        public int PTUNo { get; set; } // int, not null

        public int SuccCommCount { get; set; } // int, not null

        public int FailCommCount { get; set; } // int, not null

        public byte CurCommStatus { get; set; } // tinyint, not null

        public DateTime LastCommDTime { get; set; } // datetime, not null

        public int MaxCommValue { get; set; } // int, not null

        public int MinCommValue { get; set; } // int, not null

        public DateTime UpdateDatetime { get; set; } // datetime, not null

        public byte? isUpLoaded { get; set; } // tinyint, null

        public DateTime? UpLoadDTime { get; set; } // datetime, null

        public string CTUID { get; set; } // char(40), not null
    }
}
