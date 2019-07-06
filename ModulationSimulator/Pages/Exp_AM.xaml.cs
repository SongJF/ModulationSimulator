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
        
        public ChartCanvas _chartCanvas { get; set; }

        #region 可选参数
        /// <summary>
        /// 调制度
        /// </summary>
        public double _Ka { get; set; }
        /// <summary>
        /// 检波周期
        /// </summary>
        public double _wavecheckPeriod { get; set; }
        /// <summary>
        /// 外加直流分量
        /// </summary>
        public double _addedVoltage { get; set; }
        /// <summary>
        /// 发射波频率
        /// </summary>
        public double _transwaveFrequency { get; set; }
        /// <summary>
        /// 载波频率
        /// </summary>
        public double _carrywaveFrequency { get; set; }
        #endregion

        #region waves
        private List<double> _sourceWave { get; set; }
        private List<double> _carryWave { get; set; }
        private List<double> _sendedWave { get; set; }
        private List <double> _checkedWave { get; set; }
        #endregion

        public Exp_AM()
        {
            InitializeComponent();

            _chartCanvas = ChartCanvas;

            InitVaribles();

            DataContext = this;
        }

        private void InitVaribles()
        {
            _Ka = 1;
            _addedVoltage = 1;
            _wavecheckPeriod = 100;
            _transwaveFrequency = 1;
            _carrywaveFrequency = 6;
        }


        #region Events
        private void Click_MakeTransWave(object sender, RoutedEventArgs e)
        {
            _sendedWave = AmplitudeModulator.Modulate(_addedVoltage, _Ka, _sourceWave, _carryWave);
            _chartCanvas.AddLineSeries("发射波", _sendedWave);

            _chartCanvas.RemoveLineSeries("载波");
            _chartCanvas.RemoveLineSeries("信号");
        }

        private void Click_ClearChart(object sender, RoutedEventArgs e)
        {
            _chartCanvas.ClearChart();
        }

        private void Click_MakeSource(object sender, RoutedEventArgs e)
        {
            _sourceWave = Electrical.Sin(_transwaveFrequency);
            _carryWave = Electrical.Sin(_carrywaveFrequency);

            _chartCanvas.AddLineSeries("载波", _carryWave);
            _chartCanvas.AddLineSeries("信号", _sourceWave);
        }

        private void Click_CheckWave(object sender, RoutedEventArgs e)
        {
            _checkedWave = AmplitudeModulator.DeModulate(_sendedWave, _wavecheckPeriod);
            _chartCanvas.AddLineSeries("检测波", _checkedWave);
        }

        #endregion
    }
}
