using PassengerFlowDal.Interface;
using PassengerFlowDal.Utility;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.ServiceModel;
using System.Text;

namespace PassengerFlowDal.WcfServer
{
    // 注意: 使用“重构”菜单上的“重命名”命令，可以同时更改代码和配置文件中的类名“DalSvr”。
    public class DalSvr : IDalSvr
    {
        private ICRUD crud = null;

        public void AddInfo(DalParameter para)
        {
            crud = ContainerHandler.Instance.GetService();
            switch(para.BusinessType)
            {
                case BusinessType.AddCheckTime:
                    crud.AddCheckTime(para);
                    break;
                case BusinessType.AddData:
                    crud.AddData(para);
                    break;
            }
        }

        public DalParameter GetInfo(DalParameter para)
        {
            crud = ContainerHandler.Instance.GetService();
            DalParameter result = new DalParameter();

            switch (para.BusinessType)
            {
                case BusinessType.GetAreaInfo:
                    result = crud.GetAreaInfo(para);
                    break;
            }

            return result;
        }
    }
}
