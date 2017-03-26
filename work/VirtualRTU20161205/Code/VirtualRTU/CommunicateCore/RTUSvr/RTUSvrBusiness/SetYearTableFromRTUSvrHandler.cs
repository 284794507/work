using CommunicateCore.Model;
using CommunicateCore.Terminal;
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
    public class SetYearTableFromRTUSvrHandler
    {
        private static SetYearTableFromRTUSvrHandler _SetYearTableFromRTUSvrHandler;
        public static SetYearTableFromRTUSvrHandler GetHandler
        {
            get
            {
                if(_SetYearTableFromRTUSvrHandler==null)
                {
                    _SetYearTableFromRTUSvrHandler = new SetYearTableFromRTUSvrHandler();
                }
                return _SetYearTableFromRTUSvrHandler;
            }
        }

        public void HandlerSetYearTablePackage(LH_PackageData package)
        {
            AspectF.Define.Retry()
                .Do(() =>
                {
                    byte[] data = new byte[4];
                    Buffer.BlockCopy(package.OnlyData, 0, data, 0, 4);
                    LH_PackageData sendPackage = new LH_PackageData(package.CtuAddr, data, LHCmdWordConst.SendYearTableCFG);
                    RTUSvrShare.GetShare.SendToRTUSvr(sendPackage);

                    BrokerMessage bMsg = new BrokerMessage();
                    bMsg.MsgType = MessageType.setYearTable;
                    bMsg.TerminalAddress = new byte[2];
                    Buffer.BlockCopy(package.CtuAddr, 0, bMsg.TerminalAddress, 0, 2);
                    int curIndex = 0;
                    YearTableInfo info = new YearTableInfo();
                    info.StartMonth = package.OnlyData[curIndex++];
                    info.StartDay = package.OnlyData[curIndex++];
                    short days = BitConverter.ToInt16(package.OnlyData, curIndex);//改成2字节，一次性下发
                    Buffer.BlockCopy(package.OnlyData, curIndex, info.DayNum, 0, 2);
                    curIndex += 2;
                    info.PlanTime = new OpenAndCloseTime[days];
                    for(int i=0;i<days;i++) 
                    {
                        OpenAndCloseTime newTime = new OpenAndCloseTime();
                        Buffer.BlockCopy(package.OnlyData, curIndex, newTime.OpenTime, 0, 2);
                        curIndex += 2;
                        Buffer.BlockCopy(package.OnlyData, curIndex, newTime.CloseTime, 0, 2);
                        info.PlanTime[i] = newTime;
                        curIndex += 2;
                    }
                    bMsg.MsgBody = info;

                    SetYearTableHandler.GetHandler.SendYearTableData(bMsg);
                });
        }
    }
}
