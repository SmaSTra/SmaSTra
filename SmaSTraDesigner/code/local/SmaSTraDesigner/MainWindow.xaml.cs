namespace SmaSTraDesigner
{
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

	using Common;

	using SmaSTraDesigner.BusinessLogic;

	/// <summary>
	/// Interaction logic for MainWindow.xaml
	/// </summary>
	public partial class MainWindow : Window
	{
		#region constructors

		public MainWindow()
		{
			this.InitializeComponent();
			this.spnNodeClasses.DataContext = Singleton<ClassManager>.Instance;
            this.spnProperties.DataContext = Singleton<NodeProperties>.Instance;
            this.spnLibrary.DataContext = Singleton<Library>.Instance;
        }

		#endregion constructors

		#region event handlers

		private void mniExit_Click(object sender, RoutedEventArgs e)
		{
			App.Current.Shutdown();
		}
 
        private void mniSave_Click(object sender, RoutedEventArgs e)
        {
            this.tdTreeDesigner.Tree.saveToFile();
        }

        private void mniLoad_Click(object sender, RoutedEventArgs e)
        {
            this.tdTreeDesigner.Tree.loadFromFile();
        }

        private void mniGenerateJava_Click(object sender, RoutedEventArgs e)
		{
			this.tdTreeDesigner.Tree.createJava();
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

        private void spnLibrary_Drop(object sender, DragEventArgs e)
        {
            Singleton<Library>.Instance.Library_Drop(sender, e);
        }

        private void spnLibrary_DragEnter(object sender, DragEventArgs e)
        {
            Singleton<Library>.Instance.Library_DragEnter(sender, e);
        }

        #endregion event handlers

        #region test area



        #endregion test area

    }
}