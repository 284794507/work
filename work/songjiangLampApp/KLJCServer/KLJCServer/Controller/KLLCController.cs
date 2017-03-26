using KLJCServer.Utility;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;
using System.Threading.Tasks;
using System.Web.Http;

namespace KLJCServer.Controller
{
    [RoutePrefix("api/Upgrade")]//[RoutePrefix("api/LampCensus")]//
    public class KLLCController : ApiController
    {
        public object FileStream { get; private set; }

        [HttpGet]
        [Route("GetSVer")]
        public string GetSVer()
        {
            return Share.sVer;
        }

        [HttpGet]
        [Route("DownLoadFile")]
        public HttpResponseMessage DownLoadUpgradeFile()
        {
            string filePath = Share.FilePath;
            int no = filePath.LastIndexOf(@"\");
            string fileName = filePath.Substring(no + 1);
            FileStream fs = new FileStream(filePath, FileMode.Open, FileAccess.Read, FileShare.Read);
            HttpResponseMessage response = new HttpResponseMessage();
            response.Content = new StreamContent(fs);
            response.Content.Headers.ContentDisposition = new ContentDispositionHeaderValue("attachment");
            response.Content.Headers.ContentDisposition.FileName = fileName;
            response.Content.Headers.ContentType = new MediaTypeHeaderValue("application/octet-stream");
            response.Content.Headers.ContentLength = new FileInfo(filePath).Length;
            return response;
        }

        [HttpPost]
        [Route("UploadLampInfo")]
        public async Task<bool> UploadLampData()
        {
            bool result = true;
            if (Request.Content.IsMimeMultipartContent())
            {
                string path = Share.GetPath(Path.Combine(Environment.CurrentDirectory, "Lamp/Pic/", DateTime.Now.ToString("yyyy-MM-dd")));
                var provider = new MultipartFormDataStreamProvider(path);
                try
                {
                    await Request.Content.ReadAsMultipartAsync(provider);
                    foreach (MultipartFileData file in provider.FileData)
                    {
                        string oldName = file.LocalFileName;
                        string fileName = file.Headers.ContentDisposition.FileName;
                        fileName = fileName.Replace("\"", "");
                        string newName = Path.Combine(path, fileName);//path + fileName;
                        Console.WriteLine(oldName);//获取上传文件实际的文件名  
                        Console.WriteLine(newName);//获取上传文件在服务上默认的文件名
                        //System.IO.File.Move(oldName, newName);
                        try
                        {
                            System.IO.File.Move(oldName, newName);
                        }
                        catch{}
                    }
                    foreach (var key in provider.FormData.AllKeys)
                    {//接收FormData  
                        string val = key + ":" + provider.FormData[key];
                        Console.WriteLine(val);
                        Share.OpenLampFile();
                        Share.WriteTxtFile(val);
                        Share.CloseTxtFile();
                    }
                }
                catch (Exception ex)
                {
                    Console.WriteLine(ex.Message);
                }
            }
            return result;
        }

        [HttpPost]
        [Route("UploadCabInfo")]
        public async Task<bool> UploadCabData()
        {
            bool result = true;
            if (Request.Content.IsMimeMultipartContent())
            {
                string path = Share.GetPath(Path.Combine(Environment.CurrentDirectory, "Cab/Pic/", DateTime.Now.ToString("yyyy-MM-dd")));
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
                        Console.WriteLine(oldName);//获取上传文件实际的文件名  
                        Console.WriteLine(newName);//获取上传文件在服务上默认的文件名
                        try
                        {
                            System.IO.File.Move(oldName, newName);
                        }
                        catch{}
                    }
                    foreach (var key in provider.FormData.AllKeys)
                    {//接收FormData  
                        string val = key + ":" + provider.FormData[key];
                        Console.WriteLine(val);
                        Share.OpenCabFile();
                        Share.WriteTxtFile(val);
                        Share.CloseTxtFile();
                    }
                }
                catch (Exception ex)
                {
                    Console.WriteLine(ex.InnerException.Message);
                    Console.WriteLine(ex.Message);
                }
            }
            return result;
        }

