using LFCDal.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using System.Threading.Tasks;

namespace LFCDal.Utility
{
    [DataContract]
    [KnownType(typeof(int))]
    [KnownType(typeof(int[]))]
    [KnownType(typeof(byte))]
    [KnownType(typeof(byte[]))]
    [KnownType(typeof(string))]
    [KnownType(typeof(string[]))]
    [KnownType(typeof(tCTUInfo))]
    [KnownType(typeof(tCTUInfo[]))]
    [KnownType(typeof(tSysRunStatus))]
    [KnownType(typeof(tSysRunStatus[]))]
    [KnownType(typeof(vLampInfo))]
    [KnownType(typeof(vLampInfo[]))]
    [KnownType(typeof(tLampNewStatus))]
    [KnownType(typeof(tLampNewStatus[]))]
    [KnownType(typeof(tPTUCurDataRec))]
    [KnownType(typeof(tPTUCurDataRec[]))]
    [KnownType(typeof(tLampTmpCtrlCfg))]
    [KnownType(typeof(tLampTmpCtrlCfg[]))]
    [KnownType(typeof(pLampTmpCtrlCfg))]
    [KnownType(typeof(pLampTmpCtrlCfg[]))]
    [KnownType(typeof(tLampWeekCtrlCfg))]
    [KnownType(typeof(tLampWeekCtrlCfg[]))]
    [KnownType(typeof(tCTUOCDayCfg))]
    [KnownType(typeof(tCTUOCDayCfg[]))]
    [KnownType(typeof(tLampHisDataRec))]
    [KnownType(typeof(tLampHisDataRec[]))]
    [KnownType(typeof(tLampGrpCfg))]
    [KnownType(typeof(tLampGrpCfg[]))]    
    public class LFCDalParameter
    {
        [DataMember]
        public Object BusinessObject { get; set; }

        [DataMember]
        public BusinessType BusinessType { get; set; }

        [DataMember]
        public string CTUID { get; set; }
    }
}
