using LocalFileListApp.Models;
using LocalFileListApp.ViewModels;
using LocalFileListAppUnitTests.Mocks;

namespace LocalFileListAppUnitTests
{
    [TestClass]
    public class MainWindowViewModelTests
    {
        [TestMethod]
        public void ViewModel_ConstructorNullParam_ThrowsArgumentNullException()
        {
            // The viewmodel constructor should perform a null check on the parameter IStorageDeviceManagerModel.

            // Arrange.
            MainWindowViewModel viewModel = null;

            // Act.
            Action action = new Action(() => viewModel = new MainWindowViewModel(null));

            // Assert.
            Assert.ThrowsException<ArgumentNullException>(action);
        }

        [TestMethod]
        public void ViewModel_Constructor_ValidAvailableStorageDevices()
        {
            // Upon construction, the viewmodel should call IStorageDeviceManagerModel.RefreshAvailableDevices,
            // and the results should become a valid collection of IStorageDeviceModels in viewmodel.AvailableStorageDevices.
            // Our mock test should generate one object in the collection, and we assert that the viewmodel has the same
            // instance in its collection.

            // Arrange.
            MockStorageDeviceModel mockStorageDevice = new MockStorageDeviceModel();
            MockStorageDeviceManagerModel mockStorageDeviceManager = new MockStorageDeviceManagerModel(mockStorageDevice);
            MainWindowViewModel viewModel = new MainWindowViewModel(mockStorageDeviceManager);

            // Act.
            IStorageDeviceModel? storageDevice = viewModel.AvailableStorageDevices.FirstOrDefault();

            // Assert.
            Assert.AreSame(storageDevice, mockStorageDevice);
        }

        [TestMethod]
        public void ViewModel_ConstructorNoAvailableStorageDevices_NullSelectedStorageDevice()
        {
            // Upon construction, the viewmodel should set its property SelectedStorageDevice as the first
            // item in its collection AvailableStorageDevices. For an empty collection, this should be the default value (null).

            // Arrange.
            MockEmptyStorageDeviceManagerModel mockStorageDeviceManager = new MockEmptyStorageDeviceManagerModel();

            // Act.
            MainWindowViewModel viewModel = new MainWindowViewModel(mockStorageDeviceManager);

            // Assert.
            Assert.IsNull(viewModel.SelectedStorageDevice);
        }

        [TestMethod]
        public void ViewModel_Constructor_ValidSelectedStorageDevice()
        {
            // Upon construction, the viewmodel should set its property SelectedStorageDevice as the first
            // item in its collection AvailableStorageDevices. Our mock test should generate one object in the collection,
            // and we assert that viewmodel.SelectedStorageDevice has the same instance.

            // Arrange.
            MockStorageDeviceModel mockStorageDevice = new MockStorageDeviceModel();
            MockStorageDeviceManagerModel mockStorageDeviceManager = new MockStorageDeviceManagerModel(mockStorageDevice);

            // Act.
            MainWindowViewModel viewModel = new MainWindowViewModel(mockStorageDeviceManager);

            // Assert.
            Assert.AreSame(viewModel.SelectedStorageDevice, mockStorageDevice);
        }

        [TestMethod]
        public void SelectedStorageDevice_ChangeSelection_ValidSelectedStorageDeviceFileContents()
        {
            // Changing the viewmodel.SelectedStorageDevice should change viewmodel.SelectedStorageDeviceFileContents in turn.
            // This happens during constructor. Compare references to ensure SelectedStorageDeviceFileContents is correct.

            // Arrange.
            MockStorageDeviceModel mockStorageDevice = new MockStorageDeviceModel();
            MockStorageDeviceManagerModel mockStorageDeviceManager = new MockStorageDeviceManagerModel(mockStorageDevice);

            // Act.
            MainWindowViewModel viewModel = new MainWindowViewModel(mockStorageDeviceManager);

            // Assert.
            Assert.AreSame(viewModel.SelectedStorageDeviceFileContents, mockStorageDevice.FileItems);
        }

        [TestMethod]
        public void SelectedStorageDevice_ChangeSelection_ValidSelectedStorageDeviceFreeSpace()
        {
            // Changing the viewmodel.SelectedStorageDevice should change viewmodel.SelectedStorageDeviceFreeSpace in turn.
            // This happens during constructor. Compare values to ensure SelectedStorageDeviceFreeSpace is correct.

            // Arrange.
            long value = 9;
            MockStorageDeviceModel mockStorageDevice = new MockStorageDeviceModel();
            mockStorageDevice.AvailableFreeSpace = value;
            MockStorageDeviceManagerModel mockStorageDeviceManager = new MockStorageDeviceManagerModel(mockStorageDevice);

            // Act.
            MainWindowViewModel viewModel = new MainWindowViewModel(mockStorageDeviceManager);

            // Assert.
            Assert.AreEqual(viewModel.SelectedStorageDeviceFreeSpace, mockStorageDevice.AvailableFreeSpace);
            Assert.AreEqual(viewModel.SelectedStorageDeviceFreeSpace, value);
        }

