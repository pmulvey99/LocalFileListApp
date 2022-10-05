using LocalFileListApp.Models;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LocalFileListAppUnitTests
{
    [TestClass]
    public class StorageDeviceManagerModelTests
    {
        [TestMethod]
        public void RefreshAvailableDevices_NullParameter_ThrowsArgumentNullException()
        {
            // When passed a null parameter, the method StorageDeviceManagerModel.RefreshAvailableDevices should throw
            // an argument null exception.

            // Arrange.
            IStorageDeviceManagerModel model = new StorageDeviceManagerModel();
            ObservableCollection<IStorageDeviceModel>? collection = null;

            // Act.
            Action action = new Action(() => model.RefreshAvailableDevices(ref collection));

            // Assert.
            Assert.ThrowsException<ArgumentNullException>(action);
        }
    }
}
