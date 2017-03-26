using System;
using System.Collections.Generic;

namespace LFCDal.Model
{
    public class tPTUFinded
    {
        public string FindPTUID { get; set; } // char(40), not null

        public DateTime FindDTime { get; set; } // datetime, not null

        public byte? isUpLoad { get; set; } // tinyint, null

        public DateTime? UpLoadDTime { get; set; } // datetime, null

        public byte? SvrProcessStatus { get; set; } // tinyint, null

        public string CTUID { get; set; } // char(40), not null
    }
}
