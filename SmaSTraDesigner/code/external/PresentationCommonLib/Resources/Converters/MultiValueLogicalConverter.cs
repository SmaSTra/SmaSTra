namespace Common.Resources.Converters
{
	using System;
	using System.Globalization;
	using System.Linq;
	using System.Windows.Data;

	#region Enumerations

	public enum LogicalConversionOptions
	{
		None = 0,
		Negate = 0x1,
		CombineWithAnd = 0x2,
		CombineWithOr = 0x4
	}

	#endregion Enumerations

	// TODO: (PS) Comment this.
	[Singleton(UseWeakInstanceReference = true)]
	public class MultiValueLogicalConverter : IMultiValueConverter
	{
		#region static properties

		public static MultiValueLogicalConverter Instance
		{
			get { return Singleton<MultiValueLogicalConverter>.Instance; }
		}

		#endregion static properties

		#region methods

		public object Convert(object[] values, Type targetType, object parameter, CultureInfo culture)
		{
			if (values.Length == 0 || values.Any(obj => obj as bool? == null))
			{
				return null;
			}

			LogicalConversionOptions options;
			if (parameter is LogicalConversionOptions)
			{
				options = (LogicalConversionOptions)parameter;
			#if DEBUG
				if (!(options.HasFlag(LogicalConversionOptions.CombineWithAnd) ||
					options.HasFlag(LogicalConversionOptions.CombineWithOr)))
				{
					throw new ArgumentException("paramter has no information about how to conbine the values.", "parameter");
				}
			#endif
			}
			else
			{
				options = LogicalConversionOptions.CombineWithAnd;
			}

			var enumerator = values.Cast<bool>().GetEnumerator();
			enumerator.MoveNext();
			bool result = enumerator.Current;

			while (!this.IsTerminatingValue(result, options) && enumerator.MoveNext())
			{
				result = this.Combine(result, enumerator.Current, options);
			}

			if (options.HasFlag(LogicalConversionOptions.Negate))
			{
				result = !result;
			}

			return result;
		}

		public object[] ConvertBack(object value, Type[] targetTypes, object parameter, CultureInfo culture)
		{
			throw new NotSupportedException();
		}

		private bool Combine(bool value1, bool value2, LogicalConversionOptions options)
		{
			if (options.HasFlag(LogicalConversionOptions.CombineWithAnd))
			{
				return value1 && value2;
			}
			else
			{
				return value1 || value2;
			}
		}

		private bool IsTerminatingValue(bool value, LogicalConversionOptions options)
		{
			if (options.HasFlag(LogicalConversionOptions.CombineWithAnd))
			{
				return !value;
			}
			else
			{
				return value;
			}
		}

		#endregion methods
	}
}