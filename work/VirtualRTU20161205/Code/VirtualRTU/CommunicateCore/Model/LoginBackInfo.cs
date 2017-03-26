using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CommunicateCore.Model
{
    [Serializable]
    public class LoginBackInfo
    {
        /// <summary>
        /// 1登录成功,2 不能登录,3须登录到以下地址
        /// </summary>
        public byte? LoginReuslt { get; set; }

        /// <summary>
        /// IP或域名
        /// </summary>
        public byte?[] IPOrDomain { get; set; }

        /// <summary>
        /// 端口
        /// </summary>
        public byte?[] Port { get; set; }

        public byte?[] APN { get; set; }

        public LoginBackInfo()
        {
            this.LoginReuslt = 1;
            this.IPOrDomain = new byte?[1];
            this.IPOrDomain[0] = 1;
            this.Port = new byte?[1];
            this.Port[0] = 1;
            this.APN = new byte?[1];
            this.APN[0] = 1;
        }
    }
}
