namespace Common
{
	using System;
	using System.Collections.Generic;
	using System.Linq;

	/// <summary>
	/// TODO: (PS) Comment this.
	/// </summary>
	public static class Singleton
	{
		#region static fields

		private static HashSet<Type> initializedSingletonTypes = new HashSet<Type>();

		#endregion static fields

		#region static methods

		public static void DisposeOfAllSingletons()
		{
			List<Type> types;
			lock (initializedSingletonTypes)
			{
				types = initializedSingletonTypes.ToList();
			}

			foreach (Type singletonType in types)
			{
				try
				{
					Type type = typeof(Singleton<>).MakeGenericType(singletonType);
					type.GetMethod("DisposeOfInstance").Invoke(null, null);
				}
				catch { }
			}
		}

		public static void TryToDisposeOfAllSingletons()
		{
			List<Type> types;
			lock (initializedSingletonTypes)
			{
				types = initializedSingletonTypes.ToList();
			}

			foreach (Type singletonType in types)
			{
				try
				{
					Type type = typeof(Singleton<>).MakeGenericType(singletonType);
					type.GetMethod("TryToDisposeOfInstance").Invoke(null, null);
				}
				catch { }
			}
		}

		internal static void AddType(Type singletonType)
		{
			lock (initializedSingletonTypes)
			{
				initializedSingletonTypes.Add(singletonType);
			}
		}

		internal static void RemoveType(Type singletonType)
		{
			lock (initializedSingletonTypes)
			{
				initializedSingletonTypes.Remove(singletonType);
			}
		}

		#endregion static methods
	}

	/// <summary>
	/// TODO: (PS) Comment this.
	/// </summary>
	public static class Singleton<T>
		where T : class
	{
		#region static fields

		// TODO: (PS) Comment this.
		private static Func<T, bool> getIsInUseCallback = null;

		/// <summary>
		/// The only instance of the Singleton class.
		/// </summary>
		private static T instance = null;

		// TODO: (PS) Comment this.
		private static object monitor = new object();

		// TODO: (PS) Comment this.
		private static bool useWeakInstanceReference = false;

		// TODO: (PS) Comment this.
		private static WeakReference weakInstanceReference = null;

		#endregion static fields

		#region static constructor

		static Singleton()
		{
			SingletonAttribute attribute = (SingletonAttribute)typeof(T).GetCustomAttributes(typeof(SingletonAttribute), true).FirstOrDefault();
			if (attribute != null)
			{
				UseWeakInstanceReference = attribute.UseWeakInstanceReference;
				CreateInstanceFunction = attribute.CreateInstanceFunction;
				GetIsInUseCallback = attribute.GetIsInUseCallback;
			}
		}

		#endregion static constructor

		#region static properties

		// TODO: (PS) Comment this.
		public static Func<object> CreateInstanceFunction
		{
			get;
			set;
		}

		// TODO: (PS) Comment this.
		public static Func<T, bool> GetIsInUseCallback
		{
			get
			{
				if (getIsInUseCallback == null)
				{
					getIsInUseCallback = GetIsInUse;
				}

				return getIsInUseCallback;
			}
			set
			{
				getIsInUseCallback = value ?? GetIsInUse;
			}
		}

		/// <summary>
		/// Gets the only instance of the Singleton class (creates it if necessary).
		/// </summary>
		public static T Instance
		{
			get
			{
				lock (monitor)
				{
					if (useWeakInstanceReference)
					{
						T localInstance;
						if (weakInstanceReference == null || (localInstance = (T)weakInstanceReference.Target) == null)
						{
							localInstance = CreateInstance();
							weakInstanceReference = new WeakReference(localInstance);
						}

						return localInstance;
					}
					else
					{
						if (instance == null)
						{
							instance = CreateInstance();
						}

						return instance;
					}
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
					if (UseWeakInstanceReference)
					{
						return weakInstanceReference != null && weakInstanceReference.IsAlive;
					}
					else
					{
						return instance != null;
					}
				}
			}
		}

		// TODO: (PS) Comment this.
		public static bool UseWeakInstanceReference
		{
			get
			{
				return useWeakInstanceReference;
			}
			set
			{
				if (value != useWeakInstanceReference)
				{
					lock (monitor)
					{
						useWeakInstanceReference = value;

						if (value)
						{
							if (instance != null)
							{
								weakInstanceReference = new WeakReference(instance);
								instance = null;
							}
						}
						else
						{
							instance = (T)weakInstanceReference.Target;
							weakInstanceReference = null;
						}
					}
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
				if (useWeakInstanceReference)
				{
					T instance;
					if (weakInstanceReference != null && (instance = (T)weakInstanceReference.Target) != null)
					{
						IDisposable disposable = instance as IDisposable;
						if (disposable != null)
						{
							disposable.Dispose();
						}

						weakInstanceReference = null;
					}
				}
				else
				{
					if (instance != null)
					{
						IDisposable disposable = instance as IDisposable;
						if (disposable != null)
						{
							disposable.Dispose();
						}

						instance = null;
					}
				}
			}

			Singleton.RemoveType(typeof(T));
		}

		public static void Initialize()
		{
			lock (monitor)
			{
				if (!useWeakInstanceReference)
				{
					if (instance == null)
					{
						instance = CreateInstance();
					}
				}
				else
				{
					throw new InvalidOperationException("Initialize() method does not work when singleton is created with weak reference.");
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
				T instance = Instance;
				if (instance != null)
				{
					bool result = !GetIsInUseCallback(instance);
					if (result)
					{
						DisposeOfInstance();
					}

					return result;
				}
			}

			return true;
		}

		// TODO: (PS) Comment this.
		private static T CreateInstance()
		{
			T result;
			if (CreateInstanceFunction != null)
			{
				result = (T)CreateInstanceFunction();
			}
			else
			{
				result = (T)Activator.CreateInstance(typeof(T), true);
			}

			Singleton.AddType(typeof(T));
			return result;
		}

		// TODO: (PS) Comment this.
		private static bool GetIsInUse(T instance)
		{
			ISingleton singleton = instance as ISingleton;

			return singleton == null || singleton.IsInUse;
		}

		#endregion static methods
	}
}