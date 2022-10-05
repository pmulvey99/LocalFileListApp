using LocalFileListApp.Models;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LocalFileListAppUnitTests.Mocks
{
    internal class MockStorageDeviceManagerModel : IStorageDeviceManagerModel
    {
        public MockStorageDeviceModel? MockStorageDevice { get; set; } = null;

        public MockStorageDeviceManagerModel(MockStorageDeviceModel mockStorageDevice)
        {
            this.MockStorageDevice = mockStorageDevice;
        }

        public void RefreshAvailableDevices(ref ObservableCollection<IStorageDeviceModel> devices)
        {
            devices.Add(this.MockStorageDevice);
        }
    }
}
