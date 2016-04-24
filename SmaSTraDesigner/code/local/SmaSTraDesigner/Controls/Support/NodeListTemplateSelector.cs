using SmaSTraDesigner.BusinessLogic;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;

namespace SmaSTraDesigner.Controls.Support
{
	public class NodeListTemplateSelector : DataTemplateSelector
	{
		public override DataTemplate SelectTemplate(object item, DependencyObject container)
		{
			FrameworkElement element = (FrameworkElement)container;
			NodeClass nodeClass = item as NodeClass;
			if (nodeClass != null)
			{
				if (nodeClass.BaseNode is Transformation)
				{
					return element.FindResource("TransformationNodeTemplate") as DataTemplate;
				}
				else if (nodeClass.BaseNode is OutputNode)
				{
					return element.FindResource("OutputNodeTemplate") as DataTemplate;
				}
				else if (nodeClass.BaseNode is DataSource)
				{
					return element.FindResource("DataSourceNodeTemplate") as DataTemplate;
				}
			}

			return base.SelectTemplate(item, container);
		}
	}
}
