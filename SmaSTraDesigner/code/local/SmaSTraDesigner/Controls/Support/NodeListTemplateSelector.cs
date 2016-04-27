namespace SmaSTraDesigner.Controls.Support
{
	using System.Windows;
	using System.Windows.Controls;

	using SmaSTraDesigner.BusinessLogic;

	/// <summary>
	/// Template Selector that is used in conjunction with an ItemsControl to choose the correct
	/// control to display a given node.
	/// </summary>
	/// <seealso cref="System.Windows.Controls.DataTemplateSelector" />
	public class NodeListTemplateSelector : DataTemplateSelector
	{
		#region overrideable methods

		/// <summary>
		/// Returns a <see cref="T:System.Windows.DataTemplate" /> based on what type of node is displayed.
		/// </summary>
		/// <param name="item">The data object for which to select the template.</param>
		/// <param name="container">The data-bound object.</param>
		/// <returns>
		/// Returns a <see cref="T:System.Windows.DataTemplate" /> or null. The default value is null.
		/// </returns>
		public override DataTemplate SelectTemplate(object item, DependencyObject container)
		{
			FrameworkElement element = (FrameworkElement)container;
			if (item is Transformation)
			{
				return element.FindResource("TransformationNodeTemplate") as DataTemplate;
			}
			else if (item is OutputNode)
			{
				return element.FindResource("OutputNodeTemplate") as DataTemplate;
			}
			else if (item is DataSource)
			{
				return element.FindResource("DataSourceNodeTemplate") as DataTemplate;
			}

			return base.SelectTemplate(item, container);
		}

		#endregion overrideable methods
	}
}