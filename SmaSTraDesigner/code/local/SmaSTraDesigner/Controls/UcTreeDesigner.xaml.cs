using Common;
using Common.ExtensionMethods;
using Common.Resources.Converters;
using SmaSTraDesigner.BusinessLogic;
using System;
using System.Collections.Generic;
using System.Globalization;
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

namespace SmaSTraDesigner.Controls
{
	// TODO: (PS) Comment this.
	// TODO: (PS) Adapt for dynamic size changes for canvas.
	/// <summary>
	/// Interaction logic for UcTreeDesigner.xaml
	/// </summary>
	public partial class UcTreeDesigner : UserControl
	{
		private HashSet<Node> nodes = new HashSet<Node>();
		private LambdaConverter canvasOffsetConverter;

		public UcTreeDesigner()
		{
			this.InitializeComponent();

			this.cnvBackground.Width = 10000;
			this.cnvBackground.Height = 10000;

			this.outOutputViewer.DataContext = new OutputNode();

			this.canvasOffsetConverter = new LambdaConverter()
			{
				MultiConvertMethod = (values, targetType, parameter, culture) =>
				{
					if (values.Any(v => !(v is double) || Double.IsNaN((double)v)))
					{
						return null;
					}

					double controlSize = (double)values[0];
					double canvasSize = (double)values[1];
					double offset = (double)values[2];

					return GetOffset(canvasSize, controlSize) + offset;
				}
			};

			this.MakeBindings(this.outOutputViewer);
		}

		/// <summary>
		/// Gets or sets the value of the MovingNodeViewer property.
		/// TODO: (PS) Comment this.
		/// This is a Dependency Property.
		/// </summary>
		public UcNodeViewer MovingNodeViewer
		{
			get { return (UcNodeViewer)this.GetValue(MovingNodeViewerProperty); }
			set { this.SetValue(MovingNodeViewerProperty, value); }
		}

		/// <summary>
		/// Registration of MovingNodeViewer Dependency Property.
		/// </summary>
		public static readonly DependencyProperty MovingNodeViewerProperty =
			DependencyProperty.Register("MovingNodeViewer", typeof(UcNodeViewer), typeof(UcTreeDesigner), new FrameworkPropertyMetadata(null));

		/// <summary>
		/// Gets or sets the value of the SelectedNodeViewer property.
		/// TODO: (PS) Comment this.
		/// This is a Dependency Property.
		/// </summary>
		public UcNodeViewer SelectedNodeViewer
		{
			get { return (UcNodeViewer)this.GetValue(SelectedNodeViewerProperty); }
			set { this.SetValue(SelectedNodeViewerProperty, value); }
		}

		/// <summary>
		/// Registration of SelectedNodeViewer Dependency Property.
		/// </summary>
		public static readonly DependencyProperty SelectedNodeViewerProperty =
			DependencyProperty.Register(
				"SelectedNodeViewer", typeof(UcNodeViewer), typeof(UcTreeDesigner),
				new FrameworkPropertyMetadata(
					null,
					OnSelectedNodeViewerChanged));

		/// <summary>
		/// Property Changed Callback method of the SelectedNodeViewer Dependency Property.
		/// </summary>
		/// <param name="sender">The instance of the class that had the SelectedNodeViewer property changed.</param>
		/// <param name="e">The <see cref="System.Windows.DependencyPropertyChangedEventArgs"/> instance containing the event data.</param>
		private static void OnSelectedNodeViewerChanged(DependencyObject sender, DependencyPropertyChangedEventArgs e)
		{
			UcTreeDesigner subject = (UcTreeDesigner)sender;
			UcNodeViewer newValue = (UcNodeViewer)e.NewValue;
			UcNodeViewer oldValue = (UcNodeViewer)e.OldValue;

			if (oldValue != null)
			{
				oldValue.IsSelected = false;
			}
		}

		private static double GetOffset(double size, double viewPortSize)
		{
			return (size - viewPortSize) / 2.0;
		}

		private void MakeCanvasOffsetBinding(Control control, bool isVertical)
		{
			MultiBinding binding = new MultiBinding()
			{
				Converter = this.canvasOffsetConverter
			};

			DependencyProperty property;
			if (isVertical)
			{
				property = Canvas.TopProperty;
				binding.AddBindings(
					new Binding("ActualHeight") { RelativeSource = RelativeSource.Self },
					new Binding("ActualHeight") { Source = this.cnvBackground },
					new Binding("PosY"));
			}
			else
			{
				property = Canvas.LeftProperty;
				binding.AddBindings(
					new Binding("ActualWidth") { RelativeSource = RelativeSource.Self },
					new Binding("ActualWidth") { Source = this.cnvBackground },
					new Binding("PosX"));
			}

			BindingOperations.SetBinding(control, property, binding);
		}

		private void MakeBindings(Control control)
		{
			this.MakeCanvasOffsetBinding(control, false);
			this.MakeCanvasOffsetBinding(control, true);

			DisposablesHandler.Instance.AddDisposeConnection(control, PropertyChangedHandle.GetDistinctInstance(control, "IsMoving", this.OnIsMovingChanged));
		}

