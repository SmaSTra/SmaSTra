namespace Common.ElementSettings
{
	using System;
	using System.Windows;

	// TODO: (PS) Comment this.
	public static class SettingsHelper
	{
		#region extension methods

		/// <summary>
		/// Property getter method of Settings Dependency Property.
		/// TODO: (PS) Comment this.
		/// </summary>
		/// <param name="subject">The subject.</param>
		/// <returns>The current value of the Settings property.</returns>
		public static SettingCollection GetSettings(this FrameworkElement subject)
		{
			return (SettingCollection)subject.GetValue(SettingsProperty);
		}

		/// <summary>
		/// Property setter method of Settings Dependency Property.
		/// TODO: (PS) Comment this.
		/// </summary>
		/// <param name="subject">The subject.</param>
		/// <param name="value">The supposed value of the Settings property.</param>
		public static void SetSettings(this FrameworkElement subject, SettingCollection value)
		{
			subject.SetValue(SettingsProperty, value);
		}

		#endregion extension methods

		#region dependency properties

		/// <summary>
		/// Registration of Settings Dependency Property.
		/// </summary>
		public static readonly DependencyProperty SettingsProperty = 
			DependencyProperty.RegisterAttached(
				"Settings", typeof(SettingCollection), typeof(SettingsHelper),
				new FrameworkPropertyMetadata(
					null,
					OnSettingsChanged));

		#endregion dependency properties

		#region dependency property callbacks

		/// <summary>
		/// Property Changed Callback method of the Settings Dependency Property.
		/// </summary>
		/// <param name="sender">The instance of the class that had the Settings property changed.</param>
		/// <param name="e">The <see cref="System.Windows.DependencyPropertyChangedEventArgs"/> instance containing the event data.</param>
		private static void OnSettingsChanged(DependencyObject sender, DependencyPropertyChangedEventArgs e)
		{
			FrameworkElement subject = (FrameworkElement)sender;
			SettingCollection newValue = (SettingCollection)e.NewValue;
			SettingCollection oldValue = (SettingCollection)e.OldValue;

			if (newValue != null)
			{
				if (newValue.Owner != null)
				{
					throw new InvalidOperationException("You cannot assign the same settings to two different framework elements.");
				}

				newValue.Owner = subject;
			}

			if (oldValue != null)
			{
				oldValue.Owner = null;
			}
		}

		#endregion dependency property callbacks
	}
}