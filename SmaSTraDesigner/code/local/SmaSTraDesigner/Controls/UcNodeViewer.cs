namespace SmaSTraDesigner.Controls
{
	using System;
	using System.Collections.Generic;
	using System.Linq;
	using System.Text;
	using System.Threading.Tasks;
	using System.Windows;
	using System.Windows.Controls;
	using System.Windows.Input;

	using Common;

	using SmaSTraDesigner.BusinessLogic;

	using Support;

	/// <summary>
	/// Base class for the different node viewers.
	/// Represents a node in the tree graph on the GUI.
	/// </summary>
	public class UcNodeViewer : UserControl
	{
		#region dependency properties

		/// <summary>
		/// Registration of IsInSelectionArea Dependency Property.
		/// </summary>
		public static readonly DependencyProperty IsInSelectionAreaProperty = 
			DependencyProperty.Register("IsInSelectionArea", typeof(bool), typeof(UcNodeViewer));

		/// <summary>
		/// Registration of IsPreview Dependency Property.
		/// </summary>
		public static readonly DependencyProperty IsPreviewProperty = 
			DependencyProperty.Register("IsPreview", typeof(bool), typeof(UcNodeViewer));

		/// <summary>
		/// Registration of IsSelected Dependency Property.
		/// </summary>
		public static readonly DependencyProperty IsSelectedProperty = 
			DependencyProperty.Register(
				"IsSelected", typeof(bool), typeof(UcNodeViewer),
				new FrameworkPropertyMetadata(
					false,
					OnIsSelectedChanged));

		#endregion dependency properties

		#region dependency property callbacks

		/// <summary>
		/// Property Changed Callback method of the IsSelected Dependency Property.
		/// </summary>
		/// <param name="sender">The instance of the class that had the IsSelected property changed.</param>
		/// <param name="e">The <see cref="System.Windows.DependencyPropertyChangedEventArgs"/> instance containing the event data.</param>
		private static void OnIsSelectedChanged(DependencyObject sender, DependencyPropertyChangedEventArgs e)
		{
			UcNodeViewer subject = (UcNodeViewer)sender;
			bool newValue = (bool)e.NewValue;
			bool oldValue = (bool)e.OldValue;

			if (newValue)
			{
				UcTreeDesigner treeDesigner = LayoutHelper.FindLogicalParent<UcTreeDesigner>(subject, true);
				if (treeDesigner != null)
				{
					treeDesigner.SelectedNodeViewer = subject;
				}
			}
		}

		#endregion dependency property callbacks

		#region fields

		/// <summary>
		/// The CustomDragDropHelper instance that helps with Drag&Drop functionality in this control.
		/// </summary>
		private CustomDragDropHelper customDragDropHelper;

		#endregion fields

		#region constructors

		/// <summary>
		/// Initializes a new instance of the <see cref="UcNodeViewer"/> class.
		/// </summary>
		public UcNodeViewer()
		{
			this.MouseLeftButtonDown += UcNodeViewer_MouseLeftButtonDown;
			this.MouseRightButtonDown += UcNodeViewer_MouseRightButtonDown;
			this.Loaded += UcNodeViewer_Loaded;
		}

		#endregion constructors

		#region events

		/// <summary>
		/// Is raised when this control is being dragged.
		/// </summary>
		public event EventHandler CustomDrag;

		#endregion events

		#region properties

		/// <summary>
		/// Gets the IO handles that belong to this node viewer.
		/// Is only set after this control is loaded.
		/// </summary>
		public UcIOHandle[] IoHandles
		{
			get;
			private set;
		}

		// TODO: (PS) Make this readonly.
		/// <summary>
		/// Gets or sets the value of the IsInSelectionArea property.
		/// States whether this control is in the selection rectangle on the tree designer at this time.
		/// This is a Dependency Property.
		/// </summary>
		public bool IsInSelectionArea
		{
			get { return (bool)this.GetValue(IsInSelectionAreaProperty); }
			set { this.SetValue(IsInSelectionAreaProperty, value); }
		}

		/// <summary>
		/// Gets or sets the value of the IsPreview property.
		/// States whether this control is used as a preview in the available nodes menu.
		/// This is a Dependency Property.
		/// </summary>
		public bool IsPreview
		{
			get { return (bool)this.GetValue(IsPreviewProperty); }
			set { this.SetValue(IsPreviewProperty, value); }
		}

		/// <summary>
		/// Gets or sets the value of the IsSelected property.
		/// States whether this control is selected on the tree designer.
		/// This is a Dependency Property.
		/// </summary>
		public bool IsSelected
		{
			get { return (bool)this.GetValue(IsSelectedProperty); }
			set { this.SetValue(IsSelectedProperty, value); }
		}

		/// <summary>
		/// Gets the Node that is represented by this control.
		/// </summary>
		public Node Node
		{
			get { return this.DataContext as Node; }
		}

		#endregion properties

		#region overrideable methods

		/// <summary>
		/// Returns a <see cref="System.String" /> that represents this instance.
		/// </summary>
		/// <returns>
		/// A <see cref="System.String" /> that represents this instance.
		/// </returns>
		public override string ToString()
		{
			return String.Format("{0} Node={1}", this.GetType().Name, this.Node);
		}

		#endregion overrideable methods

		#region methods

		/// <summary>
		/// Raises the <see cref="E:CustomDrag"/> event.
		/// </summary>
		protected void OnCustomDrag()
		{
			if (this.CustomDrag != null)
			{
				this.CustomDrag(this, null);
			}
		}

		/// <summary>
		/// Is called when this control is being clicked.
		/// Marks it as selected.
		/// </summary>
		/// <param name="e">The <see cref="MouseButtonEventArgs"/> instance containing the event data.</param>
		private void OnClick(MouseButtonEventArgs e)
		{
			if (!this.IsPreview)
			{
				e.Handled = true;
				this.IsSelected = true;
				// Deselect all handles if this node viewer itsself is clicked.
				foreach (var ioHandle in this.IoHandles)
				{
					ioHandle.IsSelected = false;
				}
			}
		}

		private void OnCustomDragDrop(MouseEventArgs e)
		{
			if (this.IsPreview)
			{
				DragDrop.DoDragDrop(this, new Tuple<Node>((Node)this.DataContext), DragDropEffects.All);
			}
			else
			{
				this.OnCustomDrag();
			}
		}

		#endregion methods

		#region event handlers

		/// <summary>
		/// Handles the Loaded event of this control.
		/// </summary>
		/// <param name="sender">The source of the event.</param>
		/// <param name="e">The <see cref="RoutedEventArgs"/> instance containing the event data.</param>
		private void UcNodeViewer_Loaded(object sender, RoutedEventArgs e)
		{
            if (!System.ComponentModel.DesignerProperties.GetIsInDesignMode(this))
            {
                List<UcIOHandle> ioHandles = LayoutHelper.FindAllLogicalChildren<UcIOHandle>(this);
                if (this is UcTransformationViewer)
                {
                    ioHandles.AddRange(LayoutHelper.FindAllVisualChildren<UcIOHandle>((ItemsControl)this.FindName("icInputHandles")));
                }

                this.IoHandles = ioHandles.ToArray();

                this.customDragDropHelper = new CustomDragDropHelper(this, this.OnCustomDragDrop);
                
            }
            checkCompletelyLoaded();
        }

		/// <summary>
		/// Handles the MouseLeftButtonDown event of this control.
		/// </summary>
		/// <param name="sender">The source of the event.</param>
		/// <param name="e">The <see cref="MouseButtonEventArgs"/> instance containing the event data.</param>
		private void UcNodeViewer_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
		{
			this.OnClick(e);
		}

		/// <summary>
		/// Handles the MouseRightButtonDown event of this control.
		/// </summary>
		/// <param name="sender">The source of the event.</param>
		/// <param name="e">The <see cref="MouseButtonEventArgs"/> instance containing the event data.</param>
		private void UcNodeViewer_MouseRightButtonDown(object sender, MouseButtonEventArgs e)
		{
			this.OnClick(e);
		}

        #endregion event handlers

        #region not sorted yet

        private Boolean loadedCompletely = false;
        public Boolean LoadedCompletely
        {
            get { return loadedCompletely; }
            set { loadedCompletely = value; }
        }

        private Boolean loadedallHandles = false;
        public Boolean LoadedallHandles
        {
            get { return loadedallHandles; }
            set { loadedallHandles = value; }
        }

        public void onUcIOHandleLoadedCompletely()
        {
            Boolean allHandlesLoadedCompletely = true;
            foreach (UcIOHandle handle in IoHandles)
            {
                if (!handle.LoadedCompletely)
                {
                    allHandlesLoadedCompletely = false;
                    return;
                }
            }
            if (allHandlesLoadedCompletely)
            {
                loadedallHandles = true;
                checkCompletelyLoaded();
            }
        }

        private void checkCompletelyLoaded()
        {
            if (IsLoaded && loadedallHandles)
            {
                LoadedCompletely = true;
                UcTreeDesigner treeDesigner = LayoutHelper.FindLogicalParent<UcTreeDesigner>(this, true);
                if (treeDesigner != null)
                {
                    treeDesigner.onUcNodeViewer_LoadedCompletely();
                }
            }
        }

        #endregion not sorted yet
    }
}