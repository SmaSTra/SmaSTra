namespace Common.ExtensionMethods
{
	using System;
	using System.Collections;
	using System.Collections.Generic;
	using System.Collections.Specialized;

	/// <summary>
	/// Extension Methods for IList
	/// </summary>
	public static class IListExtensions
	{
		#region extension methods

		/// <summary>
		/// Adds all items of the given list to this one.
		/// </summary>
		/// <typeparam name="T">Type placeholder</typeparam>
		/// <param name="subject">base IList</param>
		/// <param name="listToAdd">list to be added</param>
		/// <exception cref="ArgumentNullException"/>
		public static void AddRange(this IList subject, IEnumerable listToAdd)
		{
			if (subject == null)
			{
				throw new ArgumentNullException("subject");
			}
			if (listToAdd == null)
			{
				throw new ArgumentNullException("listToAdd");
			}

			foreach (object item in listToAdd)
			{
				subject.Add(item);
			}
		}

		/// <summary>
		/// Applies changes made to an ObservableCollection to owner list.
		/// Useful if owner is supposed to mirror the ObservableCollection.
		/// Use this method inside a CollectionChanged callback method.
		/// </summary>
		/// <param name="subject">Owner of this extension method.</param>
		/// <param name="sender">ObservableCollection that sent the CollectionChanged event</param>
		/// <param name="e">NotifyCollectionChangedEventArgs argument</param>
		public static void ApplyCollectionChanges(this IList subject, object sender, NotifyCollectionChangedEventArgs e)
		{
			ApplyCollectionChanges(subject, sender, e.Action, e.NewItems, e.NewStartingIndex, e.OldItems, e.OldStartingIndex);
		}

		/// <summary>
		/// Applies changes made to an ObservableCollection to owner list.
		/// Useful if owner is supposed to mirror the ObservableCollection.
		/// Use this method inside a CollectionChanged callback method.
		/// All items involved in this list's change are converted using the conversionMethod first.
		/// </summary>
		/// <param name="subject">Owner of this extension method.</param>
		/// <param name="sender">ObservableCollection that sent the CollectionChanged event</param>
		/// <param name="e">NotifyCollectionChangedEventArgs argument</param>
		/// <param name="convertMethod">Conversion method used.</param>
		public static void ApplyCollectionChanges(this IList subject, object sender, NotifyCollectionChangedEventArgs e,
			Func<object, NotifyCollectionChangedAction, object> convertMethod)
		{
			ApplyCollectionChanges(subject, sender, e.Action, e.NewItems, e.NewStartingIndex, e.OldItems, e.OldStartingIndex, convertMethod);
		}

		/// <summary>
		/// Applies changes made to an ObservableCollection to owner list.
		/// Useful if owner is supposed to mirror the ObservableCollection.
		/// Use this method inside a CollectionChanged callback method.
		/// </summary>
		/// <param name="subject">The list.</param>
		/// <param name="sender">The sender.</param>
		/// <param name="action">The action.</param>
		/// <param name="newItems">The new items.</param>
		/// <param name="newStartingIndex">New index of the starting.</param>
		/// <param name="oldItems">The old items.</param>
		/// <param name="oldStartingIndex">Old index of the starting.</param>
		public static void ApplyCollectionChanges(this IList subject,
			object sender, NotifyCollectionChangedAction action,
			IList newItems, int newStartingIndex, IList oldItems, int oldStartingIndex)
		{
			if (subject == null)
			{
				throw new ArgumentNullException("subject");
			}

			switch (action)
			{
				case NotifyCollectionChangedAction.Add:
					if (newStartingIndex > -1)
					{
						for (int i = newItems.Count - 1; i >= 0; i--)
						{
							subject.Insert(newStartingIndex, newItems[i]);
						}
					}
					else
					{
						foreach (object item in newItems)
						{
							subject.Add(item);
						}
					}

					break;

				case NotifyCollectionChangedAction.Move:
					subject.MoveItem(oldStartingIndex, newStartingIndex);
					break;

				case NotifyCollectionChangedAction.Remove:
					foreach (object o in oldItems)
					{
						subject.Remove(o);
					}

					break;

				case NotifyCollectionChangedAction.Replace:
					foreach (object o in oldItems)
					{
						subject.Remove(o);
					}

					for (int i = 0; i < newItems.Count; i++)
					{
						subject.Insert(oldStartingIndex + i, newItems[i]);
					}

					break;

				case NotifyCollectionChangedAction.Reset:
					if (sender != null)
					{
						subject.Clear();

						foreach (object o in (IList)sender)
						{
							subject.Add(o);
						}
					}

					break;
			}
		}

		/// <summary>
		/// Applies changes made to an ObservableCollection to owner list.
		/// Useful if owner is supposed to mirror the ObservableCollection.
		/// Use this method inside a CollectionChanged callback method.
		/// All items involved in this list's change are converted using the conversionMethod first.
		/// </summary>
		/// <param name="subject">Owner of this extension method.</param>
		/// <param name="sender">ObservableCollection that sent the CollectionChanged event</param>
		/// <param name="action">The action.</param>
		/// <param name="newItems">The new items.</param>
		/// <param name="newStartingIndex">New index of the starting.</param>
		/// <param name="oldItems">The old items.</param>
		/// <param name="oldStartingIndex">Old index of the starting.</param>
		/// <param name="convertMethod">Conversion method used.</param>
		public static void ApplyCollectionChanges(this IList subject,
			object sender, NotifyCollectionChangedAction action,
			IList newItems, int newStartingIndex, IList oldItems, int oldStartingIndex,
			Func<object, NotifyCollectionChangedAction, object> convertMethod)
		{
			if (subject == null)
			{
				throw new ArgumentNullException("subject");
			}
			if (convertMethod == null)
			{
				throw new ArgumentNullException("convertMethod");
			}

			switch (action)
			{
				case NotifyCollectionChangedAction.Add:
					if (newStartingIndex > -1)
					{
						for (int i = newItems.Count - 1; i >= 0; i--)
						{
							subject.Insert(newStartingIndex, convertMethod(newItems[i], NotifyCollectionChangedAction.Add));
						}
					}
					else
					{
						foreach (object item in newItems)
						{
							subject.Add(convertMethod(item, NotifyCollectionChangedAction.Add));
						}
					}
					break;

				case NotifyCollectionChangedAction.Move:
					subject.MoveItem(oldStartingIndex, newStartingIndex);
					break;

				case NotifyCollectionChangedAction.Remove:
					foreach (object o in oldItems)
					{
						subject.Remove(convertMethod(o, NotifyCollectionChangedAction.Remove));
					}
					break;

				case NotifyCollectionChangedAction.Replace:
					foreach (object o in oldItems)
					{
						subject.Remove(convertMethod(o, NotifyCollectionChangedAction.Remove));
					}

					for (int i = 0; i < newItems.Count; i++)
					{
						subject.Insert(oldStartingIndex + i, convertMethod(newItems[i], NotifyCollectionChangedAction.Add));
					}
					break;

				case NotifyCollectionChangedAction.Reset:
					if (sender != null)
					{
						subject.Clear();

						foreach (object o in (IList)sender)
						{
							subject.Add(convertMethod(o, NotifyCollectionChangedAction.Add));
						}
					}
					break;
			}
		}

		/// <summary>
		/// Moves a list item from one index to another, putting every item that is
		/// overstepped one index down.
		/// </summary>
		/// <param name="subject">base list</param>
		/// <param name="oldIndex">old Index</param>
		/// <param name="newIndex">new Index</param>
		public static void MoveItem(this IList subject, int oldIndex, int newIndex)
		{
			if (subject == null)
			{
				throw new ArgumentNullException("subject");
			}

			if (oldIndex != newIndex &&
				oldIndex >= 0 && oldIndex < subject.Count &&
				newIndex >= 0 && newIndex < subject.Count)
			{
				var temp = subject[oldIndex];

				int direcion;
				if (oldIndex < newIndex)
				{
					direcion = 1;
				}
				else
				{
					direcion = -1;
				}

				while (oldIndex != newIndex)
				{
					subject[oldIndex] = subject[oldIndex += direcion];
				}

				subject[newIndex] = temp;
			}
		}

		public static void QuickSort<T>(this IList<T> subject)
			where T : IComparable<T>
		{
			if (subject == null)
			{
				throw new ArgumentNullException("subject");
			}

			QuickSort(subject, 0, subject.Count - 1, Comparer<T>.Default.Compare);
		}

		public static void QuickSort<T>(this IList<T> subject, IComparer<T> comparer)
			where T : IComparable<T>
		{
			if (subject == null)
			{
				throw new ArgumentNullException("subject");
			}
			if (comparer == null)
			{
				throw new ArgumentNullException("comparer");
			}

			QuickSort(subject, 0, subject.Count - 1, comparer.Compare);
		}

		public static void QuickSort<T>(this IList<T> subject, Func<T, T, int> compareMethod)
		{
			if (subject == null)
			{
				throw new ArgumentNullException("subject");
			}
			if (compareMethod == null)
			{
				throw new ArgumentNullException("compareMethod");
			}

			QuickSort(subject, 0, subject.Count - 1, compareMethod);
		}

		/// <summary>
		/// Removes the range from a list.
		/// </summary>
		/// <param name="subject">the base list</param>
		/// <param name="listToRemove">The list to be remove.</param>
		/// <exception cref="ArgumentNullException"/>
		public static void RemoveRange(this IList subject, IEnumerable listToRemove)
		{
			if (subject == null)
			{
				throw new ArgumentNullException("subject");
			}
			if (listToRemove == null)
			{
				throw new ArgumentNullException("listToRemove");
			}

			foreach (object item in listToRemove)
			{
				subject.Remove(item);
			}
		}

		/// <summary>
		/// Removes each item in this list where the given condition delegate returns true.
		/// </summary>
		/// <param name="subject">The list.</param>
		/// <param name="condition">The condition that is supposed to be fulfilled for any item that is supposed to be removed.</param>
		/// <returns>A boolean value specifying whether anything was removed.</returns>
		public static bool RemoveWhere<T>(this IList<T> subject, Func<T, bool> condition)
		{
			return RemoveWhere<T>(subject, condition, false);
		}

		public static bool RemoveWhere<T>(this IList<T> subject, Func<T, bool> condition, bool removeOnlyFirst)
		{
			if (subject == null)
			{
				throw new ArgumentNullException("subject");
			}

			bool removed = false;
			int index = subject.IndexOfWhere(condition);
			if (index >= 0)
			{
				subject.RemoveAt(index);
				removed = true;
			}

			if (!removeOnlyFirst)
			{
				while (index < subject.Count && (index = subject.IndexOfWhere(condition, index)) >= 0)
				{
					subject.RemoveAt(index);
					removed = true;
				}
			}

			return removed;
		}

		public static void SortIn<T>(this IList<T> subject, T item)
			where T : IComparable<T>
		{
			SortIn(subject, item, Comparer<T>.Default.Compare);
		}

		public static void SortIn<T>(this IList<T> subject, T item, IComparer<T> comparer)
			where T : IComparable<T>
		{
			if (comparer == null)
			{
				throw new ArgumentNullException("comparer");
			}

			SortIn(subject, item, comparer.Compare);
		}

		public static void SortIn<T>(this IList<T> subject, T item, Func<T, T, int> compareMethod)
		{
			if (subject == null)
			{
				throw new ArgumentNullException("subject");
			}
			if (compareMethod == null)
			{
				throw new ArgumentNullException("compareMethod");
			}

			int i = 0;
			while (i < subject.Count && compareMethod(item, subject[i]) > 0)
			{
				i++;
			}

			subject.Insert(i, item);
		}

		/// <summary>
		/// Swaps two items at the given indeces in this list.
		/// </summary>
		/// <param name="subject">base list</param>
		/// <param name="index1">first index</param>
		/// <param name="index2">second index</param>
		public static void SwapItems(this IList subject, int index1, int index2)
		{
			if (subject == null)
			{
				throw new ArgumentNullException("subject");
			}

			object temp = subject[index1];
			subject[index1] = subject[index2];
			subject[index2] = temp;
		}

		#endregion extension methods

		#region static methods

		private static void QuickSort<T>(IList<T> subject, int left, int right, Func<T, T, int> compareMethod)
		{
			if (left < right)
			{
				int part = QuickSortPart(subject, left, right, compareMethod);
				QuickSort(subject, left, part - 1, compareMethod);
				QuickSort(subject, part + 1, right, compareMethod);
			}
		}

		private static int QuickSortPart<T>(IList<T> subject, int left, int right, Func<T, T, int> compareMethod)
		{
			int i = left;
			int j = right;
			T pivot = subject[right];
			T w;

			do
			{
				while (i < right && compareMethod(subject[i], pivot) <= 0)
				{
					i++;
				}
				while (j > left && compareMethod(subject[j], pivot) >= 0)
				{
					j--;
				}
				if (i < j)
				{
					w = subject[i];
					subject[i] = subject[j];
					subject[j] = w;
				}
			}
			while (i < j);

			if (compareMethod(subject[i], pivot) > 0)
			{
				w = subject[i];
				subject[i] = subject[j];
				subject[j] = w;
			}

			return i;
		}

		#endregion static methods
	}
}