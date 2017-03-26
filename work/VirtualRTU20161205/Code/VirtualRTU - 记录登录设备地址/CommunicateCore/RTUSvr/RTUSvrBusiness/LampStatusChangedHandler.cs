using CommunicateCore.Utility;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CommunicateCore.RTUSvr.RTUSvrBusiness
{
    public class LampStatusChangedHandler
    {
        private static LampStatusChangedHandler _LampStatusChangedHandler;
        public static LampStatusChangedHandler GetHandler
        {
            get
            {
                if (_LampStatusChangedHandler == null)
                {
                    _LampStatusChangedHandler = new LampStatusChangedHandler();
                }
                return _LampStatusChangedHandler;
            }
        }

        public void LampStatusChangedSuccess(byte[] cutAddr,int lampNo)
        {
            AspectF.Define.Retry()
                .Do(() =>
                {
                    RTUSvrShare.GetShare.SendBackSuccess(cutAddr, LHCmdWordConst.GetLampStatusChangedCmd, lampNo, 3);//目前只有灯内故障上报
                });
        }
    }
}
