using CommunicateCore.Model;
using CommunicateCore.Terminal;
using CommunicateCore.Utility;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TerminalConfigTool.model;
using Utility;
using Utility.Model;

namespace TerminalConfigTool.business
{
    public class EthernetInterfaceHandler
    {
        private static EthernetInterfaceHandler _EthernetInterfaceHandler;
        public static EthernetInterfaceHandler Instance
        {
            get
            {
                if(_EthernetInterfaceHandler==null)
                {
                    _EthernetInterfaceHandler = new EthernetInterfaceHandler();
                }
                return _EthernetInterfaceHandler;
            }
        }

        public void SendQueryEthernetInterface(byte[] address)
        {
            string errMsg = "";
            AspectF.Define.Retry()
                .Log(TerminalShare.GetShare.WriterLog, "", errMsg)
                .Do(() =>
                {
                    byte[] devAddr = TerminalShare.GetShare.CurClient.TerminalAddr;
                    BrokerMessage sendMsg = new BrokerMessage();
                    sendMsg.MsgType = MessageType.queryEthernetInterface;
                    sendMsg.TerminalAddress = devAddr;
                    
                    TerminalShare.GetShare.SendToTerminal(sendMsg, 0);
                });
        }

        public void QueryEthernetInterfaceBackHandler(BrokerMessage BMsg)
        {
            AspectF.Define.Retry()
                .Do(() =>
                {
                    string id = ByteHelper.ByteToHexStrWithDelimiter(BMsg.TerminalAddress, " ", false);
                    if (TerminalShare.GetShare.checkDevIsLoginOrNot(id))
                    {
                        if (BMsg != null)
                        {
                            if (BMsg.MsgType == MessageType.queryEthernetInterfaceBack)
                            {
                                string jsonStr = ((Newtonsoft.Json.Linq.JToken)BMsg.MsgBody).Root.ToString();
                                EthernetInterface info = JsonSerializeHelper.GetHelper.Deserialize<EthernetInterface>(jsonStr);
                                for (int i = 0; i < info.CfgNum; i++)
                                {
                                    ConfigDetail sInfo = info.CfgDetail[i];
                                    
                                }
                            }
                        }  
                            
                    }
                });
        }
    }
}
