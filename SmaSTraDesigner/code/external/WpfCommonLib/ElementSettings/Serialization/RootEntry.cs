namespace Common.ElementSettings.Serialization
{
	using System;
	using System.Collections.Generic;
	using System.Runtime.Serialization;

	// TODO: (PS) Comment this.
	[Serializable]
	public class RootEntry : ISerializable
	{
		#region constructors

		public RootEntry()
		{
			this.Collections = new List<SettingCollectionEntry>();
		}

		#endregion constructors

		#region properties

		public List<SettingCollectionEntry> Collections
		{
			get;
			set;
		}

		public string[] ValueTypeNames
		{
			get;
			set;
		}

		#endregion properties

		#region methods

		void ISerializable.GetObjectData(SerializationInfo info, StreamingContext context)
		{
			info.SetType(this.GetType());
			info.AddValue("Collections", this.Collections);
			info.AddValue("ValueTypeNames", this.ValueTypeNames);
		}

		#endregion methods
	}
}