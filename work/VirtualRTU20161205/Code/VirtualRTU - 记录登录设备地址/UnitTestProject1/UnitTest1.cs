using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using CommunicateCore.Terminal.TerminalBusiness;

namespace UnitTestProject1
{
    [TestClass]
    public class UnitTest1
    {
        [TestMethod]
        public void TestMethod1()
        {
        }

        [TestMethod]
        public void LoginBack()
        {
            LoginHandler.GetHandler.HandlerLoginBackMessage(null);
        }

        [TestMethod]
        public void RealCtrl()
        {
            RealTimeCtrlLampHandler.GetHandler.RTCtrlByLampNo("02 54 54 09 67 12 19 22",1,1,0);
        }
    }
}
