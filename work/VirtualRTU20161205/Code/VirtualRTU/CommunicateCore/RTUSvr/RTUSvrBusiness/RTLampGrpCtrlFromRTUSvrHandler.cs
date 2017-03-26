using CommunicateCore.Utility;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CommunicateCore.RTUSvr.RTUSvrBusiness
{
    //单灯组控制
    public class RTLampGrpCtrlFromRTUSvrHandler
    {
        private static RTLampGrpCtrlFromRTUSvrHandler _RTLampGrpCtrlHandler;
        public static RTLampGrpCtrlFromRTUSvrHandler GetHandler
        {
            get
            {
                if (_RTLampGrpCtrlHandler == null)
                {
                    _RTLampGrpCtrlHandler = new RTLampGrpCtrlFromRTUSvrHandler();
                }
                return _RTLampGrpCtrlHandler;
            }
        }

        public void HandlerRTLampGrpCtrlPackage(LH_PackageData package)
        {
            AspectF.Define.Retry()
                .Do(() =>
                {
                    int grpNo = package.OnlyData[0];
                    byte opt = package.OnlyData[1];
                    byte isLock = package.OnlyData[2];

                    if (!RTUSvrShare.GetShare.CheckRepeatCmdWord(grpNo, package))
                    {
                        return;
                    }

                    SendToLFI(package.CtuAddr, grpNo, 0, opt, isLock);
                });
        }

        public void SendToLFI(byte[] ctuAddr,int GrpNo,int lampNo, byte opt, byte isLock)
        {
            AspectF.Define.Retry()
                .Do(() =>
                {
                    //byte[] data = new byte[6 + 2];
                    //data[0] = (byte)CommType.RTLampCtrl;
                    //Buffer.BlockCopy(ctuAddr, 0, data, 1, 2);
                    //data[3] = (byte)GrpNo;
                    //byte[] no = BitConverter.GetBytes((short)lampNo);
                    //data[4] = no[0];
                    //data[5] = no[1];
                    //data[6] = opt;
                    //data[7] = isLock;
                    //RTUSvrShare.GetShare.SendToLFI(data);
                });
        }

        public void SendRTCtrlBackSuccess(byte[] cutAddr, int lampNo, byte optRet)
        {
            RTUSvrShare.GetShare.SendBackSuccess(cutAddr, LHCmdWordConst.RecvRTPtuChCtrlByGroup, lampNo, optRet);
        }

        public void SendRTCtrlBackFail(byte[] cutAddr, byte[] macAddr, byte plcCmd)
        {
            RTUSvrShare.GetShare.SendBackFail(cutAddr, macAddr, plcCmd);
        }
    }
}
