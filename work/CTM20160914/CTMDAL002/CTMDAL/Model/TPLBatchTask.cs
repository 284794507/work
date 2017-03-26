using System;
using System.Collections.Generic;
using System.Runtime.Serialization;

namespace CTMDAL.Model
{
    [DataContract]
    public class TPLBatchTask
    {
        public TPLBatchTask()
        {
            //this.ObjID = Guid.NewGuid();
            this.TheDate = DateTime.Now;
            this.BatchDateTime = DateTime.Now;
        }

        [DataMember]
        public Guid ObjID { get; set; } // uniqueidentifier, not null

        [DataMember]
        public DateTime TheDate { get; set; } // datetime, not null

        [DataMember]
        public int BatchNum { get; set; } // int, not null

        [DataMember]
        public DateTime BatchDateTime { get; set; } // datetime, not null

        [DataMember]
        public string Memo { get; set; } // varchar(50), null
    }
}
