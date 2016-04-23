namespace Common.ExtensionMethods
{
	using System;
	using System.Linq;

	using Replacement = System.Tuple<string, string>;

	// TODO: (PS) Comment this.
	public static class TypeExtensions
	{
		#region extension methods

		public static T GetCustomAttribute<T>(this Type subject)
			where T : Attribute
		{
			if (subject == null)
			{
				throw new ArgumentNullException("subject");
			}

			return subject.GetCustomAttributes(typeof(T), true).FirstOrDefault() as T;
		}

		public static string GetKeyword(this Type subject)
		{
			if (subject == null)
			{
				throw new ArgumentNullException("subject");
			}

			var replacements = new Replacement[]
			{
				new Replacement(typeof(object).FullName, "object"),
				new Replacement(typeof(bool).FullName, "bool"),
				new Replacement(typeof(char).FullName, "char"),
				new Replacement(typeof(sbyte).FullName, "sbyte"),
				new Replacement(typeof(byte).FullName, "byte"),
				new Replacement(typeof(short).FullName, "short"),
				new Replacement(typeof(ushort).FullName, "ushort"),
				new Replacement(typeof(int).FullName, "int"),
				new Replacement(typeof(uint).FullName, "uint"),
				new Replacement(typeof(long).FullName, "long"),
				new Replacement(typeof(ulong).FullName, "ulong"),
				new Replacement(typeof(float).FullName, "float"),
				new Replacement(typeof(double).FullName, "double"),
				new Replacement(typeof(string).FullName, "string"),
				new Replacement(typeof(decimal).FullName, "decimal")
			};

			string result = subject.FullName.ReplaceAll(replacements);

			return result != subject.FullName ? result : null;
		}

		#endregion extension methods
	}
}