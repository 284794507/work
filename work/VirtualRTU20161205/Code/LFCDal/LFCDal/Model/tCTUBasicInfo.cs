using System;
using System.Collections.Generic;

namespace LFCDal.Model
{
    public class tCTUBasicInfo
    {
        public string FactoryCode { get; set; } // char(8), null

        public string DevCode { get; set; } // char(16), null

        public string DevSoftVer { get; set; } // char(8), null

        public string DevSWReleaseDate { get; set; } // char(12), null

        public string DevCfgInfo { get; set; } // char(22), null

        public string DevProtocolVer { get; set; } // char(8), null

        public string DevHWVer { get; set; } // char(8), null

        public string DevPLCCode { get; set; } // char(16), null

        public string DevHWReleaseDate { get; set; } // char(12), null

        public byte? DevIsHaveMidRelay { get; set; } // tinyint, null

        public byte? DevType { get; set; } // tinyint, null

        public string DevFADate { get; set; } // char(12), null

        public byte? PwrSaveSwitchPos { get; set; } // tinyint, null

        public byte? PollStatus { get; set; } // tinyint, null

        public byte? IsFindPTU { get; set; } // tinyint, null
    }
}
