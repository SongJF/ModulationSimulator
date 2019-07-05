using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DataManager.ToolBox.Modulator
{
    /// <summary>
    /// AM调制
    /// </summary>
    public static class AmplitudeModulator
    {
        public static List<double> Modulate(double A0, double Ka,List<double> sourceWave,List<double> carryWave)
        {
            List<double> data = new List<double>();
            for (int i=0;i<sourceWave.Count;i++)
            {
                data.Add((A0 + Ka * sourceWave[i]) * carryWave[i]);
            }
            return data;
        }
    }
}
