namespace SmaSTraDesigner.BusinessLogic
{
	using System;
	using System.Collections.Generic;
	using System.Linq;
	using System.Text;
	using System.Threading.Tasks;

	using Common;

	public struct Connection
	{
		#region constructors

		public Connection(Node outputNode, Node inputNode, int inputIndex)
		{
			if (outputNode == null)
			{
				throw new ArgumentNullException("outputNode");
			}
			if (inputNode == null)
			{
				throw new ArgumentNullException("inputNode");
			}
			if (outputNode.Class == null)
			{
				throw new ArgumentException("outputNode has no class.", "outputNode");
			}
			if (!(inputNode is OutputNode))
			{
				if (inputNode.Class == null)
				{
					throw new ArgumentException("inputNode has no class.", "inputNode");
				}
				if (inputIndex < 0 || inputIndex >= inputNode.Class.InputTypes.Length)
				{
					throw new ArgumentOutOfRangeException(String.Concat("Arguemnt 'inputIndex' must be between ", 0, " and ", inputNode.Class.InputTypes.Length, " (# of input types)."), "inputIndex");
				}
			}

			this.OutputNode = outputNode;
			this.InputNode = inputNode;
			this.InputIndex = inputIndex;
		}

		#endregion constructors

		#region properties

		// TODO: (PS) Comment this.
		public int InputIndex
		{
			get;
			set;
		}

		// TODO: (PS) Comment this.
		public Node InputNode
		{
			get;
			set;
		}

		// TODO: (PS) Comment this.
		public Node OutputNode
		{
			get;
			set;
		}

		#endregion properties

		#region overrideable methods

		public override bool Equals(object obj)
		{
			Connection other = (Connection)obj;

			return object.Equals(this.InputNode, other.InputNode) &&
				object.Equals(this.OutputNode, other.OutputNode) &&
				this.InputIndex == other.InputIndex;
		}

		public override int GetHashCode()
		{
			return HashCodeOperations.Combine(this.InputNode, this.OutputNode, this.InputIndex);
		}

		#endregion overrideable methods
	}
}