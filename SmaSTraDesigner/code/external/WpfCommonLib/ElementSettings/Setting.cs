namespace Common.ElementSettings
{
	using System;
	using System.ComponentModel;
	using System.Linq;
	using System.Windows;
	using System.Windows.Data;

	using Common.ExtensionMethods;

	#region Delegates

	// TODO: (PS) Comment this.
	public delegate object SourceSelectorCallback(Setting setting);

	// TODO: (PS) Comment this.
	public delegate void ValueSetterCallback(Setting setting, object writingBindingResult, object newValue);

	#endregion Delegates

	// TODO: (PS) Comment this.
	public class Setting : SettingBase, INotifyPropertyChanged
	{
		#region constants

		public static readonly object IgnoreValue = new object();

		#endregion constants

		#region static methods

		public static string GetBindingIdentifyerString(BindingBase binding)
		{
			if (binding == null)
			{
				throw new ArgumentNullException("binding");
			}

			Binding bindingAsBinding;
			MultiBinding bindingAsMultiBinding;
			if ((bindingAsBinding = binding as Binding) != null)
			{
				string elementName;
				if (bindingAsBinding.ElementName != null)
				{
					elementName = bindingAsBinding.ElementName;
				}
				else if (bindingAsBinding.RelativeSource != null)
				{
					elementName = String.Format("{{RelativeSource Mode={0}, AncestorLevel={1}, AncestorType={2}}}",
						bindingAsBinding.RelativeSource.Mode,
						bindingAsBinding.RelativeSource.AncestorLevel,
						bindingAsBinding.RelativeSource.AncestorType);
				}
				else if (bindingAsBinding.Source != null)
				{
					elementName = String.Concat('{', bindingAsBinding.Source.ToString(), '}');
				}
				else
				{
					elementName = "Self";
				}

				return String.Format("{{Binding Source={0}, Path={1}}}", elementName, bindingAsBinding.Path.Path);
			}
			else if ((bindingAsMultiBinding = binding as MultiBinding) != null)
			{
				return String.Format("{{MultiBinding {0}}}", String.Join(", ", bindingAsMultiBinding.Bindings.Select(b => GetBindingIdentifyerString(b))));
			}

			throw new NotSupportedException(String.Format("Binding type {0} not supported.", binding.GetType()));
		}

		#endregion static methods

		#region dependency properties

		/// <summary>
		/// Registration of Value Dependency Property.
		/// </summary>
		public static readonly DependencyProperty ValueProperty = 
			DependencyProperty.Register(
				"Value", typeof(object), typeof(Setting),
				new FrameworkPropertyMetadata(
					null,
					OnValueChanged,
					CoerceValue));

		#endregion dependency properties

		#region dependency property callbacks

		/// <summary>
		/// Coerce Callback method of the Value Dependency Property.
		/// </summary>
		/// <param name="sender">The instance of the class of which the Value property is about to change.</param>
		/// <param name="value">The proposed new value for the Value property.</param>
		/// <returns>The actual new value for the Value property.</returns>
		private static object CoerceValue(DependencyObject sender, object value)
		{
			Setting subject = (Setting)sender;
			if (value == Setting.IgnoreValue || value == DependencyProperty.UnsetValue)
			{
				return subject.Value;
			}

			return value;
		}

		/// <summary>
		/// Property Changed Callback method of the Value Dependency Property.
		/// </summary>
		/// <param name="sender">The instance of the class that had the Value property changed.</param>
		/// <param name="e">The <see cref="System.Windows.DependencyPropertyChangedEventArgs"/> instance containing the event data.</param>
		private static void OnValueChanged(DependencyObject sender, DependencyPropertyChangedEventArgs e)
		{
			Setting subject = (Setting)sender;

			if (!subject.propertyChangeMonitor.IsInside && subject.Owner != null && subject.Owner.SettingsFile != null)
			{
				subject.Owner.SettingsFile.SetValue(subject.Owner.Name, subject.Name, e.NewValue);
				if (e.NewValue != null)
				{
					subject.Owner.SettingsFile.AddValueType(e.NewValue.GetType());
				}
				if (subject.Owner.SaveAfterEachPropertyChange)
				{
					subject.Owner.Save();
				}
			}
		}

		#endregion dependency property callbacks

		#region fields

		/// <summary>
		/// TODO: (PS) Comment this.
		/// </summary>
		private BindingBase binding = null;
		private BindingBase modifiedBinding = null;

		#endregion fields

		#region constructors

		public Setting()
		{
		}

		public Setting(BindingBase binding, BindingBase writingBinding = null, ValueSetterCallback valueSetterCallback = null)
		{
			if (binding == null)
			{
				throw new ArgumentNullException("binding");
			}

			this.Binding = binding;
			this.WritingBinding = writingBinding;
			this.ValueSetterCallback = valueSetterCallback;
		}

		#endregion constructors

		#region events

		public event PropertyChangedEventHandler PropertyChanged;

		#endregion events

		#region properties

		/// <summary>
		/// Gets or sets the Binding property value.
		/// TODO: (PS) Comment this.
		/// </summary>
		public BindingBase Binding
		{
			get
			{
				return this.binding;
			}
			set
			{
				if (value != this.binding)
				{
					BindingBase oldValue = this.binding;
					this.binding = value;
					this.OnBindingChanged(oldValue, value);
				}
			}
		}

		// TODO: (PS) Comment this.
		public SourceSelectorCallback SourceSelectorCallback
		{
			get;
			set;
		}

		/// <summary>
		/// Gets or sets the value of the Value property.
		/// TODO: (PS) Comment this.
		/// This is a Dependency Property.
		/// </summary>
		public object Value
		{
			get { return (object)this.GetValue(ValueProperty); }
			set { this.SetValue(ValueProperty, value); }
		}

		public ValueSetterCallback ValueSetterCallback
		{
			get;
			set;
		}

		// TODO: (PS) Comment this.
		public BindingBase WritingBinding
		{
			get;
			set;
		}

		#endregion properties

		#region overrideable methods

		/// <summary>
		/// Called when the Binding property changed its value.
		/// </summary>
		/// <param name="oldValue">The old value.</param>
		/// <param name="newValue">The new value.</param>
		protected virtual void OnBindingChanged(BindingBase oldValue, BindingBase newValue)
		{
			this.OnPropertyChanged("Binding");
		}

		protected override void InitializeOverride()
		{
			if (String.IsNullOrWhiteSpace(this.Name) && this.Binding != null)
			{
				this.Name = GetBindingIdentifyerString(this.Binding);
			}

			BindingOperations.ClearBinding(this, ValueProperty);

			if (this.Binding != null && this.FrameworkElement != null && !String.IsNullOrWhiteSpace(this.Name))
			{
				this.modifiedBinding = this.GetModifiedBinding(this.Binding);

				if (this.WritingBinding != null)
				{
					this.modifiedBinding.SetMode(BindingMode.OneWay);
					this.WritingBinding = this.GetModifiedBinding(this.WritingBinding);
				}
				else
				{
					this.modifiedBinding.SetMode(BindingMode.TwoWay);
				}

				BindingOperations.SetBinding(this, ValueProperty, this.modifiedBinding);
				this.ReadValue();
			}
		}

		protected override void ReadValueOverride()
		{
			object value = this.Owner.SettingsFile.GetValue(this.Owner.Name, this.Name);
			if (value != Setting.IgnoreValue && value != DependencyProperty.UnsetValue)
			{
				if (this.WritingBinding != null)
				{
					if (this.ValueSetterCallback != null)
					{
						this.ValueSetterCallback(this, this.WritingBinding.GetValueFromSource(), value);
					}
					else
					{
						this.WritingBinding.SetValueForSource(value);
					}
				}
				else
				{
					this.Value = value;
				}
			}
		}

		#endregion overrideable methods

		#region methods

		private BindingBase GetModifiedBinding(BindingBase binding)
		{
			Binding bindingAsBinding;
			MultiBinding bindingAsMultiBinding;
			if ((bindingAsBinding = binding as Binding) != null)
			{
				if (bindingAsBinding.ElementName != null)
				{
					return bindingAsBinding.Clone(this.FrameworkElement.FindName(bindingAsBinding.ElementName));
				}
				else if (bindingAsBinding.RelativeSource != null)
				{
					return bindingAsBinding.Clone(bindingAsBinding.RelativeSource.Resolve(this.FrameworkElement));
				}
				else if (bindingAsBinding.Source != null)
				{
					return bindingAsBinding.Clone();
				}
				else if (this.SourceSelectorCallback != null)
				{
					return bindingAsBinding.Clone(this.SourceSelectorCallback(this));
				}
				else
				{
					return bindingAsBinding.Clone(this.FrameworkElement);
				}
			}
			else if ((bindingAsMultiBinding = binding as MultiBinding) != null)
			{
				var bindings = bindingAsMultiBinding.Bindings.ToList();
				MultiBinding result = bindingAsMultiBinding.Clone();
				result.Bindings.Clear();
				result.AddBindings(bindings.Select(b => this.GetModifiedBinding(b)));

				return result;
			}

			throw new NotSupportedException(String.Format("Binding type {0} not supported.", binding.GetType()));
		}

		private void OnPropertyChanged(string propertyName)
		{
			if (this.PropertyChanged != null)
			{
				this.PropertyChanged(this, new PropertyChangedEventArgs(propertyName));
			}
		}

		#endregion methods
	}
}