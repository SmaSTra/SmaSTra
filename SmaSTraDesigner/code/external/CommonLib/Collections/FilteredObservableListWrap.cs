namespace Common.Collections
{
	using System;
	using System.Collections;
	using System.Collections.Generic;
	using System.Collections.Specialized;
	using System.Linq;

	/// <summary>
	/// Wapper for a filtered view (using the FilterFunction property) of an IList that implements INotifyCollectionChanged.
	/// Provides CollectionChanged event for changes in the wrapped collection and
	/// PropertyChanged event for the Count property.
	/// </summary>
	/// <typeparam name="TList">Type of the wrapped list or collection.</typeparam>
	/// <typeparam name="TItems">Type of the items.</typeparam>
	public class FilteredObservableListWrap<TList, TItems> : FilteredObservableCollectionWrap<TList, TItems>, IList<TItems>, IList
		where TList : IList<TItems>, INotifyCollectionChanged
	{
		#region fields

		/// <summary>
		/// Specifies if the inner collection implements the IList interface and
		/// not just the IList&lt;T&gt; interface.
		/// </summary>
		private bool isIList;

		/// <summary>
		/// Holds a copy of the inner collection.
		/// Is used to compare the inner collection after it changes its' content to
		/// its' prior state.
		/// </summary>
		private List<TItems> oldInnerCollection;

		#endregion fields

		#region constructors

		/// <summary>
		/// Initializes a new instance of the <see cref="FilteredObservableListWrap&lt;TList, TItems&gt;"/> class.
		/// </summary>
		/// <param name="innerList">The inner list.</param>
		/// <param name="filterFunction">The function that is used to filter the inner list.</param>
		/// <param name="canAccessInnerCollection">if set to <c>true</c> the inner list can be accessed using the InnerCollection property.</param>
		public FilteredObservableListWrap(TList innerList, Func<TItems, bool> filterFunction, bool canAccessInnerCollection = true)
			: base(innerList, filterFunction, canAccessInnerCollection)
		{
			this.isIList = innerList is IList;
			this.oldInnerCollection = innerList.ToList();
		}

		#endregion constructors

		#region properties

		/// <summary>
		/// Gets a value indicating whether the <see cref="T:System.Collections.IList"/> has a fixed size.
		/// </summary>
		/// <value></value>
		/// <returns>true if the <see cref="T:System.Collections.IList"/> has a fixed size; otherwise, false.
		/// </returns>
		public bool IsFixedSize
		{
			get
			{
				if (this.isIList)
				{
					return ((IList)this.innerCollection).IsFixedSize;
				}
				else
				{
					throw new InvalidOperationException("Inner list does not implement IList.");
				}
			}
		}

		/// <summary>
		/// Gets or sets the <see cref="System.Object"/> at the specified index.
		/// </summary>
		/// <value></value>
		object IList.this[int index]
		{
			get
			{
				if (this.isIList)
				{
					return ((IList)this.innerCollection)[index];
				}
				else
				{
					throw new InvalidOperationException("Inner list does not implement IList.");
				}
			}
			set
			{
				if (this.isIList)
				{
					((IList)this.innerCollection)[index] = value;
				}
				else
				{
					throw new InvalidOperationException("Inner list does not implement IList.");
				}
			}
		}

		#endregion properties

		#region indexers

		/// <summary>
		/// Gets or sets the <see cref="TItems"/> at the specified index.
		/// </summary>
		public TItems this[int index]
		{
			get
			{
				return this.innerCollection[index];
			}
			set
			{
				this.innerCollection[index] = value;
			}
		}

		#endregion indexers

		#region overrideable methods

		/// <summary>
		/// Called when the inner list changes.
		/// </summary>
		/// <param name="oldFilteredList">The old filtered list.</param>
		/// <param name="e">The <see cref="System.Collections.Specialized.NotifyCollectionChangedEventArgs"/> instance containing the event data.</param>
		protected override void OnInnerCollectionChanged(List<TItems> oldFilteredList, NotifyCollectionChangedEventArgs e)
		{
			// Apply the collection changes of the inner list to the filtered list.
			List<TItems> filteredNewItems;
			switch (e.Action)
			{
				default:
				case NotifyCollectionChangedAction.Add:
					if (oldFilteredList.Count != this.filteredList.Count)
					{
						filteredNewItems = e.NewItems.Cast<TItems>().Where(this.FilterFunction).ToList();
						this.OnCollectionChanged(new NotifyCollectionChangedEventArgs(e.Action, filteredNewItems,
							this.TransfromIndex(e)));
					}
					break;

				case NotifyCollectionChangedAction.Move:
					filteredNewItems = e.OldItems.Cast<TItems>().Where(this.FilterFunction).ToList();
					if (filteredNewItems.Count != 0)
					{
						this.OnCollectionChanged(new NotifyCollectionChangedEventArgs(e.Action, filteredNewItems,
							this.TransfromIndex(e, true),
							this.TransfromIndex(e)));
					}
					break;

				case NotifyCollectionChangedAction.Remove:
					if (oldFilteredList.Count != this.filteredList.Count)
					{
						filteredNewItems = e.OldItems.Cast<TItems>().Where(this.FilterFunction).ToList();
						this.OnCollectionChanged(new NotifyCollectionChangedEventArgs(e.Action, filteredNewItems,
							this.TransfromIndex(e)));
					}
					break;

				case NotifyCollectionChangedAction.Replace:
					if (e.NewItems.Count == 1 && e.OldItems.Count == 1)
					{
						filteredNewItems = e.NewItems.Cast<TItems>().Where(this.FilterFunction).ToList();
						List<TItems> filteredOldItems = e.OldItems.Cast<TItems>().Where(this.FilterFunction).ToList();

						if (filteredNewItems.Count != 0 || filteredOldItems.Count != 0)
						{
							if (filteredNewItems.Count == filteredOldItems.Count)
							{
								this.OnCollectionChanged(new NotifyCollectionChangedEventArgs(e.Action,
									(object)filteredNewItems.First(),
									(object)filteredOldItems.First(),
									this.TransfromIndex(e, true)));
							}
							else if (filteredNewItems.Count == 0)
							{
								this.OnCollectionChanged(new NotifyCollectionChangedEventArgs(NotifyCollectionChangedAction.Remove,
									(object)filteredOldItems.First(),
									this.TransfromIndex(e, true)));
							}
							else
							{
								this.OnCollectionChanged(new NotifyCollectionChangedEventArgs(NotifyCollectionChangedAction.Add,
									(object)filteredNewItems.First(),
									this.TransfromIndex(e)));
							}
						}
					}
					else
					{
						List<TItems> newFilteredList = this.filteredList;
						this.filteredList = new List<TItems>();
						this.OnCollectionChanged(new NotifyCollectionChangedEventArgs(NotifyCollectionChangedAction.Reset));

						foreach (var item in newFilteredList)
						{
							this.filteredList.Add(item);
							this.OnCollectionChanged(new NotifyCollectionChangedEventArgs(NotifyCollectionChangedAction.Add, item));
						}
					}
					break;

				case NotifyCollectionChangedAction.Reset:
					base.OnInnerCollectionChanged(oldFilteredList, e);
					break;
			}

			this.oldInnerCollection = this.innerCollection.ToList();
		}

		#endregion overrideable methods

		#region methods

		/// <summary>
		/// Adds an item to the <see cref="T:System.Collections.IList"/>.
		/// </summary>
		/// <param name="value">The <see cref="T:System.Object"/> to add to the <see cref="T:System.Collections.IList"/>.</param>
		/// <returns>
		/// The position into which the new element was inserted.
		/// </returns>
		/// <exception cref="T:System.NotSupportedException">
		/// The <see cref="T:System.Collections.IList"/> is read-only.
		/// -or-
		/// The <see cref="T:System.Collections.IList"/> has a fixed size.
		/// </exception>
		public int Add(object value)
		{
			if (this.isIList)
			{
				return ((IList)this.innerCollection).Add(value);
			}
			else
			{
				throw new InvalidOperationException("Inner list does not implement IList.");
			}
		}

		/// <summary>
		/// Determines whether the <see cref="T:System.Collections.IList"/> contains a specific value.
		/// </summary>
		/// <param name="value">The <see cref="T:System.Object"/> to locate in the <see cref="T:System.Collections.IList"/>.</param>
		/// <returns>
		/// true if the <see cref="T:System.Object"/> is found in the <see cref="T:System.Collections.IList"/>; otherwise, false.
		/// </returns>
		public bool Contains(object value)
		{
			if (this.isIList)
			{
				return ((IList)this.innerCollection).Contains(value);
			}
			else
			{
				throw new InvalidOperationException("Inner list does not implement IList.");
			}
		}

		/// <summary>
		/// Determines the index of a specific item in the IList&lt;T&gt.
		/// </summary>
		/// <param name="item">The object to locate.</param>
		/// <returns>The index of item if found in the list; otherwise, -1.</returns>
		public int IndexOf(TItems item)
		{
			return this.innerCollection.IndexOf(item);
		}

		/// <summary>
		/// Determines the index of a specific item in the <see cref="T:System.Collections.IList"/>.
		/// </summary>
		/// <param name="value">The <see cref="T:System.Object"/> to locate in the <see cref="T:System.Collections.IList"/>.</param>
		/// <returns>
		/// The index of <paramref name="value"/> if found in the list; otherwise, -1.
		/// </returns>
		public int IndexOf(object value)
		{
			if (this.isIList)
			{
				return ((IList)this.innerCollection).IndexOf(value);
			}
			else
			{
				throw new InvalidOperationException("Inner list does not implement IList.");
			}
		}

		/// <summary>
		/// Inserts the item at the specified index.
		/// </summary>
		/// <param name="index">The index.</param>
		/// <param name="item">The item.</param>
		public void Insert(int index, TItems item)
		{
			this.innerCollection.Insert(index, item);
		}

		/// <summary>
		/// Inserts an item to the <see cref="T:System.Collections.IList"/> at the specified index.
		/// </summary>
		/// <param name="index">The zero-based index at which <paramref name="value"/> should be inserted.</param>
		/// <param name="value">The <see cref="T:System.Object"/> to insert into the <see cref="T:System.Collections.IList"/>.</param>
		/// <exception cref="T:System.ArgumentOutOfRangeException">
		/// 	<paramref name="index"/> is not a valid index in the <see cref="T:System.Collections.IList"/>.
		/// </exception>
		/// <exception cref="T:System.NotSupportedException">
		/// The <see cref="T:System.Collections.IList"/> is read-only.
		/// -or-
		/// The <see cref="T:System.Collections.IList"/> has a fixed size.
		/// </exception>
		/// <exception cref="T:System.NullReferenceException">
		/// 	<paramref name="value"/> is null reference in the <see cref="T:System.Collections.IList"/>.
		/// </exception>
		public void Insert(int index, object value)
		{
			if (this.isIList)
			{
				((IList)this.innerCollection).Insert(index, value);
			}
			else
			{
				throw new InvalidOperationException("Inner list does not implement IList.");
			}
		}

		/// <summary>
		/// Removes the first occurrence of a specific object from the <see cref="T:System.Collections.IList"/>.
		/// </summary>
		/// <param name="value">The <see cref="T:System.Object"/> to remove from the <see cref="T:System.Collections.IList"/>.</param>
		/// <exception cref="T:System.NotSupportedException">
		/// The <see cref="T:System.Collections.IList"/> is read-only.
		/// -or-
		/// The <see cref="T:System.Collections.IList"/> has a fixed size.
		/// </exception>
		public void Remove(object value)
		{
			if (this.isIList)
			{
				((IList)this.innerCollection).Remove(value);
			}
			else
			{
				throw new InvalidOperationException("Inner list does not implement IList.");
			}
		}

		/// <summary>
		/// Removes the item at the specified index.
		/// </summary>
		/// <param name="index">The index.</param>
		public void RemoveAt(int index)
		{
			this.innerCollection.RemoveAt(index);
		}

		/// <summary>
		/// Transfroms the list indeces (Old- or NewStartingIndex, depending on 'oldStartingIndex' argument) of the
		/// given NotifyCollectionChangedEventArgs to its equivalent in the filtered list.
		/// </summary>
		/// <param name="e">The <see cref="System.Collections.Specialized.NotifyCollectionChangedEventArgs"/> instance containing the event data.</param>
		/// <param name="oldStartingIndex">if set to <c>true</c> the OldStartingIndex of the NotifyCollectionChangedEventArgs is used; otherwise it's the NewStartingIndex.</param>
		/// <returns></returns>
		private int TransfromIndex(NotifyCollectionChangedEventArgs e, bool oldStartingIndex = false)
		{
			IList<TItems> innerCollection;
			int index;
			if (oldStartingIndex)
			{
				innerCollection = this.oldInnerCollection;
				index = e.OldStartingIndex;
			}
			else
			{
				innerCollection = this.innerCollection;
				index = e.NewStartingIndex;
			}

			if (this.FilterFunction != null && this.ApplyFilter)
			{
				int result = -1;
				for (int i = 0; i <= index; i++)
				{
					if (this.FilterFunction(this.innerCollection[i]))
					{
						result++;
					}
				}

				return result;
			}
			else
			{
				return index;
			}
		}

		#endregion methods
	}
}