using System;
using System.Collections.Generic;
using System.Runtime.Serialization;

namespace CTMDAL.Model
{
    [DataContract]
    public class TPLCollectorStaticRoutes
    {
        public TPLCollectorStaticRoutes()
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
        public Guid DevID { get; set; } // uniqueidentifier, not null

        [DataMember]
        public Guid StaticRouteNode { get; set; } // uniqueidentifier, not null

        [DataMember]
        public byte Status { get; set; } // tinyint, not null

        [DataMember]
        public string Memo { get; set; } // varchar(50), null

        [DataMember]
        public Guid? BatchID { get; set; } // uniqueidentifier, null
    }
}
