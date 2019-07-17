using Arction.Wpf.SemibindableCharting;
using Arction.Wpf.SemibindableCharting.Axes;
using Arction.Wpf.SemibindableCharting.ChartManager;
using Arction.Wpf.SemibindableCharting.SeriesXY;
using Arction.Wpf.SemibindableCharting.Views.ViewXY;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;

namespace ChartCanvas.Utils
{
    /// <summary>
    /// 示波器辅助类
    /// </summary>
    public class WaveformMonitor
    {
        /// <summary>
        /// 内置Lightning图表
        /// </summary>
        private LightningChartUltimate _chart;
        private Double _samplingFrequency;
        private Panel _parentControl;

        /// <summary>
        /// 构造函数
        /// </summary>
        /// <param name="parentControl">父容器</param>
        /// <param name="xAxisMax">x轴最大值(无用参数)</param>
        /// <param name="lineColor">绘制曲线颜色</param>
        /// <param name="chartManager"> 内置图表的的chartManager(可空)</param>
        public WaveformMonitor(
            Panel parentControl,
            Double xAxisMax,
            Color lineColor,
            ChartManager chartManager
        )
        {
            _samplingFrequency = 0.0;
            _parentControl = parentControl;

            _chart = new LightningChartUltimate();
            _chart.ChartName = "Waveform chart";
            _chart.ViewXY.XAxes = ViewXY.CreateDefaultXAxes();
            _chart.ViewXY.YAxes = ViewXY.CreateDefaultYAxes();
            _chart.VerticalAlignment = VerticalAlignment.Top;
            _chart.HorizontalAlignment = HorizontalAlignment.Left;

            parentControl.Children.Add(_chart);

            _chart.BeginUpdate();
            _chart.ChartManager = chartManager;

            _chart.ViewXY.AxisLayout.AutoAdjustMargins = false;

            _chart.ViewXY.DropOldSeriesData = true;

            AxisX axisX = _chart.ViewXY.XAxes[0];
            axisX.Maximum = 10;
            axisX.SweepingGap = 2;
            axisX.ScrollMode = XAxisScrollMode.Scrolling;
            axisX.Title.Text = "Range";
            axisX.Title.VerticalAlign = XAxisTitleAlignmentVertical.Top;
            axisX.Title.HorizontalAlign = XAxisTitleAlignmentHorizontal.Right;
            axisX.LabelsPosition = Alignment.Near;
            axisX.LabelsFont = new WpfFont("Segoe UI", 11, true, false);
            axisX.MajorDivTickStyle.Visible = false;
            axisX.MinorDivTickStyle.Visible = false;
            axisX.MajorGrid.Visible = false;
            axisX.MinorGrid.Visible = false;
            axisX.LabelsVisible = false;
            axisX.SteppingInterval = 1;
            axisX.MouseScaling = false;
            axisX.MouseScrolling = false;
            axisX.AxisThickness = 1;

            AxisY axisY = _chart.ViewXY.YAxes[0];
            axisY.SetRange(-30000, 30000);
            axisY.Title.Visible = false;
            axisY.LabelsFont = new WpfFont("Segoe UI", 11, true, false);

            _chart.ViewXY.GraphBackground.GradientDirection = 270;
            _chart.ViewXY.GraphBackground.GradientFill = GradientFill.Cylindrical;

            Color color = _chart.ViewXY.GraphBackground.Color;
            _chart.ViewXY.GraphBackground.Color = Color.FromArgb(150, color.R, color.G, color.B);

            _chart.Title.Font = new WpfFont("Segoe UI", 14, true, false);

            _chart.Title.Align = ChartTitleAlignment.TopCenter;
            _chart.Title.Offset.SetValues(0, 25);
            _chart.Title.Color = lineColor;

            _chart.ViewXY.Margins = new Thickness(70, 10, 15, 10);
            _chart.ViewXY.ZoomPanOptions.ZoomRectLine.Color = Colors.Lime;

            _chart.ChartBackground.Color = ChartTools.CalcGradient(lineColor, Colors.Black, 65);
            _chart.ChartBackground.GradientDirection = 0;
            _chart.ChartBackground.GradientFill = GradientFill.Cylindrical;

            //Add SampleDataSeries
            SampleDataSeries sds = new SampleDataSeries(_chart.ViewXY, axisX, axisY);
            sds.LineStyle.Width = 1f;
            sds.LineStyle.Color = lineColor;
            sds.LineStyle.AntiAliasing = LineAntialias.None;
            sds.ScrollingStabilizing = true;
            sds.MouseInteraction = false;
            _chart.ViewXY.SampleDataSeries.Add(sds);

            //Add the line as a zero level
            ConstantLine cls = new ConstantLine(_chart.ViewXY, axisX, axisY);
            cls.Title.Text = "Constant line";
            cls.Title.Visible = false;
            cls.LineStyle.Color = Colors.BlueViolet;
            cls.Behind = true;
            cls.LineStyle.Width = 2;
            cls.MouseInteraction = false;
            cls.Value = 0;
            _chart.ViewXY.ConstantLines.Add(cls);

            LineSeriesCursor cursor1 = new LineSeriesCursor(_chart.ViewXY, axisX);
            cursor1.ValueAtXAxis = 1;
            cursor1.LineStyle.Width = 6;

            color = Colors.OrangeRed;
            cursor1.LineStyle.Color = Color.FromArgb(180, color.R, color.G, color.B);

            cursor1.FullHeight = true;
            cursor1.SnapToPoints = true;
            cursor1.Style = CursorStyle.PointTracking;
            cursor1.TrackPoint.Color1 = Colors.Yellow;
            cursor1.TrackPoint.Color2 = Colors.Transparent;
            cursor1.TrackPoint.Shape = Shape.Circle;
            _chart.ViewXY.LineSeriesCursors.Add(cursor1);

            _chart.EndUpdate();
        }

