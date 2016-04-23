namespace Common.Collections
{
	using System;
	using System.Collections.Generic;

	// TODO: (PS) Comment this.
	public class ReferenceEqualityComparer<T> : IEqualityComparer<T>
	{
		#region overrideable methods

		public bool Equals(T x, T y)
		{
			return object.ReferenceEquals(x, y);
		}

		public int GetHashCode(T obj)
		{
			if (obj == null)
			{
				throw new ArgumentNullException("obj");
			}

			return obj.GetHashCode();
		}

		#endregion overrideable methods
	}
}