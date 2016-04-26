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

	using BusinessLogic;

	using Common;

	using Support;

	/// <summary>
	/// Interaction logic for UcIOHandle.xaml
	/// </summary>
	public partial class UcIOHandle : UserControl
	{
		#region static constructor

		static UcIOHandle()
		{
			IsConnectionCompatibleProperty = IsConnectionCompatiblePropertyKey.DependencyProperty;
		}

		#endregion static constructor

		#region dependency properties

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
				}
			}
		}

		#endregion dependency property callbacks

		#region fields

		private CustomDragDropHelper customDragDropHelper;
		private UcTreeDesigner treeDesigner = null;

		#endregion fields

		#region constructors

		public UcIOHandle()
		{
			this.InitializeComponent();

			this.customDragDropHelper = new CustomDragDropHelper(this, this.OnCustomDragDrop);
		}

		#endregion constructors

		#region events

		/// <summary>
		/// TODO: (PS) Comment this.
		/// </summary>
		public event EventHandler CustomDrag;

		#endregion events

		#region properties

		// TODO: (PS) Comment this.
		public DataType DataType
		{
			get { return this.DataContext as DataType; }
		}

		/// <summary>
		/// Gets the value of the IsConnectionCompatible property.
		/// This is a Dependency Property.
		/// </summary>
		public bool? IsConnectionCompatible
		{
			get { return (bool?)this.GetValue(IsConnectionCompatibleProperty); }
			private set { this.SetValue(IsConnectionCompatiblePropertyKey, value); }
		}

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

		public bool IsPreview
		{
			get
			{
				return this.NodeViewer != null ? this.NodeViewer.IsPreview : true;
			}
		}

		/// <summary>
		/// Gets or sets the value of the IsSelected property.
		/// TODO: (PS) Comment this.
		/// This is a Dependency Property.
		/// </summary>
		public bool IsSelected
		{
			get { return (bool)this.GetValue(IsSelectedProperty); }
			set { this.SetValue(IsSelectedProperty, value); }
		}

		// TODO: (PS) Comment this.
		public Node Node
		{
			get { return this.NodeViewer != null ? this.NodeViewer.DataContext as Node : null; }
		}

		// TODO: (PS) Comment this.
		public UcNodeViewer NodeViewer
		{
			get;
			private set;
		}

		#endregion properties

		#region overrideable methods

		public override string ToString()
		{
			return String.Format("{0} Node={1}, DataType={2}", this.GetType().Name, this.Node, this.DataType);
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

		private void OnClick(MouseButtonEventArgs e)
		{
			if (!this.IsPreview)
			{
				this.IsSelected = true;
				e.Handled = true;
			}
		}

		private void OnCustomDragDrop(MouseEventArgs e)
		{
			if (!this.IsPreview)
			{
				this.OnCustomDrag();
				e.Handled = true;
			}
		}

		private void OnNodeViewerIsSelectedChanged(PropertyChangedCallbackArgs args)
		{
			bool newValue = (bool)args.NewValue;

			if (!newValue)
			{
				this.IsSelected = false;
			}
		}

		private void OnTreeDesignerConnectingIOHandleChanged(PropertyChangedCallbackArgs args)
		{
			UcIOHandle newValue = args.NewValue as UcIOHandle;

			if (newValue != this)
			{
				if (newValue != null && this.IsInput != newValue.IsInput && this.NodeViewer != newValue.NodeViewer)
				{
					this.IsConnectionCompatible = this.DataType == null || newValue.DataType == null ||
						object.Equals(this.DataType, newValue.DataType);
				}
				else
				{
					this.IsConnectionCompatible = null;
				}
			}
		}

		#endregion methods

		#region event handlers

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
		}

		private void This_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
		{
			this.OnClick(e);
		}

		private void This_MouseMove(object sender, MouseEventArgs e)
		{
			if (e.LeftButton == MouseButtonState.Pressed)
			{

			}
		}

		private void This_MouseRightButtonDown(object sender, MouseButtonEventArgs e)
		{
			this.OnClick(e);
		}

		#endregion event handlers
	}
}