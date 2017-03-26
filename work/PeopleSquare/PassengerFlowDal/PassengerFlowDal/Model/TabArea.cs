using System;
using System.Collections.Generic;
using System.Runtime.Serialization;

namespace PassengerFlowDal.Model
{
    [DataContract]
    public class TabArea
    {
        public TabArea()
        {
            this.ObjID = Guid.NewGuid();
            this.TheDate = DateTime.Now;
        }

        [DataMember]
        public Guid ObjID { get; set; } // uniqueidentifier, not null

        [DataMember]
        public DateTime TheDate { get; set; } // datetime, not null

        [DataMember]
        public Guid? ParentID { get; set; } // uniqueidentifier, null

        [DataMember]
        public string AreaName { get; set; } // varchar(50), not null

        [DataMember]
        public decimal Origin_Lon { get; set; } // decimal(10,7), not null

        [DataMember]
        public decimal Origin_Lat { get; set; } // decimal(10,7), not null

        [DataMember]
        public int Width { get; set; } // int, not null

        [DataMember]
        public int Height { get; set; } // int, not null

        [DataMember]
        public decimal Center_Lon { get; set; } // decimal(10,7), not null

        [DataMember]
        public decimal Center_Lat { get; set; } // decimal(10,7), not null

        [DataMember]
        public byte ZoomLevel { get; set; } // tinyint, not null

        [DataMember]
        public string Memo { get; set; } // varchar(30), null
    }
}
