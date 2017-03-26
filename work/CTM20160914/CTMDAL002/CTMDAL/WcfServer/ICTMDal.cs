using CTMDAL.Utility;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.ServiceModel;
using System.Text;

namespace CTMDAL.WcfServer
{
    // 注意: 使用“重构”菜单上的“重命名”命令，可以同时更改代码和配置文件中的接口名“ICTMDal”。
    [ServiceContract]
    public interface ICTMDal
    {

        [OperationContract]
        CTMDalParameter GetInfo(CTMDalParameter para);

        [OperationContract]
        void DelInfo(CTMDalParameter para);

        [OperationContract]
        void AddInfo(CTMDalParameter para);

        [OperationContract]
        void UpdateInfo(CTMDalParameter para);
    }

}
