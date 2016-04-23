namespace Common.Resources.Converters
{
	using System;
	using System.Globalization;
	using System.Linq;
	using System.Windows;
	using System.Windows.Data;

	#region Enumerations

	public enum ThicknessSide
	{
		None = 0,
		Left = 1,
		Top = 2,
		Right = 4,
		Bottom = 8
	}

	#endregion Enumerations

	// TODO: (PS) Comment this.
	[ValueConversion(typeof(double), typeof(Thickness))]
	[Singleton(UseWeakInstanceReference = true)]
	public class ThicknessConverter : IValueConverter
	{
		#region static properties

		public static ThicknessConverter Instance
		{
			get { return Singleton<ThicknessConverter>.Instance; }
		}

		#endregion static properties

		#region methods

		public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
		{
			double? thickness = value as double?;
			ThicknessSide? mapping = parameter as ThicknessSide?;
			string stringMapping;
			if (mapping == null && (stringMapping = parameter as string) != null)
			{
				mapping = ThicknessSide.None;
				stringMapping = stringMapping.ToLower();
				if (stringMapping.Any(c => c == 'l'))
				{
					mapping |= ThicknessSide.Left;
				}
				if (stringMapping.Any(c => c == 't'))
				{
					mapping |= ThicknessSide.Top;
				}
				if (stringMapping.Any(c => c == 'r'))
				{
					mapping |= ThicknessSide.Right;
				}
				if (stringMapping.Any(c => c == 'b'))
				{
					mapping |= ThicknessSide.Bottom;
				}
			}

			if (thickness != null && mapping != null)
			{
				return new Thickness(
					mapping.Value.HasFlag(ThicknessSide.Left) ? thickness.Value : 0.0,
					mapping.Value.HasFlag(ThicknessSide.Top) ? thickness.Value : 0.0,
					mapping.Value.HasFlag(ThicknessSide.Right) ? thickness.Value : 0.0,
					mapping.Value.HasFlag(ThicknessSide.Bottom) ? thickness.Value : 0.0);
			}

			return null;
		}

		public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
		{
			Thickness? thickness = value as Thickness?;
			ThicknessSide? mapping = parameter as ThicknessSide?;
			if (thickness != null && mapping != null)
			{
				switch (mapping.Value)
				{
					default:
					case ThicknessSide.Left:
						return thickness.Value.Left;

					case ThicknessSide.Top:
						return thickness.Value.Top;

					case ThicknessSide.Right:
						return thickness.Value.Right;

					case ThicknessSide.Bottom:
						return thickness.Value.Bottom;
				}
			}

			return null;
		}

		#endregion methods
	}
}