namespace SmaSTraDesigner.BusinessLogic
{
	using System;
	using System.Collections.Generic;
	using System.ComponentModel;
	using System.IO;
	using System.Json;
	using System.Linq;
	using System.Text;
	using System.Threading.Tasks;

	using Common.IO;

	// TODO: (PS) Comment this.
	public class ClassManager : INotifyPropertyChanged
	{
		#region constants

		private const string JSON_PROP_DESCRIPTION = "description";
		private const string JSON_PROP_DISPLAY = "display";
		private const string JSON_PROP_INPUT = "input";
		private const string JSON_PROP_OUTPUT = "output";
		private const string JSON_PROP_TYPE = "type";
		private const string METADATA_FILENAME = "metadata.json";
		private const string NODE_TYPE_SENSOR = "sensor";
		private const string NODE_TYPE_TRANSFORMATION = "transformation";

		#endregion constants

		#region static methods

		private static NodeType GetNodeType(string type)
		{
			switch (type)
			{
				case NODE_TYPE_TRANSFORMATION:
					return NodeType.Transformation;

				case NODE_TYPE_SENSOR:
					return NodeType.Sensor;

				default:
					throw new Exception(String.Format("Unrecognized node type \"{0}\".", type));
			}
		}

		#endregion static methods

		#region fields

		/// <summary>
		/// TODO: (PS) Comment this.
		/// </summary>
		private Transformation[] baseConversions = null;

		/// <summary>
		/// TODO: (PS) Comment this.
		/// </summary>
		private DataSource[] baseDataSources = null;

		/// <summary>
		/// TODO: (PS) Comment this.
		/// </summary>
		private Transformation[] baseTransformations = null;
		private Dictionary<string, NodeClass> classes = new Dictionary<string, NodeClass>();
		private Dictionary<string, DataType> dataTypes = new Dictionary<string, DataType>();

		#endregion fields

		#region events

		public event PropertyChangedEventHandler PropertyChanged;

		#endregion events

		#region properties

		/// <summary>
		/// Gets the BaseConversions instance (creates one if none exists).
		/// TODO: (PS) Comment this.
		/// </summary>
		public Transformation[] BaseConversions
		{
			get
			{
				if (this.baseConversions == null)
				{
					this.baseConversions = this.classes.Values.Where(cls => cls.BaseNode is Transformation && cls.InputTypes.Length == 1)
						.Select(cls => (Transformation)cls.BaseNode).ToArray();
				}

				return this.baseConversions;
			}
		}

		/// <summary>
		/// Gets the BaseDataSources instance (creates one if none exists).
		/// TODO: (PS) Comment this.
		/// </summary>
		public DataSource[] BaseDataSources
		{
			get
			{
				if (this.baseDataSources == null)
				{
					this.baseDataSources = this.classes.Values.Where(cls => cls.BaseNode is DataSource)
						.Select(cls => (DataSource)cls.BaseNode).ToArray();
				}

				return this.baseDataSources;
			}
		}

		/// <summary>
		/// Gets the BaseTransformations instance (creates one if none exists).
		/// TODO: (PS) Comment this.
		/// </summary>
		public Transformation[] BaseTransformations
		{
			get
			{
				if (this.baseTransformations == null)
				{
					this.baseTransformations = this.classes.Values.Where(cls => cls.BaseNode is Transformation && cls.InputTypes.Length > 1)
						.Select(cls => (Transformation)cls.BaseNode).ToArray();
				}

				return this.baseTransformations;
			}
		}

		#endregion properties

		#region methods

		public NodeClass AddClass(string name, string type, string outputType, string[] inputTypes, string displayName = null, string description = null)
		{
			NodeClass result;
			if (this.classes.TryGetValue(name, out result))
			{
				return result;
			}

			Node baseNode;
			DataType[] actualInputTypes = null;
			NodeType nodeType = GetNodeType(type);
			switch (nodeType)
			{
				default:
				case NodeType.Transformation:
					baseNode = new Transformation();
					actualInputTypes = inputTypes.Select(this.AddDataType).ToArray();
					break;

				case NodeType.Sensor:
					baseNode = new DataSource();
					break;
			}

			if (String.IsNullOrWhiteSpace(displayName))
			{
				displayName = name;
			}

			baseNode.Name = displayName;
			result = new NodeClass(name, baseNode, this.AddDataType(outputType), actualInputTypes)
			{
				DisplayName = displayName,
				Description = description
			};
			this.classes.Add(name, result);

			switch (nodeType)
			{
				case NodeType.Transformation:
					if (result.InputTypes.Length == 1)
					{
						this.baseConversions = null;
						this.OnPropertyChanged("BaseConversions");
					}
					else
					{
						this.baseTransformations = null;
						this.OnPropertyChanged("TransformationClasses");
					}
					break;

				case NodeType.Sensor:
					this.baseDataSources = null;
					this.OnPropertyChanged("BaseDataSources");
					break;
			}

			return result;
		}

		public DataType AddDataType(string dataTypeName)
		{
			if (String.IsNullOrWhiteSpace(dataTypeName))
			{
				throw new ArgumentException("String argument 'dataTypeName' must not be null or empty (incl. whitespace).", "dataTypeName");
			}

			if (!this.dataTypes.ContainsKey(dataTypeName))
			{
				return this.dataTypes[dataTypeName] = new DataType(dataTypeName);
			}

			return this.dataTypes[dataTypeName];
		}

		public void LoadClasses(string path)
		{
			if (!Directory.Exists(path))
			{
				throw new ArgumentException(String.Format("Directory \"{0}\" does not exist", path), "path");
			}

			string[] dirs = Directory.GetDirectories(path);
			foreach (string dir in dirs)
			{
				string dirName = Path.GetFileName(dir);
				try
				{
					JsonObject jso;
					using (var stream = File.OpenRead(Path.Combine(dir, METADATA_FILENAME)))
					{
						jso = (JsonObject)JsonObject.Load(stream);
					}

					string type = jso[JSON_PROP_TYPE].ReadAs<string>();
					string[] inputTypes = GetNodeType(type) == NodeType.Transformation ?
						jso[JSON_PROP_INPUT].Select(kvp => kvp.Value.ReadAs<string>()).ToArray() :
						null;

					JsonValue value;
					string displayName = null;
					if (jso.TryGetValue(JSON_PROP_DISPLAY, out value))
					{
						displayName = value.ReadAs<string>();
					}

					string description = null;
					if (jso.TryGetValue(JSON_PROP_DESCRIPTION, out value))
					{
						description = value.ReadAs<string>();
					}

					this.AddClass(dirName, type, jso["output"].ReadAs<string>(), inputTypes, displayName, description);
				}
				catch (Exception ex)
				{
					throw new Exception(String.Format("Unable to read metadata for node class \"{0}\" in \"{1}\".", dirName, path), ex);
				}
			}
		}

		private void OnPropertyChanged(string propertyName)
		{
			if (this.PropertyChanged != null)
			{
				this.PropertyChanged(this, new PropertyChangedEventArgs(propertyName));
			}
		}

		#endregion methods

		#region enumerations

		private enum NodeType
		{
			Transformation,
			Sensor
		}

		#endregion enumerations
	}
}