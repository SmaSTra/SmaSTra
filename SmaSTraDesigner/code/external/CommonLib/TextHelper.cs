namespace Common
{
	using System;
	using System.Collections.Generic;
	using System.Globalization;
	using System.Linq;
	using System.Text;

	using Common.Resources.Localization.Global;

	/// <summary>
	/// Static class that stores various helper methods to help with text/string handling and localization.
	/// </summary>
	public static class TextHelper
	{
		#region static methods

		// TODO: (PS) Comment this.
		public static string GetFileSizeString(long size, string format = "0.0")
		{
			string[] sizeStrings =
			{
				"B",
				"KiB",
				"MiB",
				"GiB",
				"TiB",
				"PiB",
				"EiB",
				"ZiB",
				"YiB"
			};

			double rest = size;
			for (int i = 0; i < sizeStrings.Length; i++)
			{
				double newRest = rest / 1024.0;
				if (newRest < 1)
				{
					return String.Format(i == 0 ? "{0:0} {1}" : "{0:" + format + "} {1}", rest, sizeStrings[i]);
				}

				rest = newRest;
			}

			return size.ToString();
		}

		/// <summary>
		/// Gets a localized enumeration string that combines the given list of strings like the folowing:
		/// "one", "two" and "three" would be "one, two, and" three for the english language.
		/// </summary>
		/// <param name="strings">The strings.</param>
		/// <returns></returns>
		/// <exception cref="ArgumentNullException" />
		public static string GetLocalizedEnumerationString(params string[] strings)
		{
			return GetLocalizedEnumerationString(CultureInfo.CurrentCulture, strings);
		}

		/// <summary>
		/// Gets a localized enumeration string that combines the given list of strings like the folowing:
		/// "one", "two" and "three" would be "one, two, and" three for the english language.
		/// </summary>
		/// <param name="culture">The CultureInfo that is supposed to be used to localize the string.</param>
		/// <param name="strings">The strings.</param>
		/// <returns></returns>
		/// <exception cref="ArgumentNullException"/>
		public static string GetLocalizedEnumerationString(CultureInfo culture, params string[] strings)
		{
			return GetLocalizedEnumerationString(strings);
		}

		/// <summary>
		/// Gets a localized enumeration string that combines the given list of strings like the folowing:
		/// "one", "two" and "three" would be "one, two, and" three for the english language.
		/// </summary>
		/// <param name="strings">The strings.</param>
		/// <returns></returns>
		/// <exception cref="ArgumentNullException" />
		public static string GetLocalizedEnumerationString(IEnumerable<string> strings)
		{
			return GetLocalizedEnumerationString(CultureInfo.CurrentCulture, strings);
		}

		/// <summary>
		/// Gets a localized enumeration string that combines the given list of strings like the folowing:
		/// "one", "two" and "three" would be "one, two, and" three for the english language.
		/// </summary>
		/// <param name="strings">The strings.</param>
		/// <param name="culture">The CultureInfo that is supposed to be used to localize the string.</param>
		/// <returns></returns>
		/// <exception cref="ArgumentNullException" />
		public static string GetLocalizedEnumerationString(CultureInfo culture, IEnumerable<string> strings)
		{
			if (culture == null)
			{
				throw new ArgumentNullException("Argument 'culture' must not be null.");
			}
			if (strings == null)
			{
				throw new ArgumentNullException("Argument 'strings' must not be null.");
			}

			string and = SharedKeywordsResource.ResourceManager.GetString("and", culture);
			StringBuilder sb = new StringBuilder();

			int count = strings.Count();
			if (count != 0)
			{
				int i = 0;
				foreach (var item in strings)
				{
					if (i < count - 2)
					{
						sb.Append(item).Append(", ");
					}
					else if (i < count - 1)
					{
						sb.Append(item);
						if (culture.Name.StartsWith("de", StringComparison.InvariantCultureIgnoreCase))
						{
							// In the german language, no comma is added before the "und" (engl. "and") and the last
							// element of the enumeration string.
							sb.Append(' ');
						}
						else
						{
							sb.Append(", ");
						}
						sb.Append(and).Append(' ');
					}
					i++;
				}

				sb.Append(strings.Last());
			}

			return sb.ToString();
		}

		#endregion static methods
	}
}