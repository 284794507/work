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

        public void SendQueryElecData(string ctuAddr, int lampNo)
        {
            string errMsg = "";
            vLampInfo lampInfo = new vLampInfo();
            AspectF.Define.Retry()
                .Log(TerminalShare.GetShare.WriterLog,"", errMsg)
                .Do(() =>
                {
                    lampInfo = TerminalInitBusiness.GetInit.GetLampInfoByLampNo(ctuAddr, lampNo);
                    if (!string.IsNullOrEmpty(lampInfo.PtuID))
                    {
                        if (TerminalShare.GetShare.checkDevIsLoginOrNot(lampInfo.PtuID.Trim()))
                        {
                            byte[] address = ByteHelper.HexStrToByteArrayWithDelimiter(lampInfo.PtuID.Trim(), " ");
                            QueryElecData info = new QueryElecData();
                            info.StartChNo = lampInfo.PtuChNo;
                            info.ChNum = 1;
                            BrokerMessage sendMsg = new BrokerMessage(MessageType.queryElecData, 0, address, info);
                        TerminalShare.GetShare.SendToTerminal(sendMsg,lampNo);
                        }
                        else
                        {
                            byte[] address = ByteHelper.HexStrToByteArrayWithDelimiter(lampInfo.PtuID.Trim(), " ");
                            byte[] addr = ByteHelper.CtuAddrToBytes(ctuAddr);
                            QueryElecDataFromRtuSvrHandler.GetHandler.QueryElecDataFail(addr, address, 0x03);
                        }
                        errMsg = string.IsNullOrEmpty(lampInfo.PtuID) ? "SendQueryElecData：" + "灯号异常！" : "";
                    }
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

                            string ctuID = TerminalShare.GetShare.GetCtuIDByPtuAddr(ptuAddr);
                            for (int i = 0; i < info.ChNum; i++)
                            {
                                tLampHisDataRec elecData = TerminalShare.GetShare.GetElecDataFromBytes(info.ArrElecData[i]);
                                elecData.LampNo = TerminalInitBusiness.GetInit.GetLampNoByChNoAndAddress(i + info.StartChNo, ptuAddr);
                                elecData.CTUID = ctuID;
                                elecData.UpLoadDTime = DateTime.Now;
                                elecData.GetDateTime = DateTime.Now;
                                elecData.isUpLoaded = 1;
                                elecData.Memo = "";
                                listDataRec.Add(elecData);
                            }
                            tLampHisDataRec[] ArrElecData = listDataRec.ToArray();
                            DBHandler.GetHandler.SaveElecData(ArrElecData);

                            byte[] ctuAddr = TerminalShare.GetShare.GetCtuAddrByPtuAddr(BMsg.TerminalAddress);

                            for (int i = 0; i < ArrElecData.Length; i++)
                            {
                                if (ArrElecData[i].LampNo == lampNo)
                                {
                                    QueryElecDataFromRtuSvrHandler.GetHandler.QueryElecDataSuccess(ctuAddr, ArrElecData[i]);
                                    break;
                                }
                            }
                        }
                    }
                });
        }

        public void QueryElecDataNoRecevice(BrokerMessage BMsg)
        {
            AspectF.Define.Retry()
                .Do(() =>
                {
                    byte[] addr = TerminalShare.GetShare.GetCtuAddrByPtuAddr(BMsg.TerminalAddress);
                    QueryElecDataFromRtuSvrHandler.GetHandler.QueryElecDataFail(addr, BMsg.TerminalAddress, 0x03);
                });
        }
    }
}
