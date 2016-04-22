using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SmaSTraDesigner.BusinessLogic
{
	public class OutputNode : Node
	{
		/// <summary>
		/// TODO: (PS) Comment this.
		/// </summary>
		private Node inputNode = null;

		/// <summary>
		/// Gets or sets the InputNode property value.
		/// TODO: (PS) Comment this.
		/// </summary>
		public Node InputNode
		{
			get
			{
				return this.inputNode;
			}
			set
			{
				if (value != this.inputNode)
				{
					Node oldValue = this.inputNode;
					this.inputNode = value;
					this.OnPropertyChanged("InputNode");
				}
			}
		}
	}
}
