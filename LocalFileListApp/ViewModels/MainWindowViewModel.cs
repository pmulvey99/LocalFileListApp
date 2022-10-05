using LocalFileListApp.Models;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Collections.Specialized;
using System.ComponentModel;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Threading;
using System.Xml.Linq;

namespace LocalFileListApp.ViewModels
{
    /// <summary>
    /// MainWindowViewModel contains the various bindings for the view, and fills data items via the models.
    /// In summary, the view model will set a storage device as the SelectedStorageDevice and this will be used
    /// for all operations.
    /// </summary>
    internal class MainWindowViewModel : INotifyPropertyChanged
    {
        public ObservableCollection<IStorageDeviceModel> AvailableStorageDevices
        {
            get { return _availableStorageDevices; }
        }
        private readonly ObservableCollection<IStorageDeviceModel> _availableStorageDevices = new ObservableCollection<IStorageDeviceModel>();

        public IStorageDeviceManagerModel? StorageDeviceManagerModel { get; private set; } = null;

        public IStorageDeviceModel? SelectedStorageDevice
        {
            get => _selectedStorageDevice;
            set
            {
                if (null == value || _selectedStorageDevice == value)
                {
                    return;
                }

                // Disconnect any previous PropertyChanged events before switching the selected storage drive.
                if (null != _selectedStorageDevice)
                {
                    _selectedStorageDevice.PropertyChanged -= _selectedStorageDevice_PropertyChanged;
                }

                // Now switch to this storage drive and update various items including bindings for the display.
                _selectedStorageDevice = value;
                _selectedStorageDevice.PropertyChanged += _selectedStorageDevice_PropertyChanged;

                OnPropertyChanged();
                OnPropertyChanged("SelectedStorageDeviceFileContents");

                UpdateAllProperties();
            }
        }
        private IStorageDeviceModel? _selectedStorageDevice = null;

        /// <summary>
        /// Returns a reference to whatever the currently selected storage device has in its list of files.
        /// </summary>
        public ObservableCollection<IFileItemModel>? SelectedStorageDeviceFileContents
        {
            get => this.SelectedStorageDevice?.FileItems;
        }

        /// <summary>
        /// Since the primary SelectedStorageDeviceFileContents is hierarchical, we need a flat equivalent to bind to the File List tab DataGrid.
        /// Therefore a new collection is constructed using a flattening query on the original collection, in response to the "FileItemsChanged" property changed event.
        /// The query becomes expensive as the collection grows, therefore the "FileItemsChanged" event is raised at a slower rate by the SelectedStorageDevice.
        /// </summary>
        public ObservableCollection<IFileItemModel>? SelectedStorageDeviceFileContentsFlat
        {
            get => _selectedStorageDeviceFileContentsFlat;
            set
            {
                _selectedStorageDeviceFileContentsFlat = value;
                OnPropertyChanged();
            }
        }
        private ObservableCollection<IFileItemModel>? _selectedStorageDeviceFileContentsFlat = null;

        public bool IsSelectedStorageDeviceBusy
        {
            get => _isSelectedStorageDeviceBusy;
            private set
            {
                _isSelectedStorageDeviceBusy = value;
                OnPropertyChanged();
            }
        }
        private bool _isSelectedStorageDeviceBusy = false;

        public long? SelectedStorageDeviceFreeSpace
        {
            get => this.SelectedStorageDevice?.AvailableFreeSpace;
        }

        public long? SelectedStorageDeviceTotalSize
        {
            get => this.SelectedStorageDevice?.TotalSize;
        }

        public string SelectedStorageDeviceFreeSpaceDisplayText
        {
            get => _selectedStorageDeviceFreeSpaceDisplayText;
            set
            {
                _selectedStorageDeviceFreeSpaceDisplayText = value;
                OnPropertyChanged();
            }
        }
        private string _selectedStorageDeviceFreeSpaceDisplayText = string.Empty;

        public long SelectedStorageFileCount
        {
            get => _selectedStorageFileCount;
            private set
            {
                _selectedStorageFileCount = value;
                OnPropertyChanged();
            }
        }
        private long _selectedStorageFileCount = 0;

        public long SelectedStorageDirectoryCount
        {
            get => _selectedStorageDirectoryCount;
            private set
            {
                _selectedStorageDirectoryCount = value;
                OnPropertyChanged();
            }
        }
        private long _selectedStorageDirectoryCount = 0;

        public string ProcessingText
        {
            get => _processingText;
            private set
            {
                _processingText = value;
                OnPropertyChanged();
            }
        }
        private string _processingText = string.Empty;

        public double ProcessingPercentage
        {
            get => _processingPercentage;
            set
            {
                _processingPercentage = value;
                OnPropertyChanged();
            }
        }
        private double _processingPercentage;

        public string ScanDurationText
        {
            get => _scanDurationText;
            set
            {
                _scanDurationText = value;
                OnPropertyChanged();
            }
        }
        private string _scanDurationText = string.Empty;

        public event PropertyChangedEventHandler? PropertyChanged;

        /// <summary>
        /// Constructor accepts dependency of type IStorageDeviceManagerModel and refreshes the list of available storage devices,
        /// setting the first item as the current SelectedStorageDevice.
        /// </summary>
        /// <param name="storageDeviceManagerModel"></param>
        /// <exception cref="ArgumentNullException"></exception>
        public MainWindowViewModel(in IStorageDeviceManagerModel storageDeviceManagerModel)
        {
            this.StorageDeviceManagerModel = storageDeviceManagerModel ?? throw new ArgumentNullException(nameof(storageDeviceManagerModel));
            this.StorageDeviceManagerModel.RefreshAvailableDevices(ref _availableStorageDevices);

            this.SelectedStorageDevice = this.AvailableStorageDevices.FirstOrDefault();
        }

