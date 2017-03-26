using SuperSocket.SocketBase.Command;
using SuperSocket.SocketBase.Protocol;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Utility;

namespace PlatFormSocket.Command
{
    public class PlatFormProtocol:CommandBase<PlatFormSession,BinaryRequestInfo>
    {
        public override string Name
        {
            get
            {
                return "XN";
            }
        }

        public override void ExecuteCommand(PlatFormSession session, BinaryRequestInfo requestInfo)
        {
            AspectF.Define.Retry(Share.Instance.CatchExpection, (() => {
                Share.Instance.LogInfo("PlatFormProtocol:ExecuteCommand");
            }))
                .Do(() =>
                {
                    string curID = PackageHelper.GetPackageHelper.GetAddressFromXNProtocol(requestInfo.Body);
                    Share.Instance.WriteMsg("Recevice data from  PlatForm ,ID:"+session.PlatFormID+",DevID:"+ curID, 2);
                    Share.Instance.LogInfo(ByteHelper.byteToHexStr(requestInfo.Body));

                    var args = new SocketEventsArgs
                    {
                        DeviceID = curID,
                        Buffer = requestInfo.Body
                    };
                    session.AppServer.BroadcastDataToDev(args);

                    //var sessions = session.AppServer.GetAllSessions();
                    //if(sessions.Count()==0)
                    //{
                    //    Share.Instance.WriteMsg("No PlatForm is online！", 3);
                    //    return;
                    //}
                    //else
                    //{
                    //    var list = new List<PlatFormSession>(sessions);
                    //    foreach (var item in list)
                    //    {
                    //        if (item.PlatFormID == curID && item.SessionID != session.SessionID)
                    //        {
                    //            item.Close();
                    //        }
                    //    }

                    //    session.PlatFormID = curID;
                    //    var args = new SocketEventsArgs
                    //    {
                    //        DeviceID = curID,
                    //        Buffer = requestInfo.Body
                    //    };
                    //    session.AppServer.BroadcastDataToDev(args);
                    //}
                });
        }
    }
}
