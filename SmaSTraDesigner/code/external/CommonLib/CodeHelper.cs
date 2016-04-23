namespace Common
{
	using System;

	// TODO: (PS) Comment this.
	public static class CodeHelper
	{
		#region static methods

		public static string FilterOutComments(string code)
		{
			bool unclosedMultiLineComment = false;
			return FilterOutComments(code, ref unclosedMultiLineComment);
		}

		public static string FilterOutComments(string code, ref bool unclosedMultiLineComment)
		{
			int index;
			if (unclosedMultiLineComment)
			{
				index = code.IndexOf("*/");
				return index < 0 ? String.Empty : code.Substring(index + 2);
			}

			index = code.IndexOf("//");
			if (index >= 0)
			{
				code = code.Substring(0, index);
			}

			index = 0;
			while ((index = code.IndexOf("/*", index)) >= 0)
			{
				int endIndex = code.IndexOf("*/", index);
				if (endIndex >= 0)
				{
					string temp = code.Substring(0, index);
					endIndex += 2;
					code = temp + code.Substring(endIndex);
					unclosedMultiLineComment = false;
				}
				else
				{
					code = code.Substring(0, index);
					unclosedMultiLineComment = true;
				}
			}

			return code;
		}

		#endregion static methods
	}
}