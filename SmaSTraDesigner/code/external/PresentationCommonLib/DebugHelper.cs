namespace Common
{
	using System;
	using System.Collections;
	using System.Collections.Specialized;
	using System.Diagnostics;
	using System.Text;
	using System.Windows.Data;
#if DEBUG

	/// <summary>
	/// Contains functions to help with the debugging process.
	/// </summary>
	public static class DebugHelper
	{
		#region static methods

		/// <summary>
		/// Monitors an INotifyCollectionChanged property and prints out any changes in the console using Debug.WriteLine(...).
		/// </summary>
		/// <param name="source">The source object that contains the property to monitor.</param>
		/// <param name="path">The path to the property to monitor.</param>
		/// <param name="announceMonitorBegin">if set to <c>true</c> an anouncement is made using Debug.WriteLine(...) when this method is called.</param>
		/// <returns>
		/// The CollectionObservationHandle that is used to monitor the specified collection property.
		/// </returns>
		public static CollectionObservationHandle MonitorCollection(object source, string path, bool announceMonitorBegin = true)
		{
			if (announceMonitorBegin)
			{
				Console.WriteLine("Monitoring collection property path \"{0}\" of source \"{1}\".", path, source);
			}

			return CollectionObservationHandle.GetDistinctInstance(source, path, CollectionChangeCallback);
		}

		/// <summary>
		/// Monitors a property and prints out any changes in the console using Debug.WriteLine(...).
		/// </summary>
		/// <param name="source">The source object that contains the property to monitor.</param>
		/// <param name="path">The path to the property to monitor.</param>
		/// <param name="announceMonitorBegin">if set to <c>true</c> an anouncement is made using Debug.WriteLine(...) when this method is called.</param>
		/// <returns>
		/// The PropertyChangedHandle that is used to monitor the specified property.
		/// </returns>
		public static PropertyChangedHandle MonitorProperty(object source, string path, bool announceMonitorBegin = true)
		{
			if (announceMonitorBegin)
			{
				Debug.WriteLine("Monitoring property path \"{0}\" of source \"{1}\".", path, source);
			}

			return PropertyChangedHandle.GetDistinctInstance(source, path, PropertyChangeCallback, keepAlive:true);
		}

		// TODO: (PS) Comment this.
		public static PropertyChangedHandle MonitorProperty(Binding binding, bool announceMonitorBegin = true)
		{
			if (binding == null)
			{
				throw new ArgumentNullException("binding");
			}
			if (announceMonitorBegin)
			{
				Debug.WriteLine("Monitoring property path \"{0}\" of source \"{1}\".", binding.Path, binding.Source);
			}

			return PropertyChangedHandle.GetDistinctInstance(binding, PropertyChangeCallback, keepAlive: true);
		}

		/// <summary>
		/// Callback for changes in the collection property given to the MonitorCollection(...) method.
		/// Used to print out all changes in the console.
		/// </summary>
		/// <param name="sender">The sender.</param>
		/// <param name="e">The callback arguments.</param>
		private static void CollectionChangeCallback(CollectionObservationCallbackArgs e)
		{
			StringBuilder sb = new StringBuilder();
			switch (e.ChangeType)
			{
				default:
				case ChangeType.PropertyChanged:
					// The property containing the observable collection has changed.
					sb.AppendFormat("Collection property \"{0}\" of source \"{1}\" changed", e.Handle.PropertyChangedHandle.Path, e.Handle.PropertyChangedHandle.Source);
					if (e.PropertyChangedCallbackArgs.NewValue != null)
					{
						IEnumerable list = e.PropertyChangedCallbackArgs.NewValue as IEnumerable;
						if (list != null)
						{
							sb.Append(":\n");
							foreach (var item in list)
							{
								sb.AppendLine(String.Format("	{0}", item));
							}
						}
						else
						{
							sb.AppendFormat(" to \"{0}\".", e.PropertyChangedCallbackArgs.NewValue);
						}
					}
					else
					{
						sb.Append(" to null.");
					}

					break;

				case ChangeType.CollectionChanged:
					// The collection contained within the property has changed its' content in some way.
					sb.AppendFormat("Collection \"{0}\" of source \"{1}\" changed:\n", e.Handle.PropertyChangedHandle.Path, e.Handle.PropertyChangedHandle.Source);
					sb.AppendFormat("Action = {0}\n", e.NotifyCollectionChangedEventArgs.Action);
					if (e.NotifyCollectionChangedEventArgs.Action == NotifyCollectionChangedAction.Add ||
						e.NotifyCollectionChangedEventArgs.Action == NotifyCollectionChangedAction.Replace)
					{
						sb.AppendLine("Added Items:");
						foreach (var item in e.NotifyCollectionChangedEventArgs.NewItems)
						{
							sb.AppendLine(String.Format("	{0}", item));
						}
					}

					if (e.NotifyCollectionChangedEventArgs.Action == NotifyCollectionChangedAction.Remove ||
						e.NotifyCollectionChangedEventArgs.Action == NotifyCollectionChangedAction.Replace)
					{
						sb.AppendLine("Removed Items:");
						foreach (var item in e.NotifyCollectionChangedEventArgs.OldItems)
						{
							sb.AppendLine(String.Format("	{0}", item));
						}
					}

					break;
			}

			Debug.WriteLine(sb.ToString());
		}

		/// <summary>
		/// Callback for changes in the property given to the MonitorProperty(...) method.
		/// Used to print out all changes in the console.
		/// </summary>
		/// <param name="e">The callback arguments.</param>
		private static void PropertyChangeCallback(PropertyChangedCallbackArgs e)
		{
			StringBuilder sb = new StringBuilder();
			sb.AppendFormat("Property \"{0}\" of source \"{1}\" changed from ", e.Handle.Path, e.Handle.Source);

			if (e.OldValue != null)
			{
				sb.AppendFormat("\"{0}\"", e.OldValue.ToString());
			}
			else
			{
				sb.Append("null");
			}

			sb.Append(" to ");
			if (e.NewValue != null)
			{
				sb.AppendFormat("\"{0}\".", e.NewValue.ToString());
			}
			else
			{
				sb.Append("null.");
			}

			Debug.WriteLine(sb.ToString());
		}

		#endregion static methods
	}

	#endif
}