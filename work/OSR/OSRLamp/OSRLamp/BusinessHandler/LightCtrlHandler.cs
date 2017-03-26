using OSRLamp.PackageHandler;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OSRLamp.BusinessHandler
{
    public class LightCtrlHandler
    {
        private static LightCtrlHandler _LightCtrlHandler;
        public static LightCtrlHandler Instance
        {
            get
            {
                if (_LightCtrlHandler == null)
                {
                    _LightCtrlHandler = new LightCtrlHandler();
                }
                return _LightCtrlHandler;
            }
        }

        public void SetLightCtrlHandler(byte[]terminalID,byte status,byte[] OnVal,byte[] OffVal)
        {
            AspectF.Define.Retry(Utility.CatchExpection)
                .Do(() =>
                {
                    byte[] data = new byte[5];
                    data[0] = status;
                    Buffer.BlockCopy(OnVal, 0, data, 1, 2);
                    Buffer.BlockCopy(OffVal, 0, data, 3, 2);
                    NJPackageData sendPkg = new NJPackageData(CmdWord.Set_LightCtrl, terminalID, data);

                    Utility.SendDataToTerminal(sendPkg);
                });
        }

        public void QueryLightCtrlHandler(byte[] terminalID)
        {
            AspectF.Define.Retry(Utility.CatchExpection)
                .Do(() =>
                {
                    byte[] data = new byte[0];
                    NJPackageData sendPkg = new NJPackageData(CmdWord.Query_LightCtrl, terminalID, data);

                    Utility.SendDataToTerminal(sendPkg);
                });
        }

        public void QueryLightCrtlBackPackageHandler(NJPackageData pkg)
        {
            AspectF.Define.Retry(Utility.CatchExpection)
                .Log(Utility.WriteLog, "", "Recevice QueryLightCrtlBackPack！")
                .Do(() =>
                {
                    byte status = pkg.OnlyData[2];
                    ushort onVal = BitConverter.ToUInt16(pkg.OnlyData, 3);
                    ushort offVal = BitConverter.ToUInt16(pkg.OnlyData, 5);
                    Utility.WriteLog("查询光感控制回复：" + status　+　" : " + onVal + " : " + offVal);
                });
        }
    }
}