        [TestMethod]
        public void SelectedStorageDevice_ChangeSelection_ValidSelectedStorageDeviceTotalSize()
        {
            // Changing the viewmodel.SelectedStorageDevice should change viewmodel.SelectedStorageDeviceTotalSize in turn.
            // This happens during constructor. Compare values to ensure SelectedStorageDeviceTotalSize is correct.

            // Arrange.
            long value = long.MaxValue;
            MockStorageDeviceModel mockStorageDevice = new MockStorageDeviceModel();
            mockStorageDevice.TotalSize = value;
            MockStorageDeviceManagerModel mockStorageDeviceManager = new MockStorageDeviceManagerModel(mockStorageDevice);

            // Act.
            MainWindowViewModel viewModel = new MainWindowViewModel(mockStorageDeviceManager);

            // Assert.
            Assert.AreEqual(viewModel.SelectedStorageDeviceTotalSize, mockStorageDevice.TotalSize);
            Assert.AreEqual(viewModel.SelectedStorageDeviceTotalSize, value);
        }

        [DataTestMethod]
        [DataRow(100L, "100 Bytes (of 100 Bytes)")]
        [DataRow(100L * 1024L, "100.0 KB (of 100.0 KB)")]
        [DataRow(100L * 1024L * 1024L, "100.0 MB (of 100.0 MB)")]
        [DataRow(100L * 1024L * 1024L * 1024L, "100.0 GB (of 100.0 GB)")]
        [DataRow(100L * 1024L * 1024L * 1024L * 1024L, "100.0 TB (of 100.0 TB)")]
        public void SelectedStorageDevice_ChangeSelection_ValidSelectedStorageDeviceFreeSpaceDisplayText(long memoryBytes, string expectedText)
        {
            // Changing the viewmodel.SelectedStorageDevice should change viewmodel.SelectedStorageDeviceFreeSpaceDisplayText in turn.
            // This happens during constructor. Compare values to ensure SelectedStorageDeviceFreeSpaceDisplayText is correct.

            // Arrange.
            MockStorageDeviceModel mockStorageDevice = new MockStorageDeviceModel();
            mockStorageDevice.AvailableFreeSpace = memoryBytes;
            mockStorageDevice.TotalSize = memoryBytes;
            MockStorageDeviceManagerModel mockStorageDeviceManager = new MockStorageDeviceManagerModel(mockStorageDevice);

            // Act.
            MainWindowViewModel viewModel = new MainWindowViewModel(mockStorageDeviceManager);

            // Assert.
            Assert.AreEqual(expectedText, viewModel.SelectedStorageDeviceFreeSpaceDisplayText);
        }

        [DataTestMethod]
        [DataRow(true)]
        [DataRow(false)]
        public void SelectedStorageDevice_IsBusyPropertyChanged_ValidIsSelectedStorageDeviceBusy(bool expectedValue)
        {
            // When IStorageDeviceModel.IsBusy changed event is raised, this should also be reflected in the
            // viewmodel via viewmodel.IsSelectedStorageDeviceBusy for binding to the view.

            // Arrange.
            MockStorageDeviceModel mockStorageDevice = new MockStorageDeviceModel();
            MockStorageDeviceManagerModel mockStorageDeviceManager = new MockStorageDeviceManagerModel(mockStorageDevice);
            MainWindowViewModel viewModel = new MainWindowViewModel(mockStorageDeviceManager);

            // Act.
            mockStorageDevice.IsBusy = expectedValue;
            mockStorageDevice.OnPropertyChanged("IsBusy");

            // Assert.
            Assert.AreEqual(expectedValue, viewModel.IsSelectedStorageDeviceBusy);
        }

        [DataTestMethod]
        [DataRow(99)]
        [DataRow(12345)]
        public void SelectedStorageDevice_TotalFileCountPropertyChanged_ValidSelectedStorageFileCount(long expectedValue)
        {
            // When IStorageDeviceModel.TotalFileCount changed event is raised, this should also be reflected in the
            // viewmodel via viewmodel.SelectedStorageFileCount for binding to the view.

            // Arrange.
            MockStorageDeviceModel mockStorageDevice = new MockStorageDeviceModel();
            MockStorageDeviceManagerModel mockStorageDeviceManager = new MockStorageDeviceManagerModel(mockStorageDevice);
            MainWindowViewModel viewModel = new MainWindowViewModel(mockStorageDeviceManager);

            // Act.
            mockStorageDevice.TotalFileCount = expectedValue;
            mockStorageDevice.OnPropertyChanged("TotalFileCount");

            // Assert.
            Assert.AreEqual(expectedValue, viewModel.SelectedStorageFileCount);
        }

