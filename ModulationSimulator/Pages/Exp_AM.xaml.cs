using DataManager.ToolBox.Modulator;
using DataManager.ToolBox.Transmitters;
using DataManager.ToolBox.MathTool;
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
        /// 信号波频率
        /// </summary>
        public double _signalwaveFrequency { get; set; }
        /// <summary>
        /// 载波频率
        /// </summary>
        public double _carrywaveFrequency { get; set; }
        #endregion

        #region waves
        private List<double> _signalWave { get; set; }
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
            _wavecheckPeriod = 150;
            _signalwaveFrequency = .3;
            _carrywaveFrequency = 15;
        }


        #region Events
        private void Click_MakeTransWave(object sender, RoutedEventArgs e)
        {
            _sendedWave = AmplitudeModulator.Modulate(_addedVoltage, _Ka, _signalWave, _carryWave);
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
            _signalWave = Electrical.Sin(_signalwaveFrequency);
            _carryWave = Electrical.Sin(_carrywaveFrequency);

            _chartCanvas.AddLineSeries("载波", _carryWave);
            _chartCanvas.AddLineSeries("信号", _signalWave);
        }

        private void Click_CheckWave(object sender, RoutedEventArgs e)
        {
            _checkedWave = AmplitudeModulator.DeModulate(_sendedWave, _wavecheckPeriod);
            _chartCanvas.AddLineSeries("检测波", _checkedWave);
        }

        private void Click_MakeSpectrum(object sender, RoutedEventArgs e)
        {
            //FourierTransform.Transform(_signalWave);
        }
        #endregion
    }
}
