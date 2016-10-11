using SmaSTraDesigner.BusinessLogic.classhandler.nodeclasses;
using SmaSTraDesigner.BusinessLogic.nodes;

namespace SmaSTraDesigner.Controls
{
    using System;
    using System.Windows;
    using System.Windows.Controls;
    using System.Windows.Input;
    using System.Linq;

    using BusinessLogic;

    using Common;

    using Support;
    using Common.Resources.Converters;
    using BusinessLogic.classhandler;

    /// <summary>
    /// Represents an input or output of a node on the GUI as a small handle for the user to interact with.
    /// </summary>
    /// <seealso cref="System.Windows.Controls.UserControl" />
    /// <seealso cref="System.Windows.Markup.IComponentConnector" />
    public partial class UcIOHandle : UserControl
	{
		#region static constructor

		/// <summary>
		/// Initializes the <see cref="UcIOHandle"/> class.
		/// </summary>
		static UcIOHandle()
		{
			// Initialize readonly dependency properties.
			IsConnectionCompatibleProperty = IsConnectionCompatiblePropertyKey.DependencyProperty;
		}

		#endregion static constructor

		#region dependency properties

		/// <summary>
		/// Registration of InputIndex Dependency Property.
		/// </summary>
		public static readonly DependencyProperty InputIndexProperty = 
			DependencyProperty.Register("InputIndex", typeof(int), typeof(UcIOHandle), new PropertyMetadata(-1));

		/// <summary>
		/// Registration of IsConnectionCompatible Dependency Property.
		/// </summary>
		public static readonly DependencyProperty IsConnectionCompatibleProperty;

		/// <summary>
		/// Registration of IsConnectionCompatible Dependency Property Key.
		/// </summary>
		private static readonly DependencyPropertyKey IsConnectionCompatiblePropertyKey = 
			DependencyProperty.RegisterReadOnly("IsConnectionCompatible", typeof(bool?), typeof(UcIOHandle), new FrameworkPropertyMetadata(null));

		/// <summary>
		/// Registration of IsInput Dependency Property.
		/// </summary>
		public static readonly DependencyProperty IsInputProperty = 
			DependencyProperty.Register("IsInput", typeof(bool), typeof(UcIOHandle));

		/// <summary>
		/// Registration of IsSelected Dependency Property.
		/// </summary>
		public static readonly DependencyProperty IsSelectedProperty = 
			DependencyProperty.Register(
				"IsSelected", typeof(bool), typeof(UcIOHandle),
				new FrameworkPropertyMetadata(
					false,
					OnIsSelectedChanged));

        public static readonly DependencyProperty DataTypeNameProperty =
            DependencyProperty.Register(
                "DataTypeName", typeof(string), typeof(UcIOHandle),
                new FrameworkPropertyMetadata(
                    "null"));

        #endregion dependency properties

        #region dependency property callbacks

        /// <summary>
        /// Property Changed Callback method of the IsSelected Dependency Property.
        /// </summary>
        /// <param name="sender">The instance of the class that had the IsSelected property changed.</param>
        /// <param name="e">The <see cref="System.Windows.DependencyPropertyChangedEventArgs"/> instance containing the event data.</param>
        private static void OnIsSelectedChanged(DependencyObject sender, DependencyPropertyChangedEventArgs e)
		{
			UcIOHandle subject = (UcIOHandle)sender;
			bool newValue = (bool)e.NewValue;
			bool oldValue = (bool)e.OldValue;

			if (newValue)
			{
				if (subject.NodeViewer != null)
				{
                    
					subject.NodeViewer.IsSelected = true;
					foreach (var other in subject.NodeViewer.IoHandles)
					{
						if (other != subject)
						{
							other.IsSelected = false;
						}
					}
				}
			}
		}

		#endregion dependency property callbacks

		#region fields

		/// <summary>
		/// The CustomDragDropHelper instance that helps with Drag&Drop functionality in this control.
		/// </summary>
		private CustomDragDropHelper customDragDropHelper;

		/// <summary>
		/// The tree designer this control belongs to.
		/// </summary>
		private UcTreeDesigner treeDesigner = null;

		#endregion fields

		#region constructors

		/// <summary>
		/// Initializes a new instance of the <see cref="UcIOHandle"/> class.
		/// </summary>
		public UcIOHandle()
		{
			this.InitializeComponent();
			
			((LambdaConverter)this.FindResource("ToolTipConverter")).ConvertMethod = (value, targetType, parameter, culture) =>
			{
				string typeName = (string)value;

				return typeName != null ? typeName.Split('.').Last() : null;
			};

			this.customDragDropHelper = new CustomDragDropHelper(this, this.OnCustomDragDrop);

            DataContextChanged += OnDataContextChanged;
            this.LayoutUpdated += UcIOHandle_LayoutUpdated;
		}

        #endregion constructors

        #region events

        /// <summary>
        /// Is raised when this io handle is being dragged.
        /// </summary>
        public event EventHandler CustomDrag;

