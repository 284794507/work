using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using PassengerFlowDal.Utility;
using PassengerFlowDal.Model;
using PassengerFlowDal.WcfServer;

namespace UnitTestProject1
{
    [TestClass]
    public class UnitTest1
    {
        [TestMethod]
        public void TestMethod1()
        {
            TabCurData info = new TabCurData();
            info.DevMac = "47 59 32 32 52 42 12 43";
            info.Channel = 0x0b;
            info.ModuleNo = 3;
            info.DevTime = 0x8d4ec07e;
            info.RSSI = 0xa2;
            DalParameter para = new DalParameter();
            para.BusinessObject = info;
            para.BusinessType = BusinessType.AddData;

            DalSvr dal = new DalSvr();
            dal.AddInfo(para);
        }

        [TestMethod]
        public void TestGetArea()
        {
            TabArea info = new TabArea();
            info.AreaName = "fsdf";
            DalParameter para = new DalParameter();
            para.BusinessObject = "fsdf";
            para.BusinessType = BusinessType.GetAreaInfo;

            DalSvr dal = new DalSvr();
            DalParameter rPara = dal.GetInfo(para);
            TabArea[] arrInfo = rPara.BusinessObject as TabArea[];
        }
    }
}
