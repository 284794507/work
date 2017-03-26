using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.ServiceModel;
using System.Configuration;
using LFCDal.Utility;
using LFCDal.Model;
using CommunicateCore.Terminal;

namespace CommunicateCore.Utility
{
    public class DBHandler
    {
        private static DBHandler handler;
        public static DBHandler GetHandler
        {
            get
            {
                if(handler==null)
                {
                    handler = new DBHandler();
                }
                return handler;
            }
        }

        private static IDBDal DalProxy;

        public static IDBDal GetProxy
        {
            get
            {
                if (DalProxy == null)
                {
                    string dbEndPointName= ConfigurationManager.AppSettings["DBEndPoint"]; 
                    ChannelFactory<IDBDal> channelFactory = new ChannelFactory<IDBDal>(dbEndPointName);
                    DalProxy  = channelFactory.CreateChannel();
                }
                return DalProxy;
            }
        }

        private void MonitorChannel()
        {
            AspectF.Define.Retry()
                .Do(() =>
                {
                    if (((IClientChannel)DalProxy).State == CommunicationState.Closed)
                    {
                        DalProxy = null;
                    }
                });
        }

        public void GetLampList()
        {
            AspectF.Define.Retry()
                .Do(() =>
                {
                    LFCDalParameter sendPara = new LFCDalParameter();
                    sendPara.BusinessType = BusinessType.GetLampList;
                    LFCDalParameter para = GetProxy.GetInfoByPara(sendPara);
                    TerminalShare.GetShare.LampList = para.BusinessObject as vLampInfo[];
                });
        }

        public tPTUCurDataRec GetLampDataRecByLampNo(int lampNo)
        {
            tPTUCurDataRec result = new tPTUCurDataRec();
            AspectF.Define.Retry()
                .Do(() =>
                {
                    LFCDalParameter sendPara = new LFCDalParameter();
                    sendPara.BusinessType = BusinessType.GetLampDataRecByLampNo;
                    sendPara.BusinessObject = lampNo;
                    LFCDalParameter para = GetProxy.GetInfoByPara(sendPara);
                    result = para.BusinessObject as tPTUCurDataRec;
                });
            return result;
        }

        public tLampNewStatus GetLampStatusByLampNo(int lampNo)
        {
            tLampNewStatus result = new tLampNewStatus();
            AspectF.Define.Retry()
                .Do(() =>
                {
                    LFCDalParameter sendPara = new LFCDalParameter();
                    sendPara.BusinessType = BusinessType.GetLampStatusByLampNo;
                    sendPara.BusinessObject = lampNo;
                    LFCDalParameter para = GetProxy.GetInfoByPara(sendPara);
                    result = para.BusinessObject as tLampNewStatus;
                });
            return result;
        }

        public tCTUInfo []GetCtuInfo()
        {
            tCTUInfo[] info = new tCTUInfo[0];
            AspectF.Define.Retry()
                .Do(() =>
                {
                    LFCDalParameter sendPara = new LFCDalParameter();
                    sendPara.BusinessType = BusinessType.GetCtuInfo;
                    LFCDalParameter para = GetProxy.GetInfoByPara(sendPara);
                    info = para.BusinessObject as tCTUInfo[];
                });
            return info;
        }

        public tSysRunStatus[] GetRunStatus()
        {
            tSysRunStatus[] info = new tSysRunStatus[0];
            AspectF.Define.Retry()
                .Do(() =>
                {
                    LFCDalParameter sendPara = new LFCDalParameter();
                    sendPara.BusinessType = BusinessType.GetSysRunStatus;
                    LFCDalParameter para = GetProxy.GetInfoByPara(sendPara);
                    info = para.BusinessObject as tSysRunStatus[];
                });
            return info;
        }

        public tCTUOCDayCfg[] GetChronologyOfToday()
        {
            tCTUOCDayCfg[] info = new tCTUOCDayCfg[0];
            AspectF.Define.Retry()
                .Do(() =>
                {
                    LFCDalParameter sendPara = new LFCDalParameter();
                    sendPara.BusinessType = BusinessType.GetChronologyOfToday;
                    LFCDalParameter para = GetProxy.GetInfoByPara(sendPara);
                    info = para.BusinessObject as tCTUOCDayCfg[];
                });
            return info;
        }

