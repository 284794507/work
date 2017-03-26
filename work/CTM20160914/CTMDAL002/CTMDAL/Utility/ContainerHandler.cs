using CTMDAL.Interface;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace CTMDAL.Utility
{
    public class ContainerHandler
    {
        private static ICRUD _DbDal;

        private static ContainerHandler instance;

        public static ContainerHandler Instance
        {
            get
            {
                if (instance == null)
                {
                    instance = new ContainerHandler();
                    string dbType = ConfigurationManager.AppSettings["DBType"];
                    _DbDal = Assembly.Load("CTMDAL").CreateInstance("CTMDAL." + dbType) as ICRUD;

                }
                return instance;
            }
        }

        public ICRUD GetService()
        {
            return _DbDal;
        }
    }
}
