using System;
using System.Collections.Generic;

namespace LFCDal.Model
{
    public class tLampPowerConDatum
    {
        public int? DataNo { get; set; } // int, null

        public int? LampNo { get; set; } // int, null

        public double? ElecCcon { get; set; } // float, null

        public DateTime? StartTime { get; set; } // datetime, null

        public DateTime? EndTime { get; set; } // datetime, null

        public double? fU { get; set; } // float, null

        public double? fI { get; set; } // float, null

        public byte? RtuStatus { get; set; } // tinyint, null

        public string CTUID { get; set; } // char(40), not null
    }
}
