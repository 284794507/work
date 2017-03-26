using CommunicateCore.Model;
using CommunicateCore.Terminal;
using CommunicateCore.Terminal.TerminalBusiness;
using CommunicateCore.Utility;
using LFCDal.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CommunicateCore.RTUSvr.RTUSvrBusiness
{
    //实时控制
    public class RTLampCtrlFromRTUSvrHandler
    {
        private static RTLampCtrlFromRTUSvrHandler _RTLampCtrlHandler;
        public static RTLampCtrlFromRTUSvrHandler GetHandler
        {
            get
            {
                if (_RTLampCtrlHandler == null)
                {
                    _RTLampCtrlHandler = new RTLampCtrlFromRTUSvrHandler();
                }
                return _RTLampCtrlHandler;
            }
        }

        private static int ctrlType = -1;

        //单灯实时控制
        public void HandlerRTLampCtrlPackage(LH_PackageData package)
        {
            AspectF.Define.Retry()
                .Do(() =>
                {
                    byte[] LampNo = new byte[2];
                    Buffer.BlockCopy(package.OnlyData, 0, LampNo, 0, 2);
                    byte opt = package.OnlyData[2];
                    byte isLock = package.OnlyData[3];

                    int no = BitConverter.ToInt16(LampNo, 0);

                    if (!RTUSvrShare.GetShare.CheckRepeatCmdWord(no, package))
                    {
                        return;
                    }

                    //先回复确认报文
                    RTUSvrShare.GetShare.SendToRTUSvrWithNoData(package.CtuAddr, LHCmdWordConst.ConfirmRTPtuChCtrl);

                    string ctuAddr = ByteHelper.bytesToCtuAddr(package.CtuAddr, true);
                    RealTimeCtrlLampHandler.GetHandler.RTCtrlByLampNo(ctuAddr, no, opt, isLock);
                    ctrlType = 1;
                });
        }

        //虚拟箱实时控制
        public void HandlerRTCtuCtrlPackage(LH_PackageData package)
        {
            AspectF.Define.Retry()
                .Do(() =>
                {
                    vLampInfo lampInfo = new vLampInfo();
                    byte chNum = package.OnlyData[0];
                    int curIndex = 1;
                    string ctuAddr = ByteHelper.bytesToCtuAddr(package.CtuAddr, true);
                    byte[] address = new byte[7];

                    for (int i = 0; i < chNum; i++)
                    {
                        lampInfo = new vLampInfo();
                        byte chNo = package.OnlyData[curIndex];
                        if (chNo > 0)
                        {
                            lampInfo = TerminalInitBusiness.GetInit.GetLampInfoByLampNo(ctuAddr, chNo);
                        }
                        if (!string.IsNullOrEmpty(lampInfo.PtuID))
                        {
                            if (TerminalShare.GetShare.checkDevIsLoginOrNot(lampInfo.PtuID.Trim()))
                            {
                                address = ByteHelper.HexStrToByteArrayWithDelimiter(lampInfo.PtuID.Trim(), " ");
                                byte optVal = package.OnlyData[curIndex+1];
                                if (optVal == package.OnlyData[curIndex+2])
                                {
                                    RealTimeCtrlLamp rtCtrl = new RealTimeCtrlLamp();
                                    rtCtrl.ChNo = chNo;
                                    rtCtrl.OptValue = optVal;
                                    rtCtrl.IsLock = package.OnlyData[curIndex+3];
                                    BrokerMessage sendMsg = new BrokerMessage(MessageType.realTimeCtrl, 0, address, rtCtrl);
                                    RealTimeCtrlLampHandler.GetHandler.RTCtrlByLampNo(sendMsg, lampInfo.LampNo);
                                }
                            }
                        }
                        curIndex += 4;
                    }
                    ctrlType = 2;
                });
        }

        public void SendRTCtrlBackSuccess(byte[] cutAddr,int lampNo, byte optRet)
        {
            if(ctrlType==1)
            {
                RTUSvrShare.GetShare.SendBackSuccess(cutAddr, LHCmdWordConst.RecvRTPtuChCtrl, lampNo, optRet);
            }
        }

        public void SendRTCtrlBackFail(byte[] cutAddr, byte[]macAddr, byte plcCmd)
        {
            if (ctrlType == 1)
            {
                RTUSvrShare.GetShare.SendBackFail(cutAddr, macAddr, plcCmd);
            }
        }
    }
}
