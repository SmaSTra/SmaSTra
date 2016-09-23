namespace SmaSTraDesigner.BusinessLogic
{
    using System;
    using System.ComponentModel;
    using classhandler;
    using System.Collections.ObjectModel;
    using System.Linq;
    using classhandler.nodeclasses;
    using utils;


    /// <summary>
    /// Base class for all nodes.
    /// Provides information about a specific node in a TransformationTree.
    /// Implemented variations are DataSource, Transformation and OutputNode.
    /// </summary>
    public class Node : INotifyPropertyChanged
    {
        #region fields

        /// <summary>
        /// NodeClass instance that provides information about this node's type.
        /// </summary>
        protected AbstractNodeClass clazz;

        /// <summary>
        /// The Input nodes to use.
        /// </summary>
        protected Node[] inputNodes = new Node[0];

        /// <summary>
        /// This node's display name (is used as an identifier).
        /// </summary>
        private string name = null;

        /// <summary>
        /// This node's X position on the transformation tree's graph (spread out on a 2D plane).
        /// </summary>
        private double posX = 0;

        /// <summary>
        /// This node's Y position on the transformation tree's graph (spread out on a 2D plane).
        /// </summary>
        private double posY = 0;

        #endregion fields

        #region events

        /// <summary>
        /// Is raised whenever a compatible property changes its value.
        /// </summary>
        public event PropertyChangedEventHandler PropertyChanged;

        #endregion events

        #region properties

        /// <summary>
        /// This is the Unique identifier for the Node.
        /// </summary>
        public string NodeUUID { get; protected set; }

        /// <summary>
        /// Gets or sets the Class property value.
        /// NodeClass instance that provides information about this node's type.
        /// </summary>
        public AbstractNodeClass Class
        {
            get
            {
                return this.clazz;
            }
            internal set
            {
                if (value != this.clazz)
                {
                    AbstractNodeClass oldValue = this.clazz;
                    this.clazz = value;
                    this.OnClassChanged(oldValue, value);
                }
            }
        }

        /// <summary>
        /// Gets or sets the Name property value.
        /// This node's display name (is used as an identifier).
        /// </summary>
        public string Name
        {
            get
            {
                return this.name;
            }
            set
            {
                if (value != this.name)
                {
                    string oldValue = this.name;
                    this.name = value;
                    this.OnPropertyChanged("Name");
                }
            }
        }

        /// <summary>
        /// Gets or sets the PosX property value.
        /// This node's X position on the transformation tree's graph (spread out on a 2D plane).
        /// </summary>
        public double PosX
        {
            get
            {
                return this.posX;
            }
            set
            {
                if (value != this.posX)
                {
                    double oldValue = this.posX;
                    this.posX = value;
                    this.OnPropertyChanged("PosX");
                }
            }
        }

        /// <summary>
        /// Gets or sets the PosY property value.
        /// This node's Y position on the transformation tree's graph (spread out on a 2D plane).
        /// </summary>
        public double PosY
        {
            get
            {
                return this.posY;
            }
            set
            {
                if (value != this.posY)
                {
                    double oldValue = this.posY;
                    this.posY = value;
                    this.OnPropertyChanged("PosY");
                }
            }
        }

        /// <summary>
        /// Gets the TransformationTree instance this node belongs to.
        /// </summary>
        public TransformationTree Tree
        {
            get;
            internal set;
        }

        public Node[] InputNodes
        {
            get { return inputNodes.ToArray();  }
            protected set { this.inputNodes = value; }
        }

        #endregion properties

        #region overrideable methods


        /// <summary>
        /// This indicates to clear all inputs of the node programmatically.
        /// No UI stuff going on here!
        /// </summary>
        public virtual void ClearInputs()
        {
            this.InputNodes = new Node[InputNodes.Count()];
        }


        /// <summary>
        /// Creates the Input on that index.
        /// </summary>
        /// <param name="inputIndex">to set</param>
        /// <param name="outputNode">to set</param>
        public virtual void SetInput(int inputIndex, Node inputNode)
        {
            if (inputIndex < 0 || inputIndex >= inputNodes.Count())
            {
                throw new InvalidOperationException("Got input index: " + inputIndex + " but only got " + inputNodes.Count() + " Slots.");
            }

            inputNodes[inputIndex] = inputNode;
        }


        /// <summary>
        /// Called when the Class property changed its value.
        /// </summary>
        /// <param name="oldValue">The old value.</param>
        /// <param name="newValue">The new value.</param>
        protected virtual void OnClassChanged(AbstractNodeClass oldValue, AbstractNodeClass newValue)
        {
            //Create new Input-IOData, to force new Bindings.
            InputIOData = newValue.InputTypes
                .Select(t => new IOData(t,""))
                .ToObservableCollection();
            
            //Create new Output-IOData, to force new Bindings.
            OutputIOData = new IOData(newValue.OutputType, null);

            //Create new Configuration to force new Bindings.
            this.Configuration = clazz
                .Configuration.Select(c => c.GenerateDataElement())
                .ToObservableCollection();

            //Set a new UUID for the node:
            this.NodeUUID = Guid.NewGuid().ToString();

            //Set the Input data:
            this.InputNodes = new Node[newValue.InputTypes.Count()];
		}

		public override string ToString()
		{
			return String.Format("{0} {1}", this.GetType().Name, this.Name);
		}

		#endregion overrideable methods

		#region methods

		/// <summary>
		/// Raises the PropertyChanged event.
		/// </summary>
		/// <param name="propertyName">Name of the property that changed values.</param>
		protected virtual void OnPropertyChanged(string propertyName)
		{
			if (this.PropertyChanged != null)
			{
				this.PropertyChanged(this, new PropertyChangedEventArgs(propertyName));
			}
		}

        /// <summary>
        /// Forces a Change of the Node UUID.
        /// THIS SHOULD ONLY BE USED FOR DESERIALIZATION!!!
        /// </summary>
        /// <param name="NewUUID">To set.</param>
        public void ForceUUID(string NewUUID)
        {
            if (NewUUID != null)
            {
                this.NodeUUID = NewUUID;
            }
        }


        public override bool Equals(object obj)
        {
            if (obj is Node) return (obj as Node).NodeUUID.Equals(this.NodeUUID);
            return base.Equals(obj);
        }

        public override int GetHashCode()
        {
            return NodeUUID.GetHashCode();
        }

        #endregion methods

        #region not sorted yet

        private IOData outputIOData;
        public IOData OutputIOData
        {
            get
            {
                return outputIOData;
            }
            set
            {
                if(outputIOData != value)
                {
                    outputIOData = value;
                }
            }
        }

        protected ObservableCollection<IOData> inputIOData = new ObservableCollection<IOData>();
        public ObservableCollection<IOData> InputIOData
        {
            get
            {
                return inputIOData;
            }
            set
            {
                if (inputIOData != value)
                {
                    inputIOData = value;
                }
            }
        }


        protected ObservableCollection<DataConfigElement> configuration = new ObservableCollection<DataConfigElement>();
        public ObservableCollection<DataConfigElement> Configuration
        {
            get
            {
                return configuration;
            }
            set
            {
                if (configuration != value)
                {
                    configuration = value;
                }
            }
        }

        #endregion not sorted yet
    }
}