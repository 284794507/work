using Newtonsoft.Json;
using SuperSocket.SocketBase.Command;
using SuperSocket.SocketBase.Protocol;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Utility;
using Utility.Model;
using Utility.PackageCmdWord;
using Utility.PackageHandler;

namespace TerminalSocket.Command
{
    public class FromLFIProtocol : CommandBase<TerminalSession, BinaryRequestInfo>
    {
        public override string Name
        {
            get
            {
                return ProtocolType.LFI.ToString();
            }
        }

        public override void ExecuteCommand(TerminalSession session, BinaryRequestInfo requestInfo)
        {
            string curID = PackageHelper.GetHelper.GetTerminalIDFromData(requestInfo.Body,3,8);
            //如果客户端不断开连接，直接关闭程序，可能导致session一直存在
            var sessions = session.AppServer.GetAllSessions();
            if (sessions.Count() > 0)
            {
                var list = new List<TerminalSession>(sessions);
                foreach (var item in list)
                {
                    if (item.TerminalID == curID && item.SessionID != session.SessionID)
                    {
                        item.Close();
                    }
                }
            }
            session.TerminalID = curID;
            AnalyzeProtocol_LFI(requestInfo.Body, session);
            //var args = new SocketEventsArgs
            //{
            //    TerminalID = session.TerminalID,
            //    Buffer = requestInfo.Body
            //};
            //session.AppServer.SendBrokerSvr(args);
        }

        /// <summary>
        /// 解析报文
        /// </summary>
        /// <param name="data"></param>
        /// <param name="session"></param>
        private void AnalyzeProtocol_LFI(byte[]data, TerminalSession session)
        {
            byte[] afterPackge = null;
            LFIPackageHandler package = null;

            do
            {
                package = new LFIPackageHandler();
                if (afterPackge == null)
                {
                    package.BuildPackageFromBytes(data);
                }
                else
                {
                    package.BuildPackageFromBytes(afterPackge);
                }
                afterPackge = package.afterPackage;

                SendPackgeData(package, session);
            }
            while (afterPackge != null);
        }

        /// <summary>
        /// 转发报文
        /// </summary>
        /// <param name="package"></param>
        /// <param name="session"></param>
        private void SendPackgeData(LFIPackageHandler package, TerminalSession session)
        {
            string msg = "";
            AspectF.Define.Retry(UtilityHelper.GetHelper.CatchExpection)
                .Log(UtilityHelper.GetHelper.WriterLog,"", msg)
                .Do(() =>
                {
                    if (package.AnalySuccess)
                    {
                        msg = "Recevice right data" + "\r\n";
                        msg += "CmdWord：" + ByteHelper.ByteToHexStrWithDelimiter(package.CmdWord, " ", false) + "\r\n";

                        //byte[] data = new byte[0];
                        BrokerMessage bMsg = new BrokerMessage();
                        bMsg.TerminalAddress = package.DevAddr;
                        GetBrokerMsgFromPackge(package.CmdWord, package.OnlyData, ref bMsg);
                        //string jsonStr = JsonConvert.SerializeObject(bMsg);
                        byte[] data = bMsg.ToBytes();// Encoding.UTF8.GetBytes('!' + jsonStr + '$');

                        var args = new SocketEventsArgs
                        {
                            TerminalID = session.TerminalID,
                            Buffer = data
                        };
                        session.AppServer.SendBrokerSvr(args);
                    }
                    else
                    {
                        msg = "Recevice error data" + "\r\n";
                    }
                    msg += ByteHelper.ByteToHexStrWithDelimiter(package.ToBytes(), " ", false);
                    UtilityHelper.GetHelper.WriteLog_RTUSvr(msg);
                });         
        }

