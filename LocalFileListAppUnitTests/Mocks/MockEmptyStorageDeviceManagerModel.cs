using LocalFileListApp.Models;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LocalFileListAppUnitTests.Mocks
{
    internal class MockEmptyStorageDeviceManagerModel : IStorageDeviceManagerModel
    {
        public void RefreshAvailableDevices(ref ObservableCollection<IStorageDeviceModel> devices)
        {
            // Nothing to add, we want an empty collection.
        }
    }
}
