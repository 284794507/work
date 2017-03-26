using CommunicateCore.RTUSvr.RTUSvrBusiness;
using CommunicateCore.Utility;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace CommunicateCore.RTUSvr
{
    public class RTUSvr_TCPClient
    {
        private static RTUSvr_TCPClient _RTUSvr_TCPClient;
        public static RTUSvr_TCPClient GetClient
        {
            get
            {
                if(_RTUSvr_TCPClient==null)
                {
                    _RTUSvr_TCPClient = new RTUSvr_TCPClient();
                }
                return _RTUSvr_TCPClient;
            }
        }

        private TcpClient RTUSvrClient;
        byte[] ReceviceBuffer;

        public void InitClient()
        {
            AspectF.Define.Retry()
                .Log(RTUSvrShare.GetShare.WriterLog, "Initializing......", "Successed to initialize!")
                .Do(() =>
                {
                    RTUSvrClient = new TcpClient();
                    RTUSvrClient.ReceiveBufferSize = 50*1024+1024;
                    ReceviceBuffer = new byte[RTUSvrClient.ReceiveBufferSize];
                    RTUSvrShare.GetShare.SendToRTUSvr += SendData;
                    RTUSvrShare.GetShare.SendToRTUSvrWithNoData += SendNoDataPackage;
                    LHInitBusiness.GetLHInitBusiness.InitDBData();
                    RTUSvrShare.GetShare.InitConfig();
                    LHInitBusiness.GetLHInitBusiness.InitHandlerFunction();
                    ConnectRTUSvr();

                    UtilityHelper.tcp_keepalive keepalive;
                    keepalive.onoff = 1;
                    keepalive.keepalivetime = 5000;
                    keepalive.keepaliveinterval = 5000;

                    RTUSvrClient.Client.IOControl(IOControlCode.KeepAliveValues, UtilityHelper.GetHelper.StructToBytes(keepalive), null);

                    MonitorRTUSvrClient();
                });
        }

        public void ConnectRTUSvr()
        {
            AspectF.Define.Retry()
                .Log(RTUSvrShare.GetShare.WriterLog, "", "Connected to RTUSvr!")
                .Do(() =>
                {
                    IPAddress ip = RTUSvrShare.GetShare.RTUSvr_IP;
                    int port = RTUSvrShare.GetShare.RTUSvr_Port;

                    RTUSvrClient = new TcpClient();
                    RTUSvrClient.Connect(ip, port);                   
                    RTUSvrClient.GetStream().BeginRead(ReceviceBuffer, 0, ReceviceBuffer.Length, RTUSvrRecevice, ReceviceBuffer);
                    //连接成功后，发登录指令
                    string key = ByteHelper.byteToHexStr(LHCmdWordConst.GetLogin);
                    if (RTUSvrShare.GetShare.dictAllSendFunction.ContainsKey(key))
                    {
                        RTUSvrShare.GetShare.dictAllSendFunction[key]();
                    }
                });
        }


        /// <summary>
        /// 监测TCP连接
        /// </summary>
        private void MonitorRTUSvrClient()
        {
            AspectF.Define.Retry(() =>
            {
                ConnectRTUSvr();
            })
                .Do(() =>
                {
                    ThreadPool.QueueUserWorkItem(
                    delegate
                    {
                        while (true)
                        {
                            Thread.Sleep(2000);
                            if(!RTUSvrClient.Connected)
                            {
                                ConnectRTUSvr();
                            }
                            else
                            {
                                try
                                {
                                    //RTUSvrClient.GetStream().Write(new byte[1] { 0x68 }, 0, 1);
                                }
                                catch
                                {

                                }
                            }
                        }
                    }
                    );
                });
        }

        public void RTUSvrRecevice(IAsyncResult ar)
        {
            AspectF.Define.Retry()
                .Do(() =>
                {
                    if (RTUSvrClient.Connected)
                    {
                        NetworkStream NetStream = RTUSvrClient.GetStream();
                        int len = 0;
                        try
                        {
                            len = NetStream.EndRead(ar);
                        }
                        catch
                        {
                            len = 0;
                        }
                        if (len == 0)
                        {
                            Thread.Sleep(2000);
                            if (!RTUSvrClient.Connected)
                            {
                                ConnectRTUSvr();
                            }
                            return;
                        }
                        RTUSvrShare.GetShare.RefreshHeartBeatTime();

                        byte[] buffer = (byte[])ar.AsyncState;
                        byte[] receviceBytes = new byte[len];
                        Buffer.BlockCopy(buffer, 0, receviceBytes, 0, len);

                        AnalyzeProtocol_RTUSvr(receviceBytes);

                        RTUSvrClient.GetStream().BeginRead(ReceviceBuffer, 0, ReceviceBuffer.Length, RTUSvrRecevice, ReceviceBuffer);
                    }
                });
        }

        /// <summary>
        /// 解析接收到的数据包
        /// </summary>
        /// <param name="data"></param>
        private void AnalyzeProtocol_RTUSvr(byte[] data)
        {
            byte[] afterPackge = null;
            LH_PackageData package = null;
            AspectF.Define.Retry()
                .Do(() =>
                {
                    do
                    {
                        package = new LH_PackageData();
                        if (data.Length<4000)
                        {
                            if (afterPackge == null)
                            {
                                package.BuildPackageFromBytes(data);
                            }
                            else
                            {
                                package.BuildPackageFromBytes(afterPackge);
                            }
                            afterPackge = package.afterPackage;

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
                            msg += ByteHelper.ByteToHexStrWithDelimiter(package.ToBytes(), " ", false);
                            RTUSvrShare.GetShare.WriteLog_RTUSvr("AnalyzeProtocol_RTUSvr：" + msg);
                            if (package.AnalySuccess)
                            {
                                string key = ByteHelper.byteToHexStr(package.CmdWord);
                                if (RTUSvrShare.GetShare.dictAllHandlerFunction.ContainsKey(key))
                                {
                                    RTUSvrShare.GetShare.dictAllHandlerFunction[key](package);
                                }
                                else
                                {//如果当前指令还没有实现，则返回指令错误报文
                                    LH_PackageData ErrPkg = new LH_PackageData(package.CtuAddr, new byte[1] { 0x03 }, LHCmdWordConst.RecvErrorCmd);
                                    SendData(ErrPkg);
                                }
                            }
                        }
                        else//远程升级是非正常指令，无法解析
                        {
                            package.OnlyData = data;
                            UpgradeFromRTUSvrHandler.GetHandler.HandlerUpgradePackage(package);
                        }
                    }
                    while (afterPackge != null);
                });
        }

        private static Object sendLock = new object();
        private void SendData(LH_PackageData package)
        {
            byte[] data = package.ToBytes();
            AspectF.Define.Retry(() => { ConnectRTUSvr(); })
                .Log(RTUSvrShare.GetShare.WriterLog, "", "SendData：" + ByteHelper.ByteToHexStrWithDelimiter(data, " ", false))
                .Do(() =>
                {
                    lock (sendLock)
                    {
                        if (RTUSvrClient.Connected)
                        {
                            RTUSvrClient.GetStream().Write(data, 0, data.Length);
                        }
                    }
                });
        }

        private void SendNoDataPackage(byte[] cutAddr, byte[] cmdWord)
        {
            LH_PackageData package = new LH_PackageData(cutAddr, null, cmdWord);
            SendData(package);
        }
    }
}
