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
        /// PCM编码显示器
        /// </summary>
        private WaveformMonitor m_CodeMonitor;
        /// <summary>
        /// 采样频率
        /// </summary>
        private double _samplingFrequency;
        /// <summary>
        /// 本次实验所有波序列名
        /// </summary>
        private string[] _seriesNames;
        /// <summary>
        /// 本次实验的可调参数
        /// </summary>
        public Param_PCM Param { get; set; }
        #endregion

        /// <summary>
        /// 构造
        /// </summary>
        public Chart_PCM()
        {
            m_WaveformMonitor = null;
            m_CodeMonitor = null;
            _samplingFrequency = 0;
            _seriesNames = new string[]
            {
                "信号源",
                "PCM信号"
            };
            Param = new Param_PCM(2000);

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

            List<double> sampledData = new List<double>();

            //采样后的信号
            double[] sampledWave = new double[souceWave.Count()];
            int impact = (int) (_samplingFrequency / Param.secSamplingFrequency);
            for(int i = 0; i < souceWave.Count(); i ++)
            {
                if (i % impact == 0)
                {
                    sampledWave[i] = souceWave[i];
                    sampledData.Add(souceWave[i]);
                }
                else sampledWave[i] = 0.0;
            }

            var x = PCM_Encode(sampledData);
                

            //为示波器输入数据
            if (m_WaveformMonitor != null)
            {
                double[][] waveData = new double[_seriesNames.Count()][];
                waveData[0] = souceWave;
                waveData[1] = sampledWave;
                m_WaveformMonitor.FeedData(waveData);
            }

            List<double> pcmCode = new List<double>();
            foreach(var item in PCM_Encode(sampledData))
            {
                foreach(var val in item)
                {
                    pcmCode.Add(val);
                }
            }
            //为示波器输入数据
            if (m_CodeMonitor != null)
            {
                double[][] Data = new double[1][];
                Data[0] = pcmCode.ToArray();
                m_CodeMonitor.FeedData(Data);
            }
        }

        #endregion

        #region 依赖方法
        /// <summary>
        /// 初始化示波器
        /// </summary>
        private void InitWaveformMonitors()
        {
            if (m_WaveformMonitor != null || m_CodeMonitor != null)
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

                m_WaveformMonitor.Chart.ViewXY.ZoomPanOptions.MouseWheelZooming = MouseWheelZooming.Off;
            }

            if (m_CodeMonitor == null)
            {
                // Let's disable SizeChanged event handler temporarily. 
                // ArrangeMonitors method is called at the end of this 
                // method.
                gridChart.SizeChanged -= gridChart_SizeChanged;

                m_CodeMonitor =
                    new WaveformMonitor(
                            gridChart,
                            new string[] {
                                "PCM编码"
                            },
                            _samplingFrequency,
                            DefaultColors.SeriesForBlackBackgroundWpf[0],
                            null);
                m_CodeMonitor.Chart.ChartName = "PCM显示器";

                gridChart.SizeChanged += gridChart_SizeChanged;

                m_CodeMonitor.Initialize(_samplingFrequency, null,
                    XAxisScrollMode.Scrolling, 0.03125);
                //重新设置Y轴范围
                m_CodeMonitor.Chart.ViewXY.YAxes.FirstOrDefault().SetRange(-0.5, 1.5);
                m_CodeMonitor.Chart.ViewXY.ZoomPanOptions.MouseWheelZooming = MouseWheelZooming.Off;
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
                    m_CodeMonitor.SetXLenZoom(2.0);
                    break;
                case ChartZoomOption.XPlus:
                    m_WaveformMonitor.SetXLenZoom(0.5);
                    m_CodeMonitor.SetXLenZoom(0.5);
                    break;
                case ChartZoomOption.YMinus:
                    m_WaveformMonitor.SetYLenZoom(2.0);
                    m_CodeMonitor.SetYLenZoom(2.0);
                    break;
                case ChartZoomOption.YPlus:
                    m_WaveformMonitor.SetYLenZoom(0.5);
                    m_CodeMonitor.SetYLenZoom(0.5);
                    break;
                case ChartZoomOption.Auto:
                    m_WaveformMonitor.FitView();
                    m_CodeMonitor.FitView();
                    //m_aSpectrograms2D_signal.FitView();
                    //m_aSpectrograms2D_source.FitView();
                    break;
            }
        }

        #endregion

        /// <summary>
        /// PCM编码
        /// </summary>
        /// <param name="data">PCM源数据</param>
        /// <returns>PCM编码</returns>
        private List<int[]> PCM_Encode(List<double> data)
        {
            List<int[]> answer = new List<int[]>();
            foreach(var item in data)
            {
                int value = (int)item % 2048;

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
                else if (64 <= value && value <128)
                {
                    ans = setSectionCode(ans, 1, "011");
                    step = 4;
                    st = 64;
                }
                else if(128 <= value && value < 256)
                {
                    ans = setSectionCode(ans, 1, "100");
                    step = 8;
                    st = 128;
                }
                else if(256 <= value && value < 512)
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
                var insideCode = (int)Math.Floor((double)((value - st) / step));
                ans = setSectionCode(ans, 4, Convert.ToString(insideCode, 2));

                answer.Add(ans);
            }
            return answer;
        }

        /// <summary>
        /// 设置段落码和段内码
        /// </summary>
        private int[] setSectionCode(int[] data,int index,string value)
        {
            foreach(var item in value)
            {
                data[index] = int.Parse(item.ToString());
                index++;
            }
            return data;
        }

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

            if (m_CodeMonitor != null)
            {
                m_CodeMonitor.Dispose();
                m_CodeMonitor = null;
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
            if (m_WaveformMonitor == null || m_CodeMonitor == null)
            {
                return;
            }

            int chartWidth = 460;
            int chartHeight = 540;
            m_WaveformMonitor.SetBounds(0, 0, chartWidth, chartHeight * 2 / 3);
            m_CodeMonitor.SetBounds(0, chartHeight * 2 / 3, chartWidth, chartHeight / 3);
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
