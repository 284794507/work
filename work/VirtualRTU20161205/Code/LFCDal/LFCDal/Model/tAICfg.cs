using System;
using System.Collections.Generic;

namespace LFCDal.Model
{
    public class tAICfg
    {
        public int AINo { get; set; } // int, not null

        public byte? AIEnableFlag { get; set; } // tinyint, null

        public byte? AIType { get; set; } // tinyint, null

        public byte? AIChNumFlag { get; set; } // tinyint, null

        public byte? AIPhase { get; set; } // tinyint, null

        public int? OptOnMaxValue { get; set; } // int, null

        public int? OptOnMinValue { get; set; } // int, null

        public int? OptOffMaxValue { get; set; } // int, null

        public int? OptOffMinValue { get; set; } // int, null
    }
}
