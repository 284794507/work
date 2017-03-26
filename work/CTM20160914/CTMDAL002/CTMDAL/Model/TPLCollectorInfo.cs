using System;
using System.Collections.Generic;
using System.Runtime.Serialization;

namespace CTMDAL.Model
{
    [DataContract]
    public class TPLCollectorInfo
    {
        public TPLCollectorInfo()
        {
            this.ObjID = Guid.NewGuid();
            this.TheDate = DateTime.Now;
            this.DevStatus = 1;
            this.ValidStatus = 0;
            this.UpdateTime = DateTime.Now;
            this.Status = 1;
        }

        [DataMember]
        public Guid ObjID { get; set; } // uniqueidentifier, not null

        [DataMember]
        public DateTime TheDate { get; set; } // datetime, not null

        [DataMember]
        public string MacAddr { get; set; } // varchar(30), not null

        [DataMember]
        public string HVer { get; set; } // varchar(10), not null

        [DataMember]
        public string SVer { get; set; } // varchar(10), not null

        [DataMember]
        public Guid TPlatFormID { get; set; } // uniqueidentifier, not null

        [DataMember]
        public string TPlatFormCode { get; set; } // varchar(30), not null

        [DataMember]
        public byte DevType { get; set; } // tinyint, not null

        [DataMember]
        public byte DevStatus { get; set; } // tinyint, not null

        public byte? ValidStatus { get; set; } // tinyint, null

        [DataMember]
        public string APhase { get; set; } // varchar(5), not null

        [DataMember]
        public string BPhase { get; set; } // varchar(5), not null

        [DataMember]
        public string CPhase { get; set; } // varchar(5), not null

        [DataMember]
        public DateTime? UpdateTime { get; set; } // datetime, null

        [DataMember]
        public byte Status { get; set; } // tinyint, not null

        [DataMember]
        public string Memo { get; set; } // varchar(50), null

        [DataMember]
        public Guid? GprsID { get; set; } // uniqueidentifier, null

        [DataMember]
        public Guid? PlcID { get; set; } // uniqueidentifier, null

        [DataMember]
        public string SNCode { get; set; } // nvarchar(30), null

        [DataMember]
        public string Address { get; set; } // nvarchar(50), null

        [DataMember]
        public byte? ChannelNo { get; set; } // tinyint, null

        [DataMember]
        public decimal? Lon { get; set; } // decimal(9,6), null

        [DataMember]
        public decimal? Lat { get; set; } // decimal(8,6), null

        [DataMember]
        public string CollectorName { get; set; } // nvarchar(50), null
    }
}
