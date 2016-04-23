namespace Common.Collections
{
	using System;
	using System.Collections.Generic;

	// TODO: (PS) Comment this.
	public class LambdaComparer<T> : IComparer<T>
	{
		#region constructors

		public LambdaComparer(Func<T, T, int> compareMethod)
		{
			if (compareMethod == null)
			{
				throw new ArgumentNullException("compareMethod");
			}

			this.CompareMethod = compareMethod;
		}

		#endregion constructors

		#region properties

		public Func<T, T, int> CompareMethod
		{
			get;
			private set;
		}

		#endregion properties

		#region methods

		public int Compare(T x, T y)
		{
			return this.CompareMethod(x, y);
		}

		#endregion methods
	}
}