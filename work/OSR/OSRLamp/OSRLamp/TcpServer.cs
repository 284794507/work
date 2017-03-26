using OSRLamp.BusinessHandler;
using OSRLamp.PackageHandler;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Sockets;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace OSRLamp
{
    public class TcpServer
    {
        private static TcpServer _TcpServer;
        
        public static TcpServer Instance
        {
            get
            {
                if(_TcpServer==null)
                {
                    _TcpServer = new TcpServer();
                }
                return _TcpServer;
            }
        }

        TcpClient curClient;
        byte[] curBuffer;

        public void InitClient()
        {
            Utility.SendDataToTerminal += SendData;
            Utility.InitConfig();
            Utility.InitHandlerFunction();

            TcpConnect();
            MonitorDevClient();
        }

        public void TcpConnect()
        {
            AspectF.Define.Retry(Utility.CatchExpection)
                .Do(() =>
                {
                    curClient = new TcpClient();
                    curBuffer = new byte[curClient.ReceiveBufferSize];
                    curClient.Connect(Utility.Adapter_IP, Utility.Adapter_Port);
                    Utility.WriteLog("适配器连接成功");
                    curClient.GetStream().BeginRead(curBuffer, 0, curBuffer.Length, ReceviceTcpData, curBuffer);
                });
        }

        private void MonitorDevClient()
        {
            AspectF.Define.Retry(Utility.CatchExpection, () => { MonitorDevClient(); })
                .Log(Utility.WriteLog, "MonitorDevClient", "")
                .Do(() =>
                {
                    ThreadPool.QueueUserWorkItem(
                    delegate
                    {
                        while (true)
                        {
                            Thread.Sleep(5000);
                            if (!curClient.Connected)
                            {
                                TcpConnect();
                            }
                        }
                    }
                    );
                });
        }

        public void ReceviceTcpData(IAsyncResult ar)
        {
            AspectF.Define.Retry(Utility.CatchExpection)
                .Do(() =>
                {
                    if(curClient.Connected)
                    {
                        NetworkStream ns = curClient.GetStream();
                        int len = 0;
                        try
                        {
                            len = ns.EndRead(ar);
                        }
                        catch
                        {
                            len = 0;
                        }
                        if (len == 0) return;

                        byte[] buffer = (byte[])ar.AsyncState;
                        byte[] receviceData = new byte[len];
                        Buffer.BlockCopy(buffer, 0, receviceData, 0, len);

                        AnalyProtocol(receviceData);

                        curClient.GetStream().BeginRead(curBuffer, 0, curBuffer.Length, ReceviceTcpData, curBuffer);
                    }
                });
        }

        public void AnalyProtocol(byte[] data)
        {
            AspectF.Define.Retry(Utility.CatchExpection)
                .Do(() =>
                {
                    byte[] afterData = null;
                    NJPackageData package = new NJPackageData();

                    do
                    {
                        if (afterData == null || afterData.Length==0)
                        {
                            package.BuildPackageFromBytes(data);
                        }
                        else
                        {
                            package.BuildPackageFromBytes(afterData);
                        }

                        string msg = "";
                        if (package.AnalySuccess)
                        {
                            msg = "接收正确的报文" + "\r\n";
                            msg += "命令字：" + BitConverter.ToString(package.CmdWord) + "\r\n";
                        }
                        else
                        {
                            msg = "接收错误的报文" + "\r\n";
                        }
                        if (afterData == null || afterData.Length == 0)
                        {
                            msg += ByteHelper.ByteToHexStrWithDelimiter(package.ToBytes());
                        }
                        else
                        {
                            msg += ByteHelper.ByteToHexStrWithDelimiter(afterData);
                        }
                        Utility.WriteLog(msg);
                        if (package.AnalySuccess)
                        {
                            byte[] newCmd = new byte[2];
                            newCmd[0] = package.CmdWord[1];
                            newCmd[1] = package.CmdWord[0];
                            string key = ByteHelper.ByteToHexStrWithDelimiter(newCmd);
                            string addr = ByteHelper.ByteToHexStrWithDelimiter(package.TerminalID, "-");
                            if (Utility.dictAllHandlerFun.ContainsKey(key))
                            {
                                string keyLogin = ByteHelper.ByteToHexStrWithDelimiter(CmdWord.Terminal_Login);//登录
                                if (key == keyLogin || Utility.ClientList.ContainsKey(addr))
                                {
                                    Utility.dictAllHandlerFun[key](package);
                                }
                            }
                            else //if (key != keyLogin && Utility.ClientList.ContainsKey(addr))
                            {
                                Utility.MasterResponse(package);
                            }
                            if(package.Tp !=null && package.Tp.Length==6)
                            {
                                DateTime dt = ByteHelper.Bytes6ToDateTime(package.Tp);
                                TimeSpan ts = DateTime.Now - dt;
                                if(Math.Abs(ts.Minutes)>=1)
                                {
                                    DateTimeHandler.Instance.SetDateTime(package.TerminalID);
                                }
                            }
                        }
                        afterData = package.AfterData;
                    }
                    while (afterData != null &&  afterData.Length > 0);
                });
        }

        private static Object sendLock = new object();
        public void SendData(NJPackageData package)
        {
            AspectF.Define.Retry(Utility.CatchExpection)
                .Do(() =>
                {
                    lock (sendLock)
                    {
                        if (curClient.Connected)
                        {
                            byte[] data = new byte[0];
                            data = package.ToBytes();
                            curClient.GetStream().Write(data, 0, data.Length);
                            Utility.WriteLog("发送成功！" + "\r\n" + ByteHelper.ByteToHexStrWithDelimiter(data));
                            Thread.Sleep(1000);
                        }
                    }
                });
        }

        public void TcpClose()
        {
            AspectF.Define.Retry(Utility.CatchExpection)
                .Do(() =>
                {
                    curClient.GetStream().Close();
                    curClient.Close();
                    curClient.Client.Close();
                });
        }
    }
}