        /// <summary>
        /// 公开get属性
        /// </summary>
        public LightningChartUltimate Chart
        {
            get
            {
                return _chart;
            }
        }

        /// <summary>
        /// 图表初始化
        /// </summary>
        /// <param name="samplingFrequency">采样频率</param>
        /// <param name="title">示波器标题</param>
        /// <param name="scrollMode">示波器缩放模式</param>
        /// <param name="xAxisLen">x轴长度</param>
        public void Initialize(double samplingFrequency, string title,
            XAxisScrollMode scrollMode, double xAxisLen)
        {
            _samplingFrequency = samplingFrequency;
            _chart.BeginUpdate();

            _chart.Title.Text = title;
            _chart.ViewXY.SampleDataSeries[0].Clear();
            _chart.ViewXY.SampleDataSeries[0].SamplingFrequency = samplingFrequency;
            _chart.ViewXY.SampleDataSeries[0].FirstSampleTimeStamp = 0;

            _chart.ViewXY.ZoomPanOptions.MouseWheelZooming = MouseWheelZooming.Off;
            _chart.ViewXY.ZoomPanOptions.LeftMouseButtonAction = MouseButtonAction.None;

            _chart.ViewXY.XAxes[0].ScrollMode = scrollMode;
            _chart.ViewXY.XAxes[0].ScrollPosition = 0;
            _chart.ViewXY.XAxes[0].SetRange(0, xAxisLen);
            _chart.ViewXY.XAxes[0].Title.Text = string.Format("{0} s", xAxisLen.ToString("0.000"));
            _chart.ViewXY.DropOldSeriesData = true;

            _chart.ViewXY.LineSeriesCursors[0].Visible = false;

            _chart.EndUpdate();
        }

        /// <summary>
        /// 示波器自适应缩放
        /// </summary>
        public void FitView()
        {
            bool scaleChanged = false;
            _chart.ViewXY.YAxes[0].Fit(0.0, out scaleChanged, true, false);
        }

        /// <summary>
        /// 清空示波器数据
        /// </summary>
        public void Dispose()
        {
            if (_chart != null)
            {
                _parentControl.Children.Remove(_chart);

                _chart.Dispose();
                _chart = null;
            }
        }

        /// <summary>
        /// 设置x轴缩放
        /// </summary>
        /// <param name="factor">放大倍数</param>
        public void SetXLenZoom(double factor)
        {
            _chart.BeginUpdate();
            double lengthX = (_chart.ViewXY.XAxes[0].Maximum - _chart.ViewXY.XAxes[0].Minimum) * factor;

            _chart.ViewXY.XAxes[0].SetRange(_chart.ViewXY.XAxes[0].Maximum - lengthX,
                _chart.ViewXY.XAxes[0].Maximum);

            _chart.ViewXY.XAxes[0].Title.Text = string.Format("{0} s", lengthX.ToString("0.000"));
            _chart.EndUpdate();
        }

        /// <summary>
        /// 设置y轴缩放
        /// </summary>
        /// <param name="factor">放大倍数</param>
        public void SetYLenZoom(double factor)
        {
            _chart.BeginUpdate();
            double dYLen = (_chart.ViewXY.YAxes[0].Maximum - _chart.ViewXY.YAxes[0].Minimum) * factor;

            _chart.ViewXY.YAxes[0].SetRange(_chart.ViewXY.YAxes[0].Minimum * factor, _chart.ViewXY.YAxes[0].Maximum * factor);

            _chart.EndUpdate();
        }

        public void Stop()
        {
            _chart.BeginUpdate();

            _chart.ViewXY.ZoomPanOptions.MouseWheelZooming = MouseWheelZooming.Horizontal;
            _chart.ViewXY.XAxes[0].ScrollMode = XAxisScrollMode.None;
            _chart.ViewXY.DropOldSeriesData = false;
            _chart.ViewXY.ZoomPanOptions.LeftMouseButtonAction = MouseButtonAction.Zoom;

            //put cursor to center of x axis 
            _chart.ViewXY.LineSeriesCursors[0].Visible = true;
            _chart.ViewXY.LineSeriesCursors[0].ValueAtXAxis = (_chart.ViewXY.XAxes[0].Minimum + _chart.ViewXY.XAxes[0].Maximum) / 2.0;

            _chart.EndUpdate();
        }

        /// <summary>
        /// 设置示波器缩放模式
        /// </summary>
        /// <param name="scrollMode">示波器缩放模式</param>
        public void SetScrollMode(XAxisScrollMode scrollMode)
        {
            _chart.ViewXY.XAxes[0].ScrollMode = scrollMode;
        }

        /// <summary>
        /// 更新示波器数据
        /// </summary>
        /// <param name="samples">数据</param>
        public void FeedData(double[] samples)
        {
            //Only accept resolution count of data points 
            if (samples == null)
                return;

            _chart.BeginUpdate();

            SampleDataSeries series = _chart.ViewXY.SampleDataSeries[0];
            series.AddSamples(samples, false);
            _chart.ViewXY.XAxes[0].ScrollPosition = series.FirstSampleTimeStamp + (double)(series.PointCount - 1) / _samplingFrequency;

            _chart.EndUpdate();
        }

        /// <summary>
        /// 设置边框
        /// </summary>
        /// <param name="x"></param>
        /// <param name="y"></param>
        /// <param name="width"></param>
        /// <param name="height"></param>
        public void SetBounds(int x, int y, int width, int height)
        {
            _chart.Margin = new Thickness(x, y, 0, 0);
            _chart.Width = width;
            _chart.Height = height;
        }
    }
}
