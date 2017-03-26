using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Communicate_Core.Model
{
    public class DevClient
    {
        public string CommAddr { get; set; }

        public byte[] bAddr { get; set; }

        public DateTime LoginTime { get; set; }

        public DateTime HeartBeatTime { get; set; }

        public bool IsOnLine { get; set; }

        public UpgradeStatus UpgradeInfo { get; set; }

        public Guid batchID { get; set; }//电参数批次ID，5分钟更换一次

        public DateTime batchTime { get; set; }//电参数批次ID，5分钟更换一次

        //private ushort sendNo;//下发的帐序号，1~65535循环

        public ushort SendNo { get; set; }//下发的帐序号，1~65535循环

        public byte[] GetSendNo
        {
            get
            {
                if(SendNo == 65535)
                {
                    SendNo = 0;
                }
                else
                {
                    SendNo++;
                }
                return BitConverter.GetBytes(SendNo);
            }
        }

        //public byte[] GetBSendNo()
        //{
        //    return BitConverter.GetBytes(SendNo++);
        //}

        public ushort ReceviceNo { get; set; }//收到设备发过来的帧序号，表示设备接收到的包数

        public ushort LastReceviceNo { get; set; }//上次设备发过来的帧序号，存储在数据中，每登录一次，取一次，防止设备重启
    }
}
