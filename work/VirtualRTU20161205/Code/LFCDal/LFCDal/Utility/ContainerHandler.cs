using LFCDal.Interface;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace LFCDal.Utility
{
    public class ContainerHandler
    {
        private static IDBBLL _DbDll;

        private static ContainerHandler instance;

        public static ContainerHandler GetInstance
        {
            get
            {
                if (instance == null)
                {
                    instance = new ContainerHandler();
                    string dbType = ConfigurationManager.AppSettings["DBType"];
                    _DbDll = Assembly.Load("LFCDal").CreateInstance("LFCDal." + dbType) as IDBBLL;

                }
                return instance;
            }
        }

        public IDBBLL GetService()
        {
            return _DbDll;
        }

        public static IDBBLL GetDll
        {
            get
            {
                if (_DbDll == null)
                {
                    string dbType = ConfigurationManager.AppSettings["DBType"]; // ConfigurationSettings.AppSettings["DBType"];
                    _DbDll =  Assembly.Load("LFCDal").CreateInstance("LFCDal." + dbType) as IDBBLL;
                }
                return _DbDll;
            }
        }
    }
}
