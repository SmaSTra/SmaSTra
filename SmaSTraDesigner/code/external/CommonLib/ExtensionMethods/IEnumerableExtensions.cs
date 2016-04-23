namespace Common.ExtensionMethods
{
	using System;
	using System.Collections;
	using System.Collections.Generic;
	using System.Linq;

	// TODO: (PS) Comment this.
	public static class IEnumerableExtensions
	{
		#region extension methods

		public static bool EqualsElementwise(this IEnumerable subject, IEnumerable other)
		{
			return EqualsElementwise(subject, other, object.Equals);
		}

		public static bool EqualsElementwise(this IEnumerable subject, IEnumerable other, Func<object, object, bool> comparer)
		{
			if (comparer == null)
			{
				throw new ArgumentNullException("comparer");
			}

			if (subject == null || other == null)
			{
				return subject == other;
			}

			var enumerator1 = subject.GetEnumerator();
			var enumerator2 = other.GetEnumerator();
			bool canMove1 = enumerator1.MoveNext();
			bool canMove2 = enumerator2.MoveNext();
			if (canMove1 && canMove2)
			{
				do
				{
					if (!comparer(enumerator1.Current, enumerator2.Current))
					{
						return false;
					}

					canMove1 = enumerator1.MoveNext();
					canMove2 = enumerator2.MoveNext();
				}
				while (canMove1 && canMove2);
			}

			return !(canMove1 || canMove2);
		}

		public static bool EqualsElementwise(this IEnumerable subject, IEnumerable other, IEqualityComparer comparer)
		{
			if (comparer == null)
			{
				throw new ArgumentNullException("comparer");
			}

			return EqualsElementwise(subject, other, comparer.Equals);
		}

		public static bool EqualsElementwise(this IEnumerable subject, IEnumerable other, IComparer comparer)
		{
			if (comparer == null)
			{
				throw new ArgumentNullException("comparer");
			}

			return EqualsElementwise(subject, other, (x, y) => comparer.Compare(x, y) == 0);
		}

		public static int IndexOf<T>(this IEnumerable<T> subject, T item)
		{
			if (subject == null)
			{
				throw new ArgumentNullException("subject");
			}

			int i = 0;
			foreach (var listItem in subject)
			{
				if (object.Equals(listItem, item))
				{
					return i;
				}

				i++;
			}

			return -1;
		}

		public static int IndexOf<T>(this IEnumerable<T> subject, T item, IEqualityComparer<T> comparer)
		{
			if (subject == null)
			{
				throw new ArgumentNullException("subject");
			}
			if (comparer == null)
			{
				throw new ArgumentNullException("comparer");
			}

			int i = 0;
			foreach (var listItem in subject)
			{
				if (comparer.Equals(listItem, item))
				{
					return i;
				}

				i++;
			}

			return -1;
		}

		public static int IndexOf<T>(this IEnumerable<T> subject, T item, IComparer<T> comparer)
		{
			if (subject == null)
			{
				throw new ArgumentNullException("subject");
			}
			if (comparer == null)
			{
				throw new ArgumentNullException("comparer");
			}

			int i = 0;
			foreach (var listItem in subject)
			{
				if (comparer.Compare(listItem, item) == 0)
				{
					return i;
				}

				i++;
			}

			return -1;
		}

		/// <summary>
		/// Returns the index of the first item in the list that fulfills
		/// the condition discribed on the "condition" function.
		/// </summary>
		/// <param name="subject">Owner of this extension method.</param>
		/// <param name="condition">A condition as function reference or lambda statement.</param>
		/// <returns>
		/// the found index, if no index was found -1 is returned
		/// </returns>
		public static int IndexOfWhere<T>(this IEnumerable<T> subject, Func<T, bool> condition)
		{
			if (subject == null)
			{
				throw new ArgumentNullException("subject");
			}
			if (condition == null)
			{
				throw new ArgumentNullException("condition");
			}

			int i = 0;
			foreach (var item in subject)
			{
				if (condition(item))
				{
					return i;
				}

				i++;
			}

			return -1;
		}

		/// <summary>
		/// Returns the index of the first item in the list that fulfills
		/// the condition discribed on the "condition" function.
		/// </summary>
		/// <param name="subject">Owner of this extension method.</param>
		/// <param name="condition">A condition as function reference or lambda statement.</param>
		/// <param name="startIndex">The start index.</param>
		/// <returns>
		/// the found index, if no index was found -1 is returned
		/// </returns>
		public static int IndexOfWhere<T>(this IList<T> subject, Func<T, bool> condition, int startIndex)
		{
			if (subject == null)
			{
				throw new ArgumentNullException("subject");
			}

			return IndexOfWhere(subject.Skip(startIndex), condition);
		}

		public static Queue<T> ToQueue<T>(this IEnumerable<T> subject)
		{
			if (subject == null)
			{
				throw new ArgumentNullException("subject");
			}

			return new Queue<T>(subject);
		}

		public static Stack<T> ToStack<T>(this IEnumerable<T> subject)
		{
			if (subject == null)
			{
				throw new ArgumentNullException("subject");
			}

			return new Stack<T>(subject);
		}

		#endregion extension methods
	}
}