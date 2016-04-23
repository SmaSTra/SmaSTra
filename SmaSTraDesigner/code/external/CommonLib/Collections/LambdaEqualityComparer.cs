namespace Common.Collections
{
	using System;
	using System.Collections.Generic;

	// TODO: (PS) Comment this.
	public class LambdaEqualityComparer<T> : IEqualityComparer<T>
	{
		#region constructors

		public LambdaEqualityComparer(Func<T, T, bool> equalsMethod, Func<T, int> getHashCodeMethod)
		{
			if (equalsMethod == null)
			{
				throw new ArgumentNullException("equalsMethod");
			}
			if (getHashCodeMethod == null)
			{
				throw new ArgumentNullException("getHashCodeMethod");
			}

			this.EqualsMethod = equalsMethod;
			this.GetHashCodeMethod = getHashCodeMethod;
		}

		public LambdaEqualityComparer(Func<T, T, bool> equalsMethod)
		{
			if (equalsMethod == null)
			{
				throw new ArgumentNullException("equalsMethod");
			}

			this.EqualsMethod = equalsMethod;
			this.GetHashCodeMethod = EqualityComparer<T>.Default.GetHashCode;
		}

		public LambdaEqualityComparer(Func<T, int> getHashCodeMethod)
		{
			if (getHashCodeMethod == null)
			{
				throw new ArgumentNullException("getHashCodeMethod");
			}

			this.EqualsMethod = EqualityComparer<T>.Default.Equals;
			this.GetHashCodeMethod = getHashCodeMethod;
		}

		#endregion constructors

		#region properties

		public Func<T, T, bool> EqualsMethod
		{
			get;
			private set;
		}

		public Func<T, int> GetHashCodeMethod
		{
			get;
			private set;
		}

		#endregion properties

		#region methods

		public bool Equals(T x, T y)
		{
			return this.EqualsMethod(x, y);
		}

		public int GetHashCode(T obj)
		{
			return this.GetHashCodeMethod(obj);
		}

		#endregion methods
	}
}