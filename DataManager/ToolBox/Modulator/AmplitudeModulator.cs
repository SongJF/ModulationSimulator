using DataManager.Model;
using MathNet.Numerics;
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
        /// <summary>
        /// 调制
        /// </summary>
        /// <param name="A0">上调电压</param>
        /// <param name="Ka">调制度</param>
        /// <param name="sourceWave">信号波信号</param>
        /// <param name="carryWave">载波信号</param>
        /// <returns>调制后的信号</returns>
        public static List<double> Modulate(double A0, double Ka, List<double> sourceWave, List<double> carryWave)
        {
            //检空
            if (sourceWave == null || carryWave == null) return null;
            List<double> data = new List<double>();
            for (int i=0;i<sourceWave.Count;i++)
            {
                data.Add(A0 +(Ka * sourceWave[i]) * carryWave[i]);
            }
            return data;
        }

        /// <summary>
        /// 解调(包络法)
        /// </summary>
        /// <param name="recieveWave">接受波信号</param>
        /// <param name="rct">检波中心频率</param>
        /// <returns>解调后的信号</returns>
        public static List<double> DeModulate(List<double> recieveWave, double rct)
        {
            //检空
            if (recieveWave == null) return null;

            #region 拟合
            //double primeSignal = 0;
            //List<double> waveTop = new List<double>();
            //List<double> waveBottom = new List<double>();


            //Boolean risingEdge = false;
            ////线性插值索引
            //List<double> index = new List<double>();
            //for (int i = 0; i < recieveWave.Count; i++)
            //{
            //    //上升沿
            //    if (recieveWave[i] > primeSignal)
            //    {
            //        //更新前驱点
            //        primeSignal = recieveWave[i];
            //        //若之前还是上升沿
            //        if (risingEdge) continue;
            //        //之前下降沿现在上升沿 极小值点
            //        waveBottom.Add(primeSignal);
            //        risingEdge = true;
            //    }
            //    //下降沿
            //    else
            //    {
            //        //更新前驱点
            //        primeSignal = recieveWave[i];
            //        //若之前还是下降沿
            //        if (!risingEdge) continue;
            //        //之前下降沿现在上升沿 极大值点
            //        waveTop.Add(primeSignal);
            //        index.Add(i);
            //        risingEdge = false;
            //    }

            //}

            //var linear = Interpolate.CubicSpline(index, waveTop);
            //for (int i = 0; i < recieveWave.Count; i++)
            //{
            //    if (!index.Contains(i))
            //    {
            //        waveTop.Insert(i, linear.Interpolate(i));
            //    }
            //}

            //return waveTop;

            #endregion

            double primeSignal = 0;
            double presentSignal_ABS = 0;
            List<double> dataWave = new List<double>();
            foreach (var item in recieveWave)
            {
                //presentSignal_ABS = Math.Abs(item);
                presentSignal_ABS = item;

                if (presentSignal_ABS > primeSignal) primeSignal = presentSignal_ABS;
                else primeSignal = primeSignal * rct / (rct + 1);

                dataWave.Add(primeSignal);
            }
            return dataWave;
        }
    }
}
