using System;
using System.Collections.Generic;
using System.Runtime.Serialization;

namespace CTMDAL.Model
{
    [DataContract]
    public class TPlatFormInfo
    {
        public TPlatFormInfo()
        {
            this.ObjID = Guid.NewGuid();
            this.TheDate = DateTime.Now;
            this.UpdateTime = DateTime.Now;
            this.Status = 1;
        }

        [DataMember]
        public Guid ObjID { get; set; } // uniqueidentifier, not null

        [DataMember]
        public DateTime TheDate { get; set; } // datetime, not null

        [DataMember]
        public string CountryCode { get; set; } // varchar(10), not null

        [DataMember]
        public string CityCode { get; set; } // varchar(10), not null

        [DataMember]
        public string PlatFormCode { get; set; } // varchar(30), not null

        [DataMember]
        public string PlatFormName { get; set; } // varchar(30), not null

        [DataMember]
        public DateTime? UpdateTime { get; set; } // datetime, null

        [DataMember]
        public byte Status { get; set; } // tinyint, not null

        [DataMember]
        public string Memo { get; set; } // varchar(50), null
    }
}
