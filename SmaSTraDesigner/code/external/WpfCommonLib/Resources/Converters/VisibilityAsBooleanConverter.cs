namespace Common.Resources.Converters
{
	using System;
	using System.Globalization;
	using System.Windows;
	using System.Windows.Data;

	#region Enumerations

	public enum VisibilityAsBooleanConverterOptions
	{
		None = 0,
		UseHidden = 0x1,
		Negate = 0x2,
		CombineWithAnd = 0x4,
		CombineWithOr = 0x8
	}

	#endregion Enumerations

	// TODO: (PS) Comment this.
	[Singleton(UseWeakInstanceReference = true)]
	[ValueConversion(typeof(Visibility), typeof(bool))]
	public class VisibilityAsBooleanConverter : IValueConverter, IMultiValueConverter
	{
		#region static properties

		public static VisibilityAsBooleanConverter Instance
		{
			get { return Singleton<VisibilityAsBooleanConverter>.Instance; }
		}

		#endregion static properties

		#region static methods

		private static bool? GetBool(object value)
		{
			if (value is bool)
			{
				return (bool)value;
			}
			else if (value is Visibility)
			{
				return (Visibility)value == Visibility.Visible;
			}
			else
			{
				return null;
			}
		}

		private static VisibilityAsBooleanConverterOptions GetOptions(object parameter)
		{
			VisibilityAsBooleanConverterOptions? options = parameter as VisibilityAsBooleanConverterOptions?;
			if (options != null)
			{
				return options.Value;
			}
			else
			{
				return VisibilityAsBooleanConverterOptions.None;
			}
		}

		private static Visibility GetVisibility(bool boolValue, VisibilityAsBooleanConverterOptions options)
		{
			if (boolValue)
			{
				return Visibility.Visible;
			}
			else if (options.HasFlag(VisibilityAsBooleanConverterOptions.UseHidden))
			{
				return Visibility.Hidden;
			}
			else
			{
				return Visibility.Collapsed;
			}
		}

		#endregion static methods

		#region fields

		private MultiValueLogicalConverter logicalConverter = MultiValueLogicalConverter.Instance;

		#endregion fields

		#region methods

		public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
		{
			bool? b = GetBool(value);
			if (b != null)
			{
				VisibilityAsBooleanConverterOptions options = GetOptions(parameter);

				if (options.HasFlag(VisibilityAsBooleanConverterOptions.Negate))
				{
					b = !b.Value;
				}

				return GetVisibility(b.Value, options);
			}

			return null;
		}

		public object Convert(object[] values, Type targetType, object parameter, CultureInfo culture)
		{
			VisibilityAsBooleanConverterOptions options = GetOptions(parameter);
			object[] input = new object[values.Length];
			for (int i = 0; i < values.Length; i++)
			{
				bool? b = GetBool(values[i]);
				if (b != null)
				{
					input[i] = b.Value;
				}
				else
				{
					return null;
				}
			}

			LogicalConversionOptions logicalOptions = LogicalConversionOptions.None;
			if (options.HasFlag(VisibilityAsBooleanConverterOptions.CombineWithAnd))
			{
				logicalOptions |= LogicalConversionOptions.CombineWithAnd;
			}
			else
			{
				logicalOptions |= LogicalConversionOptions.CombineWithOr;
			}

			if (options.HasFlag(VisibilityAsBooleanConverterOptions.Negate))
			{
				logicalOptions |= LogicalConversionOptions.Negate;
			}

			bool boolean = (bool)this.logicalConverter.Convert(input, null, logicalOptions, culture);

			return GetVisibility(boolean, options);
		}

		public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
		{
			if (value is Visibility)
			{
				return (Visibility)value == Visibility.Visible;
			}

			return null;
		}

		public object[] ConvertBack(object value, Type[] targetTypes, object parameter, CultureInfo culture)
		{
			throw new NotSupportedException();
		}

		#endregion methods
	}
}