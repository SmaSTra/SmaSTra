namespace SmaSTraDesigner.BusinessLogic
{
	using System;
	using System.Collections.Generic;
	using System.ComponentModel;
	using System.Linq;
	using System.Text;
	using System.Threading.Tasks;

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
		private NodeClass clazz;

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
		/// Clones this node.
		/// </summary>
		/// <returns>A clone of this node.</returns>
		public virtual object Clone()
		{
			return this.MemberwiseClone();
		}

		/// <summary>
		/// Called when the Class property changed its value.
		/// </summary>
		/// <param name="oldValue">The old value.</param>
		/// <param name="newValue">The new value.</param>
		protected virtual void OnClassChanged(NodeClass oldValue, NodeClass newValue)
		{
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
		protected void OnPropertyChanged(string propertyName)
		{
			if (this.PropertyChanged != null)
			{
				this.PropertyChanged(this, new PropertyChangedEventArgs(propertyName));
			}
		}

		#endregion methods
	}
}