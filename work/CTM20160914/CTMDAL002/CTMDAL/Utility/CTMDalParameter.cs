using CTMDAL.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using System.Threading.Tasks;

namespace CTMDAL.Utility
{
    [DataContract]
    [KnownType(typeof(int))]
    [KnownType(typeof(int[]))]
    [KnownType(typeof(byte))]
    [KnownType(typeof(byte[]))]
    [KnownType(typeof(string))]
    [KnownType(typeof(string[]))]
    [KnownType(typeof(TPlatFormInfo))]
    [KnownType(typeof(TPlatFormInfo[]))]
    [KnownType(typeof(TPLBatchTask))]
    [KnownType(typeof(TPLBatchTask[]))]
    [KnownType(typeof(TPLCollectorInfo))]
    [KnownType(typeof(TPLCollectorInfo[]))]
    [KnownType(typeof(TPLCollectorMasterCommStatus_Cur))]
    [KnownType(typeof(TPLCollectorMasterCommStatus_Cur[]))]
    [KnownType(typeof(TPLCollectorMasterCommStatus_His))]
    [KnownType(typeof(TPLCollectorMasterCommStatus_His[]))]
    [KnownType(typeof(TPLCollectorPLCCommStatus_Cur))]
    [KnownType(typeof(TPLCollectorPLCCommStatus_Cur[]))]
    [KnownType(typeof(TPLCollectorPLCCommStatus_His))]
    [KnownType(typeof(TPLCollectorPLCCommStatus_His[]))]
    [KnownType(typeof(TPLCollectorStaticRoutes))]
    [KnownType(typeof(TPLCollectorStaticRoutes[]))]
    [KnownType(typeof(TPLCollectorWireLessCommStatus_Cur))]
    [KnownType(typeof(TPLCollectorWireLessCommStatus_Cur[]))]
    [KnownType(typeof(TPLCollectorWireLessCommStatus_His))]
    [KnownType(typeof(TPLCollectorWireLessCommStatus_His[]))]
    [KnownType(typeof(TPLDataRecHIS))]
    [KnownType(typeof(TPLDataRecHIS[]))]
    [KnownType(typeof(TPLDataRecRTM))]
    [KnownType(typeof(TPLDataRecRTM[]))]
    [KnownType(typeof(TPLErrorDataRecHIS))]
    [KnownType(typeof(TPLErrorDataRecHIS[]))]
    [KnownType(typeof(TPLUpgradeFileInfo))]
    [KnownType(typeof(TPLUpgradeFileInfo[]))]
    [KnownType(typeof(TPLUpgradeFileInfoDetail))]
    [KnownType(typeof(TPLUpgradeFileInfoDetail[]))]
    [KnownType(typeof(TPLBasicInfo))]
    [KnownType(typeof(TPLBasicInfo[]))]
    [KnownType(typeof(TPLCollectorAndMeter))]
    [KnownType(typeof(TPLCollectorAndMeter[]))]
    public class CTMDalParameter//
    {
        [DataMember]
        public Object BusinessObject { get; set; }

        [DataMember]
        public BusinessType BusinessType { get; set; }

        [DataMember]
        public string TerminalID { get; set; }
    }
}