        public tLampTmpCtrlCfg[] GetPlanFromLampTmpCtrlCfg()
        {
            tLampTmpCtrlCfg[] info = new tLampTmpCtrlCfg[0];
            AspectF.Define.Retry()
                .Do(() =>
                {
                    LFCDalParameter sendPara = new LFCDalParameter();
                    sendPara.BusinessType = BusinessType.GetPlanFromLampTmpCtrlCfg;
                    LFCDalParameter para = GetProxy.GetInfoByPara(sendPara);
                    info = para.BusinessObject as tLampTmpCtrlCfg[];
                });
            return info;
        }

        public tLampWeekCtrlCfg[] GetPlanFromLampWeekCtrlCfg()
        {
            tLampWeekCtrlCfg[] info = new tLampWeekCtrlCfg[0];
            AspectF.Define.Retry()
                .Do(() =>
                {
                    LFCDalParameter sendPara = new LFCDalParameter();
                    sendPara.BusinessType = BusinessType.GetPlanFromLampWeekCtrlCfg;
                    LFCDalParameter para = GetProxy.GetInfoByPara(sendPara);
                    info = para.BusinessObject as tLampWeekCtrlCfg[];
                });
            return info;
        }

        public int GetMaxLampTmpCtrlCfgNo(string ctuID)
        {
            int result = 0;
            AspectF.Define.Retry()
                .Do(() =>
                {
                    LFCDalParameter sendPara = new LFCDalParameter();
                    sendPara.BusinessType = BusinessType.GetMaxLampTmpCtrlCfgNo;
                    sendPara.CTUID = ctuID;
                    LFCDalParameter para = GetProxy.GetInfoByPara(sendPara);

                    result = int.Parse(para.BusinessObject.ToString());
                });
            return result;
        }

        /// <summary>
        /// 新增单灯(组)临时预约
        /// </summary>
        /// <param name="cfg"></param>
        public void AddLampTmpCtrlCfg(tLampTmpCtrlCfg cfg)
        {
            AspectF.Define.Retry()
                .Do(() =>
                {
                    LFCDalParameter sendPara = new LFCDalParameter();
                    sendPara.BusinessType = BusinessType.AddLampTmpCtrlCfg;
                    sendPara.BusinessObject = cfg;
                    GetProxy.AddInfoByPara(sendPara);
                });
        }

        /// <summary>
        /// 删除所有单灯(组)临时预约
        /// </summary>
        public void DelAllLampTmpCtrlCfg()
        {
            AspectF.Define.Retry()
                .Do(() =>
                {
                    LFCDalParameter sendPara = new LFCDalParameter();
                    sendPara.BusinessType = BusinessType.DelAllLampTmpCtrlCfg;
                    GetProxy.DelInfoByPara(sendPara);
                });
        }

        public void DelLampTmpCtrlCfgByCfgNo(string ctuID,int optType,int no)
        {
            AspectF.Define.Retry()
                .Do(() =>
                {
                    pLampTmpCtrlCfg cfg = new pLampTmpCtrlCfg();
                    cfg.OptType = optType;
                    cfg.No = no;
                    LFCDalParameter sendPara = new LFCDalParameter();
                    sendPara.BusinessType = BusinessType.DelLampTmpCtrlCfgByCfgNo;
                    sendPara.BusinessObject = cfg;
                    sendPara.CTUID = ctuID;
                    GetProxy.DelInfoByPara(sendPara);
                });
        }

        /// <summary>
        /// 根据灯(组)号删除单灯(组)临时预约
        /// </summary>
        public void DelLampTmpCtrlCfgByNo(string ctuID,int optType,int no)
        {
            AspectF.Define.Retry()
                .Do(() =>
                {
                    pLampTmpCtrlCfg cfg = new pLampTmpCtrlCfg();
                    cfg.OptType = optType;
                    cfg.No = no;
                    LFCDalParameter sendPara = new LFCDalParameter();
                    sendPara.BusinessType = BusinessType.DelLampTmpCtrlCfgByNo;
                    sendPara.BusinessObject = cfg;
                    sendPara.CTUID = ctuID;
                    GetProxy.DelInfoByPara(sendPara);
                });
        }

        /// <summary>
        /// 删除所有单灯(组)周期性临时预约
        /// </summary>
        public void DelAllLampWeekCtrlCfg()
        {
            AspectF.Define.Retry()
                .Do(() =>
                {
                    LFCDalParameter sendPara = new LFCDalParameter();
                    sendPara.BusinessType = BusinessType.DelAllLampWeekCtrlCfg;
                    GetProxy.DelInfoByPara(sendPara);
                });
        }

