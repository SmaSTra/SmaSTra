using System;
using System.ComponentModel;
using System.Windows;
using System.Windows.Controls;
using Common;
using SmaSTraDesigner.BusinessLogic.online;

namespace SmaSTraDesigner.Controls.Support
{
    /// <summary>
    /// Interaction logic for DialogErrorHandler.xaml
    /// </summary>
    public partial class DialogErrorHandler : Window, INotifyPropertyChanged
    {

        /// <summary>
        /// Is raised whenever a compatible property changes its value.
        /// </summary>
        public event PropertyChangedEventHandler PropertyChanged;

        /// <summary>
        /// A copy of the Exception.
        /// </summary>
        private readonly Exception exp;


        public DialogErrorHandler(Exception exp)
        {
            InitializeComponent();
            DataContext = this;

            this.exp = exp;
            this.ExpText = exp.ToString();
            this.UploadText = "Upload";

            this.OnlineServiceReachable = Singleton<OnlineServerLink>.Instance.IsReachable();
        }

        /// <summary>
        /// The Exception text to display.
        /// </summary>
        public string ExpText { get; }

        /// <summary>
        /// The Exception text to display.
        /// </summary>
        public string UploadText { get; set; }

        /// <summary>
        /// The Property if the Online-Service is reachable.
        /// </summary>
        public bool OnlineServiceReachable { get; set; }


        private void btn_Ignore(object sender, RoutedEventArgs e)
        {
            this.Close();
        }

        private void btn_Upload(object sender, RoutedEventArgs e)
        {
            var button = sender as Button;
            if (button == null) return;
            if (!button.IsEnabled) return;

            Action<UploadExceptionResponse> callback = UploadCallback;
            Singleton<OnlineServerLink>.Instance.UploadException(exp, callback);

            this.OnlineServiceReachable = false;
            this.UploadText = "Uploading...";

            this.OnPropertyChanged("OnlineServiceReachable");
            this.OnPropertyChanged("UploadText");
            
        }


        private void UploadCallback(UploadExceptionResponse response)
        {
            var success = response == UploadExceptionResponse.SUCCESS;
            this.UploadText = success ? "Success" : "Failed";
            this.OnPropertyChanged("UploadText");
        }


        /// <summary>
        /// Raises the PropertyChanged event.
        /// </summary>
        /// <param name="propertyName">Name of the property that changed values.</param>
        private void OnPropertyChanged(string propertyName)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }

    }
}
