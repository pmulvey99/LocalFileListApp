using System;
using System.Globalization;
using System.Windows;
using System.Windows.Data;

namespace LocalFileListApp
{
    internal class BytesToStringConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            return HelperFunctions.ConvertBytesToString((long)value);
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }

    internal class InvertBooleanConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            bool result = (bool)value;
            return !result;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }

    internal class FileItemIconConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            string resourceKey = String.Empty;

            FileItemType item = (FileItemType)value;
            switch (item)
            {
                case FileItemType.Drive:
                    resourceKey = "HardDriveIcon";
                    break;
                case FileItemType.Directory:
                    resourceKey = "FolderClosedIcon";
                    break;
                case FileItemType.File:
                    resourceKey = "DocumentIcon";
                    break;
            }

            if (!String.IsNullOrEmpty(resourceKey))
            {
                var resource = Application.Current.FindResource(resourceKey);
                if (resource != null)
                {
                    return resource;
                }
            }

            return Binding.DoNothing;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}