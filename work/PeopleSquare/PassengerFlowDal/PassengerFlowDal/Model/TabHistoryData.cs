using System;
using System.Collections.Generic;
using System.Runtime.Serialization;

namespace PassengerFlowDal.Model
{
    [DataContract]
    public class TabHistoryData
    {
        public TabHistoryData()
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
        public string PhoneMac { get; set; } // varchar(40), not null

        [DataMember]
        public long DevTime { get; set; } // int, not null

        [DataMember]
        public DateTime RealTime { get; set; } // datetime, not null

        [DataMember]
        public int ModuleNo { get; set; } // int, not null

        [DataMember]
        public int Channel { get; set; } // int, not null

        [DataMember]
        public int RSSI { get; set; } // int, not null

        [DataMember]
        public string Memo { get; set; } // varchar(30), null
    }
}
