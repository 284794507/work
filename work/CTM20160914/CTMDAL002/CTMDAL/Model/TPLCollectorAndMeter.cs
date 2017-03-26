using System;
using System.Collections.Generic;
using System.Runtime.Serialization;

namespace CTMDAL.Model
{
    [DataContract]
    public class TPLCollectorAndMeter
    {
        public TPLCollectorAndMeter()
        {
            this.ObjID = Guid.NewGuid();
            this.TheDate = DateTime.Now;
            this.Status = 1;
        }

        [DataMember]
        public Guid ObjID { get; set; } // uniqueidentifier, not null

        [DataMember]
        public DateTime TheDate { get; set; } // datetime, not null

        [DataMember]
        public string PlatFormCode { get; set; } // varchar(30), not null

        [DataMember]
        public string PlatFormName { get; set; } // varchar(30), not null

        [DataMember]
        public Guid CollectorID { get; set; } // uniqueidentifier, not null

        [DataMember]
        public string CollectorName { get; set; } // varchar(30), not null

        [DataMember]
        public string CollectorCode { get; set; } // varchar(30), not null

        [DataMember]
        public string SupplyCode { get; set; } // varchar(30), null

        [DataMember]
        public string MeterCode { get; set; } // varchar(50), not null

        [DataMember]
        public string UserName { get; set; } // varchar(200), null

        [DataMember]
        public string UserAddress { get; set; } // varchar(200), null

        [DataMember]
        public byte? Status { get; set; } // tinyint, null

        [DataMember]
        public string Memo { get; set; } // varchar(30), null
    }
}
