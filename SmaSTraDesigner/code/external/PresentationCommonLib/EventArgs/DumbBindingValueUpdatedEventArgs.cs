namespace Common.EventArgs
{
	// TODO: (PS) Comment this.
	public class DumbBindingValueUpdatedEventArgs : System.EventArgs
	{
		#region constructors

		public DumbBindingValueUpdatedEventArgs(object oldValue, object newValue)
		{
			this.OldValue = oldValue;
			this.NewValue = newValue;
		}

		#endregion constructors

		#region properties

		public object NewValue
		{
			get;
			private set;
		}

		public object OldValue
		{
			get;
			private set;
		}

		#endregion properties
	}
}