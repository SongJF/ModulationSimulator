using MaterialDesignColors;
using MaterialDesignThemes.Wpf;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows;
using System.Windows.Controls;

namespace ModulationSimulator.Pages
{
    /// <summary>
    /// Hello.xaml 的交互逻辑
    /// </summary>
    public partial class Hello : UserControl
    {
        public IEnumerable<Swatch> Swatches { get; }
        public Hello()
        {
            InitializeComponent();

            Swatches = new SwatchesProvider().Swatches;
        }

        private void GotoGithub(object sender, RoutedEventArgs e)
        {
            //调用系统默认的浏览器 
            System.Diagnostics.Process.Start("https://github.com/SongJF/ModulationSimulator");
        }

        private void Click_ChangeTheme(object sender, RoutedEventArgs e)
        {
            var button = sender as Button;

            var SwatchColor = Swatches.FirstOrDefault(p => p.Name == button.Tag.ToString());
            if (SwatchColor == null) return;

            try
            {
                new PaletteHelper().ReplacePrimaryColor(SwatchColor);
            }
            catch (Exception ex)
            {
                MainWindow.SnakeMessage("发生错误: " + ex.Message.ToString());
            }
        }
    }

    public enum ThemeColor
    {
        grey,
        blue,
        indigo,
        brown,
        deeppurple
    }
}
