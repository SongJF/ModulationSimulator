using LiveCharts;
using LiveCharts.Wpf;
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

namespace DrawChart
{
    /// <summary>
    /// Canvas.xaml 的交互逻辑
    /// </summary>
    public partial class ChartCanvas : UserControl
    {
        public SeriesCollection _seriesCollection { get; set; }
        public ChartCanvas()
        {
            InitializeComponent();

            //初始化图表序列
            _seriesCollection = new SeriesCollection()
            {
                new LineSeries
                {
                    Title= Properties.Resources.ChartTitle_Trans,
                    Values= new ChartValues<Double> (),
                    PointGeometry = null,
                    LineSmoothness= 0
                }
            };
            //数据绑定
            DataContext = this;

            InitWave();
        }

        /// <summary>
        /// 生成初始化正弦波
        /// </summary>
        private void InitWave()
        {
            for (int i = 0; i < 256; i++)
            {
                _seriesCollection[0].Values.Add(Math.Sin(Math.PI * i * 4 / 180));
            }
        }
    }
}
