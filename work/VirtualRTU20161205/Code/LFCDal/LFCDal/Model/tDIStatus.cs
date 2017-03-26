using System;
using System.Collections.Generic;

namespace LFCDal.Model
{
    public class tDIStatus
    {
        public int DINo { get; set; } // int, not null

        public byte? DIValue { get; set; } // tinyint, null

        public string CTUID { get; set; } // char(40), not null
    }
}
