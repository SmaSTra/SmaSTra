using System.Windows;

namespace SmaSTraDesigner.Controls.Support
{
    public partial class DialogCombinedName : Window
    {
        public DialogCombinedName()
        {
            InitializeComponent();
        }

        public string CombinedElementName
        {
            get { return tboxCombinedElementName.Text; }
            set { tboxCombinedElementName.Text = value; }
        }

        public string CombinedElementDescription
        {
            get { return tboxCombinedElementDescription.Text; }
            set { tboxCombinedElementDescription.Text = value; }
        }

        private void OK_Clicked(object sender, System.Windows.RoutedEventArgs e)
        {
            DialogResult = true;
        }

        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            tboxCombinedElementName.Focus();
        }

        private void Grid_MouseLeftButtonDown(object sender, System.Windows.Input.MouseButtonEventArgs e)
        {
            DragMove();
        }
    }
}
