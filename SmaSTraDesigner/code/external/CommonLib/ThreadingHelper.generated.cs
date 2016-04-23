namespace Common
{
	using Common.Collections;
	using System.Threading;
	using System.ComponentModel;
	using System.Collections.Generic;
	using System;
	public partial class ThreadingHelper
	{
		public static Thread InvokeActionInUnmonitoredWorkerThread<T1, T2>(Action<T1, T2> method, T1 param1, T2 param2)
		{
			if (method == null) {
				throw new ArgumentNullException("method");
			}
			Thread thread = new Thread(delegate() { method(param1, param2); });
			thread.Start();
			return thread;
		}
		public void InvokeActionInWorkerThread<T1, T2>(Action<T1, T2> method, T1 param1, T2 param2)
		{
			if (method == null) {
				throw new ArgumentNullException("method");
			}
			Thread thread = new Thread(delegate(object threadParameter) {
				method(param1, param2);
				this.FinishUpThread((Thread)threadParameter);
			});
			this.StartThread(thread);
		}
		public static Thread InvokeActionInUnmonitoredWorkerThread<T1, T2, T3>(Action<T1, T2, T3> method, T1 param1, T2 param2, T3 param3)
		{
			if (method == null) {
				throw new ArgumentNullException("method");
			}
			Thread thread = new Thread(delegate() { method(param1, param2, param3); });
			thread.Start();
			return thread;
		}
		public void InvokeActionInWorkerThread<T1, T2, T3>(Action<T1, T2, T3> method, T1 param1, T2 param2, T3 param3)
		{
			if (method == null) {
				throw new ArgumentNullException("method");
			}
			Thread thread = new Thread(delegate(object threadParameter) {
				method(param1, param2, param3);
				this.FinishUpThread((Thread)threadParameter);
			});
			this.StartThread(thread);
		}
		public static Thread InvokeActionInUnmonitoredWorkerThread<T1, T2, T3, T4>(Action<T1, T2, T3, T4> method, T1 param1, T2 param2, T3 param3, T4 param4)
		{
			if (method == null) {
				throw new ArgumentNullException("method");
			}
			Thread thread = new Thread(delegate() { method(param1, param2, param3, param4); });
			thread.Start();
			return thread;
		}
		public void InvokeActionInWorkerThread<T1, T2, T3, T4>(Action<T1, T2, T3, T4> method, T1 param1, T2 param2, T3 param3, T4 param4)
		{
			if (method == null) {
				throw new ArgumentNullException("method");
			}
			Thread thread = new Thread(delegate(object threadParameter) {
				method(param1, param2, param3, param4);
				this.FinishUpThread((Thread)threadParameter);
			});
			this.StartThread(thread);
		}
		public static Thread InvokeActionInUnmonitoredWorkerThread<T1, T2, T3, T4, T5>(Action<T1, T2, T3, T4, T5> method, T1 param1, T2 param2, T3 param3, T4 param4, T5 param5)
		{
			if (method == null) {
				throw new ArgumentNullException("method");
			}
			Thread thread = new Thread(delegate() { method(param1, param2, param3, param4, param5); });
			thread.Start();
			return thread;
		}
		public void InvokeActionInWorkerThread<T1, T2, T3, T4, T5>(Action<T1, T2, T3, T4, T5> method, T1 param1, T2 param2, T3 param3, T4 param4, T5 param5)
		{
			if (method == null) {
				throw new ArgumentNullException("method");
			}
			Thread thread = new Thread(delegate(object threadParameter) {
				method(param1, param2, param3, param4, param5);
				this.FinishUpThread((Thread)threadParameter);
			});
			this.StartThread(thread);
		}
		public static Thread InvokeActionInUnmonitoredWorkerThread<T1, T2, T3, T4, T5, T6>(Action<T1, T2, T3, T4, T5, T6> method, T1 param1, T2 param2, T3 param3, T4 param4, T5 param5, T6 param6)
		{
			if (method == null) {
				throw new ArgumentNullException("method");
			}
			Thread thread = new Thread(delegate() { method(param1, param2, param3, param4, param5, param6); });
			thread.Start();
			return thread;
		}
		public void InvokeActionInWorkerThread<T1, T2, T3, T4, T5, T6>(Action<T1, T2, T3, T4, T5, T6> method, T1 param1, T2 param2, T3 param3, T4 param4, T5 param5, T6 param6)
		{
			if (method == null) {
				throw new ArgumentNullException("method");
			}
			Thread thread = new Thread(delegate(object threadParameter) {
				method(param1, param2, param3, param4, param5, param6);
				this.FinishUpThread((Thread)threadParameter);
			});
			this.StartThread(thread);
		}
		public static Thread InvokeActionInUnmonitoredWorkerThread<T1, T2, T3, T4, T5, T6, T7>(Action<T1, T2, T3, T4, T5, T6, T7> method, T1 param1, T2 param2, T3 param3, T4 param4, T5 param5, T6 param6, T7 param7)
		{
			if (method == null) {
				throw new ArgumentNullException("method");
			}
			Thread thread = new Thread(delegate() { method(param1, param2, param3, param4, param5, param6, param7); });
			thread.Start();
			return thread;
		}
		public void InvokeActionInWorkerThread<T1, T2, T3, T4, T5, T6, T7>(Action<T1, T2, T3, T4, T5, T6, T7> method, T1 param1, T2 param2, T3 param3, T4 param4, T5 param5, T6 param6, T7 param7)
		{
			if (method == null) {
				throw new ArgumentNullException("method");
			}
			Thread thread = new Thread(delegate(object threadParameter) {
				method(param1, param2, param3, param4, param5, param6, param7);
				this.FinishUpThread((Thread)threadParameter);
			});
			this.StartThread(thread);
		}
		public static Thread InvokeActionInUnmonitoredWorkerThread<T1, T2, T3, T4, T5, T6, T7, T8>(Action<T1, T2, T3, T4, T5, T6, T7, T8> method, T1 param1, T2 param2, T3 param3, T4 param4, T5 param5, T6 param6, T7 param7, T8 param8)
		{
			if (method == null) {
				throw new ArgumentNullException("method");
			}
			Thread thread = new Thread(delegate() { method(param1, param2, param3, param4, param5, param6, param7, param8); });
			thread.Start();
			return thread;
		}
		public void InvokeActionInWorkerThread<T1, T2, T3, T4, T5, T6, T7, T8>(Action<T1, T2, T3, T4, T5, T6, T7, T8> method, T1 param1, T2 param2, T3 param3, T4 param4, T5 param5, T6 param6, T7 param7, T8 param8)
		{
			if (method == null) {
				throw new ArgumentNullException("method");
			}
			Thread thread = new Thread(delegate(object threadParameter) {
				method(param1, param2, param3, param4, param5, param6, param7, param8);
				this.FinishUpThread((Thread)threadParameter);
			});
			this.StartThread(thread);
		}
	}
}
