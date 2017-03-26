using System;
using System.Collections.Generic;

namespace LFCDal.Model
{
    public class tAIStatus
    {
        public int AINo { get; set; } // int, not null

        public byte? AIType { get; set; } // tinyint, null

        public int? AIValue { get; set; } // int, null

        public DateTime? RecTime { get; set; } // datetime, null
    }
}
