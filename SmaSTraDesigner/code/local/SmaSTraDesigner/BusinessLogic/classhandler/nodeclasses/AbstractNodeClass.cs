using System;
using System.IO;
using SmaSTraDesigner.BusinessLogic.classhandler.nodeclasses.extras;
using SmaSTraDesigner.BusinessLogic.nodes;

namespace SmaSTraDesigner.BusinessLogic.classhandler.nodeclasses
{
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
        /// <param name="displayName">The Display name to show.</param>
        /// <param name="description">The description of the Element</param>
        /// <param name="creator">The person who created this element</param>
        /// <param name="outputType">This node class's output's data type</param>
        /// <param name="mainClass">This is the MainClass in the java world</param>
        /// <param name="needsOtherClasses">The other classes needed for the Java class to work</param>
        /// <param name="neededExtras">The Extras needed for the Class.</param>
        /// <param name="configuration">The Configuration to display.</param>
        /// <param name="proxyProperties">The Proxy properties to display to the outside</param>
        /// <param name="inputTypes">This node class's input's data types</param>
        /// <param name="userCreated">If the element is user created</param>
        /// <param name="nodePath">The File-Path to the root of the node in the file system</param>
        protected AbstractNodeClass(ClassManager.NodeType nodeType, string name, string displayName, string description, string creator,  DataType outputType, 
            string mainClass, string[] needsOtherClasses, INeedsExtra[] neededExtras,
            ConfigElement[] configuration, ProxyProperty[] proxyProperties,
            DataType[] inputTypes, bool userCreated, string nodePath)
		{
			if (String.IsNullOrWhiteSpace(name))
			{
				throw new ArgumentException("String argument 'name' must not be null or empty (incl. whitespace).", nameof(name));
			}

			if (nodeType == ClassManager.NodeType.Transformation  && (inputTypes == null || inputTypes.Length == 0))
			{
				throw new ArgumentException("There must be input types given for a transformation node class", nameof(inputTypes));
			}

            
			this.Name = name;
            this.DisplayName = displayName;
            this.Description = description;
            this.Creator = creator;
			this.OutputType = outputType;
			this.InputTypes = inputTypes ?? new DataType[0];
            this.MainClass = mainClass;
            this.NeedsOtherClasses = needsOtherClasses ?? new string[0];
            this.NeededExtras = neededExtras ?? new INeedsExtra[0];
            this.Configuration = configuration ?? new ConfigElement[0];
            this.ProxyProperties = proxyProperties ?? new ProxyProperty[0];
            this.NodeType = nodeType;
            this.UserCreated = userCreated;
            this.NodePath = nodePath;
		}


        #endregion constructors

        #region variables

        /// <summary>
        /// The cached variable for Source.
        /// </summary>
        private string _source = null;

        #endregion variables


        #region properties

        /// <summary>
        /// The File path for this node.
        /// </summary>
        public string NodePath { get; }

        /// <summary>
        /// Gets or sets a description for this node class.
        /// </summary>
        public string Description { get; }

        /// <summary>
        /// Gets the creator of this element.
        /// </summary>
        public string Creator { get; }

        /// <summary>
        /// Gets or sets a display name for this node class.
        /// </summary>
        public string DisplayName { get; }

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
        public ClassManager.NodeType NodeType { get; }

        /// <summary>
        /// The main class this is dedicated to.
        /// </summary>
        public string MainClass { get; }

        /// <summary>
        /// Other classes this class needs.
        /// </summary>
        public string[] NeedsOtherClasses { get; }

        /// <summary>
        /// The Configuration of this element.
        /// </summary>
        public ConfigElement[] Configuration { get; }

        /// <summary>
        /// The Proxy Properties to use.
        /// </summary>
        public ProxyProperty[] ProxyProperties { get; }

        /// <summary>
        /// The needed Extras this class has.
        /// </summary>
        public INeedsExtra[] NeededExtras { get; }

        /// <summary>
        /// This indicates if the class was created by a user of a default class.
        /// </summary>
        public bool UserCreated { get; }

        /// <summary>
        /// The Source code of the Main-Class file.
        /// May be a bit too much at this time.
        /// Should be changed to the Method-Call only later.
        /// </summary>
        public string SourceCode
        {
            get
            {
                //Got the cached source.
                if(_source != null)
                {
                    return _source;
                }

                //We have no MainClass:
                if (string.IsNullOrWhiteSpace(MainClass))
                {
                    _source = "No Source found";
                    return _source;
                }

                //We have a Main-Class, so we can browse our Source-Code.
                _source = ReadSourceCode() ?? "No Source found.";
                return _source;
            }
        }

        #endregion properties

        #region overrideable methods

        /// <summary>
        /// Reads the Source code.
        /// By default, this reads the complete Class.
        /// </summary>
        /// <returns>The source code wanted.</returns>
        protected virtual string ReadSourceCode()
        {
            //Start reading the Source-File:
            string file = Path.Combine(NodePath, MainClass.Replace('.', Path.DirectorySeparatorChar) + ".java");
            if (!File.Exists(file))
            {
                return null;
            }

            //Found the File -> Read!
            return File.ReadAllText(file);
        }

        /// <summary>
        /// This is called when a new node is generated.
        /// </summary>
        /// <returns>The node to work with.</returns>
        public abstract Node GenerateNode();


        public override string ToString()
		{
			return $"{this.GetType().Name} {this.Name}";
		}

		#endregion overrideable methods
	}
}