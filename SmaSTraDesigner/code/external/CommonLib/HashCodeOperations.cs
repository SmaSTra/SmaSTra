namespace Common
{
	using System;
	using System.Collections.Generic;

	// TODO: (PS) Comment this.
	public static class HashCodeOperations
	{
		#region static methods

		public static int Combine(IEnumerable<int> hashCodes)
		{
			if (hashCodes == null)
			{
				throw new ArgumentNullException("hashCodes");
			}

			int result = 0;
			foreach (int hashCode in hashCodes)
			{
				result ^= hashCode;
			}

			return result;
		}

		public static int Combine(params int[] hashCodes)
		{
			return Combine((IEnumerable<int>)hashCodes);
		}

		public static int Combine(IEnumerable<object> objectsToHash)
		{
			if (objectsToHash == null)
			{
				throw new ArgumentNullException("objectsToHash");
			}
			
			int result = 17;
			foreach (object obj in objectsToHash)
			{
				int hash = obj != null ? obj.GetHashCode() : 0;
				unchecked
				{
					result = result * 31 + hash;
				}
			}

			return result;
		}

		public static int Combine(params object[] objectsToHash)
		{
			return Combine((IEnumerable<object>)objectsToHash);
		}

		#endregion static methods
	}
}