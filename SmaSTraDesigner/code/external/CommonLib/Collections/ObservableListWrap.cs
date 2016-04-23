namespace Common.Collections
{
	using System;
	using System.Collections;
	using System.Collections.Generic;
	using System.Collections.Specialized;

	/// <summary>
	/// Wrapper class for classes that implement IList&lt;T&gt;.
	/// Provides CollectionChanged event for changes in the wrapped list and
	/// PropertyChanged event for the Count property.
	/// </summary>
	/// <typeparam name="T">Type of items.</typeparam>
	public class ObservableListWrap<T> : ObservableCollectionWrapBase<IList<T>, T>, IList<T>, IList
	{
		#region fields

		/// <summary>
		/// Specifies if the inner collection implements the IList interface and
		/// not just the IList&lt;T&gt; interface.
		/// </summary>
		private bool isIList;

		#endregion fields

		#region constructors

		/// <summary>
		/// Initializes a new instance of the <see cref="ObservableListWrap&lt;T&gt;"/> class.
		/// </summary>
		/// <param name="innerList">The list that is supposed to be wrapped.</param>
		/// <param name="canAccessInnerCollection">if set to <c>true</c> the programmer can access the inner collection using the InnerCollection property.</param>
		public ObservableListWrap(IList<T> innerList, bool canAccessInnerCollection = true)
			: base(innerList, canAccessInnerCollection)
		{
			this.isIList = innerList is IList;
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
					object oldItem = this.innerCollection[index];
					((IList)this.innerCollection)[index] = value;

					this.OnCollectionChanged(new NotifyCollectionChangedEventArgs(NotifyCollectionChangedAction.Replace,
						(IList)new List<object>() { oldItem },
						(IList)new List<object> { value },
						index));
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
		/// Gets or sets the <see cref="T"/> at the specified index.
		/// </summary>
		public T this[int index]
		{
			get
			{
				return this.innerCollection[index];
			}
			set
			{
				T oldItem = this.innerCollection[index];
				this.innerCollection[index] = value;

				this.OnCollectionChanged(new NotifyCollectionChangedEventArgs(NotifyCollectionChangedAction.Replace,
					(IList)new List<T>() { oldItem },
					(IList)new List<T> { value },
					index));
			}
		}

		#endregion indexers

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
				int result = this.innerCollection.Count;

				((IList)this.innerCollection).Add(value);

				this.OnCollectionChanged(new NotifyCollectionChangedEventArgs(NotifyCollectionChangedAction.Add, (object)value));

				return result;
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
		public int IndexOf(T item)
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
		public void Insert(int index, T item)
		{
			this.innerCollection.Insert(index, item);

			this.OnCollectionChanged(new NotifyCollectionChangedEventArgs(NotifyCollectionChangedAction.Add, (object)item, index));
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

				this.OnCollectionChanged(new NotifyCollectionChangedEventArgs(NotifyCollectionChangedAction.Add, (object)value, index));
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

				this.OnCollectionChanged(new NotifyCollectionChangedEventArgs(NotifyCollectionChangedAction.Remove, (object)value));
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

			this.OnCollectionChanged(new NotifyCollectionChangedEventArgs(NotifyCollectionChangedAction.Remove, (object)this.innerCollection[index], index));
		}

		#endregion methods
	}
}