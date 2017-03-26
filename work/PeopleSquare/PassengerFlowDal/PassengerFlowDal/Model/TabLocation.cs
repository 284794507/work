using System;
using System.Collections.Generic;
using System.Runtime.Serialization;

namespace PassengerFlowDal.Model
{
    [DataContract]
    public class TabLocation
    {
        public TabLocation()
        {
            this.ObjID = Guid.NewGuid();
            this.TheDate = DateTime.Now;
        }

        [DataMember]
        public Guid ObjID { get; set; } // uniqueidentifier, not null

        [DataMember]
        public DateTime TheDate { get; set; } // datetime, not null

        [DataMember]
        public string DevMac { get; set; } // varchar(40), not null

        [DataMember]
        public DateTime RealTime { get; set; } // datetime, not null

        [DataMember]
        public decimal Longitude { get; set; } // decimal(10,7), not null

        [DataMember]
        public decimal Latitude { get; set; } // decimal(10,7), not null

        [DataMember]
        public int Radius { get; set; } // int, not null

        [DataMember]
        public string Memo { get; set; } // varchar(30), null
    }
}
