using System;
using System.Collections.Generic;
using System.Runtime.Serialization;

namespace LFCDal.Model
{
    [DataContract]
    public class tCTUInfo
    {
        [DataMember]
        public string CTUID { get; set; } // char(40), not null

        [DataMember]
        public string CTUName { get; set; } // nvarchar(40), not null

        [DataMember]
        public byte CTUType { get; set; } // tinyint, not null

        [DataMember]
        public byte CTUChNum { get; set; } // tinyint, not null

        [DataMember]
        public string CTUChByte { get; set; } // char(8), not null

        [DataMember]
        public byte? CtrlCloseDiv { get; set; } // tinyint, null

        [DataMember]
        public string CTUCommAddr { get; set; } // char(32), not null

        [DataMember]
        public string MobilePhoneNo { get; set; } // char(26), null

        [DataMember]
        public string SvrDomainIp { get; set; } // char(50), not null

        [DataMember]
        public int SvrPort { get; set; } // int, not null

        [DataMember]
        public string SvrAPN { get; set; } // char(32), null

        [DataMember]
        public int FSM { get; set; } // int, not null

        [DataMember]
        public byte HavePTUStatus { get; set; } // tinyint, not null

        [DataMember]
        public byte? DiAlarmTick { get; set; } // tinyint, null

        [DataMember]
        public byte? PollingTick { get; set; } // tinyint, null

        [DataMember]
        public byte? FindPTUTick { get; set; } // tinyint, null

        [DataMember]
        public int? PTUCount { get; set; } // int, null

        [DataMember]
        public string IPAddress { get; set; } // char(30), null

        [DataMember]
        public string MASKAddress { get; set; } // char(30), not null

        [DataMember]
        public string IPGateway { get; set; } // char(30), null

        [DataMember]
        public string CTUMacAddr { get; set; } // char(40), null

        [DataMember]
        public byte? DeviceType { get; set; } // tinyint, null

        [DataMember]
        public byte EnergyEfficient { get; set; } // tinyint, not null

        [DataMember]
        public int LogPrintTick { get; set; } // int, not null

        [DataMember]
        public byte PLCComTimer { get; set; } // tinyint, not null

        [DataMember]
        public byte EnergyTimeType { get; set; } // tinyint, not null

        [DataMember]
        public byte AutoRouteTick { get; set; } // tinyint, not null

        [DataMember]
        public string GSM_IMEI { get; set; } // char(30), null

        [DataMember]
        public string SIM_IMSI { get; set; } // char(30), null

        [DataMember]
        public byte? HaveRTU { get; set; } // tinyint, null

        [DataMember]
        public byte? NetworkType { get; set; } // tinyint, null

        [DataMember]
        public byte? DiUpdateTime { get; set; } // tinyint, null

        [DataMember]
        public string WeekCtlMark { get; set; } // char(8), null

        [DataMember]
        public string TempCtlMark { get; set; } // char(8), null

        [DataMember]
        public string SpecialDayCtlMark { get; set; } // char(8), null

        [DataMember]
        public byte? EveryDayCtlMark { get; set; } // tinyint, null

        [DataMember]
        public byte? PTUType { get; set; } // tinyint, null

        [DataMember]
        public string memo { get; set; } // nvarchar(200), null

        [DataMember]
        public int? GPRSTimeOut { get; set; } // int, null

        [DataMember]
        public byte? GPRSRetryCount { get; set; } // tinyint, null

        [DataMember]
        public byte? IsNeedRpyMask { get; set; } // tinyint, null

        [DataMember]
        public byte? GPRSHeart { get; set; } // tinyint, null

        [DataMember]
        public string EventRecValidFlag { get; set; } // char(16), null

        [DataMember]
        public string EventImportanceFlag { get; set; } // char(16), null

        [DataMember]
        public string EventAutoUploadFlag { get; set; } // char(16), null

        [DataMember]
        public int? DIClctIntval { get; set; } // int, null

        [DataMember]
        public int? AIClctIntval { get; set; } // int, null

        [DataMember]
        public int? SmartClctIntval { get; set; } // int, null

        [DataMember]
        public string CtrlLineValidFlag { get; set; } // char(8), null

        [DataMember]
        public string PwrSaveValidFlag { get; set; } // char(4), null

        [DataMember]
        public byte? PwrSaveSwitchMaxPos { get; set; } // tinyint, null

        [DataMember]
        public string CabID { get; set; } // char(24), null

        [DataMember]
        public string Longitude { get; set; } // char(22), null

        [DataMember]
        public string Latitude { get; set; } // char(22), null

        [DataMember]
        public int? IsSingleLampFunc { get; set; } // int, null

        [DataMember]
        public byte? TmnCommErrTestIntval { get; set; } // tinyint, null

        [DataMember]
        public byte? RouterCommErrTestIntval { get; set; } // tinyint, null

        [DataMember]
        public byte? SingLampOpenClctDelay { get; set; } // tinyint, null

        [DataMember]
        public byte? SingLampCloseClctDelay { get; set; } // tinyint, null

        [DataMember]
        public byte? SingLampCurrClctDelay { get; set; } // tinyint, null

        [DataMember]
        public byte? SlampOverILimtVal { get; set; } // tinyint, null

        [DataMember]
        public byte? SlampOverILRecovVal { get; set; } // tinyint, null

        [DataMember]
        public byte? SlampLowILimtVal { get; set; } // tinyint, null

        [DataMember]
        public byte? SlampLowIRecovVal { get; set; } // tinyint, null

        [DataMember]
        public double? SlaCapFailEvtPFLimtVal { get; set; } // float, null

        [DataMember]
        public double? SlaCapFailEvtRecoPFLimtVal { get; set; } // float, null

        [DataMember]
        public byte? SlaFailEvtILimitVal { get; set; } // tinyint, null

        [DataMember]
        public byte? SlaFailEvtIRecovVal { get; set; } // tinyint, null

        [DataMember]
        public byte? SlaFuseErrEvtILimtVal { get; set; } // tinyint, null

        [DataMember]
        public byte? SlaFuseErrEvtIRecoVal { get; set; } // tinyint, null
    }
}
