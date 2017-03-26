using CommunicateCore.Model;
using CommunicateCore.Utility;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Utility;
using Utility.Model;

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
                    if (!TerminalShare.GetShare.dictAllHandlerFunction.ContainsKey(MessageType.queryElecDataBack))//电参数
                    {
                        TerminalShare.GetShare.dictAllHandlerFunction.Add(MessageType.queryElecDataBack, ElecDataHandler.GetHandler.QueryElecDataBackHandler);
                    }
                    //if (!TerminalShare.GetShare.dictAllHandlerFunction.ContainsKey(MessageType.realTimeCtrlBack))//实时控制单灯
                    //{
                    //    TerminalShare.GetShare.dictAllHandlerFunction.Add(MessageType.realTimeCtrlBack, RealTimeCtrlLampHandler.GetHandler.RTCtrlBackHandler);
                    //}
                });
        }        
    }
}
