using System.Windows;
using System.Windows.Controls;

namespace ModulationSimulator.Pages
{
    /// <summary>
    /// Hello.xaml 的交互逻辑
    /// </summary>
    public partial class Hello : UserControl
    {
        public Hello()
        {
            InitializeComponent();
        }

        private void GotoGithub(object sender, RoutedEventArgs e)
        {
            //调用系统默认的浏览器 
            System.Diagnostics.Process.Start("https://github.com/SongJF/ModulationSimulator");
        }
    }
}