        [DataTestMethod]
        [DataRow(1)]
        [DataRow(9999)]
        public void SelectedStorageDevice_TotalDirectoryCountPropertyChanged_ValidSelectedStorageDirectoryCount(long expectedValue)
        {
            // When IStorageDeviceModel.TotalDirectoryCount changed event is raised, this should also be reflected in the
            // viewmodel via viewmodel.SelectedStorageDirectoryCount for binding to the view.

            // Arrange.
            MockStorageDeviceModel mockStorageDevice = new MockStorageDeviceModel();
            MockStorageDeviceManagerModel mockStorageDeviceManager = new MockStorageDeviceManagerModel(mockStorageDevice);
            MainWindowViewModel viewModel = new MainWindowViewModel(mockStorageDeviceManager);

            // Act.
            mockStorageDevice.TotalDirectoryCount = expectedValue;
            mockStorageDevice.OnPropertyChanged("TotalDirectoryCount");

            // Assert.
            Assert.AreEqual(expectedValue, viewModel.SelectedStorageDirectoryCount);
        }

        [TestMethod]
        public async Task StartSelectedDeviceScan_NoSelectedStorageDevice_ThrowsNullReferenceException()
        {
            // When the viewmodel has no SelectedStorageDevice (ie null) then viewmodel.StartSelectedDeviceScan should throw.

            // Arrange.
            MockEmptyStorageDeviceManagerModel mockStorageDeviceManager = new MockEmptyStorageDeviceManagerModel();
            MainWindowViewModel viewModel = new MainWindowViewModel(mockStorageDeviceManager);

            // Act.
            Func<Task> action = new Func<Task>(async () => await viewModel.StartSelectedDeviceScanAsync());

            // Assert.
            await Assert.ThrowsExceptionAsync<NullReferenceException>(action);
        }

        [TestMethod]
        public async Task StartSelectedDeviceScan_SelectedStorageDeviceIsBusy_ThrowsNullReferenceException()
        {
            // When the viewmodel.SelectedStorageDevice.isBusy == true, then viewmodel.StartSelectedDeviceScan should throw.

            // Arrange.
            MockStorageDeviceManagerModel mockStorageDeviceManager = new MockStorageDeviceManagerModel(new MockStorageDeviceModel() { IsBusy = true });
            MainWindowViewModel viewModel = new MainWindowViewModel(mockStorageDeviceManager);

            // Act.
            Func<Task> action = new Func<Task>(async () => await viewModel.StartSelectedDeviceScanAsync());

            // Assert.
            await Assert.ThrowsExceptionAsync<InvalidOperationException>(action);
        }

        [TestMethod]
        public async Task StartSelectedDeviceScan_RunsOk_ChangesProcessingText()
        {
            // We expect to see viewmodel.ProcessingText change when property changed event is raised by the model.

            // Arrange.
            MockStorageDeviceManagerModel mockStorageDeviceManager = new MockStorageDeviceManagerModel(new MockStorageDeviceModel());
            MainWindowViewModel viewModel = new MainWindowViewModel(mockStorageDeviceManager);

            // Act.
            await viewModel.StartSelectedDeviceScanAsync();

            // Assert.
            Assert.AreNotEqual(viewModel.ProcessingText, string.Empty);
        }

        [TestMethod]
        public async Task StartSelectedDeviceScan_RunsOk_ChangesProcessingPercentage()
        {
            // We expect to see viewmodel.ProcessingPercentage change when property changed event is raised by the model.

            // Arrange.
            MockStorageDeviceManagerModel mockStorageDeviceManager = new MockStorageDeviceManagerModel(new MockStorageDeviceModel());
            MainWindowViewModel viewModel = new MainWindowViewModel(mockStorageDeviceManager);

            // Act.
            await viewModel.StartSelectedDeviceScanAsync();

            // Assert.
            Assert.AreNotEqual(viewModel.ProcessingPercentage, 0.0);
        }

        [TestMethod]
        public async Task StartSelectedDeviceScan_RunsOk_ChangesScanDuration()
        {
            // We expect to see viewmodel.ScanDurationText change when property changed event is raised by the model.

            // Arrange.
            MockStorageDeviceManagerModel mockStorageDeviceManager = new MockStorageDeviceManagerModel(new MockStorageDeviceModel());
            MainWindowViewModel viewModel = new MainWindowViewModel(mockStorageDeviceManager);

            // Act.
            await viewModel.StartSelectedDeviceScanAsync();

            // Assert.
            Assert.AreNotEqual(viewModel.ScanDurationText, String.Empty);
        }

        [TestMethod]
        public void StopSelectedDeviceScan_NoSelectedStorageDevice_ThrowsNullReferenceException()
        {
            // When the viewmodel has no SelectedStorageDevice (ie null) then viewmodel.StopSelectedDeviceScan should throw.

            // Arrange.
            MockEmptyStorageDeviceManagerModel mockStorageDeviceManager = new MockEmptyStorageDeviceManagerModel();
            MainWindowViewModel viewModel = new MainWindowViewModel(mockStorageDeviceManager);

            // Act.
            Action action = new Action(() => viewModel.StopSelectedDeviceScan());

            // Assert.
            Assert.ThrowsException<NullReferenceException>(action);
        }
    }
}