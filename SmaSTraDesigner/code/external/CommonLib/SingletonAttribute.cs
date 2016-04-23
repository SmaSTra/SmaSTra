namespace Common
{
	using System;

	// TODO: (PS) Comment this.
	[AttributeUsage(AttributeTargets.Class, Inherited = true, AllowMultiple = false)]
	public sealed class SingletonAttribute : Attribute
	{
		#region properties

		public Func<object> CreateInstanceFunction
		{
			get;
			set;
		}

		public Func<object, bool> GetIsInUseCallback
		{
			get;
			set;
		}

		public bool UseWeakInstanceReference
		{
			get;
			set;
		}

		#endregion properties
	}
}