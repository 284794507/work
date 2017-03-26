using System;
using System.Collections.Generic;

namespace LFCDal.Model
{
    public class tCabInfo
    {
        public string CabID { get; set; } // char(40), not null

        public string CabName { get; set; } // nvarchar(40), null

        public decimal? XLat { get; set; } // decimal(10,6), null

        public decimal? YLong { get; set; } // decimal(10,6), null

        public int? PoleCount { get; set; } // int, null

        public int? LampCount { get; set; } // int, null

        public int? LampPower { get; set; } // int, null

        public byte? HaveCTUStatus { get; set; } // tinyint, null

        public string Memo { get; set; } // nvarchar(200), null
    }
}
