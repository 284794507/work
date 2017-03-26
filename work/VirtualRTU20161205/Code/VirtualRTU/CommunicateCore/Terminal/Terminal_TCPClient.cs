using CommunicateCore.Model;
using CommunicateCore.Terminal.TerminalBusiness;
using CommunicateCore.Utility;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace CommunicateCore.Terminal
{
    public class Terminal_TCPClient
    {
        private static Terminal_TCPClient _Terminal_TCPClient;
        public static Terminal_TCPClient GetClient
        {
            get
            {
                if(_Terminal_TCPClient==null)
                {
                    _Terminal_TCPClient = new Terminal_TCPClient();
                }
                return _Terminal_TCPClient;
            }
        }

        private TcpClient TerminalClient;
        byte[] ReceviceBuffer;

        struct tcp_keepalive
        {
            public uint onoff;
            public uint keepalivetime;
            public uint keepaliveinterval;
        };

        /// &lt;summary&gt;  
        /// 结构体转byte数组  
        /// &lt;/summary&gt;  
        /// &lt;param name="structObj"&gt;要转换的结构体   &lt;/param&gt;  
        /// &lt;returns&gt;转换后的byte数组&lt;/returns&gt;  
        public static byte[] StructToBytes(object structObj)
        {
            //得到结构体的大小  
            int size = Marshal.SizeOf(structObj);
            //创建byte数组  
            byte[] bytes = new byte[size];
            //分配结构体大小的内存空间  
            IntPtr structPtr = Marshal.AllocHGlobal(size);
            //将结构体拷到分配好的内存空间  
            Marshal.StructureToPtr(structObj, structPtr, false);
            //从内存空间拷到byte数组  
            Marshal.Copy(structPtr, bytes, 0, size);
            //释放内存空间  
            Marshal.FreeHGlobal(structPtr);
            //返回byte数组  
            return bytes;
        }

        public void InitClient()
        {
            TerminalClient = new TcpClient();
            TerminalClient.ReceiveBufferSize = 20000;
            ReceviceBuffer = new byte[TerminalClient.ReceiveBufferSize];
            TerminalShare.GetShare.SendToTerminal += SendData;
            TerminalInitBusiness.GetInit.InitData();

            TerminalShare.GetShare.InitConfig();
            TerminalInitBusiness.GetInit.InitHandlerFunction();
            ConnectTerminal();


            tcp_keepalive keepalive;
            keepalive.onoff = 1;
            keepalive.keepalivetime = 1000;
            keepalive.keepaliveinterval = 1000;
            
            TerminalClient.Client.IOControl(IOControlCode.KeepAliveValues, StructToBytes(keepalive), null);


            MonitorTerminalClient();

            MonitorSendPackage();
        }

        public void ConnectTerminal()
        {
            AspectF.Define.Retry()
                .Do(() =>
                {
                    IPAddress ip = TerminalShare.GetShare.LFI_Route_IP;
                    int port = TerminalShare.GetShare.LFI_Route_Port;
                    TerminalClient = new TcpClient();
                    TerminalClient.Connect(ip, port);
                    TerminalShare.GetShare.WriteLog_Terminal("Connet：" + "接入服务连接成功！");
                    TerminalClient.GetStream().BeginRead(ReceviceBuffer, 0, ReceviceBuffer.Length, TerminalClientRecevice, ReceviceBuffer);
                });
        }

        private void MonitorTerminalClient()
        {
            AspectF.Define.Retry(()=>
            {
                ConnectTerminal();
            })
                .Do(() =>
                {
                    ThreadPool.QueueUserWorkItem(
                    delegate
                    {
                        while (true)
                        {
                            Thread.Sleep(2000);
                            if (!TerminalClient.Connected)
                            {
                                ConnectTerminal();
                            }
                            else
                            {
                                try
                                {
                                    TerminalClient.GetStream().Write(new byte[2] { (byte)'!', (byte)'$' }, 0, 2);
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

        private void MonitorSendPackage()
        {
            AspectF.Define.Retry()
                .Do(() =>
                {
                    ThreadPool.QueueUserWorkItem(
                    delegate
                    {
                        while (true)
                        {
                            Thread.Sleep(5000);
                            CheckNoRecevieCmdWord();
                        }
                    }
                    );
                });
        }

        private void CheckNoRecevieCmdWord()
        {
            AspectF.Define.Retry()
                .Do(() =>
                {
                    Array arr = TerminalShare.GetShare.dictSendCmdWord.Keys.ToArray();
                    foreach (string key in arr)
                    {
                        CmdWordTime_Terminal lfi = TerminalShare.GetShare.dictSendCmdWord[key];
                        if (lfi.Num < 1)
                        {
                            SendData(lfi.BMsg);
                            lfi.SendTime = DateTime.Now;
                            lfi.Num++;
                        }
                        else
                        {
                            if (lfi.BMsg.MsgType == MessageType.realTimeCtrl)
                            {
                                RealTimeCtrlLampHandler.GetHandler.RTCtrlNoRecevice(lfi.BMsg);
                                TerminalShare.GetShare.dictSendCmdWord.Remove(key);
                            }
                            //else if (lfi.Package.Cmd == LHLFICmdWordConst.LampSelRunCfg)
                            //{
                            //    LampSelfRunCfgToLFIHandler.GetHandler.LampSelRunCfgNoRecevice(lfi.Package);
                            //    TerminalShare.GetShare.dictSendCmdWord.Remove(key);
                            //}
                            //else if (lfi.Package.Cmd == LHLFICmdWordConst.QueryLampSelRunCfg)
                            //{
                            //    QueryLampSelfRunCfgToLFIHandler.GetHandler.QueryLampSelRunCfgNoRecevice(lfi.Package);
                            //    TerminalShare.GetShare.dictSendCmdWord.Remove(key);
                            //}
                            //else if (lfi.Package.Cmd == LHLFICmdWordConst.LampGroupCfg)
                            //{
                            //    LampGroupCfgToLFIHandler.GetHandler.LampGroupCfgToLFINoRecevice(lfi.Package);
                            //    TerminalShare.GetShare.dictSendCmdWord.Remove(key);
                            //}
                            //else if (lfi.Package.Cmd == LHLFICmdWordConst.QueryLampGroupCfg)
                            //{
                            //    QueryLampGroupCfgToLFIHandler.GetHandler.QueryLampGroupCfgNoRecevice(lfi.Package);
                            //    TerminalShare.GetShare.dictSendCmdWord.Remove(key);
                            //}
                            else if (lfi.BMsg.MsgType == MessageType.queryElecData)
                            {
                                ElecDataHandler.GetHandler.QueryElecDataNoRecevice(lfi.BMsg);
                                TerminalShare.GetShare.dictSendCmdWord.Remove(key);
                            }
                        }
                    }
                });
        }

        public void TerminalClientRecevice(IAsyncResult ar)
        {
            AspectF.Define.Retry()
                .Do(() =>
                {
                    if (TerminalClient.Connected)
                    {
                        NetworkStream NetStream = TerminalClient.GetStream();
                        int len = 0;
                        try
                        {
                            len = NetStream.EndRead(ar);
                        }
                        catch
                        {
                            len = 0;
                        }
                        if (len == 0) return;

                        byte[] buffer = (byte[])ar.AsyncState;
                        byte[] receviceBytes = new byte[len];
                        Buffer.BlockCopy(buffer, 0, receviceBytes, 0, len);

                        AnalyzeProtocol_LFI(receviceBytes);

                        TerminalClient.GetStream().BeginRead(ReceviceBuffer, 0, ReceviceBuffer.Length, TerminalClientRecevice, ReceviceBuffer);
                    }
                });
        }

        /// <summary>
        /// 解析接收到的数据包
        /// </summary>
        /// <param name="data"></param>
        private void AnalyzeProtocol_LFI(byte[] data)
        {
            AspectF.Define.Retry()
                .Do(() =>
                {
                    //byte[] baddress = ByteHelper.HexStrToByteArrayWithDelimiter("06 56 50 50 87 02 12 21", " ", false);
                    //QueryElecData info = new QueryElecData();
                    ////info.StartChNo = 1;
                    ////info.ChNum = 1;
                    ////BrokerMessage sendMsg = new BrokerMessage(MessageType.queryElecData, 0, baddress, info);
                    ////TerminalShare.GetShare.SendToTerminal(sendMsg, 1);

                    //RealTimeCtrlLamp rtCtrl = new RealTimeCtrlLamp();
                    //rtCtrl.ChNo = 1;
                    //rtCtrl.OptValue = 0;
                    //rtCtrl.IsLock = 0;
                    //BrokerMessage sendMsg = new BrokerMessage(MessageType.realTimeCtrl, 0, baddress, rtCtrl);
                    //RealTimeCtrlLampHandler.GetHandler.RTCtrlByLampNo(sendMsg, 1);

                    //string str = "21 7b 22 4d 73 67 54 79 70 65 22 3a 33 2c 22 4d 73 67 4c 76 6c 22 3a 30 2c 22 54 65 72 6d 69 6e 61 6c 41 64 64 72 65 73 73 22 3a 22 42 6c 5a 51 55 49 63 43 45 69 45 3d 22 2c 22 4d 73 67 42 6f 64 79 22 3a 22 32 30 31 37 2d 30 32 2d 31 30 54 31 35 3a 30 36 3a 33 36 22 7d 24";
                    //data = ByteHelper.HexStrToByteArrayWithDelimiter(str, " ", false);
                    BrokerMessage bMsg = BrokerMessageHandler.GetHandler.BuildMessage(data);
                    string address= ByteHelper.ByteToHexStrWithDelimiter(bMsg.TerminalAddress, " ", false);
                    string strFilePath = Path.Combine(Environment.CurrentDirectory, "addr.txt");
                    if (UtilityHelper.CheckLogFile(address, strFilePath))
                    {
                        UtilityHelper.OpenLogFile(strFilePath);
                        UtilityHelper.WriteLogFile(address);
                        UtilityHelper.CloseLogFile();
                    }
                    if (bMsg != null)
                    {
                        if (bMsg.MsgType == MessageType.login)
                        {
                            if (TerminalShare.GetShare.dictAllHandlerFunction.ContainsKey(bMsg.MsgType))
                            {
                                TerminalShare.GetShare.dictAllHandlerFunction[bMsg.MsgType](bMsg);
                            }
                        }
                        else if (TerminalShare.GetShare.checkDevIsLoginOrNot(address))
                        {
                            TerminalClient curClient = TerminalShare.GetShare.ClientList[address] as TerminalClient;
                            curClient.HeartBeatTime = DateTime.Now;

                            if (TerminalShare.GetShare.dictAllHandlerFunction.ContainsKey(bMsg.MsgType))
                            {
                                TerminalShare.GetShare.dictAllHandlerFunction[bMsg.MsgType](bMsg);
                            }
                        }
                    }
                });
        }

        private void SendData(BrokerMessage bMsg, int no = -1)
        {
            byte[] data = new byte[0];
            AspectF.Define.Retry(()=> { ConnectTerminal(); })
                .Log(TerminalShare.GetShare.WriterLog,"", "SendData：" + ByteHelper.ByteToHexStrWithDelimiter(data, " ", false))
                .Do(() =>
                {
                    data = bMsg.ToBytes();
                    TerminalClient.GetStream().Write(data, 0, data.Length);

                    if (no != -1)
                    {
                        TerminalShare.GetShare.AddSendedCmdWord(bMsg, no);
                    }
                });
        }
    }
}
