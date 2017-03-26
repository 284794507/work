using System;
using System.Collections.Generic;
using System.Runtime.Serialization;

namespace LFCDal.Model
{
    [DataContract]
    public class tSysRunStatus
    {
        [DataMember]
        public DateTime? StartTime { get; set; } // datetime, null

        [DataMember]
        public byte? PLCSatus { get; set; } // tinyint, null

        [DataMember]
        public byte? SvrStatus { get; set; } // tinyint, null

        [DataMember]
        public int? GPRSCSQ { get; set; } // int, null

        [DataMember]
        public int? GPRSTryConnTimes { get; set; } // int, null

        [DataMember]
        public int? MainVer { get; set; } // int, null

        [DataMember]
        public int? SubVer { get; set; } // int, null

        [DataMember]
        public DateTime? UpdateTime { get; set; } // datetime, null

        [DataMember]
        public DateTime? EmgyCtrlEndTime { get; set; } // datetime, null

        [DataMember]
        public int? EmgyCtrlMask { get; set; } // int, null

        [DataMember]
        public int? EmgyCtrlVal { get; set; } // int, null

        [DataMember]
        public string CTUID { get; set; } // char(40), not null
    }
}
