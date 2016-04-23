namespace Common.Collections.TreeWalk
{
	using System;
	using System.Collections.Generic;

	// TODO: (PS) Comment this.
	public static class ExtensionMethods
	{
		#region extension methods

		public static IEnumerable<T> DownwardTreeWalk<T>(this T subject, Func<T, IEnumerable<T>> selectorFunction, DownwardTreeWalkType downwardTreeWalkType = DownwardTreeWalkType.BreadthFirst)
			where T : class
		{
			if (subject == null)
			{
				throw new ArgumentNullException("subject");
			}

			return new DownwardTreeWalk<T>(subject, selectorFunction, downwardTreeWalkType);
		}

		public static IEnumerable<T> UpwardTreeWalk<T>(this T subject, Func<T, T> selectorFunction)
			where T : class
		{
			if (subject == null)
			{
				throw new ArgumentNullException("subject");
			}

			return new UpwardTreeWalk<T>(subject, selectorFunction);
		}

		#endregion extension methods
	}
}