        /// <summary>
        /// 根据报文组件消息对象
        /// </summary>
        /// <param name="cmdWord"></param>
        /// <param name="data"></param>
        /// <param name="bMsg"></param>
        private void GetBrokerMsgFromPackge(byte[] cmdWord, byte[] data, ref BrokerMessage bMsg)
        {
            int curIndex = 0;
            BrokerMessage curMsg = bMsg;
            AspectF.Define.Retry(UtilityHelper.GetHelper.CatchExpection)
                .Do(() =>
                {
                    byte seq = data[curIndex++];
                    byte[] time = new byte[7];
                    DateTime curTime = DateTime.MinValue;
                    if (ByteHelper.GetBit(seq, 0) == 1)
                    {
                        Buffer.BlockCopy(data, curIndex, time, 0, 7);
                        curTime = ByteHelper.Bytes7ToDateTime(time, false);
                        curIndex += 7;
                    }
                    if (ByteHelper.GetBit(seq, 1) == 1)
                    {
                        curIndex += 2;
                    }
                    if (ByteHelper.ByteArryEquals(cmdWord, LFICmdWordConst.Login))//登录
                    {
                        curMsg.MsgType = MessageType.login;
                        LoginInfo info = new LoginInfo();
                        info.BuildLoginInfo(data);
                        curMsg.MsgBody = info;
                    }
                    else if (ByteHelper.ByteArryEquals(cmdWord, LFICmdWordConst.HeartBeat))//心跳
                    {
                        curMsg.MsgType = MessageType.heartBeat;
                        AlarmInfo info = new AlarmInfo();

                        if (ByteHelper.GetBit(seq, 0) == 1)
                        {
                            curMsg.MsgBody = curTime;
                        }
                        else
                        {
                            curMsg.MsgBody = DateTime.Now;
                        }
                    }
                    else if (ByteHelper.ByteArryEquals(cmdWord, LFICmdWordConst.RealTimeControlBack))//实时控制
                    {
                        curMsg.MsgType = MessageType.realTimeCtrlBack;
                        curMsg.MsgBody = data[0];//单灯状态
                    }
                    else if (ByteHelper.ByteArryEquals(cmdWord, LFICmdWordConst.Alarm))//报警
                    {
                        curMsg.MsgType = MessageType.alarm;
                        AlarmInfo info = new AlarmInfo();
                        info.BuildAlarmInfo(data);
                        curMsg.MsgBody = info;
                    }
                    else if (ByteHelper.ByteArryEquals(cmdWord, LFICmdWordConst.QueryElecDataBack))//电参数
                    {
                        curMsg.MsgType = MessageType.queryElecDataBack;
                        QueryElecData info = new QueryElecData();
                        info.BuildElecDataInfo(data);
                        curMsg.MsgBody = info;
                    }
                    else if (ByteHelper.ByteArryEquals(cmdWord, LFICmdWordConst.SetYearTableBack))//设置年表
                    {
                        curMsg.MsgType = MessageType.setYearTableBack;
                        curMsg.MsgBody = data[curIndex];
                    }
                    else if (ByteHelper.ByteArryEquals(cmdWord, LFICmdWordConst.QueryYearTableBack))//查询年表
                    {
                        curMsg.MsgType = MessageType.queryYearTableBack;
                        YearTableInfo info = new YearTableInfo();
                        info.BuildYearTableInfo(data);
                        curMsg.MsgBody = info;
                    }
                    else if (ByteHelper.ByteArryEquals(cmdWord, LFICmdWordConst.UpgradeBack))//远程升级
                    {
                        curMsg.MsgType = MessageType.upgradeBack;
                        curMsg.MsgBody = data[curIndex];
                    }
                    else if (ByteHelper.ByteArryEquals(cmdWord, LFICmdWordConst.QueryNetWorkParaBack))//查询网络参数
                    {
                        curMsg.MsgType = MessageType.queryNetWorkParamterBack;
                        NetWorkInfo info = new NetWorkInfo();
                        info.BuildNetWorkInfo(data);
                        curMsg.MsgBody = info;
                    }
                    else if (ByteHelper.ByteArryEquals(cmdWord, LFICmdWordConst.QueryEthernetInterfaceBack))//查询网口参数
                    {
                        curMsg.MsgType = MessageType.queryEthernetInterfaceBack;
                        EthernetInterface info = new EthernetInterface();
                        info.BuildEthernetInterfaceInfo(data);
                        curMsg.MsgBody = info;
                    }

                });
            bMsg = curMsg;
        }        
    }
}
