using DataManager.ToolBox.Modulator;
using DataManager.ToolBox.Transmitters;
using DrawChart;
using System.Collections.Generic;
using System.Windows;
using System.Windows.Controls;

namespace ModulationSimulator.Pages
{
    /// <summary>
    /// Exp_AM.xaml 的交互逻辑
    /// </summary>
    public partial class Exp_AM : UserControl
    {
        //调制度
        public double _Ka { get; set; }
        public ChartCanvas _chartCanvas { get; set; }

        private List<double> _sourceWave { get; set; }
        private List<double> _carry_Wave { get; set; }
        public Exp_AM()
        {
            InitializeComponent();

            _chartCanvas = ChartCanvas;
            _Ka = 1;

            DataContext = this;
        }


        #region Events
        private void Click_MakeTransWave(object sender, RoutedEventArgs e)
        {
            _chartCanvas.AddLineSeries("发射波", AmplitudeModulator.Modulate(0, _Ka, _sourceWave, _carry_Wave));
        }
        #endregion

        private void Click_ClearChart(object sender, RoutedEventArgs e)
        {
            _chartCanvas.ClearChart();
        }

        private void Click_MakeSource(object sender, RoutedEventArgs e)
        {
            _sourceWave = Electrical.Sin(1);
            _carry_Wave = Electrical.Sin(6);

            _chartCanvas.AddLineSeries("载波", _carry_Wave);
            _chartCanvas.AddLineSeries("信号", _sourceWave);
        }

        private void Click_ClearSource(object sender, RoutedEventArgs e)
        {
            _chartCanvas.RemoveLineSeries("载波");
            _chartCanvas.RemoveLineSeries("信号");
        }
    }
}
