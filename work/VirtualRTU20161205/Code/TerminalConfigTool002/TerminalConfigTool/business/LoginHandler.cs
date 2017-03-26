using CommunicateCore.Model;
using CommunicateCore.Utility;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Utility;
using Utility.Model;

namespace CommunicateCore.Terminal.TerminalBusiness
{
    public class LoginHandler
    {//登录
        private static LoginHandler _TerminalLoginHandler;
        public static LoginHandler GetHandler
        {
            get
            {
                if(_TerminalLoginHandler == null)
                {
                    _TerminalLoginHandler = new LoginHandler();
                }
                return _TerminalLoginHandler;
            }
        }

        public void HandlerLoginBackMessage(BrokerMessage BMsg)
        {
            string id = "";
            AspectF.Define.Retry()
                .Log(TerminalShare.GetShare.WriterLog,"", "HandlerLoginPackage：" + "设备：" + id + " 登录成功！")
                .Do(() =>
                {
                    TerminalShare.GetShare.CurClient = new TerminalClient();
                    //LoginInfo lInfo = BMsg.MsgBody as LoginInfo;
                    string jsonStr = ((Newtonsoft.Json.Linq.JToken)BMsg.MsgBody).Root.ToString();
                    LoginInfo lInfo = JsonSerializeHelper.GetHelper.Deserialize<LoginInfo>(jsonStr);
                    id = ByteHelper.ByteToHexStrWithDelimiter(BMsg.TerminalAddress, " ", false);
                    //if (TerminalShare.GetShare.ClientList.ContainsKey(id))
                    //{
                    //    curClient = TerminalShare.GetShare.ClientList[id] as TerminalClient;
                    //    curClient.LoginNum++;
                    //}
                    //else
                    //{
                    //    TerminalShare.GetShare.ClientList.Add(id, curClient);
                    //    curClient.LoginNum = 1;
                    //    MonitorLFIHeartBeat();
                    //}
                    int len = BMsg.TerminalAddress.Length;
                    TerminalShare.GetShare.CurClient.TerminalAddr = new byte[len];
                    Buffer.BlockCopy(BMsg.TerminalAddress, 0, TerminalShare.GetShare.CurClient.TerminalAddr, 0, len);
                    TerminalShare.GetShare.CurClient.Addr = id;

                    TerminalShare.GetShare.CurClient.HeartBeatTime = DateTime.Now;
                    TerminalShare.GetShare.CurClient.LoginTime = DateTime.Now;

                    LoginBackInfo backInfo = new LoginBackInfo();
                    backInfo.LoginReuslt = 1;
                    MonitorLFIHeartBeat();
                    BrokerMessage sendMsg = new BrokerMessage(MessageType.loginBack, 0, BMsg.TerminalAddress, backInfo);
                    TerminalShare.GetShare.SendToTerminal(sendMsg);
                });
        }

        /// <summary>
        /// 监测心跳包
        /// </summary>
        public void MonitorLFIHeartBeat()
        {
            AspectF.Define.Retry(MonitorLFIHeartBeat)
                .Do(() =>
                {
                    ThreadPool.QueueUserWorkItem(
                    delegate
                    {
                        while (true)
                        {
                            Thread.Sleep(60000 * TerminalShare.GetShare.HeartBeatInterval);
                            List<string> delClient = new List<string>();
                            foreach (string macAddr in TerminalShare.GetShare.ClientList.Keys)
                            {
                                TerminalClient dev = TerminalShare.GetShare.ClientList[macAddr] as TerminalClient;
                                if (dev != null)
                                {
                                    TimeSpan ts = DateTime.Now - dev.HeartBeatTime;
                                    if (ts.Minutes >= TerminalShare.GetShare.HeartBeatInterval * 3)
                                    {
                                        dev.isHasHeartBeat = false;
                                        delClient.Add(macAddr);
                                    }
                                }
                            }
                            foreach (string addr in delClient)//心跳过期则删除
                            {
                                TerminalShare.GetShare.ClientList.Remove(addr);
                            }
                        }
                    }
                    );
                });
        }
    }
}
