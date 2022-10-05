using LocalFileListApp.Models;
using LocalFileListApp.ViewModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Controls.Ribbon;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace LocalFileListApp.Views
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : RibbonWindow
    {
        internal MainWindowViewModel? ViewModel { get; private set; } = null;

        public MainWindow()
        {
            InitializeComponent();

            IStorageDeviceManagerModel storageDeviceManager = new StorageDeviceManagerModel();
            this.ViewModel = new MainWindowViewModel(storageDeviceManager);
        }

        private void mnuExit_Click(object sender, RoutedEventArgs e)
        {
            Application.Current.Shutdown();
        }

        private void RibbonWindow_Loaded(object sender, RoutedEventArgs e)
        {
            if (null != this.ViewModel)
            {
                this.DataContext = this.ViewModel;
            }
        }

        private void RibbonWindow_Closing(object sender, System.ComponentModel.CancelEventArgs e)
        {
            this.ViewModel?.StopSelectedDeviceScan();
            this.ViewModel = null;
        }

        private async void btnStartScan_Click(object sender, RoutedEventArgs e)
        {
            if (null != this.ViewModel)
            {
                await this.ViewModel.StartSelectedDeviceScanAsync();
            }
        }

        private void btnStopScan_Click(object sender, RoutedEventArgs e)
        {
            if (null != this.ViewModel)
            {
                this.ViewModel.StopSelectedDeviceScan();
            }
        }
    }
}
