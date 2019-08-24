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
    /// Chart_PCM.xaml 的交互逻辑
    /// </summary>
    public partial class Chart_PCM : UserControl
    {
        #region 对象声明
        /// <summary>
        /// 实验所用示波器
        /// </summary>
        private WaveformMonitor m_WaveformMonitor;
        /// <summary>
        /// 采样频率
        /// </summary>
        private double _samplingFrequency;
        /// <summary>
        /// 本次实验所有波序列名
        /// </summary>
        private string[] _seriesNames;
        #endregion

        /// <summary>
        /// 构造
        /// </summary>
        public Chart_PCM()
        {
            m_WaveformMonitor = null;
            _samplingFrequency = 0;
            _seriesNames = new string[]
            {
                "信号源",
                "PCM信号"
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
            //为示波器输入数据
            if (m_WaveformMonitor != null)
            {
                double[][] waveData = new double[_seriesNames.Count()][];
                waveData[0] = souceWave;
                m_WaveformMonitor.FeedData(waveData);
            }
        }

        #endregion

        #region 依赖方法
        /// <summary>
        /// 初始化示波器
        /// </summary>
        private void InitWaveformMonitors()
        {
            if (m_WaveformMonitor != null)
            {
                DisposeWaveformMonitors();
            }

            if (m_WaveformMonitor == null)
            {
                // Let's disable SizeChanged event handler temporarily. 
                // ArrangeMonitors method is called at the end of this 
                // method.
                gridChart.SizeChanged -= gridChart_SizeChanged;

                m_WaveformMonitor =
                    new WaveformMonitor(
                            gridChart,
                            _seriesNames,
                            _samplingFrequency,
                            DefaultColors.SeriesForBlackBackgroundWpf[0],
                            null);
                m_WaveformMonitor.Chart.ChartName = "示波器";

                gridChart.SizeChanged += gridChart_SizeChanged;

                string strTitle = "示波器 (幅值 / 时间)\n"
                   + " " + string.Format("采样频率 = {0} kHz",
                   ((double)+_samplingFrequency / 1000.0).ToString("0"));

                m_WaveformMonitor.Initialize(_samplingFrequency, strTitle,
                    XAxisScrollMode.Scrolling, 0.03125);
            }

            ArrangeMonitors();
        }
        /// <summary>
        /// 图表缩放事件
        /// </summary>
        private void Click_Zoom(object sender, RoutedEventArgs e)
        {
            var button = sender as Button;
            if (button == null || m_WaveformMonitor == null)
                return;

            switch (button.Tag)
            {
                case ChartZoomOption.XMinus:
                    m_WaveformMonitor.SetXLenZoom(2.0);
                    break;
                case ChartZoomOption.XPlus:
                    m_WaveformMonitor.SetXLenZoom(0.5);
                    break;
                case ChartZoomOption.YMinus:
                    m_WaveformMonitor.SetYLenZoom(2.0);
                    break;
                case ChartZoomOption.YPlus:
                    m_WaveformMonitor.SetYLenZoom(0.5);
                    break;
                case ChartZoomOption.Auto:
                    m_WaveformMonitor.FitView();
                    //m_aSpectrograms2D_signal.FitView();
                    //m_aSpectrograms2D_source.FitView();
                    break;
            }
        }

        #endregion

        /// <summary>
        /// 释放示波器
        /// </summary>
        private void DisposeWaveformMonitors()
        {
            if (m_WaveformMonitor != null)
            {
                m_WaveformMonitor.Dispose();
                m_WaveformMonitor = null;
            }
        }

        /// <summary>
        /// 图表大小改变绑定事件
        /// </summary>
        private void gridChart_SizeChanged(object sender, SizeChangedEventArgs e)
        {
            ArrangeMonitors();
        }

        /// <summary>
        /// 调整图表大小
        /// </summary>
        private void ArrangeMonitors()
        {
            //去空
            if (m_WaveformMonitor == null)
            {
                return;
            }

            int chartWidth = 460;
            int chartHeight = 540;
            m_WaveformMonitor.SetBounds(0, 0, chartWidth, chartHeight);
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

                if (m_WaveformMonitor != null)
                {
                    m_WaveformMonitor.Dispose();
                    m_WaveformMonitor = null;
                }
            }
        }
    }
}
