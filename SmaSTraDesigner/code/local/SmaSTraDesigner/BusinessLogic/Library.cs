using SmaSTraDesigner.BusinessLogic.nodes;
using SmaSTraDesigner.Controls;
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

        private ObservableCollection<UcNodeViewer> libraryNodeViewerList = new ObservableCollection<UcNodeViewer>();

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

        public ObservableCollection<UcNodeViewer> LibraryNodeViewerList
        {
            get
            {
                return libraryNodeViewerList;
            }
            set
            {
                libraryNodeViewerList = value;
                this.NotifyPropertyChanged("LibraryNodeViewerList");
            }
        }

        public void Library_DragEnter(object sender, DragEventArgs e)
        {
           
        }

        public void addLibraryNode(Node node)
        {
            Transformation nodeAsTransformation;
            DataSource nodeAsDataSource;
            OutputNode nodeAsOutputNode;
            CombinedNode nodeAsCombinedNode;

            UcNodeViewer nodeViewer = null;
            if ((nodeAsTransformation = node as Transformation) != null)
            {
                nodeViewer = new UcTransformationViewer();
            }
            else if ((nodeAsDataSource = node as DataSource) != null)
            {
                nodeViewer = new UcDataSourceViewer();
            }
            else if ((nodeAsOutputNode = node as OutputNode) != null)
            {
                nodeViewer = new UcOutputViewer();
            }
            else if ((nodeAsCombinedNode = node as CombinedNode) != null)
            {
                if (node.Class.InputTypes.Count() > 0) nodeViewer = new UcTransformationViewer();
                else nodeViewer = new UcDataSourceViewer();
            }

            LibraryNodeViewerList.Add(nodeViewer);
            nodeViewer.DataContext = node;
            nodeViewer.IsPreview = true;
        }

        public void Library_Drop(object sender, DragEventArgs e)
        {
            addLibraryNode(((Tuple<Node>)e.Data.GetData(typeof(Tuple<Node>))).Item1);
        }

        public event PropertyChangedEventHandler PropertyChanged;

        public void NotifyPropertyChanged(string propName)
        {
            if (this.PropertyChanged != null)
                this.PropertyChanged(this, new PropertyChangedEventArgs(propName));
        }

    }
}
