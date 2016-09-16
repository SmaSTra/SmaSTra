using System;
using System.Collections.Generic;

namespace Common.ExtensionMethods
{
    // TODO: (PS) Comment this.
    public static class ICollectionExtensions
	{
		/// <summary>
		/// Adds all items of the given list to this one.
		/// </summary>
		/// <typeparam name="T">Type placeholder</typeparam>
		/// <param name="subject">the base list</param>
		/// <param name="listToAdd">list to be added</param>
		/// <exception cref="ArgumentNullException"/>
		public static void AddRange<T>(this ICollection<T> subject, params T[] listToAdd)
		{
			AddRange(subject, (IEnumerable<T>)listToAdd);
		}

		/// <summary>
		/// Adds all items of the given list to this one.
		/// </summary>
		/// <typeparam name="T">Type placeholder</typeparam>
		/// <param name="subject">the base list</param>
		/// <param name="listToAdd">list to be added</param>
		/// <exception cref="ArgumentNullException"/>
		public static void AddRange<T>(this ICollection<T> subject, IEnumerable<T> listToAdd)
		{
			if (subject == null)
			{
				throw new ArgumentNullException("subject");
			}
			if (listToAdd == null)
			{
				throw new ArgumentNullException("listToAdd");
			}

			foreach (T item in listToAdd)
			{
				subject.Add(item);
			}
		}

		/// <summary>
		/// Removes the range from a list.
		/// </summary>
		/// <typeparam name="T">Type placeholder</typeparam>
		/// <param name="subject">the base list</param>
		/// <param name="listToRemove">The list to be remove.</param>
		/// <exception cref="ArgumentNullException"/>
		public static void RemoveRange<T>(this ICollection<T> subject, IEnumerable<T> listToRemove)
		{
			if (subject == null)
			{
				throw new ArgumentNullException("subject");
			}
			if (listToRemove == null)
			{
				throw new ArgumentNullException("listToRemove");
			}

			foreach (T item in listToRemove)
			{
				subject.Remove(item);
			}
		}
	}
}
