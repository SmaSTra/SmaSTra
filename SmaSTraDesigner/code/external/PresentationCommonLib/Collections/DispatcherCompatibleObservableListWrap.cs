namespace Common.Collections
{
	using System.Collections.Generic;
	using System.Windows.Threading;

	// TODO: (PS) Comment this.
	public class DispatcherCompatibleObservableListWrap<T> : DispatcherCompatibleObservableCollectionWrap<T>, IList<T>
	{
		#region fields

		protected IList<T> innerList;

		#endregion fields

		#region constructors

		public DispatcherCompatibleObservableListWrap(IList<T> innerList, Dispatcher dispatcher)
			: base(innerList, dispatcher)
		{
			this.innerList = innerList;
		}

		#endregion constructors

		#region indexers

		public T this[int index]
		{
			get
			{
				return this.innerList[index];
			}
			set
			{
				this.innerList[index] = value;
			}
		}

		#endregion indexers

		#region methods

		public int IndexOf(T item)
		{
			return this.innerList.IndexOf(item);
		}

		public void Insert(int index, T item)
		{
			this.innerList.Insert(index, item);
		}

		public void RemoveAt(int index)
		{
			this.innerList.RemoveAt(index);
		}

		#endregion methods
	}
}