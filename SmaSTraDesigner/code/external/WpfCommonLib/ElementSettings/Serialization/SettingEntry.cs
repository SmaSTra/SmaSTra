namespace Common.ElementSettings.Serialization
{
	using System;
	using System.Runtime.Serialization;

	// TODO: (PS) Comment this.
	[Serializable]
	public class SettingEntry : ISerializable
	{
		#region properties

		public string Name
		{
			get;
			set;
		}

		public object Value
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
			info.AddValue("Value", this.Value);
		}

		#endregion methods
	}
}