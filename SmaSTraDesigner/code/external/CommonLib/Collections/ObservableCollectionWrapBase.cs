﻿namespace Common.Collections
{
	using System;
	using System.Linq;
	using System.Collections;
	using System.Collections.Generic;
	using System.Collections.Specialized;
	using System.ComponentModel;

	/// <summary>
	/// Base class for wrapper classes for classes that implement TCollection type.
	/// Provides CollectionChanged event for changes in the wrapped collection and
	/// PropertyChanged event for the Count property.
	/// </summary>
	/// <typeparam name="TCollection">The type of the collection.</typeparam>
	/// <typeparam name="TItems">The type of the items.</typeparam>
	public class ObservableCollectionWrapBase<TCollection, TItems> : ICollection<TItems>, ICollection, INotifyCollectionChanged, INotifyPropertyChanged
		where TCollection : ICollection<TItems>
	{
		#region fields

		/// <summary>
		/// The wrapped collection.
		/// </summary>
		protected TCollection innerCollection;

		/// <summary>
		/// Specifies if the inner collection implements the ICollection interface and
		/// not just the ICollection&lt;T&gt; interface.
		/// </summary>
		private bool isICollection;

		#endregion fields

		#region constructors

		/// <summary>
		/// Initializes a new instance of the <see cref="ObservableCollectionWrapBase&lt;TCollection, TItems&gt;"/> class.
		/// </summary>
		/// <param name="innerList">The list that is supposed to be wrapped.</param>
		/// <param name="canAccessInnerCollection">if set to <c>true</c> the programmer can access the inner collection using the InnerCollection property.</param>
		protected ObservableCollectionWrapBase(TCollection innerCollection, bool canAccessInnerCollection)
		{
			if (innerCollection == null)
			{
				throw new ArgumentNullException("innerCollection");
			}

			this.isICollection = innerCollection is ICollection;
			this.innerCollection = innerCollection;
			this.CanAccessInnerCollection = canAccessInnerCollection;
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
			get { return this.innerCollection.Count; }
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

		#region methods

		/// <summary>
		/// Adds the specified item.
		/// </summary>
		/// <param name="item">The item.</param>
		public void Add(TItems item)
		{
			this.innerCollection.Add(item);

			this.OnCollectionChanged(new NotifyCollectionChangedEventArgs(NotifyCollectionChangedAction.Add, (object)item));
		}

		/// <summary>
		/// Clears this instance.
		/// </summary>
		public void Clear()
		{
			if (this.innerCollection.Count != 0)
			{
				var removedItems = this.innerCollection.ToList();
				this.innerCollection.Clear();

				this.OnCollectionChanged(new NotifyCollectionChangedEventArgs(NotifyCollectionChangedAction.Reset, removedItems));
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
			return this.innerCollection.Contains(item);
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
				((ICollection)this.innerCollection).CopyTo(array, index);
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
			this.innerCollection.CopyTo(array, arrayIndex);
		}

		/// <summary>
		/// Returns an enumerator that iterates through the collection.
		/// </summary>
		/// <returns>An enumerator that can be used to iterate through the collection.</returns>
		public IEnumerator<TItems> GetEnumerator()
		{
			return this.innerCollection.GetEnumerator();
		}

		/// <summary>
		/// Removes the specified item.
		/// </summary>
		/// <param name="item">The item.</param>
		/// <returns>true removing the item was successful.</returns>
		public bool Remove(TItems item)
		{
			bool result = this.innerCollection.Remove(item);

			if (result)
			{
				this.OnCollectionChanged(new NotifyCollectionChangedEventArgs(NotifyCollectionChangedAction.Remove, (object)item));
			}

			return result;
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
			return ((IEnumerable)this.innerCollection).GetEnumerator();
		}

		#endregion methods
	}
}