using Arction.Wpf.SemibindableCharting;
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

namespace ChartCanvas.Components
{
    /// <summary>
    /// PCMCaculatorPage.xaml 的交互逻辑
    /// </summary>
    public partial class PCMCaculatorPage : UserControl
    {
        /// <summary>
        /// PCM计算器的编码显示器
        /// </summary>
        private LightningChartUltimate _chart = null;

        public PCMCaculatorPage()
        {
            InitializeComponent();

            InitPCMcodeMonitor();

            //销毁时释放资源
            Dispatcher.ShutdownStarted += (object sender, EventArgs e) =>
            {
                Dispose();
            };
        }

        /// <summary>
        /// 
        /// </summary>
        private void InitPCMcodeMonitor()
        {
            _chart = Monitor;
            _chart.ViewXY.ZoomPanOptions.MouseWheelZooming = MouseWheelZooming.Off;
        }

        private void Dispose()
        {
            if (_chart != null)
            {
                _chart.Dispose();
                _chart = null;
            }
        }

        private void TextChange_InputVal(object sender, TextChangedEventArgs e)
        {
            TextBox tb = sender as TextBox;
            if (tb.Text == null) return;

            try
            {
                //填写
                double val = double.Parse(tb.Text);
                int[] codes = PCMCaculator.PCM_Encode(val);
                string ans = "|";
                foreach (var code in codes)
                {
                    ans = ans + " " + code.ToString() + " |";
                }
                EncodeStrTextBlock.Text = ans;

                //更新PCM显示器数据
                SeriesPoint[] points = new SeriesPoint[codes.Length + 1];
                for(int i = 0; i < codes.Length; i ++)
                {
                    points[i].X = i;
                    points[i].Y = codes[i];
                }
                points[codes.Length].X = codes.Length;
                points[codes.Length].Y = codes[codes.Length - 1];
                _chart.ViewXY.PointLineSeries[0].Points = points;
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex);
                return;
            }
        }
    }
}
