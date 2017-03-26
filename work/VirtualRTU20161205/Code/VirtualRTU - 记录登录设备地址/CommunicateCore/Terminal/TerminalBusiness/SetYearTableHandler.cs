using CommunicateCore.Model;
using CommunicateCore.RTUSvr;
using CommunicateCore.Utility;
using LFCDal.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CommunicateCore.Terminal.TerminalBusiness
{
    public class SetYearTableHandler
    {
        private static SetYearTableHandler _SetYearTableHandler;
        public static SetYearTableHandler GetHandler
        {
            get
            {
                if(_SetYearTableHandler==null)
                {
                    _SetYearTableHandler = new SetYearTableHandler();
                }
                return _SetYearTableHandler;
            }
        }

        public void SendYearTableData(BrokerMessage bMsg)
        {//发给虚拟RTU下面的所有单灯
            foreach(tCTUInfo info in RTUSvrShare.GetShare.CtuList)
            {
                foreach(string key in TerminalShare.GetShare.ClientList.Keys)
                {
                    TerminalClient curClient = TerminalShare.GetShare.ClientList[key] as TerminalClient;
                    if(curClient.CtuID==info.CTUID)
                    {
                        byte[] address = ByteHelper.HexStrToByteArrayWithDelimiter(key.Trim(), " ");
                        BrokerMessage sendMsg = new BrokerMessage(MessageType.setYearTable, 0, address, bMsg.MsgBody);
                        TerminalShare.GetShare.SendToTerminal(sendMsg);
                    }
                }
            }
            //foreach (vLampInfo info in TerminalShare.GetShare.LampList)
            //{
            //    string ctuAddr = ByteHelper.bytesToCtuAddr(bMsg.TerminalAddress, true); //BitConverter.ToInt16(bMsg.TerminalAddress, 0).ToString();
            //    if (info.CTUCommAddr== ctuAddr)
            //    {
            //        byte[] address = ByteHelper.HexStrToByteArrayWithDelimiter(info.PtuID.Trim(), " ");
            //        BrokerMessage sendMsg = new BrokerMessage(MessageType.setYearTable, 0, address, bMsg.MsgBody);
            //        TerminalShare.GetShare.SendToTerminal(sendMsg);
            //    }
            //}
        }
    }
}
