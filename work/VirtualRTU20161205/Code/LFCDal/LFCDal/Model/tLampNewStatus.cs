using System;
using System.Collections.Generic;
using System.Runtime.Serialization;

namespace LFCDal.Model
{
    [DataContract]
    public class tLampNewStatus
    {
        [DataMember]
        public int LampNo { get; set; } // int, not null

        [DataMember]
        public byte LampStatus { get; set; } // tinyint, not null

        [DataMember]
        public DateTime CDateTime { get; set; } // datetime, not null

        [DataMember]
        public byte? OptStatus { get; set; } // tinyint, null

        [DataMember]
        public DateTime? OptDTime { get; set; } // datetime, null

        [DataMember]
        public byte? OptBy { get; set; } // tinyint, null

        [DataMember]
        public byte? LockStatus { get; set; } // tinyint, null

        [DataMember]
        public byte? LampInsAlarm { get; set; } // tinyint, null

        [DataMember]
        public string CTUID { get; set; } // char(40), not null
    }
}
