namespace SmaSTraDesigner.Controls.Support
{
	using System;
	using System.Collections.Generic;
	using System.Linq;
	using System.Text;
	using System.Threading.Tasks;
	using System.Windows;
	using System.Windows.Controls;

	using SmaSTraDesigner.BusinessLogic;

	public class NodeListTemplateSelector : DataTemplateSelector
	{
		#region overrideable methods

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