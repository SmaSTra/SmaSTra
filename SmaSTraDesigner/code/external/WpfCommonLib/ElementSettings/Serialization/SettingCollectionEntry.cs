namespace Common.ElementSettings.Serialization
{
	using System;
	using System.Collections.Generic;
	using System.Linq;
	using System.Runtime.Serialization;

	// TODO: (PS) Comment this.
	[Serializable]
	public class SettingCollectionEntry : ISerializable
	{
		#region properties

		public string Name
		{
			get;
			set;
		}

		public List<SettingEntry> Settings
		{
			get;
			set;
		}

		#endregion properties

		#region methods

		void ISerializable.GetObjectData(SerializationInfo info, StreamingContext context)
		{
			info.SetType(this.GetType());
			info.AddValue("Name", this.Name);
			info.AddValue("Settings", this.Settings);
		}

		#endregion methods
	}
}