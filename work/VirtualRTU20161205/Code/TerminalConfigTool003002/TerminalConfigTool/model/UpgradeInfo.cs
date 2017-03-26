using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Utility;

namespace TerminalConfigTool.model
{
    public class UpgradeInfo
    {
        public byte UpgradeStatus { get; set; }

        #region 升级请求
        public byte[] FileName { get; set; }

        public byte[] FileLength { get; set; }

        /// <summary>
        /// 升级文件的总校验
        /// </summary>
        public byte[] FileCRC { get; set; }
        #endregion

        #region 升级请中
        /// <summary>
        /// 升级文件内容
        /// </summary>
        public byte[] FileData { get; set; }
        #endregion

        /// <summary>
        /// 总包数
        /// </summary>
        public byte TotalNum { get; set; }

        /// <summary>
        /// 当前
        /// </summary>
        public byte SendNo { get; set; }

        /// <summary>
        /// 升级阶段
        /// </summary>
        public byte Stage { get; set; }

        /// <summary>
        /// 上次发送升级报文时间
        /// </summary>
        public DateTime sendTime { get; set; }

        /// <summary>
        /// 同一报文重发次数
        /// </summary>
        public int ResendNo { get; set; }

        public bool IsOK { get; set; }

        public UpgradeInfo()
        {
            this.SendNo = 1;
            this.ResendNo = 0;
            this.Stage = 1;
            this.IsOK = false;
        }

        public byte[] ToBytes()
        {
            byte[] result = new byte[0];
            AspectF.Define.Retry()
                .Do(() =>
                {
                    byte[] fileSize = new byte[0];
                    int curIndex = 0;

                    switch (this.UpgradeStatus)
                    {
                        case 1://请求升级
                            int NLen = this.FileName.Length;
                            result = new byte[1 + 1 + 1 + NLen + 3 + 2];
                            result[curIndex++] = 0;//SEQ
                            result[curIndex++] = this.UpgradeStatus;
                            result[curIndex++] = (byte)NLen;
                            Buffer.BlockCopy(this.FileName, 0, result, curIndex, NLen);
                            curIndex += NLen;
                            //fileSize = BitConverter.GetBytes((int)this.FileData.Length);
                            Buffer.BlockCopy(this.FileLength, 0, result, curIndex, 3);
                            curIndex += 3;
                            //byte[] crc = BitConverter.GetBytes(ByteHelper.GetCrc16(this.FileData));
                            Buffer.BlockCopy(this.FileCRC, 0, result, curIndex, 2);
                            break;
                        case 2://升级中,如果总包数大于1，则要分包
                            int FLen = this.FileData.Length;
                            result = new byte[1 + 2 + FLen];
                            if (this.TotalNum > 1)
                            {
                                //result[curIndex++] = 0;//SEQ
                                result = new byte[3 + 1 + 2 + FLen];
                                result[curIndex++] = 2;
                                result[curIndex++] = this.TotalNum;
                                result[curIndex++] = this.SendNo;
                            }
                            result[curIndex++] = this.UpgradeStatus;
                            fileSize = BitConverter.GetBytes((ushort)FLen);
                            result[curIndex++] = fileSize[1];
                            result[curIndex++] = fileSize[0];
                            //Buffer.BlockCopy(fileSize, 0, result, curIndex, 2);
                            //curIndex += 2;
                            Buffer.BlockCopy(this.FileData, 0, result, curIndex, FLen);
                            break;
                        case 3://升级结束
                            result = new byte[2];
                            result[curIndex++] = 0;//SEQ
                            result[curIndex++] = this.UpgradeStatus;
                            break;
                    }
                });

            return result;
        }
    }
}
