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

        #region 对象声明
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

        private void Dispose()
        {
            _MusicPlayer.Dispose();
            _MusicPlayer = null;
        }

        #region 控件事件
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

        

        private void Click_SaveWave(object sender, RoutedEventArgs e)
        {
            try
            {
                _Exp.SaveImage(ImageSaveMode.Wave);
            }
            catch(Exception ex)
            {
                MainWindow._MainSnakeBar.MessageQueue.Enqueue("发生错误: " + ex.Message);
            }
        }

        private void Click_SaveSourceSpectrograms(object sender, RoutedEventArgs e)
        {
            try
            {
                _Exp.SaveImage(ImageSaveMode.SourceSpectrograms);
            }
            catch (Exception ex)
            {
                MainWindow._MainSnakeBar.MessageQueue.Enqueue("发生错误: " + ex.Message);
            }
        }

        private void Click_SaveSignalSpectrograms(object sender, RoutedEventArgs e)
        {
            try
            {
                _Exp.SaveImage(ImageSaveMode.SignalSpectrograms);
            }
            catch (Exception ex)
            {
                MainWindow._MainSnakeBar.MessageQueue.Enqueue("发生错误: " + ex.Message);
            }
        }
        #endregion
    }
}
