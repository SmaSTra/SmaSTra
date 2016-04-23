namespace Common.Threading
{
	using System;
	using System.Collections.Generic;
	using System.Linq;
	using System.Threading;

	using Common.ExtensionMethods;

	// TODO: (PS) Comment this.
	public class ThreadQueue
	{
		#region constants

		public const int DEFAULT_MAX_EXECUTING_THREADS = 5;
		public const int DEFAULT_MIN_EXECUTING_THREADS = 0;

		#endregion constants

		#region static fields

		private static int instanceCounter = 0;
		private static HashSet<HashableWeakReference> instances = new HashSet<HashableWeakReference>();

		#endregion static fields

		#region static properties

		public static IEnumerable<ThreadQueue> Instances
		{
			get
			{
				return instances.Select(wr => (ThreadQueue)wr.Target).ToArray();
			}
		}

		#endregion static properties

		#region fields

		private HashSet<ThreadQueueItem> executingThreads = new HashSet<ThreadQueueItem>();

		/// <summary>
		/// TODO: (PS) Comment this.
		/// </summary>
		private int maxExecutingThreads = DEFAULT_MAX_EXECUTING_THREADS;

		/// <summary>
		/// TODO: (PS) Comment this.
		/// </summary>
		private int minExecutingThreads = DEFAULT_MIN_EXECUTING_THREADS;
		private int threadCounter = 0;
		private Mutex threadDoneMutex = new Mutex();

		/// <summary>
		/// TODO: (PS) Comment this.
		/// </summary>
		private ThreadPriority threadPriority = System.Threading.ThreadPriority.Normal;
		private List<ThreadQueueItem> waitingThreads = new List<ThreadQueueItem>();

		#endregion fields

		#region constructors

		public ThreadQueue()
		{
			instances.Add(new HashableWeakReference(this));
			this.Name = instanceCounter.ToString();
			instanceCounter++;
		}

		public ThreadQueue(int minExecutingThreads, int maxExecutingThreads)
			: this()
		{
			this.MinExecutingThreads = minExecutingThreads;
			this.MaxExecutingThreads = maxExecutingThreads;
		}

		~ThreadQueue()
		{
			instances.Remove(new HashableWeakReference(this));
		}

		#endregion constructors

		#region properties

		public IEnumerable<ThreadQueueItem> ExecutingThreads
		{
			get
			{
				lock (this.executingThreads)
				{
					return this.executingThreads.ToArray();
				}
			}
		}

		/// <summary>
		/// Gets or sets the MaxExecutingThreads property value.
		/// TODO: (PS) Comment this.
		/// </summary>
		public int MaxExecutingThreads
		{
			get
			{
				return this.maxExecutingThreads;
			}
			set
			{
				if (value < 1)
				{
					throw new ArgumentException("MaxExecutingThreads must be >= 1.", "value");
				}

				if (value != this.maxExecutingThreads)
				{
					int oldValue = this.maxExecutingThreads;
					this.maxExecutingThreads = value;
					this.OnMaxExecutingThreadsChanged(oldValue, value);
				}
			}
		}

		/// <summary>
		/// Gets or sets the MinExecutingThreads property value.
		/// TODO: (PS) Comment this.
		/// </summary>
		public int MinExecutingThreads
		{
			get
			{
				return this.minExecutingThreads;
			}
			set
			{
				if (value < 0)
				{
					throw new ArgumentException("MinExecutingThreads must be >= 0.", "value");
				}

				if (value != this.minExecutingThreads)
				{
					int oldValue = this.minExecutingThreads;
					this.minExecutingThreads = value;
					this.OnMinExecutingThreadsChanged(oldValue, value);
				}
			}
		}

		public string Name
		{
			get;
			set;
		}

		/// <summary>
		/// Gets or sets the ThreadPriority property value.
		/// TODO: (PS) Comment this.
		/// </summary>
		public ThreadPriority ThreadPriority
		{
			get
			{
				return this.threadPriority;
			}
			set
			{
				if (value != this.threadPriority)
				{
					ThreadPriority oldValue = this.threadPriority;
					this.threadPriority = value;
					this.OnThreadPriorityChanged(oldValue, value);
				}
			}
		}

		public IEnumerable<ThreadQueueItem> WaitingThreads
		{
			get
			{
				lock (this.waitingThreads)
				{
					return this.waitingThreads.ToArray();
				}
			}
		}

		#endregion properties

		#region overrideable methods

		/// <summary>
		/// Called when the MaxExecutingThreads property changed its value.
		/// </summary>
		/// <param name="oldValue">The old value.</param>
		/// <param name="newValue">The new value.</param>
		protected virtual void OnMaxExecutingThreadsChanged(int oldValue, int newValue)
		{
			this.ExecuteThreads();
		}

		/// <summary>
		/// Called when the MinExecutingThreads property changed its value.
		/// </summary>
		/// <param name="oldValue">The old value.</param>
		/// <param name="newValue">The new value.</param>
		protected virtual void OnMinExecutingThreadsChanged(int oldValue, int newValue)
		{
			if (newValue >= this.minExecutingThreads)
			{
				lock (this.executingThreads)
				{
					foreach (var thread in this.executingThreads)
					{
						thread.Execute();
					}
				}
			}
		}

		/// <summary>
		/// Called when the ThreadPriority property changed its value.
		/// </summary>
		/// <param name="oldValue">The old value.</param>
		/// <param name="newValue">The new value.</param>
		protected virtual void OnThreadPriorityChanged(ThreadPriority oldValue, ThreadPriority newValue)
		{
			lock (this.waitingThreads)
			{
				lock (this.executingThreads)
				{
					foreach (var thread in this.executingThreads)
					{
						thread.ThreadPriority = newValue;
					}

					foreach (var thread in this.waitingThreads)
					{
						thread.ThreadPriority = newValue;
					}
				}
			}
		}

		public override string ToString()
		{
			return String.Format("{0} {1}", typeof(ThreadQueue).Name, this.Name);
		}

		#endregion overrideable methods

		#region methods

		public ThreadQueueItem EnqueueThread(Action threadAction)
		{
			if (threadAction == null)
			{
				throw new ArgumentNullException("threadAction");
			}

			ThreadQueueItem newThread = new ThreadQueueItem(this, threadAction, this.threadCounter++);
			lock (this.waitingThreads)
			{
				this.waitingThreads.Add(newThread);
			}

			this.ExecuteThreads();

			return newThread;
		}

		public void MoveToFront(ThreadQueueItem thread)
		{
			if (thread == null)
			{
				throw new ArgumentNullException("thread");
			}

			lock (this.waitingThreads)
			{
				int index = this.waitingThreads.IndexOf(thread);
				if (index > 0)
				{
					this.waitingThreads.MoveItem(index, 0);
				}
			}
		}

		internal void ThreadDone(ThreadQueueItem thread)
		{
			try
			{
				this.threadDoneMutex.WaitOne();

				lock (this.executingThreads)
				{
					this.executingThreads.Remove(thread);
				}

				this.ExecuteThreads();
			}
			finally
			{
				this.threadDoneMutex.ReleaseMutex();
			}
		}

		private void ExecuteThreads()
		{
			lock (this.waitingThreads)
			{
				lock (this.executingThreads)
				{
					while (this.executingThreads.Count < this.maxExecutingThreads && this.waitingThreads.Count != 0)
					{
						ThreadQueueItem newThread = this.waitingThreads.First();
						this.waitingThreads.RemoveAt(0);
						this.executingThreads.Add(newThread);
						if (this.executingThreads.Count >= this.minExecutingThreads)
						{
							foreach (var thread in this.executingThreads)
							{
								thread.Execute();
							}
						}
					}
				}
			}
		}

		#endregion methods
	}
}