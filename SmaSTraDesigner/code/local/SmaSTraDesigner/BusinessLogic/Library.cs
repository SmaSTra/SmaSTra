using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;

namespace SmaSTraDesigner.BusinessLogic
{
    class Library : INotifyPropertyChanged
    {

        private ObservableCollection<Node> libraryNodeList = new ObservableCollection<Node>();

        private string nodeName = "default name";

        public Library()
        {
            
        }

        public string NodeName
        {
            get
            {
                    return nodeName;
            }
            set
            {
                nodeName = value;
                this.NotifyPropertyChanged("NodeName");
            }
        }

        public ObservableCollection<Node> LibraryNodeList
        {
            get
            {
                return libraryNodeList;
            }
            set
            {
                libraryNodeList = value;
                this.NotifyPropertyChanged("LibraryNodeList");
            }
        }

        public void Library_DragEnter(object sender, DragEventArgs e)
        {
           
        }

        public void Library_Drop(object sender, DragEventArgs e)
        {
            Node node = ((Tuple<Node>)e.Data.GetData(typeof(Tuple<Node>))).Item1;
            LibraryNodeList.Add(node);
        }

        public event PropertyChangedEventHandler PropertyChanged;

        public void NotifyPropertyChanged(string propName)
        {
            if (this.PropertyChanged != null)
                this.PropertyChanged(this, new PropertyChangedEventArgs(propName));
        }

    }
}
