using Common.IO;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.IO;
using System.Json;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SmaSTraDesigner.BusinessLogic
{
	// TODO: (PS) Comment this.
	public class ClassManager : INotifyPropertyChanged
	{
		private const string METADATA_FILENAME = "metadata.json";
		private const string NODE_TYPE_TRANSFORMATION = "transformation";
		private const string NODE_TYPE_SENSOR = "sensor";
		private const string JSON_PROP_TYPE = "type";
		private const string JSON_PROP_DISPLAY = "display";
		private const string JSON_PROP_DESCRIPTION = "description";
		private const string JSON_PROP_OUTPUT = "output";
		private const string JSON_PROP_INPUT = "input";

		private enum NodeType
		{
			Transformation,
			Sensor
		}

		private Dictionary<string, NodeClass> classes = new Dictionary<string, NodeClass>();
		private Dictionary<string, DataType> dataTypes = new Dictionary<string, DataType>();

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

			result = new NodeClass(name, baseNode, this.AddDataType(outputType), actualInputTypes)
			{
				DisplayName = displayName,
				Description = description
			};
			this.classes.Add(name, result);

			switch (nodeType)
			{
				case NodeType.Transformation:
					this.OnPropertyChanged("TransformationClasses");
					break;

				case NodeType.Sensor:
					this.OnPropertyChanged("DataSourceClasses");
					break;
			}

			return result;
		}

		/// <summary>
		/// TODO: (PS) Comment this.
		/// </summary>
		private NodeClass[] dataSourceClasses = null;

		/// <summary>
		/// Gets the DataSourceClasses instance (creates one if none exists).
		/// TODO: (PS) Comment this.
		/// </summary>
		public NodeClass[] DataSourceClasses
		{
			get
			{
				if (this.dataSourceClasses == null)
				{
					this.dataSourceClasses = this.classes.Values.Where(cls => cls.BaseNode is DataSource).ToArray();
				}

				return this.dataSourceClasses;
			}
		}

		/// <summary>
		/// TODO: (PS) Comment this.
		/// </summary>
		private NodeClass[] transformationNodeClasses = null;

		/// <summary>
		/// Gets the TransformationClasses instance (creates one if none exists).
		/// TODO: (PS) Comment this.
		/// </summary>
		public NodeClass[] TransformationClasses
		{
			get
			{
				if (this.transformationNodeClasses == null)
				{
					this.transformationNodeClasses = this.classes.Values.Where(cls => cls.BaseNode is Transformation && cls.InputTypes.Length > 1).ToArray();
				}

				return this.transformationNodeClasses;
			}
		}

		/// <summary>
		/// TODO: (PS) Comment this.
		/// </summary>
		private NodeClass[] conversionClasses = null;

		/// <summary>
		/// Gets the ConversionClasses instance (creates one if none exists).
		/// TODO: (PS) Comment this.
		/// </summary>
		public NodeClass[] ConversionClasses
		{
			get
			{
				if (this.conversionClasses == null)
				{
					this.conversionClasses = this.classes.Values.Where(cls => cls.BaseNode is Transformation && cls.InputTypes.Length == 1).ToArray();
				}

				return this.conversionClasses;
			}
		}

		public event PropertyChangedEventHandler PropertyChanged;

		private void OnPropertyChanged(string propertyName)
		{
			if (this.PropertyChanged != null)
			{
				this.PropertyChanged(this, new PropertyChangedEventArgs(propertyName));
			}
		}
	}
}
