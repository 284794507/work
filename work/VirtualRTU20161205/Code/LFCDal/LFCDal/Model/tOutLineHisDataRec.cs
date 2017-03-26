using System;
using System.Collections.Generic;

namespace LFCDal.Model
{
    public class tOutLineHisDataRec
    {
        public byte OutLineNo { get; set; } // tinyint, not null

        public double? LineU { get; set; } // float, null

        public double? LineI { get; set; } // float, null

        public double? LineAP { get; set; } // float, null

        public double? LinePF { get; set; } // float, null

        public double? LineVP { get; set; } // float, null

        public DateTime GetDatetime { get; set; } // datetime, not null
    }
}
