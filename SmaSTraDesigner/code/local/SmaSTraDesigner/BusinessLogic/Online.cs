using Common;
using SmaSTraDesigner.BusinessLogic.online;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Windows;
using System.Windows.Controls;

namespace SmaSTraDesigner.BusinessLogic
{
    class Online : INotifyPropertyChanged
    {

        private Window mainWindow;
        private OnlineServerLink onlineServer;

        private List<SimpleClass> simpleOnlineElementsList;
        private DownloadAllResponse downloadAllResponse;

        private AbstractNodeClass downloadedClass;
        private DownloadSingleResponse downloadSingleResponse;

        private string n;
        private UploadResponse uploadResponse;

        private ObservableCollection<SimpleClass> onlineElementsList = new ObservableCollection<SimpleClass>();
        private SimpleClass selectedClass;

        private string statusBarText ="";

        public ObservableCollection<SimpleClass> OnlineElementsList
        {
            get { return onlineElementsList; }
            set
            {
                if (value != null)
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

        public string StatusBarText
        {
            get { return statusBarText; }
            set
            {
                if (value != null)
                {
                    statusBarText = value;
                    this.NotifyPropertyChanged("StatusBarText");
                }
            }
        }

        public Window MainWindow
        {
            get { return mainWindow; }
            set
            {
                if (value != null)
                {
                    mainWindow = value;
                    this.NotifyPropertyChanged("MainWindow");
                }
            }
        }

        public Online()
        {
            onlineServer = Singleton<OnlineServerLink>.Instance;
        }

        private void callbackGetAllOnlineElements(List<SimpleClass> allOnlineElementsList, DownloadAllResponse downloadAllResponse)
        {
            this.simpleOnlineElementsList = allOnlineElementsList;
            this.downloadAllResponse = downloadAllResponse;
            MainWindow.Dispatcher.Invoke(new Action(responseGetAllOnlineElements));
        }

        private void callbackGetOnlineElement(AbstractNodeClass downloadedClass, DownloadSingleResponse downloadSingleResponse)
        {
            this.downloadedClass = downloadedClass;
            this.downloadSingleResponse = downloadSingleResponse;
            MainWindow.Dispatcher.Invoke(new Action(responseGetOnlineElement));
        }

        private void callbackUploadElement(string n, UploadResponse uploadResponse)
        {
            this.uploadResponse = uploadResponse;
            this.n = n;
            MainWindow.Dispatcher.Invoke(new Action(responseUploadElement));
        }

        private void responseGetAllOnlineElements()
        {
            switch (downloadAllResponse)
            {
                case DownloadAllResponse.SUCCESS:
                    StatusBarText = "Success: Online elements updated";
                    //TODO: update list
                    OnlineElementsList = new ObservableCollection<SimpleClass>(simpleOnlineElementsList);
                    return;
                default:
                    StatusBarText = "Error: " + downloadAllResponse.ToString();
                    return;
            }
        }

        private void responseGetOnlineElement()
        {
            switch (downloadSingleResponse)
            {
                case DownloadSingleResponse.SUCCESS:
                    StatusBarText = "Success: Element downloaded";
                    Singleton<ClassManager>.Instance.AddClass(downloadedClass);
                    return;
                default:
                    StatusBarText = "Error: " + downloadSingleResponse.ToString();
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
                    StatusBarText = "Success: Element uploaded";
                    //TODO handle download
                    return;
                default:
                    StatusBarText = "Error: " + downloadSingleResponse.ToString();
                    return;
            }
        }

        public event PropertyChangedEventHandler PropertyChanged;

        public void NotifyPropertyChanged(string propName)
        {
            if (this.PropertyChanged != null)
                this.PropertyChanged(this, new PropertyChangedEventArgs(propName));
        }

        public void UpdateButton_Click(object sender, RoutedEventArgs e)
        {
            onlineServer.GetAllOnlineElements(new Action<List<SimpleClass>, DownloadAllResponse>(callbackGetAllOnlineElements));
        }

        public void listOnlineElements_SelectionChanged(object sender, RoutedEventArgs e)
        {
            SelectedClass = ((SimpleClass)((ListView)sender).SelectedItem);
        }

        public void btnDownloadElement_Click(object sender, RoutedEventArgs e)
        {
            if (SelectedClass != null)
            {
                onlineServer.GetOnlineElement(SelectedClass.Name, callbackGetOnlineElement);
            }
            else
            {
                StatusBarText = "Please select an element.";
            }
        }

        public void uploadDropZone_Drop(object sender, DragEventArgs e)
        {
            AbstractNodeClass uploadClass = ((Tuple<Node>)e.Data.GetData(typeof(Tuple<Node>))).Item1.Class as AbstractNodeClass;
            if (uploadClass != null)
            {
                onlineServer.UploadElement(uploadClass, callbackUploadElement);
            }
        }

        public void spnOnlinePanel_IsVisibleChanged(object sender, DependencyPropertyChangedEventArgs e)
        {
            if ((bool)e.NewValue)
            {
                onlineServer.GetAllOnlineElements(new Action<List<SimpleClass>, DownloadAllResponse>(callbackGetAllOnlineElements));
            }
        }
    }
}
