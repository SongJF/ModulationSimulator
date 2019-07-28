using ChartCanvas;
using ModulationSimulator.Components;
using System;
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

        public  Chart_AM _Exp { get; set; }

        private MusicPlayer _MusicPlayer;
        #endregion

        public Exp_AM()
        {
            InitializeComponent();

            _Exp = Exp;

            DataContext = this;

            _MusicPlayer = new MusicPlayer();

            //销毁时释放资源
            Dispatcher.ShutdownStarted += (object sender, EventArgs e) =>
            {
                Dispose();
            };
        }

        private void Click_96Hz(object sender, RoutedEventArgs e)
        {
            _MusicPlayer.Play(@"StaticResource/96hz_full.mp3");
        }

        private void Click_160to60Hz(object sender, RoutedEventArgs e)
        {
            _MusicPlayer.Play(@"StaticResource/160hz-60hz.mp3");
        }

        private void Click_Pause(object sender, RoutedEventArgs e)
        {
            _MusicPlayer.Pause();
        }

        private void Click_Piano(object sender, RoutedEventArgs e)
        {
            _MusicPlayer.Play(@"StaticResource/Piano.mp3");
        }

        private void Dispose()
        {
            _MusicPlayer.Dispose();
            _MusicPlayer = null;
        }
    }
}
