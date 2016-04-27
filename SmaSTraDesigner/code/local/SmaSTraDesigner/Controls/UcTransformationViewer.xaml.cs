﻿namespace SmaSTraDesigner.Controls
{
	using BusinessLogic;
	using Common.Resources.Converters;
	using System;
	using System.Collections.Generic;
	using System.Linq;
	using System.Text;
	using System.Threading.Tasks;
	using System.Windows;
	using System.Windows.Controls;
	using System.Windows.Data;
	using System.Windows.Documents;
	using System.Windows.Input;
	using System.Windows.Media;
	using System.Windows.Media.Imaging;
	using System.Windows.Navigation;
	using System.Windows.Shapes;

	/// <summary>
	/// Interaction logic for UcTransformationViewer.xaml
	/// </summary>
	public partial class UcTransformationViewer : UcNodeViewer
	{
		#region constructors

		public UcTransformationViewer()
		{
			this.InitializeComponent();

			((LambdaConverter)this.FindResource("InputItemsSourceConverter")).ConvertMethod = (value, targetType, parameter, culture) =>
			{
				IEnumerable<DataType> inputTypes = (IEnumerable<DataType>)value;

				return inputTypes.Select((t, i) => new Tuple<DataType, int>(t, i)).ToArray();
			};
		}

		#endregion constructors
	}
}