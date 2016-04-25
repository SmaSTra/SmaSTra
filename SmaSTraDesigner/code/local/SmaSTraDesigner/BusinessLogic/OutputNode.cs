namespace SmaSTraDesigner.BusinessLogic
{
	using System;
	using System.Collections.Generic;
	using System.Linq;
	using System.Text;
	using System.Threading.Tasks;

	public class OutputNode : Node
	{
		#region fields

		/// <summary>
		/// TODO: (PS) Comment this.
		/// </summary>
		private Node inputNode = null;

		#endregion fields

		#region properties

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

		#endregion properties
	}
}