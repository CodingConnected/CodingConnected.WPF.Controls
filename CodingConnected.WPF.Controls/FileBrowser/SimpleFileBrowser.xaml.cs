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
        #region Fields

        private List<DriveInfo> _drives;
        private DirectoryInfo _parent;
        private DirectoryInfo _current;
        private string[] _filterExt;
        private ObservableCollection<SimpleFileBrowserItem> _items;

        #endregion // Fields

        #region SelectedFile dep.prop.

        public FileSystemInfo SelectedFile
        {
            get { return (FileSystemInfo)GetValue(SelectedFileProperty); }
            set { SetValue(SelectedFileProperty, value); }
        }

        public static readonly DependencyProperty SelectedFileProperty =
            DependencyProperty.Register("SelectedFile", typeof(FileSystemInfo), typeof(SimpleFileBrowser), new PropertyMetadata(null));

        #endregion // SelectedFile dep.prop.

        #region SelectedDirectory dep.prop.

        public DirectoryInfo SelectedDirectory
        {
            get { return (DirectoryInfo)GetValue(SelectedDirectoryProperty); }
            set { SetValue(SelectedDirectoryProperty, value); }
        }

        public static readonly DependencyProperty SelectedDirectoryProperty =
            DependencyProperty.Register("SelectedDirectory", typeof(DirectoryInfo), typeof(SimpleFileBrowser), new PropertyMetadata(null, new PropertyChangedCallback(OnSelectedDirectoryChanged)));

        public static void OnSelectedDirectoryChanged(DependencyObject o, DependencyPropertyChangedEventArgs e)
        {
            var fb = o as SimpleFileBrowser;
            fb.SetDir((DirectoryInfo)e.NewValue);
        }

        #endregion // SelectedDirectory dep.prop.
        
        #region OpenOnSelect dep.prop.

        public bool OpenOnSelect
        {
            get { return (bool)GetValue(OpenOnSelectProperty); }
            set { SetValue(OpenOnSelectProperty, value); }
        }

        public static readonly DependencyProperty OpenOnSelectProperty =
            DependencyProperty.Register("OpenOnSelect", typeof(bool), typeof(SimpleFileBrowser), new PropertyMetadata(false));

        #endregion // OpenOnSelect dep.prop.

        #region FileFilter dep.prop.

        public string FileFilter
        {
            get { return (string)GetValue(FileFilterProperty); }
            set { SetValue(FileFilterProperty, value); }
        }

        public static readonly DependencyProperty FileFilterProperty =
            DependencyProperty.Register("FileFilter", typeof(string), typeof(SimpleFileBrowser), new PropertyMetadata(null, new PropertyChangedCallback(OnFileFilterChanged)));

        public static void OnFileFilterChanged(DependencyObject o, DependencyPropertyChangedEventArgs e)
        {
            var fb = o as SimpleFileBrowser;
            fb.SetFilter((string)e.NewValue);
            fb.SetDir(null);
        }

        #endregion // FileFilter dep.prop.

        #region Private methods

        private void FileList_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            var i = ((SimpleFileBrowserItem)((ListView)sender).SelectedItem);
            if (OpenOnSelect && i?.Item != null) SelectedFile = i.Item;
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
                if (_parent != null) _parent = _parent.Parent;
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

        #endregion // Private methods

        #region Public methods

        public void SetFilter(string filter)
        {
            if (string.IsNullOrWhiteSpace(filter)) _filterExt = null;
            else _filterExt = filter.Split(',');
        }

        public void SetDir(DirectoryInfo diri)
        {
            if (diri != null)
            {
                _current = diri;
            }
            if (_current == null)
            {
                return;
            }
            try
            {
                _parent = _current.Parent;
                var dirs = _current.EnumerateDirectories();
                var files = _current.EnumerateFiles();
                _items.Clear();
                _items.Add(new SimpleFileBrowserItem(_parent, "..", null));
                foreach (var d in dirs) _items.Add(new SimpleFileBrowserItem(d, d.Name, ImageUtilities.GetIconImage(d.FullName)));
                foreach (var f in files)
                {
                    if(_filterExt != null)
                    {
                        if(_filterExt.Any(x => x == f.Extension))
                        {
                            _items.Add(new SimpleFileBrowserItem(f, f.Name, ImageUtilities.GetIconImage(f.FullName)));
                        }
                    }
                    else
                    {
                        _items.Add(new SimpleFileBrowserItem(f, f.Name, ImageUtilities.GetIconImage(f.FullName)));
                    }
                }
                SelectedFile = _items.First().Item;
                FileList.SelectedItem = _items.First();
                UpdateLayout();
                ListViewItem item = FileList.ItemContainerGenerator.ContainerFromIndex(0) as ListViewItem;
                if (item != null)
                {
                    item.Focus();
                    Keyboard.Focus(item);
                }
            }
            catch
            {
                // ignored
            }
        }

        #endregion // Public methods

        #region Constructor

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

            SetDir(_drives.FirstOrDefault()?.RootDirectory);
            FileList.ItemsSource = _items;
            FileList.SelectionChanged += FileList_SelectionChanged;
        }

        #endregion // Constructor
    }
}
