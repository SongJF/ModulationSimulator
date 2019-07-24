using Arction.Wpf.SignalProcessing;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ChartCanvas.Utils
{
    public static class WaveGenerator
    {
        /// <summary>
        /// 生成正弦波序列
        /// </summary>
        /// <param name="BitNumber">序列比特数</param>
        /// <param name="Amplitude">振幅</param>
        /// <param name="Frequency">频率</param>
        public static double[] Sine(int BitNumber,int Amplitude,double sampleFrequency, double Frequency)
        {
            double[] data = new double[BitNumber];
            for(int i = 0; i < BitNumber; i++)
            {
                //data[i] = Amplitude * Math.Sin(i * Frequency / 22);
                double dt = 1.0 / sampleFrequency;
                double t = i * dt;
                double w = 2 * Math.PI * Frequency;
                data[i] = Amplitude * Math.Sin(w * t);
            }
            return data;
        }
    }
}
