using CommunicateCore.Utility;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace CommunicateCore.RTUSvr.RTUSvrBusiness
{
    public class HeartBeatHandler
    {
        private static HeartBeatHandler _HeartBeatHandler;
        public static HeartBeatHandler GetHeartBeatHandler
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

        public void HandlerHeartBeatBackPackage(LH_PackageData package)
        {
            RTUSvrShare.GetShare.HeartBeatNumber=0;
        }

        /// <summary>
        /// 监测心跳包
        /// </summary>
        public void MonitorRTUSvrHeartBeat(byte[] ctuAddr)
        {
            AspectF.Define.Retry(()=>
                {
                    MonitorRTUSvrHeartBeat(ctuAddr);
                })
                .Do(() =>
                {
                    ThreadPool.QueueUserWorkItem(
                    delegate
                    {
                        while (true)
                        {
                            TimeSpan ts = DateTime.Now - RTUSvrShare.GetShare.HeartBeatTime;
                            if (ts.Minutes >= RTUSvrShare.GetShare.HeartBeatInterval)
                            {
                                RTUSvrShare.GetShare.HeartBeatTime = DateTime.Now;
                                if (RTUSvrShare.GetShare.HeartBeatNumber >= 3)//三次连接收不到回复，则重新连接
                                {
                                    //RTUSvr_TCPClient.GetRTUSvr_TCPClient.Reconnect();
                                }
                                else
                                {
                                    SendHeartBeatPackge(ctuAddr);
                                }
                            }
                            Thread.Sleep(10000);
                        }
                    }
                    );
                });
        }
        
        private void SendHeartBeatPackge(byte[] ctuAddr)
        {
            RTUSvrShare.GetShare.SendToRTUSvrWithNoData(ctuAddr,LHCmdWordConst.GetHeartBeat);
            RTUSvrShare.GetShare.RefreshHeartBeatTime();
            RTUSvrShare.GetShare.HeartBeatNumber++;
        }
    }
}
