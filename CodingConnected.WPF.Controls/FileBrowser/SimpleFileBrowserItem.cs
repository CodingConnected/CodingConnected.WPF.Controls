using System.IO;
using System.Windows.Media;

namespace CodingConnected.WPF.Controls
{
    class SimpleFileBrowserItem
    {
        public FileSystemInfo Item { get; }
        public string Name { get; }
        public ImageSource Icon { get; }

        public SimpleFileBrowserItem(FileSystemInfo item, string name, ImageSource icon)
        {
            Item = item;
            Name = name;
            Icon = icon;
        }
    }
}
