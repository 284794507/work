using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using CTMDAL.Utility;
using CTMDAL.Model;
using CTMDAL.WcfServer;

namespace UnitTestProject1
{
    [TestClass]
    public class UnitTest1
    {
        [TestMethod]
        public void TestUpdateCollectorInfo()
        {
            CTMDalParameter sendPara = new CTMDalParameter();
            sendPara.BusinessType = BusinessType.GetCollectorInfo;
            CTMDal aDal = new CTMDal();
            aDal.GetInfo(sendPara);

            //TPLCollectorAndMeter meter = new TPLCollectorAndMeter();
            //meter.CollectorID = new Guid("a697b105-c9a2-426b-9ca0-9bdf6bf966e3");
            //meter.CollectorCode = "D032A1N17000016";
            //meter.SupplyCode = "123456";
            //meter.UserName = "路辉";
            //meter.UserAddress = "龙漕路229号2B栋6楼";

            //CTMDalParameter sendPara = new CTMDalParameter();
            //sendPara.BusinessType = BusinessType.BindSupplyCode;
            //sendPara.BusinessObject = meter;
            //CTMDal aDal = new CTMDal();
            //aDal.AddInfo(sendPara);

            //CTMDalParameter sendPara = new CTMDalParameter();
            //sendPara.BusinessType = BusinessType.GetSupplyInfo;
            //CTMDal aDal = new CTMDal();
            //CTMDalParameter para = aDal.GetInfo(sendPara);

            //TPLCollectorInfo info = new TPLCollectorInfo();
            ////info.SVer = "";
            ////info.HVer = "115-C-1";
            //info.MacAddr = "51-77-51-51-20-09-03-87";
            //info.SNCode = "12244";
            ////info.TheDate = DateTime.MinValue;
            ////info.DevType = 1;
            ////info.DevStatus=1;
            ////info.ChannelNo = 0;
            ////info.Status = 0;
            //info.Lat = decimal.Parse("31.178483");
            //info.Lon = decimal.Parse("121.449219");
            //info.Address = "上海市徐汇区龙漕路253号";

            //CTMDalParameter sendPara = new CTMDalParameter();
            //sendPara.BusinessType = BusinessType.UpdateCollectorInfo;
            //sendPara.BusinessObject = info;

            //CTMDal aDal = new CTMDal();
            //aDal.UpdateInfo(sendPara);
        }

        [TestMethod]
        public void TestUpdateDataRecRTM()
        {
            TPLDataRecRTM info = new TPLDataRecRTM();
            info.DevID = new Guid("4984F74E-2E74-4068-BAA7-BE5EAA464D2F");
            info.LampVoltageA = 655.35M;
            info.LampCurrentA = 655.35M;
            info.LampVoltageB = 655.35M;
            info.LampCurrentB = 655.35M;
            info.LampVoltageC = 655.35M;
            info.LampCurrentC = 655.35M;
            info.GetDataTime = DateTime.Now;
            info.DevStatus = 1;
            info.BatchID = new Guid("4984F74E-2E74-4068-BAA7-BE5EAA464D21");
            
            CTMDalParameter sendPara = new CTMDalParameter();
            sendPara.BusinessType = BusinessType.UpdateDataRecRTM;
            sendPara.BusinessObject = info;

            CTMDal aDal = new CTMDal();
            aDal.UpdateInfo(sendPara);
        }

        [TestMethod]
        public void TestUpdateCollectorMasterCommStatus()
        {
            TPLCollectorMasterCommStatus_Cur info = new TPLCollectorMasterCommStatus_Cur();
            info.PLCollectorInfoID = new Guid("dfb739d1-276c-474f-b9c4-a5d96d9b6fac");
            info.CommStatus = 0;
            info.ChkDataTime = DateTime.Now;
            info.SuccessfulCommTimes = 1;
            info.TotalCommTimes = 2;
            info.LostRate = new decimal(0.5);
            CTMDalParameter sendPara = new CTMDalParameter();
            sendPara.BusinessType = BusinessType.UpdateCollectorMasterCommStatus;
            sendPara.BusinessObject = info;

            CTMDal aDal = new CTMDal();
            aDal.UpdateInfo(sendPara);
        }

        [TestMethod]
        public void Testupgrade()
        {
            CTMDalParameter sendPara = new CTMDalParameter();
            sendPara.BusinessType = BusinessType.GetNextUpgradeInfoByID;
            sendPara.TerminalID = "CD21064F-A3D9-4A74-A472-9E6E3A8E50C5";
            sendPara.BusinessObject = 1;

            CTMDal aDal = new CTMDal();
            CTMDalParameter para = aDal.GetInfo(sendPara);
            TPLUpgradeFileInfoDetail detail = para.BusinessObject as TPLUpgradeFileInfoDetail;
        }

        [TestMethod]
        public void TestAddMeter()
        {
            TPLCollectorAndMeter info = new TPLCollectorAndMeter();
            info.CollectorID = new Guid("51B5533E-D8F9-48BA-9C1D-AE0568C5F6B8");
            CTMDalParameter sendPara = new CTMDalParameter();
            sendPara.BusinessType = BusinessType.InsertIntoMeter;
            sendPara.BusinessObject = info;
            CTMDal aDal = new CTMDal();
            aDal.AddInfo(sendPara);
        }

        [TestMethod]
        public void TestAddPlatForm()
        {
            TPlatFormInfo newInfo = new TPlatFormInfo();
            newInfo.ObjID = new Guid("050ed87d-5128-4e19-8ceb-54b63bbe614d");
            newInfo.CountryCode = "86";
            newInfo.CityCode = "21";
            newInfo.PlatFormCode = "23532";
            newInfo.PlatFormName = "23248";

            CTMDalParameter sendPara = new CTMDalParameter();
            sendPara.BusinessType = BusinessType.AddNewPlatForm;
            sendPara.BusinessObject = newInfo;

            CTMDal aDal = new CTMDal();
            aDal.AddInfo(sendPara);
        }
    }
}
