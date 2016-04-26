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

		private CustomDragDropHelper customDragDropHelper;

		#endregion fields

		#region constructors

		public UcNodeViewer()
		{
			this.MouseLeftButtonDown += UcNodeViewer_MouseLeftButtonDown;
			this.MouseRightButtonDown += UcNodeViewer_MouseRightButtonDown;
			this.Loaded += UcNodeViewer_Loaded;
		}

		#endregion constructors

		#region events

		/// <summary>
		/// TODO: (PS) Comment this.
		/// </summary>
		public event EventHandler StartedMoving;

		#endregion events

		#region properties

		// TODO: (PS) Comment this.
		public UcIOHandle[] IoHandles
		{
			get;
			private set;
		}

		/// <summary>
		/// Gets or sets the value of the IsInSelectionArea property.
		/// TODO: (PS) Comment this.
		/// This is a Dependency Property.
		/// </summary>
		public bool IsInSelectionArea
		{
			get { return (bool)this.GetValue(IsInSelectionAreaProperty); }
			set { this.SetValue(IsInSelectionAreaProperty, value); }
		}

		/// <summary>
		/// Gets or sets the value of the IsPreview property.
		/// TODO: (PS) Comment this.
		/// This is a Dependency Property.
		/// </summary>
		public bool IsPreview
		{
			get { return (bool)this.GetValue(IsPreviewProperty); }
			set { this.SetValue(IsPreviewProperty, value); }
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
			get { return this.DataContext as Node; }
		}

		#endregion properties

		#region overrideable methods

		public override string ToString()
		{
			return String.Format("{0} Node={1}", this.GetType().Name, this.Node);
		}

		#endregion overrideable methods

		#region methods

		/// <summary>
		/// Raises the <see cref="E:StartedMoving"/> event.
		/// </summary>
		protected void OnStartedMoving()
		{
			if (this.StartedMoving != null)
			{
				this.StartedMoving(this, null);
			}
		}

		private void OnClick(MouseButtonEventArgs e)
		{
			if (!this.IsPreview)
			{
				e.Handled = true;
				this.IsSelected = true;
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
				this.OnStartedMoving();
			}
		}

		#endregion methods

		#region event handlers

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
		}

		private void UcNodeViewer_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
		{
			this.OnClick(e);
		}

		private void UcNodeViewer_MouseRightButtonDown(object sender, MouseButtonEventArgs e)
		{
			this.OnClick(e);
		}

		#endregion event handlers
	}
}