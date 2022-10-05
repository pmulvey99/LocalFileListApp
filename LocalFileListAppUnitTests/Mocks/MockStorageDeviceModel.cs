using LocalFileListApp.Models;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;

namespace LocalFileListAppUnitTests.Mocks
{
    internal class MockStorageDeviceModel : IStorageDeviceModel
    {
        public DriveInfo? Info => null;

        public long AvailableFreeSpace { get; set; }

        public long TotalSize { get; set; }

        public long TotalFileCount { get; set; }
        public long TotalDirectoryCount { get; set; }

        public bool IsBusy { get; set; }

        public ObservableCollection<IFileItemModel> FileItems { get; set; } =  new ObservableCollection<IFileItemModel>();

        public string CurrentScanDirectory { get; set; } = String.Empty;

        public double CurrentScanDirectoryProgress { get; set; }

        public string ScanDuration { get; set; } = string.Empty;

        public event PropertyChangedEventHandler? PropertyChanged;

        public void ClearFileContents()
        {
            throw new NotImplementedException();
        }

        public Task StartReadingFileContentsAsync()
        {
            this.CurrentScanDirectory = "C:\\TestFolder";
            OnPropertyChanged("CurrentScanDirectory");
            this.CurrentScanDirectoryProgress = 0.5;
            OnPropertyChanged("CurrentScanDirectoryProgress");
            this.ScanDuration = "0:16";
            OnPropertyChanged("ScanDuration");

            return Task.CompletedTask;
        }

        public void StopReadingFileContents()
        {
            throw new NotImplementedException();
        }

        public virtual void OnPropertyChanged(string propertyName)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }
}
