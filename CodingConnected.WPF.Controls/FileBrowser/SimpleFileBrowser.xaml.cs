using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.IO;
using System.Linq;
using System.Runtime.Serialization;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;

namespace CodingConnected.WPF.Controls
{

    /// <summary>
    /// Interaction logic for FileBrowser.xaml
    /// </summary>
    public partial class SimpleFileBrowser : UserControl
    {
        private List<DriveInfo> _drives;
        private DirectoryInfo _parent;
        private ObservableCollection<SimpleFileBrowserItem> _items;

        public FileSystemInfo SelectedFile
        {
            get { return (FileSystemInfo)GetValue(SelectedFileProperty); }
            set { SetValue(SelectedFileProperty, value); }
        }

        public bool SingleClickSelect
        {
            get { return (bool)GetValue(SingleClickSelectProperty); }
            set { SetValue(SingleClickSelectProperty, value); }
        }

        // Using a DependencyProperty as the backing store for SingleClickSelect.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty SingleClickSelectProperty =
            DependencyProperty.Register("SingleClickSelect", typeof(bool), typeof(SimpleFileBrowser), new PropertyMetadata(false));

        // Using a DependencyProperty as the backing store for SelectedFile.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty SelectedFileProperty =
            DependencyProperty.Register("SelectedFile", typeof(FileSystemInfo), typeof(SimpleFileBrowser), new PropertyMetadata(null));

        public DirectoryInfo SelectedDirectory
        {
            get { return (DirectoryInfo)GetValue(SelectedDirectoryProperty); }
            set { SetValue(SelectedDirectoryProperty, value); }
        }

        // Using a DependencyProperty as the backing store for MyProperty.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty SelectedDirectoryProperty =
            DependencyProperty.Register("SelectedDirectory", typeof(DirectoryInfo), typeof(SimpleFileBrowser), new PropertyMetadata(null, new PropertyChangedCallback(OnSelectedDirectoryChanged)));

        public static void OnSelectedDirectoryChanged(DependencyObject o, DependencyPropertyChangedEventArgs e)
        {
            var fb = o as SimpleFileBrowser;
            fb.SetDir((DirectoryInfo)e.NewValue);
        }

        public SimpleFileBrowser()
        {
            InitializeComponent();

            var root = DriveInfo.GetDrives();

            _drives = new List<DriveInfo>();
            _items = new ObservableCollection<SimpleFileBrowserItem>();

            foreach (var r in root)
            {
                _drives.Add(r);
            }

            FileList.ItemsSource = _items;
            FileList.SelectionChanged += FileList_SelectionChanged;
        }

        private void FileList_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            var i = ((SimpleFileBrowserItem)((ListView)sender).SelectedItem);
            if (SingleClickSelect && i?.Item != null) SelectedFile = i.Item;
        }

        public void SetDir(DirectoryInfo diri)
        {
            try
            {
                _parent = diri.Parent;
                var dirs = diri.EnumerateDirectories();
                var files = diri.EnumerateFiles();
                _items.Clear();
                _items.Add(new SimpleFileBrowserItem(_parent, "..", null));
                foreach (var d in dirs) _items.Add(new SimpleFileBrowserItem(d, d.Name, ImageUtilities.GetIconImage(d.FullName)));
                foreach (var f in files) _items.Add(new SimpleFileBrowserItem(f, f.Name, ImageUtilities.GetIconImage(f.FullName)));
                SelectedFile = _items.First().Item;
                FileList.SelectedItem = _items.First();
            }
            catch { }
        }

        private void ListViewItem_PreviewKeyDown(object sender, KeyEventArgs e)
        {
            var i = (SimpleFileBrowserItem)((ListViewItem)sender).DataContext;
            DirectoryInfo newDir = null;
            if (e.Key == Key.Enter || e.Key == Key.Right)
            {
                switch (i.Item)
                {
                    case DirectoryInfo di:
                        _parent = di.Parent;
                        SelectedFile = newDir = di;
                        break;
                    case FileInfo fi:
                        SelectedFile = fi;
                        break;
                }
                e.Handled = true;
            }
            if (e.Key == Key.Back || e.Key == Key.Left)
            {
                newDir = _parent;
                _parent = _parent.Parent;
            }
            if (newDir != null)
            {
                SelectedDirectory = newDir;
                SetDir(newDir);
            }
        }

        private void StackPanel_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            if (e.ClickCount == 2)
            {
                var i = (SimpleFileBrowserItem)((DockPanel)sender).DataContext;
                DirectoryInfo newDir = null;
                switch (i.Item)
                {
                    case DirectoryInfo di:
                        _parent = di.Parent;
                        SelectedFile = newDir = di;
                        break;
                    case FileInfo fi:
                        SelectedFile = fi;
                        break;
                }
                if (newDir != null)
                {
                    SelectedDirectory = newDir;
                    SetDir(newDir);
                }
            }
        }

        private void FileList_KeyDown(object sender, KeyEventArgs e)
        {
            e.Handled = false;
        }
    }
}
