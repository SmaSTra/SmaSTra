using SmaSTraDesigner.BusinessLogic.nodes;

namespace SmaSTraDesigner.BusinessLogic
{
	using System;

	using Common;

	/// <summary>
	/// Contains information about node connection within a TransformationTree.
	/// </summary>
	public struct Connection
	{
		#region constructors

		/// <summary>
		/// Constructor
		/// </summary>
		/// <param name="outputNode">Node that provides data output</param>
		/// <param name="inputNode">Node that the data is fed to.</param>
		/// <param name="inputIndex">The index of the input to where the data is fed.</param>
		public Connection(Node outputNode, Node inputNode, int inputIndex)
		{
			// Validate arguments.
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

		/// <summary>
		/// The index of the input to where the data is fed.
		/// </summary>
		public int InputIndex
		{
			get;
			set;
		}

		/// <summary>
		/// Node that the data is fed to.
		/// </summary>
		public Node InputNode
		{
			get;
			set;
		}

		/// <summary>
		/// Node that provides data output
		/// </summary>
		public Node OutputNode
		{
			get;
			set;
		}

		#endregion properties

		#region overrideable methods
		
		public override bool Equals(object obj)
		{
            if (obj == null) return false;
            if (!(obj is Connection)) return false;
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