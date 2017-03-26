using System;
using System.Collections.Generic;
using System.Runtime.Serialization;

namespace PassengerFlowDal.Model
{
    [DataContract]
    public class TabTimeSync
    {
        public TabTimeSync()
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
        public DateTime ServerTime { get; set; } // datetime, not null

        [DataMember]
        public long DevTime { get; set; } // int, not null

        [DataMember]
        public int ModuleNo { get; set; } // int, not null

        [DataMember]
        public byte Valid { get; set; } // tinyint, not null

        [DataMember]
        public double DeltaTime { get; set; } // float, not null

        [DataMember]
        public string Memo { get; set; } // varchar(30), null
    }
}
