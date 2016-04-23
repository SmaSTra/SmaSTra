namespace Common.Collections
{
	using System;
	using System.Collections.Generic;
	using System.Linq;

	// TODO: (PS) Comment this.
	public class ArraySegmentWrapper<T> : IList<T>
	{
		#region fields

		private T[] array = null;

		#endregion fields

		#region constructors

		public ArraySegmentWrapper(T[] array, int offset, int count)
		{
			if (array == null)
			{
				throw new ArgumentNullException("array");
			}
			if (offset < 0 || offset >= array.Length)
			{
				throw new ArgumentException("Offset is outside the bounds of the given array.", "offset");
			}
			if (count < 0 || count > array.Length - offset)
			{
				throw new ArgumentException("Count is outside the bounds of the given array", "count");
			}

			this.array = array;
			this.Offset = offset;
			this.Count = count;
		}

		#endregion constructors

		#region properties

		public int Count
		{
			get;
			private set;
		}

		public bool IsReadOnly
		{
			get { return true; }
		}

		public int Offset
		{
			get;
			private set;
		}

		// TODO: (PS) Comment this.
		public T[] OriginalArray
		{
			get { return this.array; }
		}

		#endregion properties

		#region indexers

		public T this[int index]
		{
			get
			{
				return this.array[this.Offset + index];
			}
			set
			{
				this.array[this.Offset + index] = value;
			}
		}

		#endregion indexers

		#region methods

		public void Add(T item)
		{
			throw new NotSupportedException();
		}

		public void Clear()
		{
			throw new NotSupportedException();
		}

		public bool Contains(T item)
		{
			return this.IndexOf(item) >= 0;
		}

		public void CopyTo(T[] array, int arrayIndex)
		{
			this.array.CopyTo(array, arrayIndex);
		}

		public T[] GetArraySegment()
		{
			return this.array.Skip(this.Offset).Take(this.Count).ToArray();
		}

		public IEnumerator<T> GetEnumerator()
		{
			return this.array.Skip(this.Offset).Take(this.Offset).GetEnumerator();
		}

		public int IndexOf(T item)
		{
			if (item == null)
			{
				throw new ArgumentNullException("item");
			}

			int i = 0;
			foreach (T arrayItem in this)
			{
				if (object.Equals(item, arrayItem))
				{
					return i;
				}

				i++;
			}

			return -1;
		}

		public void Insert(int index, T item)
		{
			throw new NotSupportedException();
		}

		public bool Remove(T item)
		{
			throw new NotSupportedException();
		}

		public void RemoveAt(int index)
		{
			throw new NotSupportedException();
		}

		System.Collections.IEnumerator System.Collections.IEnumerable.GetEnumerator()
		{
			return this.GetEnumerator();
		}

		#endregion methods
	}
}