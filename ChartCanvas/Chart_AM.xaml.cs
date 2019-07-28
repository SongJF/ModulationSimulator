using Arction.Wpf.SemibindableCharting;
using Arction.Wpf.SignalProcessing;
using ChartCanvas.Utils;
using System;
using System.Linq;
using System.Windows;
using System.Windows.Controls;

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
        /// 频谱仪(源)
        /// </summary>
        private SpectrogramViewXYIntensity m_aSpectrograms2D_source;
        /// <summary>
        /// 频谱仪(信号)
        /// </summary>
        private SpectrogramViewXYIntensity m_aSpectrograms2D_signal;
        /// <summary>
        /// FFT计算辅助类(源)
        /// </summary>
        private RealtimeFFTCalculator m_fftCalculator_source;
        /// <summary>
        /// FFT计算辅助类(信号)
        /// </summary>
        private RealtimeFFTCalculator m_fftCalculator_signal;
        /// <summary>
        /// 采样频率
        /// </summary>
        private int _samplingFrequency;
        /// <summary>
        /// FFT窗口长度
        /// </summary>
        private int m_iFFTWindowLength;
        /// <summary>
        /// 频谱仪显示最大频率
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

        public Param_AM Param { get; set; }
        #endregion

        /// <summary>
        ///构造
        /// </summary>
        public Chart_AM()
        {
            m_aSpectrograms2D_source = null;
            m_aSpectrograms2D_signal = null;
            m_aWaveformMonitors = null;
            m_iFFTCalcIntervalMs = 20;
            m_iFFTWindowLength = 1024 * 4;
            m_iHighFreq = 2000;
            _samplingFrequency = 0;
            Param = new Param_AM(1000);
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
        /// 音频数据更新
        /// </summary>
        /// <param name="args"></param>
        private void AudioInput_DataGenerated(DataGeneratedEventArgs args)
        {
            double[][] samples = args.Samples;

            if (samples.Length == 0)
                return;

            //源波
            double[] souceWave = samples[0];

            //生成载波
            double MaxAmplitude = 10000;
            double[] carryWave = WaveGenerator.Sine(samples[0].Count(), 
                (int)MaxAmplitude, _samplingFrequency, Param.moudulateFrequency);

            //生成调制波
            double[] modulatedWava = new double[souceWave.Count()];
            for (int i = 0; i < souceWave.Count(); i++)
            {
                double signal = souceWave[i] * carryWave[i];
                //每个信号除以载波幅度以约束其值范围
                signal /= MaxAmplitude;
                modulatedWava[i] = signal;
            }

            //为示波器输入数据
            if (m_aWaveformMonitors != null)
            {
                double[][] waveData = new double[_seriesNames.Count()][];
                waveData[0] = souceWave;
                waveData[1] = carryWave;
                waveData[2] = modulatedWava;
                waveData[3] = samples[0];
                m_aWaveformMonitors.FeedData(waveData);
            }

            //Feed multi-channel data to FFT calculator. If it gives a calculated result, set multi-channel result in the selected FFT chart
            if (m_fftCalculator_source != null)
            {
                double[][][] yValues;
                double[][][] xValues;

                if (m_fftCalculator_source.FeedDataAndCalculate(samples, out xValues, out yValues))
                {
                    int rowCount = xValues.Length;
                     m_aSpectrograms2D_source.SetData(yValues, 0, rowCount);
                }
            }

            if(m_fftCalculator_signal != null)
            {
                double[][][] yValues;
                double[][][] xValues;

                if (m_fftCalculator_signal.FeedDataAndCalculate(
                   new double[1][] { modulatedWava },
                   out xValues, out yValues))
                {
                    int rowCount = xValues.Length;
                    m_aSpectrograms2D_signal.SetData(yValues, 0, rowCount);
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
                            _samplingFrequency,
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
            m_aSpectrograms2D_source = new SpectrogramViewXYIntensity(
                gridChart,
                true,
                resolution,
                m_iFFTCalcIntervalMs,
                5,
                0,
                m_iHighFreq,
                strTitle,
                DefaultColors.SeriesForBlackBackgroundWpf[0]);
            m_aSpectrograms2D_source.Chart.ChartName = "频谱仪(源)";

            m_aSpectrograms2D_signal = new SpectrogramViewXYIntensity(
                gridChart,
                true,
                resolution,
                m_iFFTCalcIntervalMs,
                5,
                0,
                m_iHighFreq,
                strTitle,
                DefaultColors.SeriesForBlackBackgroundWpf[0]);
            m_aSpectrograms2D_signal.Chart.ChartName = "频谱仪(信号)";

            ArrangeMonitors();
        }

        /// <summary>
        /// 释放频谱仪资源
        /// </summary>
        private void DisposeSpectrograms()
        {
            if (m_aSpectrograms2D_source != null)
            {
                m_aSpectrograms2D_source.Dispose();
                m_aSpectrograms2D_source = null;
            }
            if (m_aSpectrograms2D_signal != null)
            {
                m_aSpectrograms2D_signal.Dispose();
                m_aSpectrograms2D_signal = null;
            }
        }

        /// <summary>
        /// 初始化FFT数据
        /// </summary>
        private void InitFFTCalculator()
        {
            if (m_fftCalculator_source != null)
                m_fftCalculator_source.Dispose();
            if (m_fftCalculator_signal != null)
                m_fftCalculator_signal.Dispose();
            m_fftCalculator_source = new RealtimeFFTCalculator(m_iFFTCalcIntervalMs,
                _samplingFrequency, m_iFFTWindowLength, 1);
            m_fftCalculator_signal = new RealtimeFFTCalculator(m_iFFTCalcIntervalMs,
                _samplingFrequency, m_iFFTWindowLength, 1);
        }

        /// <summary>
        /// 调整图表大小
        /// </summary>
        private void ArrangeMonitors()
        {
            //去空
            if(m_aSpectrograms2D_source==null || 
                m_aSpectrograms2D_signal == null ||
                m_aWaveformMonitors==null)
            {
                return;
            }

            int chartWidth = 460;
            int chartHeight = 540;
            m_aWaveformMonitors.SetBounds(0, 0, chartWidth, chartHeight);
            m_aSpectrograms2D_source.SetBounds(chartWidth + 5, 0, chartWidth, chartHeight/2);
            m_aSpectrograms2D_signal.SetBounds(chartWidth + 5, chartHeight / 2, chartWidth, chartHeight / 2);
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

                if (m_aSpectrograms2D_source != null)
                {
                    m_aSpectrograms2D_source.Dispose();
                    m_aSpectrograms2D_source = null;
                }
                if (m_aSpectrograms2D_signal != null)
                {
                    m_aSpectrograms2D_signal.Dispose();
                    m_aSpectrograms2D_signal = null;
                }

                if (m_fftCalculator_source != null)
                {
                    m_fftCalculator_source.Dispose();
                    m_fftCalculator_source = null;
                }

                if (m_fftCalculator_signal != null)
                {
                    m_fftCalculator_signal.Dispose();
                    m_fftCalculator_signal = null;
                }

                // Disposing of unmanaged resources done.

                //base.DisposedOf();
            }
        }

        /// <summary>
        /// 保存为图片
        /// </summary>
        /// <param name="Chart"></param>
        public void SaveImage(ImageSaveMode mode)
        {
            LightningChartUltimate Chart;

            switch(mode)
            {
                case ImageSaveMode.Wave:
                    Chart = m_aWaveformMonitors.Chart;
                    break;
                case ImageSaveMode.SignalSpectrograms:
                    Chart = m_aSpectrograms2D_signal.Chart;
                    break;
                case ImageSaveMode.SourceSpectrograms:
                    Chart = m_aSpectrograms2D_source.Chart;
                    break;
                default:
                    return;
            }

            if(Chart != null)
            {
                var saveDialog = new Microsoft.Win32.SaveFileDialog();
                saveDialog.Filter = "图片文件 (*.png;*.bmp;*.jpg;)|*.png;*.bmp;*.jpg;|All files (*.*)|*.*";
                saveDialog.FileName = Chart.ChartName;
                if ( saveDialog.ShowDialog() == true)
                {
                    if(Chart.SaveToFile(saveDialog.FileName) == false)
                    {
                        throw new Exception("图片保存失败");
                    }
                }

            }
        }
        #endregion
    }

    public enum ImageSaveMode
    {
        Wave = 0,
        SourceSpectrograms = 1,
        SignalSpectrograms = 2
    }

    public class Param_AM
    {
        public Param_AM(double _moudulateFrequency)
        {
            moudulateFrequency = _moudulateFrequency;
        }

        public double moudulateFrequency { get; set; }
    }
}
