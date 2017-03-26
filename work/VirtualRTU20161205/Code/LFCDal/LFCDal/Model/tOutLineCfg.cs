using System;
using System.Collections.Generic;

namespace LFCDal.Model
{
    public class tOutLineCfg
    {
        public byte OutLineNo { get; set; } // tinyint, not null

        public byte? ElecPhase { get; set; } // tinyint, null

        public byte? CTUChNo { get; set; } // tinyint, null

        public byte? CTUChExtNo { get; set; } // tinyint, null

        public int? CTValue { get; set; } // int, null

        public byte? CTStatus { get; set; } // tinyint, null

        public int? AllLampPower { get; set; } // int, null

        public double? IMaxRatio { get; set; } // float, null

        public double? IMinRatio { get; set; } // float, null
    }
}
