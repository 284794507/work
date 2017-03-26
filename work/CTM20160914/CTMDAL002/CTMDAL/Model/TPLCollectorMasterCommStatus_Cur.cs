using System;
using System.Collections.Generic;
using System.Runtime.Serialization;

namespace CTMDAL.Model
{
    [DataContract]
    public class TPLCollectorMasterCommStatus_Cur
    {
        public TPLCollectorMasterCommStatus_Cur()
        {
            this.ObjID = Guid.NewGuid();
            this.TheDate = DateTime.Now;
            this.ChkDataTime = DateTime.Now;
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
        public decimal Lon { get; set; } // decimal(9,6), not null

        [DataMember]
        public decimal LAT { get; set; } // decimal(8,6), not null

        [DataMember]
        public byte CommStatus { get; set; } // tinyint, not null

        [DataMember]
        public DateTime ChkDataTime { get; set; } // datetime, not null

        [DataMember]
        public DateTime UpdateTime { get; set; } // datetime, not null

        [DataMember]
        public byte Status { get; set; } // tinyint, not null

        [DataMember]
        public int? TotalCommTimes { get; set; } // int, null

        [DataMember]
        public int? SuccessfulCommTimes { get; set; } // int, null

        [DataMember]
        public decimal? LostRate { get; set; } // decimal(4,2), null
    }
}
