using System;
using System.Collections.Generic;

namespace LFCDal.Model
{
    public class tCTUOperationRec
    {
        public int? RecNo { get; set; } // int, null

        public byte? OperType { get; set; } // tinyint, null

        public string OperVal { get; set; } // char(12), null

        public DateTime? OperTime { get; set; } // datetime, null

        public string CTUID { get; set; } // char(40), not null
    }
}
