using System;
using System.Collections.Generic;

namespace LFCDal.Model
{
    public class tPTUDayCommStat
    {
        public int PTUNo { get; set; } // int, not null

        public int SuccCommCount { get; set; } // int, not null

        public int FailCommCount { get; set; } // int, not null

        public byte CurCommStatus { get; set; } // tinyint, not null

        public DateTime? LastCommDTime { get; set; } // datetime, null

        public byte? MaxCommValue { get; set; } // tinyint, null

        public byte? MinCommValue { get; set; } // tinyint, null

        public DateTime? UpdateDatetime { get; set; } // datetime, null

        public string CTUID { get; set; } // char(40), not null
    }
}
