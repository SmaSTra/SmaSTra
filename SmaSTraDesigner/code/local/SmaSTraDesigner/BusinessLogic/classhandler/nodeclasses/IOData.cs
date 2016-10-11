using System;
using System.ComponentModel;
using SmaSTraDesigner.BusinessLogic.classhandler.nodeclasses;
using SmaSTraDesigner.BusinessLogic.nodes;

namespace SmaSTraDesigner.BusinessLogic.classhandler
{
    public class IOData : INotifyPropertyChanged, ICloneable
    {
        private Node _parentNode;
        private DataType _type;
        private string _value; //TODO: needs better, typesafe solution
        private Node _connectedNode;
        private string _inputGuiString;

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
                return _type;
            }
            private set
            {
                if (_type == value) return;

                _type = value;
                this.NotifyPropertyChanged("Type");
            }
        }

        public string Value
        {
            get
            {
                return _value;
            }
            set
            {
                if (this._value == value) return;

                if (string.IsNullOrWhiteSpace(this._value))
                {
                    ParentNode?.removeConnection(this);
                }

                InputGUIString = string.IsNullOrWhiteSpace(value) ? "{" + Type.MinimizedName + "}" : value;
                this._value = value;
                this.NotifyPropertyChanged("Value");
            }
        }

        public Node ConnectedNode
        {
            get
            {
                return _connectedNode;
            }
            set
            {
                if (_connectedNode == value) return;

                _connectedNode = value;
                this.NotifyPropertyChanged("ConnectedNode");
                if (_connectedNode == null) return;

                this.Value = "";
                InputGUIString = _connectedNode.Name;
            }
        }

        public Node ParentNode
        {
            get
            {
                return _parentNode;
            }
            set
            {
                if (_parentNode == value) return;

                _parentNode = value;
                this.NotifyPropertyChanged("ParentNode");
            }
        }

        public string InputGUIString
        {
            get
            {
                return _inputGuiString;
            }
            set
            {
                if (_inputGuiString == value) return;

                _inputGuiString = value;
                this.NotifyPropertyChanged("InputGUIString");
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
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propName));
        }
    }
}
