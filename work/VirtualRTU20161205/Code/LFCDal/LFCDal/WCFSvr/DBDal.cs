using LFCDal.Interface;
using LFCDal.Model;
using LFCDal.Utility;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.ServiceModel;
using System.Text;

namespace LFCDal.WCFSvr
{
    // 注意: 使用“重构”菜单上的“重命名”命令，可以同时更改代码和配置文件中的类名“DBDal”。
    public class DBDal : IServiceBase, IDBDal
    {
        private IDBBLL dbBll=null;

        public LFCDalParameter GetInfoByPara(LFCDalParameter para)
        {
            LFCDalParameter result = new LFCDalParameter();
            dbBll = ContainerHandler.GetDll;
            switch(para.BusinessType)
            {
                case BusinessType.GetCtuInfo:
                    result = dbBll.GetALLCtuInfo();
                    break;
                case BusinessType.GetSysRunStatus:
                    result = dbBll.GetSysRunStatus();
                    break;
                case BusinessType.GetLampList:
                    result = dbBll.GetLampList();
                    break;
                case BusinessType.GetLampStatusByLampNo:
                    result = dbBll.GetLampStatusByLampNo(para);
                    break;
                case BusinessType.GetLampDataRecByLampNo:
                    result = dbBll.GetLampDataRecByLampNo(para);
                    break;
                case BusinessType.GetChronologyOfToday:
                    result = dbBll.GetChronologyOfToday(para);
                    break;
                case BusinessType.GetPlanFromLampTmpCtrlCfg:
                    result = dbBll.GetPlanFromLampTmpCtrlCfg(para);
                    break;
                case BusinessType.GetPlanFromLampWeekCtrlCfg:
                    result = dbBll.GetPlanFromLampWeekCtrlCfg(para);
                    break;
                case BusinessType.GetMaxLampTmpCtrlCfgNo:
                    result = dbBll.GetMaxLampTmpCtrlCfgNo(para);
                    break;
                case BusinessType.QueryLampTempCtrlCfg:
                    result = dbBll.QueryLampTempCtrlCfg(para);
                    break;
                case BusinessType.QueryPlanFromLampWeekCtrlCfg:
                    result = dbBll.QueryPlanFromLampWeekCtrlCfg(para);
                    break;
                    //case BusinessType.GetMaxNoFromElecData:
                    //    result = dbBll.GetMaxNoFromElecData(para);
                    //    break;
            }
            return result;
        }

        public void DelInfoByPara(LFCDalParameter para)
        {
            LFCDalParameter result = new LFCDalParameter();
            dbBll = ContainerHandler.GetDll;
            switch (para.BusinessType)
            {
                case BusinessType.DelLampTmpCtrlCfgByNo:
                    dbBll.DelLampTmpCtrlCfgByNo(para);
                    break;
                case BusinessType.DelAllLampTmpCtrlCfg:
                     dbBll.DelAllLampTmpCtrlCfg(para);
                    break;
                case BusinessType.DelLampWeekCtrlCfgByWeekDay:
                     dbBll.DelLampWeekCtrlCfgByWeekDay(para);
                    break;
                case BusinessType.DelAllLampWeekCtrlCfg:
                     dbBll.DelAllLampWeekCtrlCfg(para);
                    break;
                case BusinessType.DelLampTmpCtrlCfgByCfgNo:
                    dbBll.DelLampTmpCtrlCfgByCfgNo(para);
                    break;
                    
            }
        }

        public void AddInfoByPara(LFCDalParameter para)
        {
            LFCDalParameter result = new LFCDalParameter();
            dbBll = ContainerHandler.GetDll;
            switch (para.BusinessType)
            {
                case BusinessType.AddLampTmpCtrlCfg:
                    dbBll.AddLampTmpCtrlCfg(para);
                    break;
                case BusinessType.AddLampWeekCtrlCfg:
                    dbBll.AddLampWeekCtrlCfg(para);
                    break;
                case BusinessType.SaveElecData:
                    dbBll.SaveElecData(para);
                    break;
                case BusinessType.SaveLampGroupCfg:
                    dbBll.SaveLampGroupCfg(para);
                    break;
            }
        }

        public void UpdateInfoByPara(LFCDalParameter para)
        {
            LFCDalParameter result = new LFCDalParameter();
            dbBll = ContainerHandler.GetDll;
            switch (para.BusinessType)
            {
                case BusinessType.UpdateLampTmpCtrlStatus:
                    dbBll.UpdateLampTmpCtrlStatus(para);
                    break;
                case BusinessType.UpdateLampGrpCfgStatus:
                    dbBll.UpdateLampGrpCfgStatus(para);
                    break;
            }
        }

        public LFCDalParameter GetInfo()
        {
            LFCDalParameter result = new LFCDalParameter();
            result.BusinessObject = System.AppDomain.CurrentDomain.BaseDirectory;
            result.BusinessType = BusinessType.GetLampList;
            return result;
        }
        //public void DelLampTmpCtrlCgfByNo(string data)
        //{

        //}

        //public void DelAllLampTmpCtrlCfg()
        //{

        //}

        //public void AddLampTmpCtrlCfg(string data)
        //{

        //}

        //public void DelLampWeekCtrlCfgByWeekDay(string data)
        //{

        //}

        //public void DelAllLampWeekCtrlCfg()
        //{

        //}

        //public void AddLampWeekCtrlCfg(string data)
        //{

        //}

        //public string GetChronologyOfToday()
        //{
        //    string result = "";

        //    return result;
        //}

        //public string GetPlanFromLampTmpCtrlCfg()
        //{
        //    string result = "";

        //    return result;
        //}

        //public string GetPlanFromLampWeekCtrlCfg()
        //{
        //    string result = "";

        //    return result;
        //}

        //public string GetMaxNoFromElecData()
        //{
        //    string result = "";

        //    return result;
        //}

        //public void SaveElecData(string data)
        //{

        //}

        //public LFCDalParameter GetALLCtuInfo()
        //{
        //    dbBll = ContainerHandler.GetDll;//ContainerHandler.GetInstance.GetService();
        //    LFCDalParameter para = dbBll.GetALLCtuInfo();
        //    return para;
        //}

        //public LFCDalParameter GetSysRunStatus()
        //{
        //    dbBll = ContainerHandler.GetDll;//ContainerHandler.GetInstance.GetService();
        //    LFCDalParameter para = dbBll.GetSysRunStatus();
        //    return para;
        //}

        //public LFCDalParameter GetLampInfo()
        //{
        //    dbBll = ContainerHandler.GetDll;
        //    LFCDalParameter para = dbBll.GetLampInfo();
        //    return para;
        //}

        //public LFCDalParameter GetLampStatusByLampNo(int lampNo)
        //{
        //    dbBll = ContainerHandler.GetDll;
        //    LFCDalParameter para = dbBll.GetLampStatusByLampNo(lampNo);
        //    return para;
        //}

        //public LFCDalParameter GetLampDataRecByLampNo(int lampNo)
        //{
        //    dbBll = ContainerHandler.GetDll;
        //    LFCDalParameter para = dbBll.GetLampDataRecByLampNo(lampNo);
        //    return para;
        //}

    }
}
