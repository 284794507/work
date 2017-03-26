using CommunicateCore.Utility;
using LFCDal.Model;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CommunicateCore.RTUSvr.RTUSvrBusiness
{
    public class LHInitBusiness
    {
        private static LHInitBusiness _LHInitBusiness;

        public static LHInitBusiness GetLHInitBusiness
        {
            get
            {
                if (_LHInitBusiness == null)
                {
                    _LHInitBusiness = new LHInitBusiness();
                }
                return _LHInitBusiness;
            }
        }

        public void InitHandlerFunction()
        {
            string key = "";
            AspectF.Define.Retry()
                .Do(() =>
                {
                    key = ByteHelper.byteToHexStr(LHCmdWordConst.GetLogin);
                    if (!RTUSvrShare.GetShare.dictAllSendFunction.ContainsKey(key))//登录
                    {
                        RTUSvrShare.GetShare.dictAllSendFunction.Add(key, LoginRTUSvrHandler.GetLoginHandler.SendLoginPackge);
                    }

                    key = ByteHelper.byteToHexStr(LHCmdWordConst.ReplyLogin);
                    if (!RTUSvrShare.GetShare.dictAllHandlerFunction.ContainsKey(key))//登录回复
                    {
                        RTUSvrShare.GetShare.dictAllHandlerFunction.Add(key, LoginRTUSvrHandler.GetLoginHandler.HandlerLoginBackPackage);
                    }

                    key = ByteHelper.byteToHexStr(LHCmdWordConst.ReplyHeartBeat);
                    if (!RTUSvrShare.GetShare.dictAllHandlerFunction.ContainsKey(key))//心跳回复
                    {
                        RTUSvrShare.GetShare.dictAllHandlerFunction.Add(key, HeartBeatHandler.GetHeartBeatHandler.HandlerHeartBeatBackPackage);
                    }

                    key = ByteHelper.byteToHexStr(LHCmdWordConst.SendRTPtuChCtrl);
                    if (!RTUSvrShare.GetShare.dictAllHandlerFunction.ContainsKey(key))//单灯实时控制
                    {
                        RTUSvrShare.GetShare.dictAllHandlerFunction.Add(key, RTLampCtrlFromRTUSvrHandler.GetHandler.HandlerRTLampCtrlPackage);
                    }

                    key = ByteHelper.byteToHexStr(LHCmdWordConst.SendRTCtuChCtrl);
                    if (!RTUSvrShare.GetShare.dictAllHandlerFunction.ContainsKey(key))//箱实时控制
                    {
                        RTUSvrShare.GetShare.dictAllHandlerFunction.Add(key, RTLampCtrlFromRTUSvrHandler.GetHandler.HandlerRTCtuCtrlPackage);
                    }

                    key = ByteHelper.byteToHexStr(LHCmdWordConst.SendRTPtuChCtrlByGroup);
                    if (!RTUSvrShare.GetShare.dictAllHandlerFunction.ContainsKey(key))//单灯组控制
                    {
                        RTUSvrShare.GetShare.dictAllHandlerFunction.Add(key, RTLampGrpCtrlFromRTUSvrHandler.GetHandler.HandlerRTLampGrpCtrlPackage);
                    }

                    key = ByteHelper.byteToHexStr(LHCmdWordConst.SendQueryCTUTime);
                    if (!RTUSvrShare.GetShare.dictAllHandlerFunction.ContainsKey(key))//查询时间
                    {
                        RTUSvrShare.GetShare.dictAllHandlerFunction.Add(key, QueryTimeHandler.GetHandler.HandlerQueryTimePackage);
                    }

                    key = ByteHelper.byteToHexStr(LHCmdWordConst.SendUpdateCTUTime);
                    if (!RTUSvrShare.GetShare.dictAllHandlerFunction.ContainsKey(key))//校时
                    {
                        RTUSvrShare.GetShare.dictAllHandlerFunction.Add(key, UpdateTimeHandler.GetHandler.HandlerUpdateTimePackage);
                    }

                    key = ByteHelper.byteToHexStr(LHCmdWordConst.SendRTQueryLampDetailStatus);
                    if (!RTUSvrShare.GetShare.dictAllHandlerFunction.ContainsKey(key))//查询电参数
                    {
                        RTUSvrShare.GetShare.dictAllHandlerFunction.Add(key, QueryElecDataFromRtuSvrHandler.GetHandler.HandlerQueryElecDataPackage);
                    }

                    key = ByteHelper.byteToHexStr(LHCmdWordConst.SendYearTableCFG);
                    if (!RTUSvrShare.GetShare.dictAllHandlerFunction.ContainsKey(key))//设置年表
                    {
                        RTUSvrShare.GetShare.dictAllHandlerFunction.Add(key, SetYearTableFromRTUSvrHandler.GetHandler.HandlerSetYearTablePackage);
                    }

                    key = ByteHelper.byteToHexStr(LHCmdWordConst.SendQueryYearTableInfo);
                    if (!RTUSvrShare.GetShare.dictAllHandlerFunction.ContainsKey(key))//查询年表
                    {
                        RTUSvrShare.GetShare.dictAllHandlerFunction.Add(key, QueryYearTableFromRTUSvrHandler.GetHandler.HandlerQueryYearTablePackage);
                    }

                    key = ByteHelper.byteToHexStr(LHCmdWordConst.SendUpdateZipFileCmd);
                    if (!RTUSvrShare.GetShare.dictAllHandlerFunction.ContainsKey(key))//远程升级
                    {
                        RTUSvrShare.GetShare.dictAllHandlerFunction.Add(key, UpgradeFromRTUSvrHandler.GetHandler.HandlerUpgradePackage);
                    }
                });
        }
        
        public void InitDBData()
        {
            AspectF.Define.Retry()
                .Do(() =>
                {
                    RTUSvrShare.GetShare.CtuList = DBHandler.GetHandler.GetCtuInfo();

                    RTUSvrShare.GetShare.curRunStatus = DBHandler.GetHandler.GetRunStatus();
                });
        }

    }
}
