namespace SmaSTraDesigner.Controls
{
	using System;
	using System.Collections.Generic;
	using System.Linq;
	using System.Runtime.Serialization.Json;
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

	using SmaSTraDesigner.BusinessLogic;

	/// <summary>
	/// Interaction logic for UcDataSourceViewer.xaml
	/// </summary>
	public partial class UcDataSourceViewer : UcNodeViewer
	{
		#region constructors

		/// <summary>
		/// Initializes a new instance of the <see cref="UcDataSourceViewer"/> class.
		/// </summary>
		public UcDataSourceViewer()
		{
			this.InitializeComponent();
		}

		#endregion constructors
	}
}