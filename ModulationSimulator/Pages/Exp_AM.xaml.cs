using DrawChart;
using DrawChart.Model;
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
    /// Exp_AM.xaml 的交互逻辑
    /// </summary>
    public partial class Exp_AM : UserControl
    {
        public ChartCanvas _chartCanvas { get; set; }
        public Exp_AM()
        {
            InitializeComponent();

            _chartCanvas = ChartCanvas;

            DataContext = this;
        }

        private void Click_MakeTransWave(object sender, RoutedEventArgs e)
        {
            List<double> data = new List<double>();
            for (int i = 0; i < 256; i++)
            {
                data.Add(Math.Sin(Math.PI * i * 4 / 180));
            }
            _chartCanvas.AddLineSeries(Properties.Resources.Text_TransWave, data);
        }
    }
}
