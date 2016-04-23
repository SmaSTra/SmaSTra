namespace Common
{
	using System;
	using System.Reflection;
	using System.Threading;

	using TO = System.Threading.Timeout;

	// TODO: (PS) Comment this.
	public class EventHook
	{
		#region static methods

		public static void WaitForCallback(object subject, EventInfo eventInfo, Action<object[], Info> callback = null, int timeout = TO.Infinite)
		{
			new EventHook(subject, eventInfo, callback, timeout).Wait();
		}

		public static void WaitForCallback(EventInfo eventInfo, Action<object[], Info> callback = null, int timeout = TO.Infinite)
		{
			WaitForCallback(null, eventInfo, callback, timeout);
		}

		#endregion static methods

		#region fields

		private Mutex callbackMutex = new Mutex();
		private bool doneWaiting = false;
		private bool waiting = false;

		#endregion fields

		#region constructors

		public EventHook(object subject, EventInfo eventInfo, Action<object[], Info> callback = null, int timeout = TO.Infinite)
		{
			if (eventInfo == null)
			{
				throw new ArgumentNullException("eventInfo");
			}

			this.Subject = subject;
			this.EventInfo = eventInfo;
			this.Callback = callback;
			this.Timeout = timeout;
			this.ResetStartTime();
			GenericAction.AddGenericEventHandler(subject, eventInfo, this.CallbackWrap);
		}

		public EventHook(EventInfo eventInfo, Action<object[], Info> callback = null, int timeout = TO.Infinite)
			: this(null, eventInfo, callback, timeout)
		{
		}

		#endregion constructors

		#region properties

		public Action<object[], Info> Callback
		{
			get;
			private set;
		}

		public EventInfo EventInfo
		{
			get;
			private set;
		}

		public DateTime StartTime
		{
			get;
			private set;
		}

		public object Subject
		{
			get;
			private set;
		}

		public int Timeout
		{
			get;
			private set;
		}

		#endregion properties

		#region methods

		public void ResetStartTime()
		{
			if (this.waiting)
			{
				throw new InvalidOperationException("Already waiting.");
			}

			this.StartTime = DateTime.Now;
		}

		public void Wait()
		{
			this.waiting = true;
			TimeSpan timeSpan = TimeSpan.FromMilliseconds(this.Timeout);

			while (!this.doneWaiting && (this.Timeout < 0 || (DateTime.Now - this.StartTime) < timeSpan)) ;

			GenericAction.RemoveGenericEventHandler(this.Subject, this.EventInfo, this.CallbackWrap);
		}

		private void CallbackWrap(object[] args)
		{
			this.callbackMutex.WaitOne();

			if (!this.doneWaiting)
			{
				Info info = new Info();
				if (this.Callback != null)
				{
					this.Callback(args, info);
				}

				this.doneWaiting = !info.ContinueWaiting;
			}

			this.callbackMutex.ReleaseMutex();
		}

		#endregion methods

		#region nested types

		public class Info
		{
			#region constructors

			public Info()
			{
				this.ContinueWaiting = true;
			}

			#endregion constructors

			#region properties

			public bool ContinueWaiting
			{
				get;
				set;
			}

			#endregion properties
		}

		#endregion nested types
	}
}