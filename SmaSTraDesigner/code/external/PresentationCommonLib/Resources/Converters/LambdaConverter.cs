namespace Common.Resources.Converters
{
	using System;
	using System.Globalization;
	using System.Windows.Data;

	#region Delegates

	public delegate object ConvertMethod(object value, Type targetType, object parameter, CultureInfo culture);

	public delegate object[] MultiConvertBackMethod(object value, Type[] targetTypes, object parameter, CultureInfo culture);

	public delegate object MultiConvertMethod(object[] values, Type targetType, object parameter, CultureInfo culture);

	#endregion Delegates

	// TODO: (PS) Comment this.
	public class LambdaConverter : IValueConverter, IMultiValueConverter
	{
		#region properties

		// TODO: (PS) Comment this.
		public ConvertMethod ConvertBackMethod
		{
			get;
			set;
		}

		// TODO: (PS) Comment this.
		public ConvertMethod ConvertMethod
		{
			get;
			set;
		}

		// TODO: (PS) Comment this.
		public MultiConvertBackMethod MultiConvertBackMethod
		{
			get;
			set;
		}

		// TODO: (PS) Comment this.
		public MultiConvertMethod MultiConvertMethod
		{
			get;
			set;
		}

		#endregion properties

		#region methods

		public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
		{
			if (this.ConvertMethod == null)
			{
				return value;
			}

			return this.ConvertMethod(value, targetType, parameter, culture);
		}

		public object Convert(object[] values, Type targetType, object parameter, CultureInfo culture)
		{
			if (this.MultiConvertMethod == null)
			{
				throw new InvalidOperationException("MultiConvertMethod not defined.");
			}

			return this.MultiConvertMethod(values, targetType, parameter, culture);
		}

		public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
		{
			if (this.ConvertBackMethod == null)
			{
				return value;
			}

			return this.ConvertBackMethod(value, targetType, parameter, culture);
		}

		public object[] ConvertBack(object value, Type[] targetTypes, object parameter, CultureInfo culture)
		{
			if (this.MultiConvertBackMethod == null)
			{
				throw new InvalidOperationException("MultiConvertBackMethod not defined.");
			}

			return this.MultiConvertBackMethod(value, targetTypes, parameter, culture);
		}

		#endregion methods
	}
}