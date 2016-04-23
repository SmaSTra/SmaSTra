namespace Common
{
	using System;
	using System.Collections.Generic;
	using System.Linq;
	using System.Windows.Data;

	using Common.ExtensionMethods;

	// TODO: (PS) Comment this.
	public class MultiPropertyChangedCallbackArgs
	{
		#region constructors

		public MultiPropertyChangedCallbackArgs(MultiPropertyChangedHandle handle, object[] oldValues, object[] newValues, int changeIndex)
		{
			this.Handle = handle;
			this.OldValues = oldValues;
			this.NewValues = newValues;
			this.ChangeIndex = changeIndex;
		}

		#endregion constructors

		#region properties

		public int ChangeIndex
		{
			get;
			private set;
		}

		public MultiPropertyChangedHandle Handle
		{
			get;
			private set;
		}

		public object[] NewValues
		{
			get;
			private set;
		}

		public object[] OldValues
		{
			get;
			private set;
		}

		#endregion properties
	}

	// TODO: (PS) Comment this.
	public class MultiPropertyChangedHandle : IDisposable
	{
		#region static fields

		private static HashSet<MultiPropertyChangedHandle> keptAliveInstances = new HashSet<MultiPropertyChangedHandle>();

		#endregion static fields

		#region static methods

		public static bool Equals(object objA, object objB, bool includeCallback)
		{
			MultiPropertyChangedHandle handleA = objA as MultiPropertyChangedHandle;
			MultiPropertyChangedHandle handleB = objB as MultiPropertyChangedHandle;

			return (objA == null && objB == null) || (handleA != null && handleB != null && handleA.Equals(handleB, includeCallback));
		}

		public static MultiPropertyChangedHandle GetDistinctInstance(Binding[] sources, Action<MultiPropertyChangedCallbackArgs> callback, bool keepAlive = false)
		{
			bool instanceAllreadyExists;
			var result = DistinctInstanceProvider<MultiPropertyChangedHandle>.Instance.GetDistinctInstance(
				new MultiPropertyChangedHandle(sources, callback, keepAlive), out instanceAllreadyExists);

			if (keepAlive && !instanceAllreadyExists)
			{
				keptAliveInstances.Add(result);
			}

			return result;
		}

		#endregion static methods

		#region fields

		private CallbackProvider[] callbackProviders;
		private object[] oldValues;

		#endregion fields

		#region constructors

		private MultiPropertyChangedHandle(Binding[] sources, Action<MultiPropertyChangedCallbackArgs> callback, bool keepAlive)
		{
			if (sources == null)
			{
				throw new ArgumentNullException("sources");
			}
			if (sources.Length == 0)
			{
				throw new ArgumentException("Argument list 'sources' must not be empty.", "sources");
			}
			if (callback == null)
			{
				throw new ArgumentNullException("callback");
			}

			this.PropertyChangedHandles = new PropertyChangedHandle[sources.Length];
			this.oldValues = new object[sources.Length];
			this.callbackProviders = new CallbackProvider[sources.Length];
			for (int i = 0; i < sources.Length; i++)
			{
				this.callbackProviders[i] = new CallbackProvider(this, i);
				this.PropertyChangedHandles[i] = PropertyChangedHandle.GetDistinctInstance(sources[i], this.callbackProviders[i].OnPropertyChanged);
				this.oldValues[i] = this.PropertyChangedHandles[i].PropertyValue;
			}

			this.Callback = callback;
			this.KeepAlive = keepAlive;
		}

		~MultiPropertyChangedHandle()
		{
			DistinctInstanceProvider<MultiPropertyChangedHandle>.Instance.RemoveDistinctInstance(this);
		}

		#endregion constructors

		#region properties

		public Action<MultiPropertyChangedCallbackArgs> Callback
		{
			get;
			private set;
		}

		public bool KeepAlive
		{
			get;
			private set;
		}

		public PropertyChangedHandle[] PropertyChangedHandles
		{
			get;
			private set;
		}

		#endregion properties

		#region overrideable methods

		public override bool Equals(object obj)
		{
			return this.Equals(obj, true);
		}

		public override int GetHashCode()
		{
			return HashCodeOperations.Combine(HashCodeOperations.Combine(this.PropertyChangedHandles.AsEnumerable()), this.Callback);
		}

		#endregion overrideable methods

		#region methods

		/// <summary>
		/// Performs application-defined tasks associated with freeing, releasing, or resetting unmanaged resources.
		/// </summary>
		public void Dispose()
		{
			keptAliveInstances.Remove(this);
		}

		public bool Equals(object obj, bool includeCallback)
		{
			MultiPropertyChangedHandle other = obj as MultiPropertyChangedHandle;

			return other != null && (object.ReferenceEquals(this, obj) ||
				(this.PropertyChangedHandles.EqualsElementwise(other.PropertyChangedHandles, (x, y) => PropertyChangedHandle.Equals(x, y, false)) &&
				(!includeCallback || this.Callback.Equals(other.Callback))));
		}

		private void OnPropertyChanged(int changedIndex, object newValue)
		{
			object[] newValues = this.oldValues.ToArray();
			newValues[changedIndex] = newValue;
			this.Callback(new MultiPropertyChangedCallbackArgs(this, this.oldValues.ToArray(), newValues, changedIndex));
			this.oldValues[changedIndex] = newValue;
		}

		#endregion methods

		#region nested types

		private struct CallbackProvider
		{
			#region fields

			public int Index;
			public MultiPropertyChangedHandle Owner;

			#endregion fields

			#region constructors

			public CallbackProvider(MultiPropertyChangedHandle owner, int index)
			{
				this.Owner = owner;
				this.Index = index;
			}

			#endregion constructors

			#region methods

			public void OnPropertyChanged(PropertyChangedCallbackArgs args)
			{
				this.Owner.OnPropertyChanged(this.Index, args.NewValue);
			}

			#endregion methods
		}

		#endregion nested types
	}
}