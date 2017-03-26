using System;
using System.Collections.Generic;

namespace LFCDal.Model
{
    public class tEventRecord
    {
        public int? ErcNo { get; set; } // int, null

        public byte? ErcType { get; set; } // tinyint, null

        public DateTime? ErcTime { get; set; } // datetime, null

        public byte? ErcReason { get; set; } // tinyint, null

        public byte? ErcOccurNo { get; set; } // tinyint, null

        public byte? ErcOccurType { get; set; } // tinyint, null

        public int? ErcOccurValue { get; set; } // int, null

        public int? ErcOccurValue2 { get; set; } // int, null

        public byte? IsReportedFlag { get; set; } // tinyint, null

        public byte? IsImportantFlag { get; set; } // tinyint, null
    }
}
