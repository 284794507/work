using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CommunicateCore.Model
{
    [Serializable]
    public class UpgradeInfo
    {
        public byte UpgradeStatus { get; set; }

        #region 升级请求
        public byte[] FileName { get; set; }

        public byte[] FileLength { get; set; }

        /// <summary>
        /// 升级文件的总校验
        /// </summary>
        public byte[] FileCRC { get; set; }
        #endregion

        #region 升级请中
        /// <summary>
        /// 升级文件内容
        /// </summary>
        public byte[] FileData { get; set; }
        #endregion    
        
        /// <summary>
        /// 总包数
        /// </summary>
        public byte TotalNum { get; set; }    

        /// <summary>
        /// 当前
        /// </summary>
        public byte SendNo { get; set; }
    }
}
