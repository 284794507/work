using System;
using System.Collections.Generic;
using System.IO;
using System.IO.Pipes;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;

namespace CommunicateCore.Utility
{
    public class UtilityHelper
    {
        private static UtilityHelper _UtilityHelper;
        public static UtilityHelper GetHelper
        {
            get
            {
                if(_UtilityHelper==null)
                {
                    _UtilityHelper = new UtilityHelper();
                }
                return _UtilityHelper;
            }
        }

        public bool IsAllowed;

        public struct tcp_keepalive
        {
            public uint onoff;
            public uint keepalivetime;
            public uint keepaliveinterval;
        };

        /// &lt;summary&gt;  
        /// 结构体转byte数组  
        /// &lt;/summary&gt;  
        /// &lt;param name="structObj"&gt;要转换的结构体   &lt;/param&gt;  
        /// &lt;returns&gt;转换后的byte数组&lt;/returns&gt;  
        public byte[] StructToBytes(object structObj)
        {
            //得到结构体的大小  
            int size = Marshal.SizeOf(structObj);
            //创建byte数组  
            byte[] bytes = new byte[size];
            //分配结构体大小的内存空间  
            IntPtr structPtr = Marshal.AllocHGlobal(size);
            //将结构体拷到分配好的内存空间  
            Marshal.StructureToPtr(structObj, structPtr, false);
            //从内存空间拷到byte数组  
            Marshal.Copy(structPtr, bytes, 0, size);
            //释放内存空间  
            Marshal.FreeHGlobal(structPtr);
            //返回byte数组  
            return bytes;
        }

        //静态变量
        private static FileStream fileStream;
        private static StreamWriter streamWrite;

        public static void OpenLogFile()
        {
            string strFilePath = Path.Combine(Environment.CurrentDirectory, DateTime.Now.ToString("yyyy-MM-dd") + "Log.txt");
            OpenLogFile(strFilePath);
        }

        public static void OpenLogFile(string strFilePath)
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

        public static void CloseLogFile()
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

        public static void WriteLogFile(string content)
        {
            if (streamWrite != null)
            {
                streamWrite.WriteLine(content);
            }
        }

        public static bool CheckLogFile(string content, string strFilePath)
        {
            if (!File.Exists(strFilePath))
            {
                System.IO.TextWriter txt = File.CreateText(strFilePath);
                txt.Close();
            }
            StreamReader sr = new StreamReader(strFilePath, Encoding.Default);
            string line;
            bool result = true;
            while ((line = sr.ReadLine()) != null)
            {
                if (line.ToString().Contains(content))//if (content == line.ToString())
                {
                    result = false;
                }
            }
            sr.Close();
            return result;
        }

    }
}
