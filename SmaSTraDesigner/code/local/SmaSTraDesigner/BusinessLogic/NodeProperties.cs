using SmaSTraDesigner.BusinessLogic.classhandler;
using SmaSTraDesigner.BusinessLogic.classhandler.nodeclasses;
using System;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Text.RegularExpressions;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Input;
using SmaSTraDesigner.BusinessLogic.nodes;

namespace SmaSTraDesigner.BusinessLogic
{
    public class NodeProperties : INotifyPropertyChanged

    {
       
        private string defaultString = "---";
        private double defaultNumber = 0;

        

        private Node activeNode = null;
        public Node ActiveNode
        {
            get { return activeNode; }
            set {
                if (ActiveNode != null)
                {
                    activeNode.PropertyChanged -= OnNodePropertyChanged;
                }
                    activeNode = value;
                if (ActiveNode != null)
                {
                    ActiveNode.PropertyChanged += OnNodePropertyChanged;
                }
                    updateNodeProperties(ActiveNode);
                    OnActiveNodeChanged(ActiveNode);
                    this.NotifyPropertyChanged("ActiveNode");
            }
        }

        public string NodeName
        {
            get
            {
                if (ActiveNode != null)
                {
                    return ActiveNode.Name;
                }
                else
                {
                    return defaultString;
                }
            }
            set
            {
                if (ActiveNode != null) {
                    ActiveNode.Name = value;
                    this.NotifyPropertyChanged("NodeName");
                }
            }
        }

        public string NodeClass
        {
            get
            {
                if (ActiveNode != null && ActiveNode.Class != null)
                {
                    return ActiveNode.Class.DisplayName;
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
                if (ActiveNode != null)
                {
                    return ActiveNode.InputIOData;
                } else
                {
                    return null;
                }
            }
        }

        public ObservableCollection<DataConfigElement> NodeConfiguration
        {
            get
            {
                if (ActiveNode != null)
                {
                    return ActiveNode.Configuration;
                }
                else
                {
                    return null;
                }
            }
        }

        public string NodeClassOutput
        {
            get
            {
                if (ActiveNode != null && ActiveNode.Class != null && ActiveNode.Class.OutputType != null)
                {
                        return ActiveNode.Class.OutputType.Name.Split('.').Last();
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
                if (ActiveNode != null)
                {
                    return ActiveNode.PosX;
                }
                else
                {
                    return defaultNumber;
                }
            }
            set
            {
                if (ActiveNode != null && value != ActiveNode.PosX)
                {
                    ActiveNode.PosX = value;
                }
                this.NotifyPropertyChanged("NodePositionX");
            }
        }

        public double NodePositionY
        {
            get
            {
                if (ActiveNode != null)
                {
                    return ActiveNode.PosY;
                }
                else
                {
                    return defaultNumber;
                }
            }
            set
            {
                if (ActiveNode != null && value != ActiveNode.PosY)
                {
                    ActiveNode.PosY = value;
                }
                this.NotifyPropertyChanged("NodePositionY");

            }
        }

        public string NodeClassDescription
        {
            get
            {
                if (ActiveNode != null && ActiveNode.Class != null)
                {
                    return ActiveNode.Class.Description;
                }
                else
                {
                    return defaultString;
                }
            }
        }

        

        protected virtual void OnNodePropertyChanged(object sender, PropertyChangedEventArgs e)
        {
            Node node = sender as Node; //TODO: Update only relevant properties instead of all
            updateNodeProperties(node);
        }

        protected virtual void OnActiveNodeChanged(Node node)
        {
            this.NotifyPropertyChanged("NodeClass");
            this.NotifyPropertyChanged("NodeClassDescription");
            this.NotifyPropertyChanged("NodeInputIOData");
            this.NotifyPropertyChanged("NodeClassOutput");


        }

        private void updateNodeProperties(Node node)
        {
            if (node != null)
            {
                NodeName = node.Name;
                NodePositionX = node.PosX;
                NodePositionY = node.PosY;
            } else
            {
                this.NotifyPropertyChanged("NodeName");
                this.NotifyPropertyChanged("NodePositionX");
                this.NotifyPropertyChanged("NodePositionY");
            }
        }

        public void onTextBoxInput(object sender, TextCompositionEventArgs e)
        {
            TextBox inputBox = sender as TextBox;
            IOData inputData = inputBox.DataContext as IOData;
            e.Handled = !textAccepted(e.Text, inputData.Type);
        }

        private bool textAccepted(string input, DataType dataType)
        {
            Regex allowedExpression;
            switch (dataType.Name){
                case "double":
                    allowedExpression = new Regex("[^0-9.-]+");
                    break;
                case "int":
                    allowedExpression = new Regex("[^0-9-]+");
                    break;
                case "de.tu_darmstadt.smastra.sensors.Vector3d":
                    allowedExpression = new Regex("[^0-9.,-]+");
                    break;
                case "de.tu_darmstadt.smastra.sensors.Picture":
                    return false;
                default:
                    return true; // allow everything
            }
            return !allowedExpression.IsMatch(input);
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
