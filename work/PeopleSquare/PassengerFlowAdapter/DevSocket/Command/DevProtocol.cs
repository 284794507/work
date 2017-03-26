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
                return "Flow";
            }
        }

        public override void ExecuteCommand(DevSession session, BinaryRequestInfo requestInfo)
        {
            AspectF.Define.Retry(Share.Instance.CatchExpection)
                .Do(() =>
                {
                    if(requestInfo== default(BinaryRequestInfo) || requestInfo.Body==null || requestInfo.Body.Length<17)
                    {
                        Share.Instance.WriteLog("包长度错误！ ");
                        return;
                    }
                    string curID = PackageHelper.Instance.GetAddrFromPackage(requestInfo.Body);
                    
                    session.DevID = curID;
                    var args = new SocketEventsArgs
                    {
                        ObjID = curID,
                        Buffer = requestInfo.Body
                    };
                    session.AppServer.BroadcastBackEndSvr(args);
                });
        }
    }
}
