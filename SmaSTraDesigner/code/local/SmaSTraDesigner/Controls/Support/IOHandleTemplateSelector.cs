namespace SmaSTraDesigner.Controls.Support
{
	using System;
	using System.Collections.Generic;
	using System.Linq;
	using System.Text;
	using System.Threading.Tasks;
	using System.Windows;
	using System.Windows.Controls;

	public class IOHandleTemplateSelector : DataTemplateSelector
	{
		#region overrideable methods

		public override DataTemplate SelectTemplate(object item, DependencyObject container)
		{
			return base.SelectTemplate(item, container);
		}

		#endregion overrideable methods
	}
}