		#endregion events

		#region properties

		/// <summary>
		/// Gets the DataType that this control represents.
		/// </summary>
		public DataType DataType
		{
			get {
                if (this.IsInput)
                {
                    if (this.Node.InputIOData.Count > 0)
                    {
                        return this.Node.InputIOData[InputIndex].Type as DataType;
                    } else { return null;}
                }
                else { return this.Node.OutputIOData.Type as DataType; }
            }
		}

		/// <summary>
		/// Gets or sets the value of the InputIndex property.
		/// If this control is used to represent a node's input then this is the index of that input.
		/// This is a Dependency Property.
		/// </summary>
		public int InputIndex
		{
			get { return (int)this.GetValue(InputIndexProperty); }
			set { this.SetValue(InputIndexProperty, value); }
		}

		/// <summary>
		/// Gets the value of the IsConnectionCompatible property.
		/// States whether the input or output this control represents is compatible with the data type
		/// the user is currently trying to connect.
		/// null = no data connection is being made.
		/// true = data is compatible with this input/output.
		/// false = data is NOT compatible.
		/// This is a Dependency Property.
		/// </summary>
		public bool? IsConnectionCompatible
		{
			get { return (bool?)this.GetValue(IsConnectionCompatibleProperty); }
			private set { this.SetValue(IsConnectionCompatiblePropertyKey, value); }
		}

		/// <summary>
		/// Gets or sets the value of the IsInput property.
		/// States whether this IO handle represents an input (or output otherwise)
		/// This is a Dependency Property.
		/// </summary>
		public bool IsInput
		{
			get { return (bool)this.GetValue(IsInputProperty); }
			set { this.SetValue(IsInputProperty, value); }
		}

		/// <summary>
		/// Gets a value indicating whether this instance is used in a preview (left side menu on the main window).
		/// </summary>
		/// <value>
		/// <c>true</c> if this instance is preview; otherwise, <c>false</c>.
		/// </value>
		public bool IsPreview
		{
			get
			{
				return this.NodeViewer != null ? this.NodeViewer.IsPreview : true;
			}
		}

		/// <summary>
		/// Gets or sets the value of the IsSelected property.
		/// States whether this input/output is currently selected in the tree designer.
		/// This is a Dependency Property.
		/// </summary>
		public bool IsSelected
		{
			get { return (bool)this.GetValue(IsSelectedProperty); }
			set { this.SetValue(IsSelectedProperty, value); }
		}

        public string DataTypeName
        {
            get { return (string)this.GetValue(DataTypeNameProperty); }
            set { this.SetValue(DataTypeNameProperty, value); }
        }

        /// <summary>
        /// Gets the Node instance the represented input/output is attached to.
        /// </summary>
        public Node Node
		{
			get { return this.NodeViewer != null ? this.NodeViewer.DataContext as Node : null; }
		}

		/// <summary>
		/// Gets the node viewer this input/output is attached to.
		/// </summary>
		public UcNodeViewer NodeViewer
		{
			get;
			private set;
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
			return String.Format("{0} Node={1}, DataType={2}", this.GetType().Name, this.Node, this.DataType);
		}

		#endregion overrideable methods

		#region methods

		/// <summary>
		/// Raises the <see cref="E:Click" /> event.
		/// </summary>
		/// <param name="e">The <see cref="MouseButtonEventArgs"/> instance containing the event data.</param>
		private void OnClick(MouseButtonEventArgs e)
		{
			if (!this.IsPreview)
			{
				this.IsSelected = true;
                if (CommandBindings.Count > 0) {
                    CommandBindings[0].Command.Execute(this);
                        }
				e.Handled = true;
			}
		}

		/// <summary>
		/// Raises the <see cref="E:CustomDrag"/> event.
		/// </summary>
		private void OnCustomDrag()
		{
			if (this.CustomDrag != null)
			{
				this.CustomDrag(this, null);
			}
		}

		/// <summary>
		/// Callback for the CustomDragDropHelper
		/// </summary>
		/// <param name="e">The <see cref="MouseEventArgs"/> instance containing the event data.</param>
		private void OnCustomDragDrop(MouseEventArgs e)
		{
			if (!this.IsPreview)
			{
				this.OnCustomDrag();
				e.Handled = true;
			}
		}

		/// <summary>
		/// Callback for when the associated UcNodeViewer's IsSelected property changes.
		/// </summary>
		/// <param name="args">The change arguments.</param>
		private void OnNodeViewerIsSelectedChanged(PropertyChangedCallbackArgs args)
		{
			bool newValue = (bool)args.NewValue;

			if (!newValue)
			{
				// Deselect this IO handle if node is deselected.
				this.IsSelected = false;
			}
		}

