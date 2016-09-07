using SmaSTraDesigner.BusinessLogic.classhandler;
using SmaSTraDesigner.Controls;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Data;

namespace SmaSTraDesigner.BusinessLogic
{
    public class NodeProperties : INotifyPropertyChanged

    {
       
        private string defaultString = "---";
        private double defaultNumber = 0;

        

        private UcNodeViewer nodeViewer = null;
        public UcNodeViewer NodeViewer
        {
            get { return nodeViewer; }
            set {
                if (NodeViewer != null)
                {
                    nodeViewer.Node.PropertyChanged -= OnNodePropertyChanged;
                }
                    nodeViewer = value;
                    NodeViewer.Node.PropertyChanged += OnNodePropertyChanged;
                    updateNodeProperties(NodeViewer.Node);
                    OnNodeViewerChanged(NodeViewer);
                    this.NotifyPropertyChanged("NodeViewer");
            }
        }

        public string NodeName
        {
            get
            {
                if (nodeViewer != null)
                {
                    return nodeViewer.Node.Name;
                }
                else
                {
                    return defaultString;
                }
            }
            set
            {
                nodeViewer.Node.Name = value;
                this.NotifyPropertyChanged("NodeName");
            }
        }

        public string NodeClass
        {
            get
            {
                if (nodeViewer != null && nodeViewer.Node.Class != null)
                {
                    return nodeViewer.Node.Class.DisplayName;
                }
                else
                {
                    return defaultString;
                }
            }
        }

        //public UcIOHandle[] NodeViewerIOHandles
        //{
        //    get
        //    {
        //        if (NodeViewer != null)
        //        {
        //            return NodeViewer.IoHandles;
        //        }
        //        else
        //        {
        //            return null;
        //        }
        //    }
        //}

        public ObservableCollection<IOData> NodeInputIOData
        {
            get
            {
                if (NodeViewer != null)
                {
                    return NodeViewer.Node.InputIOData;
                } else
                {
                    return null;
                }
            }
        }

        public string NodeClassOutput
        {
            get
            {
                if (NodeViewer != null && NodeViewer.Node.Class != null && NodeViewer.Node.Class.OutputType != null)
                {
                        return NodeViewer.Node.Class.OutputType.Name.Split('.').Last();
                }
                else
                {
                    return defaultString;
                }
            }
        }

        public double NodePositionX
        {
            get
            {
                if (nodeViewer != null)
                {
                    return nodeViewer.Node.PosX;
                }
                else
                {
                    return defaultNumber;
                }
            }
            set
            {
                if (value != nodeViewer.Node.PosX)
                {
                    nodeViewer.Node.PosX = value;
                }
                this.NotifyPropertyChanged("NodePositionX");
            }
        }

        public double NodePositionY
        {
            get
            {
                if (nodeViewer != null)
                {
                    return nodeViewer.Node.PosY;
                }
                else
                {
                    return defaultNumber;
                }
            }
            set
            {
                if (value != nodeViewer.Node.PosY)
                {
                    nodeViewer.Node.PosY = value;
                }
                this.NotifyPropertyChanged("NodePositionY");

            }
        }

        public string NodeClassDescription
        {
            get
            {
                if (nodeViewer != null && nodeViewer.Node.Class != null)
                {
                    return nodeViewer.Node.Class.Description;
                }
                else
                {
                    return defaultString;
                }
            }
        }

        

        protected virtual void OnNodePropertyChanged(object sender, PropertyChangedEventArgs e)
        {
            Node node = sender as Node;
            updateNodeProperties(node);
        }

        protected virtual void OnNodeViewerChanged(UcNodeViewer nodeViewer)
        {
            this.NotifyPropertyChanged("NodeClass");
            this.NotifyPropertyChanged("NodeClassDescription");
            this.NotifyPropertyChanged("NodeInputIOData");
            this.NotifyPropertyChanged("NodeClassOutput");


        }

        private void updateNodeProperties(Node node)
        {
            NodeName = node.Name;
            NodePositionX = node.PosX;
            NodePositionY = node.PosY;
        }

        public void onUcNodeViewer_LoadedCompletely(UcNodeViewer loadedNodeViewer)
        {
            if (loadedNodeViewer.Equals(NodeViewer))
            {
                NodeViewer = loadedNodeViewer;
            }
        }

        public event PropertyChangedEventHandler PropertyChanged;

        public void NotifyPropertyChanged(string propName)
        {
            if (this.PropertyChanged != null)
                this.PropertyChanged(this, new PropertyChangedEventArgs(propName));
        }

    }
    

    public class ConverterInputTypeName : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
           return value.ToString().Split('.').Last();
        }

        public object ConvertBack(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            return value;
        }
    }


}
