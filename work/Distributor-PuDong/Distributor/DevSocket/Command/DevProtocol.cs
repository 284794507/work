using SuperSocket.SocketBase.Command;
using SuperSocket.SocketBase.Protocol;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Utility;

namespace DevSocket.Command
{
    public class DevProtocol:CommandBase<DevSession,BinaryRequestInfo>
    {
        public override string Name
        {
            get
            {
                return "XN";
            }
        }

        public override void ExecuteCommand(DevSession session, BinaryRequestInfo requestInfo)
        {
            AspectF.Define.Retry(Share.Instance.CatchExpection, (() => {
                Share.Instance.LogInfo("DevProtocol:ExecuteCommand");
            }))
                .Do(() =>
                {
                    string curID = PackageHelper.GetPackageHelper.GetAddressFromXNProtocol(requestInfo.Body);
                    Share.Instance.WriteMsg("Recevice data from  Dev ,ID:" + curID, 2);
                    var sessions = session.AppServer.GetAllSessions();
                    if (sessions.Count() == 0)
                    {
                        Share.Instance.WriteMsg("No Dev is online！", 3);
                        return;
                    }
                    else
                    {
                        //var list = new List<DevSession>(sessions);
                        //foreach (var item in list)
                        //{
                        //    if (item.DevID == curID && item.SessionID != session.SessionID)
                        //    {
                        //        item.Close();
                        //    }
                        //}
                    }

                    session.DevID = curID;
                    var args = new SocketEventsArgs
                    {
                        DeviceID = curID,
                        Buffer = requestInfo.Body
                    };
                    session.AppServer.BroadcastPlatFormSvr(args);
                });
        }
    }
}
