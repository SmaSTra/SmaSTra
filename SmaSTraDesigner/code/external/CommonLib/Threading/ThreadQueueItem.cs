namespace Common.Threading
{
	using System;
	using System.Threading;

	// TODO: (PS) Comment this.
	public class ThreadQueueItem
	{
		#region fields

		private ThreadPriority threadPriority;
		private Thread workerThread = null;
		private object workerThreadMonitor = new object();

		#endregion fields

		#region constructors

		internal ThreadQueueItem(ThreadQueue owner, Action workerAction, int number)
		{
			if (owner == null)
			{
				throw new ArgumentNullException("owner");
			}
			if (workerAction == null)
			{
				throw new ArgumentNullException("workerAction");
			}
			if (number < 0)
			{
				throw new ArgumentException("number should be >= 0.", "number");
			}

			this.Owner = owner;
			this.WorkerAction = workerAction;
			this.Number = number;
			this.threadPriority = owner.ThreadPriority;
		}

		#endregion constructors

		#region events

		public event EventHandler ThreadDone;

		#endregion events

		#region properties

		public bool Done
		{
			get;
			private set;
		}

		public bool Executing
		{
			get;
			private set;
		}

		public int Number
		{
			get;
			private set;
		}

		public ThreadQueue Owner
		{
			get;
			private set;
		}

		public ThreadPriority ThreadPriority
		{
			get
			{
				return this.threadPriority;
			}
			set
			{
				this.threadPriority = value;
				lock (this.workerThreadMonitor)
				{
					if (this.workerThread != null)
					{
						this.workerThread.Priority = value;
					}
				}
			}
		}

		public Action WorkerAction
		{
			get;
			private set;
		}

		#endregion properties

		#region overrideable methods

		public override string ToString()
		{
			return String.Format("{0} Thread #{1}", this.Owner, this.Number);
		}

		#endregion overrideable methods

		#region methods

		public void Abort()
		{
			lock (this.workerThreadMonitor)
			{
				if (this.workerThread != null)
				{
					this.workerThread.Abort();
					this.workerThread = null;
				}
			}
		}

		public void Execute()
		{
			lock (this.workerThreadMonitor)
			{
				if (this.workerThread != null)
				{
					return;
				}

				this.workerThread = new Thread(delegate()
				{
					this.WorkerAction();

					lock (this.workerThreadMonitor)
					{
						this.workerThread = null;
					}

					this.Owner.ThreadDone(this);
					this.OnThreadDone();
					this.Done = true;
					this.Executing = false;
				})
				{
					Priority = this.ThreadPriority,
					Name = this.ToString()
				};

				this.workerThread.Start();
			}

			this.Executing = true;
		}

		public void MoveToFront()
		{
			this.Owner.MoveToFront(this);
		}

		/// <summary>
		/// Raises the <see cref="E:ThreadDone"/> event.
		/// </summary>
		private void OnThreadDone()
		{
			if (this.ThreadDone != null)
			{
				this.ThreadDone(this, null);
			}
		}

		#endregion methods
	}
}