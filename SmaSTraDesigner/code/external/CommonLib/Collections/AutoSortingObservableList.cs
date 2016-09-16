namespace Common.Collections
{
	using System;
	using System.Collections;
	using System.Collections.Generic;
	using System.Collections.ObjectModel;
	using System.Collections.Specialized;
	using System.ComponentModel;
	using System.Linq;

	using Common.ExtensionMethods;

	// TODO: (PS) Comment this.
	public class AutoSortingObservableList<T> : IList<T>, INotifyCollectionChanged, INotifyPropertyChanged
	{
		#region fields

		private Func<T, T, int> compareMethod;
		private ObservableCollection<T> sortedCollection = new ObservableCollection<T>();
		private ICollection<T> wrappedCollection;

		#endregion fields

		#region constructors

		public AutoSortingObservableList(IComparer<T> comparer, ICollection<T> wrappedCollection = null)
		{
			if (comparer == null)
			{
				throw new ArgumentNullException("comparer");
			}

			this.compareMethod = comparer.Compare;
			this.Init(wrappedCollection);
		}

		public AutoSortingObservableList(Func<T, T, int> compareMethod, ICollection<T> wrappedCollection = null)
		{
			if (compareMethod == null)
			{
				throw new ArgumentNullException("compareMethod");
			}

			this.compareMethod = compareMethod;
			this.Init(wrappedCollection);
		}

		public AutoSortingObservableList(IComparer comparer, ICollection<T> wrappedCollection = null)
		{
			if (comparer == null)
			{
				throw new ArgumentNullException("comparer");
			}

			this.compareMethod = (x, y) => comparer.Compare(x, y);
			this.Init(wrappedCollection);
		}

		#endregion constructors

		#region events

		public event NotifyCollectionChangedEventHandler CollectionChanged
		{
			add
			{
				this.sortedCollection.CollectionChanged += value;
			}
			remove
			{
				this.sortedCollection.CollectionChanged -= value;
			}
		}

		public event PropertyChangedEventHandler PropertyChanged
		{
			add
			{
				((INotifyPropertyChanged)this.sortedCollection).PropertyChanged += value;
			}
			remove
			{
				((INotifyPropertyChanged)this.sortedCollection).PropertyChanged -= value;
			}
		}

		#endregion events

		#region properties

		public int Count
		{
			get { return this.sortedCollection.Count; }
		}

		public bool IsReadOnly
		{
			get { return false; }
		}

		#endregion properties

		#region indexers

		public T this[int index]
		{
			get
			{
				return this.sortedCollection[index];
			}
			set
			{
				this.sortedCollection[index] = value;
			}
		}

		#endregion indexers

		#region methods

		public void Add(T item)
		{
			this.sortedCollection.SortIn(item, this.compareMethod);
		}

		public void Clear()
		{
			this.sortedCollection.Clear();
		}

		public bool Contains(T item)
		{
			return this.sortedCollection.Contains(item);
		}

		public void CopyTo(T[] array, int index)
		{
			this.sortedCollection.CopyTo(array, index);
		}

		public IEnumerator<T> GetEnumerator()
		{
			return this.sortedCollection.GetEnumerator();
		}

		public int IndexOf(T item)
		{
			return this.sortedCollection.IndexOf(item);
		}

		public void Insert(int index, T item)
		{
			this.sortedCollection.Insert(index, item);
		}

		public bool Remove(T item)
		{
			return this.sortedCollection.Remove(item);
		}

		public void RemoveAt(int index)
		{
			this.sortedCollection.RemoveAt(index);
		}

		private void Init(ICollection<T> wrappedCollection)
		{
			this.wrappedCollection = wrappedCollection;
			if (wrappedCollection != null)
			{
				foreach (T item in wrappedCollection)
				{
					this.sortedCollection.SortIn(item, this.compareMethod);
				}

				INotifyCollectionChanged observable = wrappedCollection as INotifyCollectionChanged;
				if (observable != null)
				{
					observable.CollectionChanged += this.WrappedCollection_CollectionChanged;
				}
			}
		}

		System.Collections.IEnumerator System.Collections.IEnumerable.GetEnumerator()
		{
			return this.GetEnumerator();
		}

		#endregion methods

		#region event handlers

		private void WrappedCollection_CollectionChanged(object sender, NotifyCollectionChangedEventArgs e)
		{
			if (e.Action == NotifyCollectionChangedAction.Remove || e.Action == NotifyCollectionChangedAction.Replace)
			{
				if (e.OldItems != null)
				{
					foreach (var item in e.OldItems.Cast<T>())
					{
						this.sortedCollection.Remove(item);
					}
				}
			}

			if (e.Action == NotifyCollectionChangedAction.Add || e.Action == NotifyCollectionChangedAction.Replace)
			{
				if (e.NewItems != null)
				{
					foreach (var item in e.NewItems.Cast<T>())
					{
						this.Add(item);
					}
				}
			}

			if (e.Action == NotifyCollectionChangedAction.Reset)
			{
				this.sortedCollection.Clear();
			}
		}

		#endregion event handlers
	}
}