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
    using Controls;

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

        #region command handlers

        private void Save_CanExecute(object sender, CanExecuteRoutedEventArgs e)
        {
            e.CanExecute = true;
        }
        private void Save_Executed(object sender, ExecutedRoutedEventArgs e)
        {
            this.tdTreeDesigner.Tree.saveToFile();
        }

        private void Load_CanExecute(object sender, CanExecuteRoutedEventArgs e)
        {
            e.CanExecute = true;
        }
        private void Load_Executed(object sender, ExecutedRoutedEventArgs e)
        {
            this.tdTreeDesigner.Tree.loadFromFile();
        }

        private void Exit_CanExecute(object sender, CanExecuteRoutedEventArgs e)
        {
            e.CanExecute = true;
        }
        private void Exit_Executed(object sender, ExecutedRoutedEventArgs e)
        {
            App.Current.Shutdown();
        }

        private void Generate_CanExecute(object sender, CanExecuteRoutedEventArgs e)
        {
            e.CanExecute = true;
        }
        private void Generate_Executed(object sender, ExecutedRoutedEventArgs e)
        {
            foreach(Node con in this.tdTreeDesigner.Tree.Nodes)
            {
                Transformation nodeAsTrans = con as Transformation;
                if (nodeAsTrans != null)
                {
                    foreach (Node child in nodeAsTrans.InputNodes)
                    {
                        System.Diagnostics.Debug.Print("++++ nodeAsTrans: " + nodeAsTrans.Name + "InputNode: " + child.Name);
                    }
                } else
                {
                    DataSource nodeAsSource = con as DataSource;
                    if (nodeAsSource != null)
                    {
                        System.Diagnostics.Debug.Print("++++ nodeAsSource: " + nodeAsSource.Name );
                    }
                }
            }
            this.tdTreeDesigner.Tree.createJava();
        }

        private void Delete_CanExecute(object sender, CanExecuteRoutedEventArgs e)
        {
            if (e.OriginalSource as UcNodeViewer != null)
            {
                UcNodeViewer nodeViewer = (UcNodeViewer)e.OriginalSource;
                if (nodeViewer.IsLibrary)
                {
                    e.CanExecute = true;
                }
                else if (nodeViewer.IsPreview || !nodeViewer.IsSelected)
                {
                    e.CanExecute = false;
                }
                else
                {
                    e.CanExecute = true;
                }
                e.Handled = true;
            }
            else
            {
                e.CanExecute = true;
            }
        }
        private void Delete_Executed(object sender, ExecutedRoutedEventArgs e)
        {
            UcNodeViewer sourceAsNodeViewer = e.OriginalSource as UcNodeViewer;
            if (sourceAsNodeViewer != null && sourceAsNodeViewer.IsLibrary)
            {
                Singleton<Library>.Instance.removeLibraryNode((UcNodeViewer)e.OriginalSource);
            }
            else
            {
                this.tdTreeDesigner.onDeleteCommand();
            }
        }

        private void SelectConnected_CanExecute(object sender, CanExecuteRoutedEventArgs e)
        {
            if (e.OriginalSource as UcNodeViewer != null)
            {
                UcNodeViewer nodeViewer = (UcNodeViewer)e.OriginalSource;
                if (nodeViewer.IsPreview)
                {
                    e.CanExecute = false;
                }
                else
                {
                    e.CanExecute = true;
                }
                e.Handled = true;
            } else
            {

            }
        }
        private void SelectConnected_Executed(object sender, ExecutedRoutedEventArgs e)
        {
            this.tdTreeDesigner.onNodeViewerDoubleClick((UcNodeViewer)e.OriginalSource);
        }

        private void AddSelected_CanExecute(object sender, CanExecuteRoutedEventArgs e)
        {
            if (e.OriginalSource as UcNodeViewer != null)
            {
                UcNodeViewer nodeViewer = (UcNodeViewer)e.OriginalSource;
                if (nodeViewer.IsPreview)
                {
                    e.CanExecute = false;
                }
                else
                {
                    e.CanExecute = true;
                }
                e.Handled = true;
            } else
            {

            }
        }
        private void AddSelected_Executed(object sender, ExecutedRoutedEventArgs e)
        {
            this.tdTreeDesigner.onNodeViewerSelectAdded((UcNodeViewer) e.OriginalSource);
        }

        private void ToOutputViewer_CanExecute(object sender, CanExecuteRoutedEventArgs e)
        {
            e.CanExecute = true;
        }
        private void ToOutputViewer_Executed(object sender, ExecutedRoutedEventArgs e)
        {
            this.tdTreeDesigner.outOutputViewer.BringIntoView();
        }

        private void AddToLibrary_CanExecute(object sender, CanExecuteRoutedEventArgs e)
        {
            if (e.OriginalSource as UcNodeViewer != null)
            {
                UcNodeViewer nodeViewer = (UcNodeViewer)e.OriginalSource;
                if (nodeViewer.IsLibrary)
                {
                    e.CanExecute = false;
                }
                else
                {
                    e.CanExecute = true;
                }
                e.Handled = true;
            }
            else
            {

            }
        }
        private void AddToLibrary_Executed(object sender, ExecutedRoutedEventArgs e)
        {
            Singleton<Library>.Instance.addLibraryNode((Node)((UcNodeViewer)e.OriginalSource).Node.Clone());
        }

        #endregion command handlers

    }
}