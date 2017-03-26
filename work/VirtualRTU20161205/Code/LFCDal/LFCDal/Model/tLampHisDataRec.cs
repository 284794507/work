using System;
using System.Collections.Generic;
using System.Runtime.Serialization;

namespace LFCDal.Model
{
    [DataContract]
    public class tLampHisDataRec
    {
        [DataMember]
        public int LampDataRecNo { get; set; } // int, not null

        [DataMember]
        public int LampNo { get; set; } // int, not null

        [DataMember]
        public double LampU { get; set; } // float, not null

        [DataMember]
        public double LampI { get; set; } // float, not null

        [DataMember]
        public double LampAP { get; set; } // float, not null

        [DataMember]
        public double LampVP { get; set; } // float, not null

        [DataMember]
        public double LampPF { get; set; } // float, not null

        [DataMember]
        public byte? LampStatus { get; set; } // tinyint, null

        [DataMember]
        public DateTime GetDateTime { get; set; } // datetime, not null

        [DataMember]
        public byte CommValue { get; set; } // tinyint, not null

        [DataMember]
        public byte? isUpLoaded { get; set; } // tinyint, null

        [DataMember]
        public DateTime? UpLoadDTime { get; set; } // datetime, null

        [DataMember]
        public string Memo { get; set; } // nvarchar(200), null

        [DataMember]
        public string CTUID { get; set; } // char(40), not null
    }
}
