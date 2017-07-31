using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;
using System.Threading.Tasks;
using System.Web.Http;

namespace FileDownloadServer.Controller
{
    [RoutePrefix("Api/DownLoad")]
    public class FileDownloadController:ApiController
    {
        [HttpGet]
        [Route("GetSVer_IntelligentSingleLamp")]
        public string GetSVer_IntelligentSingleLamp()
        {
            return AspectF.Define.Retry(Utility.CatchExpection)
                .Return<string>(() =>
                {
                    return Utility.Sver_IntelligentSingleLamp;
                });
        }

        [HttpGet]
        [Route("DownLoad_IntelligentSingleLamp")]
        public HttpResponseMessage IntelligentSingleLamp()
        {
            return AspectF.Define.Retry(Utility.CatchExpection)
                .Log(Utility.WriteLog, "开始下载智能单灯APP", "下载智能单灯APP成功！")
                .Return<HttpResponseMessage>(() =>
                {
                    string filePath = Utility.FilePath_IntelligentSingleLamp;
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
                });
        }

        [HttpGet]
        [Route("DownLoad_mvn_smartswitch")]
        public HttpResponseMessage Mvn_smartswitch()
        {
            return AspectF.Define.Retry(Utility.CatchExpection)
                .Log(Utility.WriteLog, "maven项目文件", "下载maven项目文件成功！")
                .Return<HttpResponseMessage>(() =>
                {
                    string filePath = Utility.FilePath_mvn_smartswitch;
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
                });
        }

        [HttpGet]
        [Route("DownLoad_CTM_App")]
        public HttpResponseMessage CTM_App()
        {
            return AspectF.Define.Retry(Utility.CatchExpection)
                .Log(Utility.WriteLog, "开始下载台区测试仪APP", "下载台区测试仪APP成功！")
                .Return<HttpResponseMessage>(() =>
                {
                    string filePath = Utility.FilePath_CTM_App;
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
                });
        }
    }
}
