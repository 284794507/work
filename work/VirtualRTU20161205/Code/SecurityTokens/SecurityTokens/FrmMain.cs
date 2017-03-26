using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Configuration;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;
using Utility;
using static SecurityTokens.Totp;

namespace SecurityTokens
{
    public partial class FrmMain : Form
    {
        public FrmMain()
        {
            InitializeComponent();

            InitForm();

            CreateKey();
        }

        TcpListener TcpServer;
        private static int TCPNum;
        private byte[] ReceviceBuffer;
        TcpClient CurClient;

        private void InitForm()
        {
            InitConfig();

            this.proBar.Minimum = 0;
            this.proBar.Maximum = (int)Utility.Interval;
            this.proBar.Value = 0;
                        
            MonitorProBar();

            TcpServer = new TcpListener(IPAddress.Parse(Utility.IP), Utility.Port);

            ThreadStart s = new ThreadStart(AcceptConnect);
            Thread tcpThread = new Thread(s);
            tcpThread.Start();
        }

        private void AcceptConnect()
        {
            AspectF.Define.Retry()
                .Do(() =>
                {
                    TcpServer.Start(1);
                    while(true)
                    {
                        AsyncCallback callback = new AsyncCallback(AcceptTcpClient);
                        TcpServer.BeginAcceptTcpClient(callback, TcpServer);
                    }
                });
        }

        private void AcceptTcpClient(IAsyncResult ar)
        {
            AspectF.Define.Retry().
                Do(() =>
                {
                    TcpListener myListener = ar.AsyncState as TcpListener;
                    TcpClient tcpClient = myListener.EndAcceptTcpClient(ar);
                    TCPNum++;
                    if(TCPNum>1)
                    {
                        if(CurClient!=null)
                        {
                            CurClient.Close();
                        }
                    }
                    CurClient = tcpClient;
                    ReceviceBuffer = new byte[tcpClient.ReceiveBufferSize];
                    tcpClient.GetStream().BeginRead(ReceviceBuffer, 0, ReceviceBuffer.Length, ReadCallBack, ReceviceBuffer);
                });
        }

        private void ReadCallBack(IAsyncResult ar)
        {
            AspectF.Define.Retry().
                Do(() =>
                {
                    if (CurClient.Connected)
                    {
                        NetworkStream NetStream = CurClient.GetStream();
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

                        string code = ByteHelper.AsciiBytesToString(receviceBytes);
                        byte[] result = new byte[1];
                        RefreshKey();
                        if (code == Utility.VerificationCode || code == Utility.VerificationCode_old || code== Utility.VerificationCode_old_old)
                        {
                            result[0] = 1;
                        }
                        else
                        {
                            result[0] = 0;
                        }
                        CurClient.GetStream().Write(result, 0, result.Length);

                        CurClient.GetStream().BeginRead(ReceviceBuffer, 0, ReceviceBuffer.Length, ReadCallBack, ReceviceBuffer);
                    }
                });
        }

        public void InitConfig()
        {            
            string key = ConfigurationManager.AppSettings["ServerKey"];
            Utility.CurKey = Base64.DecryptDES(key, Utility.PrivateKey);

            string time = ConfigurationManager.AppSettings["Interval"];
            Utility.Interval = uint.Parse(time);

            string ip = ConfigurationManager.AppSettings["IP"];
            Utility.IP = ip;

            string port = ConfigurationManager.AppSettings["Port"];
            Utility.Port = int.Parse(port);
        }

        public void MonitorProBar()
        {
            ThreadPool.QueueUserWorkItem(
                    delegate
                    {
                        while (true)
                        {
                            Thread.Sleep(1000);
                            AlterProcessBar();
                        }
                    }
                    );
        }
        
        private delegate void SetPos(int curValue);//代理

        private void AlterProcessBar(int curValue=0)
        {
            AspectF.Define.Retry()
                .Do(() =>
                {
                    if (this.InvokeRequired)
                    {
                        SetPos setpos = new SetPos(AlterProcessBar);
                        this.Invoke(setpos, curValue);
                    }
                    else
                    {
                        if (this.proBar.Value >= this.proBar.Maximum || curValue == -1)
                        {
                            this.proBar.Value = 0;
                            CreateKey();
                        }
                        this.proBar.Value++;
                    }
                });
        }

        private void RefreshKey()
        {
            CreateKey();
            AlterProcessBar(-1);
        }

        private void btnCreate_Click(object sender, EventArgs e)
        {
            RefreshKey();
        }

        /// <summary>
        /// 生成随机数的种子
        /// </summary>
        /// <returns></returns>
        private static int getNewSeed()
        {
            byte[] rndBytes = new byte[4];
            System.Security.Cryptography.RNGCryptoServiceProvider rng = new System.Security.Cryptography.RNGCryptoServiceProvider();
            rng.GetBytes(rndBytes);
            return BitConverter.ToInt32(rndBytes, 0);
        }     /// <summary>
              /// 生成8位随机数
              /// </summary>
              /// <param name="length"></param>
              /// <returns></returns>
        static public string GetRandomString(int len)
        {
            string s = "123456789abcdefghijklmnpqrstuvwxyzABCDEFGHIJKLMNPQRSTUVWXYZ";
            string reValue = string.Empty;
            Random rnd = new Random(getNewSeed());
            while (reValue.Length < len)
            {
                string s1 = s[rnd.Next(0, s.Length)].ToString();
                if (reValue.IndexOf(s1) == -1) reValue += s1;
            }
            return reValue;
        }

        private void CreateKey()
        {
            // Seed for HMAC-SHA1 - 20 bytes
            string seed = "3132333435363738393031323334353637383930";
            // Seed for HMAC-SHA256 - 32 bytes
            string seed32 = "3132333435363738393031323334353637383930" +
            "313233343536373839303132";
            // Seed for HMAC-SHA512 - 64 bytes
            string seed64 = "3132333435363738393031323334353637383930" +
            "3132333435363738393031323334353637383930" +
            "3132333435363738393031323334353637383930" +
            "31323334";

            this.proBar.Maximum = (int)Utility.Interval; 

            long T0 = 0;
            long X = Utility.Interval;
            TimeSpan ts = DateTime.Now - DateTime.MinValue;
            long Time = (long)ts.TotalSeconds;
            string steps = "0";

            long T = (Time - T0) / X;
            steps = T.ToString("X2");
            while (steps.Length < 16)
            {
                steps = "0" + steps;
            }

            Utility.VerificationCode = Totp.GenerateTOTP(Utility.CurKey, steps, "8");
            Utility.VerificationCode_old = Utility.VerificationCode;
            Utility.VerificationCode_old_old = Utility.VerificationCode_old;
            ShowKey();
        }

        private void btnAlterKey_Click(object sender, EventArgs e)
        {
            FrmAlterKey frm = new FrmAlterKey();
            frm.Show();
        }


        private delegate void SetKey();//代理

        private void ShowKey()
        {
            if (this.InvokeRequired)
            {
                SetKey setpos = new SetKey(ShowKey);
                this.Invoke(setpos);
            }
            else
            {
                this.txtVerificationCode.Text = Utility.VerificationCode;
            }
        }
    }
}
