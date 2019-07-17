using Arction.Wpf.SemibindableCharting;
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
    /// 频谱仪类
    /// </summary>
    internal sealed class SpectrogramViewXYIntensity
    {
        LightningChartUltimate _chart = null;
        IntensityGridSeries _grid = null;
        Panel _parentControl;
        double[][] m_aFastData;
        double m_dStepTime = 0;
        double m_dCurrentTime = 0;
        bool m_bIsHorizontalScrolling = true;
        int m_iSizeTimeSlots;
        int m_iSizeResolution;
        double m_dTimeRangeLengthSec;

        /// <summary>
        /// 构造
        /// </summary>
        /// <param name="parentControl">父控件</param>
        /// <param name="verticalScrolling">是否可以垂直滚动</param>
        /// <param name="resolution">分辨率</param>
        /// <param name="timeStepMs"></param>
        /// <param name="timeRangeLengthSec"></param>
        /// <param name="freqMin">最小频率</param>
        /// <param name="freqMax">最大频率</param>
        /// <param name="title">频谱仪标题(未使用)</param>
        /// <param name="toneColor"></param>
        public SpectrogramViewXYIntensity(Panel parentControl, bool verticalScrolling,
            int resolution, double timeStepMs, double timeRangeLengthSec,
            double freqMin, double freqMax, string title, Color toneColor)
        {
            _parentControl = parentControl;

            double dDefaultYMax = 7000000;

            double dAxisTimeScaleMin = timeStepMs / 1000.0 - timeRangeLengthSec;
            double dAxisTimeScaleMax = 0;
            m_dStepTime = timeStepMs / 1000.0;
            m_dTimeRangeLengthSec = timeRangeLengthSec;

            //Create chart 
            LightningChartUltimate chart = new LightningChartUltimate();

            _chart = chart;
            _chart.VerticalAlignment = VerticalAlignment.Top;
            _chart.HorizontalAlignment = HorizontalAlignment.Left;

            m_bIsHorizontalScrolling = !verticalScrolling;

            chart.BeginUpdate();

            parentControl.Children.Add(chart);

            _chart.ChartBackground.Color = ChartTools.CalcGradient(toneColor, Colors.Black, 65);
            _chart.ChartBackground.GradientDirection = 0;
            _chart.ChartBackground.GradientFill = GradientFill.Cylindrical;

            chart.ChartName = "Spectrogram chart";
            chart.ViewXY.XAxes = ViewXY.CreateDefaultXAxes();
            chart.ViewXY.YAxes = ViewXY.CreateDefaultYAxes();

            chart.Title.Visible = false;
            chart.Title.Font = new WpfFont("Segoe UI", 13, true, false);
            chart.Title.Offset.SetValues(0, 20);
            chart.ViewXY.ZoomPanOptions.RightMouseButtonAction = MouseButtonAction.None;
            chart.ViewXY.ZoomPanOptions.LeftMouseButtonAction = MouseButtonAction.None;

            //Disable automatic axis layouts 
            chart.ViewXY.AxisLayout.AutoAdjustMargins = false;
            chart.ViewXY.AxisLayout.XAxisAutoPlacement = XAxisAutoPlacement.Off;
            chart.ViewXY.AxisLayout.YAxisAutoPlacement = YAxisAutoPlacement.Off;
            chart.ViewXY.AxisLayout.XAxisTitleAutoPlacement = false;
            chart.ViewXY.AxisLayout.YAxisTitleAutoPlacement = false;
            chart.ViewXY.Margins = new Thickness(60, 10, 15, 50);

            if (m_bIsHorizontalScrolling)
            {
                chart.ViewXY.XAxes[0].ValueType = AxisValueType.Time;
                chart.ViewXY.XAxes[0].Title.Text = "Time";
                chart.ViewXY.XAxes[0].SetRange(dAxisTimeScaleMin, dAxisTimeScaleMax);

                chart.ViewXY.YAxes[0].ValueType = AxisValueType.Number;
                chart.ViewXY.YAxes[0].Title.Text = "Frequency(Hz)";
                chart.ViewXY.YAxes[0].SetRange(freqMin, freqMax);
            }
            else
            {

                chart.ViewXY.XAxes[0].ValueType = AxisValueType.Number;
                chart.ViewXY.XAxes[0].Title.Text = "Frequency(Hz)";
                chart.ViewXY.XAxes[0].SetRange(freqMin, freqMax);

                chart.ViewXY.YAxes[0].ValueType = AxisValueType.Time;
                chart.ViewXY.YAxes[0].Title.Text = "Time";
                chart.ViewXY.YAxes[0].SetRange(dAxisTimeScaleMin, dAxisTimeScaleMax);
            }

            m_dCurrentTime = dAxisTimeScaleMax;

            Color color = Colors.White;

            chart.ViewXY.XAxes[0].MinorDivTickStyle.Visible = false;
            chart.ViewXY.XAxes[0].LabelsColor = Color.FromArgb(200, color.R, color.G, color.B);
            chart.ViewXY.XAxes[0].MajorDivTickStyle.Color = Colors.Orange;
            chart.ViewXY.XAxes[0].Title.Shadow.Style = TextShadowStyle.Off;
            chart.ViewXY.XAxes[0].LabelsNumberFormat = "0";

            chart.ViewXY.YAxes[0].MinorDivTickStyle.Visible = false;
            chart.ViewXY.YAxes[0].LabelsColor = Color.FromArgb(200, color.R, color.G, color.B);
            chart.ViewXY.YAxes[0].MajorDivTickStyle.Color = Colors.Orange;
            chart.ViewXY.YAxes[0].Title.Shadow.Style = TextShadowStyle.Off;
            chart.ViewXY.YAxes[0].LabelsNumberFormat = "0";

            //Setup legend box
            chart.ViewXY.LegendBoxes = ViewXY.CreateDefaultLegendBoxes();
            chart.ViewXY.LegendBoxes[0].SeriesTitleColor = toneColor;
            chart.ViewXY.LegendBoxes[0].ValueLabelColor = Colors.White;
            chart.ViewXY.LegendBoxes[0].IntensityScales.ScaleBorderColor = Colors.White;
            chart.ViewXY.LegendBoxes[0].Position = LegendBoxPositionXY.RightCenter;
            chart.ViewXY.LegendBoxes[0].Layout = LegendBoxLayout.Vertical;
            chart.ViewXY.LegendBoxes[0].Offset.SetValues(-20, 0);
            chart.ViewXY.LegendBoxes[0].Fill.Style = RectFillStyle.None;
            chart.ViewXY.LegendBoxes[0].Shadow.Visible = false;
            chart.ViewXY.LegendBoxes[0].BorderWidth = 0;
            chart.ViewXY.LegendBoxes[0].IntensityScales.ScaleSizeDim1 = 100;
            chart.ViewXY.LegendBoxes[0].IntensityScales.ScaleSizeDim2 = 15;
            chart.ViewXY.LegendBoxes[0].ShowCheckboxes = false;
            chart.ViewXY.LegendBoxes[0].UnitsColor = Colors.Transparent;
            chart.Name = "Spectrogram";

            //Create grid
            _grid = new IntensityGridSeries(chart.ViewXY, chart.ViewXY.XAxes[0], chart.ViewXY.YAxes[0]);
            chart.ViewXY.IntensityGridSeries.Add(_grid);

            m_iSizeTimeSlots = (int)Math.Round(timeRangeLengthSec / (timeStepMs / 1000.0));
            m_iSizeResolution = resolution;

            m_aFastData = new double[m_iSizeTimeSlots][];
            for (int iTimeSlot = 0; iTimeSlot < m_iSizeTimeSlots; iTimeSlot++)
            {
                m_aFastData[iTimeSlot] = new double[m_iSizeResolution];
            }
            if (m_bIsHorizontalScrolling)
                _grid.SetValuesData(m_aFastData, IntensityGridValuesDataOrder.ColumnsRows);
            else
                _grid.SetValuesData(m_aFastData, IntensityGridValuesDataOrder.RowsColumns);

            _grid.Data = null;

            _grid.SetRangesXY(chart.ViewXY.XAxes[0].Minimum, chart.ViewXY.XAxes[0].Maximum,
                chart.ViewXY.YAxes[0].Minimum, chart.ViewXY.YAxes[0].Maximum);
            _grid.ContourLineType = ContourLineTypeXY.None;
            _grid.WireframeType = SurfaceWireframeType.None;
            _grid.PixelRendering = true;
            _grid.MouseInteraction = false;
            _grid.ValueRangePalette = CreatePalette(_grid, dDefaultYMax);
            _grid.Title.Text = "P(f)";
            chart.EndUpdate();
        }

        /// <summary>
        /// 公开内置图表get属性
        /// </summary>
        public LightningChartUltimate Chart
        {
            get
            {
                return _chart;
            }
        }

        /// <summary>
        /// 清空表
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
        /// 频谱仪自适应
        /// </summary>
        public void FitView()
        {
            double minY = double.MaxValue;
            double maxY = double.MinValue;
            double y;

            _chart.BeginUpdate();
            if (_grid != null && m_aFastData != null)
            {
                for (int i = 0; i < m_iSizeTimeSlots; i++)
                {
                    for (int j = 0; j < m_iSizeResolution; j++)
                    {
                        y = m_aFastData[i][j];
                        if (y > maxY)
                            maxY = y;
                        if (y < minY)
                            minY = y;
                    }
                }
                _grid.ValueRangePalette = CreatePalette(_grid, maxY);
            }

            _chart.EndUpdate();

        }

        /// <summary>
        /// 输入数据
        /// </summary>
        /// <param name="yValuesData"></param>
        /// <param name="channelIndex">频道索引</param>
        /// <param name="rowCount"></param>
        public void SetData(double[][][] yValuesData, int channelIndex, int rowCount)
        {
            if (_chart == null)
                return;

            _chart.BeginUpdate();

            for (int row = 0; row < rowCount; row++)
            {
                double[] yValues = yValuesData[row][channelIndex];

                //Only accept resolution count of data points 
                Array.Resize(ref yValues, m_iSizeResolution);

                //move the old time slots one step earlier
                for (int iTimeSlot = 1; iTimeSlot < m_iSizeTimeSlots; iTimeSlot++)
                {
                    m_aFastData[iTimeSlot - 1] = m_aFastData[iTimeSlot]; //change the reference 
                }

                m_aFastData[m_iSizeTimeSlots - 1] = yValues;
            }

            _grid.InvalidateValuesDataOnly();

            double dCurrentTimeMin = m_dCurrentTime - m_dTimeRangeLengthSec;
            double dTotalTimeShift = m_dStepTime * (double)rowCount;

            if (m_bIsHorizontalScrolling)
                _chart.ViewXY.XAxes[0].SetRange(dCurrentTimeMin + dTotalTimeShift, dCurrentTimeMin + dTotalTimeShift + m_dTimeRangeLengthSec);
            else
                _chart.ViewXY.YAxes[0].SetRange(dCurrentTimeMin + dTotalTimeShift, dCurrentTimeMin + dTotalTimeShift + m_dTimeRangeLengthSec);

            _grid.SetRangesXY(_chart.ViewXY.XAxes[0].Minimum, _chart.ViewXY.XAxes[0].Maximum,
                _chart.ViewXY.YAxes[0].Minimum, _chart.ViewXY.YAxes[0].Maximum);

            m_dCurrentTime += dTotalTimeShift;

            _chart.EndUpdate();
        }

        /// <summary>
        /// 设置边线
        /// </summary>
        public void SetBounds(int x, int y, int width, int height)
        {
            _chart.Margin = new Thickness(x, y, 0, 0);
            _chart.Width = width;
            _chart.Height = height;
        }

        /// <summary>
        /// 创建强度多色阶调色板
        /// </summary>
        /// <param name="ownerSeries">父</param>
        /// <param name="yRange">y值范围</param>
        /// <returns></returns>
        private ValueRangePalette CreatePalette(IntensitySeriesBase ownerSeries, double yRange)
        {
            ValueRangePalette palette = new ValueRangePalette(ownerSeries);

            DisposeAllAndClear(palette.Steps);
            palette.Steps.Add(new PaletteStep(palette, Colors.Black, 0));
            palette.Steps.Add(new PaletteStep(palette, Colors.Lime, 30 * yRange / 100.0));
            palette.Steps.Add(new PaletteStep(palette, Colors.Yellow, 60.0 * yRange / 100.0));
            palette.Steps.Add(new PaletteStep(palette, Colors.Red, 100.0 * yRange / 100.0));
            palette.Type = PaletteType.Gradient;

            return palette;
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