        /// <summary>
        /// Executes async scanning of selected storage device.
        /// </summary>
        /// <returns></returns>
        /// <exception cref="NullReferenceException"></exception>
        /// <exception cref="InvalidOperationException"></exception>
        public async Task StartSelectedDeviceScanAsync()
        {
            if (null == this.SelectedStorageDevice)
            {
                throw new NullReferenceException(nameof(this.SelectedStorageDevice));
            }

            if (this.SelectedStorageDevice.IsBusy)
            {
                throw new InvalidOperationException("SelectedStorageDevice is busy");
            }

            await this.SelectedStorageDevice.StartReadingFileContentsAsync();
            this.ProcessingText = "Done";
        }

        /// <summary>
        /// Cancels async operations.
        /// </summary>
        /// <exception cref="NullReferenceException"></exception>
        public void StopSelectedDeviceScan()
        {
            if (null == this.SelectedStorageDevice)
            {
                throw new NullReferenceException(nameof(this.SelectedStorageDevice));
            }

            this.SelectedStorageDevice.StopReadingFileContents();
        }

        /// <summary>
        /// Handles various property changes in the SelectedStorageDevice, and updates the relevant viewmodel data items.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void _selectedStorageDevice_PropertyChanged(object? sender, PropertyChangedEventArgs e)
        {
            if (e.PropertyName == "IsBusy")
            {
                UpdateStorageDeviceBusy();
            }
            else if (e.PropertyName == "TotalFileCount")
            {
                UpdateStorageFileCount();
            }
            else if (e.PropertyName == "TotalDirectoryCount")
            {
                UpdateStorageDirectoryCount();
            }
            else if (e.PropertyName == "CurrentScanDirectory")
            {
                UpdateProcessingText();
            }
            else if (e.PropertyName == "CurrentScanDirectoryProgress")
            {
                UpdateProcessingPercentage();
            }
            else if (e.PropertyName == "FileItemsChanged")
            {
                UpdateFlatFileContents();
            }
            else if (e.PropertyName == "ScanDuration")
            {
                UpdateScanDurationText();
            }
        }

        private void UpdateStorageDeviceBusy()
        {
            if (null == this.SelectedStorageDevice)
            {
                return;
            }

            this.IsSelectedStorageDeviceBusy = this.SelectedStorageDevice.IsBusy;
        }

        private void UpdateStorageFileCount()
        {
            if (null == this.SelectedStorageDevice)
            {
                return;
            }

            this.SelectedStorageFileCount = this.SelectedStorageDevice.TotalFileCount;
        }

        private void UpdateStorageDirectoryCount()
        {
            if (null == this.SelectedStorageDevice)
            {
                return;
            }

            this.SelectedStorageDirectoryCount = this.SelectedStorageDevice.TotalDirectoryCount;
        }

        private void UpdateProcessingText()
        {
            if (null == this.SelectedStorageDevice)
            {
                return;
            }

            this.ProcessingText = this.SelectedStorageDevice.CurrentScanDirectory;
        }

        private void UpdateProcessingPercentage()
        {
            if (null == this.SelectedStorageDevice)
            {
                return;
            }

            this.ProcessingPercentage = this.SelectedStorageDevice.CurrentScanDirectoryProgress;
        }

        private void UpdateFlatFileContents()
        {
            var results = this.SelectedStorageDeviceFileContents?.Expand(x => x.Items);
            if (null != results)
            {
                this.SelectedStorageDeviceFileContentsFlat = new ObservableCollection<IFileItemModel>(results);
            }
        }

        private void UpdateScanDurationText()
        {
            if (null == this.SelectedStorageDevice)
            {
                return;
            }

            this.ScanDurationText = this.SelectedStorageDevice.ScanDuration;
        }

        /// <summary>
        /// Uses SelectedStorageDeviceFreeSpace and SelectedStorageDeviceTotalSize to create a single string eg. "100mb (of 512mb)"
        /// </summary>
        /// <returns></returns>
        private void UpdateFreeSpaceText()
        {
            if (null == this.SelectedStorageDeviceFreeSpace || null == this.SelectedStorageDeviceTotalSize)
            {
                return;
            }

            string freeSpaceText = HelperFunctions.ConvertBytesToString((long)this.SelectedStorageDeviceFreeSpace);
            string totalSpaceText = HelperFunctions.ConvertBytesToString((long)this.SelectedStorageDeviceTotalSize);

            StringBuilder sb = new StringBuilder();
            sb.Append(freeSpaceText);
            sb.Append(" (of ");
            sb.Append(totalSpaceText);
            sb.Append(')');

            this.SelectedStorageDeviceFreeSpaceDisplayText = sb.ToString();
        }

        private void UpdateAllProperties()
        {
            UpdateFlatFileContents();
            UpdateStorageDirectoryCount();
            UpdateStorageFileCount();
            UpdateProcessingText();
            UpdateProcessingPercentage();
            UpdateScanDurationText();
            UpdateFreeSpaceText();
        }

        protected virtual void OnPropertyChanged([CallerMemberName] string? propertyName = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }
}
