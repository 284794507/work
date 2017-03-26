using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CTM_Route_Utility
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

        public string GetAddressFromCTMProtocol(byte[]data)
        {
            byte[] addr = new byte[6];
            Buffer.BlockCopy(data,8, addr, 0, 6);
            string devID = ByteHelper.byteToHexStr(addr);

            return devID;
        }
    }
}
