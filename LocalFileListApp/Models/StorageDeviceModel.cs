using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Runtime;
using System.Runtime.CompilerServices;
using System.Security;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Data;
using System.Windows.Threading;
using static System.Net.WebRequestMethods;

namespace LocalFileListApp.Models
{
    /// <summary>
    /// Performs disk-based operations, currently reads disk contents via StartReadingFileContentsAsync
    /// which fills the collection FileItems.
    /// </summary>
    internal class StorageDeviceModel : IStorageDeviceModel, INotifyPropertyChanged
    {
        public DriveInfo? Info { get; private set; }

        public long AvailableFreeSpace
        {
            get { return _availableFreeSpace; }
            private set
            {
                _availableFreeSpace = value;
                OnPropertyChanged();
            }
        }
        private long _availableFreeSpace = 0;

        public long TotalSize
        {
            get { return _totalSize; }
            private set
            {
                _totalSize = value;
                OnPropertyChanged();
            }
        }
        private long _totalSize = 0;

        public long TotalFileCount
        {
            get => _totalFileCount;
            set
            {
                _totalFileCount = value;
                OnPropertyChanged();
            }
        }
        private long _totalFileCount = 0;

        public long TotalDirectoryCount
        {
            get => _totalDirectoryCount;
            set
            {
                _totalDirectoryCount = value;
                OnPropertyChanged();
            }
        }
        private long _totalDirectoryCount = 0;

        public string CurrentScanDirectory
        {
            get => _currentScanDirectory;
            set
            {
                _currentScanDirectory = value;
                OnPropertyChanged();
            }
        }
        private string _currentScanDirectory = string.Empty;

        public double CurrentScanDirectoryProgress
        {
            get => _currentScanDirectoryProgress;
            set
            {
                _currentScanDirectoryProgress = value;
                OnPropertyChanged();
            }
        }
        private double _currentScanDirectoryProgress;

        public string ScanDuration
        {
            get => _scanDuration;
            set
            {
                _scanDuration = value;
                OnPropertyChanged();
            }
        }
        private string _scanDuration = string.Empty;

        public bool IsBusy
        {
            get { return _isBusy; }
            private set
            {
                _isBusy = value;
                OnPropertyChanged();
            }
        }
        private bool _isBusy = false;

        public ObservableCollection<IFileItemModel> FileItems { get; private set; } = new ObservableCollection<IFileItemModel>();

        private CancellationTokenSource? CancellationTokenSource { get; set; } = null;

        private DispatcherTimer Timer { get; set; } = new DispatcherTimer();
        private Stopwatch ScanStopwatch { get; set; } = new Stopwatch();

        public event PropertyChangedEventHandler? PropertyChanged;

        /// <summary>
        /// 
        /// </summary>
        /// <param name="driveInfo"></param>
        /// <exception cref="ArgumentNullException"></exception>
        public StorageDeviceModel(in DriveInfo driveInfo)
        {
            this.Info = driveInfo ?? throw new ArgumentNullException(nameof(driveInfo));
            if (this.Info.IsReady)
            {
                this.AvailableFreeSpace = this.Info.AvailableFreeSpace;
                this.TotalSize = this.Info.TotalSize;
            }

            this.Timer.Tick += Timer_Tick;
            this.Timer.Interval = new TimeSpan(0, 0, 0, 0, 1000);
        }

        /// <summary>
        /// Timer is used to signal to the viewmodel to obtain a flat items collection via linq query on the hierarchical file collection.
        /// This operates on a low frequency, to keep the UI responsive. Potentially this timer belongs in the viewmodel...
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void Timer_Tick(object? sender, EventArgs e)
        {
            this.ScanDuration = this.ScanStopwatch.Elapsed.ToString("m\\:ss");
            OnPropertyChanged("FileItemsChanged");
        }

        /// <summary>
        /// Starts asynchronous reading of disk contents, and disposes of cancellation token when completed.
        /// </summary>
        /// <returns></returns>
        /// <exception cref="NullReferenceException"></exception>
        public async Task StartReadingFileContentsAsync()
        {
            if (null == this.Info)
            {
                throw new NullReferenceException("DriveInfo");
            }

            try
            {
                this.IsBusy = true;
                this.CancellationTokenSource = new CancellationTokenSource();

                ClearFileContents();

                this.ScanStopwatch.Restart();
                this.Timer.Start();

                await GetFileContentsAsync();
                Console.WriteLine("GetFileContentsAsync completed");
            }
            catch (OperationCanceledException e)
            {
                Console.WriteLine(e.Message);
            }
            finally
            {
                if (null != this.CancellationTokenSource)
                {
                    this.CancellationTokenSource.Dispose();
                    this.CancellationTokenSource = null;
                }

                this.ScanStopwatch.Stop();
                this.Timer.Stop();
                this.IsBusy = false;
            }
        }

