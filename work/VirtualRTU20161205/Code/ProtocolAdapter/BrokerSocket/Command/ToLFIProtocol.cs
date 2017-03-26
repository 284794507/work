using Newtonsoft.Json;
using SuperSocket.SocketBase.Command;
using SuperSocket.SocketBase.Protocol;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Utility;
using Utility.Model;
using Utility.PackageCmdWord;
using Utility.PackageHandler;

namespace BrokerSocket.Command
{
    public class ToLFIProtocol:CommandBase<BrokerSession,BinaryRequestInfo>
    {
        public override string Name
        {
            get
            {
                return ProtocolType.LFI.ToString();
            }
        }

        public static BrokerMessage curMsg = new BrokerMessage();

        public override void ExecuteCommand(BrokerSession session, BinaryRequestInfo requestInfo)
        {
            AspectF.Define.Retry(UtilityHelper.GetHelper.CatchExpection)
                .Do(() =>
                {
                    string jsonStr = Encoding.UTF8.GetString(requestInfo.Body, 0, requestInfo.Body.Length);
                    if (string.IsNullOrEmpty(jsonStr)) return;
                    //JsonSerializer serializer = new JsonSerializer();
                    //StringReader sr = new StringReader(jsonStr);
                    //object obj = serializer.Deserialize(new JsonTextReader(sr), typeof(BrokerMessage));
                    BrokerMessage bMsg = JsonSerializeHelper.GetHelper.Deserialize<BrokerMessage>(jsonStr);
                    if (bMsg == null || bMsg.TerminalAddress == null) return;
                    UtilityHelper.GetHelper.WriterLog("MessageType："+bMsg.MsgType.ToString());
                    //BrokerMessage bMsg = curMsg;
                    string curID = ByteHelper.byteToHexStr(bMsg.TerminalAddress);
                    //如果客户端不断开连接，直接关闭程序，可能导致session一直存在
                    var sessions = session.AppServer.GetAllSessions();
                    if (sessions.Count() > 0)
                    {
                        var list = new List<BrokerSession>(sessions);
                        foreach (var item in list)
                        {
                            if (item.BrokerID == curID && item.SessionID != session.SessionID)
                            {
                                item.Close();
                            }
                        }
                    }
                    session.BrokerID = curID;
                    if (bMsg.MsgType == MessageType.setYearTable)
                    {
                        DivPackagePara.total = 2;
                        DivPackagePara.curNo = 1;
                    }
                    var args = new SocketEventsArgs
                    {
                        TerminalID = session.BrokerID,
                        Buffer = GetDataFromBrokerMsg(bMsg)
                    };
                    session.AppServer.BroadcastDataToTerminal(args);
                    while (DivPackagePara.curNo < DivPackagePara.total)
                    {
                        Thread.Sleep(4000);
                        DivPackagePara.curNo++;
                        args = new SocketEventsArgs
                        {
                            TerminalID = session.BrokerID,
                            Buffer = GetDataFromBrokerMsg(bMsg)
                        };
                        session.AppServer.BroadcastDataToTerminal(args);
                    }
                });
        }

        /// <summary>
        /// 组装报文
        /// </summary>
        /// <param name="bMsg"></param>     
        private byte[] GetDataFromBrokerMsg(BrokerMessage bMsg)
        {
            LFIPackageHandler package = null;
            AspectF.Define.Retry(UtilityHelper.GetHelper.CatchExpection)
                .Do(() =>
                {
                    byte[] data = new byte[0];
                    byte[] cmdWord = new byte[2];
                    string jsonStr = "";
                    if (bMsg.MsgBody != null)
                    {
                        jsonStr = ((Newtonsoft.Json.Linq.JToken)bMsg.MsgBody).Root.ToString();
                        UtilityHelper.GetHelper.WriterLog("MessageBody：" + jsonStr);
                    }
                    switch (bMsg.MsgType)
                    {
                        case MessageType.loginBack://登录返回
                            LoginBackInfo loginBackinfo = JsonSerializeHelper.GetHelper.Deserialize<LoginBackInfo>(jsonStr);
                            data = loginBackinfo.ToBytes();
                            cmdWord = LFICmdWordConst.LoginBack;
                            break;
                        case MessageType.heartBeatBack://心跳返回
                            cmdWord = LFICmdWordConst.HeartBeatBack;
                            data = new byte[1];
                            data[0] = 0;
                            break;
                        case MessageType.setTime://设置时间
                            SetTimeInfo setTime = JsonSerializeHelper.GetHelper.Deserialize<SetTimeInfo>(jsonStr);
                            cmdWord = LFICmdWordConst.SetTime;
                            data = setTime.ToBytes();
                            break;
                        case MessageType.realTimeCtrl://实时控制
                            RealTimeCtrlLamp rtCtrl = JsonSerializeHelper.GetHelper.Deserialize<RealTimeCtrlLamp>(jsonStr);
                            cmdWord = LFICmdWordConst.RealTimeControl;
                            data = rtCtrl.ToBytes();
                            break;
                        case MessageType.queryElecData://查询电参数
                            QueryElecData elecData = JsonSerializeHelper.GetHelper.Deserialize<QueryElecData>(jsonStr);
                            data = elecData.ToBytes();
                            cmdWord = LFICmdWordConst.QueryElecData;
                            break;
                        case MessageType.setYearTable://设置年表
                            YearTableInfo sYearTable = JsonSerializeHelper.GetHelper.Deserialize<YearTableInfo>(jsonStr);
                            data = sYearTable.ToSetBytes();
                            cmdWord = LFICmdWordConst.SetYearTable;
                            break;
                        case MessageType.queryYearTable://查询年表
                            YearTableInfo qYearTable = JsonSerializeHelper.GetHelper.Deserialize<YearTableInfo>(jsonStr);
                            data = qYearTable.ToQueryBytes();
                            cmdWord = LFICmdWordConst.QueryYearTable;
                            break;
                        case MessageType.upgrade://远程升级
                            UpgradeInfo upgrade = JsonSerializeHelper.GetHelper.Deserialize<UpgradeInfo>(jsonStr);
                            data = upgrade.ToBytes();
                            cmdWord = LFICmdWordConst.Upgrade;
                            break;
                        case MessageType.setNetWorkParamter://设置网络参数
                            NetWorkInfo netWork = JsonSerializeHelper.GetHelper.Deserialize<NetWorkInfo>(jsonStr);
                            data = netWork.ToBytes();
                            cmdWord = LFICmdWordConst.SetNetWorkPara;
                            break;
                        case MessageType.queryNetWorkParamter://查询网络参数
                            data = new byte[1];
                            data[0] = 0;
                            cmdWord = LFICmdWordConst.QueryNetWorkPara;
                            break;
                        case MessageType.setEthernetInterface://设置网口参数
                            EthernetInterface eInterface = JsonSerializeHelper.GetHelper.Deserialize<EthernetInterface>(jsonStr);
                            data = eInterface.ToBytes();
                            cmdWord = LFICmdWordConst.SetEthernetInterface;
                            break;
                        case MessageType.queryEthernetInterface://查询网口参数
                            data = new byte[1];
                            data[0] = 0;
                            cmdWord = LFICmdWordConst.QueryEthernetInterface;
                            break;
                    }

                    package = new LFIPackageHandler(bMsg.TerminalAddress, data, cmdWord);
                });
            return package.ToBytes();
        }
    }
}
