using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CommunicateCore.Utility
{
    public class JsonSerializeHelper
    {
        private static JsonSerializeHelper _JsonSerializeHelper;
        public static JsonSerializeHelper GetHelper
        {
            get
            {
                if(_JsonSerializeHelper==null)
                {
                    _JsonSerializeHelper = new JsonSerializeHelper();
                }
                return _JsonSerializeHelper;
            }
        }

        public T Deserialize<T>( string jsonStr)
        {
            JsonSerializer serializer = new JsonSerializer();
            StringReader sr = new StringReader(jsonStr);
            object obj = serializer.Deserialize(new JsonTextReader(sr), typeof(T));
            T result = (T)obj;

            return result;
        }
    }
}
