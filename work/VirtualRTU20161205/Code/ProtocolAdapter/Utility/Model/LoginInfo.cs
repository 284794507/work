using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Utility.Model
{
    [Serializable]
    public class LoginInfo
    {
        /// <summary>
        /// 信号强度
        /// </summary>
        public byte Signal { get; set; }

        /// <summary>
        /// GSM模块IMSI识别码,ASCII ,无法获取时为0XFF
        /// </summary>
        public byte[] GSM_IMSI { get; set; }

        /// <summary>
        /// SIM卡IMEI识别码,ASCII ,无法获取时为0XFF
        /// </summary>
        public byte[] SIM_IMEI { get; set; }

        /// <summary>
        /// =1为南纬 =2为北纬
        /// </summary>
        public byte isNOrS { get; set; }

        /// <summary>
        /// 纬度
        /// </summary>
        public byte[] Latitude { get; set; }

        /// <summary>
        /// = 1为东经 =2为西经
        /// </summary>
        public byte isWOrE { get; set; }

        /// <summary>
        /// 经度
        /// </summary>
        public byte[] Longitude { get; set; }

        public byte[] SoftVersion { get; set; }

        public byte[] HardVersion { get; set; }

        public byte[] ProtocolVersion { get; set; }

        public byte[] NetMacAddr { get; set; }

        public LoginInfo()
        {
            this.GSM_IMSI = new byte[15];
            this.SIM_IMEI = new byte[15];
            this.Latitude = new byte[4];
            this.Longitude = new byte[4];
            this.SoftVersion = new byte[2];
            this.HardVersion = new byte[2];
            this.ProtocolVersion = new byte[2];
            this.NetMacAddr = new byte[6];
        }
        public void BuildLoginInfo(byte[] data)
        {
            AspectF.Define.Retry().Do(() =>
            {
                int curIndex = 0;
                this.Signal = data[curIndex++];
                Buffer.BlockCopy(data, curIndex, this.GSM_IMSI, 0, 15);
                curIndex += 15;
                Buffer.BlockCopy(data, curIndex, this.SIM_IMEI, 0, 15);
                curIndex += 15;
                this.isNOrS = data[curIndex++];
                Buffer.BlockCopy(data, curIndex, this.Latitude, 0, 4);
                curIndex += 4;
                this.isNOrS = data[curIndex++];
                Buffer.BlockCopy(data, curIndex, this.Longitude, 0, 4);
                curIndex += 4;
                Buffer.BlockCopy(data, curIndex, this.SoftVersion, 0, 2);
                curIndex += 2;
                Buffer.BlockCopy(data, curIndex, this.HardVersion, 0, 2);
                curIndex += 2;
                Buffer.BlockCopy(data, curIndex, this.ProtocolVersion, 0, 2);
                curIndex += 2;
                Buffer.BlockCopy(data, curIndex, this.NetMacAddr, 0, 6);
                curIndex += 6;

            });
        }
    }
}
