using CommunicateCore.Model;
using CommunicateCore.Utility;
using LFCDal.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Utility;
using Utility.Model;

namespace CommunicateCore.Terminal.TerminalBusiness
{
    public class ElecDataHandler
    {
        private static ElecDataHandler _ElecDataHandler;
        public static ElecDataHandler GetHandler
        {
            get
            {
                if(_ElecDataHandler==null)
                {
                    _ElecDataHandler = new ElecDataHandler();
                }
                return _ElecDataHandler;
            }
        }

        public void SendQueryElecData(byte[] address)
        {
            string errMsg = "";
            AspectF.Define.Retry()
                .Log(TerminalShare.GetShare.WriterLog,"", errMsg)
                .Do(() =>
                {
                    QueryElecData info = new QueryElecData();
                    info.StartChNo = 1;
                    info.ChNum = 2;
                    BrokerMessage sendMsg = new BrokerMessage(MessageType.queryElecData, 0, address, info);
                    TerminalShare.GetShare.SendToTerminal(sendMsg, 0);
                });
        }

        public void QueryElecDataBackHandler(BrokerMessage BMsg)
        {
            AspectF.Define.Retry()
                .Do(() =>
                {
                    string id = ByteHelper.ByteToHexStrWithDelimiter(BMsg.TerminalAddress, " ", false);
                    if (TerminalShare.GetShare.checkDevIsLoginOrNot(id))
                    {
                        string jsonStr = ((Newtonsoft.Json.Linq.JToken)BMsg.MsgBody).Root.ToString();
                        QueryElecData info = JsonSerializeHelper.GetHelper.Deserialize<QueryElecData>(jsonStr);
                        int lampNo = TerminalShare.GetShare.GetSendNoByCmdWord(BMsg);
                        if (lampNo > 0)
                        {
                            List<tLampHisDataRec> listDataRec = new List<tLampHisDataRec>();
                            string ptuAddr = ByteHelper.ByteToHexStrWithDelimiter(BMsg.TerminalAddress, " ", false);
                            
                            for (int i = 0; i < info.ChNum; i++)
                            {
                                tLampHisDataRec elecData = TerminalShare.GetShare.GetElecDataFromBytes(info.ArrElecData[i]);
                                listDataRec.Add(elecData);
                            }
                            tLampHisDataRec[] ArrElecData = listDataRec.ToArray();
                        }
                    }
                });
        }

        //public void QueryElecDataNoRecevice(BrokerMessage BMsg)
        //{
        //    AspectF.Define.Retry()
        //        .Do(() =>
        //        {
        //            byte[] addr = TerminalShare.GetShare.GetCtuAddrByPtuAddr(BMsg.TerminalAddress);
        //            QueryElecDataFromRtuSvrHandler.GetHandler.QueryElecDataFail(addr, BMsg.TerminalAddress, 0x03);
        //        });
        //}
    }
}
