namespace SmaSTraDesigner
{
    using System;
    using System.Collections.Generic;
    using System.Windows;
    using System.Windows.Input;

    using Common;

    using SmaSTraDesigner.BusinessLogic;
    using Controls;
    using Controls.Support;
    using BusinessLogic.codegeneration.loader;
    using System.IO;
    using BusinessLogic.online;
    using System.Diagnostics;

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
			//var toggleButtons = LayoutHelper.FindAllLogicalChildren<ToggleButton>(this.spnSideMenu);
			//foreach (var toggleButton in toggleButtons)
			//{
			//	if (toggleButton != sender)
			//	{
			//		toggleButton.IsChecked = false;
			//	}
			//}
        }

        private void spnLibrary_Drop(object sender, DragEventArgs e)
        {
            Singleton<Library>.Instance.Library_Drop(sender, e);
        }

        private void spnLibrary_DragEnter(object sender, DragEventArgs e)
        {
            Singleton<Library>.Instance.Library_DragEnter(sender, e);
        }
        
        private void InputType_PreviewTextInput(object sender, TextCompositionEventArgs e)
        {
            Singleton<NodeProperties>.Instance.onTextBoxInput(sender, e);
        }

        #endregion event handlers

        #region command handlers

        private void DebugTest_CanExecute(object sender, CanExecuteRoutedEventArgs e)
        {
            e.CanExecute = true;
        }
        private void DebugTest_Executed(object sender, ExecutedRoutedEventArgs e)
        {            
            // executed with "Ctrl+T". Put anything that shall be tested here. a command is less intrusive than a "debug test button" on the GUI
            foreach (Node node in this.tdTreeDesigner.Tree.Nodes)
            {
                Console.WriteLine("++++++ node.Configuration.Count: " + node.Configuration.Count);
            }
        }

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
            This.Close();
           // App.Current.Shutdown();
        }

        private void Generate_CanExecute(object sender, CanExecuteRoutedEventArgs e)
        {
            e.CanExecute = true;
        }
        private void Generate_Executed(object sender, ExecutedRoutedEventArgs e)
        {
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
            Singleton<Library>.Instance.addLibraryNode((Node)((UcNodeViewer)e.OriginalSource).Node.Class.generateNode());
        }


        private void Merge_CanExecute(object sender, CanExecuteRoutedEventArgs e)
        {
            e.CanExecute = this.tdTreeDesigner.CanMergeCurrentSelection();
        }


        private void Merge_Execute(object sender, ExecutedRoutedEventArgs e)
        {
            this.tdTreeDesigner.TryMergeCurrentSelection();
        }


        private void Unmerge_CanExecute(object sender, CanExecuteRoutedEventArgs e)
        {
            e.CanExecute = this.tdTreeDesigner.CanUnmerge();
        }


        private void Unmerge_Execute(object sender, ExecutedRoutedEventArgs e)
        {
            this.tdTreeDesigner.TryUnmergeSelectedNode();
        }

        private void CreateCustomElement_CanExecute(object sender, CanExecuteRoutedEventArgs e)
        {
            e.CanExecute = true;
        }
        // moved to Ctrl+N
        private void CreateCustomElement_Executed(object sender, ExecutedRoutedEventArgs e)
        { 
            DialogCreateCustomElement dialogNewElement = new DialogCreateCustomElement();
            if (dialogNewElement.ShowDialog() == true)
            {
                string newElementName = dialogNewElement.ElementName;
                List<DataType> inputTypes = dialogNewElement.InputTypes;
                DataType outputType = dialogNewElement.OutputType;
                string methodCode = dialogNewElement.MethodCode;

                AbstractNodeClass generatedClass = dialogNewElement.GenerateClassFromInputs();
                Singleton<NodeLoader>.Instance.saveToFolder(generatedClass, Path.Combine("created", generatedClass.Name), dialogNewElement.MethodCode);
                Singleton<ClassManager>.Instance.AddClass(generatedClass);
            }
        }

        private void SwitchWorkspace_CanExecute(object sender, CanExecuteRoutedEventArgs e)
        {
            e.CanExecute = true;
        }

        private void SwitchWorkspace_Executed(object sender, ExecutedRoutedEventArgs e)
        {
            var dialog = new System.Windows.Forms.FolderBrowserDialog();
            if (dialog.ShowDialog() == System.Windows.Forms.DialogResult.OK)
            {
                //Switch workspace:
                App.SwitchWorkspace(dialog.SelectedPath, this.tdTreeDesigner);
            }
        }

        #endregion command handlers

        private void This_Closing(object sender, System.ComponentModel.CancelEventArgs e)
        {
            DialogClosingApplication closingDialog = new DialogClosingApplication();
            if(closingDialog.ShowDialog() == true)
            {
                if (closingDialog.YesClicked)
                {
                    e.Cancel = false;
                }
                else
                {
                    e.Cancel = true;
                }
            }
        }
    }
}