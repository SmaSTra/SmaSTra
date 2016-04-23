namespace Common.ElementSettings
{
	using System;
	using System.Reflection;
	using System.Windows;
	using System.Windows.Data;

	// TODO: (PS) Comment this.
	public abstract class SettingBase : DependencyObject
	{
		#region dependency properties

		/// <summary>
		/// Registration of FrameworkElement Dependency Property.
		/// </summary>
		private static readonly DependencyProperty FrameworkElementProperty = 
			DependencyProperty.Register("FrameworkElement", typeof(FrameworkElement), typeof(SettingBase));

		/// <summary>
		/// Registration of Name Dependency Property.
		/// </summary>
		public static readonly DependencyProperty NameProperty = 
			DependencyProperty.Register("Name", typeof(string), typeof(SettingBase));

		/// <summary>
		/// Registration of Owner Dependency Property.
		/// </summary>
		public static readonly DependencyProperty OwnerProperty = 
			DependencyProperty.Register(
				"Owner", typeof(SettingCollection), typeof(SettingBase),
				new FrameworkPropertyMetadata(
					null,
					OnOwnerChanged));

		/// <summary>
		/// Registration of ValueType Dependency Property.
		/// </summary>
		public static readonly DependencyProperty ValueTypeProperty = 
			DependencyProperty.Register(
				"ValueType", typeof(Type), typeof(SettingBase),
				new FrameworkPropertyMetadata(
					null,
					OnValueTypeChanged));

		#endregion dependency properties

		#region dependency property callbacks

		/// <summary>
		/// Property Changed Callback method of the Owner Dependency Property.
		/// </summary>
		/// <param name="sender">The instance of the class that had the Owner property changed.</param>
		/// <param name="e">The <see cref="System.Windows.DependencyPropertyChangedEventArgs"/> instance containing the event data.</param>
		private static void OnOwnerChanged(DependencyObject sender, DependencyPropertyChangedEventArgs e)
		{
			SettingBase subject = (SettingBase)sender;
			SettingCollection newValue = (SettingCollection)e.NewValue;
			SettingCollection oldValue = (SettingCollection)e.OldValue;

			if (newValue != null && subject.ValueType != null)
			{
				newValue.SettingsFile.AddValueType(subject.ValueType);
			}
		}

		/// <summary>
		/// Property Changed Callback method of the ValueType Dependency Property.
		/// </summary>
		/// <param name="sender">The instance of the class that had the ValueType property changed.</param>
		/// <param name="e">The <see cref="System.Windows.DependencyPropertyChangedEventArgs"/> instance containing the event data.</param>
		private static void OnValueTypeChanged(DependencyObject sender, DependencyPropertyChangedEventArgs e)
		{
			SettingBase subject = (SettingBase)sender;
			Type newValue = (Type)e.NewValue;
			Type oldValue = (Type)e.OldValue;

			if (subject.Owner != null)
			{
				subject.Owner.SettingsFile.AddValueType(newValue);
			}
		}

		#endregion dependency property callbacks

		#region fields

		protected BlockMonitor propertyChangeMonitor = new BlockMonitor(true);
		private Func<object[], bool> deferringEventCallback = null;
		private Tuple<object, EventInfo> deferringEventData = null;
		private Func<PropertyChangedCallbackArgs, bool> deferringPropertyChangeCallback = null;
		private PropertyChangedHandle deferringPropertyChangeHandle = null;
		private bool initializationDeferred = false;
		private bool initialized = false;

		#endregion fields

		#region constructors

		/// <summary>
		/// Initializes a new instance of the <see cref="SettingBase"/> class.
		/// </summary>
		public SettingBase()
		{
			BindingOperations.SetBinding(this, FrameworkElementProperty, new Binding("Owner.Owner")
			{
				Source = this
			});
		}

		#endregion constructors

		#region properties

		/// <summary>
		/// Gets the value of the FrameworkElement property.
		/// TODO: (PS) Comment this.
		/// This is a Dependency Property.
		/// </summary>
		public FrameworkElement FrameworkElement
		{
			get { return (FrameworkElement)this.GetValue(FrameworkElementProperty); }
			private set { this.SetValue(FrameworkElementProperty, value); }
		}

		/// <summary>
		/// Gets or sets the value of the Name property.
		/// TODO: (PS) Comment this.
		/// This is a Dependency Property.
		/// </summary>
		public string Name
		{
			get { return (string)this.GetValue(NameProperty); }
			set { this.SetValue(NameProperty, value); }
		}

		/// <summary>
		/// Gets or sets the value of the Owner property.
		/// TODO: (PS) Comment this.
		/// This is a Dependency Property.
		/// </summary>
		public SettingCollection Owner
		{
			get { return (SettingCollection)this.GetValue(OwnerProperty); }
			set { this.SetValue(OwnerProperty, value); }
		}

		/// <summary>
		/// Gets or sets the value of the ValueType property.
		/// TODO: (PS) Comment this.
		/// This is a Dependency Property.
		/// </summary>
		public Type ValueType
		{
			get { return (Type)this.GetValue(ValueTypeProperty); }
			set { this.SetValue(ValueTypeProperty, value); }
		}

		#endregion properties

		#region overrideable methods

		protected abstract void InitializeOverride();

		protected abstract void ReadValueOverride();

		#endregion overrideable methods

		#region methods

		public void DeferInitialization()
		{
			if (!this.initialized)
			{
				this.initializationDeferred = true;
			}
		}

		public void DeferInitialization(Binding binding, Func<PropertyChangedCallbackArgs, bool> deferringPropertyChangeCallback = null)
		{
			if (binding == null)
			{
				throw new ArgumentNullException("binding");
			}

			if (!(this.initialized || this.initializationDeferred))
			{
				this.initializationDeferred = true;
				this.deferringPropertyChangeCallback = deferringPropertyChangeCallback;
				this.deferringPropertyChangeHandle = PropertyChangedHandle.GetDistinctInstance(binding, this.OnDefferingPropertyChangeOcurred);
			}
		}

		public void DeferInitialization(object source, EventInfo eventInfo, Func<object[], bool> deferringEventCallback = null)
		{
			if (eventInfo == null)
			{
				throw new ArgumentNullException("eventInfo");
			}

			if (!(this.initialized || this.initializationDeferred))
			{
				this.initializationDeferred = true;
				this.deferringEventData = new Tuple<object, EventInfo>(source, eventInfo);
				this.deferringEventCallback = deferringEventCallback;
				GenericAction.AddGenericEventHandler(source, eventInfo, this.OnDefferingEventOcurred);
			}
		}

		public void InitializeAfterDeferrence()
		{
			if (this.initializationDeferred)
			{
				this.initializationDeferred = false;
				this.Initialize();
			}
		}

		public void ReadValue()
		{
			if (this.Owner != null && this.Owner.Name != null && this.Owner.SettingsFile != null && this.Name != null)
			{
				using (var monitor = this.propertyChangeMonitor.Enter())
				{
					this.ReadValueOverride();
				}
			}
		}

		internal void Initialize()
		{
			if (!this.initializationDeferred)
			{
				using (var monitor = this.propertyChangeMonitor.Enter())
				{
					this.InitializeOverride();
				}
			}

			this.initialized = true;
		}

		private void OnDefferingEventOcurred(object[] args)
		{
			if (this.deferringEventCallback == null || this.deferringEventCallback(args))
			{
				GenericAction.RemoveGenericEventHandler(this.deferringEventData.Item1, this.deferringEventData.Item2, this.OnDefferingEventOcurred);
				this.deferringEventData = null;
				this.deferringEventCallback = null;
				this.InitializeAfterDeferrence();
			}
		}

		private void OnDefferingPropertyChangeOcurred(PropertyChangedCallbackArgs args)
		{
			if (this.deferringPropertyChangeCallback == null || this.deferringPropertyChangeCallback(args))
			{
				args.Handle.Dispose();
				this.deferringPropertyChangeHandle = null;
				this.deferringPropertyChangeCallback = null;
				this.InitializeAfterDeferrence();
			}
		}

		#endregion methods
	}
}