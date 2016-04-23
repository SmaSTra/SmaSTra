namespace Common
{
	using System;
	using System.Reflection;

	public class GenericAction<T> : GenericActionBase
	{
		#region static methods

		public static GenericAction<T> GetInstance(Action<object[]> callback)
		{
			return (GenericAction<T>)DistinctInstanceProvider<GenericActionBase>.Instance.GetDistinctInstance(new GenericAction<T>(callback));
		}

		#endregion static methods

		#region constructors

		protected GenericAction(Action<object[]> callback)
			: base(callback)
		{
		}

		#endregion constructors

		#region methods

		public void Invoke(T param)
		{
			this.DynamicInvoke(param);
		}

		#endregion methods
	}

	public class GenericAction<T1, T2> : GenericActionBase
	{
		#region static methods

		public static GenericAction<T1, T2> GetInstance(Action<object[]> callback)
		{
			return (GenericAction<T1, T2>)DistinctInstanceProvider<GenericActionBase>.Instance.GetDistinctInstance(new GenericAction<T1, T2>(callback));
		}

		#endregion static methods

		#region constructors

		protected GenericAction(Action<object[]> callback)
			: base(callback)
		{
		}

		#endregion constructors

		#region methods

		public void Invoke(T1 param1, T2 param2)
		{
			this.DynamicInvoke(param1, param2);
		}

		#endregion methods
	}

	public class GenericAction<T1, T2, T3> : GenericActionBase
	{
		#region static methods

		public static GenericAction<T1, T2, T3> GetInstance(Action<object[]> callback)
		{
			return (GenericAction<T1, T2, T3>)DistinctInstanceProvider<GenericActionBase>.Instance.GetDistinctInstance(new GenericAction<T1, T2, T3>(callback));
		}

		#endregion static methods

		#region constructors

		protected GenericAction(Action<object[]> callback)
			: base(callback)
		{
		}

		#endregion constructors

		#region methods

		public void Invoke(T1 param1, T2 param2, T3 param3)
		{
			this.DynamicInvoke(param1, param2, param3);
		}

		#endregion methods
	}

	public class GenericAction<T1, T2, T3, T4> : GenericActionBase
	{
		#region static methods

		public static GenericAction<T1, T2, T3, T4> GetInstance(Action<object[]> callback)
		{
			return (GenericAction<T1, T2, T3, T4>)DistinctInstanceProvider<GenericActionBase>.Instance.GetDistinctInstance(new GenericAction<T1, T2, T3, T4>(callback));
		}

		#endregion static methods

		#region constructors

		protected GenericAction(Action<object[]> callback)
			: base(callback)
		{
		}

		#endregion constructors

		#region methods

		public void Invoke(T1 param1, T2 param2, T3 param3, T4 param4)
		{
			this.DynamicInvoke(param1, param2, param3, param4);
		}

		#endregion methods
	}

	public class GenericAction<T1, T2, T3, T4, T5> : GenericActionBase
	{
		#region static methods

		public static GenericAction<T1, T2, T3, T4, T5> GetInstance(Action<object[]> callback)
		{
			return (GenericAction<T1, T2, T3, T4, T5>)DistinctInstanceProvider<GenericActionBase>.Instance.GetDistinctInstance(new GenericAction<T1, T2, T3, T4, T5>(callback));
		}

		#endregion static methods

		#region constructors

		protected GenericAction(Action<object[]> callback)
			: base(callback)
		{
		}

		#endregion constructors

		#region methods

		public void Invoke(T1 param1, T2 param2, T3 param3, T4 param4, T5 param5)
		{
			this.DynamicInvoke(param1, param2, param3, param4, param5);
		}

		#endregion methods
	}

	public class GenericAction<T1, T2, T3, T4, T5, T6> : GenericActionBase
	{
		#region static methods

		public static GenericAction<T1, T2, T3, T4, T5, T6> GetInstance(Action<object[]> callback)
		{
			return (GenericAction<T1, T2, T3, T4, T5, T6>)DistinctInstanceProvider<GenericActionBase>.Instance.GetDistinctInstance(new GenericAction<T1, T2, T3, T4, T5, T6>(callback));
		}

		#endregion static methods

		#region constructors

		protected GenericAction(Action<object[]> callback)
			: base(callback)
		{
		}

		#endregion constructors

		#region methods

		public void Invoke(T1 param1, T2 param2, T3 param3, T4 param4, T5 param5, T6 param6)
		{
			this.DynamicInvoke(param1, param2, param3, param4, param5, param6);
		}

		#endregion methods
	}

	public class GenericAction<T1, T2, T3, T4, T5, T6, T7> : GenericActionBase
	{
		#region static methods

		public static GenericAction<T1, T2, T3, T4, T5, T6, T7> GetInstance(Action<object[]> callback)
		{
			return (GenericAction<T1, T2, T3, T4, T5, T6, T7>)DistinctInstanceProvider<GenericActionBase>.Instance.GetDistinctInstance(new GenericAction<T1, T2, T3, T4, T5, T6, T7>(callback));
		}

		#endregion static methods

		#region constructors

		protected GenericAction(Action<object[]> callback)
			: base(callback)
		{
		}

		#endregion constructors

		#region methods

		public void Invoke(T1 param1, T2 param2, T3 param3, T4 param4, T5 param5, T6 param6, T7 param7)
		{
			this.DynamicInvoke(param1, param2, param3, param4, param5, param6, param7);
		}

		#endregion methods
	}

	public class GenericAction<T1, T2, T3, T4, T5, T6, T7, T8> : GenericActionBase
	{
		#region static methods

		public static GenericAction<T1, T2, T3, T4, T5, T6, T7, T8> GetInstance(Action<object[]> callback)
		{
			return (GenericAction<T1, T2, T3, T4, T5, T6, T7, T8>)DistinctInstanceProvider<GenericActionBase>.Instance.GetDistinctInstance(new GenericAction<T1, T2, T3, T4, T5, T6, T7, T8>(callback));
		}

		#endregion static methods

		#region constructors

		protected GenericAction(Action<object[]> callback)
			: base(callback)
		{
		}

		#endregion constructors

		#region methods

		public void Invoke(T1 param1, T2 param2, T3 param3, T4 param4, T5 param5, T6 param6, T7 param7, T8 param8)
		{
			this.DynamicInvoke(param1, param2, param3, param4, param5, param6, param7, param8);
		}

		#endregion methods
	}

	public class GenericAction<T1, T2, T3, T4, T5, T6, T7, T8, T9> : GenericActionBase
	{
		#region static methods

		public static GenericAction<T1, T2, T3, T4, T5, T6, T7, T8, T9> GetInstance(Action<object[]> callback)
		{
			return (GenericAction<T1, T2, T3, T4, T5, T6, T7, T8, T9>)DistinctInstanceProvider<GenericActionBase>.Instance.GetDistinctInstance(new GenericAction<T1, T2, T3, T4, T5, T6, T7, T8, T9>(callback));
		}

		#endregion static methods

		#region constructors

		protected GenericAction(Action<object[]> callback)
			: base(callback)
		{
		}

		#endregion constructors

		#region methods

		public void Invoke(T1 param1, T2 param2, T3 param3, T4 param4, T5 param5, T6 param6, T7 param7, T8 param8, T9 param9)
		{
			this.DynamicInvoke(param1, param2, param3, param4, param5, param6, param7, param8, param9);
		}

		#endregion methods
	}

	public class GenericAction<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10> : GenericActionBase
	{
		#region static methods

		public static GenericAction<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10> GetInstance(Action<object[]> callback)
		{
			return (GenericAction<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10>)DistinctInstanceProvider<GenericActionBase>.Instance.GetDistinctInstance(new GenericAction<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10>(callback));
		}

		#endregion static methods

		#region constructors

		protected GenericAction(Action<object[]> callback)
			: base(callback)
		{
		}

		#endregion constructors

		#region methods

		public void Invoke(T1 param1, T2 param2, T3 param3, T4 param4, T5 param5, T6 param6, T7 param7, T8 param8, T9 param9, T10 param10)
		{
			this.DynamicInvoke(param1, param2, param3, param4, param5, param6, param7, param8, param9, param10);
		}

		#endregion methods
	}

	public class GenericAction<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11> : GenericActionBase
	{
		#region static methods

		public static GenericAction<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11> GetInstance(Action<object[]> callback)
		{
			return (GenericAction<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10,
			T11>)DistinctInstanceProvider<GenericActionBase>.Instance.GetDistinctInstance(new GenericAction<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10,
			T11>(callback));
		}

		#endregion static methods

		#region constructors

		protected GenericAction(Action<object[]> callback)
			: base(callback)
		{
		}

		#endregion constructors

		#region methods

		public void Invoke(T1 param1, T2 param2, T3 param3, T4 param4, T5 param5, T6 param6, T7 param7, T8 param8, T9 param9, T10 param10,
			T11 param11)
		{
			this.DynamicInvoke(param1, param2, param3, param4, param5, param6, param7, param8, param9, param10,
			param11);
		}

		#endregion methods
	}

	public class GenericAction<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12> : GenericActionBase
	{
		#region static methods

		public static GenericAction<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12> GetInstance(Action<object[]> callback)
		{
			return (GenericAction<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10,
			T11, T12>)DistinctInstanceProvider<GenericActionBase>.Instance.GetDistinctInstance(new GenericAction<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10,
			T11, T12>(callback));
		}

		#endregion static methods

		#region constructors

		protected GenericAction(Action<object[]> callback)
			: base(callback)
		{
		}

		#endregion constructors

		#region methods

		public void Invoke(T1 param1, T2 param2, T3 param3, T4 param4, T5 param5, T6 param6, T7 param7, T8 param8, T9 param9, T10 param10,
			T11 param11, T12 param12)
		{
			this.DynamicInvoke(param1, param2, param3, param4, param5, param6, param7, param8, param9, param10,
			param11, param12);
		}

		#endregion methods
	}

	public class GenericAction<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, T13> : GenericActionBase
	{
		#region static methods

		public static GenericAction<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, T13> GetInstance(Action<object[]> callback)
		{
			return (GenericAction<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10,
			T11, T12, T13>)DistinctInstanceProvider<GenericActionBase>.Instance.GetDistinctInstance(new GenericAction<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10,
			T11, T12, T13>(callback));
		}

		#endregion static methods

		#region constructors

		protected GenericAction(Action<object[]> callback)
			: base(callback)
		{
		}

		#endregion constructors

		#region methods

		public void Invoke(T1 param1, T2 param2, T3 param3, T4 param4, T5 param5, T6 param6, T7 param7, T8 param8, T9 param9, T10 param10,
			T11 param11, T12 param12, T13 param13)
		{
			this.DynamicInvoke(param1, param2, param3, param4, param5, param6, param7, param8, param9, param10,
			param11, param12, param13);
		}

		#endregion methods
	}

	public class GenericAction<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, T13, T14> : GenericActionBase
	{
		#region static methods

		public static GenericAction<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, T13, T14> GetInstance(Action<object[]> callback)
		{
			return (GenericAction<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10,
			T11, T12, T13, T14>)DistinctInstanceProvider<GenericActionBase>.Instance.GetDistinctInstance(new GenericAction<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10,
			T11, T12, T13, T14>(callback));
		}

		#endregion static methods

		#region constructors

		protected GenericAction(Action<object[]> callback)
			: base(callback)
		{
		}

		#endregion constructors

		#region methods

		public void Invoke(T1 param1, T2 param2, T3 param3, T4 param4, T5 param5, T6 param6, T7 param7, T8 param8, T9 param9, T10 param10,
			T11 param11, T12 param12, T13 param13, T14 param14)
		{
			this.DynamicInvoke(param1, param2, param3, param4, param5, param6, param7, param8, param9, param10,
			param11, param12, param13, param14);
		}

		#endregion methods
	}

	public class GenericAction<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, T13, T14, T15> : GenericActionBase
	{
		#region static methods

		public static GenericAction<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, T13, T14, T15> GetInstance(Action<object[]> callback)
		{
			return (GenericAction<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10,
			T11, T12, T13, T14, T15>)DistinctInstanceProvider<GenericActionBase>.Instance.GetDistinctInstance(new GenericAction<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10,
			T11, T12, T13, T14, T15>(callback));
		}

		#endregion static methods

		#region constructors

		protected GenericAction(Action<object[]> callback)
			: base(callback)
		{
		}

		#endregion constructors

		#region methods

		public void Invoke(T1 param1, T2 param2, T3 param3, T4 param4, T5 param5, T6 param6, T7 param7, T8 param8, T9 param9, T10 param10,
			T11 param11, T12 param12, T13 param13, T14 param14, T15 param15)
		{
			this.DynamicInvoke(param1, param2, param3, param4, param5, param6, param7, param8, param9, param10,
			param11, param12, param13, param14, param15);
		}

		#endregion methods
	}

	public class GenericAction<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, T13, T14, T15, T16> : GenericActionBase
	{
		#region static methods

		public static GenericAction<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, T13, T14, T15, T16> GetInstance(Action<object[]> callback)
		{
			return (GenericAction<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10,
			T11, T12, T13, T14, T15, T16>)DistinctInstanceProvider<GenericActionBase>.Instance.GetDistinctInstance(new GenericAction<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10,
			T11, T12, T13, T14, T15, T16>(callback));
		}

		#endregion static methods

		#region constructors

		protected GenericAction(Action<object[]> callback)
			: base(callback)
		{
		}

		#endregion constructors

		#region methods

		public void Invoke(T1 param1, T2 param2, T3 param3, T4 param4, T5 param5, T6 param6, T7 param7, T8 param8, T9 param9, T10 param10,
			T11 param11, T12 param12, T13 param13, T14 param14, T15 param15, T16 param16)
		{
			this.DynamicInvoke(param1, param2, param3, param4, param5, param6, param7, param8, param9, param10,
			param11, param12, param13, param14, param15, param16);
		}

		#endregion methods
	}
}