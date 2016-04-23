namespace Common.Collections
{
	using System;
	using System.Collections.Generic;
	using System.Collections.Specialized;
	using System.ComponentModel;
	using System.Windows.Threading;

	// TODO: (PS) Comment this.
	public class DispatcherCompatibleObservableCollectionWrap<T> : ICollection<T>, INotifyCollectionChanged, INotifyPropertyChanged
	{
		#region fields

		protected ICollection<T> innerCollection;

		#endregion fields

		#region constructors

		public DispatcherCompatibleObservableCollectionWrap(ICollection<T> innerCollection, Dispatcher dispatcher)
		{
			if (innerCollection == null)
			{
				throw new ArgumentNullException("innerCollection");
			}
			if (!(innerCollection is INotifyCollectionChanged && innerCollection is INotifyPropertyChanged))
			{
				throw new ArgumentException("Inner collection must implement INotifyCollectionChanged and INotifyPropertyChanged", "innerCollection");
			}
			if (dispatcher == null)
			{
				throw new ArgumentNullException("dispatcher");
			}

			this.innerCollection = innerCollection;
			this.Dispatcher = dispatcher;
			((INotifyCollectionChanged)innerCollection).CollectionChanged += this.InnerCollection_CollectionChanged;
			((INotifyPropertyChanged)innerCollection).PropertyChanged += this.InnerCollection_PropertyChanged;
		}

		#endregion constructors

		#region events

		public event NotifyCollectionChangedEventHandler CollectionChanged;

		public event PropertyChangedEventHandler PropertyChanged;

		#endregion events

		#region properties

		public int Count
		{
			get { return this.innerCollection.Count; }
		}

		public Dispatcher Dispatcher
		{
			get;
			private set;
		}

		public bool IsReadOnly
		{
			get { return this.innerCollection.IsReadOnly; }
		}

		#endregion properties

		#region methods

		public void Add(T item)
		{
			this.innerCollection.Add(item);
		}

		public void Clear()
		{
			this.innerCollection.Clear();
		}

		public bool Contains(T item)
		{
			return this.innerCollection.Contains(item);
		}

		public void CopyTo(T[] array, int arrayIndex)
		{
			this.innerCollection.CopyTo(array, arrayIndex);
		}

		public IEnumerator<T> GetEnumerator()
		{
			return this.innerCollection.GetEnumerator();
		}

		public bool Remove(T item)
		{
			return this.innerCollection.Remove(item);
		}

		/// <summary>
		/// Raises the <see cref="E:CollectionChanged"/> event.
		/// </summary>
		/// <param name="e">The CollectionChangeEventArgs instance containing the event data.</param>
		protected void OnCollectionChanged(NotifyCollectionChangedEventArgs e)
		{
			if (this.CollectionChanged != null)
			{
				this.Dispatcher.BeginInvoke(new Action(delegate()
				{
					this.CollectionChanged(this, e);
				}));
			}
		}

		protected void OnPropertyChanged(string propertyName)
		{
			if (this.PropertyChanged != null)
			{
				this.Dispatcher.BeginInvoke(new Action(delegate()
				{
					this.PropertyChanged(this, new PropertyChangedEventArgs(propertyName));
				}));
			}
		}

		System.Collections.IEnumerator System.Collections.IEnumerable.GetEnumerator()
		{
			return this.GetEnumerator();
		}

		#endregion methods

		#region event handlers

		private void InnerCollection_PropertyChanged(object sender, PropertyChangedEventArgs e)
		{
			this.OnPropertyChanged(e.PropertyName);
		}

		private void InnerCollection_CollectionChanged(object sender, NotifyCollectionChangedEventArgs e)
		{
			this.OnCollectionChanged(e);
		}

		#endregion event handlers
	}
}