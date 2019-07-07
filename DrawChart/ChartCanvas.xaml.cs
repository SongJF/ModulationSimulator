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
        /// <summary>
        /// 添加一个数据序列
        /// </summary>
        /// <param name="tiltle">系列名称</param>
        /// <param name="data">数据</param>
        public void AddLineSeries(string tiltle, List<double> data)
        {
            //去空
            if (tiltle == null || data == null) return;
            //去重
            var result = _seriesCollection.FirstOrDefault(p => p.Title == tiltle);
            if (result != null) _seriesCollection.Remove(result);
            _seriesCollection.Add(new LineSeries
            {
                Title = tiltle,
                Fill = Brushes.Transparent,
                Values = new ChartValues<double>(data),
                PointGeometry = null,
                LineSmoothness = 0
            });
        }

        /// <summary>
        /// 清除一个数据序列
        /// </summary>
        /// <param name="name">系列名称</param>
        public void RemoveLineSeries(string name)
        {
            var series = _seriesCollection.FirstOrDefault(p => p.Title == name);
            if (series == null) return;
            _seriesCollection.Remove(series);
        }

        /// <summary>
        /// 清空图表
        /// </summary>
        public void ClearChart()
        {
            _seriesCollection.Clear();
        }
        #endregion
    }
}
