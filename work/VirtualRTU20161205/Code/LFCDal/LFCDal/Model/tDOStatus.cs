using System;
using System.Collections.Generic;

namespace LFCDal.Model
{
    public class tDOStatus
    {
        public int DONo { get; set; } // int, not null

        public byte? DOType { get; set; } // tinyint, null

        public byte? DOValue { get; set; } // tinyint, null

        public byte? DOTimeFlag { get; set; } // tinyint, null

        public byte? EmgencyFlag { get; set; } // tinyint, null

        public string CTUID { get; set; } // char(40), not null
    }
}
