using System;
using System.Collections.Generic;

namespace LFCDal.Model
{
    public class tPTUInfo
    {
        public int PTUNo { get; set; } // int, not null

        public string PTUID { get; set; } // char(40), not null

        public string PTUChNoByte { get; set; } // char(4), null

        public byte? PTUCommFSM { get; set; } // tinyint, null

        public string PTUVer { get; set; } // char(8), null

        public DateTime? PTURecDateTime { get; set; } // datetime, null

        public int? PTUWorkStatus { get; set; } // int, null

        public byte? CTUChNo { get; set; } // tinyint, null

        public byte? CTUChExtNo { get; set; } // tinyint, null

        public byte? ElectPhase { get; set; } // tinyint, null

        public byte? FromPTUNo { get; set; } // tinyint, null

        public string Memo { get; set; } // nvarchar(200), null

        public byte? PTUValidFlag { get; set; } // tinyint, null

        public string PTUChValidFlag { get; set; } // char(4), null

        public int? PTUPoleNo { get; set; } // int, null

        public string CTUID { get; set; } // char(40), not null
    }
}
