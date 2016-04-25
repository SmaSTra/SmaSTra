namespace Common.ExtensionMethods
{
	using System;
	using System.Collections.Generic;

	// TODO: (PS) Comment this.
	public static class IDictionaryExtensions
	{
		#region extension methods

		public static void Add<TKey, TValue, TCollection>(this IDictionary<TKey, TCollection> subject, TKey key, TValue value, bool checkRedundancy = false)
			where TCollection : ICollection<TValue>
		{
			if (subject == null)
			{
				throw new ArgumentNullException("subject");
			}

			TCollection collection;
			if (!subject.TryGetValue(key, out collection))
			{
				collection = Activator.CreateInstance<TCollection>();
				collection.Add(value);
				subject.Add(key, collection);
			}
			else if (!checkRedundancy || collection.Contains(value))
			{
				collection.Add(value);
			}
		}

		public static void AddForwardAndReverse<TKey, TValue, TCollection, TCollectionReverse>(this IDictionary<TKey, TCollection> subject, TKey key, TValue value,
			IDictionary<TValue, TCollectionReverse> reverseDictionary, bool checkRedundancy = false)
			where TCollection : ICollection<TValue>
			where TCollectionReverse : ICollection<TKey>
		{
			if (subject == null)
			{
				throw new ArgumentNullException("subject");
			}
			if (reverseDictionary == null)
			{
				throw new ArgumentNullException("reverseDictionary");
			}

			Add(subject, key, value, checkRedundancy);
			Add(reverseDictionary, value, key, checkRedundancy);
		}

		public static bool Remove<TKey, TValue, TCollection>(this IDictionary<TKey, TCollection> subject, TKey key, TValue value)
			where TCollection : ICollection<TValue>
		{
			if (subject == null)
			{
				throw new ArgumentNullException("subject");
			}

			TCollection collection;
			if (subject.TryGetValue(key, out collection))
			{
				bool result = collection.Remove(value);
				if (collection.Count == 0)
				{
					subject.Remove(key);
				}

				return result;
			}

			return false;
		}

		public static bool RemoveForwardAndReverse<TKey, TValue, TCollection, TCollectionReverse>(this IDictionary<TKey, TCollection> subject, TKey key, TValue value,
			IDictionary<TValue, TCollectionReverse> reverseDictionary)
			where TCollection : ICollection<TValue>
			where TCollectionReverse : ICollection<TKey>
		{
			if (subject == null)
			{
				throw new ArgumentNullException("subject");
			}
			if (reverseDictionary == null)
			{
				throw new ArgumentNullException("reverseDictionary");
			}

			return Remove(subject, key, value) &&
				Remove(reverseDictionary, value, key);
		}

		#endregion extension methods
	}
}