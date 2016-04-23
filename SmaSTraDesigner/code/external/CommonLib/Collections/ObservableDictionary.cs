namespace Common.Collections
{
	using System;
	using System.Collections;
	using System.Collections.Generic;
	using System.Collections.Specialized;
	using System.ComponentModel;
	using System.Linq;

	// TODO: (PS) Comment this.
	public class ObservableDictionary<TKey, TValue> : IDictionary<TKey, TValue>, IDictionary, INotifyCollectionChanged, INotifyPropertyChanged
	{
		#region fields

		private IDictionary<TKey, TValue> innerDictionary;

		#endregion fields

		#region constructors

		public ObservableDictionary()
		{
			this.innerDictionary = new Dictionary<TKey, TValue>();
		}

		public ObservableDictionary(IDictionary<TKey, TValue> innerDictionary)
		{
			if (innerDictionary == null)
			{
				throw new ArgumentNullException("innerDictionary");
			}

			this.innerDictionary = innerDictionary;
		}

		#endregion constructors

		#region events

		public event NotifyCollectionChangedEventHandler CollectionChanged;

		public event PropertyChangedEventHandler PropertyChanged;

		#endregion events

		#region overrideable properties

		public virtual TValue this[TKey key]
		{
			get
			{
				return this.innerDictionary[key];
			}
			set
			{
				TValue oldValue = this.innerDictionary[key];
				this.innerDictionary[key] = value;
				this.OnCollectionChanged(new NotifyCollectionChangedEventArgs(NotifyCollectionChangedAction.Replace,
					new KeyValuePair<TKey, TValue>(key, value), new KeyValuePair<TKey, TValue>(key, oldValue)));
			}
		}

		#endregion overrideable properties

		#region properties

		public int Count
		{
			get { return this.innerDictionary.Count; }
		}

		public bool IsFixedSize
		{
			get
			{
				IDictionary dictionary = this.innerDictionary as IDictionary;
				if (dictionary != null)
				{
					return dictionary.IsFixedSize;
				}

				throw new InvalidOperationException("Inner dictionary is not a IDictionary.");
			}
		}

		public bool IsReadOnly
		{
			get { return this.innerDictionary.IsReadOnly; }
		}

		public bool IsSynchronized
		{
			get
			{
				IDictionary dictionary = this.innerDictionary as IDictionary;
				if (dictionary != null)
				{
					return dictionary.IsSynchronized;
				}

				throw new InvalidOperationException("Inner dictionary is not a IDictionary.");
			}
		}

		public ICollection<TKey> Keys
		{
			get { return this.innerDictionary.Keys; }
		}

		public object SyncRoot
		{
			get
			{
				IDictionary dictionary = this.innerDictionary as IDictionary;
				if (dictionary != null)
				{
					return dictionary.SyncRoot;
				}

				throw new InvalidOperationException("Inner dictionary is not a IDictionary.");
			}
		}

		public ICollection<TValue> Values
		{
			get { return this.innerDictionary.Values; }
		}

		ICollection IDictionary.Keys
		{
			get
			{
				IDictionary dictionary = this.innerDictionary as IDictionary;
				if (dictionary != null)
				{
					return dictionary.Keys;
				}

				throw new InvalidOperationException("Inner dictionary is not a IDictionary.");
			}
		}

		object IDictionary.this[object key]
		{
			get
			{
				return this[(TKey)key];
			}
			set
			{
				this[(TKey)key] = (TValue)value;
			}
		}

		ICollection IDictionary.Values
		{
			get
			{
				IDictionary dictionary = this.innerDictionary as IDictionary;
				if (dictionary != null)
				{
					return dictionary.Values;
				}

				throw new InvalidOperationException("Inner dictionary is not a IDictionary.");
			}
		}

		#endregion properties

		#region overrideable methods

		public virtual void Add(TKey key, TValue value)
		{
			var item = new KeyValuePair<TKey, TValue>(key, value);
			this.innerDictionary.Add(key, value);
			this.OnCollectionChanged(new NotifyCollectionChangedEventArgs(NotifyCollectionChangedAction.Add, item));
			this.OnPropertyChanged("Count");
		}

		public virtual void Clear()
		{
			if (this.innerDictionary.Count != 0)
			{
				var removedItems = this.innerDictionary.ToList();
				this.innerDictionary.Clear();
				this.OnCollectionChanged(new NotifyCollectionChangedEventArgs(NotifyCollectionChangedAction.Reset, removedItems));
				this.OnPropertyChanged("Count");
			}
		}

		public virtual bool Remove(TKey key)
		{
			TValue value;
			bool result = this.innerDictionary.TryGetValue(key, out value);
			if (result)
			{
				KeyValuePair<TKey, TValue> item = new KeyValuePair<TKey, TValue>(key, value);
				result &= this.innerDictionary.Remove(key);
				if (result)
				{
					this.OnCollectionChanged(new NotifyCollectionChangedEventArgs(NotifyCollectionChangedAction.Remove, item));
					this.OnPropertyChanged("Count");
				}
			}

			return result;
		}

		protected virtual void OnCollectionChanged(NotifyCollectionChangedEventArgs e)
		{
			if (this.CollectionChanged != null)
			{
				this.CollectionChanged(this, e);
			}
		}

		#endregion overrideable methods

		#region methods

		public bool ContainsKey(TKey key)
		{
			return this.innerDictionary.ContainsKey(key);
		}

		public void CopyTo(KeyValuePair<TKey, TValue>[] array, int arrayIndex)
		{
			this.innerDictionary.CopyTo(array, arrayIndex);
		}

		public IEnumerator<KeyValuePair<TKey, TValue>> GetEnumerator()
		{
			return this.innerDictionary.GetEnumerator();
		}

		public bool TryGetValue(TKey key, out TValue value)
		{
			return this.innerDictionary.TryGetValue(key, out value);
		}

		private void OnPropertyChanged(string propertyName)
		{
			if (this.PropertyChanged != null)
			{
				this.PropertyChanged(this, new PropertyChangedEventArgs(propertyName));
			}
		}

		void ICollection.CopyTo(Array array, int index)
		{
			IDictionary dictionary = this.innerDictionary as IDictionary;
			if (dictionary != null)
			{
				dictionary.CopyTo(array, index);
			}

			throw new InvalidOperationException("Inner dictionary is not a IDictionary.");
		}

		void ICollection<KeyValuePair<TKey, TValue>>.Add(KeyValuePair<TKey, TValue> item)
		{
			this.Add(item.Key, item.Value);
		}

		bool ICollection<KeyValuePair<TKey, TValue>>.Contains(KeyValuePair<TKey, TValue> item)
		{
			return this.innerDictionary.Contains(item);
		}

		bool ICollection<KeyValuePair<TKey, TValue>>.Remove(KeyValuePair<TKey, TValue> item)
		{
			return this.Remove(item.Key);
		}

		void IDictionary.Add(object key, object value)
		{
			this.Add((TKey)key, (TValue)value);
		}

		bool IDictionary.Contains(object key)
		{
			IDictionary dictionary = this.innerDictionary as IDictionary;
			if (dictionary != null)
			{
				return dictionary.Contains(key);
			}

			throw new InvalidOperationException("Inner dictionary is not a IDictionary.");
		}

		IDictionaryEnumerator IDictionary.GetEnumerator()
		{
			IDictionary dictionary = this.innerDictionary as IDictionary;
			if (dictionary != null)
			{
				return dictionary.GetEnumerator();
			}

			throw new InvalidOperationException("Inner dictionary is not a IDictionary.");
		}

		void IDictionary.Remove(object key)
		{
			this.Remove((TKey)key);
		}

		IEnumerator IEnumerable.GetEnumerator()
		{
			return ((IEnumerable)this.innerDictionary).GetEnumerator();
		}

		#endregion methods
	}
}