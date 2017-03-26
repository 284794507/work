using CTM_Route_Utility;
using SuperSocket.SocketBase.Command;
using SuperSocket.SocketBase.Protocol;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Utility;

namespace CTM_Route_AnalySocket.Command
{
    public class CTMProtocol:CommandBase<AnalySession,BinaryRequestInfo>
    {
        public override string Name
        {
            get
            {
                return "CTM";
            }
        }

        public override void ExecuteCommand(AnalySession session, BinaryRequestInfo requestInfo)
        {
            AspectF.Define.Retry(Route_Utility.Instance.CatchExpection)
                .Do(() =>
                {
                    string curID = PackageHelper.GetPackageHelper.GetAddressFromCTMProtocol(requestInfo.Body);
                    //如果客户端不断开连接，直接关闭程序，可能导致session一直存在
                    var sessions = session.AppServer.GetAllSessions();
                    if (sessions.Count() > 0)
                    {
                        var list = new List<AnalySession>(sessions);
                        foreach (var item in list)
                        {
                            if (item.ServerID == curID && item.SessionID != session.SessionID)
                            {
                                item.Close();
                            }
                        }
                    }
                    session.ServerID = curID;
                    var args = new SocketEventsArgs
                    {
                        DeviceID = session.ServerID,
                        Buffer = requestInfo.Body
                    };
                    session.AppServer.BroadcastMessageToDev(args);
                });
        }
    }
}
