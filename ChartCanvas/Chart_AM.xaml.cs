using Arction.Wpf.SemibindableCharting;
using Arction.Wpf.SignalProcessing;
using ChartCanvas.Utils;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace ChartCanvas
{
    /// <summary>
    /// Chart_AM.xaml 的交互逻辑
    /// </summary>
    public partial class Chart_AM : UserControl
    {
        #region 私有对象声明
        /// <summary>
        /// 示波器
        /// </summary>
        private WaveformMonitor m_aWaveformMonitors;
        /// <summary>
        /// 频谱仪
        /// </summary>
        private SpectrogramViewXYIntensity m_aSpectrograms2D;
        /// <summary>
        /// FFT计算辅助类
        /// </summary>
        private RealtimeFFTCalculator m_fftCalculator;
        /// <summary>
        /// 采样频率
        /// </summary>
        private int _samplingFrequency;
        /// <summary>
        /// FFT窗口长度
        /// </summary>
        private int m_iFFTWindowLength;
        /// <summary>
        /// 
        /// </summary>
        private int m_iHighFreq;
        /// <summary>
        /// FFT计算间隔
        /// </summary>
        private int m_iFFTCalcIntervalMs;
        /// <summary>
        /// 本次实验所有波序列名
        /// </summary>
        private string[] _seriesNames;
        #endregion

        /// <summary>
        ///构造
        /// </summary>
        public Chart_AM()
        {
            m_aSpectrograms2D = null;
            m_aWaveformMonitors = null;
            m_iFFTCalcIntervalMs = 20;
            m_iFFTWindowLength = 1024 * 4;
            m_iHighFreq = 2000;
            _samplingFrequency = 0;
            _seriesNames = new string[]
            {
                "音频",
                "载波",
                "调制",
                "检波"
            };

            InitializeComponent();

            //销毁时释放资源
            Dispatcher.ShutdownStarted += (object sender, EventArgs e) =>
            {
                Dispose();
            };
        }

        #region Audio输入的绑定方法
        /// <summary>
        /// 音频输入开始
        /// </summary>
        private void AudioInput_Started(StartedEventArgs args)
        {
            _samplingFrequency = (int)args.SamplesPerSecond;

            InitWaveformMonitors();
            InitSpectrograms();
            InitFFTCalculator();
        }

        /// <summary>
        /// 音频输入结束
        /// </summary>
        private void AudioInput_Stopped()
        {
            //TODO
            //base.Stopped();
            Dispose();
        }

        /// <summary>
        /// 音频
        /// </summary>
        /// <param name="args"></param>
        private void AudioInput_DataGenerated(DataGeneratedEventArgs args)
        {
            double[][] samples = args.Samples;

            if (samples.Length == 0)
                return;

            if (m_aWaveformMonitors != null)
            {
                double[][] waveData = new double[_seriesNames.Count()][];

                var x = samples[0];
                var y = new AmplitudeSineSweepComponent() { Frequency = 750, AmplitudeFrom = 4000, AmplitudeTo = 100, DurationMs = 2000 };

                waveData[0] = samples[0];
                m_aWaveformMonitors.FeedData(waveData);
            }

            //Feed multi-channel data to FFT calculator. If it gives a calculated result, set multi-channel result in the selected FFT chart
            if (m_fftCalculator != null)
            {
                double[][][] yValues;
                double[][][] xValues;

                if (m_fftCalculator.FeedDataAndCalculate(samples, out xValues, out yValues))
                {
                    int rowCount = xValues.Length;
                    int channelIndex = 0;
                    for (channelIndex = 0; channelIndex < 1; channelIndex++)
                    {
                        if (m_aSpectrograms2D != null)
                        {
                            m_aSpectrograms2D.SetData(yValues, channelIndex, rowCount);
                        }
                    }

                }
            }

            //TODO
            //base.DataGenerated(samples[0].Length);
        }

        #endregion


        #region 依赖方法
        /// <summary>
        /// 初始化示波器
        /// </summary>
        private void InitWaveformMonitors()
        {
            if(m_aWaveformMonitors != null)
            {
                DisposeWaveformMonitors();
            }

            if (m_aWaveformMonitors == null)
            {
                // Let's disable SizeChanged event handler temporarily. 
                // ArrangeMonitors method is called at the end of this 
                // method.
                gridChart.SizeChanged -= gridChart_SizeChanged;

                m_aWaveformMonitors =
                    new WaveformMonitor(
                            gridChart,
                            _seriesNames,
                            DefaultColors.SeriesForBlackBackgroundWpf[0],
                            null);
                m_aWaveformMonitors.Chart.ChartName = "示波器";

                gridChart.SizeChanged += gridChart_SizeChanged;

                string strTitle = "示波器 (幅值 / 时间)\n"
                   + " " + string.Format("采样频率 = {0} kHz",
                   ((double)+_samplingFrequency / 1000.0).ToString("0"));

                m_aWaveformMonitors.Initialize(_samplingFrequency, strTitle,
                    XAxisScrollMode.Scrolling, 0.03125);
            }

            ArrangeMonitors();
        }

        /// <summary>
        /// 释放示波器
        /// </summary>
        private void DisposeWaveformMonitors()
        {
            if(m_aWaveformMonitors != null)
            {
                m_aWaveformMonitors.Dispose();
                m_aWaveformMonitors = null;
            }
        }

        /// <summary>
        /// 初始化频谱仪
        /// </summary>
        private void InitSpectrograms()
        {
            DisposeSpectrograms();

            int resolution = m_iFFTWindowLength;

            if (m_iHighFreq < _samplingFrequency / 2)
            {
                resolution = (int)Math.Round((double)m_iHighFreq /
                    (double)(_samplingFrequency / 2) * m_iFFTWindowLength);
            }

            string strTitle = "P(f)";
            m_aSpectrograms2D = new SpectrogramViewXYIntensity(
                gridChart,
                true,
                resolution,
                m_iFFTCalcIntervalMs,
                5,
                0,
                m_iHighFreq,
                strTitle,
                DefaultColors.SeriesForBlackBackgroundWpf[0]);
            m_aSpectrograms2D.Chart.ChartName = "频谱仪";

            ArrangeMonitors();
        }

        /// <summary>
        /// 释放频谱仪资源
        /// </summary>
        private void DisposeSpectrograms()
        {
            if (m_aSpectrograms2D != null)
            {
                m_aSpectrograms2D.Dispose();
                m_aSpectrograms2D = null;
            }
        }

        /// <summary>
        /// 初始化FFT数据
        /// </summary>
        private void InitFFTCalculator()
        {
            if (m_fftCalculator != null)
                m_fftCalculator.Dispose();
            m_fftCalculator = new RealtimeFFTCalculator(m_iFFTCalcIntervalMs,
                _samplingFrequency, m_iFFTWindowLength, 1);
        }

        /// <summary>
        /// 调整图表大小
        /// </summary>
        private void ArrangeMonitors()
        {
            if(m_aSpectrograms2D==null||m_aWaveformMonitors==null)
            {
                return;
            }
            //整个grid的划分
            //int columnCount = 2;
            //int rowCount = 1;

            ////
            //int iTotalWidth = (int)gridChart.ActualWidth;
            //int iTotalHeight = (int)gridChart.ActualHeight;

            //m_aWaveformMonitors.SetBounds(0, 0, iTotalWidth / 2, iTotalHeight);
            //m_aSpectrograms2D.SetBounds(0, iTotalWidth / 2, iTotalWidth, iTotalHeight);
            int chartWidth = 480;
            int chartHeight = 560;
            m_aWaveformMonitors.SetBounds(0, 0, chartWidth, chartHeight);
            m_aSpectrograms2D.SetBounds(chartWidth, 0, chartWidth, chartHeight);
        }

        /// <summary>
        /// 图表大小改变绑定事件
        /// </summary>
        private void gridChart_SizeChanged(object sender, SizeChangedEventArgs e)
        {
            ArrangeMonitors();
        }

        /// <summary>
        /// 停止模拟 释放资源
        /// </summary>
        public void Dispose()
        {

            if (audioInput1.Input.IsInputEnabled == true)
            {
                audioInput1.Input.StopRequest();
            }
            else
            {
                gridChart.Children.Clear();

                if (m_aWaveformMonitors != null)
                {
                    m_aWaveformMonitors.Dispose();
                    m_aWaveformMonitors = null;
                }

                if (m_aSpectrograms2D != null)
                {
                    m_aSpectrograms2D.Dispose();
                    m_aSpectrograms2D = null;
                }

                if (m_fftCalculator != null)
                {
                    m_fftCalculator.Dispose();
                    m_fftCalculator = null;
                }

                // Disposing of unmanaged resources done.

                //base.DisposedOf();
            }
        }
        #endregion
    }
}
