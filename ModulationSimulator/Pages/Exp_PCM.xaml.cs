using ChartCanvas;
using ModulationSimulator.Components;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace ModulationSimulator.Pages
{
    /// <summary>
    /// Exp_PCM.xaml 的交互逻辑
    /// </summary>
    public partial class Exp_PCM : UserControl
    {
        public Chart_PCM _ExpPCM { get; set; }

        private MusicPlayer _MusicPlayer;

        public Exp_PCM()
        {
            InitializeComponent();

            _ExpPCM = Exp;

            DataContext = this;

            _MusicPlayer = new MusicPlayer();

            //销毁时释放资源
            Dispatcher.ShutdownStarted += (object sender, EventArgs e) =>
            {
                Dispose();
            };
        }

        private void Dispose()
        {
            _MusicPlayer.Dispose();
            _MusicPlayer = null;
        }

        #region 绑定事件
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
        #endregion
    }
}
