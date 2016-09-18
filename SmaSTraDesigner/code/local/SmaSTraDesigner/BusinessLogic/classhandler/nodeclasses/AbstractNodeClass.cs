namespace SmaSTraDesigner.BusinessLogic
{
    using classhandler.nodeclasses;
    using System;
    using static ClassManager;

    /// <summary>
    /// Prodives information about a specific type of node.
    /// </summary>
    public abstract class AbstractNodeClass
	{

        #region constructors

        /// <summary>
        /// Constructor.
        /// </summary>
        /// <param name="nodeType">The type of the Node.</param>
        /// <param name="name">This node class's identifying name.</param>
        /// <param name="baseNode">This node class's base node that is cloned for creation of new nodes.</param>
        /// <param name="outputType">This node class's output's data type</param>
        /// <param name="inputTypes">This node class's input's data types</param>
        /// <param name="mainClass">This is the MainClass in the java world</param>
        /// <param name="needsOtherClasses">The other classes needed for the Java class to work</param>
        /// <param name="needsPermissions">The Permissions needed for this element</param>
        protected AbstractNodeClass(NodeType nodeType, string name, string displayName, string description,  
            DataType outputType, 
            string mainClass, string[] needsOtherClasses, string[] needsPermissions,
            ConfigElement[] configuration, ProxyProperty[] proxyProperties,
            DataType[] inputTypes)
		{
			if (String.IsNullOrWhiteSpace(name))
			{
				throw new ArgumentException("String argument 'name' must not be null or empty (incl. whitespace).", "name");
			}
			if (nodeType == NodeType.Transformation  && (inputTypes == null || inputTypes.Length == 0))
			{
				throw new ArgumentException("There must be input types given for a transformation node class", "inputTypes");
			}

            
			this.Name = name;
            this.DisplayName = displayName;
            this.Description = description;
			this.OutputType = outputType;
			this.InputTypes = inputTypes == null ? new DataType[0] : inputTypes;
            this.MainClass = mainClass;
            this.NeedsOtherClasses = needsOtherClasses == null ? new string[0] : needsOtherClasses;
            this.NeedsPermissions = needsPermissions == null ? new string[0] : needsPermissions;
            this.Configuration = configuration == null ? new ConfigElement[0] : configuration;
            this.ProxyProperties = proxyProperties == null ? new ProxyProperty[0] : proxyProperties;
            this.NodeType = nodeType;
		}


        #endregion constructors

        #region vars

        /// <summary>
        /// This is the basenode. It is a cached node.
        /// </summary>
        protected Node baseNode;

        #endregion vars

        #region properties

        /// <summary>
        /// Gets this node class's base node that is cloned for creation of new nodes.
        /// </summary>
        public Node BaseNode {
            get
            {
                if(this.baseNode == null)
                {
                    baseNode = generateBaseNode();
                    baseNode.Class = this;
                    baseNode.Name = this.DisplayName;
                }

                return baseNode;
            }
        }

        /// <summary>
        /// Gets or sets a description for this node class.
        /// </summary>
        public string Description { get; }

        /// <summary>
        /// Gets or sets a display name for this node class.
        /// </summary>
        public string DisplayName { get; set; }

        /// <summary>
        /// Gets this node class's input's data types.
        /// </summary>
        public DataType[] InputTypes { get; }

        /// <summary>
        /// Gets this node class's identifying name.
        /// </summary>
        public string Name { get; }

        /// <summary>
        /// Gets this node class's output's data type
        /// </summary>
        public DataType OutputType { get; }

        /// <summary>
        /// The Node type of this class.
        /// </summary>
        public NodeType NodeType { get; }

        /// <summary>
        /// The main class this is dedicated to.
        /// </summary>
        public string MainClass { get; }

        /// <summary>
        /// Other classes this class needs.
        /// </summary>
        public string[] NeedsOtherClasses { get; }

        /// <summary>
        /// The Permissions needed for this Element.
        /// </summary>
        public string[] NeedsPermissions { get; }

        /// <summary>
        /// The Configuration of this element.
        /// </summary>
        public ConfigElement[] Configuration { get; }

        /// <summary>
        /// The Proxy Properties to use.
        /// </summary>
        public ProxyProperty[] ProxyProperties { get; }

        #endregion properties

        #region overrideable methods

        /// <summary>
        /// This is called when the node is generated FROM THE CACHE!
        /// </summary>
        /// <returns>The base node.</returns>
        protected abstract Node generateBaseNode();


        public override string ToString()
		{
			return String.Format("{0} {1}", this.GetType().Name, this.Name);
		}

		#endregion overrideable methods
	}
}