        /// <summary>
        /// 根据周天参数删除所有单灯(组)周期性临时预约
        /// </summary>
        public void DelLampWeekCtrlCfgByWeekDay(int week)
        {
            AspectF.Define.Retry()
                .Do(() =>
                {
                    LFCDalParameter sendPara = new LFCDalParameter();
                    sendPara.BusinessType = BusinessType.DelLampWeekCtrlCfgByWeekDay;
                    sendPara.BusinessObject = week;
                    GetProxy.DelInfoByPara(sendPara);
                });
        }

        /// <summary>
        /// 新增单灯(组)周期性临时预约
        /// </summary>
        public void AddLampWeekCtrlCfg(tLampWeekCtrlCfg []cfg)
        {
            AspectF.Define.Retry()
                .Do(() =>
                {
                    LFCDalParameter sendPara = new LFCDalParameter();
                    sendPara.BusinessType = BusinessType.AddLampWeekCtrlCfg;
                    sendPara.BusinessObject = cfg;
                    GetProxy.AddInfoByPara(sendPara);
                });
        }

        public void SaveElecData(tLampHisDataRec[] ArrElecData)
        {
            AspectF.Define.Retry()
                .Do(() =>
                {
                    LFCDalParameter sendPara = new LFCDalParameter();
                    sendPara.BusinessType = BusinessType.SaveElecData;
                    sendPara.BusinessObject = ArrElecData;
                    GetProxy.AddInfoByPara(sendPara);
                });
        }

        public tLampTmpCtrlCfg[] QueryLampTempCtrlCfg(pLampTmpCtrlCfg cfg,string ctuID)
        {
            tLampTmpCtrlCfg[] result = new tLampTmpCtrlCfg[0];
            AspectF.Define.Retry()
                .Do(() =>
                {
                    LFCDalParameter sendPara = new LFCDalParameter();
                    sendPara.BusinessType = BusinessType.QueryLampTempCtrlCfg;
                    sendPara.BusinessObject = cfg;
                    sendPara.CTUID = ctuID;
                    LFCDalParameter para = GetProxy.GetInfoByPara(sendPara);
                    result= para.BusinessObject as tLampTmpCtrlCfg[];
                });
            return result;
        }


        //public void UpdateLampTmpCtrlStatus(LampCtrlPlan plan)
        //{
        //    try
        //    {
        //        pLampTmpCtrlCfg cfg = new pLampTmpCtrlCfg();
        //        cfg.OptType = plan.OptType;
        //        cfg.No = plan.CfgNo;
        //        string ctuID = RTUSvrShare.GetShare.GetCtuIDByCtuAddr(plan.CtuAddr);

        //        LFCDalParameter sendPara = new LFCDalParameter();
        //        sendPara.BusinessObject = cfg;
        //        sendPara.BusinessType = BusinessType.UpdateLampTmpCtrlStatus;
        //        sendPara.CTUID = ctuID;

        //        GetProxy.UpdateInfoByPara(sendPara);
        //    }
        //    catch (Exception ex)
        //    {
        //        Console.WriteLine(ex.Message);
        //    }
        //}

        public tLampWeekCtrlCfg[] QueryPlanFromLampWeekCtrlCfg(pLampTmpCtrlCfg cfg, string ctuID)
        {
            tLampWeekCtrlCfg[] result = new tLampWeekCtrlCfg[0];
            AspectF.Define.Retry()
                .Do(() =>
                {
                    LFCDalParameter sendPara = new LFCDalParameter();
                    sendPara.BusinessType = BusinessType.QueryPlanFromLampWeekCtrlCfg;
                    sendPara.BusinessObject = cfg;
                    sendPara.CTUID = ctuID;
                    LFCDalParameter para = GetProxy.GetInfoByPara(sendPara);
                    result= para.BusinessObject as tLampWeekCtrlCfg[];
                });
            return result;
        }

        public void SaveLampGroupCfg(tLampGrpCfg[] data)
        {
            AspectF.Define.Retry()
                .Do(() =>
                {
                    LFCDalParameter sendPara = new LFCDalParameter();
                    sendPara.BusinessType = BusinessType.SaveLampGroupCfg;
                    sendPara.BusinessObject = data;
                    GetProxy.AddInfoByPara(sendPara);
                });
        }

        public void UpdateLampGrpCfgStatus(tLampGrpCfg cfg)
        {
            AspectF.Define.Retry()
                .Do(() =>
                {
                    LFCDalParameter sendPara = new LFCDalParameter();
                    sendPara.BusinessType = BusinessType.UpdateLampGrpCfgStatus;
                    sendPara.BusinessObject = cfg;
                    GetProxy.UpdateInfoByPara(sendPara);
                });
        }
    }
}
