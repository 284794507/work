using PassengerFlowDal.Utility;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PassengerFlowDal.Interface
{
    public interface ICRUD
    {
        void AddCheckTime(DalParameter data);

        void AddData(DalParameter data);

        DalParameter GetAreaInfo(DalParameter para);
    }
}
