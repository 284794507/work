using CommunicateCore.Model;
using CommunicateCore.Utility;
using LFCDal.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CommunicateCore.Terminal.TerminalBusiness
{
    public class TerminalInitBusiness
    {
        private static TerminalInitBusiness _TerminalInitBusiness;
        public static TerminalInitBusiness GetInit
        {
            get
            {
                if (_TerminalInitBusiness == null)
                {
                    _TerminalInitBusiness = new TerminalInitBusiness();
                }
                return _TerminalInitBusiness;
            }
        }

        public object LFCCommon { get; private set; }

        public void InitData()
        {
            DBHandler.GetHandler.GetLampList();
        }

        public void InitHandlerFunction()
        {
            AspectF.Define.Retry()
                .Do(() =>
                {
                    if (!TerminalShare.GetShare.dictAllHandlerFunction.ContainsKey(MessageType.login))//登录
                    {
                        TerminalShare.GetShare.dictAllHandlerFunction.Add(MessageType.login, LoginHandler.GetHandler.HandlerLoginBackMessage);
                    }
                    if (!TerminalShare.GetShare.dictAllHandlerFunction.ContainsKey(MessageType.heartBeat))//心跳
                    {
                        TerminalShare.GetShare.dictAllHandlerFunction.Add(MessageType.heartBeat, HeartBeatHandler.GetHandler.HandlerHeartBeatMessage);
                    }
                    if (!TerminalShare.GetShare.dictAllHandlerFunction.ContainsKey(MessageType.alarm))//报警
                    {
                        TerminalShare.GetShare.dictAllHandlerFunction.Add(MessageType.alarm, AlarmHandler.GetHandler.AlarmBackHandler);
                    }
                    if (!TerminalShare.GetShare.dictAllHandlerFunction.ContainsKey(MessageType.queryElecDataBack))//电参数
                    {
                        TerminalShare.GetShare.dictAllHandlerFunction.Add(MessageType.queryElecDataBack, ElecDataHandler.GetHandler.QueryElecDataBackHandler);
                    }
                    if (!TerminalShare.GetShare.dictAllHandlerFunction.ContainsKey(MessageType.realTimeCtrlBack))//实时控制单灯
                    {
                        TerminalShare.GetShare.dictAllHandlerFunction.Add(MessageType.realTimeCtrlBack, RealTimeCtrlLampHandler.GetHandler.RTCtrlBackHandler);
                    }
                    if (!TerminalShare.GetShare.dictAllHandlerFunction.ContainsKey(MessageType.upgradeBack))//远程升级
                    {
                        TerminalShare.GetShare.dictAllHandlerFunction.Add(MessageType.upgradeBack, UpgradeHandler.GetHandler.QueryElecDataBackHandler);
                    }
                    //if (!TerminalShare.GetShare.dictAllHandlerFunction.ContainsKey(MessageType.setYearTableBack))//年表设置
                    //{
                    //    TerminalShare.GetShare.dictAllHandlerFunction.Add(MessageType.upgradeBack, SetYearTableHandler.GetHandler.SendYearTableData);
                    //}
                    if (!TerminalShare.GetShare.dictAllHandlerFunction.ContainsKey(MessageType.queryYearTableBack))//年表查询
                    {
                        TerminalShare.GetShare.dictAllHandlerFunction.Add(MessageType.queryYearTableBack, QueryYearTableHandler.GetHandler.QueryYearTableBackHandler);
                    }
                });
        }

        /// <summary>
        /// 判断设备是否在单灯列表中
        /// </summary>
        /// <param name="ID"></param>
        /// <param name="ptuInfo"></param>
        /// <returns></returns>
        public bool isLFIInList(byte[] ID, ref vLampInfo ptuInfo)//ref JObject ptuInfo)
        {
            bool flag = false;
            vLampInfo rInfo = new vLampInfo();
            AspectF.Define.Retry()
                .Do(() =>
                {
                    string id = ByteHelper.ByteToHexStrWithDelimiter(ID, " ", false);
                    foreach (vLampInfo info in TerminalShare.GetShare.LampList)//foreach (JObject jp in TerminalShare.GetShare.LampList)
                    {
                        if (info.PtuID.Trim() == id)//if (jp["PTUID"].ToString().Trim() == id)
                        {
                            flag = true;
                            rInfo = info;
                            break;
                        }
                    }
                });
            ptuInfo = rInfo;
            return flag;
        }

        /// <summary>
        /// 根据灯号获取单灯信息
        /// </summary>
        /// <param name="lampNo"></param>
        /// <returns></returns>
        public vLampInfo GetLampInfoByLampNo(string ctuAddr, int lampNo)
        {
            vLampInfo ptuInfo = new vLampInfo();
            AspectF.Define.Retry()
                .Do(() =>
                {
                    foreach (vLampInfo info in TerminalShare.GetShare.LampList)
                    {
                        if (info.LampNo == lampNo && ctuAddr == info.CTUCommAddr)
                        {
                            ptuInfo = info;
                            break;
                        }
                    }
                });
            return ptuInfo;
        }

        /// <summary>
        /// 根据组号获取单灯信息
        /// </summary>
        /// <param name="grpNo"></param>
        /// <returns></returns>
        public vLampInfo GetLampInfoByGrpNo(string ctuAddr, int grpNo)
        {
            vLampInfo ptuInfo = new vLampInfo();
            AspectF.Define.Retry()
                .Do(() =>
                {
                    foreach (vLampInfo info in TerminalShare.GetShare.LampList)
                    {
                        if (info.GrpNo == grpNo && ctuAddr == info.CTUCommAddr)
                        {
                            ptuInfo = info;
                            break;
                        }
                    }
                });
            return ptuInfo;
        }

        /// <summary>
        /// 根据灯号获取ptuid
        /// </summary>
        /// <param name="lampNo"></param>
        /// <returns></returns>
        public byte[] GetPtuIDByLampNo(int lampNo)
        {
            byte[] id = new byte[0];
            AspectF.Define.Retry()
                .Do(() =>
                {
                    foreach (vLampInfo info in TerminalShare.GetShare.LampList)
                    {
                        if (info.LampNo == lampNo)
                        {
                            id = ByteHelper.HexStrToByteArrayWithDelimiter(info.PtuID.Trim(), " ");
                            break;
                        }
                    }
                });
            return id;
        }

        /// <summary>
        /// 根据设备地址获取灯号
        /// </summary>
        /// <param name="ptuChNo"></param>
        /// <param name="address"></param>
        /// <returns></returns>
        public int GetLampNoByChNoAndAddress(int ptuChNo, string address)
        {
            int lampNo = -1;
            AspectF.Define.Retry()
                .Do(() =>
                {
                    foreach (vLampInfo info in TerminalShare.GetShare.LampList)
                    {
                        if (info.PtuID.Trim() == address && info.PtuChNo == ptuChNo)
                        {
                            lampNo = info.LampNo;
                            break;
                        }
                    }
                });
            return lampNo;
        }

        /// <summary>
        /// 根据回路号获取单灯信息
        /// </summary>
        /// <param name="lampNo"></param>
        /// <returns></returns>
        public vLampInfo GetLampInfoByChNo(string ctuAddr, int ChNo)
        {
            vLampInfo ptuInfo = new vLampInfo();
            AspectF.Define.Retry()
                .Do(() =>
                {
                    foreach (vLampInfo info in TerminalShare.GetShare.LampList)
                    {
                        if (info.PtuChNo == ChNo && ctuAddr == info.CTUCommAddr)
                        {
                            ptuInfo = info;
                            break;
                        }
                    }
                });
            return ptuInfo;
        }
    }
}
