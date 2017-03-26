using System;
using System.Collections.Generic;
using System.Runtime.Serialization;

namespace LFCDal.Model
{
    [DataContract]
    public class tLampTmpCtrlCfg
    {
        [DataMember]
        public int CfgNo { get; set; } // int, not null

        [DataMember]
        public DateTime PlanOptDateTime { get; set; } // datetime, not null

        [DataMember]
        public byte OptType { get; set; } // tinyint, not null

        [DataMember]
        public int LampObjNo { get; set; } // int, not null

        [DataMember]
        public byte OptValue { get; set; } // tinyint, not null

        [DataMember]
        public byte? CtrlStatus { get; set; } // tinyint, null

        [DataMember]
        public DateTime? OptDateTime { get; set; } // datetime, null

        [DataMember]
        public string Memo { get; set; } // nvarchar(200), null

        [DataMember]
        public string CTUID { get; set; } // char(40), not null
    }
}
