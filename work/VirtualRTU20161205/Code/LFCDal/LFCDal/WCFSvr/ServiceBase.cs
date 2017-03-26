using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.ServiceModel;

namespace LFCDal.WCFSvr
{
    [ServiceBehavior(InstanceContextMode = InstanceContextMode.PerSession, ConcurrencyMode = ConcurrencyMode.Multiple)]
    public class IServiceBase
    {
        public IServiceBase()
        {
            //服务启动时间
            //Console.WriteLine(name + " start up time ...." + System.DateTime.Now.ToString());
        }
    }
}
