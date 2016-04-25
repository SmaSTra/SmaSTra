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
		private static double GetOffset(double size, double viewPortSize)
		{
			return (size - viewPortSize) / 2.0;
		}

		private LambdaConverter canvasOffsetConverter;

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
		private void MakeCanvasOffsetBinding(Control control)
		{
			this.MakeCanvasOffsetBinding(control, false);
			this.MakeCanvasOffsetBinding(control, true);
		}

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

			this.MakeCanvasOffsetBinding(this.outOutputViewer);
		}

		private void This_Loaded(object sender, RoutedEventArgs e)
		{
			this.scvCanvas.ScrollToHorizontalOffset(GetOffset(this.cnvBackground.ActualWidth, this.scvCanvas.ViewportWidth));
			this.scvCanvas.ScrollToVerticalOffset(GetOffset(this.cnvBackground.ActualHeight, this.scvCanvas.ViewportHeight));
		}

		private HashSet<Node> nodes = new HashSet<Node>();

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
			this.MakeCanvasOffsetBinding(control);
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
	}
}
