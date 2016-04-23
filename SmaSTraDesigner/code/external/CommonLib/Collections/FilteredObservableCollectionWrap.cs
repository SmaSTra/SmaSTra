namespace Common.Collections
{
	using System;
	using System.Collections;
	using System.Collections.Generic;
	using System.Collections.Specialized;
	using System.ComponentModel;
	using System.Linq;

	/// <summary>
	/// Wapper for a filtered view (using the FilterFunction property) of an ICollection that implements INotifyCollectionChanged.
	/// Provides CollectionChanged event for changes in the wrapped collection and
	/// PropertyChanged event for the Count property.
	/// </summary>
	/// <typeparam name="TCollection">The type of the collection.</typeparam>
	/// <typeparam name="TItems">The type of the items.</typeparam>
	public class FilteredObservableCollectionWrap<TCollection, TItems> : ICollection<TItems>, ICollection, INotifyCollectionChanged, INotifyPropertyChanged, IDisposable
		where TCollection : ICollection<TItems>, INotifyCollectionChanged
	{
		#region fields

		/// <summary>
		/// List of the items that are not filtered out by the filter function.
		/// </summary>
		protected List<TItems> filteredList;

		/// <summary>
		/// The wrapped collection.
		/// </summary>
		protected TCollection innerCollection;

		/// <summary>
		/// Specifies whether the given filter function is applied or not.
		/// </summary>
		private bool applyFilter = true;

		/// <summary>
		/// The function that is used to filter the inner collection.
		/// If the function returns true for an item, that item shows up in this list instance;
		/// otherwise it does not.
		/// </summary>
		private Func<TItems, bool> filterFunction;

		/// <summary>
		/// Specifies whether the inner collection implements the ICollection interface and
		/// not just the ICollection&lt;T&gt; interface.
		/// </summary>
		private bool isICollection;

		#endregion fields

		#region constructors

		/// <summary>
		/// Initializes a new instance of the <see cref="FilteredObservableCollectionWrap&lt;TCollection, TItems&gt;"/> class.
		/// </summary>
		/// <param name="innerCollection">The inner collection.</param>
		/// <param name="filterFunction">The function that is used to filter the inner collection.</param>
		/// <param name="canAccessInnerCollection">if set to <c>true</c> the inner collection can be accessed using the InnerCollection property.</param>
		public FilteredObservableCollectionWrap(TCollection innerCollection, Func<TItems, bool> filterFunction, bool canAccessInnerCollection = true)
		{
			if (innerCollection == null)
			{
				throw new ArgumentNullException("innerCollection");
			}

			this.isICollection = innerCollection is ICollection;
			this.innerCollection = innerCollection;
			this.filterFunction = filterFunction;
			this.CanAccessInnerCollection = canAccessInnerCollection;

			this.filteredList = this.GetFilteredList();
			this.innerCollection.CollectionChanged += this.InnerCollection_CollectionChanged;
		}

		#endregion constructors

		#region events

		/// <summary>
		/// Occurs when the underlying collecton is changed.
		/// </summary>
		public event NotifyCollectionChangedEventHandler CollectionChanged;

		/// <summary>
		/// Occurs when a property value changes.
		/// </summary>
		public event PropertyChangedEventHandler PropertyChanged;

		#endregion events

		#region properties

		/// <summary>
		/// Gets or sets a value indicating whether the given filter function is applied or not.
		/// Any changes to this property forces this instance to refilter and raise a
		/// CollectionChanged event.
		/// </summary>
		public bool ApplyFilter
		{
			get
			{
				return this.applyFilter;
			}
			set
			{
				if (this.applyFilter != value)
				{
					this.applyFilter = value;
					this.OnPropertyChanged("ApplyFilter");

					if (value || this.FilterFunction != null)
					{
						this.Refilter();
					}
				}
			}
		}

		/// <summary>
		/// Gets a value indicating whether external code can access the inner collection.
		/// </summary>
		public bool CanAccessInnerCollection
		{
			get;
			private set;
		}

		/// <summary>
		/// Gets the item count.
		/// </summary>
		public int Count
		{
			get
			{
				if (this.filteredList != null)
				{
					return this.filteredList.Count;
				}
				else
				{
					return 0;
				}
			}
		}

		/// <summary>
		/// Gets or sets the function that is used to filter the inner collection.
		/// If the function returns true for an item, that item shows up in this list instance;
		/// otherwise it does not.
		/// Any changes to this property forces this instance to refilter and raise a
		/// CollectionChanged event.
		/// </summary>
		public Func<TItems, bool> FilterFunction
		{
			get
			{
				return this.filterFunction;
			}
			set
			{
				if (this.filterFunction != value)
				{
					this.filterFunction = value;
					this.OnPropertyChanged("FilterFunction");

					if (value != null || this.ApplyFilter)
					{
						this.Refilter();
					}
				}
			}
		}

		/// <summary>
		/// Gets the inner collection if allowed by the CanAccessInnerCollection property.
		/// </summary>
		public TCollection InnerCollection
		{
			get
			{
				if (!this.CanAccessInnerCollection)
				{
					throw new InvalidOperationException("InnerCollection cannot be accessed when CanAccessInnerCollection is false");
				}

				return this.innerCollection;
			}
		}

		/// <summary>
		/// Gets a value indicating whether this instance is read only.
		/// </summary>
		/// <value>
		/// 	<c>true</c> if this instance is read only; otherwise, <c>false</c>.
		/// </value>
		public bool IsReadOnly
		{
			get { return this.innerCollection.IsReadOnly; }
		}

		/// <summary>
		/// Gets a value indicating whether access to the <see cref="T:System.Collections.ICollection"/> is synchronized (thread safe).
		/// </summary>
		/// <value></value>
		/// <returns>true if access to the <see cref="T:System.Collections.ICollection"/> is synchronized (thread safe); otherwise, false.
		/// </returns>
		public bool IsSynchronized
		{
			get
			{
				if (this.isICollection)
				{
					return ((ICollection)this.innerCollection).IsSynchronized;
				}
				else
				{
					throw new InvalidOperationException("Inner collection does not implement ICollection.IsSynchronized.");
				}
			}
		}

		/// <summary>
		/// Gets an object that can be used to synchronize access to the <see cref="T:System.Collections.ICollection"/>.
		/// </summary>
		/// <value></value>
		/// <returns>
		/// An object that can be used to synchronize access to the <see cref="T:System.Collections.ICollection"/>.
		/// </returns>
		public object SyncRoot
		{
			get
			{
				if (this.isICollection)
				{
					return ((ICollection)this.innerCollection).SyncRoot;
				}
				else
				{
					throw new InvalidOperationException("Inner collection does not implement ICollection.SyncRoot.");
				}
			}
		}

		#endregion properties

		#region overrideable methods

		/// <summary>
		/// Called when the inner collection changed.
		/// </summary>
		/// <param name="oldFilteredList">The old filtered list.</param>
		/// <param name="e">The <see cref="System.Collections.Specialized.NotifyCollectionChangedEventArgs"/> instance containing the event data.</param>
		protected virtual void OnInnerCollectionChanged(List<TItems> oldFilteredList, NotifyCollectionChangedEventArgs e)
		{
			// Apply the collection changes of the inner list to the filtered list.
			List<TItems> filteredNewItems;
			switch (e.Action)
			{
				default:
				case NotifyCollectionChangedAction.Add:
				case NotifyCollectionChangedAction.Remove:
					if (oldFilteredList.Count != this.filteredList.Count)
					{
						filteredNewItems = e.NewItems.Cast<TItems>().Where(this.FilterFunction).ToList();
						this.OnCollectionChanged(new NotifyCollectionChangedEventArgs(e.Action, filteredNewItems));
					}
					break;

				case NotifyCollectionChangedAction.Replace:
					filteredNewItems = e.NewItems.Cast<TItems>().Where(this.FilterFunction).ToList();
					List<TItems> filteredOldItems = e.OldItems.Cast<TItems>().Where(this.FilterFunction).ToList();

					this.OnCollectionChanged(new NotifyCollectionChangedEventArgs(e.Action, filteredNewItems, filteredOldItems));
					break;

				case NotifyCollectionChangedAction.Reset:
					if (this.filteredList.Count != 0)
					{
						this.OnCollectionChanged(new NotifyCollectionChangedEventArgs(e.Action));
					}
					break;

				case NotifyCollectionChangedAction.Move:
					break;
			}
		}

		#endregion overrideable methods

		#region methods

		/// <summary>
		/// Adds the specified item.
		/// </summary>
		/// <param name="item">The item.</param>
		public void Add(TItems item)
		{
			this.innerCollection.Add(item);
		}

		/// <summary>
		/// Clears this instance.
		/// </summary>
		public void Clear()
		{
			if (this.innerCollection.Count != 0)
			{
				this.innerCollection.Clear();
			}
		}

		/// <summary>
		/// Determines whether the wrapped list contains the specified item.
		/// </summary>
		/// <param name="item">The item.</param>
		/// <returns>
		/// 	<c>true</c> if the wrapped list contains the specified item; otherwise, <c>false</c>.
		/// </returns>
		public bool Contains(TItems item)
		{
			return this.filteredList.Contains(item);
		}

		/// <summary>
		/// Copies the elements of the <see cref="T:System.Collections.ICollection"/> to an <see cref="T:System.Array"/>, starting at a particular <see cref="T:System.Array"/> index.
		/// </summary>
		/// <param name="array">The one-dimensional <see cref="T:System.Array"/> that is the destination of the elements copied from <see cref="T:System.Collections.ICollection"/>. The <see cref="T:System.Array"/> must have zero-based indexing.</param>
		/// <param name="index">The zero-based index in <paramref name="array"/> at which copying begins.</param>
		/// <exception cref="T:System.ArgumentNullException">
		/// 	<paramref name="array"/> is null.
		/// </exception>
		/// <exception cref="T:System.ArgumentOutOfRangeException">
		/// 	<paramref name="index"/> is less than zero.
		/// </exception>
		/// <exception cref="T:System.ArgumentException">
		/// 	<paramref name="array"/> is multidimensional.
		/// -or-
		/// <paramref name="index"/> is equal to or greater than the length of <paramref name="array"/>.
		/// -or-
		/// The number of elements in the source <see cref="T:System.Collections.ICollection"/> is greater than the available space from <paramref name="index"/> to the end of the destination <paramref name="array"/>.
		/// </exception>
		/// <exception cref="T:System.ArgumentException">
		/// The type of the source <see cref="T:System.Collections.ICollection"/> cannot be cast automatically to the type of the destination <paramref name="array"/>.
		/// </exception>
		public void CopyTo(Array array, int index)
		{
			if (this.isICollection)
			{
				((ICollection)this.filteredList).CopyTo(array, index);
			}
			else
			{
				throw new InvalidOperationException("Inner collection does not implement ICollection.CopyTo(Array array, int index).");
			}
		}

		/// <summary>
		/// Copies the contents of the wrappec collection to the target array.
		/// </summary>
		/// <param name="array">The one-dimensional array that is the destination of the elements copied from the wrapped collection. The array must have zero-based indexing.</param>
		/// <param name="arrayIndex">The zero-based index in <paramref name="array"/> at which copying begins.</param>
		public void CopyTo(TItems[] array, int arrayIndex)
		{
			this.filteredList.CopyTo(array, arrayIndex);
		}

		/// <summary>
		/// Performs application-defined tasks associated with freeing, releasing, or resetting unmanaged resources.
		/// </summary>
		public void Dispose()
		{
			this.innerCollection.CollectionChanged -= this.InnerCollection_CollectionChanged;
		}

		/// <summary>
		/// Returns an enumerator that iterates through the collection.
		/// </summary>
		/// <returns>An enumerator that can be used to iterate through the collection.</returns>
		public IEnumerator<TItems> GetEnumerator()
		{
			return this.filteredList.GetEnumerator();
		}

		/// <summary>
		/// Refilters this instance.
		/// </summary>
		public void Refilter()
		{
			this.filteredList = new List<TItems>();
			this.OnCollectionChanged(new NotifyCollectionChangedEventArgs(NotifyCollectionChangedAction.Reset));

			List<TItems> newFilteredList = this.GetFilteredList();
			foreach (var item in newFilteredList)
			{
				this.filteredList.Add(item);
				this.OnCollectionChanged(new NotifyCollectionChangedEventArgs(NotifyCollectionChangedAction.Add, item));
			}
		}

		/// <summary>
		/// Removes the specified item.
		/// </summary>
		/// <param name="item">The item.</param>
		/// <returns>true removing the item was successful.</returns>
		public bool Remove(TItems item)
		{
			if (this.FilterFunction(item))
			{
				return this.innerCollection.Remove(item);
			}

			return false;
		}

		/// <summary>
		/// Raises the <see cref="E:CollectionChanged"/> event.
		/// </summary>
		/// <param name="e">The NotifyCollectionChangedEventArgs instance containing the event data.</param>
		protected void OnCollectionChanged(NotifyCollectionChangedEventArgs e)
		{
			switch (e.Action)
			{
				case NotifyCollectionChangedAction.Add:
				case NotifyCollectionChangedAction.Reset:
				case NotifyCollectionChangedAction.Remove:
					this.OnPropertyChanged("Count");
					break;

				case NotifyCollectionChangedAction.Move:
				case NotifyCollectionChangedAction.Replace:
				default:
					break;
			}

			if (this.CollectionChanged != null)
			{
				this.CollectionChanged(this, e);
			}
		}

		/// <summary>
		/// Gets a filtered list from the inner collection depending on the ApplyFilter and FilterFunction properties.
		/// </summary>
		/// <returns>See summary.</returns>
		private List<TItems> GetFilteredList()
		{
			if (this.applyFilter && this.filterFunction != null)
			{
				return this.innerCollection.Where(this.filterFunction).ToList();
			}
			else
			{
				return this.innerCollection.ToList();
			}
		}

		/// <summary>
		/// Called when a property changes its' value.
		/// </summary>
		/// <param name="propertyName">Name of the property.</param>
		private void OnPropertyChanged(string propertyName)
		{
			if (this.PropertyChanged != null)
			{
				this.PropertyChanged(this, new PropertyChangedEventArgs(propertyName));
			}
		}

		/// <summary>
		/// Returns an enumerator that iterates through a collection.
		/// </summary>
		/// <returns>
		/// An <see cref="T:System.Collections.IEnumerator"/> object that can be used to iterate through the collection.
		/// </returns>
		IEnumerator IEnumerable.GetEnumerator()
		{
			return this.GetEnumerator();
		}

		#endregion methods

		#region event handlers

		/// <summary>
		/// Handles the CollectionChanged event of the InnerCollection.
		/// </summary>
		/// <param name="sender">The source of the event.</param>
		/// <param name="e">The <see cref="System.Collections.Specialized.NotifyCollectionChangedEventArgs"/> instance containing the event data.</param>
		private void InnerCollection_CollectionChanged(object sender, NotifyCollectionChangedEventArgs e)
		{
			List<TItems> oldFilteredList = this.filteredList;
			this.filteredList = this.GetFilteredList();

			this.OnInnerCollectionChanged(oldFilteredList, e);
		}

		#endregion event handlers
	}
}