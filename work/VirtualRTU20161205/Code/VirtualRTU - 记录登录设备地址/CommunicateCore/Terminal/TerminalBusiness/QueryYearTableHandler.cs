using CommunicateCore.Model;
using CommunicateCore.RTUSvr.RTUSvrBusiness;
using CommunicateCore.Utility;
using LFCDal.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CommunicateCore.Terminal.TerminalBusiness
{
    public class QueryYearTableHandler
    {
        private static QueryYearTableHandler _QueryYearTableHandler;
        public static QueryYearTableHandler GetHandler
        {
            get
            {
                if(_QueryYearTableHandler==null)
                {
                    _QueryYearTableHandler = new QueryYearTableHandler();
                }
                return _QueryYearTableHandler;
            }
        }

        public void SendQueryYearTableData(BrokerMessage bMsg)
        {//发给虚拟RTU下面的某个单灯
            foreach (vLampInfo info in TerminalShare.GetShare.LampList)
            {
                string ctuAddr = ByteHelper.bytesToCtuAddr(bMsg.TerminalAddress, true);
                if (info.CTUCommAddr == ctuAddr)
                {
                    byte[] address = ByteHelper.HexStrToByteArrayWithDelimiter(info.PtuID.Trim(), " ");
                    BrokerMessage sendMsg = new BrokerMessage(MessageType.queryYearTable, 0, address, bMsg.MsgBody);
                    TerminalShare.GetShare.SendToTerminal(sendMsg);
                }
                break;
            }
        }

        public void QueryYearTableBackHandler(BrokerMessage bMsg)
        {
            AspectF.Define.Retry()
                .Do(() =>
                {
                    string id = ByteHelper.ByteToHexStrWithDelimiter(bMsg.TerminalAddress, " ", false);
                    if (TerminalShare.GetShare.checkDevIsLoginOrNot(id))
                    {
                        byte[] ctuAddr = TerminalShare.GetShare.GetCtuAddrByPtuAddr(bMsg.TerminalAddress);
                        QueryYearTableFromRTUSvrHandler.GetHandler.QueryYearTableSuccess(ctuAddr, bMsg);
                    }
                });
        }
    }
}
