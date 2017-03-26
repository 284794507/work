using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CommunicateCore.RTUSvr.RTUSvrBusiness
{
    public class UpdateTimeHandler
    {//校时
        private static UpdateTimeHandler _UpdateTimeHandler;
        public static UpdateTimeHandler GetHandler
        {
            get
            {
                if (_UpdateTimeHandler == null)
                {
                    _UpdateTimeHandler = new UpdateTimeHandler();
                }
                return _UpdateTimeHandler;
            }
        }

        public void HandlerUpdateTimePackage(LH_PackageData package)
        {
            LH_PackageData back = new LH_PackageData(package.CtuAddr, null, LHCmdWordConst.RecvUpdateCTUTime);
            RTUSvrShare.GetShare.SendToRTUSvr(back);
        }
    }
}
