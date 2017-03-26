using System;
using System.Collections.Generic;
using System.Runtime.Serialization;

namespace CTMDAL.Model
{
    [DataContract]
    public class TPLBasicInfo
    {
        [DataMember]
        public double? ID { get; set; } // float, null

        [DataMember]
        public string LineName { get; set; } // nvarchar(255), null

        [DataMember]
        public string MeterName { get; set; } // nvarchar(255), null

        [DataMember]
        public string UserName { get; set; } // nvarchar(255), null

        [DataMember]
        public string UserAddress { get; set; } // nvarchar(255), null

        [DataMember]
        public string Phase { get; set; } // nvarchar(255), null

        [DataMember]
        public string UserType { get; set; } // nvarchar(255), null

        [DataMember]
        public string SupplyVoltage { get; set; } // nvarchar(255), null

        [DataMember]
        public string PlatFormCode { get; set; } // nvarchar(255), null

        [DataMember]
        public string UserCode { get; set; } // nvarchar(255), null

        [DataMember]
        public string OperateSign { get; set; } // nvarchar(255), null

        [DataMember]
        public string PowerNo { get; set; } // nvarchar(255), null

        [DataMember]
        public string RegisterCode { get; set; } // nvarchar(255), null

        [DataMember]
        public string ContactNumber { get; set; } // nvarchar(255), null

        [DataMember]
        public string ZipCOde { get; set; } // nvarchar(255), null

        [DataMember]
        public string InstalledCapacity { get; set; } // nvarchar(255), null

        [DataMember]
        public string ContractCapacity { get; set; } // nvarchar(255), null

        [DataMember]
        public string District { get; set; } // nvarchar(255), null

        [DataMember]
        public string Street { get; set; } // nvarchar(255), null

        [DataMember]
        public string NeighborhoodCommittee { get; set; } // nvarchar(255), null

        [DataMember]
        public string ContactAdress { get; set; } // nvarchar(255), null

        [DataMember]
        public string MeterReadingCoefficient { get; set; } // nvarchar(255), null

        [DataMember]
        public string ReadingOrder { get; set; } // nvarchar(255), null

        [DataMember]
        public string SupplyTime { get; set; } // nvarchar(255), null

        [DataMember]
        public string ElectricalSystemName { get; set; } // nvarchar(255), null

        [DataMember]
        public string SupplyCode { get; set; } // nvarchar(255), null

        [DataMember]
        public string Company { get; set; } // nvarchar(255), null

        [DataMember]
        public string LineLossRate { get; set; } // nvarchar(255), null
    }
}
