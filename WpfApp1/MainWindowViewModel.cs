using System.IO;

namespace WpfApp1
{
    public class MainWindowViewModel : ViewModelBase
    {
        #region Fields

        private FileSystemInfo _selectedFile;
        private DirectoryInfo _selectedDirectory;
        private string _selectedFileContent;

        #endregion // Fields

        #region Properties

        public DirectoryInfo SelectedDirectory
        {
            get => _selectedDirectory;
            set
            {
                _selectedDirectory = value;
                NotifyPropertyChanged();
            }
        }

        public FileSystemInfo SelectedFile
        {
            get => _selectedFile;
            set
            {
                _selectedFile = value;
                if (_selectedFile is FileInfo fi)
                {
                    try
                    {
                        SelectedFileContent = File.ReadAllText(fi.FullName);
                    }
                    catch
                    {
                        // ignored
                    }
                }
                NotifyPropertyChanged();
            }
        }

        public string SelectedFileContent
        {
            get => _selectedFileContent;
            set
            {
                _selectedFileContent = value;
                NotifyPropertyChanged();
            }
        }

        #endregion // Properties
    }
}