        [HttpPost]
        [Route("UploadLampJson")]
        public async Task<HttpResponseMessage> UploadLamp()//public HttpResponseMessage UploadLamp(string value)//
        {
            //bool result = true;
            Share.OpenLampFile();
            string value = await Request.Content.ReadAsStringAsync();
            string realVal = System.Web.HttpUtility.UrlDecode(value);
            Console.WriteLine(realVal);
            Share.WriteTxtFile(realVal);
            Share.CloseTxtFile();
            return Request.CreateResponse();
            //return result;
        }

        [HttpPost]
        [Route("UploadCabJson")]
        public async Task<HttpResponseMessage> UploadCab()//public bool UploadCab(string value)
        {
            //bool result = true;
            Share.OpenCabFile();
            string value = await Request.Content.ReadAsStringAsync();
            string realVal = System.Web.HttpUtility.UrlDecode(value);
            Console.WriteLine(realVal);
            Share.WriteTxtFile(realVal);
            Share.CloseTxtFile();
            return Request.CreateResponse();
            //return result;
        }

        //[HttpPost]
        //[Route("UploadImage")]
        //public async Task<bool> UploadPic()
        //{
        //    bool result = true;
        //    if (Request.Content.IsMimeMultipartContent())
        //    {
        //        var provider = new MultipartFormDataStreamProvider(@"F:\upload");
        //        try
        //        {
        //            await Request.Content.ReadAsMultipartAsync(provider);
        //            foreach (MultipartFileData file in provider.FileData)
        //            {
        //                //Console.WriteLine(file.Headers.ContentDisposition.FileName);//获取上传文件实际的文件名  
        //                //Console.WriteLine("Server file path: " + file.LocalFileName);//获取上传文件在服务上默认的文件名  
        //                string oldName = file.LocalFileName;
        //                string fileName = file.Headers.ContentDisposition.FileName;
        //                fileName = fileName.Replace("\"", "");
        //                string newName = @"F:\upload\" + fileName;
        //                //newName = newName.Trim('"');
        //                Console.WriteLine(oldName);//获取上传文件实际的文件名  
        //                Console.WriteLine(newName);//获取上传文件在服务上默认的文件名
        //                try
        //                {
        //                    System.IO.File.Move(oldName, newName);
        //                }
        //                catch
        //                {

        //                }

        //                Share.OpenCabFile();
        //                Share.WriteTxtFile(oldName);
        //                Share.WriteTxtFile(newName);
        //                Share.CloseTxtFile();
        //            }
        //            foreach (var key in provider.FormData.AllKeys)
        //            {//接收FormData  
        //                Console.WriteLine(key);
        //                Console.WriteLine(provider.FormData[key]);
        //                //dic.Add(key, provider.FormData[key]);
        //            }
        //        }
        //        catch (Exception ex)
        //        {
        //            Console.WriteLine(ex.Message);
        //        }
        //    }
        //    return result;
        //}

        //public bool UploadPic()
        //{
        //    return AspectF.Define.Retry(Share.Instance.CatchExpection)
        //        .Return<bool>(() =>
        //        {
        //            bool result = true;
        //            if (!Request.Content.IsMimeMultipartContent())
        //            {
        //                Share.Instance.WriteLog("Invalid Request");
        //                result = false;
        //            }
        //            var provider = new MultipartFormDataStreamProvider(@"F:\upload");

        //            //foreach (MultipartFileData file in provider.FileData)
        //            //{//接收文件  
        //            //    Console.WriteLine(file.Headers.ContentDisposition.FileName);//获取上传文件实际的文件名  
        //            //    Console.WriteLine("Server file path: " + file.LocalFileName);//获取上传文件在服务上默认的文件名  
        //            //}//TODO:这样做直接就将文件存到了指定目录下，暂时不知道如何实现只接收文件数据流但并不保存至服务器的目录下，由开发自行指定如何存储，比如通过服务存到图片服务器  
        //            //foreach (var key in provider.FormData.AllKeys)
        //            //{//接收FormData  
        //            //    Console.WriteLine(key);
        //            //    Console.WriteLine(provider.FormData[key]);
        //            //    //dic.Add(key, provider.FormData[key]);
        //            //}
        //            string oldName = "";
        //            string newName = "";
        //            Dictionary<string, string> dictFile = new Dictionary<string, string>();
        //            //IEnumerable<HttpContent> parts = null;
        //            //Task.Factory.StartNew(() =>
        //            //{
        //            //    parts = Request.Content.ReadAsMultipartAsync(provider).Result.Contents;
        //            //    foreach(HttpContent val in parts)
        //            //    {
        //            //        if(val.IsFormData())
        //            //        {
        //            //            MultipartFileData file = val as MultipartFileData;
        //            //            oldName = @"F:\upload\" + file.LocalFileName;
        //            //            newName = @"F:\upload\" + file.Headers.ContentDisposition.FileName;
        //            //        }
        //            //        else
        //            //        {

