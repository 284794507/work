using PassengerFlowDal.Interface;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace PassengerFlowDal.Utility
{
    public class ContainerHandler
    {
        private static ICRUD _Dal;

        private static ContainerHandler _ContainerHandler;

        public static ContainerHandler Instance
        {
            get
            {
                if(_ContainerHandler ==null)
                {
                    _ContainerHandler = new ContainerHandler();
                    string dbType = ConfigurationManager.AppSettings["DBType"];
                    _Dal = Assembly.Load("PassengerFlowDal").CreateInstance("PassengerFlowDal." + dbType) as ICRUD;
                }

                return _ContainerHandler;
            }
        }

        public ICRUD GetService()
        {
            return _Dal;
        }
    }
}
