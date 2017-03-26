using PassengerFlowDal.Utility;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.ServiceModel;
using System.Text;

namespace PassengerFlowDal.WcfServer
{
    // 注意: 使用“重构”菜单上的“重命名”命令，可以同时更改代码和配置文件中的接口名“IDalSvr”。
    [ServiceContract]
    public interface IDalSvr
    {
        [OperationContract]
        void AddInfo(DalParameter para);

        [OperationContract]
        DalParameter GetInfo(DalParameter para);
    }
}
