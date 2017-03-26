using Communicate_Core.Utility;
using CTMDAL.Model;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using System.Web.Http;
using System.Runtime.Serialization.Json;
using Newtonsoft.Json;

namespace CTM_DataAnaly.Controller
{
    public class DevBasicInfo
    {
        public string SNCode { get; set; }
        public string MacAddr { get; set; }
    }

    public class SupplyInfo
    {
        public string UserAddress { get; set; }
        public string PlatFormCode { get; set; }
        public string MeterCode { get; set; }
        public string SupplyCode { get; set; }
    }

    public class DevInfo
    {
        public string SNCode { get; set; }
        public string MacAddr { get; set; }
        public string Lon { get; set; }
        public string Lat { get; set; }
        public string Address { get; set; }
        public string PicAddr { get; set; }

    }

    public class MeterInfo
    {
        public string SNCode { get; set; }
        public string MacAddr { get; set; }
        public string MeterSNCode { get; set; }
        public string SupplyCode { get; set; }
    }

    public class DevAndMeterInfo
    {
        public string SNCode { get; set; }
        public string MacAddr { get; set; }
        public string Lon { get; set; }
        public string Lat { get; set; }
        public string Address { get; set; }
        public string PicAddr { get; set; }
        public string MeterSNCode { get; set; }
        public string SupplyCode { get; set; }
    }

    [RoutePrefix("api/CTMSvr")]
    public class AppSvrController : ApiController
    {
        [HttpGet]
        [Route("DownloadDevBasic")]
        public HttpResponseMessage DownloadDevBasicInfo()
        {//上传设备信息
            try
            {
                List<DevBasicInfo> listInfo = new List<DevBasicInfo>();
                foreach (TPLCollectorInfo item in Share.Instance.listCollector)
                {
                    DevBasicInfo info = new DevBasicInfo();
                    info.SNCode = item.SNCode;
                    info.MacAddr = item.MacAddr;
                    listInfo.Add(info);
                }
                DataContractJsonSerializer jsonSerializer = new DataContractJsonSerializer(listInfo.GetType());
                string val = "";
                using (MemoryStream ms = new MemoryStream())
                {
                    jsonSerializer.WriteObject(ms, listInfo);
                    val = Encoding.UTF8.GetString(ms.ToArray());
                }
                HttpResponseMessage result = new HttpResponseMessage { Content = new StringContent(val, Encoding.GetEncoding("UTF-8"), "application/json") };
                return result;
            }
            catch (Exception ex)
            {
                Share.Instance.WriteLog(ex.Message);
                return Request.CreateResponse(System.Net.HttpStatusCode.NotAcceptable);
            }
        }

        //[HttpGet]
        //[Route("DownloadDevBasic")]
        //public string DownloadDevBasicInfo()
        //{//上传设备信息
        //    try
        //    {
        //        List<DevBasicInfo> listInfo = new List<DevBasicInfo>();
        //        foreach (TPLCollectorInfo item in Share.Instance.listCollector)
        //        {
        //            DevBasicInfo info = new DevBasicInfo();
        //            info.SNCode = item.SNCode;
        //            info.MacAddr = item.MacAddr;
        //            listInfo.Add(info);
        //        }
        //        DataContractJsonSerializer jsonSerializer = new DataContractJsonSerializer(listInfo.GetType());
        //        string val = "";
        //        using (MemoryStream ms = new MemoryStream())
        //        {
        //            jsonSerializer.WriteObject(ms, listInfo);
        //            val = Encoding.UTF8.GetString(ms.ToArray());
        //        }
        //        HttpResponseMessage result = new HttpResponseMessage { Content = new StringContent(val, Encoding.GetEncoding("UTF-8"), "application/json") };
        //        return val;
        //    }
        //    catch (Exception ex)
        //    {
        //        Share.Instance.WriteLog(ex.Message);
        //        return "";
        //    }
        //}

        [HttpGet]
        [Route("DownloadSupplyCode")]
        public HttpResponseMessage DownloadSupplyCodeAndMeter()
        {//下载电表信息
            try
            {
                List<SupplyInfo> listInfo = new List<SupplyInfo>();
                foreach (TPLBasicInfo item in Share.Instance.listSupply)
                {
                    SupplyInfo info = new SupplyInfo();
                    string code=item.PlatFormCode;
                    info.PlatFormCode = code;
                    int no = code.IndexOf('-');
                    if(no>0)
                    {
                        info.PlatFormCode = code.Substring(0, no);
                    }
                    info.MeterCode = item.MeterName;
                    info.SupplyCode = item.SupplyCode;
                    info.UserAddress = item.UserAddress;

                    listInfo.Add(info);
                }
                DataContractJsonSerializer jsonSerializer = new DataContractJsonSerializer(listInfo.GetType());
                string val = "";
                using (MemoryStream ms = new MemoryStream())
                {
                    jsonSerializer.WriteObject(ms, listInfo);
                    val = Encoding.UTF8.GetString(ms.ToArray());
                }
                HttpResponseMessage result = new HttpResponseMessage { Content = new StringContent(val, Encoding.GetEncoding("UTF-8"), "application/json") };
                return result;
            }
            catch (Exception ex)
            {
                Share.Instance.WriteLog(ex.Message);
                return Request.CreateResponse(System.Net.HttpStatusCode.NotAcceptable);
            }
        }

