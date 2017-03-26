using CommunicateCore.Model;
using CommunicateCore.Terminal.TerminalBusiness;
using CommunicateCore.Utility;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CommunicateCore.RTUSvr.RTUSvrBusiness
{
    public class QueryYearTableFromRTUSvrHandler
    {
        private static QueryYearTableFromRTUSvrHandler _QueryYearTableFromRTUSvrHandler;
        public static QueryYearTableFromRTUSvrHandler GetHandler
        {
            get
            {
                if(_QueryYearTableFromRTUSvrHandler==null)
                {
                    _QueryYearTableFromRTUSvrHandler = new QueryYearTableFromRTUSvrHandler();
                }
                return _QueryYearTableFromRTUSvrHandler;
            }
        }

        public void HandlerQueryYearTablePackage(LH_PackageData package)
        {
            AspectF.Define.Retry()
                .Do(() =>
                {
                    byte[] data = new byte[4];
                    Buffer.BlockCopy(package.OnlyData, 0, data, 0, 3);
                    LH_PackageData sendPackage = new LH_PackageData(package.CtuAddr, data, LHCmdWordConst.SendYearTableCFG);
                    RTUSvrShare.GetShare.SendToRTUSvr(sendPackage);

                    BrokerMessage bMsg = new BrokerMessage();
                    bMsg.MsgType = MessageType.queryYearTable;
                    bMsg.TerminalAddress = new byte[2];
                    Buffer.BlockCopy(package.CtuAddr, 0, bMsg.TerminalAddress, 0, 2);
                    int curIndex = 0;
                    YearTableInfo info = new YearTableInfo();
                    info.StartMonth = package.OnlyData[curIndex++];
                    info.StartDay = package.OnlyData[curIndex++];
                    info.DayNum[0]= package.OnlyData[curIndex++];
                    bMsg.MsgBody = info;

                    QueryYearTableHandler.GetHandler.SendQueryYearTableData(bMsg);
                });
        }

        public void QueryYearTableSuccess(byte[] cutAddr, BrokerMessage bMsg)
        {
            AspectF.Define.Retry()
                .Do(() =>
                {
                    //YearTableInfo info = bMsg.MsgBody as YearTableInfo;
                    string jsonStr = ((Newtonsoft.Json.Linq.JToken)bMsg.MsgBody).Root.ToString();
                    YearTableInfo info = JsonSerializeHelper.GetHelper.Deserialize<YearTableInfo>(jsonStr);
                    int len = BitConverter.ToInt16(info.DayNum, 0);
                    byte[] data = new byte[1 + 1 + 1 + len * 4];
                    int curIndex = 0;
                    data[curIndex++] = byte.Parse(info.StartMonth.ToString("X2"));
                    data[curIndex++] = byte.Parse(info.StartDay.ToString("X2"));
                    //查询时字节数为1
                    data[curIndex++] = info.DayNum[0];
                    //Buffer.BlockCopy(info.DayNum, 0, data, curIndex, 2);
                    //curIndex += 2;
                    for (int i=0;i<len;i++)
                    {
                        Buffer.BlockCopy(info.PlanTime[i].OpenTime, 0, data, curIndex, 2);
                        curIndex += 2;
                        Buffer.BlockCopy(info.PlanTime[i].CloseTime, 0, data, curIndex, 2);
                        curIndex += 2;
                    }

                    LH_PackageData package = new LH_PackageData(cutAddr, data, LHCmdWordConst.RecvQueryYearTableInfo);
                    RTUSvrShare.GetShare.SendToRTUSvr(package);
                });
        }
    }
}
