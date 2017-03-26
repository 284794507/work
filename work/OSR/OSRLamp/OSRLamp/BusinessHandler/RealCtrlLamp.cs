using OSRLamp.PackageHandler;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OSRLamp.BusinessHandler
{
    public class RealCtrlLamp
    {

        private static RealCtrlLamp _RealCtrlLamp;
        public static RealCtrlLamp Instance
        {
            get
            {
                if (_RealCtrlLamp == null)
                {
                    _RealCtrlLamp = new RealCtrlLamp();
                }
                return _RealCtrlLamp;
            }
        }

        public void SendRealCtrlLamp(byte[] terminalID,byte ctrlVal,byte lampNo=1)
        {
            byte[] data = new byte[3];
            data[0] = 1;//目前不做批量控制
            data[1] = lampNo;
            data[2] = ctrlVal;

            //NJPackageData sendPkg = new NJPackageData(CmdWord.Lamp_RealCtrl, new byte[] { 0x00, 0x00 }, terminalID, Utility.GetSerialNo(), new byte[] { 0x01, 0x00, 0x01, 0x00 }, new byte[0], data);
            NJPackageData sendPkg = new NJPackageData(CmdWord.Lamp_RealCtrl, terminalID, data);

            Utility.SendDataToTerminal(sendPkg);
        }        
    }
}
