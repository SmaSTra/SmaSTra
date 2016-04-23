namespace Common
{
	using System;
	using System.ComponentModel;
	using System.Reflection;

	// TODO: (PS) Comment this.
	public class StaticDumbBindingTarget : DumbBindingTarget
	{
		#region fields

		private EventInfo propertyChangedEvent = null;
		private Type targetType = null;

		#endregion fields

		#region constructors

		public StaticDumbBindingTarget(Type targetType, string targetPropertyName, bool read, bool write, object[] targetPropertyIndex)
			: base(null, targetType, targetPropertyName, read, write, BindingFlags.Static, targetPropertyIndex)
		{
			this.targetType = targetType;
		}

		#endregion constructors

		#region overrideable properties

		public override object PropertyValue
		{
			get
			{
				return this.TargetProperty.GetValue(null, this.TargetPropertyIndex);
			}
			set
			{
				this.TargetProperty.SetValue(null, value, this.TargetPropertyIndex);
			}
		}

		// TODO: (PS) Comment this.
		public override object TargetObject
		{
			get
			{
				return this.targetType;
			}
			protected set
			{
				this.targetType = value as Type;
			}
		}

		#endregion overrideable properties

		#region overrideable methods

		public override void Dispose()
		{
			if (this.propertyChangedEvent != null)
			{
				this.propertyChangedEvent.RemoveEventHandler(null, new PropertyChangedEventHandler(this.Target_PropertyChanged));
				this.propertyChangedEvent = null;
			}
		}

		protected override void AddPropertyChangedEventHandler(EventInfo propertyChangedEvent)
		{
			this.propertyChangedEvent = propertyChangedEvent;
			propertyChangedEvent.AddEventHandler(null, new PropertyChangedEventHandler(this.Target_PropertyChanged));
		}

		#endregion overrideable methods
	}
}