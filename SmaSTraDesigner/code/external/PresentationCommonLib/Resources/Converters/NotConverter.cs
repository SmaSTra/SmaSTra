namespace Common.Resources.Converters
{
	using System;
	using System.Globalization;
	using System.Windows.Data;

	// TODO: (PS) Comment this.
	[ValueConversion(typeof(bool), typeof(bool))]
	[Singleton(UseWeakInstanceReference = true)]
	public class NotConverter : IValueConverter
	{
		#region static properties

		public static NotConverter Instance
		{
			get { return Singleton<NotConverter>.Instance; }
		}

		#endregion static properties

		#region methods

		public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
		{
			if (value is bool)
			{
				return !(bool)value;
			}

			return value;
		}

		public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
		{
			return this.Convert(value, targetType, parameter, culture);
		}

		#endregion methods
	}
}