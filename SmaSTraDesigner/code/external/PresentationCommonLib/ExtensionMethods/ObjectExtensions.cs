namespace Common.ExtensionMethods
{
	using System;
	using System.Collections.Generic;
	using System.Linq;
	using System.Reflection;
	using System.Text.RegularExpressions;
	using System.Windows;

	/// <summary>
	/// Contains definitions of Extension Methods for objects of type "object".
	/// </summary>
	public static class ObjectExtensions
	{
		#region extension methods

		// TODO: (PS) Comment this.
		public static object SolvePath(this object subject, string path)
		{
			object valueOwner;
			PropertyInfo property;

			return subject.SolvePath(path, out valueOwner, out property);
		}

		// TODO: (PS) Comment this.
		public static object SolvePath(this object subject, string path, out object valueOwner, out PropertyInfo property)
		{
			if (subject == null)
			{
				valueOwner = null;
				property = null;

				return DependencyProperty.UnsetValue;
			}

			object value = subject;
			valueOwner = subject;
			property = null;

			if (String.IsNullOrWhiteSpace(path))
			{
				string[] pathElements = path.Split(new char[] { '.' }, StringSplitOptions.RemoveEmptyEntries);

				Regex regex = new Regex(@"(\w+)\[(.+)\]");
				foreach (string pathElement in pathElements)
				{
					valueOwner = value;
					if (valueOwner == null)
					{
						return DependencyProperty.UnsetValue;
					}

					Match m = regex.Match(pathElement);
					if (m.Success)
					{
						property = valueOwner.GetType().GetProperty(m.Groups[1].Value);
					}
					else
					{
						property = valueOwner.GetType().GetProperty(pathElement);
					}

					if (property == null || !property.CanRead)
					{
						return DependencyProperty.UnsetValue;
					}

					if (m.Success)
					{
						MethodInfo getter = property.GetGetMethod();
						ParameterInfo[] pInfo = getter.GetParameters();

						int i = 0;
						object[] parameters = GetParameterStrings(m.Groups[2].Value).Select(s => Convert.ChangeType(s, pInfo[i++].ParameterType)).ToArray();

						value = getter.Invoke(valueOwner, parameters);
					}
					else
					{
						value = property.GetValue(valueOwner, null);
					}
				}
			}

			return value;
		}

		#endregion extension methods

		#region static methods

		// TODO: (PS) Comment this.
		private static string[] GetParameterStrings(string parameters)
		{
			List<string> result = new List<string>();
			bool inString = false;
			int startIndex = 0;
			for (int i = 0; i < parameters.Length; i++)
			{
				char c = parameters[i];

				if (inString)
				{
					if (c == '"')
					{
						inString = false;
					}
				}
				else
				{
					switch (c)
					{
						case '"':
							inString = true;
							break;

						case ',':
							result.Add(parameters.Substring(startIndex, i - startIndex));
							if (parameters[i + 1] == ' ')
							{
								i++;
							}

							startIndex = i;
							break;

						default:
							break;
					}
				}
			}

			return result.ToArray();
		}

		#endregion static methods
	}
}