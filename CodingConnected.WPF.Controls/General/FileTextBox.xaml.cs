using Microsoft.Win32;
using System.Windows;
using System.Windows.Controls;

namespace CodingConnected.WPF.Controls
{
    /// <summary>
    /// Interaction logic for FileTextBox.xaml
    /// </summary>
    public partial class FileTextBox : UserControl
    {
        public string File
        {
            get => (string)GetValue(FileProperty);
            set => SetValue(FileProperty, value);
        }

        public static readonly DependencyProperty FileProperty =
            DependencyProperty.Register("File", typeof(string), typeof(FileTextBox), new PropertyMetadata(""));

        public string ChooseDialogTitle
        {
            get => (string)GetValue(ChooseDialogTitleProperty);
            set => SetValue(ChooseDialogTitleProperty, value);
        }

        public static readonly DependencyProperty ChooseDialogTitleProperty =
            DependencyProperty.Register("ChooseDialogTitle", typeof(string), typeof(FileTextBox), new PropertyMetadata("Choose file"));

        public string ChooseDialogFilter
        {
            get => (string)GetValue(ChooseDialogFilterProperty);
            set => SetValue(ChooseDialogFilterProperty, value);
        }

        public static readonly DependencyProperty ChooseDialogFilterProperty =
            DependencyProperty.Register("ChooseDialogFilter", typeof(string), typeof(FileTextBox), new PropertyMetadata("*.*|All files"));

        public bool CheckFileExists
        {
            get { return (bool)GetValue(CheckFileExistsProperty); }
            set { SetValue(CheckFileExistsProperty, value); }
        }

        public static readonly DependencyProperty CheckFileExistsProperty =
            DependencyProperty.Register("CheckFileExists", typeof(bool), typeof(FileTextBox), new PropertyMetadata(true));

        public FileTextBox()
        {
            InitializeComponent();
        }

        private void SelectFileButton_Click(object sender, RoutedEventArgs e)
        {
            var ofd = new OpenFileDialog
            {
                Title = ChooseDialogTitle,
                Filter = ChooseDialogFilter,
                CheckFileExists = CheckFileExists,
                Multiselect = false
            };

            if (ofd.ShowDialog(Window.GetWindow(this)) == true)
            {
                File = ofd.FileName;
            }
        }
    }
}
