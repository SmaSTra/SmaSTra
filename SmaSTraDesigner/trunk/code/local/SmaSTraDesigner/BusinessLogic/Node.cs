using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SmaSTraDesigner.BusinessLogic
{
	public class Node : INotifyPropertyChanged, ICloneable
	{
		public event PropertyChangedEventHandler PropertyChanged;

		/// <summary>
		/// TODO: (PS) Comment this.
		/// </summary>
		private string name = null;

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

		public Node()
		{
		}

		// TODO: (PS) Comment this.
		public NodeClass Class
		{
			get;
			internal set;
		}

		public virtual object Clone()
		{
			return this.MemberwiseClone();
		}

		protected void OnPropertyChanged(string propertyName)
		{
			if (this.PropertyChanged != null)
			{
				this.PropertyChanged(this, new PropertyChangedEventArgs(propertyName));
			}
		}
	}
}
