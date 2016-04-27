namespace SmaSTraDesigner.Controls
{
	using System;
	using System.Collections.Generic;
	using System.Linq;

	using BusinessLogic;

	using Common.Resources.Converters;

	/// <summary>
	/// Represents a Transformation from the TransformationTree on the GUI
	/// </summary>
	public partial class UcTransformationViewer : UcNodeViewer
	{
		#region constructors

		public UcTransformationViewer()
		{
			this.InitializeComponent();

			// initialize converter for the ItemsControl that shows the input handles.
			((LambdaConverter)this.FindResource("InputItemsSourceConverter")).ConvertMethod = (value, targetType, parameter, culture) =>
			{
				IEnumerable<DataType> inputTypes = (IEnumerable<DataType>)value;

				// Inclde index of the input, so the UcIOHandle's InputIndex property can use it.
				return inputTypes.Select((t, i) => new Tuple<DataType, int>(t, i)).ToArray();
			};
		}

		#endregion constructors
	}
}