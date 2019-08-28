using Arction.Wpf.SemibindableCharting;
using Arction.Wpf.SemibindableCharting.Axes;
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
    public class AreaSpectrumMonitor
    {
        [System.Runtime.InteropServices.DllImport("user32")]
        static extern Boolean SetWindowPos(
            IntPtr hWnd,
            IntPtr hWndInsertAfter,
            Int32 x,
            Int32 y,
            Int32 cx,
            Int32 cy,
            UInt32 uFlags
        );

        private LightningChartUltimate _chart;
        private Int32 m_iResolution;

        [Obsolete]
        public AreaSpectrumMonitor(
            Panel parentControl,
            Int32 resolution,
            Double xAxisMax,
            String title,
            Color lineColor
        )
        {
            m_iResolution = resolution;

            _chart = new LightningChartUltimate();
            _chart.ChartName = "Area spectrum chart";
            _chart.ViewXY.XAxes = ViewXY.CreateDefaultXAxes();
            _chart.ViewXY.YAxes = ViewXY.CreateDefaultYAxes();
            _chart.VerticalAlignment = VerticalAlignment.Top;
            _chart.HorizontalAlignment = HorizontalAlignment.Left;

            _chart.BeginUpdate();

            _chart.Title.Visible = true;
            _chart.Title.Text = title;
            _chart.Title.Color = lineColor;
            _chart.Title.Font = new WpfFont("Segoe UI", 13, true, false);
            _chart.Title.Offset.SetValues(0, 20);
            _chart.ChartBackground.Color = ChartTools.CalcGradient(lineColor, Colors.Black, 65);
            _chart.ChartBackground.GradientDirection = 0;
            _chart.ChartBackground.GradientFill = GradientFill.Cylindrical;
            _chart.ViewXY.GraphBackground.GradientDirection = 270;
            _chart.ViewXY.GraphBackground.GradientFill = GradientFill.Linear;
            _chart.ViewXY.ZoomPanOptions.ZoomRectLine.Color = Colors.White;
            _chart.ViewXY.GraphBordersOverSeries = false;
            _chart.ViewXY.AxisLayout.YAxesLayout = YAxesLayout.Layered;
            _chart.ViewXY.AxisLayout.AutoAdjustMargins = false;

            Color color = _chart.ViewXY.GraphBackground.Color;
            _chart.ViewXY.GraphBackground.Color = Color.FromArgb(150, color.R, color.G, color.B);

            AxisX axisX = _chart.ViewXY.XAxes[0];
            axisX.SetRange(0, xAxisMax);
            axisX.Title.Font = new WpfFont("Segoe UI", 13, true, false);
            axisX.Title.Visible = true;
            axisX.Title.Text = "Frequency (Hz)";
            axisX.Units.Visible = false;
            axisX.ValueType = AxisValueType.Number;
            axisX.Position = 100;
            axisX.LabelsPosition = Alignment.Far;
            axisX.MajorDivTickStyle.Alignment = Alignment.Far;
            axisX.MinorDivTickStyle.Alignment = Alignment.Far;
            axisX.MajorDivTickStyle.Color = Colors.Gray;
            axisX.MinorDivTickStyle.Color = Colors.DimGray;
            axisX.LabelsColor = Colors.White;
            axisX.LabelsFont = new WpfFont("Segoe UI", 11, true, false);
            axisX.ScrollMode = XAxisScrollMode.None;
            axisX.Title.Visible = false;

            AxisY axisY = _chart.ViewXY.YAxes[0];
            axisY.MajorDivTickStyle.Color = Colors.Gray;
            axisY.MinorDivTickStyle.Color = Colors.DimGray;
            axisY.AutoFormatLabels = false;
            axisY.LabelsNumberFormat = "0";
            axisY.SetRange(0, 7000000);
            axisY.Title.Visible = false;
            axisY.LabelsColor = Colors.White;
            axisY.LabelsFont = new WpfFont("Segoe UI", 11, true, false);
            axisY.Units.Visible = false;
            axisY.Visible = false;

            AreaSeries areaSeries = new AreaSeries(_chart.ViewXY, axisX, axisY);
            areaSeries.Title.Visible = false;
            areaSeries.LineStyle.Color = lineColor;
            areaSeries.LineStyle.Width = 1f;
            areaSeries.Fill.Color = ChartTools.CalcGradient(lineColor, Colors.Black, 50);
            areaSeries.Fill.GradientFill = GradientFill.Solid;
            areaSeries.MouseInteraction = false;
            areaSeries.PointsVisible = false;

            _chart.ViewXY.AreaSeries.Add(areaSeries);

            _chart.EndUpdate();

            _chart.ViewXY.ZoomPanOptions.MouseWheelZooming = MouseWheelZooming.Off;

            parentControl.Children.Add(_chart);
        }

        public LightningChartUltimate Chart
        {
            get
            {
                return _chart;
            }
        }

        public void FitView()
        {
            _chart.ViewXY.ZoomToFit();
        }

        public void Dispose()
        {
            if (_chart != null)
            {
                _chart.Dispose();
                _chart = null;
            }
        }

        public void SetData(Double[] xValues, Double[] yValues)
        {
            // Only accept resolution count of data points.

            if (xValues.Length > m_iResolution)
            {
                Array.Resize(ref xValues, m_iResolution);
            }

            if (yValues.Length > m_iResolution)
            {
                Array.Resize(ref yValues, m_iResolution);
            }

            _chart.BeginUpdate();

            // Set data to area series.

            Int32 iPointCount = xValues.Length;
            AreaSeriesPoint[] aPoints = new AreaSeriesPoint[iPointCount];
            for (Int32 i = 0; i < iPointCount; i++)
            {
                aPoints[i].X = xValues[i];
                aPoints[i].Y = yValues[i];
            }

            _chart.ViewXY.AreaSeries[0].Points = aPoints;

            _chart.EndUpdate();
        }

        public void SetBounds(Double x, Double y, Double width, Double height)
        {
            _chart.Margin = new Thickness(x, y, 0.0, 0.0);
            _chart.Width = width;
            _chart.Height = height;
        }
    }
}
