namespace Common.ExtensionMethods
{
	using System;
	using System.Linq;
	using System.Text;

	using Common.Collections.TreeWalk;

	// TODO: (PS) Comment this.
	public static class ExceptionExtensions
	{
		#region extension methods

		public static string GetExceptionStackString(this Exception subject)
		{
			StringBuilder sb = new StringBuilder(subject.ToString());
			sb.AppendLine();

			foreach (Exception ex in subject.GetInnerExceptions())
			{
				sb.AppendLine("\nInner exception:");
				sb.AppendLine(ex.ToString());
			}

			return sb.ToString();
		}

		public static Exception[] GetInnerExceptions(this Exception subject)
		{
			return new UpwardTreeWalk<Exception>(subject, ex => ex.InnerException).Skip(1).ToArray();
		}

		#endregion extension methods
	}
}