namespace Common
{
	using System.Windows;

	// TODO: (PS) Comment this.
	public class DependencyPropertyProvider : DependencyPropertyProvider<object>
	{
	}

	// TODO: (PS) Comment this.
	public class DependencyPropertyProvider<T> : DependencyObject
	{
		#region dependency properties

		/// <summary>
		/// Registration of PropertyValue Dependency Property.
		/// </summary>
		public static readonly DependencyProperty PropertyValueProperty = 
			DependencyProperty.Register(
				"PropertyValue", typeof(T), typeof(DependencyPropertyProvider<T>),
				new FrameworkPropertyMetadata(
					default(T),
					OnPropertyValueChanged,
					CoercePropertyValue));

		#endregion dependency properties

		#region dependency property callbacks

		/// <summary>
		/// Coerce Callback method of the PropertyValue Dependency Property.
		/// </summary>
		/// <param name="sender">The instance of the class of which the PropertyValue property is about to change.</param>
		/// <param name="value">The proposed new value for the PropertyValue property.</param>
		/// <returns>The actual new value for the PropertyValue property.</returns>
		private static object CoercePropertyValue(DependencyObject sender, object value)
		{
			DependencyPropertyProvider<T> subject = (DependencyPropertyProvider<T>)sender;
			if (subject.CoerceValueCallback != null)
			{
				value = subject.CoerceValueCallback(sender, value);
			}

			return value;
		}

		/// <summary>
		/// Property Changed Callback method of the PropertyValue Dependency Property.
		/// </summary>
		/// <param name="sender">The instance of the class that had the PropertyValue property changed.</param>
		/// <param name="e">The <see cref="System.Windows.DependencyPropertyChangedEventArgs"/> instance containing the event data.</param>
		private static void OnPropertyValueChanged(DependencyObject sender, DependencyPropertyChangedEventArgs e)
		{
			DependencyPropertyProvider<T> subject = (DependencyPropertyProvider<T>)sender;
			if (subject.PropertyChangedCallback != null)
			{
				subject.PropertyChangedCallback(sender, e);
			}
		}

		#endregion dependency property callbacks

		#region properties

		// TODO: (PS) Comment this.
		public CoerceValueCallback CoerceValueCallback
		{
			get;
			set;
		}

		// TODO: (PS) Comment this.
		public PropertyChangedCallback PropertyChangedCallback
		{
			get;
			set;
		}

		/// <summary>
		/// Gets or sets the value of the PropertyValue property.
		/// TODO: (PS) Comment this.
		///	This is a Dependency Property.
		/// </summary>
		public T PropertyValue
		{
			get { return (T)this.GetValue(PropertyValueProperty); }
			set { this.SetValue(PropertyValueProperty, value); }
		}

		#endregion properties
	}
}