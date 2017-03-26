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
    public class HeartBeatHandler
    {//心跳
        private static HeartBeatHandler _HeartBeatHandler;
        public static HeartBeatHandler GetHandler
        {
            get
            {
                if(_HeartBeatHandler==null)
                {
                    _HeartBeatHandler = new HeartBeatHandler();
                }
                return _HeartBeatHandler;
            }
        }

        public void HandlerHeartBeatMessage(BrokerMessage BMsg)
        {
            AspectF.Define.Retry()
                .Do(() =>
                {
                    DateTime recTime = DateTime.Parse(BMsg.MsgBody.ToString());
                    TimeSpan ts = DateTime.Now - recTime;

                    //TerminalClient curClient = TerminalShare.GetShare.ClientList[id] as TerminalClient;
                    TerminalShare.GetShare.CurClient.HeartBeatTime = DateTime.Now;

                    BrokerMessage sendMsg = new BrokerMessage(MessageType.heartBeatBack, 0, BMsg.TerminalAddress, null);
                    TerminalShare.GetShare.SendToTerminal(sendMsg);

                    if (Math.Abs(ts.TotalSeconds) > 30)//时间相差超过30S
                    {
                        Thread.Sleep(1000);
                        SetTimeInfo info = new SetTimeInfo();
                        info.BuildTime(DateTime.Now);
                        BrokerMessage checkMsg = new BrokerMessage(MessageType.setTime, 0, BMsg.TerminalAddress, info);
                        TerminalShare.GetShare.SendToTerminal(checkMsg);
                    }
                });
        }
    }
}
