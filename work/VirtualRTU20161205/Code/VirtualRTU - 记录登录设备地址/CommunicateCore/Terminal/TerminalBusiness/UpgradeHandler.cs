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
    public class UpgradeHandler
    {
        private static UpgradeHandler _UpgradeHandler;
        public static UpgradeHandler GetHandler
        {
            get
            {
                if(_UpgradeHandler==null)
                {
                    _UpgradeHandler = new UpgradeHandler();
                }
                return _UpgradeHandler;
            }
        }

        public void SendUpgradeInfo(string ctuAddr, int lampNo, UpgradeInfo info)
        {
            string errMsg = "";
            vLampInfo lampInfo = new vLampInfo();
            AspectF.Define.Retry()
                .Log(TerminalShare.GetShare.WriterLog, "", errMsg)
                .Do(() =>
                {
                    lampInfo = TerminalInitBusiness.GetInit.GetLampInfoByLampNo(ctuAddr, lampNo);
                    if (!string.IsNullOrEmpty(lampInfo.PtuID))
                    {
                        if (TerminalShare.GetShare.checkDevIsLoginOrNot(lampInfo.PtuID.Trim()))
                        {
                            byte[] address = ByteHelper.HexStrToByteArrayWithDelimiter(lampInfo.PtuID.Trim(), " ");
                            BrokerMessage sendMsg = new BrokerMessage(MessageType.upgrade, 0, address, info);
                            TerminalShare.GetShare.SendToTerminal(sendMsg, lampNo);
                        }
                        errMsg = string.IsNullOrEmpty(lampInfo.PtuID) ? "SendUpgradeInfo：" + "灯号异常！" : "";
                    }
                });
        }

        public void QueryElecDataBackHandler(BrokerMessage BMsg)
        {
            AspectF.Define.Retry()
                .Do(() =>
                {
                    //QueryElecData info = BMsg.MsgBody as QueryElecData;
                    //string jsonStr = ((Newtonsoft.Json.Linq.JToken)BMsg.MsgBody).Root.ToString();
                    //QueryElecData info = JsonSerializeHelper.GetHelper.Deserialize<QueryElecData>(jsonStr);
                    int lampNo = TerminalShare.GetShare.GetSendNoByCmdWord(BMsg);
                    if (lampNo > 0)
                    {
                        UpgradeFromRTUSvrHandler.GetHandler.UpgradeSuccess(BMsg, lampNo);
                    }
                });
        }
    }
}
