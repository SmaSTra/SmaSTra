using System.Windows;

namespace SmaSTraDesigner.Controls.Support
{
    public partial class DialogCombinedName : Window
    {
        public DialogCombinedName()
        {
            InitializeComponent();
        }

        public string ResponseText
        {
            get { return ResponseTextBox.Text; }
            set { ResponseTextBox.Text = value; }
        }

        private void OK_Clicked(object sender, System.Windows.RoutedEventArgs e)
        {
            DialogResult = true;
        }
    }
}
