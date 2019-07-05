using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DataManager.Model
{
    public class GlobalVariable
    {
        public static void SetValue(int bitNums = 256, int fps = 1)
        {
            BitNumbers = bitNums;
            FPS = fps;
        }

        /// <summary>
        /// 程序存储的最大比特数
        /// </summary>
        public static int BitNumbers { get; set; }

       /// <summary>
       /// 重绘的帧速率
       /// </summary>
        public static int FPS { get; set; }
    }
}
