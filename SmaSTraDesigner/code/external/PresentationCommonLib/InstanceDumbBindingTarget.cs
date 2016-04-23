namespace Common
{
	using System;
	using System.Diagnostics;
	using System.Reflection;
	using System.Windows;

	// TODO: (PS) Comment this.
	public class InstanceDumbBindingTarget : DumbBindingTarget
	{
		#region fields

		private int? hashCode = null;
		private EventInfo propertyChangedEvent;

		/// <summary>
		/// TODO: (PS) Comment this.
		/// </summary>
		private WeakReference weakTargetObjectReference = null;

		#endregion fields

		#region constructors

		public InstanceDumbBindingTarget(object targetObject, string targetPropertyName, bool read, bool write, object[] targetPropertyIndex)
			: base(targetObject, null, targetPropertyName, read, write, BindingFlags.Instance, targetPropertyIndex)
		{
			this.hashCode = base.GetHashCode();
		}

		#endregion constructors

		#region overrideable properties

		public override object PropertyValue
		{
			get
			{
				object targetObject = this.TargetObject;
				if (targetObject != null)
				{
					return this.TargetProperty.GetValue(targetObject, this.TargetPropertyIndex);
				}
				else
				{
					return DependencyProperty.UnsetValue;
				}
			}
			set
			{
				object targetObject = this.TargetObject;
				if (targetObject != null)
				{
					try
					{
						object valueToSet = value;
						if (value != null)
						{
							Type valueType = value.GetType();
							if (!(this.TargetProperty.PropertyType.IsGenericType &&
								this.TargetProperty.PropertyType.GetGenericTypeDefinition() == typeof(Nullable<>) &&
								Nullable.GetUnderlyingType(this.TargetProperty.PropertyType) == valueType) &&
								this.TargetProperty.PropertyType != value.GetType())
							{
								valueToSet = Convert.ChangeType(value, this.TargetProperty.PropertyType);
							}
						}
						
						if (!(valueToSet == null && this.TargetProperty.PropertyType.IsPrimitive))
						{
							this.TargetProperty.SetValue(targetObject, valueToSet, this.TargetPropertyIndex);
						}
					}
					catch (Exception ex)
					{
						Debug.WriteLine("Property value could not be set.\n{0}", ex);
					}
				}
			}
		}

		/// <summary>
		/// Gets or sets the TargetObject weak reference.
		/// (Getter returns null if reference is no longer alive.)
		/// TODO: (PS) Comment this.
		/// </summary>
		public override object TargetObject
		{
			get
			{
				if (this.weakTargetObjectReference == null || !this.weakTargetObjectReference.IsAlive)
				{
					return null;
				}
				else
				{
					return this.weakTargetObjectReference.Target;
				}
			}
			protected set
			{
				if (value != null)
				{
					this.weakTargetObjectReference = new WeakReference(value);
				}
				else
				{
					this.weakTargetObjectReference = null;
				}
			}
		}

		#endregion overrideable properties

		#region overrideable methods

		public override void Dispose()
		{
			object targetObject = this.TargetObject;
			if (targetObject != null && this.propertyChangedEvent != null)
			{
				this.propertyChangedEvent.RemoveEventHandler(targetObject, WeakEventHandler.GetHandler(this.Target_PropertyChanged));
			}
		}

		public override int GetHashCode()
		{
			return this.hashCode.Value;
		}

		protected override void AddPropertyChangedEventHandler(EventInfo propertyChangedEvent)
		{
			this.propertyChangedEvent = propertyChangedEvent;
			object targetObject = this.TargetObject;
			if (targetObject != null)
			{
				propertyChangedEvent.AddEventHandler(targetObject, WeakEventHandler.GetHandler(this.Target_PropertyChanged));
			}
		}

		#endregion overrideable methods
	}
}