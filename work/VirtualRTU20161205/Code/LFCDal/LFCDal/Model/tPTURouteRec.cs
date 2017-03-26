using System;
using System.Collections.Generic;

namespace LFCDal.Model
{
    public class tPTURouteRec
    {
        public byte PTUNo { get; set; } // tinyint, not null

        public byte TraStep { get; set; } // tinyint, not null

        public byte TraPTUNo1 { get; set; } // tinyint, not null

        public byte TraPTUNo2 { get; set; } // tinyint, not null

        public byte TraPTUNo3 { get; set; } // tinyint, not null

        public byte TraPTUNo4 { get; set; } // tinyint, not null

        public byte TraPTUNo5 { get; set; } // tinyint, not null

        public byte Status { get; set; } // tinyint, not null

        public byte SetWay { get; set; } // tinyint, not null

        public DateTime? UpdateTime { get; set; } // datetime, null

        public string CTUID { get; set; } // char(40), not null
    }
}
