namespace SmaSTraDesigner.BusinessLogic
{
	using System;
	using System.Linq;

	/// <summary>
	/// Represents a transformation that combines data inputs to another output value.
	/// </summary>
	public class Transformation : Node
	{
		#region fields

		/// <summary>
		/// The Nodes that provide data input for this transformation.
		/// </summary>
		private Node[] inputNodes = null;

		#endregion fields

		#region properties

		/// <summary>
		/// Gets the Nodes that provide data input for this transformation.
		/// </summary>
		public Node[] InputNodes
		{
			get
			{
				return this.inputNodes.ToArray();
			}
            set
            {
                this.inputNodes = value;
            }
		}

		#endregion properties

		#region overrideable methods

		/// <summary>
		/// Called when the Class property changed its value.
		/// </summary>
		/// <param name="oldValue">The old value.</param>
		/// <param name="newValue">The new value.</param>
		protected override void OnClassChanged(NodeClass oldValue, NodeClass newValue)
		{
			base.OnClassChanged(oldValue, newValue);

			this.inputNodes = new Node[this.Class.InputTypes.Length];
		}

        public override object Clone()
        {
            Transformation clonedNode = (Transformation)base.Clone();
            clonedNode.InputNodes = this.InputNodes;

            return clonedNode;
        }

        #endregion overrideable methods

        #region methods

        /// <summary>
        /// Adds an input node at the first free space.
        /// </summary>
        /// <param name="inputNode"></param>
        /// <returns></returns>
        public bool AddInput(Node inputNode)
		{
			if (this.Class == null)
			{
				throw new InvalidOperationException("This transformation has no set class.");
			}

			for (int i = 0; i < this.inputNodes.Length; i++)
			{
				if (this.inputNodes[i] == null)
				{
					this.inputNodes[i] = inputNode;

					return true;
				}
			}

			return false;
		}

		/// <summary>
		/// Sets input node at the specified index.
		/// </summary>
		/// <param name="index">input index.</param>
		/// <param name="inputNode">input providing node.</param>
		public void SetInput(int index, Node inputNode)
		{
			if (this.Class == null)
			{
				throw new InvalidOperationException("This transformation has no set class.");
			}
            if(this.inputNodes[index] != null && inputNode != null)
            {
                throw new InvalidOperationException("This InputNode is already occupied.");
            }
			this.inputNodes[index] = inputNode;
        }

		#endregion methods
	}
}