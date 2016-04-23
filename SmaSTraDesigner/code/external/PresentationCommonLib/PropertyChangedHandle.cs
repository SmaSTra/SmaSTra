namespace Common
{
	using System;
	using System.Collections.Generic;
	using System.Windows;
	using System.Windows.Data;

	/// <summary>
	/// Class that holds the arguments for the PropertyChangedHandle callback delegate.
	/// </summary>
	public class PropertyChangedCallbackArgs
	{
		#region constructors

		/// <summary>
		/// Initializes a new instance of the <see cref="PropertyChangedCallbackArgs"/> class.
		/// </summary>
		/// <param name="handle">The PropertyChangedHandle instance that executes the callback.</param>
		/// <param name="oldValue">The old value of the property that is being watched.</param>
		/// <param name="newValue">The new value of the property that is being watched.</param>
		public PropertyChangedCallbackArgs(PropertyChangedHandle handle, object oldValue, object newValue)
		{
			this.Handle = handle;
			this.OldValue = oldValue;
			this.NewValue = newValue;
		}

		#endregion constructors

		#region properties

		/// <summary>
		/// Gets or sets the PropertyChangedHandle instance that executes the callback.
		/// </summary>
		public PropertyChangedHandle Handle
		{
			get;
			private set;
		}

		/// <summary>
		/// Gets or sets the new value of the property that is being watched.
		/// </summary>
		public object NewValue
		{
			get;
			private set;
		}

		/// <summary>
		/// Gets or sets the old value of the property that is being watched.
		/// </summary>
		public object OldValue
		{
			get;
			private set;
		}

		#endregion properties
	}

	/// <summary>
	/// Class that is used to call a callback method when a specific property has changed.
	/// </summary>
	public class PropertyChangedHandle : IDisposable
	{
		#region static fields

		// TODO: (PS) Comment this.
		private static HashSet<PropertyChangedHandle> keptAliveInstances = new HashSet<PropertyChangedHandle>();

		#endregion static fields

		#region static methods

		// TODO: (PS) Comment this.
		public static bool BindingEquals(Binding binding1, Binding binding2)
		{
			return (binding1 == null && binding2 == null) ||
				(binding1 != null && binding2 != null &&
				object.Equals(binding1.Converter, binding2.Converter) &&
				object.Equals(binding1.ConverterCulture, binding2.ConverterCulture) &&
				object.Equals(binding1.ConverterParameter, binding2.ConverterParameter) &&
				object.Equals(binding1.FallbackValue, binding2.FallbackValue) &&
				object.Equals(binding1.Mode, binding2.Mode) &&
				object.Equals(binding1.Path.Path, binding2.Path.Path) &&
				object.Equals(binding1.Source, binding2.Source) &&
				object.Equals(binding1.StringFormat, binding2.StringFormat) &&
				object.Equals(binding1.TargetNullValue, binding2.TargetNullValue) &&
				object.Equals(binding1.UpdateSourceExceptionFilter, binding2.UpdateSourceExceptionFilter) &&
				object.Equals(binding1.UpdateSourceTrigger, binding2.UpdateSourceTrigger));
		}

		// TODO: (PS) Comment this.
		public static bool DoesBindingRead(Binding binding)
		{
			if (binding == null)
			{
				throw new ArgumentNullException("binding");
			}

			return binding.Mode == BindingMode.TwoWay || binding.Mode == BindingMode.OneWay || binding.Mode == BindingMode.Default;
		}

		// TODO: (PS) Comment this.
		public static bool DoesBindingWrite(Binding binding)
		{
			if (binding == null)
			{
				throw new ArgumentNullException("binding");
			}

			return binding.Mode == BindingMode.TwoWay || binding.Mode == BindingMode.OneWayToSource;
		}

		// TODO: (PS) Comment this.
		public static bool Equals(object objA, object objB, bool includeCallback)
		{
			PropertyChangedHandle handleA = objA as PropertyChangedHandle;
			PropertyChangedHandle handleB = objB as PropertyChangedHandle;

			return (objA == null && objB == null) || (handleA != null && handleB != null && handleA.Equals(handleB, includeCallback));
		}

		/// <summary>
		/// Initializes a new instance of the <see cref="PropertyChangedHandle"/> class.
		/// Returned instance calls the specified callback method when the specified property changes its' value.
		/// ATTENTION: Call the Dispose() method when the returned instance is no longer needed.
		/// </summary>
		/// <param name="source">The source of the binding used to monitor property changes.</param>
		/// <param name="path">The binding path to the monitored property.</param>
		/// <param name="callback">The callback method that is supposed to be called whenever the specified property changes.</param>
		/// <param name="keepAlive">Specifies if this instance is supposed to be kept alive even if there are no refrerences to it.
		/// It can be exposed to the Garbage Collection again by calling the Dospose() method.</param>
		/// <exception cref="ArgumentNullException" />
		/// <exception cref="ArgumentException" />
		public static PropertyChangedHandle GetDistinctInstance(object source, string path,
			Action<PropertyChangedCallbackArgs> callback, bool keepAlive = false)
		{
			return GetDistinctInstance(new Binding(path) { Source = source }, callback, keepAlive);
		}

		// TODO: (PS) Comment this.
		public static PropertyChangedHandle GetDistinctInstance(Binding binding, Action<PropertyChangedCallbackArgs> callback, bool keepAlive = false)
		{
			bool instanceAllreadyExists;
			var result = DistinctInstanceProvider<PropertyChangedHandle>.Instance.GetDistinctInstance(
				new PropertyChangedHandle(binding, callback, keepAlive), out instanceAllreadyExists);

			if (keepAlive && !instanceAllreadyExists)
			{
				keptAliveInstances.Add(result);
			}

			return result;
		}

		#endregion static methods

		#region fields

		/// <summary>
		/// Instance of the BindingHandler class that uses a DependencyProperty binding
		/// to the specified property to monitor changes of that properties value
		/// and start this class's callback method.
		/// </summary>
		private BindingHandler bindingHandler = null;

		/// <summary>
		/// TODO: (PS) Comment this.
		/// </summary>
		private WeakReference source = null;

		#endregion fields

		#region constructors

		// TODO: (PS) Comment this.
		/// <summary>
		/// Initializes a new instance of the <see cref="PropertyChangedHandle"/> class.
		/// </summary>
		/// <param name="binding">The binding.</param>
		/// <param name="callback">The callback method that is supposed to be called whenever the specified property changes.</param>
		/// <param name="keepAlive">Specifies if this instance is supposed to be kept alive even if there are no refrerences to it.
		/// It can be exposed to the Garbage Collection again by calling the Dospose() method.</param>
		private PropertyChangedHandle(Binding binding, Action<PropertyChangedCallbackArgs> callback, bool keepAlive)
		{
			if (binding == null)
			{
				throw new ArgumentNullException("binding");
			}
			if (binding.Source == null)
			{
				throw new ArgumentException("Binding must have a Source set.", "binding");
			}
			if (binding.Mode == BindingMode.OneTime)
			{
				throw new ArgumentException("BindingMode 'OneTime' is not supported.", "binding");
			}
			if (!String.IsNullOrWhiteSpace(binding.BindingGroupName))
			{
				throw new ArgumentException("BindingGroup not supported.", "binding");
			}
			if (binding.IsAsync)
			{
				throw new ArgumentException("Async Binding not supported.", "binding");
			}
			if (binding.ValidationRules.Count != 0)
			{
				throw new ArgumentException("ValidationRules not supported.", "binding");
			}
			if (binding.BindsDirectlyToSource)
			{
				throw new ArgumentException("BindsDirectlyToSource not supported.", "binding");
			}
			if (binding.NotifyOnSourceUpdated || binding.NotifyOnTargetUpdated || binding.NotifyOnValidationError)
			{
				throw new ArgumentException("Notification not supported.", "binding");
			}
			if (String.IsNullOrWhiteSpace(binding.Path.Path))
			{
				throw new ArgumentException("Binding path must not be null or empty (incl. whitespace).", "binding");
			}
			if (callback == null)
			{
				throw new ArgumentNullException("callback");
			}

			this.Source = binding.Source;
			binding.Source = this;
			binding.Path = new PropertyPath("Source." + binding.Path.Path);
			this.Binding = binding;
			this.Callback = callback;
			this.KeepAlive = keepAlive;
			this.bindingHandler = new BindingHandler(this);
		}

		/// <summary>
		/// Releases unmanaged resources and performs other cleanup operations before the
		/// <see cref="PropertyChangedHandle"/> is reclaimed by garbage collection.
		/// </summary>
		~PropertyChangedHandle()
		{
			DistinctInstanceProvider<PropertyChangedHandle>.Instance.RemoveDistinctInstance(this);
		}

		#endregion constructors

		#region properties

		// TODO: (PS) Comment this.
		public Binding Binding
		{
			get;
			private set;
		}

		/// <summary>
		/// Gets the binding status.
		/// </summary>
		public BindingStatus BindingStatus
		{
			get
			{
				return BindingOperations.GetBindingExpression(this.bindingHandler, BindingHandler.PropertyProperty).Status;
			}
		}

		// TODO: (PS) (Optimization) Remove first argument of type object.
		/// <summary>
		/// Gets the callback method that is supposed to be called whenever the specified property changes.
		/// </summary>
		public Action<PropertyChangedCallbackArgs> Callback
		{
			get;
			private set;
		}

		// TODO: (PS) Comment this.
		public bool IsDisposed
		{
			get;
			private set;
		}

		// TODO: (PS) Comment this.
		public bool KeepAlive
		{
			get;
			private set;
		}

		/// <summary>
		/// Gets the binding path to the monitored property.
		/// </summary>
		public string Path
		{
			get { return this.Binding.Path.Path; }
		}

		/// <summary>
		/// Gets the current property value.
		/// </summary>
		public object PropertyValue
		{
			get
			{
				lock (this)
				{
					if (this.bindingHandler != null && DoesBindingRead(this.Binding))
					{
						return this.bindingHandler.Property;
					}
					else
					{
						return DependencyProperty.UnsetValue;
					}
				}
			}
			set
			{
				lock (this)
				{
					if (this.bindingHandler != null && DoesBindingWrite(this.Binding))
					{
						this.bindingHandler.ignorePropertyChange = true;
						this.bindingHandler.Property = value;
						this.bindingHandler.ignorePropertyChange = false;
					}
				}
			}
		}

		/// <summary>
		/// Gets or sets the Source weak reference.
		/// (Getter returns null if reference is no longer alive.)
		/// TODO: (PS) Comment this.
		/// </summary>
		public object Source
		{
			get
			{
				if (source == null || !source.IsAlive)
				{
					return null;
				}
				else
				{
					return source.Target;
				}
			}
			set
			{
				if (value != null)
				{
					source = new WeakReference(value);
				}
				else
				{
					source = null;
				}
			}
		}

		#endregion properties

		#region overrideable methods

		/// <summary>
		/// Determines whether the specified <see cref="System.Object"/> is equal to this instance.
		/// </summary>
		/// <param name="obj">The <see cref="System.Object"/> to compare with this instance.</param>
		/// <returns>
		/// 	<c>true</c> if the specified <see cref="System.Object"/> is equal to this instance; otherwise, <c>false</c>.
		/// </returns>
		public override bool Equals(object obj)
		{
			return this.Equals(obj, true);
		}

		/// <summary>
		/// Returns a hash code for this instance.
		/// </summary>
		/// <returns>
		/// A hash code for this instance, suitable for use in hashing algorithms and data structures like a hash table. 
		/// </returns>
		public override int GetHashCode()
		{
			return HashCodeOperations.Combine(this.Binding, this.Callback, this.KeepAlive);
		}

		#endregion overrideable methods

		#region methods

		/// <summary>
		/// Performs application-defined tasks associated with freeing, releasing, or resetting unmanaged resources.
		/// </summary>
		public void Dispose()
		{
			this.IsDisposed = true;
			keptAliveInstances.Remove(this);
			this.Callback = null;
		}

		// TODO: (PS) Comment this.
		public bool Equals(object obj, bool includeCallback)
		{
			PropertyChangedHandle other = (PropertyChangedHandle)obj;
			return other != null &&
				(object.ReferenceEquals(this, obj) ||
				(BindingEquals(other.Binding, this.Binding) &&
				(!includeCallback || object.Equals(other.Callback, this.Callback)) &&
				this.KeepAlive == other.KeepAlive));
		}

		#endregion methods

		#region nested types

		/// <summary>
		/// Handles the Binding that is used to monitor the property, the owner PropertyChangedHandle instance
		/// was created to supervise.
		/// The PropertyChangedHandle class itsself does not inherit the DependencyObject class because the Equals(...)
		/// method can no longer be overridden when doing so.
		/// </summary>
		private class BindingHandler : DependencyObject
		{
			#region static methods

			/// <summary>
			/// Property Changed Callback method of the Property Dependency Property.
			/// </summary>
			private static void OnPropertyChanged(object sender, DependencyPropertyChangedEventArgs e)
			{
				BindingHandler subject = (BindingHandler)sender;
				if (subject.initialized && !subject.ignorePropertyChange && !subject.owner.IsDisposed)
				{
					subject.owner.Callback(new PropertyChangedCallbackArgs(subject.owner, e.OldValue, e.NewValue));
				}
			}

			#endregion static methods

			#region dependency properties

			/// <summary>
			/// Registration of Property Dependency Property.
			/// </summary>
			public static readonly DependencyProperty PropertyProperty = 
				DependencyProperty.Register(
					"Property", typeof(object), typeof(BindingHandler),
					new PropertyMetadata(
						null,
						OnPropertyChanged));

			#endregion dependency properties

			#region fields

			// TODO: (PS) Comment this.
			public bool ignorePropertyChange = false;

			/// <summary>
			/// Specifies whether the binding is initialized.
			/// </summary>
			private bool initialized = false;

			/// <summary>
			/// The owner of this instance.
			/// </summary>
			private PropertyChangedHandle owner;

			#endregion fields

			#region constructors

			/// <summary>
			/// Initializes a new instance of the <see cref="BindingHandler"/> class.
			/// </summary>
			/// <param name="owner">The PropertyChangedHandle owner instance.</param>
			public BindingHandler(PropertyChangedHandle owner)
			{
				this.owner = owner;
				BindingOperations.SetBinding(this, PropertyProperty, owner.Binding);

				this.initialized = true;
			}

			#endregion constructors

			#region properties

			/// <summary>
			/// Gets or sets the value of the Property property.
			///	This is a Dependency Property.
			/// </summary>
			public object Property
			{
				get { return (object)this.GetValue(PropertyProperty); }
				set { this.SetValue(PropertyProperty, value); }
			}

			#endregion properties
		}

		#endregion nested types
	}
}