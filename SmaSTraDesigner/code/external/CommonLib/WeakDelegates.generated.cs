namespace Common
{
	using System;

	public class WeakAction<T1> : WeakDelegate
	{
		#region static methods

		public static implicit operator Action<T1>(WeakAction<T1> subject)
		{
			return subject.Invoke;
		}

		public static WeakAction<T1> GetInstance(Action<T1> action)
		{
			return (WeakAction<T1>)DistinctInstanceProvider<WeakDelegate>.Instance.GetDistinctInstance(new WeakAction<T1>(action));
		}

		#endregion static methods

		#region constructors

		protected WeakAction(Action<T1> action)
			: base(action)
		{
		}

		#endregion constructors

		#region methods

		public void Invoke(T1 param1)
		{
			this.InvokeMethod(new object[] { param1 });
		}

		#endregion methods
	}

	public class WeakAction<T1, T2> : WeakDelegate
	{
		#region static methods

		public static implicit operator Action<T1, T2>(WeakAction<T1, T2> subject)
		{
			return subject.Invoke;
		}

		public static WeakAction<T1, T2> GetInstance(Action<T1, T2> action)
		{
			return (WeakAction<T1, T2>)DistinctInstanceProvider<WeakDelegate>.Instance.GetDistinctInstance(new WeakAction<T1, T2>(action));
		}

		#endregion static methods

		#region constructors

		protected WeakAction(Action<T1, T2> action)
			: base(action)
		{
		}

		#endregion constructors

		#region methods

		public void Invoke(T1 param1, T2 param2)
		{
			this.InvokeMethod(new object[] {
				param1,
				param2
			});
		}

		#endregion methods
	}

	public class WeakAction<T1, T2, T3> : WeakDelegate
	{
		#region static methods

		public static implicit operator Action<T1, T2, T3>(WeakAction<T1, T2, T3> subject)
		{
			return subject.Invoke;
		}

		public static WeakAction<T1, T2, T3> GetInstance(Action<T1, T2, T3> action)
		{
			return (WeakAction<T1, T2, T3>)DistinctInstanceProvider<WeakDelegate>.Instance.GetDistinctInstance(new WeakAction<T1, T2, T3>(action));
		}

		#endregion static methods

		#region constructors

		protected WeakAction(Action<T1, T2, T3> action)
			: base(action)
		{
		}

		#endregion constructors

		#region methods

		public void Invoke(T1 param1, T2 param2, T3 param3)
		{
			this.InvokeMethod(new object[] {
				param1,
				param2,
				param3
			});
		}

		#endregion methods
	}

	public class WeakAction<T1, T2, T3, T4> : WeakDelegate
	{
		#region static methods

		public static implicit operator Action<T1, T2, T3, T4>(WeakAction<T1, T2, T3, T4> subject)
		{
			return subject.Invoke;
		}

		public static WeakAction<T1, T2, T3, T4> GetInstance(Action<T1, T2, T3, T4> action)
		{
			return (WeakAction<T1, T2, T3, T4>)DistinctInstanceProvider<WeakDelegate>.Instance.GetDistinctInstance(new WeakAction<T1, T2, T3, T4>(action));
		}

		#endregion static methods

		#region constructors

		protected WeakAction(Action<T1, T2, T3, T4> action)
			: base(action)
		{
		}

		#endregion constructors

		#region methods

		public void Invoke(T1 param1, T2 param2, T3 param3, T4 param4)
		{
			this.InvokeMethod(new object[] {
				param1,
				param2,
				param3,
				param4
			});
		}

		#endregion methods
	}

	public class WeakAction<T1, T2, T3, T4, T5> : WeakDelegate
	{
		#region static methods

		public static implicit operator Action<T1, T2, T3, T4, T5>(WeakAction<T1, T2, T3, T4, T5> subject)
		{
			return subject.Invoke;
		}

		public static WeakAction<T1, T2, T3, T4, T5> GetInstance(Action<T1, T2, T3, T4, T5> action)
		{
			return (WeakAction<T1, T2, T3, T4, T5>)DistinctInstanceProvider<WeakDelegate>.Instance.GetDistinctInstance(new WeakAction<T1, T2, T3, T4, T5>(action));
		}

		#endregion static methods

		#region constructors

		protected WeakAction(Action<T1, T2, T3, T4, T5> action)
			: base(action)
		{
		}

		#endregion constructors

		#region methods

		public void Invoke(T1 param1, T2 param2, T3 param3, T4 param4, T5 param5)
		{
			this.InvokeMethod(new object[] {
				param1,
				param2,
				param3,
				param4,
				param5
			});
		}

		#endregion methods
	}

	public class WeakAction<T1, T2, T3, T4, T5, T6> : WeakDelegate
	{
		#region static methods

		public static implicit operator Action<T1, T2, T3, T4, T5, T6>(WeakAction<T1, T2, T3, T4, T5, T6> subject)
		{
			return subject.Invoke;
		}

		public static WeakAction<T1, T2, T3, T4, T5, T6> GetInstance(Action<T1, T2, T3, T4, T5, T6> action)
		{
			return (WeakAction<T1, T2, T3, T4, T5, T6>)DistinctInstanceProvider<WeakDelegate>.Instance.GetDistinctInstance(new WeakAction<T1, T2, T3, T4, T5, T6>(action));
		}

		#endregion static methods

		#region constructors

		protected WeakAction(Action<T1, T2, T3, T4, T5, T6> action)
			: base(action)
		{
		}

		#endregion constructors

		#region methods

		public void Invoke(T1 param1, T2 param2, T3 param3, T4 param4, T5 param5, T6 param6)
		{
			this.InvokeMethod(new object[] {
				param1,
				param2,
				param3,
				param4,
				param5,
				param6
			});
		}

		#endregion methods
	}

	public class WeakAction<T1, T2, T3, T4, T5, T6, T7> : WeakDelegate
	{
		#region static methods

		public static implicit operator Action<T1, T2, T3, T4, T5, T6, T7>(WeakAction<T1, T2, T3, T4, T5, T6, T7> subject)
		{
			return subject.Invoke;
		}

		public static WeakAction<T1, T2, T3, T4, T5, T6, T7> GetInstance(Action<T1, T2, T3, T4, T5, T6, T7> action)
		{
			return (WeakAction<T1, T2, T3, T4, T5, T6, T7>)DistinctInstanceProvider<WeakDelegate>.Instance.GetDistinctInstance(new WeakAction<T1, T2, T3, T4, T5, T6, T7>(action));
		}

		#endregion static methods

		#region constructors

		protected WeakAction(Action<T1, T2, T3, T4, T5, T6, T7> action)
			: base(action)
		{
		}

		#endregion constructors

		#region methods

		public void Invoke(T1 param1, T2 param2, T3 param3, T4 param4, T5 param5, T6 param6, T7 param7)
		{
			this.InvokeMethod(new object[] {
				param1,
				param2,
				param3,
				param4,
				param5,
				param6,
				param7
			});
		}

		#endregion methods
	}

	public class WeakAction<T1, T2, T3, T4, T5, T6, T7, T8> : WeakDelegate
	{
		#region static methods

		public static implicit operator Action<T1, T2, T3, T4, T5, T6, T7, T8>(WeakAction<T1, T2, T3, T4, T5, T6, T7, T8> subject)
		{
			return subject.Invoke;
		}

		public static WeakAction<T1, T2, T3, T4, T5, T6, T7, T8> GetInstance(Action<T1, T2, T3, T4, T5, T6, T7, T8> action)
		{
			return (WeakAction<T1, T2, T3, T4, T5, T6, T7, T8>)DistinctInstanceProvider<WeakDelegate>.Instance.GetDistinctInstance(new WeakAction<T1, T2, T3, T4, T5, T6, T7, T8>(action));
		}

		#endregion static methods

		#region constructors

		protected WeakAction(Action<T1, T2, T3, T4, T5, T6, T7, T8> action)
			: base(action)
		{
		}

		#endregion constructors

		#region methods

		public void Invoke(T1 param1, T2 param2, T3 param3, T4 param4, T5 param5, T6 param6, T7 param7, T8 param8)
		{
			this.InvokeMethod(new object[] {
				param1,
				param2,
				param3,
				param4,
				param5,
				param6,
				param7,
				param8
			});
		}

		#endregion methods
	}

	public class WeakFunc<T1, TResult> : WeakDelegate
	{
		#region static methods

		public static implicit operator Func<T1, TResult>(WeakFunc<T1, TResult> subject)
		{
			return subject.Invoke;
		}

		public static WeakFunc<T1, TResult> GetInstance(Func<T1, TResult> function)
		{
			return (WeakFunc<T1, TResult>)DistinctInstanceProvider<WeakDelegate>.Instance.GetDistinctInstance(new WeakFunc<T1, TResult>(function));
		}

		#endregion static methods

		#region constructors

		protected WeakFunc(Func<T1, TResult> function)
			: base(function)
		{
		}

		#endregion constructors

		#region methods

		public TResult Invoke(T1 param1)
		{
			return (TResult)this.InvokeMethod(new object[] { param1 });
		}

		#endregion methods
	}

	public class WeakFunc<T1, T2, TResult> : WeakDelegate
	{
		#region static methods

		public static implicit operator Func<T1, T2, TResult>(WeakFunc<T1, T2, TResult> subject)
		{
			return subject.Invoke;
		}

		public static WeakFunc<T1, T2, TResult> GetInstance(Func<T1, T2, TResult> function)
		{
			return (WeakFunc<T1, T2, TResult>)DistinctInstanceProvider<WeakDelegate>.Instance.GetDistinctInstance(new WeakFunc<T1, T2, TResult>(function));
		}

		#endregion static methods

		#region constructors

		protected WeakFunc(Func<T1, T2, TResult> function)
			: base(function)
		{
		}

		#endregion constructors

		#region methods

		public TResult Invoke(T1 param1, T2 param2)
		{
			return (TResult)this.InvokeMethod(new object[] {
				param1,
				param2
			});
		}

		#endregion methods
	}

	public class WeakFunc<T1, T2, T3, TResult> : WeakDelegate
	{
		#region static methods

		public static implicit operator Func<T1, T2, T3, TResult>(WeakFunc<T1, T2, T3, TResult> subject)
		{
			return subject.Invoke;
		}

		public static WeakFunc<T1, T2, T3, TResult> GetInstance(Func<T1, T2, T3, TResult> function)
		{
			return (WeakFunc<T1, T2, T3, TResult>)DistinctInstanceProvider<WeakDelegate>.Instance.GetDistinctInstance(new WeakFunc<T1, T2, T3, TResult>(function));
		}

		#endregion static methods

		#region constructors

		protected WeakFunc(Func<T1, T2, T3, TResult> function)
			: base(function)
		{
		}

		#endregion constructors

		#region methods

		public TResult Invoke(T1 param1, T2 param2, T3 param3)
		{
			return (TResult)this.InvokeMethod(new object[] {
				param1,
				param2,
				param3
			});
		}

		#endregion methods
	}

	public class WeakFunc<T1, T2, T3, T4, TResult> : WeakDelegate
	{
		#region static methods

		public static implicit operator Func<T1, T2, T3, T4, TResult>(WeakFunc<T1, T2, T3, T4, TResult> subject)
		{
			return subject.Invoke;
		}

		public static WeakFunc<T1, T2, T3, T4, TResult> GetInstance(Func<T1, T2, T3, T4, TResult> function)
		{
			return (WeakFunc<T1, T2, T3, T4, TResult>)DistinctInstanceProvider<WeakDelegate>.Instance.GetDistinctInstance(new WeakFunc<T1, T2, T3, T4, TResult>(function));
		}

		#endregion static methods

		#region constructors

		protected WeakFunc(Func<T1, T2, T3, T4, TResult> function)
			: base(function)
		{
		}

		#endregion constructors

		#region methods

		public TResult Invoke(T1 param1, T2 param2, T3 param3, T4 param4)
		{
			return (TResult)this.InvokeMethod(new object[] {
				param1,
				param2,
				param3,
				param4
			});
		}

		#endregion methods
	}

	public class WeakFunc<T1, T2, T3, T4, T5, TResult> : WeakDelegate
	{
		#region static methods

		public static implicit operator Func<T1, T2, T3, T4, T5, TResult>(WeakFunc<T1, T2, T3, T4, T5, TResult> subject)
		{
			return subject.Invoke;
		}

		public static WeakFunc<T1, T2, T3, T4, T5, TResult> GetInstance(Func<T1, T2, T3, T4, T5, TResult> function)
		{
			return (WeakFunc<T1, T2, T3, T4, T5, TResult>)DistinctInstanceProvider<WeakDelegate>.Instance.GetDistinctInstance(new WeakFunc<T1, T2, T3, T4, T5, TResult>(function));
		}

		#endregion static methods

		#region constructors

		protected WeakFunc(Func<T1, T2, T3, T4, T5, TResult> function)
			: base(function)
		{
		}

		#endregion constructors

		#region methods

		public TResult Invoke(T1 param1, T2 param2, T3 param3, T4 param4, T5 param5)
		{
			return (TResult)this.InvokeMethod(new object[] {
				param1,
				param2,
				param3,
				param4,
				param5
			});
		}

		#endregion methods
	}

	public class WeakFunc<T1, T2, T3, T4, T5, T6, TResult> : WeakDelegate
	{
		#region static methods

		public static implicit operator Func<T1, T2, T3, T4, T5, T6, TResult>(WeakFunc<T1, T2, T3, T4, T5, T6, TResult> subject)
		{
			return subject.Invoke;
		}

		public static WeakFunc<T1, T2, T3, T4, T5, T6, TResult> GetInstance(Func<T1, T2, T3, T4, T5, T6, TResult> function)
		{
			return (WeakFunc<T1, T2, T3, T4, T5, T6, TResult>)DistinctInstanceProvider<WeakDelegate>.Instance.GetDistinctInstance(new WeakFunc<T1, T2, T3, T4, T5, T6, TResult>(function));
		}

		#endregion static methods

		#region constructors

		protected WeakFunc(Func<T1, T2, T3, T4, T5, T6, TResult> function)
			: base(function)
		{
		}

		#endregion constructors

		#region methods

		public TResult Invoke(T1 param1, T2 param2, T3 param3, T4 param4, T5 param5, T6 param6)
		{
			return (TResult)this.InvokeMethod(new object[] {
				param1,
				param2,
				param3,
				param4,
				param5,
				param6
			});
		}

		#endregion methods
	}

	public class WeakFunc<T1, T2, T3, T4, T5, T6, T7, TResult> : WeakDelegate
	{
		#region static methods

		public static implicit operator Func<T1, T2, T3, T4, T5, T6, T7, TResult>(WeakFunc<T1, T2, T3, T4, T5, T6, T7, TResult> subject)
		{
			return subject.Invoke;
		}

		public static WeakFunc<T1, T2, T3, T4, T5, T6, T7, TResult> GetInstance(Func<T1, T2, T3, T4, T5, T6, T7, TResult> function)
		{
			return (WeakFunc<T1, T2, T3, T4, T5, T6, T7, TResult>)DistinctInstanceProvider<WeakDelegate>.Instance.GetDistinctInstance(new WeakFunc<T1, T2, T3, T4, T5, T6, T7, TResult>(function));
		}

		#endregion static methods

		#region constructors

		protected WeakFunc(Func<T1, T2, T3, T4, T5, T6, T7, TResult> function)
			: base(function)
		{
		}

		#endregion constructors

		#region methods

		public TResult Invoke(T1 param1, T2 param2, T3 param3, T4 param4, T5 param5, T6 param6, T7 param7)
		{
			return (TResult)this.InvokeMethod(new object[] {
				param1,
				param2,
				param3,
				param4,
				param5,
				param6,
				param7
			});
		}

		#endregion methods
	}

	public class WeakFunc<T1, T2, T3, T4, T5, T6, T7, T8, TResult> : WeakDelegate
	{
		#region static methods

		public static implicit operator Func<T1, T2, T3, T4, T5, T6, T7, T8, TResult>(WeakFunc<T1, T2, T3, T4, T5, T6, T7, T8, TResult> subject)
		{
			return subject.Invoke;
		}

		public static WeakFunc<T1, T2, T3, T4, T5, T6, T7, T8, TResult> GetInstance(Func<T1, T2, T3, T4, T5, T6, T7, T8, TResult> function)
		{
			return (WeakFunc<T1, T2, T3, T4, T5, T6, T7, T8, TResult>)DistinctInstanceProvider<WeakDelegate>.Instance.GetDistinctInstance(new WeakFunc<T1, T2, T3, T4, T5, T6, T7, T8, TResult>(function));
		}

		#endregion static methods

		#region constructors

		protected WeakFunc(Func<T1, T2, T3, T4, T5, T6, T7, T8, TResult> function)
			: base(function)
		{
		}

		#endregion constructors

		#region methods

		public TResult Invoke(T1 param1, T2 param2, T3 param3, T4 param4, T5 param5, T6 param6, T7 param7, T8 param8)
		{
			return (TResult)this.InvokeMethod(new object[] {
				param1,
				param2,
				param3,
				param4,
				param5,
				param6,
				param7,
				param8
			});
		}

		#endregion methods
	}
}