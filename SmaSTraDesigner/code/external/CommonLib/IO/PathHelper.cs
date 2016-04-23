namespace Common.IO
{
	using System;
	using System.IO;

	// TODO: (PS) Comment this.
	public static class PathHelper
	{
		#region static methods

		public static string GetFullPathWithoutFileName(string path)
		{
			if (String.IsNullOrWhiteSpace(path))
			{
				throw new ArgumentException("String argument 'path' must not be null or empty (incl. whitespace).", "path");
			}

			return path.Substring(0, path.Length - Path.GetFileName(path).Length).TrimEnd(Path.DirectorySeparatorChar);
		}

		private static void CreatePath(string path)
		{
			if (String.IsNullOrWhiteSpace(path))
			{
				throw new ArgumentException("String argument 'path' must not be null or empty (incl. whitespace).", "path");
			}
			if (!Path.IsPathRooted(path))
			{
				throw new ArgumentException("Path is not rooted.", "outputDirectory");
			}

			string[] elements = path.Split(Path.DirectorySeparatorChar);
			string seperator = Path.DirectorySeparatorChar.ToString();
			for (int i = 1; i < elements.Length; i++)
			{
				string dir = String.Join(seperator, new ArraySegment<string>(elements, 0, i + 1).Array);
				if (!Directory.Exists(dir))
				{
					Directory.CreateDirectory(dir);
				}
			}
		}

		#endregion static methods
	}
}