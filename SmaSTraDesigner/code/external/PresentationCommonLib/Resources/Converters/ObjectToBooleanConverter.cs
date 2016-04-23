namespace Common.Resources.Converters
{
	using System;
	using System.Globalization;
	using System.Windows.Data;

	// TODO: (PS) Comment this.
	[ValueConversion(typeof(object), typeof(bool))]
	[Singleton(UseWeakInstanceReference = true)]
	public class ObjectToBooleanConverter : IValueConverter
	{
		#region static properties

		public static ObjectToBooleanConverter Instance
		{
			get { return Singleton<ObjectToBooleanConverter>.Instance; }
		}

		#endregion static properties

		#region methods

		public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
		{
			return value != null;
		}

		public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
		{
			throw new NotSupportedException();
		}

		#endregion methods
	}
}