		private void OnIsMovingChanged(PropertyChangedCallbackArgs args)
		{
			bool isMoving = (bool)args.NewValue;
			if (isMoving)
			{
				this.MovingNodeViewer = (UcNodeViewer)args.Handle.Source;
			}
		}

		private void This_Loaded(object sender, RoutedEventArgs e)
		{
			this.scvCanvas.ScrollToHorizontalOffset(GetOffset(this.cnvBackground.ActualWidth, this.scvCanvas.ViewportWidth));
			this.scvCanvas.ScrollToVerticalOffset(GetOffset(this.cnvBackground.ActualHeight, this.scvCanvas.ViewportHeight));
		}

		private void ShowTree(TransformationTree tree)
		{
			this.cnvBackground.Children.Clear();
			this.nodes.Clear();
			if (tree != null)
			{
				this.cnvBackground.Children.Add(this.outOutputViewer);
				this.outOutputViewer.DataContext = tree.OutputNode;
			}
		}

		private void ShowNode(Node node)
		{
			if (this.nodes.Contains(node))
			{
				return;
			}

			Transformation nodeAsTransformation;
			//DataSource nodeAsDataSource;
			Control control;
			if ((nodeAsTransformation = node as Transformation) != null)
			{
				control = new UcTransformationViewer();
			}
			else // if ((nodeAsDataSource = node as DataSource) != null)
			{
				control = new UcDataSourceViewer();
			}

			control.DataContext = node;
			this.cnvBackground.Children.Add(control);
			this.MakeBindings(control);
			control.BringIntoView();
		}

		// TODO: (PS) Remove this.
		public void UseTestTree()
		{
		}

		/// <summary>
		/// Gets or sets the value of the Tree property.
		/// TODO: (PS) Comment this.
		/// This is a Dependency Property.
		/// </summary>
		public TransformationTree Tree
		{
			get { return (TransformationTree)this.GetValue(TreeProperty); }
			set { this.SetValue(TreeProperty, value); }
		}

		/// <summary>
		/// Registration of Tree Dependency Property.
		/// </summary>
		public static readonly DependencyProperty TreeProperty =
			DependencyProperty.Register(
				"Tree", typeof(TransformationTree), typeof(UcTreeDesigner),
				new FrameworkPropertyMetadata(
					null,
					OnTreeChanged));

		/// <summary>
		/// Property Changed Callback method of the Tree Dependency Property.
		/// </summary>
		/// <param name="sender">The instance of the class that had the Tree property changed.</param>
		/// <param name="e">The <see cref="System.Windows.DependencyPropertyChangedEventArgs"/> instance containing the event data.</param>
		private static void OnTreeChanged(DependencyObject sender, DependencyPropertyChangedEventArgs e)
		{
			UcTreeDesigner subject = (UcTreeDesigner)sender;
			TransformationTree newValue = (TransformationTree)e.NewValue;
			TransformationTree oldValue = (TransformationTree)e.OldValue;

			subject.ShowTree(newValue);
		}

		private void This_DragOver(object sender, DragEventArgs e)
		{
		}

		private void This_Drop(object sender, DragEventArgs e)
		{
			Node node = ((Tuple<Node>)e.Data.GetData(typeof(Tuple<Node>))).Item1;
			Point mousePos = e.GetPosition(this.cnvBackground);
			Node newNode = (Node)node.Clone();
			newNode.PosX = mousePos.X - this.cnvBackground.ActualWidth / 2;
			newNode.PosY = mousePos.Y - this.cnvBackground.ActualHeight / 2;

			this.ShowNode(newNode);
		}

		private void This_MouseMove(object sender, MouseEventArgs e)
		{
			if (e.LeftButton == MouseButtonState.Pressed && this.MovingNodeViewer != null)
			{
				Point mousePos = e.GetPosition(this.cnvBackground);
				Point mousePosOnViewer = e.GetPosition(this.MovingNodeViewer);
				Node node = (Node)this.MovingNodeViewer.DataContext;
				node.PosX = mousePos.X - this.cnvBackground.ActualWidth / 2/* - mousePosOnViewer.X*/;
				node.PosY = mousePos.Y - this.cnvBackground.ActualHeight / 2/* - mousePosOnViewer.Y*/;
			}
		}

		private void This_PreviewMouseLeftButtonUp(object sender, MouseButtonEventArgs e)
		{
			if (this.MovingNodeViewer != null)
			{
				this.MovingNodeViewer.IsMoving = false;
				this.MovingNodeViewer = null;
			}
		}

		private void This_MouseLeave(object sender, MouseEventArgs e)
		{
			if (this.MovingNodeViewer != null)
			{
				this.MovingNodeViewer.IsMoving = false;
				this.MovingNodeViewer = null;
			}
		}

		private void cnvBackground_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
		{
			this.SelectedNodeViewer = null;
			e.Handled = true;
		}

		private void cnvBackground_MouseRightButtonDown(object sender, MouseButtonEventArgs e)
		{
			this.SelectedNodeViewer = null;
			e.Handled = true;
		}
	}
}
