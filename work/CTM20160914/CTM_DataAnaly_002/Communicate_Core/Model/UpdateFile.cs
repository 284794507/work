using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Communicate_Core.Model
{
    public class UpdateFile
    {
        public string Addr { get; set; }

        public string DevID { get; set; }

        public string FileName { get; set; }

        public byte[] FileData { get; set; }

        public int FileSize { get; set; }

        public string FileSVer { get; set; }

        public string FileHVer { get; set; }

        public DateTime UploadTime { get; set; }
    }
}
