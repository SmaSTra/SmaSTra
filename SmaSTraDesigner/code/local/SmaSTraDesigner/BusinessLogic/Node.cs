namespace SmaSTraDesigner.BusinessLogic
{
	using System;
	using System.Collections.Generic;
	using System.ComponentModel;
	using System.Linq;
	using System.Text;
	using System.Threading.Tasks;

	public class Node : INotifyPropertyChanged, ICloneable
	{
		#region fields

		/// <summary>
		/// TODO: (PS) Comment this.
		/// </summary>
		private NodeClass clazz;

		/// <summary>
		/// TODO: (PS) Comment this.
		/// </summary>
		private string name = null;

		/// <summary>
		/// TODO: (PS) Comment this.
		/// </summary>
		private double posX = 0;

		/// <summary>
		/// TODO: (PS) Comment this.
		/// </summary>
		private double posY = 0;

		#endregion fields

		#region constructors

		public Node()
		{
		}

		#endregion constructors

		#region events

		public event PropertyChangedEventHandler PropertyChanged;

		#endregion events

		#region properties

		/// <summary>
		/// Gets or sets the Class property value.
		/// TODO: (PS) Comment this.
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
		/// TODO: (PS) Comment this.
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
		/// TODO: (PS) Comment this.
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
		/// TODO: (PS) Comment this.
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

		// TODO: (PS) Comment this.
		internal TransformationTree Tree
		{
			get;
			set;
		}

		#endregion properties

		#region overrideable methods

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