using Arction.Wpf.SignalProcessing;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ChartCanvas.Utils
{
    /// <summary>
    /// 实时傅里叶计算辅助类
    /// </summary>
    public class RealtimeFFTCalculator
    {
        #region 私有对象声明
        /// <summary>
        /// 前次FFT的数据
        /// </summary>
        private double[][] _oldData;
        /// <summary>
        /// FFT执行的时间间隔
        /// </summary>
        private double _intervalMs;
        /// <summary>
        /// 频道数
        /// </summary>
        private int m_iChannelCount;
        /// <summary>
        /// FFT的窗口长度 不要求一定是2的幂次
        /// </summary>
        private int m_iFFTWindowLen;
        /// <summary>
        /// 采样频率
        /// </summary>
        private int _samplingFrequency;
        /// <summary>
        /// FFT开始时刻
        /// </summary>
        private long _startTicks;
        /// <summary>
        /// 上次FFT的时刻
        /// </summary>
        private long _lastTicks;
        /// <summary>
        /// FFT计算间隔
        /// </summary>
        private long _updateInterval;
        /// <summary>
        /// 本辅助类使用的频谱计算对象
        /// </summary>
        private SpectrumCalculator _spectrumCalculator;
        /// <summary>
        /// FFT入口索引(用于标记FFT次数)
        /// </summary>
        private int _FFTEntryIndex;
        private long m_lRefTicks;
        #endregion

        /// <summary>
        /// 构造
        /// </summary>
        /// <param name="updateIntervalMs">FFT计算间隔</param>
        /// <param name="samplingFrequency">采样频率</param>
        /// <param name="windowLength">FFT的窗口长度</param>
        /// <param name="channelCount">频道数</param>
        public RealtimeFFTCalculator(double updateIntervalMs,
            int samplingFrequency, int windowLength, int channelCount)
        {
            _oldData = new double[channelCount][];
            for (int i = 0; i < channelCount; i++)
            {
                _oldData[i] = new double[0];
            }

            _intervalMs = updateIntervalMs;
            m_iChannelCount = channelCount;
            m_iFFTWindowLen = windowLength;
            _samplingFrequency = samplingFrequency;
            _startTicks = DateTime.Now.Ticks;
            _lastTicks = _startTicks;
            _updateInterval = TimeSpan.FromMilliseconds(updateIntervalMs).Ticks;
            _spectrumCalculator = new SpectrumCalculator();
        }

        /// <summary>
        /// 从多频道数据流中计算FFT
        /// </summary>
        /// <param name="data">样例数据</param>
        /// <param name="xValues"></param>
        /// <param name="yValues"></param>
        /// <returns>如果成功计算出FFT返回true</returns>
        public bool FeedDataAndCalculate(double[][] data,
            out double[][][] xValues, out double[][][] yValues)
        {
            xValues = null;
            yValues = null;
            if (data == null)
                return false;

            int channelCounter = m_iChannelCount;
            if (data.Length < channelCounter)
                channelCounter = data.Length;

            long ticksNow = DateTime.Now.Ticks;

            bool giveDataOut = false;

            //每次更新的样例数
            int samplesPerUpdate = (int)(_intervalMs * (double)_samplingFrequency / 1000.0);
            int repeatFFT;

            if (_FFTEntryIndex == 0)
                repeatFFT = 1;
            else
                repeatFFT = (int)Math.Ceiling((double)data[0].Length / samplesPerUpdate);

            double[][][] valuesX = new double[repeatFFT][][]; //dimensions: repeatFFTcalc, channelCounter, fftResult.Length
            double[][][] valuesY = new double[repeatFFT][][];

            for (int i = 0; i < repeatFFT; i++)
            {
                //for FFT calculus copy only less or samplesPerUpdate points from DATA
                int samplesCopiesAmount = Math.Min(samplesPerUpdate, data[0].Length - i * samplesPerUpdate);
                if (_FFTEntryIndex == 0)
                    samplesCopiesAmount = data[0].Length;  //for 1st m_iFFTWindowLen number of points

                valuesX[i] = new double[channelCounter][];
                valuesY[i] = new double[channelCounter][];
                int samplesInCombinedData = 0;

                System.Threading.Tasks.Parallel.For(0, channelCounter, iChannel =>
                {
                    double[] combinedData = new double[_oldData[iChannel].Length + samplesCopiesAmount];
                    Array.Copy(_oldData[iChannel], 0, combinedData, 0, _oldData[iChannel].Length);
                    Array.Copy(data[iChannel], i * samplesPerUpdate, combinedData, _oldData[iChannel].Length, samplesCopiesAmount);

                    samplesInCombinedData = combinedData.Length;

                    // initiate FFTcalculus at fixed interval from Time-reference 
                    if (samplesInCombinedData >= m_iFFTWindowLen &&
                        ticksNow >= (m_lRefTicks + _updateInterval * (_FFTEntryIndex + 1)))
                    {
                        double[] dataCombined = new double[m_iFFTWindowLen];
                        int index = 0;

                        for (int j = samplesInCombinedData - m_iFFTWindowLen; j < samplesInCombinedData; j++)
                            dataCombined[index++] = combinedData[j];

                        double[] fftResult;
                        if (_spectrumCalculator.PowerSpectrum(dataCombined, out fftResult))
                        {

                            int length = fftResult.Length;
                            valuesX[i][iChannel] = new double[length];
                            valuesY[i][iChannel] = new double[length];


                            double stepX = (double)_samplingFrequency / 2.0 / ((double)length - 1.0);
                            //accurate estimate of DC (0 HZ) component is difficult, therefore, maybe a good idea to skip it
                            for (int point = 1; point < length; point++)
                            {
                                valuesX[i][iChannel][point] = (double)point * stepX;
                                valuesY[i][iChannel][point] = fftResult[point];
                            }

                            int samplesAmountNew = samplesPerUpdate;
                            if (m_iFFTWindowLen > samplesAmountNew)
                                samplesAmountNew = m_iFFTWindowLen;
                            if (samplesInCombinedData > samplesAmountNew)
                            {
                                //Drop oldest samples out 
                                _oldData[iChannel] = new double[samplesAmountNew];
                                Array.Copy(combinedData, samplesInCombinedData - samplesAmountNew, _oldData[iChannel], 0, samplesAmountNew);
                            }

                            giveDataOut = true;
                        }
                    }
                    else
                        _oldData[iChannel] = combinedData;
                });

                //repeat IF block to avoid some uncertainties of Parallel loop
                if (samplesInCombinedData >= m_iFFTWindowLen &&
                    ticksNow >= (m_lRefTicks + _updateInterval * (_FFTEntryIndex + 1)))
                {
                    if (_FFTEntryIndex == 0)
                        m_lRefTicks = ticksNow;

                    _FFTEntryIndex++;
                }
            }

            if (giveDataOut)
            {
                while (repeatFFT > 0 && valuesX[repeatFFT - 1][0] == null)
                    repeatFFT--; //reduce 1st dimension if it was not enough points to Calculate FFT

                if (repeatFFT == 0)
                {
                    giveDataOut = false;
                    return giveDataOut;
                }

                xValues = new double[repeatFFT][][];
                yValues = new double[repeatFFT][][];

                for (int i = 0; i < repeatFFT; i++)
                {
                    xValues[i] = new double[channelCounter][];
                    yValues[i] = new double[channelCounter][];

                    for (int iChannel = 0; iChannel < channelCounter; iChannel++)
                    {   // copy FFT results to output
                        xValues[i][iChannel] = valuesX[i][iChannel];
                        yValues[i][iChannel] = valuesY[i][iChannel];
                    }
                }
            }

            if (giveDataOut)
                _lastTicks = ticksNow;

            return giveDataOut;
        }


        /// <summary>
        /// 释放并清空
        /// </summary>
        public void Dispose()
        {
            if (_spectrumCalculator != null)
            {
                _spectrumCalculator.Dispose();
                _spectrumCalculator = null;
            }
        }
    }
}
