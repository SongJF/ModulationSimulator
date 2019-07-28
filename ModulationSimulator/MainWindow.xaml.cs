using MaterialDesignThemes.Wpf;
using ModulationSimulator.Domain;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls.Primitives;
using System.Windows.Input;
using System.Windows.Media;

namespace ModulationSimulator
{
    /// <summary>
    /// MainWindow.xaml 的交互逻辑
    /// </summary>
    public partial class MainWindow : Window
    {
        //设置主悬浮条
        public static Snackbar _MainSnakeBar;
        public MainWindow()
        {
            InitializeComponent();

            _MainSnakeBar = this.MainSnackbar;

            //数据绑定
            DataContext = new MainWindowViewModel(InitSnakeBar());

        }

        /// <summary>
        /// 初始化SnakeBar并欢迎
        /// </summary>
        private SnackbarMessageQueue InitSnakeBar()
        {
            Task.Factory.StartNew(() =>
            {
                Thread.Sleep(1000);
            }).ContinueWith(t =>
            {
                //note you can use the message queue from any thread, but just for the demo here we 
                //need to get the message queue from the snackbar, so need to be on the dispatcher
                MainSnackbar.MessageQueue.Enqueue(Properties.Resources.SnakeMSG_Welcome);
            }, TaskScheduler.FromCurrentSynchronizationContext());

            return MainSnackbar.MessageQueue;
        }

        #region Click事件
        /// <summary>
        /// 窗口关闭
        /// </summary>
        private void MainDialogClosing(object sender, DialogClosingEventArgs eventArgs)
        {
            if (!Equals(eventArgs.Parameter, true)) return;
            Application.Current.Shutdown();
        }

        private void MenuListBox_PreviewMouseLeftButtonUp(object sender, MouseButtonEventArgs e)
        {
            //until we had a StaysOpen glag to Drawer, this will help with scroll bars
            var dependencyObject = Mouse.Captured as DependencyObject;
            while (dependencyObject != null)
            {
                if (dependencyObject is ScrollBar) return;
                dependencyObject = VisualTreeHelper.GetParent(dependencyObject);
            }

            MenuToggleButton.IsChecked = false;
        }

        /// <summary>
        /// 拖动窗口
        /// </summary>
        private void Drag_Window(object sender, MouseButtonEventArgs e)
        {
            this.DragMove();
        }
        #endregion
    }
}
