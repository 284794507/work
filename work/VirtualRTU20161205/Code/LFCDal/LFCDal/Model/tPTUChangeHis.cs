using System;
using System.Collections.Generic;

namespace LFCDal.Model
{
    public class tPTUChangeHis
    {
        public string OldPTUID { get; set; } // char(40), not null

        public string NewPTUID { get; set; } // char(40), not null

        public DateTime? ChangeDateTime { get; set; } // datetime, null

        public byte? ChangeStatus { get; set; } // tinyint, null

        public string CTUID { get; set; } // char(40), not null
    }
}
