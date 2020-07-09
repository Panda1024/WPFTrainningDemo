using System.Windows;
using TranningDemo.ViewModel;

namespace TranningDemo.View
{
    /// <summary>
    /// MainWindowView.xaml 的交互逻辑
    /// </summary>
    public partial class MainWindowView : Window
    {
        public MainWindowView(string dataSourceMode, string openFileName)
        {
            InitializeComponent();
            this.DataContext = new MainWindowViewModel(dataSourceMode, openFileName);
        }
    }
}
