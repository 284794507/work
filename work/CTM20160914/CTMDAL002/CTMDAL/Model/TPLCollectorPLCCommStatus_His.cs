using System;
using System.Collections.Generic;
using System.Runtime.Serialization;

namespace CTMDAL.Model
{
    [DataContract]
    public class TPLCollectorPLCCommStatus_His
    {
        public TPLCollectorPLCCommStatus_His()
        {
            this.ObjID = Guid.NewGuid();
            this.TheDate = DateTime.Now;
            this.ChkDataTime1 = DateTime.Now;
            this.ChkDataTime2 = DateTime.Now;
            this.ChkDataTime3 = DateTime.Now;
            this.UpdateTime = DateTime.Now;
            this.Status = 1;
        }

        [DataMember]
        public Guid ObjID { get; set; } // uniqueidentifier, not null

        [DataMember]
        public DateTime TheDate { get; set; } // datetime, not null

        [DataMember]
        public Guid PLCollectorInfoID { get; set; } // uniqueidentifier, not null

        [DataMember]
        public byte PLCL1Status { get; set; } // tinyint, not null

        [DataMember]
        public byte PLCL2Status { get; set; } // tinyint, not null

        [DataMember]
        public byte PLCL3Status { get; set; } // tinyint, not null

        [DataMember]
        public DateTime? ChkDataTime1 { get; set; } // datetime, null

        [DataMember]
        public DateTime? ChkDataTime2 { get; set; } // datetime, null

        [DataMember]
        public DateTime? ChkDataTime3 { get; set; } // datetime, null

        [DataMember]
        public DateTime UpdateTime { get; set; } // datetime, not null

        [DataMember]
        public byte Status { get; set; } // tinyint, not null

        [DataMember]
        public int? L1TotalCommTimes { get; set; } // int, null

        [DataMember]
        public int? L2TotalCommTimes { get; set; } // int, null

        [DataMember]
        public int? L3TotalCommTimes { get; set; } // int, null

        [DataMember]
        public int? L1SuccessfulCommTimes { get; set; } // int, null

        [DataMember]
        public int? L2SuccessfulCommTimes { get; set; } // int, null

        [DataMember]
        public int? L3SuccessfulCommTimes { get; set; } // int, null

        [DataMember]
        public decimal? L1LostRate { get; set; } // decimal(4,2), null

        [DataMember]
        public decimal? L2LostRate { get; set; } // decimal(4,2), null

        [DataMember]
        public decimal? L3LostRate { get; set; } // decimal(4,2), null
    }
}
