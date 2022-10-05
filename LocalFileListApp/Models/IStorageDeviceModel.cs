using System.Collections.ObjectModel;
using System.ComponentModel;
using System.IO;
using System.Threading;
using System.Threading.Tasks;

namespace LocalFileListApp.Models
{
    internal interface IStorageDeviceModel : INotifyPropertyChanged
    {
        DriveInfo? Info { get; }
        long AvailableFreeSpace { get; }
        long TotalSize { get; }
        long TotalFileCount { get; }
        long TotalDirectoryCount { get; }
        string CurrentScanDirectory { get; }
        double CurrentScanDirectoryProgress { get; }
        string ScanDuration { get; }
        bool IsBusy { get; }
        ObservableCollection<IFileItemModel> FileItems { get; }
        Task StartReadingFileContentsAsync();
        void StopReadingFileContents();
        void ClearFileContents();
    }
}