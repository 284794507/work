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
    public class AlarmHandler
    {//报警
        private static AlarmHandler _AlarmHandler;
        public static AlarmHandler GetHandler
        {
            get
            {
                if (_AlarmHandler == null)
                {
                    _AlarmHandler = new AlarmHandler();
                }
                return _AlarmHandler;
            }
        }

        public void AlarmBackHandler(BrokerMessage BMsg)
        {
            AspectF.Define.Retry()
                .Do(() =>
                {
                    string id = ByteHelper.ByteToHexStrWithDelimiter(BMsg.TerminalAddress, " ", false);
                    if (TerminalShare.GetShare.checkDevIsLoginOrNot(id))
                    {
                        //目前只服光源故障（灯内故障）
                        //AlarmInfo info = BMsg.MsgBody as AlarmInfo;
                        string jsonStr = ((Newtonsoft.Json.Linq.JToken)BMsg.MsgBody).Root.ToString();
                        AlarmInfo info = JsonSerializeHelper.GetHelper.Deserialize<AlarmInfo>(jsonStr);
                        int len1 = 16, len2 = 2;
                        for (int i = 0; i < len1; i++)
                        {
                            for (int j = 0; j < len2; j++)
                            {
                                byte status = info.ChAlarmInfo[i, 0];
                                if (ByteHelper.GetBit(status, 2) == 1)
                                {
                                    string ptuAddr = ByteHelper.ByteToHexStrWithDelimiter(BMsg.TerminalAddress, " ", false);
                                    int lampNo = TerminalInitBusiness.GetInit.GetLampNoByChNoAndAddress(i, ptuAddr);
                                    byte[] ctuAddr = TerminalShare.GetShare.GetCtuAddrByPtuAddr(BMsg.TerminalAddress);
                                    LampStatusChangedHandler.GetHandler.LampStatusChangedSuccess(ctuAddr, lampNo);
                                }
                            }
                        }
                    }
                });
        }
    }
}
