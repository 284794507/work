using OSRLamp.PackageHandler;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OSRLamp.BusinessHandler
{
    public class ControlType
    {
        private static ControlType _ControlType;
        public static ControlType Instance
        {
            get
            {
                if (_ControlType == null)
                {
                    _ControlType = new ControlType();
                }
                return _ControlType;
            }
        }

        /// <summary>
        /// 设置控制模式
        /// </summary>
        /// <param name="terminalID"></param>
        /// <param name="mode"></param>
        public void SetCtrlType(byte[] terminalID,byte mode)
        {
            AspectF.Define.Retry(Utility.CatchExpection)
                .Do(() =>
                {
                    byte[] data = new byte[1];
                    data[0] = mode;//0-自动时间控制,1-手动控制，2~255-保留
                    NJPackageData sendPkg = new NJPackageData(CmdWord.Set_CtrlMode, terminalID, data);

                    Utility.SendDataToTerminal(sendPkg);
                });
        }
    }
}
