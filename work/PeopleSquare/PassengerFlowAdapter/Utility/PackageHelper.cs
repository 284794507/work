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
        public static PackageHelper Instance
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

        public string GetAddrFromPackage(byte[]data,int startIndex=3,int len=8)
        {
            byte[] addr = new byte[8];
            Buffer.BlockCopy(data, startIndex, addr, 0, len);
            string macAddr = ByteHelper.byteToHexStr(addr);

            return macAddr;
        }
    }
}
