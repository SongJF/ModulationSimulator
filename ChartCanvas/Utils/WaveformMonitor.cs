﻿using Arction.Wpf.SemibindableCharting;
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
        /// <param name="seriesNames">示波器各序列名</param>
        /// <param name="sampleFrequency">绘制曲线颜色</param>
        /// <param name="titleColor">绘制曲线颜色</param>
        /// <param name="chartManager"> 内置图表的的chartManager(可空)</param>
        public WaveformMonitor(
            Panel parentControl,
            string[] seriesNames,
            double sampleFrequency,
            Color titleColor,
            ChartManager chartManager
        )
        {
            _samplingFrequency = sampleFrequency;
            _parentControl = parentControl;

            _chart = new LightningChartUltimate();
            _chart.ChartName = "Waveform chart";
            _chart.ViewXY.XAxes = ViewXY.CreateDefaultXAxes();
            _chart.ViewXY.YAxes = ViewXY.CreateDefaultYAxes();
            _chart.VerticalAlignment = VerticalAlignment.Top;
            _chart.HorizontalAlignment = HorizontalAlignment.Left;

            _chart.ViewXY.AxisLayout.YAxesLayout = YAxesLayout.Stacked;
            _chart.ViewXY.AxisLayout.SegmentsGap = 10;

            parentControl.Children.Add(_chart);

            _chart.BeginUpdate();
            _chart.ChartManager = chartManager;

            _chart.ViewXY.AxisLayout.AutoAdjustMargins = false;

            _chart.ViewXY.DropOldSeriesData = true;
            _chart.ChartRenderOptions.AntiAliasLevel = 0; // Disable hw anti-aliasing.

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

            //AxisY axisY = _chart.ViewXY.YAxes[0];
            //axisY.SetRange(-30000, 30000);
            //axisY.Title.Visible = false;
            //axisY.LabelsFont = new WpfFont("Segoe UI", 11, true, false);

            _chart.ViewXY.GraphBackground.GradientDirection = 270;
            _chart.ViewXY.GraphBackground.GradientFill = GradientFill.Cylindrical;

            Color color = _chart.ViewXY.GraphBackground.Color;
            _chart.ViewXY.GraphBackground.Color = Color.FromArgb(150, color.R, color.G, color.B);

            _chart.Title.Font = new WpfFont("Segoe UI", 14, true, false);

            _chart.Title.Align = ChartTitleAlignment.TopCenter;
            _chart.Title.Offset.SetValues(0, 25);
            _chart.Title.Color = titleColor;

            _chart.ViewXY.Margins = new Thickness(70, 10, 15, 10);
            _chart.ViewXY.ZoomPanOptions.ZoomRectLine.Color = Colors.Lime;

            _chart.ChartBackground.Color = ChartTools.CalcGradient(titleColor, Colors.Black, 65);
            _chart.ChartBackground.GradientDirection = 0;
            _chart.ChartBackground.GradientFill = GradientFill.Cylindrical;

            //清除之前的y轴与数据序列
            DisposeAllAndClear(_chart.ViewXY.YAxes);
            DisposeAllAndClear(_chart.ViewXY.SampleDataSeries);
            //添加多序列的y轴属性
            for (int index = 0; index < seriesNames.Count(); index++)
            {
                AxisY axisY = new AxisY(_chart.ViewXY);
                axisY.SetRange(-30000, 30000);
                axisY.Title.Font = new WpfFont("Segoe UI", 10, false, false);
                axisY.Title.Text = string.Format(seriesNames[index]);
                axisY.Title.Angle = 0;
                axisY.Units.Visible = false;
                //axisY.Title.Visible = false;
                axisY.LabelsFont = new WpfFont("Segoe UI", 11, true, false);
                _chart.ViewXY.YAxes.Add(axisY);

                //Add SampleDataSeries
                SampleDataSeries sds = new SampleDataSeries(_chart.ViewXY, axisX, axisY);
                _chart.ViewXY.SampleDataSeries.Add(sds);
                sds.SampleFormat = SampleFormat.DoubleFloat;
                sds.LineStyle.Color = DefaultColors.SeriesForBlackBackgroundWpf[index % DefaultColors.SeriesForBlackBackgroundWpf.Length];
                sds.SamplingFrequency = _samplingFrequency;
                sds.FirstSampleTimeStamp = 1.0 / _samplingFrequency;
                sds.LineStyle.Width = 1f;
                sds.LineStyle.AntiAliasing = LineAntialias.None;
                sds.ScrollModePointsKeepLevel = 1;
                sds.ScrollingStabilizing = true;
                sds.MouseInteraction = false;



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
            }

            //LineSeriesCursor cursor1 = new LineSeriesCursor(_chart.ViewXY, axisX);
            //cursor1.ValueAtXAxis = 1;
            //cursor1.LineStyle.Width = 6;

            //color = Colors.OrangeRed;
            //cursor1.LineStyle.Color = Color.FromArgb(180, color.R, color.G, color.B);

            //cursor1.FullHeight = true;
            //cursor1.SnapToPoints = true;
            //cursor1.Style = CursorStyle.PointTracking;
            //cursor1.TrackPoint.Color1 = Colors.Yellow;
            //cursor1.TrackPoint.Color2 = Colors.Transparent;
            //cursor1.TrackPoint.Shape = Shape.Circle;
            //_chart.ViewXY.LineSeriesCursors.Add(cursor1);

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

            _chart.ViewXY.ZoomPanOptions.MouseWheelZooming = MouseWheelZooming.Horizontal;
            _chart.ViewXY.ZoomPanOptions.LeftMouseButtonAction = MouseButtonAction.None;

            _chart.ViewXY.XAxes[0].ScrollMode = scrollMode;
            _chart.ViewXY.XAxes[0].ScrollPosition = 0;
            _chart.ViewXY.XAxes[0].SetRange(0, xAxisLen);
            _chart.ViewXY.XAxes[0].Title.Text = string.Format("{0} s", xAxisLen.ToString("0.000"));
            _chart.ViewXY.DropOldSeriesData = true;

            //_chart.ViewXY.LineSeriesCursors[0].Visible = false;

            _chart.EndUpdate();
        }

        /// <summary>
        /// 示波器自适应缩放
        /// </summary>
        public void FitView()
        {
            foreach(var item in _chart.ViewXY.YAxes)
            {
                bool scaleChanged = false;
                item.Fit(0.0,out scaleChanged, true, false);
            }
            //bool scaleChanged = false;
            //_chart.ViewXY.YAxes[0].Fit(0.0, out scaleChanged, true, false);
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
            foreach(var item in _chart.ViewXY.YAxes)
            {
                item.SetRange(item.Minimum * factor, item.Maximum * factor);
            }
            //double dYLen = (_chart.ViewXY.YAxes[0].Maximum - _chart.ViewXY.YAxes[0].Minimum) * factor;

            //_chart.ViewXY.YAxes[0].SetRange(_chart.ViewXY.YAxes[0].Minimum * factor, _chart.ViewXY.YAxes[0].Maximum * factor);

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
        public void FeedData(double[][] samples)
        {
            //Only accept resolution count of data points 
            if (samples == null)
                return;

            _chart.BeginUpdate();

            for (int index = 0; index < samples.Count(); index++)
            {
                if (samples[index] != null)
                {
                    //SampleDataSeries series = _chart.ViewXY.SampleDataSeries[index];
                    //series.AddSamples(samples[index], false);
                    SampleDataSeries series = _chart.ViewXY.SampleDataSeries[index];
                    var data = samples[index];
                    series.AddSamples(data, true);
                    //每次更新数据x轴只滚动一次
                    if (index == 0)
                    {
                        _chart.ViewXY.XAxes[0].ScrollPosition = series.FirstSampleTimeStamp +
                       (double)(series.PointCount - 1) / _samplingFrequency;
                    }
                }

            }
            

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

        /// <summary>
        /// 释放并清空传入的列表
        /// </summary>
        /// <typeparam name="T">列表类型</typeparam>
        /// <param name="list">欲清空的列表</param>
        private static void DisposeAllAndClear<T>(System.Windows.FreezableCollection<T> list) where T : System.Windows.Freezable
        {
            if (list == null)
                return;

            while (list.Count > 0)
            {
                int lastInd = list.Count - 1;
                T item = list[lastInd]; // take item ref from list. 
                list.RemoveAt(lastInd); // remove item first
                if (item != null)
                    (item as IDisposable).Dispose();     // then dispose it. 
            }


        }
    }
}