		/// <summary>
		/// Callback for when the assiciated UcTreeDesigner's ConnectingIOHandle property changes.
		/// </summary>
		/// <param name="args">The change arguments.</param>
		private void OnTreeDesignerConnectingIOHandleChanged(PropertyChangedCallbackArgs args)
		{
			UcIOHandle newValue = args.NewValue as UcIOHandle;

			// If user is trying to connect another IO handle, provide information about
			// whether this handle is compatible with its data by setting the IsConnectionCompatible
			// property.
			if (newValue != this)
			{
				if (newValue != null && this.IsInput != newValue.IsInput && this.NodeViewer != newValue.NodeViewer)
				{
					this.IsConnectionCompatible = this.DataType == null || newValue.DataType == null ||
						(!this.IsInput && this.DataType.CanConvertTo(newValue.DataType)) || (this.IsInput && newValue.DataType.CanConvertTo(this.DataType));
				}
				else
				{
					this.IsConnectionCompatible = null;
				}
			}
		}

		#endregion methods

		#region event handlers

		/// <summary>
		/// Handles the Loaded event of this control.
		/// </summary>
		/// <param name="sender">The source of the event.</param>
		/// <param name="e">The <see cref="RoutedEventArgs"/> instance containing the event data.</param>
		private void This_Loaded(object sender, RoutedEventArgs e)
		{
			if (!System.ComponentModel.DesignerProperties.GetIsInDesignMode(this))
			{
				this.NodeViewer = LayoutHelper.FindLogicalParent<UcNodeViewer>(this, true);
				if (this.NodeViewer == null)
				{
					this.NodeViewer = LayoutHelper.FindVisualParent<UcNodeViewer>(this, true);
				}

                if (this.NodeViewer != null && !this.IsPreview)
				{
					this.treeDesigner = LayoutHelper.FindLogicalParent<UcTreeDesigner>(this.NodeViewer, true);
					if (this.treeDesigner != null)
					{
						this.treeDesigner.RegisterIOHandle(this);

						DisposablesHandler.Instance.AddDisposeConnection(this,
							PropertyChangedHandle.GetDistinctInstance(this.treeDesigner, "ConnectingIOHandle", this.OnTreeDesignerConnectingIOHandleChanged));
					}

					DisposablesHandler.Instance.AddDisposeConnection(this,
						PropertyChangedHandle.GetDistinctInstance(this.NodeViewer, "IsSelected", this.OnNodeViewerIsSelectedChanged));
				}
			}
            LoadedCompletely = true;
		}

		/// <summary>
		/// Handles the MouseLeftButtonDown event of this control.
		/// </summary>
		/// <param name="sender">The source of the event.</param>
		/// <param name="e">The <see cref="MouseButtonEventArgs"/> instance containing the event data.</param>
		private void This_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
		{
			this.OnClick(e);
		}

		/// <summary>
		/// Handles the MouseRightButtonDown event of this control.
		/// </summary>
		/// <param name="sender">The source of the event.</param>
		/// <param name="e">The <see cref="MouseButtonEventArgs"/> instance containing the event data.</param>
		private void This_MouseRightButtonDown(object sender, MouseButtonEventArgs e)
		{
			this.OnClick(e);
		}

        private void OnDataContextChanged(object sender, DependencyPropertyChangedEventArgs e)
        {
            if (e.NewValue is IOData)
            {
                string typeName = ((IOData)e.NewValue).Type.Name;
                DataTypeName = typeName != null ? typeName.Split('.').Last() : null;
            }
        }

        #endregion event handlers

        #region not sorted yet

        private Boolean loadedCompletely = false;
        public Boolean LoadedCompletely
        {
            get { return loadedCompletely; }
            set {
                loadedCompletely = value;
                if (loadedCompletely && NodeViewer != null)
                {
                    NodeViewer.onUcIOHandleLoadedCompletely();
                }
            }
        }

        public static readonly DependencyProperty PositionProperty =
        DependencyProperty.Register("Position", typeof(Point), typeof(UcIOHandle));
         

    public Point Position
        {
            get
            {
                return (Point)GetValue(PositionProperty);
            }
            set
            {
                SetValue(PositionProperty, value);
            }
        }

        private void UcIOHandle_LayoutUpdated(object sender, EventArgs e)
        {
            if (this.NodeViewer == null || treeDesigner == null)
            {
                return;
            }
            Point center = new Point(this.ActualWidth / 2, this.ActualHeight / 2);
            try {
                Point centerRelativeToNodeViewer = this.TransformToAncestor(treeDesigner.cnvBackground).Transform(center);
                Position = centerRelativeToNodeViewer;
            }
            catch (InvalidOperationException exeption)
            { // this Exception only occured while debug breakpoints are set. Should not occur during normal usage
                System.Diagnostics.Debug.Print("ERROR while TransformToAncestor: " + exeption.Message);
                return;
            }
        }

        public void unregisterHandlers()
        {
            DataContextChanged -= OnDataContextChanged;
            this.LayoutUpdated -= UcIOHandle_LayoutUpdated;
        }

        #endregion not sorted yet
    }
}