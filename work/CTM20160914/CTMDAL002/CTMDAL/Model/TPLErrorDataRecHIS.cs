using System;
using System.Collections.Generic;
using System.Runtime.Serialization;

namespace CTMDAL.Model
{
    [DataContract]
    public class TPLErrorDataRecHIS
    {
        public TPLErrorDataRecHIS()
        {
            this.ObjID = Guid.NewGuid();
            this.TheDate = DateTime.Now;
            this.GetDataTime = DateTime.Now;
            this.UpdateTime = DateTime.Now;
            this.Status = 1;
        }

        [DataMember]
        public Guid ObjID { get; set; } // uniqueidentifier, not null

        [DataMember]
        public DateTime TheDate { get; set; } // datetime, not null

        [DataMember]
        public Guid DevID { get; set; } // uniqueidentifier, not null

        [DataMember]
        public decimal LampVoltageA { get; set; } // decimal(7,2), not null

        [DataMember]
        public decimal LampCurrentA { get; set; } // decimal(7,2), not null

        [DataMember]
        public decimal LampActivePowerA { get; set; } // decimal(7,2), not null

        [DataMember]
        public decimal LampPowerFactA { get; set; } // decimal(7,2), not null

        [DataMember]
        public decimal LampVoltageB { get; set; } // decimal(7,2), not null

        [DataMember]
        public decimal LampCurrentB { get; set; } // decimal(7,2), not null

        [DataMember]
        public decimal LampActivePowerB { get; set; } // decimal(7,2), not null

        [DataMember]
        public decimal LampPowerFactB { get; set; } // decimal(7,2), not null

        [DataMember]
        public decimal LampVoltageC { get; set; } // decimal(7,2), not null

        [DataMember]
        public decimal LampCurrentC { get; set; } // decimal(7,2), not null

        [DataMember]
        public decimal LampActivePowerC { get; set; } // decimal(7,2), not null

        [DataMember]
        public decimal LampPowerFactC { get; set; } // decimal(7,2), not null

        [DataMember]
        public DateTime GetDataTime { get; set; } // datetime, not null

        [DataMember]
        public DateTime UpdateTime { get; set; } // datetime, not null

        [DataMember]
        public byte Status { get; set; } // tinyint, not null

        [DataMember]
        public string Memo { get; set; } // varchar(50), null

        [DataMember]
        public Guid? BatchID { get; set; } // uniqueidentifier, null
    }
}
