namespace Common
{
	using System;
	using System.Collections;
	using System.Collections.Generic;
	using System.Linq;
	using System.Windows;
	using System.Windows.Data;

	using Common.ExtensionMethods;

	// TODO: (PS) Comment this.
	public class DPHelper : IDisposable
	{
		#region static methods

		public static PropertyChangedCallback CombineCallbacks(params PropertyChangedCallback[] callbacks)
		{
			return (PropertyChangedCallback)Delegate.Combine(callbacks);
		}

		public static CoerceValueCallback CombineCallbacks(params CoerceValueCallback[] callbacks)
		{
			return (sender, value) =>
			{
				foreach (var callback in callbacks)
				{
					value = callback(sender, value);
				}

				return value;
			};
		}

		private static void ValidateArguments(object callback, Type type, DependencyProperty[] properties)
		{
			if (callback == null)
			{
				throw new ArgumentNullException("callback");
			}
			ValidateArguments(type, properties);
		}

		private static void ValidateArguments(Type type, DependencyProperty[] properties)
		{
			if (type == null)
			{
				throw new ArgumentNullException("type");
			}
			if (properties == null)
			{
				throw new ArgumentNullException("properties");
			}
			if (((ICollection)properties).Count == 0)
			{
				throw new ArgumentException("Argument list 'properties' must not be empty.", "properties");
			}
		}

		#endregion static methods

		#region fields

		private Dictionary<Tuple<DependencyProperty, Type>, PropertyMetadata> properties = new Dictionary<Tuple<DependencyProperty, Type>, PropertyMetadata>();

		#endregion fields

		#region constructors

		~DPHelper()
		{
			this.Dispose();
		}

		#endregion constructors

		#region properties

		/// <summary>
		/// Gets or sets a value indicating whether this <see cref="DPHelper"/> is disposed.
		/// </summary>
		/// <value><c>true</c> if disposed; otherwise, <c>false</c>.</value>
		public bool IsDisposed
		{
			get;
			protected set;
		}

		#endregion properties

		#region methods

		public void AddChangeCallback(PropertyChangedCallback callback, Type type, params DependencyProperty[] properties)
		{
			ValidateArguments(callback, type, properties);

			foreach (DependencyProperty property in properties)
			{
				PropertyMetadata metadata = this.GetMetadataClone(property, type);

				if (metadata.PropertyChangedCallback == null)
				{
					metadata.PropertyChangedCallback = callback;
				}
				else
				{
					metadata.PropertyChangedCallback = (PropertyChangedCallback)PropertyChangedCallback.Combine(metadata.PropertyChangedCallback, callback);
				}
			}
		}

		public void AddCoerceCallback(CoerceValueCallback callback, Type type, params DependencyProperty[] properties)
		{
			ValidateArguments(callback, type, properties);

			foreach (DependencyProperty property in properties)
			{
				PropertyMetadata metadata = this.GetMetadataClone(property, type);

				if (metadata.CoerceValueCallback == null)
				{
					metadata.CoerceValueCallback = callback;
				}
				else
				{
					metadata.CoerceValueCallback = (sender, value) => callback(sender, metadata.CoerceValueCallback(sender, value));
				}
			}
		}

		/// <summary>
		/// Performs application-defined tasks associated with freeing, releasing, or resetting unmanaged resources.
		/// </summary>
		public void Dispose()
		{
			if (!this.IsDisposed)
			{
				this.IsDisposed = true;

				foreach (var kvp in this.properties)
				{
					kvp.Key.Item1.OverrideMetadata(kvp.Key.Item2, kvp.Value);
				}
			}
		}

		public void MergeMetadata(PropertyMetadataMerge merge, Type type, params DependencyProperty[] properties)
		{
			if (merge == null)
			{
				throw new ArgumentNullException("mergeData");
			}
			ValidateArguments(type, properties);

			var mergeProperties = typeof(PropertyMetadataMerge).GetProperties().OrderBy(p => p.Name).ToArray();
			var metadataProperties = typeof(FrameworkPropertyMetadata).GetProperties().Where(p => p.CanRead && p.CanWrite).OrderBy(p => p.Name).ToList();

			foreach (DependencyProperty property in properties)
			{
				PropertyMetadata metadata = this.GetMetadataClone(property, type);

				for (int i = 0; i < mergeProperties.Length; i++)
				{
					if (metadataProperties[i].DeclaringType.IsAssignableFrom(metadata.GetType()))
					{
						object value = mergeProperties[i].GetValue(merge, null);
						if (value != null)
						{
							Wrap wrap = value as Wrap;
							if (wrap != null)
							{
								value = wrap.Value;
							}

							metadataProperties[i].SetValue(metadata, value, null);
						}
					}
					else
					{
						FrameworkPropertyMetadata newMetadata = new FrameworkPropertyMetadata();
						foreach (var p in metadataProperties)
						{
							if (p.DeclaringType.IsAssignableFrom(metadata.GetType()))
							{
								p.SetValue(newMetadata, p.GetValue(metadata, null), null);
							}
						}

						metadata = newMetadata;
						this.properties[new Tuple<DependencyProperty, Type>(property, type)] = metadata;
						i = -1;
					}
				}
			}
		}

		public void SetDefaultValue(object defaultValue, Type type, params DependencyProperty[] properties)
		{
			ValidateArguments(type, properties);

			foreach (DependencyProperty property in properties)
			{
				PropertyMetadata metadata = this.GetMetadataClone(property, type);

				metadata.DefaultValue = defaultValue;
			}
		}

		private PropertyMetadata GetMetadataClone(DependencyProperty property, Type type)
		{
			PropertyMetadata metadata = property.GetMetadata(type);
			Tuple<DependencyProperty, Type> key = new Tuple<DependencyProperty, Type>(property, type);
			if (metadata != null && this.properties.ContainsKey(key))
			{
				metadata = this.properties[key];
			}
			else
			{
				if (metadata == null)
				{
					metadata = new PropertyMetadata();
				}
				else
				{
					metadata = metadata.Clone();
				}

				this.properties.Add(key, metadata);
			}

			return metadata;
		}

		#endregion methods
	}

	public class PropertyMetadataMerge
	{
		#region properties

		public bool? AffectsArrange
		{
			get;
			set;
		}

		public bool? AffectsMeasure
		{
			get;
			set;
		}

		public bool? AffectsParentArrange
		{
			get;
			set;
		}

		public bool? AffectsParentMeasure
		{
			get;
			set;
		}

		public bool? AffectsRender
		{
			get;
			set;
		}

		public bool? BindsTwoWayByDefault
		{
			get;
			set;
		}

		public CoerceValueCallback CoerceValueCallback
		{
			get;
			set;
		}

		public UpdateSourceTrigger? DefaultUpdateSourceTrigger
		{
			get;
			set;
		}

		public Wrap DefaultValue
		{
			get;
			set;
		}

		public bool? Inherits
		{
			get;
			set;
		}

		public bool? IsAnimationProhibited
		{
			get;
			set;
		}

		public bool? IsNotDataBindable
		{
			get;
			set;
		}

		public bool? Journal
		{
			get;
			set;
		}

		public bool? OverridesInheritanceBehavior
		{
			get;
			set;
		}

		public PropertyChangedCallback PropertyChangedCallback
		{
			get;
			set;
		}

		public bool? SubPropertiesDoNotAffectRender
		{
			get;
			set;
		}

		#endregion properties
	}
}