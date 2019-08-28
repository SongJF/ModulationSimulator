using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ChartCanvas.Utils
{
    public static class PCMCaculator
    {
        /// <summary>
        /// PCM编码
        /// </summary>
        /// <param name="data">PCM源数据</param>
        /// <returns>PCM编码</returns>
        public static int[] PCM_Encode(double data)
        {
            int value = (int)data;
            if (value >= 2048)
                value = 2047;
            //极性码
            int[] ans = new int[8];
            if (value > 0) ans[0] = 1;
            else ans[0] = 0;

            value = Math.Abs(value);

            //段落码

            //量化间隔
            int step;
            //起始电平
            int st;
            if (0 <= value && value < 16)
            {
                ans = setSectionCode(ans, 1, "000");
                step = 1;
                st = 0;
            }
            else if (16 <= value && value < 32)
            {
                ans = setSectionCode(ans, 1, "001");
                step = 1;
                st = 16;
            }
            else if (32 <= value && value < 64)
            {
                ans = setSectionCode(ans, 1, "010");
                step = 2;
                st = 32;
            }
            else if (64 <= value && value < 128)
            {
                ans = setSectionCode(ans, 1, "011");
                step = 4;
                st = 64;
            }
            else if (128 <= value && value < 256)
            {
                ans = setSectionCode(ans, 1, "100");
                step = 8;
                st = 128;
            }
            else if (256 <= value && value < 512)
            {
                ans = setSectionCode(ans, 1, "101");
                step = 16;
                st = 256;
            }
            else if (512 <= value && value < 1024)
            {
                ans = setSectionCode(ans, 1, "110");
                step = 32;
                st = 512;
            }
            else
            {
                ans = setSectionCode(ans, 1, "111");
                step = 64;
                st = 1024;
            }

            //段内码
            var insideCode = Convert.ToString(((int)Math.Floor((double)((value - st) / step))), 2);
            if (insideCode.Length < 4)
            {
                string tmp = "";
                for (int i = 0; i < 4 - insideCode.Length; i++)
                {
                    tmp += "0";
                }
                insideCode = tmp + insideCode;
            }
            ans = setSectionCode(ans, 4, insideCode);

            return ans;
        }

        /// <summary>
        /// 设置段落码和段内码
        /// </summary>
        private static int[] setSectionCode(int[] data, int index, string value)
        {
            try
            {
                foreach (var item in value)
                {
                    data[index] = int.Parse(item.ToString());
                    index++;
                }
            }
            catch(Exception ex)
            {
                Console.WriteLine(ex);
                return new int[8];
            }
            return data;
        }
    }
}
