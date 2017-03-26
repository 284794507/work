using System;
using System.Collections.Generic;
using System.Configuration;
using System.IO;
using System.Linq;
using System.Logger;
using System.Text;
using System.Threading.Tasks;

namespace KLJCServer.Utility
{
    public class Share
    {
        private static Share _Share;
        public static Share Instance
        {
            get
            {
                if(_Share==null)
                {
                    _Share = new Share();
                }
                return _Share;
            }
        }

        public static string sVer;
        public static string FilePath;

        public void InitConfig()
        {
            string val = "";

            val = ConfigurationManager.AppSettings["SVer"];
            sVer = val;

            val = ConfigurationManager.AppSettings["FilePath"];
            FilePath = val;
        }

        SimpleLogger SimpleLog = SimpleLogger.GetInstance();

        public void WriteLog(string msg, int lvl = 2)
        {
            switch (lvl)
            {
                case 1:
                    SimpleLog.Debug(msg);
                    break;
                case 2:
                    SimpleLog.Info(msg);
                    break;
                case 3:
                    SimpleLog.Warn(msg);
                    break;
                case 4:
                    SimpleLog.Error(msg);
                    break;
                case 5:
                    SimpleLog.Fatal(msg);
                    break;
            }
            Console.WriteLine(DateTime.Now.ToString() + " : " + msg);
        }

        public void CatchExpection(Exception ex)
        {
            WriteLog(ex.Message, 4);
        }

        //静态变量
        private static FileStream fileStream;
        private static StreamWriter streamWrite;

        public static void OpenLampFile()
        {
            //string strFilePath = Path.Combine(Environment.CurrentDirectory, "Lamp/Lamp" + DateTime.Now.ToString("yyyy-MM-dd") + "Log.txt");
            string path = GetPath(Path.Combine(Environment.CurrentDirectory, "Lamp/Data/" + DateTime.Now.ToString("yyyy-MM-dd") + "/"));
            string strFilePath = Path.Combine(path,"Lamp" + DateTime.Now.ToString("yyyy-MM-dd") + ".txt");
            OpenTxtFile(strFilePath);
        }

        public static void OpenCabFile()
        {
            //string strFilePath = Path.Combine(Environment.CurrentDirectory, "Cab/Cab" + DateTime.Now.ToString("yyyy-MM-dd") + "Log.txt");
            string path = GetPath(Path.Combine(Environment.CurrentDirectory, "Cab/Data/" + DateTime.Now.ToString("yyyy-MM-dd") + "/"));
            string strFilePath = Path.Combine(path, "Cab" + DateTime.Now.ToString("yyyy-MM-dd") + ".txt");
            OpenTxtFile(strFilePath);
        }

        public static void OpenTxtFile()
        {
            string strFilePath = Path.Combine(Environment.CurrentDirectory, DateTime.Now.ToString("yyyy-MM-dd") + "Log.txt");
            OpenTxtFile(strFilePath);
        }

        public static void OpenTxtFile(string strFilePath)
        {
            if (fileStream == null || streamWrite == null)
            {
                if (!File.Exists(strFilePath))
                {
                    System.IO.TextWriter txt = File.CreateText(strFilePath);
                    txt.Close();
                }
                fileStream = new System.IO.FileStream(strFilePath, System.IO.FileMode.Append, FileAccess.Write, FileShare.ReadWrite);
                streamWrite = new System.IO.StreamWriter(fileStream, System.Text.Encoding.Default);
            }
        }

        public static void WriteTxtFile(string content)
        {
            if (streamWrite != null)
            {
                streamWrite.WriteLine(content);
            }
        }

        public static void CloseTxtFile()
        {
            AspectF.Define.Retry()
                .Do(() =>
                {
                    streamWrite.Close();
                    fileStream.Close();
                    streamWrite = null;
                    streamWrite = null;
                });
        }

        /// <summary>
        /// 获取相应的文件夹，如果不存在，则新建
        /// </summary>
        /// <param name="path"></param>
        /// <returns></returns>
        public static string GetPath(string path)
        {
            string tPath = path;
            if (!Directory.Exists(tPath))
            {
                Directory.CreateDirectory(tPath);
            }
            return tPath;
        }
    }
}
