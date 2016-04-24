using Common;
using SmaSTraDesigner.BusinessLogic;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace SmaSTraDesigner
{
	/// <summary>
	/// Interaction logic for MainWindow.xaml
	/// </summary>
	public partial class MainWindow : Window
	{
		public MainWindow()
		{
			this.InitializeComponent();
			this.spnNodeClasses.DataContext = Singleton<ClassManager>.Instance;
		}

		private void mniExit_Click(object sender, RoutedEventArgs e)
		{
			App.Current.Shutdown();
		}

		private void ToggleButton_Checked(object sender, RoutedEventArgs e)
		{
			var toggleButtons = LayoutHelper.FindAllLogicalChildren<ToggleButton>(this.spnSideMenu);
			foreach (var toggleButton in toggleButtons)
			{
				if (toggleButton != sender)
				{
					toggleButton.IsChecked = false;
				}
			}
		}

		private void mniTestJavaGen_Click(object sender, RoutedEventArgs e)
		{
			new TransformationTree().test();
		}
	}
}
