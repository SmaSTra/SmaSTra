namespace Common
{
	using System;
	using System.Reflection;
	using System.Windows.Data;

	#region Enumerations

	// TODO: (PS) Comment this.
	public enum UniversalEventType
	{
		Event,
		PropertyChange
	}

	#endregion Enumerations

	#region Delegates

	// TODO: (PS) Comment this.
	public delegate void UniversalEventCallback(UniversalEventCallbackArgs args);

	#endregion Delegates

	// TODO: (PS) Comment this.
	public class UniversalEventCallbackArgs
	{
		#region constructors

		public UniversalEventCallbackArgs(UniversalEventHandle handle, PropertyChangedCallbackArgs args)
		{
			if (handle == null)
			{
				throw new ArgumentNullException("handle");
			}
			if (args == null)
			{
				throw new ArgumentNullException("args");
			}

			this.Handle = handle;
			this.PropertyChangedCallbackArgs = args;
		}

		public UniversalEventCallbackArgs(UniversalEventHandle handle, object[] args)
		{
			if (handle == null)
			{
				throw new ArgumentNullException("handle");
			}
			if (args == null)
			{
				throw new ArgumentNullException("args");
			}

			this.Handle = handle;
			this.EventArgs = args;
		}

		#endregion constructors

		#region properties

		public object[] EventArgs
		{
			get;
			private set;
		}

		public UniversalEventHandle Handle
		{
			get;
			private set;
		}

		public PropertyChangedCallbackArgs PropertyChangedCallbackArgs
		{
			get;
			private set;
		}

		#endregion properties
	}

	// TODO: (PS) Comment this.
	// TODO: (PS) Finish this.
	public class UniversalEventHandle
	{
		#region static methods

		public static UniversalEventHandle GetInstance(object source, EventInfo eventInfo, UniversalEventCallback callback)
		{
			return DistinctInstanceProvider<UniversalEventHandle>.Instance.GetDistinctInstance(new UniversalEventHandle(source, eventInfo, callback));
		}

		public static UniversalEventHandle GetInstance(Binding binding, UniversalEventCallback callback)
		{
			return DistinctInstanceProvider<UniversalEventHandle>.Instance.GetDistinctInstance(new UniversalEventHandle(binding, callback));
		}

		#endregion static methods

		#region fields

		private int hashCode;

		/// <summary>
		/// TODO: (PS) Comment this.
		/// </summary>
		private WeakReference weakSourceReference = null;

		#endregion fields

		#region constructors

		private UniversalEventHandle(object source, EventInfo eventInfo, UniversalEventCallback callback)
		{
			if (eventInfo == null)
			{
				throw new ArgumentNullException("eventInfo");
			}
			if (callback == null)
			{
				throw new ArgumentNullException("callback");
			}

			this.UniversalEventType = UniversalEventType.Event;
			this.EventInfo = eventInfo;
			GenericAction.AddGenericEventHandler(source, eventInfo, this.OnEventChanged);
			this.Callback = callback;

			this.hashCode = HashCodeOperations.Combine(source ?? 0, eventInfo, callback);
		}

		private UniversalEventHandle(Binding binding, UniversalEventCallback callback)
		{
			if (binding == null)
			{
				throw new ArgumentNullException("binding");
			}
			if (callback == null)
			{
				throw new ArgumentNullException("callback");
			}

			this.UniversalEventType = UniversalEventType.PropertyChange;
			this.PropertyChangedHandle = PropertyChangedHandle.GetDistinctInstance(binding, this.OnPropertyChanged);
			this.Callback = callback;

			this.hashCode = HashCodeOperations.Combine(this.PropertyChangedHandle, callback);
		}

		#endregion constructors

		#region properties

		public UniversalEventCallback Callback
		{
			get;
			private set;
		}

		public EventInfo EventInfo
		{
			get;
			private set;
		}

		public PropertyChangedHandle PropertyChangedHandle
		{
			get;
			private set;
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
				if (this.weakSourceReference == null || !this.weakSourceReference.IsAlive)
				{
					return null;
				}
				else
				{
					return this.weakSourceReference.Target;
				}
			}
			set
			{
				if (value != null)
				{
					this.weakSourceReference = new WeakReference(value);
				}
				else
				{
					this.weakSourceReference = null;
				}
			}
		}

		public UniversalEventType UniversalEventType
		{
			get;
			private set;
		}

		#endregion properties

		#region overrideable methods

		public override bool Equals(object obj)
		{
			UniversalEventHandle other = obj as UniversalEventHandle;

			return other != null && (object.ReferenceEquals(this, obj) ||
				(object.Equals(this.Source, other.Source) &&
				object.Equals(this.EventInfo, other.EventInfo) &&
				object.Equals(this.PropertyChangedHandle, other.PropertyChangedHandle) &&
				object.Equals(this.Callback, other.Callback)));
		}

		public override int GetHashCode()
		{
			return this.hashCode;
		}

		#endregion overrideable methods

		#region methods

		private void OnEventChanged(object[] args)
		{
			this.Callback(new UniversalEventCallbackArgs(this, args));
		}

		private void OnPropertyChanged(PropertyChangedCallbackArgs args)
		{
			this.Callback(new UniversalEventCallbackArgs(this, args));
		}

		#endregion methods
	}
}