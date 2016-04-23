namespace Common
{
	using System;
	using System.ComponentModel;
	using System.Threading;

	// TODO: (PS) Comment this.
	public class BlockMonitor : IDisposable, INotifyPropertyChanged
	{
		#region fields

		/// <summary>
		/// TODO: (PS) Comment this.
		/// </summary>
		private bool isInside = false;

		/// <summary>
		/// TODO: (PS) Comment this.
		/// </summary>
		private int lockStrength = 0;
		private Mutex mutex = null;

		#endregion fields

		#region constructors

		/// <summary>
		/// Initializes a new instance of the <see cref="BlockMonitor"/> class.
		/// </summary>
		public BlockMonitor()
			: this(true)
		{
		}

		public BlockMonitor(bool threadSafe)
		{
			if (threadSafe)
			{
				this.mutex = new Mutex();
			}
		}

		public BlockMonitor(bool threadSafe, EventHandler enteredCallback, EventHandler leftCallback)
			: this(threadSafe)
		{
			if (enteredCallback != null)
			{
				this.Entered += enteredCallback;
			}
			if (leftCallback != null)
			{
				this.Left += leftCallback;
			}
		}

		#endregion constructors

		#region events

		/// <summary>
		/// Occurs when block is entered.
		/// </summary>
		public event EventHandler Entered;

		/// <summary>
		/// Occurs when block is left.
		/// </summary>
		public event EventHandler Left;

		/// <summary>
		/// Occurs when a property value changes.
		/// </summary>
		public event PropertyChangedEventHandler PropertyChanged;

		#endregion events

		#region properties

		/// <summary>
		/// Gets or sets the IsInside property value.
		/// TODO: (PS) Comment this.
		/// </summary>
		public bool IsInside
		{
			get
			{
				return this.isInside;
			}
			private set
			{
				if (value != this.isInside)
				{
					bool oldValue = this.isInside;
					this.isInside = value;
					this.OnIsInsideChanged(oldValue, value);
				}
			}
		}

		// TODO: (PS) Comment this.
		public bool IsThreadSafe
		{
			get { return this.mutex != null; }
		}

		/// <summary>
		/// Gets or sets the LockStrength property value.
		/// TODO: (PS) Comment this.
		/// </summary>
		public int LockStrength
		{
			get
			{
				return this.lockStrength;
			}
			private set
			{
				if (value != this.lockStrength)
				{
					if (this.IsThreadSafe)
					{
						this.mutex.WaitOne();
					}

					int oldValue = this.lockStrength;
					this.lockStrength = value;
					this.OnLockStrengthChanged(oldValue, value);

					if (this.IsThreadSafe)
					{
						this.mutex.ReleaseMutex();
					}
				}
			}
		}

		#endregion properties

		#region overrideable methods

		/// <summary>
		/// Raises the <see cref="E:Entered"/> event.
		/// </summary>
		protected virtual void OnEntered()
		{
			if (this.Entered != null)
			{
				this.Entered(this, null);
			}
		}

		/// <summary>
		/// Called when the IsInside property changed its value.
		/// </summary>
		/// <param name="oldValue">The old value.</param>
		/// <param name="newValue">The new value.</param>
		protected virtual void OnIsInsideChanged(bool oldValue, bool newValue)
		{
			this.OnPropertyChanged("IsInside");
		}

		/// <summary>
		/// Raises the <see cref="E:Left"/> event.
		/// </summary>
		protected virtual void OnLeft()
		{
			if (this.Left != null)
			{
				this.Left(this, null);
			}
		}

		/// <summary>
		/// Called when the LockStrength property changed its value.
		/// </summary>
		/// <param name="oldValue">The old value.</param>
		/// <param name="newValue">The new value.</param>
		protected virtual void OnLockStrengthChanged(int oldValue, int newValue)
		{
			this.IsInside = newValue != 0;
			this.OnPropertyChanged("LockStrength");
		}

		#endregion overrideable methods

		#region methods

		public BlockMonitor Enter()
		{
			this.LockStrength++;
			this.OnEntered();

			return this;
		}

		protected void Leave()
		{
			if (this.LockStrength > 0)
			{
				this.LockStrength--;
				this.OnLeft();
			}
		}

		/// <summary>
		/// Called when property is changed.
		/// </summary>
		/// <param name="propertyName">Name of the property.</param>
		protected void OnPropertyChanged(string propertyName)
		{
			if (this.PropertyChanged != null)
			{
				this.PropertyChanged(this, new PropertyChangedEventArgs(propertyName));
			}
		}

		void IDisposable.Dispose()
		{
			this.Leave();
		}

		#endregion methods
	}
}