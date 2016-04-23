namespace Common.ExtensionMethods
{
	using System;
	using System.Collections.Generic;
	using System.Linq;
	using System.Text;

	/// <summary>
	/// Extension Methods for String class
	/// </summary>
	public static class StringExtensions
	{
		#region extension methods

		/// <summary>
		/// Returns if the given "substring" is in the calling string object
		/// </summary>
		/// <param name="subject">the string which might contain the substring </param>
		/// <param name="substring">the substring to be searched</param>
		/// <param name="ignoreCase">Ignores case if ignoreCase is true.</param>
		/// <returns>true: item included otherwise false</returns>
		public static bool Contains(this string subject, string substring, bool ignoreCase)
		{
			if (subject == null)
			{
				throw new ArgumentNullException("subject");
			}

			if (ignoreCase)
			{
				if (substring.Length == subject.Length)
				{
					return string.Compare(subject, substring, true) == 0;
				}
				else if (substring.Length < subject.Length)
				{
					bool found;
					int length = subject.Length - substring.Length;

					for (int i = 0; i < length; i++)
					{
						found = true;

						for (int h = 0; h < substring.Length; h++)
						{
							if (char.ToLower(subject[i + h]) != char.ToLower(substring[h]))
							{
								found = false;
								break;
							}
						}

						if (found)
						{
							return true;
						}
					}
				}

				return false;
			}
			else
			{
				return subject.Contains(substring);
			}
		}

		// TODO: (PS) Comment this.
		public static int Count(this string subject, string substring, StringComparison stringComparison = StringComparison.InvariantCulture)
		{
			if (subject == null)
			{
				throw new ArgumentNullException("subject");
			}

			int index = 0;
			int result = 0;
			while ((index = subject.IndexOf(substring, index, stringComparison)) >= 0)
			{
				result++;
				index += substring.Length;
			}

			return result;
		}

		// TODO: (PS) Comment this.
		public static string RemoveAll(this string subject, params char[] toRemove)
		{
			return RemoveAll(subject, false, toRemove.AsEnumerable());
		}

		// TODO: (PS) Comment this.
		/// <summary>
		/// Removes the specified char values from this string.
		/// </summary>
		/// <param name="subject">The string.</param>
		/// <param name="toRemove">The char values that are supposed to be removed.</param>
		/// <returns>A copy of this string where all the given char values have been removed.</returns>
		public static string RemoveAll(this string subject, bool ignoreCase, params char[] toRemove)
		{
			return RemoveAll(subject, ignoreCase, toRemove.AsEnumerable());
		}

		// TODO: (PS) Comment this.
		public static string RemoveAll(this string subject, IEnumerable<char> toRemove)
		{
			return RemoveAll(subject, false, toRemove);
		}

		// TODO: (PS) Comment this.
		/// <summary>
		/// Removes the specified char values from this string.
		/// </summary>
		/// <param name="subject">The string.</param>
		/// <param name="toRemove">The char values that are supposed to be removed.</param>
		/// <returns>A copy of this string where all the given char values have been removed.</returns>
		public static string RemoveAll(this string subject, bool ignoreCase, IEnumerable<char> toRemove)
		{
			if (subject == null)
			{
				throw new ArgumentNullException("subject");
			}

			if (toRemove.Count() == 0)
			{
				return subject;
			}

			StringBuilder sb = new StringBuilder();

			int startIndex = 0;
			int length;
			for (int i = 0; i < subject.Length; i++)
			{
				char b = subject[i];
				foreach (char c in toRemove)
				{
					if (b == c || (ignoreCase && Char.ToLowerInvariant(b) == Char.ToLowerInvariant(c)))
					{
						length = i - startIndex;
						if (length != 0)
						{
							sb.Append(subject.Substring(startIndex, length));
						}

						startIndex = i + 1;
						break;
					}
				}
			}

			length = subject.Length - startIndex;
			if (length != 0)
			{
				sb.Append(subject.Substring(startIndex, length));
			}

			return sb.ToString();
		}

		/// <summary>
		/// Removes the specified substrings from this string.
		/// </summary>
		/// <param name="subject">The string.</param>
		/// <param name="toRemove">The substrings that are supposed to be removed.</param>
		/// <returns>A copy of this string where all the given substrings have been removed.</returns>
		public static string RemoveAll(this string subject, params string[] toRemove)
		{
			return RemoveAll(subject, StringComparison.InvariantCultureIgnoreCase, toRemove.AsEnumerable());
		}

		// TODO: (PS) Comment this.
		public static string RemoveAll(this string subject, StringComparison stringComparison, params string[] toRemove)
		{
			return RemoveAll(subject, stringComparison, toRemove.AsEnumerable());
		}

		// TODO: (PS) Comment this.
		public static string RemoveAll(this string subject, IEnumerable<string> toRemove)
		{
			return RemoveAll(subject, StringComparison.InvariantCultureIgnoreCase, toRemove);
		}

		/// <summary>
		/// Removes the specified substrings from this string.
		/// </summary>
		/// <param name="subject">The string.</param>
		/// <param name="toRemove">The substrings that are supposed to be removed.</param>
		/// <returns>A copy of this string where all the given substrings have been removed.</returns>
		public static string RemoveAll(this string subject, StringComparison stringComparison, IEnumerable<string> toRemove)
		{
			if (subject == null)
			{
				throw new ArgumentNullException("subject");
			}

			int toRemoveCount = toRemove.Count();
			if (toRemoveCount == 0)
			{
				return subject;
			}

			StringBuilder sb = new StringBuilder();

			int startIndex = 0;
			int length;
			for (int i = 0; i < subject.Length; i++)
			{
				int skipCount = 0;
				foreach (string text in toRemove)
				{
					if (text.Length <= subject.Length - i)
					{
						if (String.Equals(text, subject.Substring(i, text.Length), stringComparison))
						{
							length = i - startIndex;
							if (length != 0)
							{
								sb.Append(subject.Substring(startIndex, length));
							}

							i += text.Length - 1;
							startIndex = i + 1;
							break;
						}
					}
					else
					{
						skipCount++;
					}
				}

				if (skipCount >= toRemoveCount)
				{
					break;
				}
			}

			length = subject.Length - startIndex;
			if (length != 0)
			{
				sb.Append(subject.Substring(startIndex, length));
			}

			return sb.ToString();
		}

		/// <summary>
		/// Replaces the all instances of 'oldValue' in this string with the given 'newValue' string,
		/// while taking the given 'comparisonType' into consideration.
		/// (May be slower than the String class's original Replace(...) method.)
		/// </summary>
		/// <param name="subject">The string instance.</param>
		/// <param name="oldValue">The value that is supposed to be replaced.</param>
		/// <param name="newValue">The value that is supposet to be put in the old value's place.</param>
		/// <param name="comparisonType">Specifies how this string is compared with the old value.</param>
		/// <returns></returns>
		public static string Replace(this string subject, string oldValue, string newValue, StringComparison comparisonType)
		{
			if (subject == null)
			{
				throw new ArgumentNullException("subject");
			}

			int index = 0;
			string result = subject;
			while ((index = result.IndexOf(oldValue, index, comparisonType)) >= 0)
			{
				int endIndex = index + oldValue.Length;
				result = String.Concat(result.Substring(0, index), newValue, result.Substring(endIndex, result.Length - endIndex));
				index += newValue.Length;
			}

			return result;
		}

		// TODO: (PS) Comment this.
		public static string ReplaceAll(this string subject, params string[] replacements)
		{
			return ReplaceAll(subject, StringComparison.InvariantCulture, replacements);
		}

		// TODO: (PS) Comment this.
		public static string ReplaceAll(this string subject, StringComparison comparisonType, params string[] replacements)
		{
			if (subject == null)
			{
				throw new ArgumentNullException("subject");
			}
			if (replacements.Length == 0 || (replacements.Length & 1) == 1)
			{
				throw new ArgumentException("Argument array 'replacements' length must not be 0 or odd.", "replacements");
			}

			Tuple<string, string>[] tuples = new Tuple<string, string>[replacements.Length / 2];
			int h = 0;
			for (int i = 0; i < tuples.Length; i++)
			{
				tuples[i] = new Tuple<string, string>(replacements[h++], replacements[h++]);
			}

			return ReplaceAll(subject, tuples, comparisonType);
		}

		// TODO: (PS) Comment this.
		public static string ReplaceAll(this string subject, IEnumerable<Tuple<string, string>> replacements, StringComparison comparisonType = StringComparison.InvariantCulture)
		{
			if (subject == null)
			{
				throw new ArgumentNullException("subject");
			}
			if (replacements == null)
			{
				throw new ArgumentNullException("replacements");
			}

			int startIndex = 0;
			StringBuilder sb = new StringBuilder();
			bool longEnough = true;
			for (int i = 0; i < subject.Length && longEnough; i++)
			{
				longEnough = false;
				foreach (var replacement in replacements)
				{
					if (i + replacement.Item1.Length <= subject.Length)
					{
						longEnough = true;

						if (String.Equals(subject.Substring(i, replacement.Item1.Length), replacement.Item1, comparisonType))
						{
							if (i != 0)
							{
								sb.Append(subject.Substring(startIndex, i - startIndex));
							}

							sb.Append(replacement.Item2);

							i += replacement.Item1.Length;
							startIndex = i;
							i--;
							break;
						}
					}
				}
			}

			if (startIndex < subject.Length)
			{
				sb.Append(subject.Substring(startIndex, subject.Length - startIndex));
			}

			return sb.ToString();
		}

		#endregion extension methods
	}
}