        /// <summary>
        /// Requests cancellation of running task.
        /// </summary>
        public void StopReadingFileContents()
        {
            if (null != this.CancellationTokenSource)
            {
                this.CancellationTokenSource.Cancel();
            }
        }

        public void ClearFileContents()
        {
            this.FileItems.Clear();
            this.TotalFileCount = 0;
            this.TotalDirectoryCount = 0;
            this.CurrentScanDirectoryProgress = 0.0;
        }

        private async Task GetFileContentsAsync()
        {
            if (null == this.Info)
            {
                return;
            }

            Task rootDriveTask = Task.Run(() => WalkDirectoryTree(this.Info.RootDirectory, this.FileItems, true));
            await rootDriveTask;
        }

        private FileInfo[]? GetFiles(in DirectoryInfo directoryInfo)
        {
            FileInfo[]? files = null;

            try
            {
                files = directoryInfo.GetFiles("*.*");
            }
            catch (SecurityException e)
            {
                // Application does not provide sufficient permissions, to be expected.
                Console.WriteLine(e.Message);
            }
            catch (DirectoryNotFoundException e)
            {
                Console.WriteLine(e.Message);
            }

            return files;
        }

        private DirectoryInfo[]? GetDirectories(in DirectoryInfo directoryInfo)
        {
            DirectoryInfo[]? directories = null;

            try
            {
                directories = directoryInfo.GetDirectories();
            }
            catch (UnauthorizedAccessException e)
            {
                // Application does not provide sufficient permissions, to be expected.
                Console.WriteLine(e.Message);
            }
            catch (DirectoryNotFoundException e)
            {
                Console.WriteLine(e.Message);
            }

            return directories;
        }

        /// <summary>
        /// Inspired by https://learn.microsoft.com/en-us/dotnet/csharp/programming-guide/file-system/how-to-iterate-through-a-directory-tree
        /// Operates recursively. Could be further broken down into smaller functions -- out of time!
        /// </summary>
        /// <param name="root"></param>
        /// <param name="items"></param>
        /// <param name="isDrive"></param>
        /// <returns></returns>
        private async Task WalkDirectoryTree(DirectoryInfo root, ObservableCollection<IFileItemModel> items, bool isDrive = false)
        {
            this.CancellationTokenSource?.Token.ThrowIfCancellationRequested();

            FileInfo[]? files = null;
            DirectoryInfo[]? subDirs = null;

            try
            {
                files = GetFiles(root);
            }
            catch (Exception e)
            {
                Console.WriteLine(e.Message);
            }

            try
            {
                subDirs = GetDirectories(root);
            }
            catch (Exception e)
            {
                Console.WriteLine(e.Message);
            }

            // Create the (current) root directory item with data on number of files and subfolders.
            FileItemModel dirItem = new FileItemModel()
            {
                ItemType = isDrive ? FileItemType.Drive : FileItemType.Directory,
                Name = root.Name,
                FullName = root.FullName,
                FileCount = files != null ? files.Length : 0,
                LastModifiedTime = root.LastWriteTime,
                FolderCount = subDirs != null ? subDirs.Length : 0,
                Size = 0
            };

            // Ensure we add to ObservableCollection safely using UI thread.
            Application.Current.Dispatcher.Invoke(() => items.Add(dirItem));

            this.CurrentScanDirectory = root.FullName;

            // Resursive call for each subdirectory.
            if (subDirs != null)
            {
                this.TotalDirectoryCount += subDirs.LongLength;
                double increment = (double)subDirs.LongLength / 100.0;

                for (long i = 0; i < subDirs.LongLength; i++)
                {
                    this.CurrentScanDirectoryProgress = (double)i * increment;
                    await WalkDirectoryTree(subDirs[i], dirItem.Items);
                }
            }

            // Append the list of files for this particular directory.
            if (files != null)
            {
                this.TotalFileCount += files.LongLength;

                foreach (FileInfo fi in files)
                {
                    this.CancellationTokenSource?.Token.ThrowIfCancellationRequested();

                    FileItemModel fileItem = new FileItemModel()
                    {
                        ItemType = FileItemType.File,
                        Name = fi.Name,
                        FullName = fi.FullName,
                        FileCount = 0,
                        LastModifiedTime = fi.LastWriteTime,
                        FolderCount = 0,
                        Size = fi.Length
                    };

                    Application.Current.Dispatcher.Invoke(() => dirItem.Items.Add(fileItem));
                }
            }
        }

        protected virtual void OnPropertyChanged([CallerMemberName] string? propertyName = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }
}
