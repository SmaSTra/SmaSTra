namespace Common.ExtensionMethods
{
	using System;
	using System.Collections;
	using System.Reflection;

	/// <summary>
	/// Contains definitions of Extension Methods for objects of type "object".
	/// </summary>
	public static class ObjectExtensions
	{
		#region extension methods

		// TODO: (PS) Comment this.
		public static T Cast<T>(this object subject)
		{
			return (T)subject;
		}

		// TODO: (PS) Comment this.
		public static T Manipulate<T>(this T subject, Action<T> action)
		{
			if (action == null)
			{
				throw new ArgumentNullException("action");
			}

			action(subject);

			return subject;
		}

		// TODO: (PS) Comment this.
		public static T MemberwiseClone<T>(this T subject)
		{
			if (subject == null)
			{
				throw new ArgumentNullException("subject");
			}

			IList list = subject as IList;
			if (list != null)
			{
				T clone = (T)Activator.CreateInstance(subject.GetType());
				((IList)clone).AddRange(list);

				return clone;
			}
			else
			{
				return (T)subject.GetType().GetMethod("MemberwiseClone", BindingFlags.Instance | BindingFlags.NonPublic).Invoke(subject, null);
			}
		}

		#endregion extension methods
	}
}