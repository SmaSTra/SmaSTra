namespace Common.ElementSettings
{
	using System;
	using System.Globalization;
	using System.Windows;
	using System.Windows.Data;

	using Common.ExtensionMethods;
	using Common.Resources.Converters;

	using DPoint = System.Drawing.Point;

	using Rectangle = System.Drawing.Rectangle;

	using Screen = System.Windows.Forms.Screen;

	// TODO: (PS) Comment this.
	public class WindowStateSetting : CompountSetting
	{
		#region constants

		private const int TOLERANCE_X = 64;
		private const int TOLERANCE_Y = 10;

		#endregion constants

		#region static constructor

		/// <summary>
		/// Initializes the <see cref="WindowStateSetting"/> class.
		/// </summary>
		static WindowStateSetting()
		{
			OwnerProperty.OverrideMetadata(typeof(WindowStateSetting), new FrameworkPropertyMetadata(null, OnOwnerChanged));
			NameProperty.OverrideMetadata(typeof(WindowStateSetting), new FrameworkPropertyMetadata(GetName(), null, (sender, value) => GetName()));
		}

		#endregion static constructor

		#region static methods

		private static string GetName()
		{
			return typeof(WindowStateSetting).Name;
		}

		#endregion static methods

		#region dependency property callbacks

		/// <summary>
		/// Property Changed Callback method of the Owner Dependency Property.
		/// </summary>
		/// <param name="sender">The instance of the class that had the Owner property changed.</param>
		/// <param name="e">The <see cref="System.Windows.DependencyPropertyChangedEventArgs"/> instance containing the event data.</param>
		private static void OnOwnerChanged(DependencyObject sender, DependencyPropertyChangedEventArgs e)
		{
			WindowStateSetting subject = (WindowStateSetting)sender;
			SettingCollection newValue = (SettingCollection)e.NewValue;
			SettingCollection oldValue = (SettingCollection)e.OldValue;

			if (newValue != null)
			{
				newValue.Initializing += subject.SettingCollection_Initializing;
				newValue.Initialized += subject.SettingCollection_Initialized;
			}
			if (oldValue != null)
			{
				oldValue.Initializing -= subject.SettingCollection_Initializing;
				newValue.Initialized -= subject.SettingCollection_Initialized;
			}
		}

		#endregion dependency property callbacks

		#region fields

		private int[] positionAndSizeBackup = null;

		#endregion fields

		#region constructors

		/// <summary>
		/// Initializes a new instance of the <see cref="WindowStateSetting"/> class.
		/// </summary>
		public WindowStateSetting()
		{
			this.AddSetting(new Setting(new Binding("WindowState")
			{
				Converter = new LambdaConverter()
				{
					ConvertMethod = this.WindowStateConversion,
					ConvertBackMethod = this.WindowStateConversion
				}
			}));

			WindowSizeConverter windowSizeConverter = new WindowSizeConverter();
			this.AddWindowSizeBinding(windowSizeConverter, "Left", "Left");
			this.AddWindowSizeBinding(windowSizeConverter, "Top", "Top");
			this.AddWindowSizeBinding(windowSizeConverter, "ActualWidth", "Width");
			this.AddWindowSizeBinding(windowSizeConverter, "ActualHeight", "Height");
		}

		#endregion constructors

		#region methods

		public object WindowStateConversion(object value, Type targetType, object parameter, CultureInfo culture)
		{
			WindowState state = (WindowState)value;
			Window window = (Window)this.Owner.Owner;
			if (state == WindowState.Minimized || window.WindowState == WindowState.Minimized)
			{
				return Setting.IgnoreValue;
			}

			return state;
		}

		private void AddWindowSizeBinding(WindowSizeConverter windowSizeConverter, string sizeReadParameterName, string sizeWriteParameterName)
		{
			this.AddSetting(new Setting(new MultiBinding()
			{
				Converter = windowSizeConverter
			}.AddBindings(
				new Binding(sizeReadParameterName), new Binding("WindowState")),
				new Binding(sizeWriteParameterName)
				{
					Converter = new LambdaConverter()
					{
						ConvertMethod = (value, targetType, parameter, culture) =>
						{
							Window window = (Window)this.Owner.Owner;
							if (window.WindowState != WindowState.Normal)
							{
								return Setting.IgnoreValue;
							}

							return value;
						}
					}
				}));
		}

		#endregion methods

		#region event handlers

		private void SettingCollection_Initialized(object sender, System.EventArgs e)
		{
			SettingCollection subject = (SettingCollection)sender;
			Window window = (Window)subject.Owner;

			// Restore the original window position and size in case the window is outside the desktop working area.
			int left = (int)window.Left;
			int top = (int)window.Top;
			int width = (int)window.ActualWidth;
			int height = (int)window.ActualHeight;
			Screen screen = Screen.FromRectangle(new Rectangle(left, top, width, height));
			if (screen == null ||
				!(screen.WorkingArea.Contains(new DPoint(left + TOLERANCE_X, top + TOLERANCE_Y)) ||
				screen.WorkingArea.Contains(new DPoint(left + width - TOLERANCE_X, top + TOLERANCE_Y))))
			{
				window.Left = this.positionAndSizeBackup[0];
				window.Top = this.positionAndSizeBackup[1];
				window.Width = this.positionAndSizeBackup[2];
				window.Height = this.positionAndSizeBackup[3];
			}

			this.positionAndSizeBackup = null;
		}

		private void SettingCollection_Initializing(object sender, System.EventArgs e)
		{
			SettingCollection subject = (SettingCollection)sender;
			Window window = subject.Owner as Window;
			if (window == null)
			{
				throw new InvalidOperationException(String.Format("The {0}'s owner must be a Window.", typeof(WindowStateSetting).Name));
			}

			// Back up the original window position and size in case the restored one is not valid.
			this.positionAndSizeBackup = new int[]
			{
				(int)window.Left,
				(int)window.Top,
				(int)window.ActualWidth,
				(int)window.ActualHeight
			};
		}

		#endregion event handlers

		#region nested types

		private class WindowSizeConverter : IMultiValueConverter
		{
			#region methods

			public object Convert(object[] values, Type targetType, object parameter, CultureInfo culture)
			{
				double size = (double)values[0];
				WindowState windowState = (WindowState)values[1];
				if (windowState != WindowState.Normal)
				{
					return Setting.IgnoreValue;
				}

				return size;
			}

			public object[] ConvertBack(object value, Type[] targetTypes, object parameter, CultureInfo culture)
			{
				throw new NotImplementedException();
			}

			#endregion methods
		}

		#endregion nested types
	}
}