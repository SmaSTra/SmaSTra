using SmaSTraDesigner.BusinessLogic;
using SmaSTraDesigner.BusinessLogic.online;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;

namespace SmaSTraDesigner.Controls.Support
{
    /// <summary>
    /// Interaktionslogik für DialogOnlineTransformatins.xaml
    /// </summary>
    public partial class DialogOnlineTransformatins : Window, INotifyPropertyChanged
    {

        public static bool IsOpen { get; private set; }

        private OnlineServerLink onlineServer;

        private List<SimpleClass> simpleOnlineElementsList;
        private DownloadAllResponse downloadAllResponse;

        private AbstractNodeClass downloadedClass;
        private DownloadSingleResponse downloadSingleResponse;

        private string n;
        private UploadResponse uploadResponse;

        private ObservableCollection<SimpleClass> onlineElementsList = new ObservableCollection<SimpleClass>();
        private SimpleClass selectedClass;

        public ObservableCollection<SimpleClass> OnlineElementsList
        {
            get { return onlineElementsList; }
            set
            {   if (value != null)
                {
                    onlineElementsList = value;
                    this.NotifyPropertyChanged("OnlineElementsList");
                }
            }
        }

        public SimpleClass SelectedClass
        {
            get { return selectedClass; }
            set
            {
                if (value != null)
                {
                    selectedClass = value;
                    this.NotifyPropertyChanged("SelectedClass");
                }
            }
        }

        public DialogOnlineTransformatins(OnlineServerLink onlineServer)
        {
            this.onlineServer = onlineServer;
            DataContext = this;
            InitializeComponent();

            onlineServer.GetAllOnlineElements(new Action<List<SimpleClass>, DownloadAllResponse>(callbackGetAllOnlineElements));
        }

        private void callbackGetAllOnlineElements(List<SimpleClass> allOnlineElementsList, DownloadAllResponse downloadAllResponse)
        {
            this.simpleOnlineElementsList = allOnlineElementsList;
            this.downloadAllResponse = downloadAllResponse;
            Dispatcher.Invoke(new Action(responseGetAllOnlineElements));
        }

        private void callbackGetOnlineElement(AbstractNodeClass downloadedClass, DownloadSingleResponse downloadSingleResponse)
        {
            this.downloadedClass = downloadedClass;
            this.downloadSingleResponse = downloadSingleResponse;
            Dispatcher.Invoke(new Action(responseGetOnlineElement));
        }

        private void callbackUploadElement(string n, UploadResponse uploadResponse)
        {
            this.uploadResponse = uploadResponse;
            this.n = n;
            Dispatcher.Invoke(new Action(responseUploadElement));
        }

        private void responseGetAllOnlineElements()
        {
            switch (downloadAllResponse)
            {
                case DownloadAllResponse.SUCCESS:
                    tbStatusBar.Text = "Success: Online elements updated";
                    //TODO: update list
                    OnlineElementsList = new ObservableCollection<SimpleClass>(simpleOnlineElementsList);
                    return;
                default:
                    tbStatusBar.Text = "Error: " + downloadAllResponse.ToString();
                    return;
            }
        }

        private void responseGetOnlineElement()
        {
            switch (downloadSingleResponse)
            {
                case DownloadSingleResponse.SUCCESS:
                    tbStatusBar.Text = "Success: Element downloaded";
                    //TODO handle download
                    return;
                default:
                    tbStatusBar.Text = "Error: " + downloadSingleResponse.ToString();
                    return;
            }
        }

        private void uploadElement(AbstractNodeClass clazz, Action<string, UploadResponse> callback)
        {
            onlineServer.UploadElement(clazz, callbackUploadElement);
        }

        private void responseUploadElement()
        {
            switch (uploadResponse)
            {
                case UploadResponse.SUCCESS:
                    tbStatusBar.Text = "Success: Element uploaded";
                    //TODO handle download
                    return;
                default:
                    tbStatusBar.Text = "Error: " + downloadSingleResponse.ToString();
                    return;
            }
        }

        public event PropertyChangedEventHandler PropertyChanged;

        public void NotifyPropertyChanged(string propName)
        {
            if (this.PropertyChanged != null)
            this.PropertyChanged(this, new PropertyChangedEventArgs(propName));
        }

        private void UpdateButton_Click(object sender, RoutedEventArgs e)
        {
            onlineServer.GetAllOnlineElements(new Action<List<SimpleClass>, DownloadAllResponse>(callbackGetAllOnlineElements));
        }

        private void listOnlineElements_SelectionChanged(object sender, RoutedEventArgs e)
        {
            SelectedClass = ((SimpleClass)((ListView)sender).SelectedItem);
        }

        private void btnDownloadElement_Click(object sender, RoutedEventArgs e)
        {
            if (SelectedClass != null) {
                onlineServer.GetOnlineElement(SelectedClass.Name, callbackGetOnlineElement);
                    } else
            {
                tbStatusBar.Text = "Please select an element.";
            }
        }

        private void uploadDropZone_Drop(object sender, DragEventArgs e)
        {
            AbstractNodeClass uploadClass = ((Tuple<Node>)e.Data.GetData(typeof(Tuple<Node>))).Item1.Class as AbstractNodeClass;
            if (uploadClass != null)
            {
                onlineServer.UploadElement(uploadClass, callbackUploadElement);
            }
        }

        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            IsOpen = true;
        }

        private void Window_Unloaded(object sender, RoutedEventArgs e)
        {
            IsOpen = false;
        }
    }
}
