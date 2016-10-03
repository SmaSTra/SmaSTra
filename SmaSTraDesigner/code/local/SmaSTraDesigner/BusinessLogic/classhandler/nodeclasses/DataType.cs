namespace SmaSTraDesigner.BusinessLogic
{
    using config;
    using Newtonsoft.Json.Linq;
    using System;
    using System.Collections.Generic;
    using System.Diagnostics;
    using System.IO;
    using System.Linq;
    using utils;

    /// <summary>
    /// Stores information about a data type used by nodes for their inpot or output data.
    /// </summary>
    public class DataType
    {

        /// <summary>
        /// The List of DataTypes present in the system.
        /// </summary>
        private static List<DataType> knownTypes = new List<DataType>();


        #region constructors

        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="name">Data type name (is used as a unique identifier)</param>
        private DataType(string name) : this(name, "{0}", new string[] { "Object" } )
		{}

        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="name">Data type name (is used as a unique identifier)</param>
        private DataType(string name, string template, string type) 
            : this(name, template, new string[] { type } )
        {}

        /// <summary>
        /// This is a special constructor for datatypes with fixed values.
        /// </summary>
        /// <param name="name"></param>
        /// <param name="values"></param>
        private DataType(string name, string[] values)
            : this(name)
        {
            this.Creatable = true;
            this.FixedValues = values == null ? new String[0] : values;
        }

        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="name">Data type name (is used as a unique identifier)</param>
        private DataType(string name, string template, string[] types = null, bool creatable = true)
        {
            System.Diagnostics.Debug.Print("DataType created: " + name);
            if (String.IsNullOrWhiteSpace(name))
            {
                throw new ArgumentException("String argument 'name' must not be null or empty (incl. whitespace).", "name");
            }

            this.Name = name;
            this.MinimizedName = minimizeToClass(name);

            this.TypeTemplate = template;
            this.TypeTemplateVars = types == null ? new string[] { "object" } : types;
            this.Creatable = true;
        }

        #endregion constructors

        #region properties

        /// <summary>
        /// Data type name (is used as a unique identifier)
        /// </summary>
        public string Name { get; }

        /// <summary>
        /// The Name of the Class minimized to Java criteria.
        /// </summary>
        public string MinimizedName { get; }


        /// <summary>
        /// The Template to use to create a new Object as Source.
        /// </summary>
        public string TypeTemplate { get; }


        /// <summary>
        /// The Template to use to create a new Object as Source.
        /// </summary>
        public string[] TypeTemplateVars { get; }

        /// <summary>
        /// If the element may be created by hand.
        /// </summary>
        public bool Creatable { get; }

        /// <summary>
        /// If this is not empty, this type can only have fixed values.
        /// This is for example for Enums.
        /// </summary>
        public String[] FixedValues { get; }


        #endregion properties

        #region overrideable methods

        public override bool Equals(object obj)
		{
			DataType other = obj as DataType;
			if (other == null)
			{
				return false;
			}

			return String.Equals(this.Name, other.Name, StringComparison.InvariantCultureIgnoreCase);
		}

		public override int GetHashCode()
		{
			return this.Name.GetHashCode();
		}

		public override string ToString()
		{
			return String.Format("{0} {1}", this.GetType().Name, this.Name);
		}

		#endregion overrideable methods

		#region methods

		/// <summary>
		/// Checks whether this dataType can be implicitly converted into another.
		/// 
		/// NOT IMPLEMENTED YET!
		/// Currently analogous to Equals method.
		/// </summary>
		/// <param name="other">data type to check for implicit conversion.</param>
		/// <returns></returns>
		public bool CanConvertTo(DataType other)
		{
			// TODO: (PS) Implement this.
			return this.Equals(other);
		}

        /// <summary>
        /// Copies the Element.
        /// </summary>
        /// <returns></returns>
        public DataType Copy()
        {
            return new DataType(Name);
        }

        /// <summary>
        /// This gets the last part of a class from an complete Class name.
        /// Eg.: java.util.List -> List.
        /// </summary>
        /// <param name="completeClassName"> This is the complete class name. For example 'java.util.List'</param>
        /// <returns>The last part of the Class name.</returns>
        public static string minimizeToClass(string completeClassName)
        {
            return completeClassName.Split('.').Last();
        }

        #endregion methods

        #region StaticMethods

        /// <summary>
        /// Gets the cached Type.
        /// Creates a New one, if the cached does not exist.
        /// </summary>
        /// <param name="name">To get</param>
        public static DataType GetCachedType(string name)
        {
            DataType first = knownTypes
                .FirstOrDefault(t => t.Name == name);

            if(first == null)
            {
                first = new DataType(name);
                knownTypes.Add(first);
            }

            return first;
        }

        /// <summary>
        /// Gets Temp non registered types.
        /// Does NOT register a new one if not exists.
        /// </summary>
        /// <param name="name">To get</param>
        public static DataType GetCachedOrNonRegisteredType(string name)
        {
            DataType first = knownTypes
                .FirstOrDefault(t => t.Name == name);

            return first == null ? new DataType(name) : first;
        }

        /// <summary>
        /// Gets all Datatypes known.
        /// </summary>
        /// <returns>all known DataTypes.</returns>
        public static DataType[] getDataTypes()
        {
            return knownTypes.ToArray();
        }

        /// <summary>
        /// Reloads the data from the base-Folder.
        /// </summary>
        public static void ReloadFromBaseFolder()
        {
            //Clear the olds and readd the basics.
            knownTypes.Clear();
            ReaddBasics();

            //Read the Base-Folder:
            string BaseFolder = Path.Combine(WorkSpace.DIR, WorkSpace.DATA_TYPES_DIR);
            if (!Directory.Exists(BaseFolder)) return;

            //Save the Stream, since we need to filter it twice:
            IEnumerable<JObject> stream = Directory.GetFiles(BaseFolder)
                .Select(File.ReadAllText)
                .Select(JObject.Parse);

            //Read normal:
            stream
                .Where(o => o.GetValueAsString("type") == "class")
                .ForEachTryIgnore( root => 
                    {
                        string name = root.GetValueAsString("name");
                        string template = root.GetValueAsString("template");
                        string[] types = root.GetValueAsStringArray("types");
                        bool creatable = root.GetValueAsBool("creatable", true);
                        knownTypes.Add(new DataType(name, template, types, creatable));
                    }
                );


            //Read Enum:
            stream
                .Where(o => o.GetValueAsString("type") == "enum")
                .ForEachTryIgnore(root =>
                    {
                        string name = root.GetValueAsString("name");
                        string[] values = root.GetValueAsStringArray("values", new String[0]);
                        knownTypes.Add(new DataType(name, values));
                    }
                );
        }

        /// <summary>
        /// Readds the basic components to the System.
        /// </summary>
        private static void ReaddBasics()
        {
            //Define the basic types:
            knownTypes.Add(new DataType("double", "{0}d", "double"));
            knownTypes.Add(new DataType("int", "{0}", "integer"));
            knownTypes.Add(new DataType("boolean", "{0}", "boolean"));
            knownTypes.Add(new DataType("string", "\"{0}\"", "string"));
        }


        #endregion StaticMethods
    }
}