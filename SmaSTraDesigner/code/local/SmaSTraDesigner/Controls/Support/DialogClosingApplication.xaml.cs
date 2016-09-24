using System.Windows;

namespace SmaSTraDesigner.Controls.Support
{
    /// <summary>
    /// Interaktionslogik für DialogClosingApplication.xaml
    /// </summary>
    public partial class DialogClosingApplication : Window
    {
        public DialogClosingApplication()
        {
            InitializeComponent();
        }

        private bool yesClicked = false;
        public bool YesClicked
        {
            get
            {
                return yesClicked;
            }
            set
            {
                yesClicked = value;
            }
        }
        private void btnYes_Click(object sender, RoutedEventArgs e)
        {
            YesClicked = true;
            DialogResult = true;
        }

        private void btnNo_Click(object sender, RoutedEventArgs e)
        {
            YesClicked = false;
            DialogResult = true;
        }

        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            btnNo.Focus();
        }

        private void Window_Closing(object sender, System.ComponentModel.CancelEventArgs e)
        {
            DialogResult = true;
        }
    }
}
