using CommunicateCore.Utility;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace VirtualRTU
{
    public class ValidateFromServer
    {
        private static ValidateFromServer _ValidateFromServer;
        public static ValidateFromServer Instance
        {
            get
            {
                if(_ValidateFromServer == null)
                {
                    _ValidateFromServer = new ValidateFromServer();
                }
                return _ValidateFromServer;
            }
        }

        private IPAddress ipAddress;
        private int port;
        private int tokenInterval;
        private string tokenKey;
        private TcpClient tokenClient;
        private byte[] ReceviceBuffer;
        public bool IsStart;
        public static string PrivateKey = "LH3ICtrl";
        public string VerificationCode;

        public void InitClient()
        {
            AspectF.Define.Retry()
                .Do(() =>
                {
                    UtilityHelper.GetHelper.IsAllowed = false;
                    InitConfig();
                    tokenClient = new TcpClient();
                    ReceviceBuffer = new byte[tokenClient.ReceiveBufferSize];
                    ConnectServer();
                    IsStart = true;
                    MonitorTokenClient();
                    RequestAllow();
                });
        }

        public void InitConfig()
        {
            string ip = ConfigurationManager.AppSettings["TokenIP"];
            ipAddress = IPAddress.Parse(ip);

            string sPort = ConfigurationManager.AppSettings["TokenPort"];
            port = int.Parse(sPort);

            string key = ConfigurationManager.AppSettings["ServerKey"];
            tokenKey = Base64.DecryptDES(key, PrivateKey);

            string interval = ConfigurationManager.AppSettings["TokenInterval"];
            tokenInterval = int.Parse(interval);
        }


        public void ConnectServer()
        {
            AspectF.Define.Retry()
                .Do(() =>
                {
                    tokenClient.Connect(ipAddress, port);                    
                    tokenClient.GetStream().BeginRead(ReceviceBuffer, 0, ReceviceBuffer.Length, TokenClientRecevice, ReceviceBuffer);
                });
        }

        private void MonitorTokenClient()
        {
            AspectF.Define.Retry(() =>
            {
                ConnectServer();
            })
                .Do(() =>
                {
                    ThreadPool.QueueUserWorkItem(
                    delegate
                    {
                        while (IsStart)
                        {
                            Thread.Sleep(1000);
                            if (IsStart && !tokenClient.Connected)
                            {
                                ConnectServer();
                            }
                            else
                            {
                                try
                                {
                                    tokenClient.GetStream().Write(new byte[0] {}, 0,0);
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

        private void TokenClientRecevice(IAsyncResult ar)
        {
            AspectF.Define.Retry()
                .Do(() =>
                {
                    if (tokenClient.Connected)
                    {
                        NetworkStream NetStream = tokenClient.GetStream();
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

                        if (receviceBytes[0] == 1)
                        {
                            UtilityHelper.GetHelper.IsAllowed = true;
                        }
                        else
                        {
                            RequestAllow();
                        }
                        if (!UtilityHelper.GetHelper.IsAllowed)
                        {
                            tokenClient.GetStream().BeginRead(ReceviceBuffer, 0, ReceviceBuffer.Length, TokenClientRecevice, ReceviceBuffer);
                        }
                    }
                });
                    
        }

        public  void RequestAllow()
        {
            CreateKey();
            Console.WriteLine(VerificationCode);
            byte[] bKey = Encoding.UTF8.GetBytes(VerificationCode);
            if(tokenClient.Connected)
            {
                tokenClient.GetStream().Write(bKey, 0, bKey.Length);
            }
        }

        public void Close()
        {
            IsStart = false;
            tokenClient.Close();
        }


        private void CreateKey()
        {
            long T0 = 0;
            long X = tokenInterval;
            TimeSpan ts = DateTime.Now - DateTime.MinValue;
            long Time = (long)ts.TotalSeconds;
            string steps = "0";

            long T = (Time - T0) / X;
            steps = T.ToString("X2");
            while (steps.Length < 16)
            {
                steps = "0" + steps;
            }

            VerificationCode = TOTP.GenerateTOTP(tokenKey, steps, "8");
        }
    }
}