        //            //        }
        //            //        Console.WriteLine(val.Headers.ContentDisposition.FileName);//获取上传文件实际的文件名  
        //            //    }

        //            //},CancellationToken.None,TaskCreationOptions.LongRunning,TaskScheduler.Default).Wait();
        //            Request.Content.ReadAsMultipartAsync(provider).ContinueWith(p =>
        //            {
        //                //MultipartFileData file = provider.FileData[0];
        //                //string name = file.Headers.ContentDisposition.FileName;
        //                //var content = p.Result.Contents;
        //                //foreach(var item in content)
        //                //{
        //                //    if(string.IsNullOrEmpty(item.Headers.ContentDisposition.FileName))
        //                //    {
        //                //        continue;
        //                //    }
        //                //    item.ReadAsStreamAsync().ContinueWith(a =>
        //                //    {

        //                //        newName = @"F:\upload\" + item.Headers.ContentDisposition.FileName;
        //                //        // 把 Stream 转换成 byte[]
        //                //        Stream s = a.Result;
        //                //        byte[] bytes = new byte[s.Length];
        //                //        s.Read(bytes, 0, bytes.Length);
        //                //        // 设置当前流的位置为流的开始
        //                //        s.Seek(0, SeekOrigin.Begin);
        //                //        // 把 byte[] 写入文件
        //                //        FileStream fs = new FileStream(newName, FileMode.Create);
        //                //        BinaryWriter bw = new BinaryWriter(fs);
        //                //        bw.Write(bytes);
        //                //        bw.Close();
        //                //        fs.Close();
        //                //    });
        //                //}
        //                foreach (MultipartFileData file in provider.FileData)
        //                {//接收文件  
        //                    Console.WriteLine(file.Headers.ContentDisposition.FileName);//获取上传文件实际的文件名  
        //                    Console.WriteLine("Server file path: " + file.LocalFileName);//获取上传文件在服务上默认的文件名  
        //                    oldName = @"F:\upload\" + file.LocalFileName;
        //                    newName = @"F:\upload\" + file.Headers.ContentDisposition.FileName;
        //                    //dictFile.Add(oldName, newName);
        //                    Share.OpenCabFile();
        //                    Share.WriteTxtFile(oldName);
        //                    Share.WriteTxtFile(newName);
        //                    Share.CloseTxtFile();
        //                    //FileInfo curFile = new FileInfo(oldName);
        //                    //curFile.MoveTo(newName);                            

        //                }//TODO:这样做直接就将文件存到了指定目录下，暂时不知道如何实现只接收文件数据流但并不保存至服务器的目录下，由开发自行指定如何存储，比如通过服务存到图片服务器  
        //                System.IO.File.Move(oldName, newName);
        //                foreach (var key in provider.FormData.AllKeys)
        //                {//接收FormData  
        //                    Console.WriteLine(key);
        //                    Console.WriteLine(provider.FormData[key]);
        //                    //dic.Add(key, provider.FormData[key]);
        //                }

        //            });
        //            //System.IO.File.Move(oldName, newName);
        //            //string strFilePath = Path.Combine(Environment.CurrentDirectory, "Lamp123" + DateTime.Now.ToString("yyyy-MM-dd") + "Log.txt");
        //            //string abtPath = HttpContext.Current.Server.MapPath(strFilePath);
        //            //string val = HttpContext.Current.Request.ToString();

        //            return result;
        //        });
        //}
    }
}
