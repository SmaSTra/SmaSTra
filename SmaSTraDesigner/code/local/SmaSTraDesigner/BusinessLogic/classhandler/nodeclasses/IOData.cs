using System;
using System.ComponentModel;

namespace SmaSTraDesigner.BusinessLogic.classhandler
{
    public class IOData : INotifyPropertyChanged, ICloneable
    {
        private Node parentNode;
        private DataType type;
        private string value; //TODO: needs better, typesafe solution
        private Node connectedNode;
        private string inputGUIString;

        public IOData(DataType type, string value)
        {
            this.Type = type;
            this.Value = value;
            this.InputGUIString = "{" + Type.MinimizedName + "}";
        }

        public DataType Type
        {
            get
            {
                return type;
            }
            set
            {
                if (type != value)
                {
                    type = value;
                    this.NotifyPropertyChanged("Type");
                }
            }
        }

        public string Value
        {
            get
            {
                return value;
            }
            set
            {
                if (this.value != value)
                {
                    if (string.IsNullOrWhiteSpace(this.value) && ParentNode != null)
                    {
                        ParentNode.removeConnection(this);
                    }
                    InputGUIString = string.IsNullOrWhiteSpace(value) ? "{" + Type.MinimizedName + "}" : value;
                    this.value = value;
                    this.NotifyPropertyChanged("Value");
                }
            }
        }

        public Node ConnectedNode
        {
            get
            {
                return connectedNode;
            }
            set
            {
                if (connectedNode != value)
                {
                    connectedNode = value;
                    this.NotifyPropertyChanged("ConnectedNode");
                    if(connectedNode != null)
                    {
                        this.Value = "";
                        InputGUIString = connectedNode.Name;
                    }
                }
            }
        }

        public Node ParentNode
        {
            get
            {
                return parentNode;
            }
            set
            {
                if (parentNode != value)
                {
                    parentNode = value;
                    this.NotifyPropertyChanged("ParentNode");
                }
            }
        }

        public string InputGUIString
        {
            get
            {
                return inputGUIString;
            }
            set
            {
                if (inputGUIString != value)
                {
                    inputGUIString = value;
                    this.NotifyPropertyChanged("InputGUIString");
                }
            }
        }


        /// <summary>
        /// If the value is set to something.
        /// </summary>
        public bool IsSet()
        {
            return !string.IsNullOrWhiteSpace(Value);
        }


        public object Clone()
        {
            return MemberwiseClone();
        }


        public event PropertyChangedEventHandler PropertyChanged;

        public void NotifyPropertyChanged(string propName)
        {
            if (this.PropertyChanged != null)
                this.PropertyChanged(this, new PropertyChangedEventArgs(propName));
        }
    }
}
