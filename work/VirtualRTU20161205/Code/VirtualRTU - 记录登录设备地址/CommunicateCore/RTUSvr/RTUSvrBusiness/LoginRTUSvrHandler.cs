using CommunicateCore.Utility;
using LFCDal.Model;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace CommunicateCore.RTUSvr.RTUSvrBusiness
{
    public class LoginRTUSvrHandler
    {
        private static LoginRTUSvrHandler _LoginHandler;
        public static LoginRTUSvrHandler GetLoginHandler
        {
            get
            {
                if (_LoginHandler == null)
                {
                    _LoginHandler = new LoginRTUSvrHandler();
                }
                return _LoginHandler;
            }
        }

        public void SendLoginPackge()
        {
            AspectF.Define.Retry()
                .Do(() =>
                {
                    tCTUInfo[] ctuInfo = RTUSvrShare.GetShare.CtuList;
                    byte[] macAddress = new byte[] { 0xFF, 0xFF, 0xFF, 0xFF, 0xFF, 0xFF };
                    foreach (tCTUInfo info in ctuInfo)
                    {
                        macAddress = ByteHelper.HexStrToByteArrayWithDelimiter(info.CTUMacAddr.Trim(), "-");
                        byte[] IMSI = new byte[] { 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00 };
                        byte[] temp = ByteHelper.HexStrToByteArrayWithDelimiter(info.SIM_IMSI.Trim(), "-");
                        int len = IMSI.Length - temp.Length;
                        if (len < 0) len = 0;
                        for (int i = len; i < 8; i++)
                        {
                            IMSI[i] = temp[i - len];
                        }
                        byte[] IMEI = new byte[] { 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00 };
                        temp = ByteHelper.HexStrToByteArrayWithDelimiter(info.GSM_IMEI.Trim(), "-");
                        len = IMSI.Length - temp.Length;
                        if (len < 0) len = 0;
                        for (int i = len; i < 8; i++)
                        {
                            IMEI[i] = temp[i - len];
                        }
                        byte[] data = new byte[0];

                        byte[] CurVersion = new byte[] { 0x00, 0x00 };
                        tSysRunStatus runStatus = RTUSvrShare.GetShare.GetRunStatusByCtuID(info.CTUID);
                        if (runStatus != null)
                        {
                            int ver = runStatus.MainVer.Value;
                            CurVersion[0] = (byte)((byte)(ver / 10) << 4 | (byte)(ver % 10)); // byte.Parse(runStatus["MainVer"].ToString().Trim());
                            ver = runStatus.SubVer.Value;
                            CurVersion[1] = (byte)((byte)(ver / 10) << 4 | (byte)(ver % 10)); //byte.Parse(runStatus["SubVer"].ToString().Trim());
                        }

                        using (MemoryStream mem = new MemoryStream())
                        {
                            BinaryWriter writer = new BinaryWriter(mem);
                            writer.Write(macAddress);
                            writer.Write(CurVersion);
                            writer.Write(IMSI);
                            writer.Write(IMEI);
                            data = mem.ToArray();
                            writer.Close();
                        }
                        byte[] ctuAddr = ByteHelper.CtuAddrToBytes(info.CTUCommAddr);//ByteHelper.CtuAddrToBytes(int.Parse(info.CTUCommAddr.Trim()).ToString("X2"));
                        LH_PackageData package = new LH_PackageData(ctuAddr, data, LHCmdWordConst.GetLogin);
                        RTUSvrShare.GetShare.SendToRTUSvr(package);
                    }
                });
        }

        public void HandlerLoginBackPackage(LH_PackageData package)
        {
            byte result = package.OnlyData[0];
            string msg = "";
            RTUSvrShare.GetShare.LoginSuccess = false;

            switch (result)
            {
                case 0x00:
                    msg = "登录成功！";
                    RTUSvrShare.GetShare.HeartBeatTime = DateTime.Now;
                    RTUSvrShare.GetShare.LoginSuccess = true;
                    HeartBeatHandler.GetHeartBeatHandler.MonitorRTUSvrHeartBeat(package.CtuAddr);
                    break;
                case 0x01:
                    msg = "非法CTUMAC！";
                    break;
                case 0x02:
                    msg = "IMSI非法！";
                    break;
                case 0x03:
                    msg = "IMEI非法！";
                    break;
                case 0x04:
                    msg = "版本号过低！";
                    break;
                case 0x05:
                    msg = "重复登录！";
                    break;
                case 0x06:
                    msg = "登录CTU数已达上限！";
                    break;
                case 0x07:
                    msg = "服务器拒绝登录！";
                    break;
                case 0x08:
                    msg = "通讯地址不正确！";
                    break;
            }

            RTUSvrShare.GetShare.WriteLog_RTUSvr("HandlerLoginBackPackage：" + msg);
            if (result != 0x00)
            {
                Thread.Sleep(2000);
                //重复登录，直到成功为止
                SendLoginPackge();
            }
        }        
    }
}