        [HttpPost]
        [Route("UploadDev")]
        public async Task<HttpResponseMessage> UploadDevInfo()
        {//上传设备信息
            try
            {
                string value = await Request.Content.ReadAsStringAsync();
                Share.Instance.WriteLog(value);
                //using (var ms = new MemoryStream(Encoding.Unicode.GetBytes(value)))
                //{
                //    DataContractJsonSerializer deseralizer = new DataContractJsonSerializer(typeof(DevInfo));
                //    DevInfo devInfo = deseralizer.ReadObject(ms) as DevInfo;
                //}
                //string realVal = System.Web.HttpUtility.UrlDecode(value);
                List<DevInfo> lstInfo = JsonConvert.DeserializeObject<List<DevInfo>>(value);
                foreach (DevInfo item in lstInfo)
                {
                    //SNCode,MacAddr,Lon,Lat,Address,PicAddr
                    TPLCollectorInfo info = new TPLCollectorInfo();
                    info.ObjID=Guid.NewGuid();
                    info.MacAddr = item.MacAddr;
                    info.Address = item.Address;
                    info.Lon = decimal.Parse(item.Lon);
                    info.Lat = decimal.Parse(item.Lat);
                    info.SNCode = item.SNCode;
                    //Share.Instance.GetDevType(devType);

                    DBHandler.Instance.InsertCollectorInfo(info);
                }
            }
            catch(Exception ex)
            {
                Share.Instance.WriteLog(ex.Message);
            }
            return Request.CreateResponse(System.Net.HttpStatusCode.OK);
        }

        [HttpPost]
        [Route("UploadMeterBinding")]
        public async Task<HttpResponseMessage> UploadMeterBindingInfo()
        {//上传电表绑定进户点
            //SNCode,MacAddr,MeterSNCode,SupplyCode
            string result = "";//返回进户点号
            try
            {
                string value = await Request.Content.ReadAsStringAsync();
                Share.Instance.WriteLog(value);
                using (var ms = new MemoryStream(Encoding.Unicode.GetBytes(value)))
                {
                    DataContractJsonSerializer deseralizer = new DataContractJsonSerializer(typeof(MeterInfo));
                    MeterInfo devInfo = deseralizer.ReadObject(ms) as MeterInfo;
                    TPLCollectorAndMeter meter = new TPLCollectorAndMeter();
                    meter.CollectorID = new Guid(Share.Instance.GetDevIDByAddr(devInfo.MacAddr));
                    meter.CollectorCode = devInfo.SNCode;
                    foreach(TPLBasicInfo item in Share.Instance.listSupply)
                    {
                        if(devInfo.MeterSNCode==item.MeterName)
                        {
                            meter.MeterCode = devInfo.MeterSNCode;
                            meter.SupplyCode = item.SupplyCode;
                            meter.UserName = item.UserName;
                            meter.UserAddress = item.UserAddress;
                            break;
                        }
                    }

                    DBHandler.Instance.BindSupplyCode(meter);
                }
            }
            catch (Exception ex)
            {
                Share.Instance.WriteLog(ex.Message);
            }
            return Request.CreateResponse<string>(System.Net.HttpStatusCode.OK,result);
        }

        [HttpPost]
        [Route("UploadDevAndMeter")]
        public async Task<HttpResponseMessage> UploadDevAndMeterBinging()
        {//上传设备信息与绑定的进户点、电表信息
            try
            {
                string value = await Request.Content.ReadAsStringAsync();
                Share.Instance.WriteLog(value);
                List<DevAndMeterInfo> lstInfo = JsonConvert.DeserializeObject<List<DevAndMeterInfo>>(value);
                foreach (DevAndMeterInfo item in lstInfo)
                {
                    //SNCode,MacAddr,Lon,Lat,Address,PicAddr,MeterSNCode,SupplyCode
                    TPLCollectorInfo info = new TPLCollectorInfo();
                    info.ObjID = Guid.NewGuid();
                    info.MacAddr = item.MacAddr;
                    info.Address = item.Address;
                    info.Lon = decimal.Parse(item.Lon);
                    info.Lat = decimal.Parse(item.Lat);
                    info.SNCode = item.SNCode;

                    DBHandler.Instance.InsertCollectorInfo(info);

                    TPLCollectorAndMeter meter = new TPLCollectorAndMeter();
                    meter.CollectorID = new Guid(Share.Instance.GetDevIDByAddr(item.MacAddr));
                    meter.CollectorCode = item.SNCode;
                    meter.MeterCode = item.MeterSNCode;
                    foreach (TPLBasicInfo basicInfo in Share.Instance.listSupply)
                    {
                        if (item.MeterSNCode == basicInfo.MeterName)
                        {
                            meter.SupplyCode = basicInfo.SupplyCode;
                            meter.UserName = basicInfo.UserName;
                            meter.UserAddress = basicInfo.UserAddress;
                            break;
                        }
                    }

                    DBHandler.Instance.BindSupplyCode(meter);
                }
            }
            catch (Exception ex)
            {
                Share.Instance.WriteLog(ex.Message);
            }
            return Request.CreateResponse(System.Net.HttpStatusCode.OK);
        }

