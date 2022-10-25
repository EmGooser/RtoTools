using RtoTools.Common;
using RtoTools.ViewModel;
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

namespace RtoTools
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        // The view models handle all the localization to make it easier to manage

        public MainWindow()
        {
            InitializeComponent();

            var vm = new MainWindowViewModel();

            AppController.Startup(this.Dispatcher, vm);

            this.DataContext = vm;

            AppController.Instance.AddEventLog("Applicaiton Loaded");
        }
    }
}
