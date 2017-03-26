using SuperSocket.SocketBase.Command;
using SuperSocket.SocketBase.Protocol;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Utility;

namespace BackEndSocket.Command
{
    public class BackEndProtocol : CommandBase<BackEndSession, BinaryRequestInfo>
    {
        public override string Name
        {
            get
            {
                return "Flow";
            }
        }

        public override void ExecuteCommand(BackEndSession session, BinaryRequestInfo requestInfo)
        {
            AspectF.Define.Retry(Share.Instance.CatchExpection)
                .Do(() =>
                {
                    if (requestInfo.Body == null || requestInfo.Body.Length < 17)
                    {
                        Share.Instance.WriteLog("包长度错误！ ");
                        return;
                    }
                    string curID = PackageHelper.Instance.GetAddrFromPackage(requestInfo.Body);

                    var args = new SocketEventsArgs
                    {
                        ObjID = curID,
                        Buffer = requestInfo.Body
                    };
                    session.AppServer.BroadcastDataToDev(args);                    
                });
        }
    }
}
