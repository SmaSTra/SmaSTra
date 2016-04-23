namespace Common.ExtensionMethods
{
	using System.Linq;
	using System.Reflection;

	// TODO: (PS) Comment this.
	public static class MethodInfoExtensions
	{
		#region extension methods

		public static bool MatchSignature(this MethodInfo subject, MethodInfo other)
		{
			if (subject.ReturnType != other.ReturnType)
			{
				return false;
			}

			var parameters = subject.GetParameters();
			var otherParameters = other.GetParameters();
			if (parameters.Length != otherParameters.Length)
			{
				return false;
			}

			using (var enumerator1 = parameters.AsEnumerable().GetEnumerator())
			{
				using (var enumerator2 = otherParameters.AsEnumerable().GetEnumerator())
				{
					while (enumerator1.MoveNext() && enumerator2.MoveNext())
					{
						var param = enumerator1.Current;
						var otherParam = enumerator2.Current;

						if (param.ParameterType != otherParam.ParameterType)
						{
							return false;
						}
					}
				}
			}

			return true;
		}

		#endregion extension methods
	}
}