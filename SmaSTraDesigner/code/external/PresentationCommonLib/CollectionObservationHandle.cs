namespace Common
{
	using System;
	using System.Collections.Specialized;
	using System.Windows.Data;

	#region Enumerations

	/// <summary>
	/// Enumeration used to specify what kind of change the
	/// property that is hooked to the CollectionObservationHandle
	/// instance underwent.
	/// </summary>
	public enum ChangeType
	{
		PropertyChanged = 0,
		CollectionChanged
	}

	#endregion Enumerations

	/// <summary>
	/// Arguments for the callback method of a CollectionObservationHandle.
	/// </summary>
	public class CollectionObservationCallbackArgs
	{
		#region constructors

		/// <summary>
		/// Initializes a new instance of the <see cref="CollectionObservationCallbackArgs"/> class.
		/// </summary>
		/// <param name="handle">The CollectionObservationHandle instance that is assiciated with the callback.</param>
		/// <param name="propertyChangedCallbackArgs">The PropertyChangedCallbackArgs instance that was caused by a property change.</param>
		public CollectionObservationCallbackArgs(CollectionObservationHandle handle, PropertyChangedCallbackArgs propertyChangedCallbackArgs)
		{
			this.ChangeType = ChangeType.PropertyChanged;
			this.Handle = handle;
			this.PropertyChangedCallbackArgs = propertyChangedCallbackArgs;
		}

		/// <summary>
		/// Initializes a new instance of the <see cref="CollectionObservationCallbackArgs"/> class.
		/// </summary>
		/// <param name="handle">The CollectionObservationHandle instance that is assiciated with the callback.</param>
		/// <param name="notifyCollectionChangedEventArgs">The <see cref="System.Collections.Specialized.NotifyCollectionChangedEventArgs"/> instance containing the event data.</param>
		public CollectionObservationCallbackArgs(CollectionObservationHandle handle, NotifyCollectionChangedEventArgs notifyCollectionChangedEventArgs)
		{
			this.ChangeType = ChangeType.CollectionChanged;
			this.Handle = handle;
			this.NotifyCollectionChangedEventArgs = notifyCollectionChangedEventArgs;
		}

		#endregion constructors

		#region properties

		/// <summary>
		/// Gets or sets the type of the property change which can be either a
		/// change in value of the property itsself or a change in the containing collection.
		/// </summary>
		public ChangeType ChangeType
		{
			get;
			private set;
		}

		/// <summary>
		/// Gets or sets the CollectionObservationHandle instance that is assiciated with the callback.
		/// </summary>
		public CollectionObservationHandle Handle
		{
			get;
			private set;
		}

		/// <summary>
		/// Gets or sets the NotifyCollectionChangedEventArgs that were given with the collection change.
		/// Is null if callback was called as a result of a property change.
		/// </summary>
		public NotifyCollectionChangedEventArgs NotifyCollectionChangedEventArgs
		{
			get;
			private set;
		}

		/// <summary>
		/// Gets or sets the PropertyChangedCallbackArgs that were given with the property change.
		/// Is null if callback was called as a result of a collection change.
		/// </summary>
		public PropertyChangedCallbackArgs PropertyChangedCallbackArgs
		{
			get;
			private set;
		}

		#endregion properties
	}

	/// <summary>
	/// Used to monitor changes in a poperty of a type that implements INotifyCollectionChanged.
	/// A callback method is called whenever the property changes to another value or the collection it contains
	/// raises the CollectionChanged event.
	/// </summary>
	public class CollectionObservationHandle : IDisposable
	{
		#region static methods

		// TODO: (PS) Comment this.
		public static CollectionObservationHandle GetDistinctInstance(Binding binding, Action<CollectionObservationCallbackArgs> callback, bool keepAlive = false)
		{
			if (callback == null)
			{
				throw new ArgumentNullException("callback");
			}

			return DistinctInstanceProvider<CollectionObservationHandle>.Instance.GetDistinctInstance(new CollectionObservationHandle(binding, callback, keepAlive));
		}

		/// <summary>
		/// Creates a new instance of this class or if one with the same parameters as the given ones allready exists returns that one.
		/// </summary>
		/// <param name="source">The source object that contains the property to monitor.</param>
		/// <param name="path">The path from the source object to the property to monitor.</param>
		/// <param name="callback">The callback method that is supposed to be called when changes happen in the property.</param>
		/// <returns>Returns a new instance of this class or if one with the same parameters as the given ones allready exists returns that one</returns>
		public static CollectionObservationHandle GetDistinctInstance(object source, string path, Action<CollectionObservationCallbackArgs> callback, bool keepAlive = false)
		{
			if (source == null)
			{
				throw new ArgumentNullException("source");
			}
			if (String.IsNullOrWhiteSpace(path))
			{
				throw new ArgumentException("String argument 'path' must not be null or empty (incl. whitespace).", "path");
			}

			return GetDistinctInstance(new Binding(path) { Source = source }, callback);
		}

		#endregion static methods

		#region constructors

		// TODO: (PS) Comment this.
		/// <summary>
		/// Initializes a new instance of the <see cref="CollectionObservationHandle"/> class.
		/// </summary>
		/// <param name="source">The source object that contains the property to monitor.</param>
		/// <param name="path">The path from the source object to the property to monitor.</param>
		/// <param name="callback">The callback method that is supposed to be called when changes happen in the property.</param>
		private CollectionObservationHandle(Binding binding, Action<CollectionObservationCallbackArgs> callback, bool keepAlive)
		{
			this.PropertyChangedHandle = PropertyChangedHandle.GetDistinctInstance(binding, this.OnMonitoredPropertyChanged, keepAlive);
			this.UpdateCollectionChangedEventHandler(null, this.PropertyChangedHandle.PropertyValue as INotifyCollectionChanged);
			this.Callback = callback;
		}

		/// <summary>
		/// Releases unmanaged resources and performs other cleanup operations before the
		/// <see cref="CollectionObservationHandle"/> is reclaimed by garbage collection.
		/// </summary>
		~CollectionObservationHandle()
		{
			DistinctInstanceProvider<CollectionObservationHandle>.Instance.RemoveDistinctInstance(this);
			this.Dispose();
		}

		#endregion constructors

		#region properties

		/// <summary>
		/// Gets the callback method of this instance that is called whenever the property or its'
		/// containing collection change.
		/// </summary>
		public Action<CollectionObservationCallbackArgs> Callback
		{
			get;
			private set;
		}

		/// <summary>
		/// Gets or sets a value indicating whether this <see cref="CollectionObservationHandle"/> is disposed.
		/// </summary>
		/// <value><c>true</c> if disposed; otherwise, <c>false</c>.</value>
		public bool Disposed
		{
			get;
			protected set;
		}

		/// <summary>
		/// Gets the PropertyChangedHandle that is used to monitor value changes in the monitored property.
		/// </summary>
		public PropertyChangedHandle PropertyChangedHandle
		{
			get;
			private set;
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
			CollectionObservationHandle other = obj as CollectionObservationHandle;

			return other != null &&
				this.PropertyChangedHandle.Equals(other.PropertyChangedHandle, false) &&
				this.Callback.Equals(other.Callback);
		}

		/// <summary>
		/// Returns a hash code for this instance.
		/// Used object class's GetHashCode() method.
		/// </summary>
		/// <returns>
		/// A hash code for this instance, suitable for use in hashing algorithms and data structures like a hash table. 
		/// </returns>
		public override int GetHashCode()
		{
			return HashCodeOperations.Combine(this.PropertyChangedHandle, this.Callback);
		}

		#endregion overrideable methods

		#region methods

		/// <summary>
		/// Performs application-defined tasks associated with freeing, releasing, or resetting unmanaged resources.
		/// </summary>
		public void Dispose()
		{
			if (!this.Disposed)
			{
				this.Disposed = true;

				this.UpdateCollectionChangedEventHandler(this.PropertyChangedHandle.PropertyValue as INotifyCollectionChanged, null);
				this.PropertyChangedHandle.Dispose();
			}
		}

		/// <summary>
		/// Called when the monitored property changes its' value.
		/// </summary>
		/// <param name="e">The callback arguments of the property change.</param>
		private void OnMonitoredPropertyChanged(PropertyChangedCallbackArgs e)
		{
			this.UpdateCollectionChangedEventHandler(e.OldValue as INotifyCollectionChanged, e.NewValue as INotifyCollectionChanged);
			this.Callback(new CollectionObservationCallbackArgs(this, e));
		}

		/// <summary>
		/// Adds/removes the CollectionChanged event handler to/from the new/old value.
		/// </summary>
		/// <param name="newValue">The new value of the monitored property.</param>
		private void UpdateCollectionChangedEventHandler(INotifyCollectionChanged oldValue, INotifyCollectionChanged newValue)
		{
			if (oldValue != null)
			{
				newValue.CollectionChanged -= WeakEventHandler.GetHandler(this.MonitoredCollection_CollectionChanged);
			}

			if (newValue != null)
			{
				newValue.CollectionChanged += WeakEventHandler.GetHandler(this.MonitoredCollection_CollectionChanged);
			}
		}

		#endregion methods

		#region event handlers

		/// <summary>
		/// Handles the CollectionChanged event of the current value of the monitored property.
		/// </summary>
		/// <param name="sender">The source of the event.</param>
		/// <param name="e">The <see cref="System.Collections.Specialized.NotifyCollectionChangedEventArgs"/> instance containing the event data.</param>
		private void MonitoredCollection_CollectionChanged(object sender, NotifyCollectionChangedEventArgs e)
		{
			this.Callback(new CollectionObservationCallbackArgs(this, e));
		}

		#endregion event handlers
	}
}