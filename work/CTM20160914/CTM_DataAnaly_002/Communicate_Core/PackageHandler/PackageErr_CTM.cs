﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Communicate_Core.PackageHandler
{
    public enum PackageErr_CTM
    {
        /// <summary>
        /// 初始化为正常
        /// </summary>
        Normal,

        /// <summary>
        /// 同步字错误
        /// </summary>
        SynWordError,

        /// <summary>
        /// 同步字匹配错误
        /// </summary>
        SynWordMatchError,

        /// <summary>
        /// 长度域错误
        /// </summary>
        DataLengthError,

        /// <summary>
        /// 包定义的长度和实际收到长度不符合错误
        /// </summary>
        PackageLengthError,

        /// <summary>
        /// CRC错误
        /// </summary>
        CRCError,

        /// <summary>
        /// CTU通讯地址错误
        /// </summary>
        CTUADRRError,

        /// <summary>
        /// 包尾错误
        /// </summary>
        EndTagError,

        /// <summary>
        /// 分析包时未知错误
        /// </summary>
        ExceptionError
    }
}
