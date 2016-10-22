using System;
using SmaSTraDesigner.BusinessLogic.classhandler.nodeclasses;
using SmaSTraDesigner.BusinessLogic.nodes;
using SmaSTraDesigner.BusinessLogic.savingloading;

namespace SmaSTraDesigner
{
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
    using BusinessLogic.config;
    using System.Windows.Controls.Primitives;
    using BusinessLogic.classhandler;
    using System.Reflection;
    using System.Windows.Controls;

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
            this.spnNodeTypeMenu.DataContext = Singleton<ClassManager>.Instance;
            this.spnProperties.DataContext = Singleton<NodeProperties>.Instance;
            this.spnLibrary.DataContext = Singleton<Library>.Instance;
            Online online = Singleton<Online>.Instance;
            online.MainWindow = this;
            this.spnOnlinePanel.DataContext = online;

            textVersionNumber.Text = "Version: " + Assembly.GetExecutingAssembly().GetName().Version;
        }

		#endregion constructors

		#region event handlers


		private void ToggleButton_Checked(object sender, RoutedEventArgs e)
		{
            var toggleButtons = LayoutHelper.FindAllLogicalChildren<ToggleButton>(this.spnNodeTypeMenu);
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
        
        private void InputType_PreviewTextInput(object sender, TextCompositionEventArgs e)
        {
            Singleton<NodeProperties>.Instance.onTextBoxInput(sender, e);
        }
        private void UpdateButton_Click(object sender, RoutedEventArgs e)
        {
            Singleton<Online>.Instance.UpdateButton_Click(sender, e);
        }

        private void listOnlineElements_SelectionChanged(object sender, RoutedEventArgs e)
        {
            Singleton<Online>.Instance.listOnlineElements_SelectionChanged(sender, e);
        }

        private void btnDownloadElement_Click(object sender, RoutedEventArgs e)
        {
            Singleton<Online>.Instance.btnDownloadElement_Click(sender, e);
        }

        private void uploadDropZone_Drop(object sender, DragEventArgs e)
        {
            Singleton<Online>.Instance.uploadDropZone_Drop(sender, e);
        }
        private void spnOnlinePanel_IsVisibleChanged(object sender, DependencyPropertyChangedEventArgs e)
        {
            Singleton<Online>.Instance.spnOnlinePanel_IsVisibleChanged(sender, e);
        }

        private void focusNodePropertieInput(int index)
        {
            if (icNodeClassInputTypes.Items.Count > 0 && index < icNodeClassInputTypes.Items.Count) {
                var itemContainer = icNodeClassInputTypes.ItemContainerGenerator.ContainerFromItem(icNodeClassInputTypes.Items[index]) as FrameworkElement;
                if (itemContainer != null)
                {
                    try
                    {
                        TextBox inputTextBox = icNodeClassInputTypes.ItemTemplate.FindName("tboxInputValue", itemContainer) as TextBox;
                        if (inputTextBox != null)
                        {
                            inputTextBox.Focus();
                        }
                    }
                    catch
                    {

                    }
                }
            }
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
            focusNodePropertieInput(0);
            throw new Exception();
        }

        private void New_CanExecute(object sender, CanExecuteRoutedEventArgs e)
        {
            e.CanExecute = true;
        }
        private void New_Executed(object sender, ExecutedRoutedEventArgs e)
        {
            this.tdTreeDesigner.Clear(true);
        }

        private void Save_CanExecute(object sender, CanExecuteRoutedEventArgs e)
        {
            e.CanExecute = true;
        }
        private void Save_Executed(object sender, ExecutedRoutedEventArgs e)
        {
            this.tdTreeDesigner.Tree.SaveToFile();
        }

        private void Load_CanExecute(object sender, CanExecuteRoutedEventArgs e)
        {
            e.CanExecute = true;
        }
        private void Load_Executed(object sender, ExecutedRoutedEventArgs e)
        {
            this.tdTreeDesigner.Tree.LoadFromFile();
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
            this.tdTreeDesigner.Tree.CreateJava();
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
                else if (nodeViewer.IsPreview)
                {
                    if (nodeViewer.Node.Class.UserCreated)
                    {
                        e.CanExecute = true;
                    } else
                    {
                        e.CanExecute = false;
                    }
                }
                else if (!nodeViewer.IsSelected)
                {
                    e.CanExecute = true;
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
            else if (sourceAsNodeViewer != null && sourceAsNodeViewer.IsPreview && !sourceAsNodeViewer.IsLibrary) {
                NodeBlacklist blacklist = Singleton<NodeBlacklist>.Instance;
                if (!blacklist.IsOnBlackList(sourceAsNodeViewer.Node.Class))
                {
                    blacklist.Add(sourceAsNodeViewer.Node.Class.Name);
                    Singleton<ClassManager>.Instance.Reload();
                }
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
            UcNodeViewer nodeViewer = (UcNodeViewer)e.OriginalSource;
            if (!nodeViewer.IsSelected)
            {
                this.tdTreeDesigner.onNodeViewerSelectAdded(nodeViewer);
            } else
            {
                this.tdTreeDesigner.onNodeViewerSelectRemoved(nodeViewer);
            }
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


        private void Merge_CanExecute(object sender, CanExecuteRoutedEventArgs e)
        {
            e.CanExecute = this.tdTreeDesigner.CanMergeCurrentSelection();
        }


        private void Merge_Executed(object sender, ExecutedRoutedEventArgs e)
        {
            this.tdTreeDesigner.TryMergeCurrentSelection(true);
        }


        private void Unmerge_CanExecute(object sender, CanExecuteRoutedEventArgs e)
        {
            e.CanExecute = this.tdTreeDesigner.CanUnmerge();
        }


        private void Unmerge_Executed(object sender, ExecutedRoutedEventArgs e)
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
                Singleton<NodeLoader>.Instance.saveToFolder(generatedClass, Path.Combine(WorkSpace.DIR, "created", generatedClass.Name), dialogNewElement.MethodCode);
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

        private void OnlineTransformations_CanExecute(object sender, CanExecuteRoutedEventArgs e)
        {
            e.CanExecute = true;
        }

        private void OnlineTransformations_Executed(object sender, ExecutedRoutedEventArgs e)
        {
            if (!DialogOnlineTransformatins.IsOpen)
            {
                DialogOnlineTransformatins onlineDialog = new DialogOnlineTransformatins(Singleton<OnlineServerLink>.Instance);
                onlineDialog.Show();
            }
        }



        private void Undo_CanExecute(object sender, CanExecuteRoutedEventArgs e)
        {
            if (tdTreeDesigner != null)
            {
                e.CanExecute = tdTreeDesigner.CanUndo();
            }
        }

        private void Undo_Executed(object sender, ExecutedRoutedEventArgs e)
        {
            tdTreeDesigner.Undo();
        }

        private void Redo_CanExecute(object sender, CanExecuteRoutedEventArgs e)
        {
            if (tdTreeDesigner != null)
            {
                e.CanExecute = tdTreeDesigner.CanRedo();
            }
        }

        private void Redo_Executed(object sender, ExecutedRoutedEventArgs e)
        {
            tdTreeDesigner.Redo();
        }

        private void PasteNode_CanExecute(object sender, CanExecuteRoutedEventArgs e)
        {
            e.CanExecute = this.tdTreeDesigner.SelectedNodeViewer != null;
        }


        private void PasteNode_Executed(object sender, ExecutedRoutedEventArgs e)
        {
            List<Node> nodeListToAdd = new List<Node>();
            List<UcNodeViewer> nodeViewerListToRemove = new List<UcNodeViewer>();
            foreach (UcNodeViewer nodeViewer in this.tdTreeDesigner.SelectedNodeViewers)
            { if (!(nodeViewer == null || nodeViewer.Node is OutputNode))
                {
                    Node pastedNode = nodeViewer.Node.Clone();
                    pastedNode.Name = nodeViewer.Node.Name;
                    pastedNode.PosX = nodeViewer.Node.PosX;
                    pastedNode.PosY = nodeViewer.Node.PosY + 10;
                    nodeListToAdd.Add(pastedNode);
                    nodeViewerListToRemove.Add(nodeViewer);
                }
            }
            foreach (UcNodeViewer nodeViewerToRemove in nodeViewerListToRemove)
            {
            this.tdTreeDesigner.onNodeViewerSelectRemoved(nodeViewerToRemove);
            }
            foreach (Node nodeToAdd in nodeListToAdd)
            {
                this.tdTreeDesigner.AddNode(nodeToAdd, false, true);
            }
            foreach (Node nodeToAdd in nodeListToAdd)
            {
                this.tdTreeDesigner.onNodeSelectAdded(nodeToAdd);
            }
        }

        private void About_CanExecute(object sender, CanExecuteRoutedEventArgs e)
        {
            e.CanExecute = true;
        }


        private void About_Executed(object sender, ExecutedRoutedEventArgs e)
        {
            new DialogAbout().Show();
        }

        private void OrganizeNodes_CanExecute(object sender, CanExecuteRoutedEventArgs e)
        {
            e.CanExecute = true;
        }
        
        private void OrganizeNodes_Executed(object sender, ExecutedRoutedEventArgs e)
        {
            tdTreeDesigner.OrganizeNodes();
        }

        private void CustomCode_CanExecute(object sender, CanExecuteRoutedEventArgs e)
        {
            if (e.OriginalSource as UcNodeViewer != null)
            {
                Node node = (e.OriginalSource as UcNodeViewer).Node;
                if (node.Class.UserCreated && !(node is CombinedNode))
                {
                    e.CanExecute = true;
                } else
                {
                    e.CanExecute = false;
                }
            }
            else
            {
                e.CanExecute = false;
            }
        }


        private void CustomCode_Executed(object sender, ExecutedRoutedEventArgs e)
        {
            if (e.OriginalSource as UcNodeViewer != null)
            {
                UcNodeViewer nodeViewer = (UcNodeViewer)e.OriginalSource;
                new DialogCustomCode(nodeViewer.Node).Show();
            }
        }

        private void FocusInputValue_CanExecute(object sender, CanExecuteRoutedEventArgs e)
        {
            UcIOHandle ioHandle = e.Parameter as UcIOHandle;
            if (ioHandle != null && ioHandle.NodeViewer != null && ioHandle.Node != null)
            {
                e.CanExecute = ioHandle.IsInput && !ioHandle.NodeViewer.IsPreview && Singleton<NodeProperties>.Instance.ActiveNode == ioHandle.Node;
            }
            else
            {
                e.CanExecute = false;
            }
        }

        private void FocusInputValue_Executed(object sender, ExecutedRoutedEventArgs e)
        {
            UcIOHandle ioHandle = e.Parameter as UcIOHandle;
            if(ioHandle != null)
            {
                if (Singleton<NodeProperties>.Instance.ActiveNode == ioHandle.Node) {
                    focusNodePropertieInput(ioHandle.InputIndex);
                }
            }
            e.Handled = false;
        }


        #endregion command handlers

        private void This_Closing(object sender, System.ComponentModel.CancelEventArgs e)
        {
            //Before closing, we try to save the current state:
            RegularSaver.Save(this.tdTreeDesigner);

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

        private void propagate_MouseWheel(object sender, MouseWheelEventArgs e)
        { // Prevents internal ScrollViewer from blocking outer ScrollViewer
            if (!e.Handled)
            {
                e.Handled = true;
                var propagateEvent = new MouseWheelEventArgs(e.MouseDevice, e.Timestamp, e.Delta);
                propagateEvent.RoutedEvent = UIElement.MouseWheelEvent;
                propagateEvent.Source = sender;
                var parent = ((FrameworkElement)sender).Parent as UIElement;
                parent.RaiseEvent(propagateEvent);
            }
        }

        private void btnFilterClear_Click(object sender, RoutedEventArgs e)
        {
            tboxNodeFilter.Text = "";
        }
    }
}