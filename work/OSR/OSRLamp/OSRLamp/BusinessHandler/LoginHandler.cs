using OSRLamp.PackageHandler;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OSRLamp.BusinessHandler
{
    public class LoginHandler
    {
        private static LoginHandler _LoginHandler;
        public static LoginHandler Instance
        {
            get
            {
                if (_LoginHandler == null)
                {
                    _LoginHandler = new LoginHandler();
                }
                return _LoginHandler;
            }
        }

        public void LoginPackageHandler(NJPackageData package)
        {
            AspectF.Define.Retry(Utility.CatchExpection)
                .Log(Utility.WriteLog, "", "Login Successed！")
                .Do(() =>
                {
                    byte[] data = new byte[3];
                    Buffer.BlockCopy(package.SerialNumber, 0, data, 0, 2);
                    data[2] = 0;//0：成功；    1：已登入；2：数据库中无该终端；3：消息有误

                    DevClient curClient = new DevClient();

                    string key = ByteHelper.ByteToHexStrWithDelimiter(package.TerminalID, "-");
                    if (Utility.ClientList.ContainsKey(key))
                    {
                        curClient = Utility.ClientList[key] as DevClient;
                        data[2] = 1;
                    }
                    curClient.StrID = key;
                    curClient.TerminalID = package.TerminalID;
                    curClient.LoginTime = DateTime.Now;

                    if (!Utility.ClientList.ContainsKey(key))
                    {
                        Utility.ClientList.Add(key, curClient);
                    }
                    
                    //NJPackageData sendPkg = new NJPackageData(CmdWord.Terminal_LoginResponse, new byte[] { 0x00, 0x00 }, package.TerminalID, Utility.GetSerialNo(), new byte[] { 0x01, 0x00, 0x01, 0x00 }, new byte[0], data);
                    NJPackageData sendPkg = new NJPackageData(CmdWord.Terminal_LoginResponse, package.TerminalID, data);

                    Utility.SendDataToTerminal(sendPkg);
                });
        }
    }
}
