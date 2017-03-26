using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.ServiceModel;
using System.Text;
using LFCDal.Model;
using LFCDal.Utility;

namespace LFCDal.WCFSvr
{
    // 注意: 使用“重构”菜单上的“重命名”命令，可以同时更改代码和配置文件中的接口名“IDBDal”。
    [ServiceContract]
    public interface IDBDal
    {
        [OperationContract]
        LFCDalParameter GetInfoByPara(LFCDalParameter para);

        [OperationContract]
        void DelInfoByPara(LFCDalParameter para);

        [OperationContract]
        void AddInfoByPara(LFCDalParameter para);

        [OperationContract]
        void UpdateInfoByPara(LFCDalParameter para);

        [OperationContract]
        LFCDalParameter GetInfo();

        //[OperationContract]
        //LFCDalParameter GetALLCtuInfo();

        //[OperationContract]
        //LFCDalParameter GetSysRunStatus();

        //[OperationContract]
        //LFCDalParameter GetLampInfo();

        //[OperationContract]
        //LFCDalParameter GetLampStatusByLampNo(int lampNo);

        //[OperationContract]
        //LFCDalParameter GetLampDataRecByLampNo(int lampNo);

        //[OperationContract]
        //void DelLampTmpCtrlCgfByNo(string data);

        //[OperationContract]
        //void DelAllLampTmpCtrlCfg();

        //[OperationContract]
        //void AddLampTmpCtrlCfg(string data);

        //[OperationContract]
        //void DelLampWeekCtrlCfgByWeekDay(string data);

        //[OperationContract]
        //void DelAllLampWeekCtrlCfg();

        //[OperationContract]
        //void AddLampWeekCtrlCfg(string data);

        //[OperationContract]
        //string GetChronologyOfToday();

        //[OperationContract]
        //string GetPlanFromLampTmpCtrlCfg();

        //[OperationContract]
        //string GetPlanFromLampWeekCtrlCfg();

        //[OperationContract]
        //string GetMaxNoFromElecData();

        //[OperationContract]
        //void SaveElecData(string data);
    }
}
