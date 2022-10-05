using System;
using System.Collections.ObjectModel;

namespace LocalFileListApp.Models
{
    internal interface IFileItemModel
    {
        string Name { get; set; }
        string FullName { get; set; }
        long Size { get; set; }
        int FileCount { get; set; }
        int FolderCount { get; set; }
        DateTime LastModifiedTime { get; set; }
        FileItemType ItemType { get; set; }
        ObservableCollection<IFileItemModel> Items { get; set; }
    }
}