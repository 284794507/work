using System;
using System.Collections.Generic;
using System.Runtime.Serialization;

namespace LFCDal.Model
{
    [DataContract]
    public class tLampWeekCtrlCfg
    {
        [DataMember]
        public byte CmdType { get; set; } // tinyint, not null

        [DataMember]
        public int GrpNo { get; set; } // int, not null

        [DataMember]
        public byte? TimeType { get; set; } // tinyint, null

        [DataMember]
        public string OptTime { get; set; } // varchar(8), null

        [DataMember]
        public byte? OptType { get; set; } // tinyint, null

        [DataMember]
        public int? LampOrGrpNo { get; set; } // int, null

        [DataMember]
        public int? OptValue { get; set; } // int, null

        [DataMember]
        public byte? CtrlStatus { get; set; } // tinyint, null

        [DataMember]
        public DateTime? OptDateTime { get; set; } // datetime, null

        [DataMember]
        public string CTUID { get; set; } // char(40), not null
    }
}
