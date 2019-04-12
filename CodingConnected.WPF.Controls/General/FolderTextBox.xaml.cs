using System.Windows;
using System.Windows.Controls;

namespace CodingConnected.WPF.Controls
{
    /// <summary>
    /// Interaction logic for FolderTextBox.xaml
    /// </summary>
    public partial class FolderTextBox : UserControl
    {
        public string Folder
        {
            get { return (string)GetValue(FolderProperty); }
            set { SetValue(FolderProperty, value); }
        }

        // Using a DependencyProperty as the backing store for Text.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty FolderProperty =
            DependencyProperty.Register("Folder", typeof(string), typeof(FolderTextBox), new PropertyMetadata(""));

        public string ChooseDialogTitle
        {
            get { return (string)GetValue(ChooseDialogTitleProperty); }
            set { SetValue(ChooseDialogTitleProperty, value); }
        }

        // Using a DependencyProperty as the backing store for ChooseDialogTitle.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty ChooseDialogTitleProperty =
            DependencyProperty.Register("ChooseDialogTitle", typeof(string), typeof(FolderTextBox), new PropertyMetadata("Choose folder"));

        public FolderTextBox()
        {
            InitializeComponent();
        }

        private void SelectStdFolderButton_Click(object sender, RoutedEventArgs e)
        {
            var dlg = new System.Windows.Forms.FolderBrowserDialog()
            {
                 Description = ChooseDialogTitle
            };
            if (dlg.ShowDialog() == System.Windows.Forms.DialogResult.OK)
            {
                Folder = dlg.SelectedPath;
            }
        }
    }
}
