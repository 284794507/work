using System;
using System.Collections.Generic;

namespace LFCDal.Model
{
    public class tDICfg
    {
        public int DINo { get; set; } // int, not null

        public byte? DIType { get; set; } // tinyint, null

        public byte? DIChNumFlag { get; set; } // tinyint, null

        public byte? DIPhase { get; set; } // tinyint, null

        public string CTUID { get; set; } // char(40), not null
    }
}
