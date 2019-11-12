using System.Collections;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.IO;
using System.Linq;
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

        #region FileOpenedCommand dep.prop.

        public ICommand FileOpenedCommand
        {
            get { return (ICommand)GetValue(FileOpenedCommandProperty); }
            set { SetValue(FileOpenedCommandProperty, value); }
        }

        public static readonly DependencyProperty FileOpenedCommandProperty =
            DependencyProperty.Register("FileOpenedCommand", typeof(ICommand), typeof(SimpleFileBrowser), new PropertyMetadata(null));

        #endregion // FileOpenedCommand dep.prop.

        #region SelectedFiles dep.prop.

        public IList SelectedFiles
        {
            get { return (IList)GetValue(SelectedFilesProperty); }
            set { SetValue(SelectedFilesProperty, value); }
        }

        public static readonly DependencyProperty SelectedFilesProperty =
            DependencyProperty.Register("SelectedFiles", typeof(IList), typeof(SimpleFileBrowser), new PropertyMetadata(null));

        #endregion // SelectedFiles dep.prop.

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
            var ii = ((ListView)sender).SelectedItems;
            if(i != null)
            {
                SelectedFile = i.Item;
            }
            else
            {
                SelectedFile = null;
            }
            var files = new List<FileSystemInfo>();
            if(ii != null && ii.Count > 0)
            {
                foreach(SimpleFileBrowserItem bi in ii)
                {
                    files.Add(bi.Item);
                }
                SelectedFiles = files;
            }
            else
            {
                SelectedFiles = null;
            }
            if (OpenOnSelect)
            {
                ExecuteOpen();
            }
        }

        private void ExecuteOpen()
        {
            if (SelectedFiles != null && SelectedFiles.Count > 0)
            {
                if (FileOpenedCommand?.CanExecute(SelectedFiles) == true)
                {
                    FileOpenedCommand.Execute(SelectedFiles);
                }
            }
            else if (SelectedFile != null)
            {
                var files = new List<FileSystemInfo> { SelectedFile };
                if (FileOpenedCommand?.CanExecute(files) == true)
                {
                    FileOpenedCommand.Execute(files);
                }
            }
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
                        SelectedFile = newDir = di;
                        if (e.Key == Key.Enter &&
                            (Keyboard.IsKeyDown(Key.LeftShift) || Keyboard.IsKeyDown(Key.RightShift)))
                        {
                            ExecuteOpen();
                            e.Handled = true;
                            return;
                        }
                        _parent = di.Parent;
                        break;
                    case FileInfo fi:
                        SelectedFile = fi;
                        if (e.Key == Key.Enter)
                        {
                            ExecuteOpen();
                        }
                        break;
                }
                e.Handled = true;
            }
            else if (e.Key == Key.Back || e.Key == Key.Left)
            {
                newDir = _parent;
                if (_parent != null) _parent = _parent.Parent;
            }
            else if(e.Key >= Key.A && e.Key <= Key.Z)
            {
                foreach(var f in _items)
                {
                    if (f.Name.ToLower().StartsWith(e.Key.ToString().ToLower()))
                    {
                        var id = _items.IndexOf(f);
                        FileList.SelectedItem = f;
                        this.UpdateLayout();
                        var item = ((ListViewItem)FileList.ItemContainerGenerator.ContainerFromIndex(id));
                        if (item != null) item.Focus();
                        e.Handled = true;
                        break;
                    }
                }
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
                        ExecuteOpen();
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
