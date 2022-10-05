using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LocalFileListApp.Models
{
    internal class FileItemModel : IFileItemModel
    {
        public string Name { get; set; } = string.Empty;
        public string FullName { get; set; } = string.Empty;
        public long Size { get; set; }
        public int FileCount { get; set; }
        public int FolderCount { get; set; }
        public DateTime LastModifiedTime { get; set; }
        public FileItemType ItemType { get; set; }
        public ObservableCollection<IFileItemModel> Items { get; set; } = new ObservableCollection<IFileItemModel>();
    }
}
