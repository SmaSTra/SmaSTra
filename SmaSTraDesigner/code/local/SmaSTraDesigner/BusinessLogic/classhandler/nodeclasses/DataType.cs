using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using Newtonsoft.Json.Linq;
using SmaSTraDesigner.BusinessLogic.config;
using SmaSTraDesigner.BusinessLogic.utils;

namespace SmaSTraDesigner.BusinessLogic.classhandler.nodeclasses
{
    /// <summary>
    /// Stores information about a data type used by nodes for their inpot or output data.
    /// </summary>
    public class DataType
    {

        /// <summary>
        /// The List of DataTypes present in the system.
        /// </summary>
        private static readonly List<DataType> KnownTypes = new List<DataType>();


        #region constructors

        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="name">Data type name (is used as a unique identifier)</param>
        private DataType(string name) : this(name, "{0}", new[] { "Object" } )
		{}

        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="name">Data type name (is used as a unique identifier)</param>
        /// <param name="template">The Template to apply while code generation</param>
        /// <param name="type">The Type to apply in the template</param>
        private DataType(string name, string template, string type) 
            : this(name, template, new[] { type } )
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
            this.FixedValues = values ?? new string[0];
        }

        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="name">Data type name (is used as a unique identifier)</param>
        /// <param name="template">The Template to apply while code generation</param>
        /// <param name="types">The Types to apply in the template</param>
        /// <param name="creatable">If the element is creatable</param>
        private DataType(string name, string template, string[] types = null, bool creatable = true)
        {
            System.Diagnostics.Debug.Print("DataType created: " + name);
            if (String.IsNullOrWhiteSpace(name))
            {
                throw new ArgumentException("String argument 'name' must not be null or empty (incl. whitespace).", nameof(name));
            }

            this.Name = name;
            this.MinimizedName = MinimizeToClass(name);

            this.TypeTemplate = template;
            this.TypeTemplateVars = types ?? new[] { "object" };
            this.Creatable = creatable;
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
        public string[] FixedValues { get; }


        #endregion properties

        #region overrideable methods

        public override bool Equals(object obj)
		{
			var other = obj as DataType;
			return other != null && string.Equals(this.Name, other.Name, StringComparison.InvariantCultureIgnoreCase);
		}

		public override int GetHashCode()
		{
			return this.Name.GetHashCode();
		}

		public override string ToString()
		{
			return $"{this.GetType().Name} {this.Name}";
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
            return new DataType(Name, TypeTemplate, TypeTemplateVars, Creatable);
        }

        /// <summary>
        /// This gets the last part of a class from an complete Class name.
        /// Eg.: java.util.List -> List.
        /// </summary>
        /// <param name="completeClassName"> This is the complete class name. For example 'java.util.List'</param>
        /// <returns>The last part of the Class name.</returns>
        public static string MinimizeToClass(string completeClassName)
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
            var first = KnownTypes
                .FirstOrDefault(t => t.Name == name);

            if (first != null) return first;

            first = new DataType(name);
            KnownTypes.Add(first);

            return first;
        }

        /// <summary>
        /// Gets Temp non registered types.
        /// Does NOT register a new one if not exists.
        /// </summary>
        /// <param name="name">To get</param>
        public static DataType GetCachedOrNonRegisteredType(string name)
        {
            var first = KnownTypes
                .FirstOrDefault(t => t.Name == name);

            return first ?? new DataType(name);
        }

        /// <summary>
        /// Gets all Datatypes known.
        /// </summary>
        /// <returns>all known DataTypes.</returns>
        public static DataType[] GetDataTypes()
        {
            return KnownTypes.ToArray();
        }

        /// <summary>
        /// Reloads the data from the base-Folder.
        /// </summary>
        public static void ReloadFromBaseFolder()
        {
            //Clear the olds and readd the basics.
            KnownTypes.Clear();
            ReaddBasics();

            //Read the Base-Folder:
            var baseFolder = Path.Combine(WorkSpace.DIR, WorkSpace.DATA_TYPES_DIR);
            if (!Directory.Exists(baseFolder)) return;

            //Save the Stream, since we need to filter it twice:
            var stream = Directory.GetFiles(baseFolder)
                .Select(File.ReadAllText)
                .Select(JObject.Parse);

            //Read normal:
            stream
                .Where(o => o.GetValueAsString("type") == "class")
                .ForEachTryIgnore( root => 
                    {
                        var name = root.GetValueAsString("name");
                        var template = root.GetValueAsString("template");
                        var types = root.GetValueAsStringArray("types");
                        var creatable = root.GetValueAsBool("creatable", true);
                        KnownTypes.Add(new DataType(name, template, types, creatable));
                    }
                );


            //Read Enum:
            stream
                .Where(o => o.GetValueAsString("type") == "enum")
                .ForEachTryIgnore(root =>
                    {
                        var name = root.GetValueAsString("name");
                        var values = root.GetValueAsStringArray("values", new string[0]);
                        KnownTypes.Add(new DataType(name, values));
                    }
                );
        }

        /// <summary>
        /// Readds the basic components to the System.
        /// </summary>
        private static void ReaddBasics()
        {
            //Define the basic types:
            KnownTypes.Add(new DataType("double", "{0}d", "double"));
            KnownTypes.Add(new DataType("int", "{0}", "integer"));
            KnownTypes.Add(new DataType("boolean", "{0}", "boolean"));
            KnownTypes.Add(new DataType("string", "\"{0}\"", "string"));
        }


        #endregion StaticMethods
    }
}