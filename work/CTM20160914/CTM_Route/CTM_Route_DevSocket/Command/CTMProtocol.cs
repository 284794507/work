using CTM_Route_AnalySocket;
using CTM_Route_Utility;
using SuperSocket.SocketBase.Command;
using SuperSocket.SocketBase.Protocol;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Utility;

namespace CTM_Route_DevSocket.Command
{
    public class CTMProtocol:CommandBase<DevSession,BinaryRequestInfo>
    {
        public override string Name
        {
            get
            {
                return "CTM";
            }
        }

        public override void ExecuteCommand(DevSession session, BinaryRequestInfo requestInfo)
        {
            AspectF.Define.Retry(Route_Utility.Instance.CatchExpection)
                   .Do(() =>
                   {
                       string curID = PackageHelper.GetPackageHelper.GetAddressFromCTMProtocol(requestInfo.Body);
                       //如果客户端不断开连接，直接关闭程序，可能导致session一直存在
                       var sessions = session.AppServer.GetAllSessions();
                       if (sessions.Count() > 0)
                       {
                           var list = new List<DevSession>(sessions);
                           foreach (var item in list)
                           {
                               if (item.DeviceID == curID && item.SessionID != session.SessionID)
                               {
                                   item.Close();
                               }
                           }
                       }

                       session.DeviceID = curID;
                       var args = new SocketEventsArgs
                       {
                           DeviceID = session.DeviceID,
                           Buffer = requestInfo.Body
                       };
                       session.AppServer.SendAnalySvr(args);
                   });
        }
    }
}
