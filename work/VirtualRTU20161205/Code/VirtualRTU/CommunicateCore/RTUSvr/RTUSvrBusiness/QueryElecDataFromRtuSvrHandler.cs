using CommunicateCore.Terminal.TerminalBusiness;
using CommunicateCore.Utility;
using LFCDal.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CommunicateCore.RTUSvr.RTUSvrBusiness
{
    public class QueryElecDataFromRtuSvrHandler
    {
        //查询电参数
        private static QueryElecDataFromRtuSvrHandler _QueryElecDataFromRtuSvrHandler;
        public static QueryElecDataFromRtuSvrHandler GetHandler
        {
            get
            {
                if(_QueryElecDataFromRtuSvrHandler==null)
                {
                    _QueryElecDataFromRtuSvrHandler = new QueryElecDataFromRtuSvrHandler();
                }
                return _QueryElecDataFromRtuSvrHandler;
            }
        }

        public void HandlerQueryElecDataPackage(LH_PackageData package)
        {
            AspectF.Define.Retry()
                .Do(() =>
                {
                    byte[] LampNo = new byte[2];
                    Buffer.BlockCopy(package.OnlyData, 0, LampNo, 0, 2);

                    int no = BitConverter.ToInt16(LampNo, 0);

                    if (!RTUSvrShare.GetShare.CheckRepeatCmdWord(no, package))
                    {
                        return;
                    }

                    string key = ByteHelper.byteToHexStr(LHCmdWordConst.SendRTQueryLampDetailStatus) + "_" + no.ToString();
                    if (!RTUSvrShare.GetShare.dictSendByRTUSvr.ContainsKey(key))
                    {
                        RTUSvrShare.GetShare.dictSendByRTUSvr.Add(key, "");
                    }

                    //先回复确认报文
                    RTUSvrShare.GetShare.SendToRTUSvrWithNoData(package.CtuAddr, LHCmdWordConst.RecvRTQueryLampDetailStatus_Quick);

                    string ctuAddr = ByteHelper.bytesToCtuAddr(package.CtuAddr, true);
                    ElecDataHandler.GetHandler.SendQueryElecData(ctuAddr, no);
                });
        }

        public void QueryElecDataSuccess(byte[] cutAddr, tLampHisDataRec val)
        {
            AspectF.Define.Retry()
                .Do(() =>
                {
                    string key = ByteHelper.byteToHexStr(LHCmdWordConst.SendRTQueryLampDetailStatus) + "_" + val.LampNo.ToString();
                    if (!RTUSvrShare.GetShare.dictSendByRTUSvr.ContainsKey(key))//非平台发送请求，不返回
                    {
                        return;
                    }

                    byte[] data = new byte[15];
                    byte[] arr = BitConverter.GetBytes((short)val.LampNo);
                    data[0] = arr[0];
                    data[1] = arr[1];
                    data[2] = 1;//默认写１,通信次数
                    data[3] = (byte)val.LampStatus;
                    arr = BitConverter.GetBytes((short)(val.LampU * 100));
                    data[4] = arr[0];
                    data[5] = arr[1];
                    arr = BitConverter.GetBytes((short)(val.LampI * 100));
                    data[6] = arr[0];
                    data[7] = arr[1];
                    arr = BitConverter.GetBytes((short)(val.LampAP * 10));
                    data[8] = arr[0];
                    data[9] = arr[1];
                    data[10] = (byte)(val.LampPF * 100);
                    arr = BitConverter.GetBytes((short)(val.LampVP * 100));
                    data[11] = arr[0];
                    data[12] = arr[1];
                    data[13] = 0;
                    data[14] = 0;

                    LH_PackageData package = new LH_PackageData(cutAddr, data, LHCmdWordConst.RecvRTQueryLampDetailStatus);
                    RTUSvrShare.GetShare.SendToRTUSvr(package);
                });
        }

        public void QueryElecDataFail(byte[] cutAddr, byte[] macAddr, byte plcCmd)
        {
            RTUSvrShare.GetShare.SendBackFail(cutAddr, macAddr, plcCmd);
        }
    }
}