        [HttpPost]
        [Route("UploadMeterInfo")]
        public async Task<HttpResponseMessage> UploadAllMeterInfo()
        {//上传电表信息
            try
            {
                string value = await Request.Content.ReadAsStringAsync();
                Share.Instance.WriteLog(value);
                using (var ms = new MemoryStream(Encoding.Unicode.GetBytes(value)))
                {
                    DataContractJsonSerializer deseralizer = new DataContractJsonSerializer(typeof(MeterInfo));
                    MeterInfo devInfo = deseralizer.ReadObject(ms) as MeterInfo;

                    TPLCollectorAndMeter meter = new TPLCollectorAndMeter();
                    meter.CollectorID = new Guid(Share.Instance.GetDevIDByAddr(devInfo.MacAddr));
                    meter.CollectorCode = devInfo.SNCode;
                    foreach (TPLBasicInfo item in Share.Instance.listSupply)
                    {
                        if (devInfo.MeterSNCode == item.MeterName)
                        {
                            meter.SupplyCode = item.SupplyCode;
                            meter.UserName = item.UserName;
                            meter.UserAddress = item.UserAddress;
                        }
                    }

                    DBHandler.Instance.InsertIntoMeter(meter);
                }
            }
            catch (Exception ex)
            {
                Share.Instance.WriteLog(ex.Message);
            }
            //SNCode,MacAddr,MeterSNCode
            return Request.CreateResponse(System.Net.HttpStatusCode.OK);
        }

        [HttpPost]
        [Route("UploadDevPic")]
        public async Task<bool> UploadDevPic()
        {//上传设备图片
            bool result = true;
            try
            {
                if (Request.Content.IsMimeMultipartContent())
                {
                    string path = Share.GetPath(Path.Combine(Environment.CurrentDirectory, "DevPic/", DateTime.Now.ToString("yyyy-MM-dd")));
                    var provider = new MultipartFormDataStreamProvider(path);
                    try
                    {
                        await Request.Content.ReadAsMultipartAsync(provider);
                        foreach (MultipartFileData file in provider.FileData)
                        {
                            string oldName = file.LocalFileName;
                            string fileName = file.Headers.ContentDisposition.FileName;
                            fileName = fileName.Replace("\"", "");
                            string newName = Path.Combine(path, fileName);// path + fileName;
                            Share.Instance.WriteLog(oldName);//获取上传文件实际的文件名  
                            Share.Instance.WriteLog(newName);//获取上传文件在服务上默认的文件名
                            try
                            {
                                System.IO.File.Move(oldName, newName);
                            }
                            catch { }
                        }
                        foreach (var key in provider.FormData.AllKeys)
                        {//接收FormData  
                            string val = key + ":" + provider.FormData[key];
                            Share.Instance.WriteLog(val);
                        }
                    }
                    catch (Exception ex)
                    {
                        Share.Instance.WriteLog(ex.Message);
                    }
                }
            }
            catch (Exception ex)
            {
                Share.Instance.WriteLog(ex.Message);
            }
            return result;
        }

        [HttpPost]
        [Route("UploadMeterPic")]
        public async Task<bool> UploadMeterPic()
        {//上传电表图片
            bool result = true;
            try
            {
                if (Request.Content.IsMimeMultipartContent())
                {
                    string path = Share.GetPath(Path.Combine(Environment.CurrentDirectory, "MeterPic/", DateTime.Now.ToString("yyyy-MM-dd")));
                    var provider = new MultipartFormDataStreamProvider(path);
                    try
                    {
                        await Request.Content.ReadAsMultipartAsync(provider);
                        foreach (MultipartFileData file in provider.FileData)
                        {
                            string oldName = file.LocalFileName;
                            string fileName = file.Headers.ContentDisposition.FileName;
                            fileName = fileName.Replace("\"", "");
                            string newName = Path.Combine(path, fileName);// path + fileName;
                            Share.Instance.WriteLog(oldName);//获取上传文件实际的文件名  
                            Share.Instance.WriteLog(newName);//获取上传文件在服务上默认的文件名
                            try
                            {
                                System.IO.File.Move(oldName, newName);
                            }
                            catch { }
                        }
                        foreach (var key in provider.FormData.AllKeys)
                        {//接收FormData  
                            string val = key + ":" + provider.FormData[key];
                            Share.Instance.WriteLog(val);
                        }
                    }
                    catch (Exception ex)
                    {
                        Share.Instance.WriteLog(ex.Message);
                    }
                }
            }
            catch (Exception ex)
            {
                Share.Instance.WriteLog(ex.Message);
            }
            return result;
        }
    }
}
