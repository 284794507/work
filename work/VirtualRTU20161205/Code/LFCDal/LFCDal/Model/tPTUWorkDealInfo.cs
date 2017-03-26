using System;
using System.Collections.Generic;
using System.Runtime.Serialization;

namespace LFCDal.Model
{
    public class tPTUWorkDealInfo
    {
        public int? CMDRecNo { get; set; } // int, null

        public byte? CMDLevel { get; set; } // tinyint, null

        public string CMDData { get; set; } // varchar(256), null

        public DateTime? CreateTime { get; set; } // datetime, null

        public int? FailedTimes { get; set; } // int, null

        public DateTime? RunTime { get; set; } // datetime, null

        public string Memo { get; set; } // nvarchar(200), null

        [DataMember]
        public string CTUID { get; set; } // char(40), not null
    }
}
