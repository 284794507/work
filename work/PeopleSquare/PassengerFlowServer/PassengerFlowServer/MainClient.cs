using PassengerFlowDal.Model;
using PassengerFlowServer.BusinessHandler;
using PassengerFlowServer.PackageHandler;
using PassengerFlowServer.Utility;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Sockets;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace PassengerFlowServer
{
    public class MainClient
    {
        private static  MainClient _Main;
        public static MainClient Instance
        {
            get
            {
                if (_Main == null)
                {
                    _Main = new MainClient();
                }
                return _Main;
            }
        }

        TcpClient mainClient;
        byte[] ReceivceBuffer;

        public void InitClient()
        {
            Share.Instance.InitConfig();
            ConnectTcp();
            MonitorTcp();
        }

        public void CloseClient()
        {
            AspectF.Define.Retry(Share.Instance.CatchExpection)
                .Do(() =>
                {
                    mainClient.GetStream().Close();
                    mainClient.Close();
                    mainClient.Client.Close();
                });
        }

        public void ConnectTcp()
        {
            AspectF.Define.Retry(Share.Instance.CatchExpection)
                .Do(() =>
                {
                    mainClient = new TcpClient();
                    ReceivceBuffer = new byte[mainClient.ReceiveBufferSize];
                    mainClient.Connect(Share.RemotePoint);
                    Share.Instance.WriteLog("接入服务器连接成功！");
                    MonitorTcp();
                    mainClient.GetStream().BeginRead(ReceivceBuffer, 0, ReceivceBuffer.Length, ReceivceData, ReceivceBuffer);
                });
        }

        private void MonitorTcp()
        {
            AspectF.Define.Retry(Share.Instance.CatchExpection)
                .Do(() =>
                {
                    ThreadPool.QueueUserWorkItem((a) =>
                    {
                        while(true)
                        {
                            Thread.Sleep(5000);
                            if(!mainClient.Connected)
                            {
                                ConnectTcp();
                            }
                        }
                    });
                });
        }

        public void ReceivceData(IAsyncResult ar)
        {
            AspectF.Define.Retry(Share.Instance.CatchExpection)
                .Do(() =>
                {
                    if (mainClient.Connected)
                    {
                        NetworkStream ns = mainClient.GetStream();

                        //已经关闭的流就退出
                        if (!mainClient.GetStream().CanRead)
                        {
                            return;
                        }
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
                        //DistriAliveTime = DateTime.Now;
                        AnalyData(receviceData);

                        mainClient.GetStream().BeginRead(ReceivceBuffer, 0, ReceivceBuffer.Length, ReceivceData, ReceivceBuffer);
                    }
                });
        }

        public void AnalyData(byte[] data)
        {
            AspectF.Define.Retry(Share.Instance.CatchExpection)
                .Do(() =>
                {
                    //包解析后的粘包
                    byte[] afterPackage = new byte[0];
                    PackageData package = new PackageData();
                    do
                    {
                        if (afterPackage == null || afterPackage.Length==0)
                        {
                            //无粘包，解析欣能协议
                            afterPackage=package.BuildPackgeFromBytes(data);
                        }
                        else
                        {
                            //拿出粘包，解析欣能协议
                            afterPackage = package.BuildPackgeFromBytes(afterPackage);
                        }

                        if (package.AnalySuccess)
                        {
                            HandlerSubPackage(package);
                        }
                        else
                        {
                            Share.Instance.WriteLog("Analy Package data fail!");
                        }
                    }
                    while (afterPackage != null && afterPackage.Length != 0);
                });
        }

        public void HandlerSubPackage(PackageData pkg)
        {
            AspectF.Define.Retry(Share.Instance.CatchExpection)
                .Do(() =>
                {
                    //包解析后的粘包
                    byte[] afterPackage = new byte[0];
                    SubPackage sPkg = new SubPackage();
                    do
                    {
                        if (afterPackage == null || afterPackage.Length == 0)
                        {
                            //无粘包，解析欣能协议
                            afterPackage = sPkg.BuildPackgeFromBytes(pkg.OnlyData);
                        }
                        else
                        {
                            //拿出粘包，解析欣能协议
                            afterPackage = sPkg.BuildPackgeFromBytes(afterPackage);
                        }

                        if (sPkg.AnalySuccess)
                        {
                            HandlerPackage(pkg, sPkg);
                        }
                        else
                        {
                            Share.Instance.WriteLog("Analy SubPackage data fail!");
                        }
                    }
                    while (afterPackage != null && afterPackage.Length != 0);                    
                });
        }

        public void HandlerPackage(PackageData pkg, SubPackage sPkg)
        {
            AspectF.Define.Retry(Share.Instance.CatchExpection)
                .Do(() =>
                {
                    switch(sPkg.CmdWord)
                    {
                        case PackageCmdWord.GetTime:
                            TabTimeSync info = new TabTimeSync();
                            info.DevMac = ByteHelper.ByteToHexStrWithDelimiter(pkg.Addr, " ", false);
                            info.ModuleNo = pkg.USart;
                            CheckTimeHandler.Instance.CheckTimePkgHandler(sPkg, info);
                            break;
                        case PackageCmdWord.GetData:
                            TabCurData curData = new TabCurData();
                            curData.DevMac = ByteHelper.ByteToHexStrWithDelimiter(pkg.Addr, " ", false);
                            curData.ModuleNo = pkg.USart;
                            GetDataHandler.Instance.GetCurDataPkgHandler(sPkg, curData);
                            break;
                    }
                });
        }
    }
}
