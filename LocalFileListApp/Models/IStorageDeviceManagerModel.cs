using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LocalFileListApp.Models
{
    internal interface IStorageDeviceManagerModel
    {
        void RefreshAvailableDevices(ref ObservableCollection<IStorageDeviceModel> devices);
    }
}
