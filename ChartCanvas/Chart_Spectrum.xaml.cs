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
    /// Chart_Spectrum.xaml 的交互逻辑
    /// </summary>
    public partial class Chart_Spectrum : UserControl
    {
        #region 私有对象声明
        /// <summary>
        /// 示波器
        /// </summary>
        private WaveformMonitor[] m_aWaveformMonitors;
        /// <summary>
        /// 频谱仪
        /// </summary>
        private SpectrogramViewXYIntensity[] m_aSpectrograms2D;
        /// <summary>
        /// FFT计算辅助类
        /// </summary>
        private RealtimeFFTCalculator m_fftCalculator;
        /// <summary>
        /// 实验所用频道数
        /// </summary>
        private int _channelCount;
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
        /// 清除标记
        /// </summary>
        private bool _cleanUp;
        #endregion

        /// <summary>
        /// 构造
        /// </summary>
        public Chart_Spectrum()
        {
            m_aSpectrograms2D = null;
            m_aWaveformMonitors = null;
            _cleanUp = false;
            _channelCount = 0;
            m_iFFTCalcIntervalMs = 20;
            m_iFFTWindowLength = 1024 * 4;
            m_iHighFreq = 2000;
            _samplingFrequency = 0;

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
            _channelCount = args.ChannelCount;
            _samplingFrequency = (int)args.SamplesPerSecond;

            InitWaveformMonitors();
            InitSpectrograms();
            InitFFTCalculator();

            List<LightningChartUltimate> charts = new List<LightningChartUltimate>(m_aWaveformMonitors.Length + m_aSpectrograms2D.Length);

            foreach (WaveformMonitor wm in m_aWaveformMonitors)
            {
                charts.Add(wm.Chart);
            }

            foreach (SpectrogramViewXYIntensity svi in m_aSpectrograms2D)
            {
                charts.Add(svi.Chart);
            }

            //TODO
            //base.ChartsCreated(charts);
            //base.Started();
        }

        /// <summary>
        /// 音频输入结束
        /// </summary>
        private void AudioInput_Stopped()
        {
            //TODO
            //base.Stopped();

            if (_cleanUp == true)
            {
                Dispose();
            }
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

            int channelIndex = 0;
            if (m_aWaveformMonitors != null)
            {
                foreach (WaveformMonitor wm in m_aWaveformMonitors)
                {
                    wm.FeedData(samples[channelIndex]);
                    channelIndex++;
                }
            }

            //Feed multi-channel data to FFT calculator. If it gives a calculated result, set multi-channel result in the selected FFT chart
            if (m_fftCalculator != null)
            {
                double[][][] yValues;
                double[][][] xValues;

                if (m_fftCalculator.FeedDataAndCalculate(samples, out xValues, out yValues))
                {
                    int rowCount = xValues.Length;

                    for (channelIndex = 0; channelIndex < _channelCount; channelIndex++)
                    {
                        if (m_aSpectrograms2D != null)
                        {
                            m_aSpectrograms2D[channelIndex].SetData(yValues, channelIndex, rowCount);
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
            bool bCreate = false;
            if (m_aWaveformMonitors != null)
            {
                if (_channelCount != m_aWaveformMonitors.Length)
                {
                    DisposeWaveformMonitors();
                    bCreate = true;
                }
            }
            else
                bCreate = true;

            if (bCreate)
            {
                m_aWaveformMonitors = new WaveformMonitor[_channelCount];

                

                for (int i = 0; i < _channelCount; i++)
                {
                    WaveformMonitor wm =
                        new WaveformMonitor(
                            gridChart,
                            0.25,
                            DefaultColors.SeriesForBlackBackgroundWpf[i % DefaultColors.SeriesForBlackBackgroundWpf.Length],
                            null
                        );
                    wm.Chart.ChartName = "Waveform Chart " + (i + 1).ToString();

                    m_aWaveformMonitors[i] = wm;
                }

                // Re-enable SizeChanged event handler.
                gridChart.SizeChanged += gridChart_SizeChanged;
            }

            for (int i = 0; i < _channelCount; i++)
            {
                string strTitle = "Audio waveform (amplitude / time)\n"
                    + " " + string.Format("sfreq = {0} kHz",
                    ((double)+_samplingFrequency / 1000.0).ToString("0"));

                if (_channelCount == 2)
                {
                    if (i == 0)
                        strTitle += " - Left";
                    else
                        strTitle += " - Right";
                }

                m_aWaveformMonitors[i].Initialize(_samplingFrequency, strTitle, XAxisScrollMode.Scrolling, 0.03125);
            }

            ArrangeMonitors();
        }

        /// <summary>
        /// 初始化频谱仪
        /// </summary>
        private void InitSpectrograms()
        {
            DisposeSpectrograms();

            int resolution = m_iFFTWindowLength;
            if (m_iHighFreq < _samplingFrequency / 2)
                resolution = (int)Math.Round((double)m_iHighFreq / (double)(_samplingFrequency / 2) * m_iFFTWindowLength);

            m_aSpectrograms2D = new SpectrogramViewXYIntensity[_channelCount];
            for (int channelIndex = 0; channelIndex < _channelCount; channelIndex++)
            {
                string strTitle = "P(f)\n - Channel " + (channelIndex + 1).ToString();
                if (_channelCount == 2)
                {
                    if (channelIndex == 0)
                        strTitle = "P(f) - Left";
                    else
                        strTitle = "P(f) - Right";
                }

                SpectrogramViewXYIntensity svi =
                    new SpectrogramViewXYIntensity(
                        gridChart,
                        true,
                        resolution,
                        m_iFFTCalcIntervalMs,
                        5,
                        0,
                        m_iHighFreq,
                        strTitle,
                        DefaultColors.SeriesForBlackBackgroundWpf[channelIndex % DefaultColors.SeriesForBlackBackgroundWpf.Length]
                    );
                svi.Chart.ChartName = "Spectrogram Chart " + (channelIndex + 1).ToString();

                m_aSpectrograms2D[channelIndex] = svi;
            }

            ArrangeMonitors();
        }

        /// <summary>
        /// 初始化FFT计算
        /// </summary>
        private void InitFFTCalculator()
        {
            if (m_fftCalculator != null)
                m_fftCalculator.Dispose();
            m_fftCalculator = new RealtimeFFTCalculator(m_iFFTCalcIntervalMs,
                _samplingFrequency, m_iFFTWindowLength, _channelCount);
        }

        /// <summary>
        /// 图表大小改变绑定事件
        /// </summary>
        private void gridChart_SizeChanged(object sender, SizeChangedEventArgs e)
        {
            ArrangeMonitors();
        }

        /// <summary>
        /// 设置图表大小
        /// </summary>
        private void ArrangeMonitors()
        {
            if (_channelCount == 0)
                return;
            int columnCount = _channelCount;
            int iTotalWidth = (int)gridChart.ActualWidth;
            int iTotalHeight = (int)gridChart.ActualHeight;
            int width = iTotalWidth / columnCount;
            int rowCount = 0;
            if (m_aWaveformMonitors != null)
                rowCount++;
            if (m_aSpectrograms2D != null)
                rowCount++;

            int iHeightPerRow = iTotalHeight / rowCount;

            for (int row = 0; row < rowCount; row++)
            {
                for (int columnIndex = 0; columnIndex < columnCount; columnIndex++)
                {
                    int iH = iHeightPerRow;
                    int iW = width;
                    if (row == rowCount - 1)
                        iH = iTotalHeight - row * iHeightPerRow;
                    if (columnIndex == columnCount - 1)
                        iW = iTotalWidth - columnIndex * width;

                    if (row == 0)
                        m_aWaveformMonitors[columnIndex].SetBounds(columnIndex * width, row * iHeightPerRow, iW, iH);
                    else if (row == 1)
                        m_aSpectrograms2D[columnIndex].SetBounds(columnIndex * width, row * iHeightPerRow, iW, iH);
                }
            }
        }

        /// <summary>
        /// 释放示波器
        /// </summary>
        private void DisposeWaveformMonitors()
        {
            if (m_aWaveformMonitors != null)
            {
                foreach (WaveformMonitor wm in m_aWaveformMonitors)
                {
                    wm.Dispose();
                }
                m_aWaveformMonitors = null;
            }
        }

        /// <summary>
        /// 释放频谱仪
        /// </summary>
        private void DisposeSpectrograms()
        {
            if (m_aSpectrograms2D != null)
            {
                foreach (SpectrogramViewXYIntensity chart in m_aSpectrograms2D)
                {
                    chart.Dispose();
                }
                m_aSpectrograms2D = null;
            }
        }

        /// <summary>
        /// 停止模拟 释放资源
        /// </summary>
        public void Dispose()
        {
            _cleanUp = true;

            if (audioInput1.Input.IsInputEnabled == true)
            {
                audioInput1.Input.StopRequest();
            }
            else
            {
                gridChart.Children.Clear();

                if (m_aWaveformMonitors != null)
                {
                    foreach (WaveformMonitor wm in m_aWaveformMonitors)
                    {
                        wm.Dispose();
                    }
                    m_aWaveformMonitors = null;
                }

                if (m_aSpectrograms2D != null)
                {
                    foreach (SpectrogramViewXYIntensity spectrogram2D in m_aSpectrograms2D)
                    {
                        spectrogram2D.Dispose();
                    }
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
