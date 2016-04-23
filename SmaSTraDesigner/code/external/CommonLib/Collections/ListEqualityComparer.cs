namespace Common.Collections
{
	using System;
	using System.Collections.Generic;

	// TODO: (PS) Comment this.
	public class ListEqualityComparer<T> : IEqualityComparer<IEnumerable<T>>, IEqualityComparer<ICollection<T>>, IEqualityComparer<T[]>
	{
		#region static methods

		public static bool Equals(IEnumerable<T> x, IEnumerable<T> y, IEqualityComparer<T> internalComparer = null)
		{
			if (internalComparer != null)
			{
				return ListEquals(x, -1, y, -1, internalComparer.Equals);
			}
			else
			{
				return ListEquals(x, -1, y, -1, DefaultElementEquals);
			}
		}

		public static bool Equals(ICollection<T> x, ICollection<T> y, IEqualityComparer<T> internalComparer = null)
		{
			if (internalComparer != null)
			{
				return ListEquals(x, x.Count, y, y.Count, internalComparer.Equals);
			}
			else
			{
				return ListEquals(x, x.Count, y, y.Count, DefaultElementEquals);
			}
		}

		public static bool Equals(T[] x, T[] y, IEqualityComparer<T> internalComparer = null)
		{
			if (internalComparer != null)
			{
				return ListEquals(x, x.Length, y, y.Length, internalComparer.Equals);
			}
			else
			{
				return ListEquals(x, x.Length, y, y.Length, DefaultElementEquals);
			}
		}

		private static bool ListEquals(IEnumerable<T> x, int xCount, IEnumerable<T> y, int yCount,
			Func<T, T, bool> compareFunction)
		{
			if (xCount != yCount)
			{
				return false;
			}

			using (var enumerator1 = x.GetEnumerator())
			{
				using (var enumerator2 = y.GetEnumerator())
				{
					bool xHasNext = enumerator1.MoveNext();
					bool yHasNext = enumerator2.MoveNext();
					while (xHasNext && yHasNext)
					{
						var item1 = enumerator1.Current;
						var item2 = enumerator2.Current;

						if (!compareFunction(item1, item2))
						{
							return false;
						}

						xHasNext = enumerator1.MoveNext();
						yHasNext = enumerator2.MoveNext();
					}

					if (xHasNext != yHasNext)
					{
						return false;
					}
				}
			}

			return true;
		}

		private static bool DefaultElementEquals(T x, T y)
		{
			return object.Equals(x, y);
		}

		#endregion static methods

		#region fields

		private IEqualityComparer<T> internalComparer = null;

		#endregion fields

		#region constructors

		public ListEqualityComparer()
		{
		}

		public ListEqualityComparer(IEqualityComparer<T> internalComparer)
		{
			this.internalComparer = internalComparer;
		}

		#endregion constructors

		#region methods

		bool IEqualityComparer<ICollection<T>>.Equals(ICollection<T> x, ICollection<T> y)
		{
			return Equals(x, y, this.internalComparer);
		}

		int IEqualityComparer<ICollection<T>>.GetHashCode(ICollection<T> obj)
		{
			return HashCodeOperations.Combine(obj);
		}

		bool IEqualityComparer<IEnumerable<T>>.Equals(IEnumerable<T> x, IEnumerable<T> y)
		{
			return Equals(x, y, this.internalComparer);
		}

		int IEqualityComparer<IEnumerable<T>>.GetHashCode(IEnumerable<T> obj)
		{
			return HashCodeOperations.Combine(obj);
		}

		bool IEqualityComparer<T[]>.Equals(T[] x, T[] y)
		{
			return Equals(x, y, this.internalComparer);
		}

		int IEqualityComparer<T[]>.GetHashCode(T[] obj)
		{
			return HashCodeOperations.Combine(obj);
		}

		#endregion methods
	}
}