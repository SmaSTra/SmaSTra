namespace Common.ExtensionMethods
{
	using System.Windows;
	using System.Windows.Data;

	public static class DependencyObjectExtensions
	{
		#region extension methods

		public static void ClearBinding(this DependencyObject subject, DependencyProperty dependencyProperty)
		{
			BindingOperations.ClearBinding(subject, dependencyProperty);
		}

		public static BindingExpressionBase SetBinding(this DependencyObject subject, DependencyProperty dependencyProperty, BindingBase binding)
		{
			return BindingOperations.SetBinding(subject, dependencyProperty, binding);
		}

		#endregion extension methods
	}
}