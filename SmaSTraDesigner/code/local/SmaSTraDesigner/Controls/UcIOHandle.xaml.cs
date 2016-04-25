namespace SmaSTraDesigner.Controls
{
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
	/// Interaction logic for UcIOHandle.xaml
	/// </summary>
	public partial class UcIOHandle : UserControl
	{
		#region dependency properties

		/// <summary>
		/// Registration of IsInput Dependency Property.
		/// </summary>
		public static readonly DependencyProperty IsInputProperty = 
			DependencyProperty.Register("IsInput", typeof(bool), typeof(UcIOHandle));

		#endregion dependency properties

		#region constructors

		public UcIOHandle()
		{
			this.InitializeComponent();
		}

		#endregion constructors

		#region properties

		/// <summary>
		/// Gets or sets the value of the IsInput property.
		/// TODO: (PS) Comment this.
		/// This is a Dependency Property.
		/// </summary>
		public bool IsInput
		{
			get { return (bool)this.GetValue(IsInputProperty); }
			set { this.SetValue(IsInputProperty, value); }
		}

		#endregion properties
	}
}