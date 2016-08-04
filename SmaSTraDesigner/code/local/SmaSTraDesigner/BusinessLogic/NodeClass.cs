namespace SmaSTraDesigner.BusinessLogic
{
    using Newtonsoft.Json;
    using System;
    using System.Collections.ObjectModel;

    /// <summary>
    /// Prodives information about a specific type of node.
    /// </summary>
    public class NodeClass
	{
		#region constructors

		/// <summary>
		/// Constructor.
		/// </summary>
		/// <param name="name">This node class's identifying name.</param>
		/// <param name="baseNode">This node class's base node that is cloned for creation of new nodes.</param>
		/// <param name="outputType">This node class's output's data type</param>
		/// <param name="inputTypes">This node class's input's data types</param>
		public NodeClass(string name, Node baseNode, DataType outputType, DataType[] inputTypes = null)
		{
			if (String.IsNullOrWhiteSpace(name))
			{
				throw new ArgumentException("String argument 'name' must not be null or empty (incl. whitespace).", "name");
			}
			if (baseNode == null)
			{
				throw new ArgumentNullException("baseNode");
			}
			if (baseNode is Transformation && (inputTypes == null || inputTypes.Length == 0))
			{
				throw new ArgumentException("There must be input types given for a transformation node class", "inputTypes");
			}

			this.Name = name;
			this.BaseNode = baseNode;
			this.OutputType = outputType;
			this.InputTypes = inputTypes;

			baseNode.Class = this;
		}

		#endregion constructors

		#region properties

		/// <summary>
		/// Gets this node class's base node that is cloned for creation of new nodes.
		/// </summary>
		public Node BaseNode
		{
			get;
			private set;
		}

		/// <summary>
		/// Gets or sets a description for this node class.
		/// </summary>
		public string Description
		{
			get;
			set;
		}

		/// <summary>
		/// Gets or sets a display name for this node class.
		/// </summary>
		public string DisplayName
		{
			get;
			set;
		}

		/// <summary>
		/// Gets this node class's input's data types.
		/// </summary>
		public DataType[] InputTypes
		{
			get;
			private set;
		}

        /// <summary>
        /// Gets this node class's identifying name.
        /// </summary>
        public string Name
		{
			get;
			private set;
		}

		/// <summary>
		/// Gets this node class's output's data type
		/// </summary>
		public DataType OutputType
		{
			get;
			private set;
		}

		#endregion properties

		#region overrideable methods

		public override string ToString()
		{
			return String.Format("{0} {1}", this.GetType().Name, this.Name);
		}

		#endregion overrideable methods
	}
}