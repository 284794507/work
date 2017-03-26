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
    public class RealTimeCtrlLampHandler
    {//实时控制单灯
        private static RealTimeCtrlLampHandler _RealTimeCtrlLampHandler;
        public static RealTimeCtrlLampHandler GetHandler
        {
            get
            {
                if(_RealTimeCtrlLampHandler==null)
                {
                    _RealTimeCtrlLampHandler = new RealTimeCtrlLampHandler();
                }
                return _RealTimeCtrlLampHandler;
            }
        }

        /// <summary>
        /// opt=00 关灯； = 01 开灯
        /// isLock=0x00，表示本次操作之后，不影响自主控制命令
        /// isLock=0x01，表示本次操作之后，本设备的自主控制命令失效；不管是否涉及到本次控制的回路情况，均失效
        /// </summary>
        /// <param name="lampNo"></param>
        /// <param name="isLock"></param>
        public void RTCtrlByLampNo(string ctuAddr, int lampNo, byte opt, byte isLock)
        {
            string errMsg = "";
            vLampInfo lampInfo = new vLampInfo();
            AspectF.Define.Retry()
                .Log(TerminalShare.GetShare.WriterLog, "", errMsg)
                .Do(() =>
                {
                    byte[] address = new byte[7];
                    byte ptuChNo = 0x00;
                    if (lampNo > 0)
                    {
                        lampInfo = TerminalInitBusiness.GetInit.GetLampInfoByLampNo(ctuAddr, lampNo);
                    }
                    if (!string.IsNullOrEmpty(lampInfo.PtuID))
                    {
                        if (TerminalShare.GetShare.checkDevIsLoginOrNot(lampInfo.PtuID.Trim()))
                        {
                            address = ByteHelper.HexStrToByteArrayWithDelimiter(lampInfo.PtuID.Trim(), " ");
                            ptuChNo = lampInfo.PtuChNo;
                        }
                        else
                        {
                            address = ByteHelper.HexStrToByteArrayWithDelimiter(lampInfo.PtuID.Trim(), " ");
                            byte[] addr = ByteHelper.CtuAddrToBytes(ctuAddr);
                            RTLampCtrlFromRTUSvrHandler.GetHandler.SendRTCtrlBackFail(addr, address, 0x01);
                            return;
                        }
                        RealTimeCtrlLamp rtCtrl = new RealTimeCtrlLamp();
                        rtCtrl.ChNo = ptuChNo;
                        rtCtrl.OptValue = opt;
                        rtCtrl.IsLock = isLock;
                        BrokerMessage sendMsg = new BrokerMessage(MessageType.realTimeCtrl, 0, address, rtCtrl);
                        RTCtrlByLampNo(sendMsg, lampNo);
                        errMsg = string.IsNullOrEmpty(lampInfo.PtuID) ? "RTCtrlByLampNo：" + "灯号异常！" : "";
                    }
                });
        }

        public void RTCtrlByLampNo(BrokerMessage sendMsg,int lampNo)
        {
            TerminalShare.GetShare.SendToTerminal(sendMsg, lampNo);
        }

        public void RTCtrlBackHandler(BrokerMessage bMsg)
        {
            AspectF.Define.Retry()
                .Do(() =>
                {
                    byte optRet =(byte)bMsg.MsgBody;
                    int lampNo = TerminalShare.GetShare.GetSendNoByCmdWord(bMsg);
                    byte[] ctuAddr = TerminalShare.GetShare.GetCtuAddrByPtuAddr(bMsg.TerminalAddress);

                    RTLampCtrlFromRTUSvrHandler.GetHandler.SendRTCtrlBackSuccess(ctuAddr, lampNo, optRet);
                });
        }

        public void RTCtrlNoRecevice(BrokerMessage bMsg)
        {
            AspectF.Define.Retry()
                .Do(() =>
                {
                    byte[] addr = TerminalShare.GetShare.GetCtuAddrByPtuAddr(bMsg.TerminalAddress);
                    RTLampCtrlFromRTUSvrHandler.GetHandler.SendRTCtrlBackFail(addr, bMsg.TerminalAddress, 0x01);
                });
        }
    }
}
