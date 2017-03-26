using System;
using System.Collections.Generic;

namespace LFCDal.Model
{
    public class tPTUGrpOptRec
    {
        public int GrpOptRecNo { get; set; } // int, not null

        public byte PTUGrpNo { get; set; } // tinyint, not null

        public short OptValue { get; set; } // smallint, not null

        public byte isLocked { get; set; } // tinyint, not null

        public DateTime OptDateTime { get; set; } // datetime, not null

        public short OptTimes { get; set; } // smallint, not null

        public string CTUID { get; set; } // char(40), not null
    }
}
