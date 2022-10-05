using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LocalFileListApp.Models
{
    /// <summary>
    /// Encapsulates functionality for managing collections of storage devices.
    /// Currently only one method exists RefreshAvailableDevices, however further
    /// functionality could be added such as responding to USB device connect/disconnect...
    /// </summary>
    internal class StorageDeviceManagerModel : IStorageDeviceManagerModel
    {
        void IStorageDeviceManagerModel.RefreshAvailableDevices(ref ObservableCollection<IStorageDeviceModel> devices)
        {
            if (null == devices)
            {
                throw new ArgumentNullException(nameof(devices));
            }

            devices.Clear();

            try
            {
                DriveInfo[] drives = DriveInfo.GetDrives();
                foreach (DriveInfo drive in drives)
                {
                    if (drive.IsReady)
                    {
                        devices.Add(new StorageDeviceModel(drive));
                    }
                }
            }
            catch (Exception e)
            {
                Console.WriteLine(e.Message);
            }
        }
    }
}
