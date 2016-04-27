namespace SmaSTraDesigner.BusinessLogic
{
	/// <summary>
	/// Represents a TransformationTree's data output.
	/// </summary>
	public class OutputNode : Node
	{
		#region fields

		/// <summary>
		/// The node that provides input data to the tree's output.
		/// </summary>
		private Node inputNode = null;

		#endregion fields

		#region properties

		/// <summary>
		/// Gets or sets the InputNode property value.
		/// The node that provides input data to the tree's output.
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