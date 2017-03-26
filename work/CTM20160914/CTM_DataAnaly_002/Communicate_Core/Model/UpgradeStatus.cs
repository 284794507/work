using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Communicate_Core.Model
{
    public class UpgradeStatus
    {
        /// <summary>
        /// 升级设备ID
        /// </summary>
        public string TerminalID { get; set; }

        /// <summary>
        /// 当前升级包号
        /// </summary>
        public int CurBagNo { get; set; }

        /// <summary>
        /// 总包数
        /// </summary>
        public int AllBagNum { get; set; }

        /// <summary>
        /// 上次发包时间
        /// </summary>
        public DateTime SendTime { get; set; }

        /// <summary>
        /// 当前包发送次数
        /// </summary>
        public int SendNum { get; set; }

        /// <summary>
        /// 是否新升级
        /// </summary>
        public bool IsNewUpgrade { get; set; }

        /// <summary>
        /// 是否升级成功
        /// </summary>
        public bool IsUpgradeSuccess { get; set; }

        public UpgradeStatus()
        {
            this.CurBagNo = 0;
            this.AllBagNum = 0;
            this.SendTime = DateTime.MinValue;
            this.SendNum = 0;
            this.IsNewUpgrade = true;
            this.IsUpgradeSuccess = true;
        }
    }
}
