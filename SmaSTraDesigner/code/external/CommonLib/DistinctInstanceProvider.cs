namespace Common
{
	using System;
	using System.Collections.Generic;
	using System.Collections.ObjectModel;
	using System.Linq;
	using System.Threading;

	/// <summary>
	/// TODO: (PS) Comment this.
	/// This is a Singleton class.
	/// </summary>
	public class DistinctInstanceProvider<T> : IDisposable
		where T : class
	{
		#region static fields

		/// <summary>
		/// The only instance of the NonAmbiguesInstanceProvider class.
		/// </summary>
		private static DistinctInstanceProvider<T> instance = null;
		private static object monitor = new object();

		#endregion static fields

		#region static properties

		/// <summary>
		/// Gets the only instance of the NonAmbiguesInstanceProvider class (creates it if necessary).
		/// </summary>
		public static DistinctInstanceProvider<T> Instance
		{
			get
			{
				lock (monitor)
				{
					if (instance == null)
					{
						instance = new DistinctInstanceProvider<T>();
					}

					return instance;
				}
			}
		}

		/// <summary>
		/// Gets a value indicating whether this singleton is instantiated.
		/// If not, a new instance is created, as soon as the Instance property is
		/// accessed.
		/// </summary>
		public static bool IsInstantiated
		{
			get
			{
				lock (monitor)
				{
					return instance != null;
				}
			}
		}

		#endregion static properties

		#region static methods

		/// <summary>
		/// Disposes of the one instance of this Singleton class.
		/// </summary>
		public static void DisposeOfInstance()
		{
			lock (monitor)
			{
				if (instance != null)
				{
					instance.Dispose();
				}
			}
		}

		/// <summary>
		/// Disposes of the instance, if the instance does not contain any valuable data (any more).
		/// </summary>
		/// <returns>A value indicating whether the instance was disposed of. Is allways true if there was no instance to begin with.</returns>
		public static bool TryToDisposeOfInstance()
		{
			lock (monitor)
			{
				if (instance != null)
				{
					bool result = instance.distinctInstances.Count == 0;
					if (result)
					{
						instance.Dispose();
					}

					return result;
				}

				return true;
			}
		}

		#endregion static methods

		#region fields

		private Dictionary<HashableWeakReference, HashableWeakReference> distinctInstances = 
			new Dictionary<HashableWeakReference, HashableWeakReference>();

		#endregion fields

		#region constructors

		/// <summary>
		/// Initializes a new instance of the <see cref="DistinctInstanceProvider"/> class.
		/// </summary>
		private DistinctInstanceProvider()
		{
			#if !NO_AUTO_CLEAN_UP_OF_DISTINCT_INSTANCES
			this.AutoCleanup = true;
			#endif
		}

		#endregion constructors

		#region properties

		public ReadOnlyCollection<T> DistinctInstances
		{
			get
			{
				lock (monitor)
				{
					return new ReadOnlyCollection<T>(this.GetListOfInstances());
				}
			}
		}

		#endregion properties

		#region methods

		public void CleanUpUnusedReferences()
		{
			lock (monitor)
			{
				foreach (var reference in this.distinctInstances.Values.ToArray())
				{
					if (!reference.IsAlive)
					{
						this.distinctInstances.Remove(reference);
					}
				}

				TryToDisposeOfInstance();
			}
		}

		/// <summary>
		/// Performs application-defined tasks associated with freeing, releasing, or resetting unmanaged resources.
		/// </summary>
		public void Dispose()
		{
			lock (monitor)
			{
				if (instance != null)
				{
					instance = null;
				}

			#if !NO_AUTO_CLEAN_UP_OF_DISTINCT_INSTANCES
				if (this.autoCleanUpTimer != null)
				{
					this.autoCleanUpTimer.Dispose();
				}
			#endif
			}
		}

		public T GetDistinctInstance(T newInstance)
		{
			bool instanceReplaced;

			return GetDistinctInstance(newInstance, out instanceReplaced);
		}

		public T GetDistinctInstance(T newInstance, out bool instanceAlreadyExists)
		{
			lock (monitor)
			{
				if (newInstance == null)
				{
					throw new ArgumentNullException("newInstance");
				}

				HashableWeakReference weakRef = new HashableWeakReference(newInstance);
				if (this.distinctInstances.ContainsKey(weakRef))
				{
					HashableWeakReference existing = this.distinctInstances[weakRef];
					if (existing.IsAlive)
					{
						IDisposable disposable = newInstance as IDisposable;
						if (disposable != null)
						{
							disposable.Dispose();
						}

						instanceAlreadyExists = true;
						return (T)existing.Target;
					}
					else
					{
						this.distinctInstances.Remove(existing);
					}
				}

			#if !NO_AUTO_CLEAN_UP_OF_DISTINCT_INSTANCES
				if (this.AutoCleanup && this.autoCleanUpTimer == null)
				{
					this.autoCleanUpTimer = new Timer(new TimerCallback((state) => this.CleanUpUnusedReferences()), null, 0, AUTO_CLEANUP_INTERVAL);
				}
			#endif

				this.distinctInstances.Add(weakRef, weakRef);

				instanceAlreadyExists = false;
				return newInstance;
			}
		}

		public bool RemoveDistinctInstance(T instance)
		{
			lock (monitor)
			{
				bool result = this.distinctInstances.Remove(new HashableWeakReference(instance));
				if (result)
				{
					TryToDisposeOfInstance();
				}

				return result;
			}
		}

		private List<T> GetListOfInstances()
		{
			return this.distinctInstances.Values
						.Where(reference => reference.IsAlive)
						.Select(reference => (T)reference.Target).ToList();
		}

		#endregion methods

		#if !NO_AUTO_CLEAN_UP_OF_DISTINCT_INSTANCES

		// TODO: (PS) Comment this.
		public bool AutoCleanup
		{
			get;
			set;
		}

		private Timer autoCleanUpTimer = null;
		private const int AUTO_CLEANUP_INTERVAL = 30000;

		#endif
	}
}