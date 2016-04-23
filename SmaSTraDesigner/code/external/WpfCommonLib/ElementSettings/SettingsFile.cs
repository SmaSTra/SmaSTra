namespace Common.ElementSettings
{
	using System;
	using System.Collections.Generic;
	using System.Diagnostics;
	using System.IO;
	using System.Linq;
	using System.Threading;
	using System.Windows;
	using System.Xml;
	using System.Xml.Serialization;

	using Common.ElementSettings.Serialization;
	using Common.ExtensionMethods;

	// TODO: (PS) Comment this.
	[Singleton(UseWeakInstanceReference = true)]
	public class SettingsFile
	{
		#region static constructor

		static SettingsFile()
		{
			Singleton<SettingsFile>.CreateInstanceFunction = () =>
				new SettingsFile(Path.Combine(Environment.CurrentDirectory, "settings.xml"));
		}

		#endregion static constructor

		#region static properties

		/// <summary>
		/// Gets the default instance (creates one if none exists).
		/// TODO: (PS) Comment this.
		/// </summary>
		public static SettingsFile Default
		{
			get
			{
				return Singleton<SettingsFile>.Instance;
			}
		}

		#endregion static properties

		#region fields

		private Dictionary<string, Dictionary<string, object>> data = new Dictionary<string, Dictionary<string, object>>();
		private object monitor = new object();
		private HashSet<Type> valueTypes = new HashSet<Type>();
		private Timer writeTimer = null;

		#endregion fields

		#region constructors

		public SettingsFile(string filename)
			: this()
		{
			if (String.IsNullOrWhiteSpace(filename))
			{
				throw new ArgumentException("String argument 'filename' must not be null or empty (incl. whitespace).", "filename");
			}

			this.Filename = filename;
			if (File.Exists(filename))
			{
				this.Read();
			}
		}

		private SettingsFile()
		{
			this.WriteTimeBuffer = 500;
		}

		#endregion constructors

		#region properties

		public string Filename
		{
			get;
			private set;
		}

		public int WriteTimeBuffer
		{
			get;
			set;
		}

		#endregion properties

		#region methods

		public void AddValueType(Type type)
		{
			if (!(type.IsPrimitive || type.IsEnum || type == typeof(string)))
			{
				this.valueTypes.Add(type);
			}
		}

		public void Read()
		{
			try
			{
				lock (this.monitor)
				{
					bool reRead;
					do
					{
						using (Stream stream = File.OpenRead(this.Filename))
						{
							RootEntry root = this.GetSerializer().Deserialize(stream) as RootEntry;
							if (root == null)
							{
								throw new FileFormatException("Settings file could not be read.");
							}

							IEnumerable<Type> types = null;
							reRead = root.ValueTypeNames != null;
							if (reRead)
							{
								types = root.ValueTypeNames.Select(typeName => Type.GetType(typeName));
								reRead = types.Any(type => !this.valueTypes.Contains(type));
							}

							if (reRead)
							{
								this.valueTypes.AddRange(types);
							}
							else
							{
								this.data = root.Collections.ToDictionary(sc => sc.Name, sc => sc.Settings.ToDictionary(s => s.Name, s => s.Value));
							}
						}
					}
					while (reRead);
				}
			}
			catch (Exception ex)
			{
				Debug.WriteLine("Reading XML-File \"{0}\" failed:\n{1}", this.Filename, ex);
			}
		}

		public void Write(bool writeNow = false)
		{
			lock (this.monitor)
			{
				if (writeNow || this.WriteTimeBuffer <= 0)
				{
					this.WriteNow();
				}
				else if (this.writeTimer == null)
				{
					this.writeTimer = new Timer(new TimerCallback((state) =>
					{
						lock (this.monitor)
						{
							this.WriteNow();

							if (this.writeTimer != null)
							{
								this.writeTimer.Dispose();
								this.writeTimer = null;
							}
						}
					}), null, this.WriteTimeBuffer, Timeout.Infinite);
				}
			}
		}

		internal object GetValue(string collectionName, string settingName)
		{
			if (String.IsNullOrWhiteSpace(collectionName))
			{
				throw new ArgumentException("String argument 'collectionName' must not be null or empty (incl. whitespace).", "collectionName");
			}
			if (String.IsNullOrWhiteSpace(settingName))
			{
				throw new ArgumentException("String argument 'settingName' must not be null or empty (incl. whitespace).", "settingName");
			}

			Dictionary<string, object> collection;
			if (this.data.ContainsKey(collectionName))
			{
				collection = this.data[collectionName];
				if (collection.ContainsKey(settingName))
				{
					return collection[settingName];
				}
			}

			return DependencyProperty.UnsetValue;
		}

		internal void SetValue(string collectionName, string settingName, object value)
		{
			if (String.IsNullOrWhiteSpace(collectionName))
			{
				throw new ArgumentException("String argument 'collectionName' must not be null or empty (incl. whitespace).", "collectionName");
			}
			if (String.IsNullOrWhiteSpace(settingName))
			{
				throw new ArgumentException("String argument 'settingName' must not be null or empty (incl. whitespace).", "settingName");
			}

			Dictionary<string, object> collection;
			if (this.data.ContainsKey(collectionName))
			{
				collection = this.data[collectionName];
				if (collection.ContainsKey(settingName))
				{
					collection[settingName] = value;
				}
				else
				{
					collection.Add(settingName, value);
				}
			}
			else
			{
				collection = new Dictionary<string, object>();
				collection.Add(settingName, value);
				this.data.Add(collectionName, collection);
			}

			if (value != null)
			{
				this.AddValueType(value.GetType());
			}
		}

		private XmlSerializer GetSerializer()
		{
			Type[] types = new Type[] { typeof(List<SettingCollectionEntry>), typeof(List<SettingEntry>) }.Concat(this.valueTypes).ToArray();

			return new XmlSerializer(typeof(RootEntry), types);
		}

		private void WriteNow()
		{
			try
			{
				using (Stream stream = File.Open(this.Filename, FileMode.Create))
				using (XmlWriter writer = XmlWriter.Create(stream, new XmlWriterSettings()
				{
					Indent = true,
					IndentChars = "\t"
				}))
				{
					RootEntry root = new RootEntry()
					{
						Collections = this.data.Select(sc => new SettingCollectionEntry()
						{
							Name = sc.Key,
							Settings = sc.Value.Select(s => new SettingEntry()
							{
								Name = s.Key,
								Value = s.Value
							}).ToList()
						}).ToList(),
						ValueTypeNames = this.valueTypes.Select(type => type.AssemblyQualifiedName).ToArray()
					};
					this.GetSerializer().Serialize(writer, root);
				}
			}
			catch (Exception ex)
			{
				Debug.WriteLine("Writing XML-File \"{0}\" failed:\n{1}", this.Filename, ex);
			}
		}

		#endregion methods
	}
}