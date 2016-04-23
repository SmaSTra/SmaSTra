namespace Common
{
	using System;
	using System.Collections.Specialized;
	using System.ComponentModel;

	// TODO: (PS) Comment this.
	public static class WeakEventHandler
	{
		#region static methods

		public static EventHandler<T> GetHandler<T>(EventHandler<T> handler)
			where T : EventArgs
		{
			return GetInstance<T>(handler).Invoke;
		}

		public static EventHandler GetHandler(EventHandler handler)
		{
			return GetInstance(handler).Invoke;
		}

		public static NotifyCollectionChangedEventHandler GetHandler(NotifyCollectionChangedEventHandler handler)
		{
			return GetInstance(handler).Invoke;
		}

		public static PropertyChangedEventHandler GetHandler(PropertyChangedEventHandler handler)
		{
			return GetInstance(handler).Invoke;
		}

		public static PropertyChangingEventHandler GetHandler(PropertyChangingEventHandler handler)
		{
			return GetInstance(handler).Invoke;
		}

		public static WeakAction<object, T> GetInstance<T>(EventHandler<T> handler)
			where T : EventArgs
		{
			return WeakAction<object, T>.GetInstance(new Action<object, T>(handler));
		}

		public static WeakAction<object, EventArgs> GetInstance(EventHandler handler)
		{
			return WeakAction<object, EventArgs>.GetInstance(new Action<object, EventArgs>(handler));
		}

		public static WeakAction<object, NotifyCollectionChangedEventArgs> GetInstance(NotifyCollectionChangedEventHandler handler)
		{
			return WeakAction<object, NotifyCollectionChangedEventArgs>.GetInstance(new Action<object, NotifyCollectionChangedEventArgs>(handler));
		}

		public static WeakAction<object, PropertyChangedEventArgs> GetInstance(PropertyChangedEventHandler handler)
		{
			return WeakAction<object, PropertyChangedEventArgs>.GetInstance(new Action<object, PropertyChangedEventArgs>(handler));
		}

		public static WeakAction<object, PropertyChangingEventArgs> GetInstance(PropertyChangingEventHandler handler)
		{
			return WeakAction<object, PropertyChangingEventArgs>.GetInstance(new Action<object, PropertyChangingEventArgs>(handler));
		}

		#endregion static methods
	}
}