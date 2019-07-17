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

        public Exp_AM()
        {
            InitializeComponent();

            DataContext = this;
        }


        #region Events
        private void Click_MakeTransWave(object sender, RoutedEventArgs e)
        {
        }

        private void Click_ClearChart(object sender, RoutedEventArgs e)
        {
        }

        private void Click_MakeSource(object sender, RoutedEventArgs e)
        {
        }

        private void Click_CheckWave(object sender, RoutedEventArgs e)
        {
        }

        private void Click_MakeSpectrum(object sender, RoutedEventArgs e)
        {
            //FourierTransform.Transform(_signalWave);
        }
        #endregion
    }
}
