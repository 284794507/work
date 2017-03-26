using CommunicateCore.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TerminalConfigTool.model
{
    public delegate void NetEvent(object sender, NetEventArgs e);

    public class NetEventArgs:EventArgs
    {
        #region 字段

        /// <summary> 
        /// 客户端与服务器之间的会话 
        /// </summary> 
        private TerminalClient _ctuclient;

        #endregion

        #region 构造函数
        /// <summary> 
        /// 构造函数 
        /// </summary> 
        /// <param name="client">客户端会话</param> 
        public NetEventArgs(TerminalClient client)
        {
            if (null == client)
            {
                throw (new ArgumentNullException());
            }

            _ctuclient = client;
        }
        #endregion

        #region 属性

        /// <summary> 
        /// 获得激发该事件的会话对象 
        /// </summary> 
        public TerminalClient Client
        {
            get
            {
                return _ctuclient;
            }

        }

        #endregion
    }
}
