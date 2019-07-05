using DataManager.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DataManager.ToolBox.Transmitters
{
    public class Electrical
    {
        /// <summary>
        /// 生成正弦波
        /// </summary>
        /// <param name="Period">生成波的周期</param>
        /// <param name="Amplitude">生成波的幅值</param>
        /// <returns></returns>
        public static List<double> Sin(double Period = 1,double Amplitude = 1)
        {
            List<double> data = new List<double>();
            for (int i = 0; i < GlobalVariable.BitNumbers; i++)
            {
                data.Add(Amplitude * Math.Sin( i * Period / 4));
            }
            return data;
        }
    }
}
