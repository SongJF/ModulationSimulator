using DataManager.Model;
using DrawChart.Model;
using LiveCharts;
using LiveCharts.Wpf;
using System.Collections.Generic;
using System.Linq;
using System.Windows.Controls;
using System.Windows.Media;

namespace DrawChart
{
    /// <summary>
    /// Canvas.xaml 的交互逻辑
    /// </summary>
    public partial class ChartCanvas : UserControl
    {
        public SeriesCollection _seriesCollection { get; set; }

        public ChartZooming _chartZooming { get; set; }

        /// <summary>
        /// 初始化
        /// </summary>
        public ChartCanvas()
        {
            InitializeComponent();

            //初始化图表序列
            _seriesCollection = new SeriesCollection();
            //设置缩放值
            _chartZooming = new ChartZooming(0, GlobalVariable.BitNumbers);
            //数据绑定
            DataContext = this;
        }

        #region 封装的公开方法
        public void AddLineSeries(string tiltle, List<double> data)
        {
            //去重
            if (_seriesCollection.FirstOrDefault(p => p.Title == tiltle) != null) return;
            _seriesCollection.Add(new LineSeries
            {
                Title = tiltle,
                Fill = Brushes.Transparent,
                Values = new ChartValues<double>(data),
                PointGeometry = null,
                LineSmoothness = 0
            });
        }

        public void ClearChart()
        {
            _seriesCollection.Clear();
        }
        #endregion
    }
}
