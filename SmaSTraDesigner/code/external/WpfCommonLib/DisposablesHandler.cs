namespace Common
{
	using System;
	using System.Collections.Generic;
	using System.Diagnostics;
	using System.Linq;
	using System.Reflection;
	using System.Windows;

	using Common.ExtensionMethods;

	/// <summary>
	/// Provides functions to automatically dispose of an IDisposable object,
	/// when a specified FrameworkElement or FrameworkContentElement is unloaded.
	/// This is a Singleton class.
	/// The instance is automatically disposed of, when all connections are removed/cleared.
	/// </summary>
	public class DisposablesHandler : IDisposable, ISingleton
	{
		#region static properties

		/// <summary>
		/// Gets the only instance of the DisposablesHandler class (creates it if necessary).
		/// </summary>
		public static DisposablesHandler Instance
		{
			get { return Singleton<DisposablesHandler>.Instance; }
		}

		#endregion static properties

		#region fields

		/// <summary>
		/// Stores all connections from the objects who's Unloaded event to react to, to the IDisposable objects
		/// to dispose of, once the key object is unloaded.
		/// </summary>
		private Dictionary<ControllingObjectWrap, HashSet<IDisposable>> disposeConnections = new Dictionary<ControllingObjectWrap, HashSet<IDisposable>>();
		private object monitor = new object();

		/// <summary>
		/// Stores the connections in reverse order.
		/// (All the objects a disposable connection monitors for each corresponding disposable object instead of vice-versa)
		/// </summary>
		private Dictionary<IDisposable, HashSet<ControllingObjectWrap>> reverseDisposeConnections = new Dictionary<IDisposable, HashSet<ControllingObjectWrap>>();

		// TODO: (PS) Comment this.
		private Dictionary<object, ControllingObjectWrap> wrapDictionary = new Dictionary<object, ControllingObjectWrap>();

		#endregion fields

		#region constructors

		/// <summary>
		/// Initializes a new instance of the <see cref="DisposablesHandler"/> class.
		/// </summary>
		private DisposablesHandler()
		{
		}

		~DisposablesHandler()
		{
			this.Dispose();
		}

		#endregion constructors

		#region properties

		/// <summary>
		/// Gets or sets a value indicating whether this <see cref="DisposablesHandler"/> is disposed.
		/// </summary>
		/// <value><c>true</c> if disposed; otherwise, <c>false</c>.</value>
		public bool IsDisposed
		{
			get;
			protected set;
		}

		public bool IsInUse
		{
			get { return !(this.IsDisposed || this.disposeConnections.Count == 0); }
		}

		// TODO: (PS) Comment this.
		public Type[] SupportedTypes
		{
			get { return DefaultControllingObjectWrap.SupportedTypes; }
		}

		#endregion properties

		#region overrideable methods

		protected virtual void OnDispose()
		{
			foreach (var wrap in this.disposeConnections.Keys)
			{
				wrap.RemoveEventHandler();
			}

			this.disposeConnections = null;
			this.reverseDisposeConnections = null;
			this.wrapDictionary = null;
		}

		#endregion overrideable methods

		#region methods

		public bool AddDisposeConnection(ControllingObjectWrap wrap, IDisposable disposable)
		{
			lock (this.monitor)
			{
				if (this.IsDisposed)
				{
					throw new InvalidOperationException("DisposablesHandler instance was allready disposed of.");
				}
				if (wrap == null)
				{
					throw new ArgumentNullException("wrap");
				}
				if (disposable == null)
				{
					throw new ArgumentNullException("disposable");
				}

				bool result = this.AddEventHandler(wrap);
				if (result)
				{
					if (!this.wrapDictionary.ContainsKey(wrap.Subject))
					{
						this.wrapDictionary.Add(wrap.Subject, wrap);
					}

					this.disposeConnections.AddForwardAndReverse(wrap, disposable, this.reverseDisposeConnections);
				}

				return result;
			}
		}

		public bool AddDisposeConnection(object objectToDisposeAlongWith, IDisposable disposable, bool throwOnWrongType = true)
		{
			lock (this.monitor)
			{
				if (this.IsDisposed)
				{
					throw new InvalidOperationException("DisposablesHandler instance was allready disposed of.");
				}
				if (objectToDisposeAlongWith == null)
				{
					throw new ArgumentNullException("objectToDisposeAlongWith");
				}
				if (disposable == null)
				{
					throw new ArgumentNullException("disposable");
				}

				bool result = AddDisposeConnection(new DefaultControllingObjectWrap(objectToDisposeAlongWith), disposable);
				if (!result && throwOnWrongType)
				{
					throw new ArgumentException("The type of the given object to dispose along with is not among the supported types:\n" +
						String.Join(Environment.NewLine, DefaultControllingObjectWrap.SupportedTypes.Select(type => type.Name)),
						"objectToDisposeAlongWith");
				}

				return result;
			}
		}

		/// <summary>
		/// Performs application-defined tasks associated with freeing, releasing, or resetting unmanaged resources.
		/// </summary>
		public void Dispose()
		{
			lock (this.monitor)
			{
				if (!this.IsDisposed)
				{
					this.IsDisposed = true;

					this.OnDispose();
				}
			}
		}

		public bool ExecuteDisposeConnection(ControllingObjectWrap wrap)
		{
			lock (this.monitor)
			{
				if (this.IsDisposed)
				{
					throw new InvalidOperationException("DisposablesHandler instance was allready disposed of.");
				}

				return this.RemoveAndExecuteConnection(wrap, true);
			}
		}

		/// <summary>
		/// Executes a dispose connection.
		/// All IDisposable objects that belong to the given object are disposed of.
		/// </summary>
		/// <param name="objectToDisposeAlongWith">The object to dispose along with.</param>
		/// <returns>A value indicating whether the execution was successful.</returns>
		public bool ExecuteDisposeConnection(object objectToDisposeAlongWith)
		{
			return this.ExecuteDisposeConnection(this.wrapDictionary[objectToDisposeAlongWith]);
		}

		/// <summary>
		/// Gets the dispose connections as array.
		/// Key		is the object who's Unloaded event is monitored and
		/// Value	is the list of disposable objects that are supposed to be disposed of
		///			when that event occurs.
		/// </summary>
		public KeyValuePair<ControllingObjectWrap, IDisposable[]>[] GetDisposeConnections()
		{
			lock (this.monitor)
			{
				if (this.IsDisposed)
				{
					throw new InvalidOperationException("DisposablesHandler instance was allready disposed of.");
				}

				return this.disposeConnections.Select(kvp => new KeyValuePair<ControllingObjectWrap, IDisposable[]>(kvp.Key, kvp.Value.ToArray())).ToArray();
			}
		}

		/// <summary>
		/// Removes all dispose connections corresponding to the given object.
		/// </summary>
		/// <param name="objectToDisposeAlongWith">The object to dispose along with.</param>
		/// <returns>A value indicating whether the removal was successful.</returns>
		public bool RemoveAllDisposeConnectionsOf(object objectToDisposeAlongWith)
		{
			return this.RemoveAllDisposeConnectionsOf(this.wrapDictionary[objectToDisposeAlongWith]);
		}

		public bool RemoveAllDisposeConnectionsOf(ControllingObjectWrap wrap)
		{
			lock (this.monitor)
			{
				if (this.IsDisposed)
				{
					throw new InvalidOperationException("DisposablesHandler instance was allready disposed of.");
				}

				return this.RemoveAndExecuteConnection(wrap, false);
			}
		}

		/// <summary>
		/// Removes all dispose connections corresponding to the given disposable object.
		/// </summary>
		/// <param name="disposable">The disposable.</param>
		/// <returns>A value indicating whether the removal was successful.</returns>
		public bool RemoveAllDisposeConnectionsTo(IDisposable disposable)
		{
			lock (this.monitor)
			{
				if (this.IsDisposed)
				{
					throw new InvalidOperationException("DisposablesHandler instance was allready disposed of.");
				}
				if (disposable == null)
				{
					throw new ArgumentNullException("disposable");
				}

				bool result = this.reverseDisposeConnections.ContainsKey(disposable);
				if (result)
				{
					var objects = this.reverseDisposeConnections[disposable];
					foreach (var obj in objects)
					{
						this.RemoveItemAndListIfEmpty(this.disposeConnections, obj, disposable);
					}

					this.reverseDisposeConnections.Remove(disposable);

					//  If the dispose connections dictionary is empty now, dispose of this instance.
					Singleton<DisposablesHandler>.TryToDisposeOfInstance();
				}

				return result;
			}
		}

		/// <summary>
		/// Removes a specific dispose connection.
		/// </summary>
		/// <param name="objectToDisposeAlongWith">The object to dispose along with.</param>
		/// <param name="disposable">The disposable.</param>
		/// <returns>A value indicating whether the removal was successful.</returns>		
		public bool RemoveDisposeConnection(object objectToDisposeAlongWith, IDisposable disposable)
		{
			return RemoveDisposeConnection(this.wrapDictionary[objectToDisposeAlongWith], disposable);
		}

		public bool RemoveDisposeConnection(ControllingObjectWrap wrap, IDisposable disposable)
		{
			lock (this.monitor)
			{
				if (this.IsDisposed)
				{
					throw new InvalidOperationException("DisposablesHandler instance was allready disposed of.");
				}
				if (this.disposeConnections.Count == 0)
				{
					throw new InvalidOperationException("DisposablesHandler contains no dispose connections. Use AddDisposeConnection(...) methods.");
				}
				if (wrap == null)
				{
					throw new ArgumentNullException("wrap");
				}
				if (disposable == null)
				{
					throw new ArgumentNullException("disposable");
				}

				HashSet<IDisposable> disposables;
				if (this.disposeConnections.TryGetValue(wrap, out disposables) &&
					disposables.Remove(disposable))
				{
					bool removeReverseConnection = true;
					if (disposables.Count == 0)
					{
						this.disposeConnections.Remove(wrap);
						wrap.RemoveEventHandler();

						removeReverseConnection = !Singleton<DisposablesHandler>.TryToDisposeOfInstance();
					}

					if (removeReverseConnection)
					{
						this.RemoveItemAndListIfEmpty(this.reverseDisposeConnections, disposable, wrap);
					}

					return true;
				}

				return false;
			}
		}

		private bool AddEventHandler(ControllingObjectWrap wrap)
		{
			return this.disposeConnections.ContainsKey(wrap) || wrap.AddEventHandler();
		}

		private bool RemoveAndExecuteConnection(ControllingObjectWrap wrap, bool execute)
		{
			if (wrap == null)
			{
				throw new ArgumentNullException("wrap");
			}
			if (this.IsDisposed)
			{
				throw new InvalidOperationException("DisposablesHandler instance was allready disposed of.");
			}
			if (this.disposeConnections.Count == 0)
			{
				throw new InvalidOperationException("DisposablesHandler contains no dispose connections. Use AddDisposeConnection(...) methods.");
			}

			HashSet<IDisposable> disposables;
			bool result = this.disposeConnections.TryGetValue(wrap, out disposables);
			if (result)
			{
				foreach (IDisposable disposable in disposables)
				{
					// Remove all other connections to the item.
					foreach (var otherWrap in this.reverseDisposeConnections[disposable])
					{
						if (!otherWrap.Equals(wrap))
						{
							this.RemoveItemAndListIfEmpty(this.disposeConnections, otherWrap, disposable);
						}
					}

					this.reverseDisposeConnections.Remove(disposable);

					if (execute)
					{
						Debug.WriteLine("Disposing of \"{0}\" because \"{1}\" unloaded.", disposable, wrap.Subject);

						try
						{
							disposable.Dispose();
						}
						catch (Exception ex)
						{
							Debug.WriteLine("Disposing of \"{0}\" caused an Exception:\n{1}", disposable, ex);
						}
					}
				}

				this.disposeConnections.Remove(wrap);
				wrap.RemoveEventHandler();

				//  If the dispose connections dictionary is empty now, dispose of this instance.
				Singleton<DisposablesHandler>.TryToDisposeOfInstance();
			}

			return result;
		}

		/// <summary>
		/// Removes the given item from the list in the dictionary under the given key and
		/// also the list itsself, if it is empty afterwards.
		/// </summary>
		/// <typeparam name="TKey">The type of the key ('object' for forward connections and 'IDisposable' for reverse).</typeparam>
		/// <typeparam name="TItem">The type of the item ('IDisposable' for forward connections and 'object' for reverse).</typeparam>
		/// <param name="dictionary">The dictionary to add the connection to (either disposeConnections or reverseDisposeConnections).</param>
		/// <param name="key">The key (the object to dispose along with or the disposable (in case of reverse connection)).</param>
		/// <param name="item">The item (the disposable or the object to dispose along with (in case of reverse connection)).</param>
		private void RemoveItemAndListIfEmpty<TKey, TItem>(Dictionary<TKey, HashSet<TItem>> dictionary, TKey key, TItem item)
		{
			HashSet<TItem> items = dictionary[key];
			items.Remove(item);
			if (items.Count == 0)
			{
				dictionary.Remove(key);

				// If this method is used to remove a forward connection...
				ControllingObjectWrap wrap = key as ControllingObjectWrap;
				if (wrap != null)
				{
					// Remove all event handlers.
					wrap.RemoveEventHandler();
				}
			}
		}

		#endregion methods

		#region nested types

		// TODO: (PS) Comment this.
		public abstract class ControllingObjectWrap
		{
			#region constructors

			public ControllingObjectWrap(object subject)
			{
				if (subject == null)
				{
					throw new ArgumentNullException("subject");
				}

				this.Subject = subject;
			}

			#endregion constructors

			#region properties

			public object Subject
			{
				get;
				private set;
			}

			#endregion properties

			#region overrideable methods

			public abstract bool AddEventHandler();

			public abstract void RemoveEventHandler();

			public override bool Equals(object obj)
			{
				if (object.ReferenceEquals(this, obj))
				{
					return true;
				}

				ControllingObjectWrap other = obj as ControllingObjectWrap;

				return other != null && object.Equals(this.Subject, other.Subject);
			}

			public override int GetHashCode()
			{
				return this.Subject.GetHashCode();
			}

			#endregion overrideable methods

			#region methods

			protected void ExecuteDisposeConnection()
			{
				DisposablesHandler.Instance.ExecuteDisposeConnection(this);
			}

			#endregion methods
		}

		// TODO: (PS) Comment this.
		public class GenericControllingObjectWrap : ControllingObjectWrap
		{
			#region constructors

			public GenericControllingObjectWrap(object subject, EventInfo eventInfo)
				: base(subject)
			{
				if (eventInfo == null)
				{
					throw new ArgumentNullException("eventInfo");
				}

				this.EventInfo = eventInfo;
			}

			#endregion constructors

			#region properties

			public EventInfo EventInfo
			{
				get;
				private set;
			}

			#endregion properties

			#region overrideable methods

			public override bool AddEventHandler()
			{
				GenericAction.AddGenericEventHandler(this.Subject, this.EventInfo, this.OnEvent);

				return true;
			}

			public override void RemoveEventHandler()
			{
				GenericAction.RemoveGenericEventHandler(this.Subject, this.EventInfo, this.OnEvent);
			}

			#endregion overrideable methods

			#region methods

			private void OnEvent(object[] args)
			{
				this.ExecuteDisposeConnection();
			}

			#endregion methods
		}

		// TODO: (PS) Comment this.
		private class DefaultControllingObjectWrap : ControllingObjectWrap
		{
			#region static fields

			private static SupportedTypeInfo[] supportedTypes;

			#endregion static fields

			#region static constructor

			static DefaultControllingObjectWrap()
			{
				supportedTypes = new SupportedTypeInfo[]
				{
					new SupportedTypeInfo(typeof(FrameworkElement),
						(wrap) =>
							{
								DefaultControllingObjectWrap defaultWrap = (DefaultControllingObjectWrap)wrap;
								FrameworkElement fe = (FrameworkElement)wrap.Subject;

								fe.Unloaded += defaultWrap.FrameworkElement_Unloaded;
								fe.Dispatcher.ShutdownStarted += defaultWrap.Dispatcher_ShutdownStarted;
							},
						(wrap) =>
							{
								DefaultControllingObjectWrap defaultWrap = (DefaultControllingObjectWrap)wrap;
								FrameworkElement fe = (FrameworkElement)wrap.Subject;

								fe.Unloaded -= defaultWrap.FrameworkElement_Unloaded;
								fe.Dispatcher.ShutdownStarted -= defaultWrap.Dispatcher_ShutdownStarted;
							}),
					new SupportedTypeInfo(typeof(FrameworkContentElement),
						(wrap) =>
							{
								DefaultControllingObjectWrap defaultWrap = (DefaultControllingObjectWrap)wrap;
								FrameworkContentElement fe = (FrameworkContentElement)wrap.Subject;

								fe.Unloaded += defaultWrap.FrameworkElement_Unloaded;
								fe.Dispatcher.ShutdownStarted += defaultWrap.Dispatcher_ShutdownStarted;
							},
						(wrap) =>
							{
								DefaultControllingObjectWrap defaultWrap = (DefaultControllingObjectWrap)wrap;
								FrameworkContentElement fe = (FrameworkContentElement)wrap.Subject;

								fe.Unloaded -= defaultWrap.FrameworkElement_Unloaded;
								fe.Dispatcher.ShutdownStarted -= defaultWrap.Dispatcher_ShutdownStarted;
							})
				};
			}

			#endregion static constructor

			#region static properties

			public static Type[] SupportedTypes
			{
				get { return supportedTypes.Select(st => st.Type).ToArray(); }
			}

			#endregion static properties

			#region constructors

			public DefaultControllingObjectWrap(object subject)
				: base(subject)
			{
			}

			#endregion constructors

			#region overrideable methods

			public override bool AddEventHandler()
			{
				Type subjectType = this.Subject.GetType();
				foreach (var typeInfo in supportedTypes)
				{
					if (typeInfo.Type.IsAssignableFrom(subjectType))
					{
						typeInfo.AddHandlerMethod(this);

						return true;
					}
				}

				return false;
			}

			public override void RemoveEventHandler()
			{
				Type subjectType = this.Subject.GetType();
				foreach (var typeInfo in supportedTypes)
				{
					if (typeInfo.Type.IsAssignableFrom(subjectType))
					{
						typeInfo.RemoveHandlerMethod(this);

						return;
					}
				}
			}

			#endregion overrideable methods

			#region event handlers

			public void Dispatcher_ShutdownStarted(object sender, System.EventArgs e)
			{
				this.ExecuteDisposeConnection();
			}

			public void FrameworkElement_Unloaded(object sender, RoutedEventArgs e)
			{
				if (e.OriginalSource == sender)
				{
					this.ExecuteDisposeConnection();
				}
			}

			#endregion event handlers
		}

		// TODO: (PS) Comment this.
		private class SupportedTypeInfo
		{
			#region fields

			public Action<ControllingObjectWrap> AddHandlerMethod;
			public Action<ControllingObjectWrap> RemoveHandlerMethod;
			public Type Type;

			#endregion fields

			#region constructors

			public SupportedTypeInfo(Type type, Action<ControllingObjectWrap> addHandlerMethod, Action<ControllingObjectWrap> removeHandlerMethod)
			{
				this.Type = type;
				this.AddHandlerMethod = addHandlerMethod;
				this.RemoveHandlerMethod = removeHandlerMethod;
			}

			#endregion constructors
		}

		#endregion nested types
	}
}