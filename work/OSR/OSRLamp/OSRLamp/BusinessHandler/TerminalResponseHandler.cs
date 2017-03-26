using OSRLamp.PackageHandler;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OSRLamp.BusinessHandler
{
    public class TerminalResponseHandler
    {
        private static TerminalResponseHandler _TerminalResponseHandler;
        public static TerminalResponseHandler Instance
        {
            get
            {
                if (_TerminalResponseHandler == null)
                {
                    _TerminalResponseHandler = new TerminalResponseHandler();
                }
                return _TerminalResponseHandler;
            }
        }

        /// <summary>
        /// 通用回复
        /// </summary>
        /// <param name="package"></param>
        public void TerminalResponsePackageHandler(NJPackageData package)
        {
            AspectF.Define.Retry(Utility.CatchExpection)
                .Log(Utility.WriteLog, "", "Recevice TerminalResponse！")
                .Do(() =>
                {
                    byte[] ReceCmdWord = new byte[2];
                    Buffer.BlockCopy(package.OnlyData, 2, ReceCmdWord, 0, 2);
                    Array.Reverse(ReceCmdWord);
                    if(ByteHelper.ByteArryEquals(ReceCmdWord,CmdWord.Lamp_RealCtrl))
                    {
                        Utility.WriteLog("接收单灯实现控制操作回复：" + GetStatus(package.OnlyData[4]));
                    }
                    else if (ByteHelper.ByteArryEquals(ReceCmdWord, CmdWord.Set_LightCtrl))
                    {
                        Utility.WriteLog("接收设置光感回复：" + GetStatus(package.OnlyData[4]));
                    }
                    else if (ByteHelper.ByteArryEquals(ReceCmdWord, CmdWord.Set_CtrlMode))
                    {
                        Utility.WriteLog("接收设置控制模式回复：" + GetStatus(package.OnlyData[4]));
                    }
                    else if (ByteHelper.ByteArryEquals(ReceCmdWord, CmdWord.Set_Datetime))
                    {
                        Utility.WriteLog("接收设置日历时钟回复：" + GetStatus(package.OnlyData[4]));
                    }
                });
        }

        private string GetStatus(byte status)
        {
            string result = "";

            switch(status)
            {
                case 0:
                    result = "成功";
                    break;
                case 1:
                    result = "失败";
                    break;
                case 2:
                    result = "消息有误";
                    break;
                case 3:
                    result = "不支持";
                    break;
            }

            return result;
        }
    }
}
