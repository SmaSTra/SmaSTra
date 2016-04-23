namespace Common.ExtensionMethods
{
	using System.Collections;
	using System.Text;

	/// <summary>
	/// Holds Extension Methods for the StringBuilder class.
	/// </summary>
	public static class StringBuilderExtensions
	{
		#region extension methods

		/// <summary>
		/// Appends the lines.
		/// </summary>
		/// <param name="sb">The StringBuilder.</param>
		/// <param name="list">The object list.</param>
		/// <returns>the complete string</returns>
		public static string AppendLines(this StringBuilder sb, IEnumerable list)
		{
			foreach (object o in list)
			{
				sb.AppendLine(o.ToString());
			}

			return sb.ToString();
		}

		/// <summary>
		/// Appends all of the items in the given list to the StringBuilder.
		/// </summary>
		/// <param name="sb">The StringBuilder.</param>
		/// <param name="parameters">The list of items that are to be appended.</param>
		/// <returns>The StringBuilder again.</returns>
		public static StringBuilder AppendList(this StringBuilder sb, IEnumerable list)
		{
			foreach (object item in list)
			{
				sb.Append(item);
			}

			return sb;
		}

		/// <summary>
		/// Appends all of the given arguments to the StringBuilder.
		/// </summary>
		/// <param name="sb">The StringBuilder.</param>
		/// <param name="parameters">The parameters that are to be appended.</param>
		/// <returns>The StringBuilder again.</returns>
		public static StringBuilder AppendMultiple(this StringBuilder sb, params object[] parameters)
		{
			sb.AppendList(parameters);

			return sb;
		}

		#endregion extension methods
	}
}