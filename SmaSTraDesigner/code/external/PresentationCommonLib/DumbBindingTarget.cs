namespace Common
{
	using System;
	using System.ComponentModel;
	using System.Reflection;

	// TODO: (PS) Comment this.
	public abstract class DumbBindingTarget : IDisposable
	{
		#region static methods

		private static bool IndexArraysEquals(object[] index1, object[] index2)
		{
			if (index1.Length != index2.Length)
			{
				return false;
			}

			var enumerator1 = index1.GetEnumerator();
			var enumerator2 = index2.GetEnumerator();
			while (enumerator1.MoveNext() && enumerator2.MoveNext())
			{
				var item1 = enumerator1.Current;
				var item2 = enumerator2.Current;

				if (!object.Equals(item1, item2))
				{
					return false;
				}
			}

			return true;
		}

		#endregion static methods

		#region fields

		private BindingFlags bindingFlags;

		// TODO: (PS) Comment this.
		private Action<object> propertyChangedCallback = null;
		private bool reads;

		#endregion fields

		#region constructors

		protected DumbBindingTarget(object targetObject, Type targetType, string targetPropertyName, bool read, bool write, BindingFlags bindingFlags, object[] targetPropertyIndex)
		{
			if (targetObject == null && targetType == null)
			{
				throw new ArgumentNullException("targetObject");
			}
			else if (targetObject != null)
			{
				targetType = targetObject.GetType();
			}
			else if (targetType == null)
			{
				throw new ArgumentNullException("targetType");
			}
			if (String.IsNullOrWhiteSpace(targetPropertyName))
			{
				throw new ArgumentException("String argument 'targetPropertyName' must not be null or empty (incl. whitespace).", "targetPropertyName");
			}
			if (targetPropertyIndex != null && targetPropertyIndex.Length == 0)
			{
				throw new ArgumentException("Argument array 'targetPropertyIndex' must not be empty.", "targetPropertyIndex");
			}

			this.reads = read;
			this.TargetObject = targetObject;
			this.TargetType = targetType;
			this.TargetProperty = targetType.GetProperty(targetPropertyName, BindingFlags.Public | bindingFlags);
			this.TargetPropertyIndex = targetPropertyIndex;
			this.bindingFlags = bindingFlags;

			if (this.TargetProperty == null)
			{
				throw new ArgumentException(String.Format("Target property \"{0}\" could not be found in type {1}.", targetPropertyName, targetType), "targetPropertyName");
			}
			if (read && !this.TargetProperty.CanRead)
			{
				throw new ArgumentException(String.Format("Target property \"{0}\" of type {1} cannot read.", targetPropertyName, targetType), "targetPropertyName");
			}
			if (write && !this.TargetProperty.CanWrite)
			{
				throw new ArgumentException(String.Format("Target property \"{0}\" of type {1} cannot write.", targetPropertyName, targetType), "targetPropertyName");
			}
		}

		#endregion constructors

		#region overrideable properties

		// TODO: (PS) Comment this.
		public abstract object PropertyValue
		{
			get;
			set;
		}

		// TODO: (PS) Comment this.
		public abstract object TargetObject
		{
			get;
			protected set;
		}

		#endregion overrideable properties

		#region properties

		// TODO: (PS) Comment this.
		public Action<object> PropertyChangedCallback
		{
			get
			{
				return this.propertyChangedCallback;
			}
			set
			{
				if (this.propertyChangedCallback == null || value == null)
				{
					this.propertyChangedCallback = value;

					if (value != null)
					{
						EventInfo propertyChangedEvent = this.TargetType.GetEvent("PropertyChanged", BindingFlags.Public | this.bindingFlags);
						if (propertyChangedEvent != null && propertyChangedEvent.EventHandlerType == typeof(PropertyChangedEventHandler))
						{
							this.AddPropertyChangedEventHandler(propertyChangedEvent);
						}
					}
				}
			}
		}

		// TODO: (PS) Comment this.
		public PropertyInfo TargetProperty
		{
			get;
			protected set;
		}

		// TODO: (PS) Comment this.
		public object[] TargetPropertyIndex
		{
			get;
			protected set;
		}

		// TODO: (PS) Comment this.
		public Type TargetType
		{
			get;
			private set;
		}

		#endregion properties

		#region overrideable methods

		public abstract void Dispose();

		protected abstract void AddPropertyChangedEventHandler(EventInfo propertyChangedEvent);

		public override bool Equals(object obj)
		{
			StaticDumbBindingTarget other = obj as StaticDumbBindingTarget;

			return other != null &&
				object.Equals(this.TargetObject, other.TargetObject) &&
				object.Equals(this.TargetProperty, other.TargetProperty) &&
				((this.TargetPropertyIndex == null && other.TargetPropertyIndex == null) ||
				(!(this.TargetPropertyIndex == null || other.TargetPropertyIndex == null) &&
				IndexArraysEquals(this.TargetPropertyIndex, other.TargetPropertyIndex)));
		}

		public override int GetHashCode()
		{
			return HashCodeOperations.Combine(this.TargetObject, this.TargetType, this.TargetProperty, this.TargetPropertyIndex);
		}

		#endregion overrideable methods

		#region event handlers

		protected void Target_PropertyChanged(object sender, PropertyChangedEventArgs e)
		{
			if (e.PropertyName == this.TargetProperty.Name && this.PropertyChangedCallback != null)
			{
				this.PropertyChangedCallback(this.PropertyValue);
			}
		}

		#endregion event handlers
	}
}