using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Utility
{
    public class PackageHelper
    {
        private static PackageHelper _PackageHelper;
        public static PackageHelper GetPackageHelper
        {
            get
            {
                if(_PackageHelper==null)
                {
                    _PackageHelper = new PackageHelper();
                }
                return _PackageHelper;
            }
        }

        public string GetAddressFromXNProtocol(byte[]data)
        {
            byte[] addr = new byte[8];
            Buffer.BlockCopy(data, 6, addr, 0, 8);
            string devID = ByteHelper.byteToHexStr(addr);

            return devID;
        }
    }
}
