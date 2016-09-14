namespace SmaSTraDesigner.BusinessLogic
{
    using System;
    using System.ComponentModel;
    using classhandler;
    using System.Collections.ObjectModel;


    /// <summary>
    /// Base class for all nodes.
    /// Provides information about a specific node in a TransformationTree.
    /// Implemented variations are DataSource, Transformation and OutputNode.
    /// </summary>
    public class Node : INotifyPropertyChanged, ICloneable
	{
		#region fields

		/// <summary>
		/// NodeClass instance that provides information about this node's type.
		/// </summary>
		protected NodeClass clazz;

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
        public string NodeUUID{ get; protected set; }

		/// <summary>
		/// Gets or sets the Class property value.
		/// NodeClass instance that provides information about this node's type.
		/// </summary>
		public NodeClass Class
		{
			get
			{
				return this.clazz;
			}
			internal set
			{
				if (value != this.clazz)
				{
					NodeClass oldValue = this.clazz;
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

        #endregion properties

        #region overrideable methods


        /// <summary>
        /// This indicates to clear all inputs of the node programmatically.
        /// No UI stuff going on here!
        /// </summary>
        public virtual void ClearInputs()
        { }

        /// <summary>
        /// Clones this node.
        /// </summary>
        /// <returns>A clone of this node.</returns>
        public virtual object Clone()
		{
			Node clone = (Node)this.MemberwiseClone();
            clone.NodeUUID = Guid.NewGuid().ToString();

            clone.InputIOData = new ObservableCollection<IOData>();
            foreach(IOData ioData in this.InputIOData)
            {
                clone.InputIOData.Add(new IOData(ioData.Type, " "));
            }
            return clone;
		}

		/// <summary>
		/// Called when the Class property changed its value.
		/// </summary>
		/// <param name="oldValue">The old value.</param>
		/// <param name="newValue">The new value.</param>
		protected virtual void OnClassChanged(NodeClass oldValue, NodeClass newValue)
		{
            InputIOData.Clear();
            foreach(DataType inputType in newValue.InputTypes)
            {
                InputIOData.Add(new IOData(inputType, ""));
            }

            OutputIOData = new IOData(newValue.OutputType, null);

            //Set a new UUID for the node:
            this.NodeUUID = Guid.NewGuid().ToString();
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

        private ObservableCollection<IOData> inputIOData = new ObservableCollection<IOData>();
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

        #endregion not sorted yet
    }
}