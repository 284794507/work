using OSRLamp.PackageHandler;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace OSRLamp.BusinessHandler
{
    public class HeartBeatHandler
    {
        private static HeartBeatHandler _HeartBeatHandler;
        public static HeartBeatHandler Instance
        {
            get
            {
                if (_HeartBeatHandler == null)
                {
                    _HeartBeatHandler = new HeartBeatHandler();
                }
                return _HeartBeatHandler;
            }
        }

        /// <summary>
        /// 心跳
        /// </summary>
        /// <param name="package"></param>
        public void HeartBeatPackageHandler(NJPackageData package)
        {
            AspectF.Define.Retry(Utility.CatchExpection)
                .Log(Utility.WriteLog, "", "Recevice HeartBeat！")
                .Do(() =>
                {
                    Utility.MasterResponse(package);
                    //byte[] data = new byte[5];
                    //Buffer.BlockCopy(package.SerialNumber, 0, data, 0, 2);
                    //Buffer.BlockCopy(package.CmdWord, 0, data, 2, 2);
                    //data[4] = 0;//0：成功/确认；1：失败；2：消息有误；3：不支持
                    ////NJPackageData sendPkg = new NJPackageData(CmdWord.Master_Response, new byte[] { 0x00, 0x00 }, package.TerminalID, Utility.GetSerialNo(), new byte[] { 0x01, 0x00, 0x01, 0x00 }, new byte[0], data);
                    //NJPackageData sendPkg = new NJPackageData(CmdWord.Master_Response, package.TerminalID, data);
                    //Utility.SendDataToTerminal(sendPkg);
                });
        }

        /// <summary>
        /// 监测心跳包
        /// </summary>
        public void MonitorRTUSvrHeartBeat()
        {
            try
            {
                ThreadPool.QueueUserWorkItem(
                    delegate
                    {
                        while (true)
                        {
                            Thread.Sleep(60000);
                            List<string> delClient = new List<string>();
                            foreach (string devID in Utility.ClientList.Keys)
                            {
                                DevClient dev = Utility.ClientList[devID] as DevClient;
                                TimeSpan ts = DateTime.Now - dev.HeartBeatTime;
                                if (ts.Minutes >= Utility.HeartBeatTime * 3)
                                {
                                    delClient.Add(devID);
                                }

                            }
                            foreach (string devID in delClient)//心跳过期则删除
                            {
                                Utility.ClientList.Remove(devID);
                            }
                        }
                    }
                    );
            }
            catch (Exception ex)
            {
                Utility.WriteLog(ex.Message, 4);
                MonitorRTUSvrHeartBeat();
            }
        }
    }
}
