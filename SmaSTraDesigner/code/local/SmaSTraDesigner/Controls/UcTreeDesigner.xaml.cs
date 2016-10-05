namespace SmaSTraDesigner.Controls
{
    using System;
    using System.Collections.Generic;
    using System.Collections.ObjectModel;
    using System.Collections.Specialized;
    using System.Linq;
    using System.Windows;
    using System.Windows.Controls;
    using System.Windows.Data;
    using System.Windows.Input;
    using System.Windows.Media;
    using System.Windows.Shapes;

    using Common;
    using Common.ExtensionMethods;
    using Common.Resources.Converters;

    using BusinessLogic;
    using BusinessLogic.classhandler;
    using BusinessLogic.nodes;
    using Support;
    using BusinessLogic.utils;
    using System.Windows.Threading;
    using BusinessLogic.uitransactions;

    // TODO: (PS) Comment this.
    // TODO: (PS) Adapt for dynamic size changes for canvas.
    /// <summary>
    /// Interaction logic for UcTreeDesigner.xaml
    /// Also this is the mighty ZEUS God-Class!
    /// </summary>
    public partial class UcTreeDesigner : UserControl
	{
		#region static constructor

		static UcTreeDesigner()
		{
			ConnectingIOHandleProperty = ConnectingIOHandlePropertyKey.DependencyProperty;
		}

		#endregion static constructor

		#region static methods

		private static double GetOffset(double size, double viewPortSize)
		{
			return (size - viewPortSize) / 2.0;
		}


        private static bool SubTreeContains(Node root, List<Node> nodes)
        {
            //Remove the first element:
            if (root != null) nodes.Remove(root);

            //If empty -> Everything is okay!
            if (!nodes.Any()) return true;

            //Check recursivcely:
            foreach ( Node input in root.InputNodes.NonNull() )
            {
                if (SubTreeContains(input, nodes)) return true;
            }

            return false;
        }


        #endregion static methods

        #region dependency properties

        /// <summary>
        /// Registration of ConnectingIOHandle Dependency Property.
        /// </summary>
        public static readonly DependencyProperty ConnectingIOHandleProperty;

		/// <summary>
		/// Registration of ConnectingIOHandle Dependency Property Key.
		/// </summary>
		private static readonly DependencyPropertyKey ConnectingIOHandlePropertyKey = 
			DependencyProperty.RegisterReadOnly("ConnectingIOHandle", typeof(UcIOHandle), typeof(UcTreeDesigner), new FrameworkPropertyMetadata(null));

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
		/// Registration of Tree Dependency Property.
		/// </summary>
		public static readonly DependencyProperty TreeProperty = 
			DependencyProperty.Register(
				"Tree", typeof(TransformationTree), typeof(UcTreeDesigner),
				new FrameworkPropertyMetadata(
					null,
					OnTreeChanged));

		#endregion dependency properties

		#region dependency property callbacks

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

            Singleton<NodeProperties>.Instance.ActiveNode = newValue != null ? newValue.Node : null;

            if (!subject.changingSelectedNodeViewers)
			{
				subject.SelectedNodeViewers.CollectionChanged -= subject.SelectedNodeViewers_CollectionChanged;

				foreach (var noveViewer in subject.SelectedNodeViewers)
				{
					noveViewer.IsSelected = false;
				}

				subject.SelectedNodeViewers.Clear();
				if (newValue != null)
				{
					subject.SelectedNodeViewers.Add(newValue);
					newValue.IsSelected = true;
				}

				subject.previouslySelectedItems = subject.SelectedNodeViewers.ToArray();

				subject.AdjustZIndex();

				subject.SelectedNodeViewers.CollectionChanged += subject.SelectedNodeViewers_CollectionChanged;

                System.Diagnostics.Debug.Print("SelectedNodeViewer: " + newValue);
            }
		}

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

			subject.OnTreeChanged(oldValue, newValue);
		}

		#endregion dependency property callbacks


        #region constants

        private const double scaleRate = 1.1;

        #endregion constants


        #region fields

        private LambdaConverter canvasOffsetConverter;
		private bool changingSelectedNodeViewers = false;
		private LambdaConverter connectionLineCoordConverter;
		private Dictionary<Connection, Line> connectionLines = new Dictionary<Connection, Line>();
		private Point? dragStart = null;
        private Point? lastScrollPosition;
        private Point? mousePosOnViewer = null;
		private UcNodeViewer movingNodeViewer = null;
		private Dictionary<Node, UcNodeViewer> nodeViewers = new Dictionary<Node, UcNodeViewer>();
		private UcNodeViewer[] previouslySelectedItems = { };
		private HashSet<UcIOHandle> registeredIoHandles = new HashSet<UcIOHandle>();
        private ScaleTransform scaletransform;
        private int gridSize = 50; // TODO: Make to global variable

        private UIConnectionRefresher connectionRefresher;
        private DispatcherTimer timer;

        private Stack<UITransaction> undoStack = new Stack<UITransaction>();
        private Stack<UITransaction> redoStack = new Stack<UITransaction>();


        #endregion fields

        #region constructors

        public UcTreeDesigner()
		{
			this.SelectedNodeViewers = new ObservableCollection<UcNodeViewer>();
			this.SelectedNodeViewers.CollectionChanged += SelectedNodeViewers_CollectionChanged;
            this.connectionRefresher = new UIConnectionRefresher(AddConnection);

            this.InitializeComponent();

            //Start a timer to update the connections:
            timer = new DispatcherTimer();
            timer.Interval = TimeSpan.FromMilliseconds(100);
            timer.Tick += timer_tick;
            timer.Start();


            if (!System.ComponentModel.DesignerProperties.GetIsInDesignMode(this))
			{
				this.Tree = new TransformationTree(this);

				Panel.SetZIndex(this.bdrSelectionBorder, Int32.MaxValue);
				Panel.SetZIndex(this.linPreviewConnection, Int32.MaxValue);

				this.cnvBackground.Width = 10000;
				this.cnvBackground.Height = 10000;

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
				this.connectionLineCoordConverter = new LambdaConverter()
				{
					ConvertMethod = (value, targetType, parameter, culture) =>
					{
						Tuple<bool, UcIOHandle> paramz = (Tuple<bool, UcIOHandle>)parameter;
                        Point p = (Point)value;
                        if (!paramz.Item2.IsInput) // Place Connection at the tip of outputHandles
                        {
                            p.X = p.X - 2 + paramz.Item2.Width / 2;
                        }
                        return paramz.Item1 ? p.X : p.Y;

                    }
				};

				this.MakeBindings(this.outOutputViewer);
			}
		}

        #endregion constructors

        #region properties

        /// <summary>
        /// Gets the value of the ConnectingIOHandle property.
        /// This is a Dependency Property.
        /// </summary>
        public UcIOHandle ConnectingIOHandle
		{
			get { return (UcIOHandle)this.GetValue(ConnectingIOHandleProperty); }
			private set { this.SetValue(ConnectingIOHandlePropertyKey, value); }
		}

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

		// TODO: (PS) Comment this.
		public ObservableCollection<UcNodeViewer> SelectedNodeViewers
		{
			get;
			private set;
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

		#endregion properties

		#region methods

		public void AddConnection(Connection connection)
		{
			if (this.Tree == null || this.connectionLines.ContainsKey(connection))
			{
				return;
			}

			UcNodeViewer oNode = null;
			if (!this.nodeViewers.TryGetValue(connection.OutputNode, out oNode))
			{
				throw new Exception(String.Format("OutputNode {0} not found.", connection.OutputNode));
			}
			UcNodeViewer iNode = null;
            if(connection.InputNode is OutputNode)
            {
                iNode = outOutputViewer;
            }
            else if (!this.nodeViewers.TryGetValue(connection.InputNode, out iNode))
			{
				throw new Exception(String.Format("InputNode {0} not found.", connection.InputNode));
			}

            //Fix for Not inited Node Vierwers:
            if (oNode.IoHandles == null || iNode.IoHandles == null)
            {
                connectionRefresher.AddPendingConnection(connection, iNode, oNode);
                return;
            }


			UcIOHandle oHandle = oNode.IoHandles.FirstOrDefault(h => !h.IsInput);
            UcIOHandle iHandle = iNode.IoHandles.FirstOrDefault(h => h.IsInput && h.InputIndex == connection.InputIndex);

            if (iHandle != null && iHandle.Node != null)
            {
                this.AddConnection(oHandle, iHandle, connection);
            }
		}


        void timer_tick(object sender, EventArgs e)
        {
            connectionRefresher.Tick();
        }


        public void AddNodes(Node[] nodes, bool saveTransaction = false)
        {
            nodes.ForEach(n => AddNode(n, false, false));
            if (saveTransaction)
            {
                this.undoStack.Push(new UITransactionAddNodes(nodes));
                this.redoStack.Clear();
            }
        }


        public void AddNode(Node node, bool select = false, bool saveTransaction = false)
		{
            if (node == null)
			{
				throw new ArgumentNullException("node");
			}

			if (this.Tree == null || this.nodeViewers.ContainsKey(node))
			{
				return;
			}

			node.Tree = this.Tree;
			Transformation nodeAsTransformation;
			DataSource nodeAsDataSource;
			OutputNode nodeAsOutputNode;
			CombinedNode nodeAsCombinedNode;

            UcNodeViewer nodeViewer = null;
			if ((nodeAsTransformation = node as Transformation) != null)
			{
				nodeViewer = new UcTransformationViewer();
			}
			else if ((nodeAsDataSource = node as DataSource) != null)
			{
				nodeViewer = new UcDataSourceViewer();
			}
            else if ((nodeAsOutputNode = node as OutputNode) != null)
            {
                nodeViewer = new UcOutputViewer();
            }
            else if ((nodeAsCombinedNode = node as CombinedNode) != null)
            {
                if (node.Class.InputTypes.Count() > 0) nodeViewer = new UcTransformationViewer();
                else nodeViewer = new UcDataSourceViewer();
            }
            this.nodeViewers.Add(node, nodeViewer);

            nodeViewer.DataContext = node;
            this.Tree.Nodes.Add(node);
			this.cnvBackground.Children.Add(nodeViewer);
			this.MakeBindings(nodeViewer);

			if (select)
			{
				nodeViewer.IsSelected = true;
				nodeViewer.BringIntoView();
			}

            //Save the transaction if wanted to revert if possible.
            if (saveTransaction)
            {
                this.undoStack.Push(new UITransactionAddNodes(node));
                this.redoStack.Clear();
            }
        }

		public void RemoveConnection(Connection connection)
		{
			this.RemoveConnection(null, connection);
		}


        public void RemoveNodes(Node[] nodes, bool saveTransaction = false)
        {
            nodes = nodes.Where(n => !(n is OutputNode)).ToArray();

            Connection[] connections = Tree.Connections
                .Where(c => nodes.Contains(c.InputNode) || nodes.Contains(c.OutputNode))
                .ToArray();

            nodes.ForEach(n => RemoveNode(n, false));
            if (saveTransaction)
            {
                this.undoStack.Push(new UITransactionDeleteNodes(nodes, connections));
                this.redoStack.Clear();
            }
        }


		public void RemoveNode(Node node, bool saveTransaction = false)
		{
			UcNodeViewer nodeViewer;
			if (this.nodeViewers.TryGetValue(node, out nodeViewer))
			{
				this.RemoveNode(nodeViewer);
			}

            //Add the Remove-Transaction back.
            if (saveTransaction && node != null)
            {
                Connection[] connections = Tree.Connections
                    .Where(c => (node == c.InputNode) || node == c.OutputNode)
                    .ToArray();
                this.undoStack.Push(new UITransactionDeleteNodes(node,connections));
                this.redoStack.Clear();
            }
		}


		internal void RegisterIOHandle(UcIOHandle ioHandle)
		{
			ioHandle.CustomDrag += IoHandle_CustomDrag;
			this.registeredIoHandles.Add(ioHandle);
		}

		private void AddConnection(UcIOHandle oHandle, UcIOHandle iHandle, Connection? connection)
		{
			this.RemoveConnection(iHandle, null);
            System.Diagnostics.Debug.Print("adding connection: oNode: " + oHandle.Node.Name + "  iNode: " + iHandle.Node.Name);

			if (connection == null)
			{
				connection = new Connection(oHandle.Node, iHandle.Node, iHandle.InputIndex);
			}
            this.Tree.Connections.Add(connection.Value);
			Transformation iNodeAsTransformation;
			OutputNode iNodeAsOutputNode;
            CombinedNode iNodeAsCombined;
			if ((iNodeAsTransformation = connection.Value.InputNode as Transformation) != null)
			{
				iNodeAsTransformation.SetInput(connection.Value.InputIndex, connection.Value.OutputNode);
            }
            else if ((iNodeAsCombined = connection.Value.InputNode as CombinedNode) != null)
            {
                iNodeAsCombined.SetInput(connection.Value.InputIndex, connection.Value.OutputNode);
            }
            else if ((connection.Value.InputNode as OutputNode) != null)
            {
                iNodeAsOutputNode = Tree.OutputNode;
                iNodeAsOutputNode.SetInput(0, connection.Value.OutputNode);
            }

            Line newLine = new Line()
			{
				DataContext = connection.Value,
				Stroke = getColorFromType(iHandle.DataTypeName),
				StrokeThickness = 2
			};
			BindingOperations.SetBinding(newLine, Line.X1Property,
				new Binding("(Position)")
				{
					Source = oHandle,
					Converter = this.connectionLineCoordConverter,
					ConverterParameter = new Tuple<bool, UcIOHandle>(true, oHandle)
				});
			BindingOperations.SetBinding(newLine, Line.Y1Property,
				new Binding("(Position)")
				{
					Source = oHandle,
					Converter = this.connectionLineCoordConverter,
					ConverterParameter = new Tuple<bool, UcIOHandle>(false, oHandle)
				});
			BindingOperations.SetBinding(newLine, Line.X2Property,
				new Binding("(Position)")
				{
					Source = iHandle,
					Converter = this.connectionLineCoordConverter,
					ConverterParameter = new Tuple<bool, UcIOHandle>(true, iHandle)
				});
			BindingOperations.SetBinding(newLine, Line.Y2Property,
				new Binding("(Position)")
				{
					Source = iHandle,
					Converter = this.connectionLineCoordConverter,
					ConverterParameter = new Tuple<bool, UcIOHandle>(false, iHandle)
				});

			Panel.SetZIndex(newLine, -1);
			this.connectionLines.Add(connection.Value, newLine);
			this.cnvBackground.Children.Add(newLine);
		}

        private SolidColorBrush getColorFromType(String type)
        {
            SolidColorBrush color = new SolidColorBrush();

            switch (type)
            {
                case "double":
                    color = (SolidColorBrush)Application.Current.FindResource("ColorDouble");
                    break;
                case "boolean":
                    color = (SolidColorBrush)Application.Current.FindResource("ColorBoolean");
                    break;
                case "long":
                    color = (SolidColorBrush)Application.Current.FindResource("ColorLong");
                    break;
                case "Map":
                    color = (SolidColorBrush)Application.Current.FindResource("ColorMap");
                    break;
                case "Vector3d":
                    color = (SolidColorBrush)Application.Current.FindResource("ColorVector3d");
                    break;
                case "Collection":
                    color = (SolidColorBrush)Application.Current.FindResource("ColorCollection");
                    break;
                case "Buffer":
                    color = (SolidColorBrush)Application.Current.FindResource("ColorBuffer");
                    break;
                case "int":
                    color = (SolidColorBrush)Application.Current.FindResource("Colorint");
                    break;
                case "Picture":
                    color = (SolidColorBrush)Application.Current.FindResource("ColorPicture");
                    break;
                default:
                    color = (SolidColorBrush)Application.Current.FindResource("ColorDefault");
                    break;
            }

            return color;
        }

        private void AdjustZIndex()
		{
			int i = 0;
			foreach (var nodeViewer in this.cnvBackground.Children.OfType<UcNodeViewer>().OrderBy(Panel.GetZIndex))
			{
				Panel.SetZIndex(nodeViewer, i++);
			}

			if (this.SelectedNodeViewers.Count != 0)
			{
				i = Int32.MaxValue - 2;
				foreach (var nodeViewer in this.SelectedNodeViewers.OrderBy(Panel.GetZIndex))
				{
					Panel.SetZIndex(nodeViewer, i--);
				}

				Panel.SetZIndex(this.SelectedNodeViewers.First(), Int32.MaxValue - 1);
			}
		}

		private Point GetCanvasElementPosition(FrameworkElement element, bool center)
		{
			Point p = center ? new Point(element.ActualWidth / 2, element.ActualHeight / 2) : new Point();

			return element.TransformToAncestor(this.cnvBackground).Transform(p);
		}

        private Point GetAncestorElementPosition(FrameworkElement element, FrameworkElement ancestor, bool center)
        {
            Point p = center ? new Point(element.ActualWidth / 2, element.ActualHeight / 2) : new Point();

            return element.TransformToAncestor(ancestor).Transform(p);
        }

        private void MakeBindings(UcNodeViewer nodeViewer)
		{
			this.MakeCanvasOffsetBinding(nodeViewer, false);
			this.MakeCanvasOffsetBinding(nodeViewer, true);

			nodeViewer.CustomDrag += this.UcNodeViewer_StartedMoving;
        }

		private void MakeCanvasOffsetBinding(UcNodeViewer nodeViewer, bool isVertical)
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

			BindingOperations.SetBinding(nodeViewer, property, binding);
		}

		private void MarkNodesInSelectionArea(bool select)
		{
			IEnumerable<UcNodeViewer> nodeViewers = this.cnvBackground.Children.OfType<UcNodeViewer>();
			if (select)
			{
				nodeViewers = nodeViewers.OrderByDescending(Panel.GetZIndex);
			}

			foreach (var nodeViewer in nodeViewers)
			{
				double centerX = Canvas.GetLeft(nodeViewer) + nodeViewer.ActualWidth / 2;
				double centerY = Canvas.GetTop(nodeViewer) + nodeViewer.ActualHeight / 2;
				Rect selectionBorder = new Rect(new Point(Canvas.GetLeft(this.bdrSelectionBorder), Canvas.GetTop(this.bdrSelectionBorder)), this.bdrSelectionBorder.RenderSize);
				bool inside = selectionBorder.Contains(centerX, centerY);
				nodeViewer.IsInSelectionArea = !select && inside;
				if (inside && select)
				{
					this.SelectedNodeViewers.Add(nodeViewer);
				}
			}
		}

		private void OnTreeChanged(TransformationTree oldTree, TransformationTree newTree)
		{
			foreach (var nodeViewer in this.nodeViewers.Values)
			{
				this.SeverTiesToNodeViewer(nodeViewer);
			}

			foreach (var line in this.connectionLines.Values)
			{
				BindingOperations.ClearAllBindings(line);
			}

			this.cnvBackground.Children.Clear();
			this.nodeViewers.Clear();
			this.connectionLines.Clear();
			if (newTree != null)
			{
				if (newTree.OutputNode == null)
				{
					newTree.OutputNode = new OutputNode() { Tree = newTree };
                    newTree.Nodes.Add(newTree.OutputNode);
				}

				this.outOutputViewer.DataContext = newTree.OutputNode;
				this.cnvBackground.Children.Add(this.bdrSelectionBorder);
				this.cnvBackground.Children.Add(this.linPreviewConnection);
				this.cnvBackground.Children.Add(this.outOutputViewer);
				this.outOutputViewer.DataContext = newTree.OutputNode;

				this.outOutputViewer.BringIntoView();

				newTree.Nodes.CollectionChanged += TreeNodes_CollectionChanged;
				newTree.Connections.CollectionChanged += TreeConnections_CollectionChanged;
			}

			if (oldTree != null)
			{
				newTree.Nodes.CollectionChanged -= TreeNodes_CollectionChanged;
				newTree.Connections.CollectionChanged -= TreeConnections_CollectionChanged;
			}
		}

		public void RemoveConnection(UcIOHandle handle, Connection? connection)
		{
			if (this.Tree == null || handle == null)
			{
				return;
			}

			if (connection == null)
			{
				if (handle.IsInput)
				{
					connection = this.Tree.Connections.Cast<Connection?>()
						.FirstOrDefault(c => object.Equals(c.Value.InputNode, handle.Node) && handle.InputIndex == c.Value.InputIndex);
				}
				else
				{
					connection = this.Tree.Connections.Cast<Connection?>()
						.FirstOrDefault(c => object.Equals(c.Value.OutputNode, handle.Node));
				}
			}

			if (connection != null)
			{
				this.Tree.Connections.Remove(connection.Value);

				Node iNode = connection.Value.InputNode;
				Transformation iNodeAsTransformation;
                CombinedNode iNodeAsCombined;
				OutputNode iNodeAsOutputNode;
				if ((iNodeAsTransformation = iNode as Transformation) != null)
				{
					iNodeAsTransformation.SetInput(connection.Value.InputIndex, null);
				}
                else if ((iNodeAsCombined = iNode as CombinedNode) != null)
                {
                    iNodeAsCombined.SetInput(connection.Value.InputIndex, null);
                }
                else if ((iNodeAsOutputNode = iNode as OutputNode) != null)
				{
					iNodeAsOutputNode.SetInput(0, null);
				}

				Line line = null;
				if (this.connectionLines.TryGetValue(connection.Value, out line))
				{
					this.connectionLines.Remove(connection.Value);
					BindingOperations.ClearAllBindings(line);
					this.cnvBackground.Children.Remove(line);
				}
			}
		}

		private void RemoveNode(UcNodeViewer nodeViewer)
		{
			if (this.Tree == null || nodeViewer is UcOutputViewer)
			{
				return;
			}

			foreach (var handle in nodeViewer.IoHandles)
			{
				this.RemoveConnection(handle, null);
			}

			this.Tree.Nodes.Remove(nodeViewer.Node);
			this.nodeViewers.Remove(nodeViewer.Node);
			this.cnvBackground.Children.Remove(nodeViewer);
			this.SeverTiesToNodeViewer(nodeViewer);
		}

		private void SeverTiesToNodeViewer(UcNodeViewer nodeViewer)
		{
			BindingOperations.ClearAllBindings(nodeViewer);
			nodeViewer.CustomDrag -= this.UcNodeViewer_StartedMoving;
			foreach (var handle in nodeViewer.IoHandles)
			{
                handle.unregisterHandlers();
				this.registeredIoHandles.Remove(handle);
			}
		}

		private void StopDragging(MouseButtonEventArgs e, bool saveTransaction = false)
		{
            if (movingNodeViewer != null)
            {
                //Save the Transaction before actioning.
                if (saveTransaction && e != null)
                {
                    this.undoStack.Push(new UITransactionMoveElements(
                            this.SelectedNodeViewers.Select(v => v.Node).ToArray(),
                            e.GetPosition(this.cnvBackground).X - dragStart.Value.X,
                            e.GetPosition(this.cnvBackground).Y - dragStart.Value.Y
                        ));
                }


                if ((Keyboard.Modifiers & ModifierKeys.Shift) > 0)
                {
                    foreach (UcNodeViewer nodeViewer in SelectedNodeViewers)
                    {
                        snapToGrid(nodeViewer);
                    }
                }
            }


            this.mousePosOnViewer = null;
            this.dragStart = null;
            this.lastScrollPosition = null;
            this.movingNodeViewer = null;

			if (this.bdrSelectionBorder.Visibility == Visibility.Visible)
			{
				this.MarkNodesInSelectionArea(true);

				this.bdrSelectionBorder.Visibility = Visibility.Collapsed;
			}
			if (this.ConnectingIOHandle != null)
			{
				this.linPreviewConnection.Visibility = Visibility.Collapsed;

				this.TryToConnect(e);

				this.ConnectingIOHandle = null;
			}
		}

		private void TryToConnect(MouseButtonEventArgs e)
		{
			if (e != null && this.ConnectingIOHandle != null)
			{
				Point mousePos = e.GetPosition(this.cnvBackground);
				UcIOHandle handleUnderCursor = null;
				int zIndex = Int32.MinValue;
				foreach (var ioHandle in this.registeredIoHandles)
				{
                    if (ioHandle != this.ConnectingIOHandle)
					{
                        Point handlePos = this.GetCanvasElementPosition(ioHandle, false);
						if (new Rect(handlePos, ioHandle.RenderSize).Contains(mousePos) &&
							(handleUnderCursor == null || Panel.GetZIndex(ioHandle) > zIndex))
						{
							zIndex = Panel.GetZIndex(ioHandle);
							handleUnderCursor = ioHandle;
						}
					}
				}

				if (handleUnderCursor != null && handleUnderCursor.IsConnectionCompatible == true)
				{
					UcIOHandle iHandle, oHandle;
					if (handleUnderCursor.IsInput)
					{
						iHandle = handleUnderCursor;
						oHandle = this.ConnectingIOHandle;
					}
					else
					{
						iHandle = this.ConnectingIOHandle;
						oHandle = handleUnderCursor;
					}

					this.AddConnection(oHandle, iHandle, null);
				}
			}
		}



        public bool CanUnmerge()
        {
            return this.SelectedNodeViewers.Count == 1
                && this.SelectedNodeViewers[0].Node is CombinedNode;
        }

        /// <summary>
        /// Tries to unmerge a Node.
        /// Only works for Combined nodes.
        /// Null is a valid node arg.
        /// </summary>
        /// <param name="node">To unmerge</param>
        public void TryUnmergeSelectedNode()
        {
            if (!CanUnmerge())
            {
                return;
            }

            CombinedNode node = this.SelectedNodeViewers[0].Node as CombinedNode;
            if(node == null)
            {
                return;
            }

            //Get a name for the New Element:
            MessageBoxResult result = MessageBox.Show("Do you want to unmerge " + node.Name + "?", "Unmerge " + node.Name, MessageBoxButton.OKCancel, MessageBoxImage.Question, MessageBoxResult.Cancel);
            if (result != MessageBoxResult.OK) return;

            UnmergeNode(node, true);
        }

        /// <summary>
        /// Unmerges the node passed.
        /// </summary>
        /// <param name="node">To unmerge</param>
        public void UnmergeNode(CombinedNode node, bool saveTransaction = false)
        {
            if (node == null) return;

            //Preserve the input Connections:
            List<Connection> newConnections = new List<Connection>();
            for (int i = 0; i < node.InputNodes.Count(); i++)
            {
                Node output = node.InputNodes[i];
                Tuple<Node, int> input = node.inputConnections.GetValue(i, new Tuple<Node, int>(null, 0));
                if (input != null && output != null)
                {
                    newConnections.Add(new Connection(output, input.Item1, input.Item2));
                }
            }

            //Build the internal Connections:
            foreach (Node internalNode in node.includedNodes)
            {
                List<Node> inputs = internalNode.InputNodes.ToList();
                for (int i = 0; i < inputs.Count(); i++)
                {
                    Node internalInput = inputs[i];
                    if (internalInput != null) newConnections.Add(new Connection(internalInput, internalNode, i));
                }
            }

            //find if output node is connected:
            Connection outputConnection = Tree.Connections
                .FirstOrDefault(c => c.OutputNode == node);

            if (outputConnection.InputNode != null && outputConnection.OutputNode != null)
            {
                RemoveConnection(outputConnection);
                newConnections.Add(new Connection(node.outputNode, outputConnection.InputNode, outputConnection.InputIndex));
            }

            //Save the Transaction if needed.
            //Be sure to save, before we remove the Stuff.
            if (saveTransaction)
            {
                Connection[] oldConnections = Tree.Connections
                    .Where(c => (c.InputNode == node || c.OutputNode == node))
                    .ToArray();
                this.undoStack.Push(new UITransactionUnmerge(node, newConnections.ToArray(), oldConnections));
                this.redoStack.Clear();
            }

            //Remove the old node:
            RemoveNode(node);

            //add all internal nodes to the System:
            node.includedNodes
                .ForEach(n => n.ClearInputs())
                .ForEach(n => n.PosX += node.PosX)
                .ForEach(n => n.PosY += node.PosY)
                .ForEach(n => AddNode(n));

            //Now add the Connections:
            newConnections.ForEach(AddConnection);
        }


        /// <summary>
        /// Checks if we can merge the current selection. 
        /// </summary>
        /// <returns>true if can be merged.</returns>
        public bool CanMergeCurrentSelection()
        {
            //Get all selected Nodes:
            var nodes = new List<UcNodeViewer>(this.SelectedNodeViewers).Select(v => v.Node).Distinct().ToList();

            //Check if any nodes highlighted:
            if (nodes.Count <= 1) return false;

            // Ensure OutputNode is not selected
            foreach(Node node in nodes)
            {
                if (node is OutputNode) return false;
            }

            //Check if connected:
            CombinedClassGenerator generator = new CombinedClassGenerator(nodes);
            if (!generator.IsConnected()) return false;
            if (generator.IsCyclic()) return false;

            return true;
        }


        /// <summary>
        /// Does a Merge action on the current selection.
        /// </summary>
        public void TryMergeCurrentSelection()
        {
            if (!CanMergeCurrentSelection()) return;

            //Get a name for the New Element:
            IEnumerable<Node> nodes = this.SelectedNodeViewers.Select(v => v.Node).Distinct();
            CombinedClassGenerator generator = new CombinedClassGenerator(nodes);

            MessageBoxResult result = MessageBox.Show("Generate a new Element out of " + nodes.Count() + " Elements?", "Merge", MessageBoxButton.OKCancel, MessageBoxImage.Question, MessageBoxResult.Cancel);
            if (result != MessageBoxResult.OK) return;

            string newName = "";
            while (newName == "")
            {
                DialogCombinedName dialog = new DialogCombinedName();
                dialog.CombinedElementName = "New Name";
                if (dialog.ShowDialog() == true)
                {
                    if (!string.IsNullOrWhiteSpace(dialog.CombinedElementName))
                    {
                        newName = dialog.CombinedElementName;
                        if (generator.ExistsName(newName))
                        {
                            MessageBox.Show("The name " + newName + " already exists. Please choose another one!", "Name taken");
                            newName = "";
                        }
                    }else
                    {
                        MessageBox.Show("An empty name is not supported. Aborting.", "Abort Merging");
                        return;
                    }
                }else
                {
                    return;
                }
            }

            generator.Name = newName;
            AbstractNodeClass generatedClass = generator.GenerateClass();
            if (generatedClass == null) return;

            //Save the just generated class:
            generator.SaveToDisc();

            //Register the new Node:
            ClassManager classManager = Singleton<ClassManager>.Instance;
            classManager.AddClass(generatedClass);

            //Generate the own Node:
            Node newNode = generatedClass.generateNode();
            newNode.PosX = nodes.Average(n => n.PosX);
            newNode.PosY = nodes.Average(n => n.PosY);

            //add the new Node:
            AddNode(newNode, false);

            //Change the Connections:
            int index = 0;
            foreach (Node node in nodes)
            {
                AbstractNodeClass nodeClass = node.Class;
                List<Node> nodeInputs = node.InputNodes.ToList();
                List<IOData> inputIOData = node.InputIOData.ToList();

                for (int i = 0; i < nodeInputs.Count(); i++)
                {
                    Node subNode = nodeInputs[i];
                    IOData ioData = inputIOData[i];

                    //We found an IOdata.
                    if (newNode.InputIOData.Contains(ioData))
                    {
                        index++;
                    }

                    //We found a connection.
                    if (subNode == null || !nodes.Contains(subNode))
                    {
                        if(subNode != null) AddConnection(new Connection(subNode, newNode, index));
                        index++;
                    }
                }
            }

            //Check for the output connection:
            Node root = generator.GetRootNode();
            if(root != null)
            {
                //I hate Structs....
                Connection? rootOutputConnection = Tree.Connections
                    .Where((c) => c.OutputNode == root)
                    .Cast<Connection?>()
                    .FirstOrDefault();

                if (rootOutputConnection != null)
                {
                    RemoveConnection(rootOutputConnection.Value);
                    AddConnection(new Connection(newNode, rootOutputConnection.Value.InputNode, rootOutputConnection.Value.InputIndex));
                }
            }

            //At end -> Remove old ones!
            foreach (Node old in nodes) RemoveNode(old);
        }

        /// <summary>
        /// This clears the complete GUI.
        /// </summary>
        public void Clear(bool saveTransaction = false)
        {
            Node[] toRemove = this.nodeViewers.Keys
                .ToArray()
                .Where(n => !(n is OutputNode))
                .ToArray();

            RemoveNodes(toRemove, saveTransaction);
        }


        #endregion methods

        #region event handlers

        private void cnvBackground_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
		{
			this.SelectedNodeViewers.Clear();
		}

		private void cnvBackground_MouseRightButtonDown(object sender, MouseButtonEventArgs e)
		{
		//	this.SelectedNodeViewers.Clear();
		}

		private void IoHandle_CustomDrag(object sender, EventArgs e)
		{
			this.ConnectingIOHandle = (UcIOHandle)sender;
			this.RemoveConnection(this.ConnectingIOHandle, null);
		}

		private void SelectedNodeViewers_CollectionChanged(object sender, NotifyCollectionChangedEventArgs e)
		{
			this.changingSelectedNodeViewers = true;

			this.SelectedNodeViewer = this.SelectedNodeViewers.Count != 0 ?
				this.SelectedNodeViewers.First() :
				null;

			if (e.Action == NotifyCollectionChangedAction.Reset)
			{
				foreach (UcNodeViewer nodeViewer in this.previouslySelectedItems)
				{
					nodeViewer.IsSelected = false;
				}
			}
			else
			{
				if (e.NewItems != null)
				{
					foreach (UcNodeViewer nodeViewer in e.NewItems.Cast<UcNodeViewer>())
					{
						nodeViewer.IsSelected = true;
					}
				}
				if (e.OldItems != null)
				{
					foreach (UcNodeViewer nodeViewer in e.OldItems.Cast<UcNodeViewer>())
					{
						nodeViewer.IsSelected = false;
					}
				}

				this.AdjustZIndex();
			}

			this.previouslySelectedItems = this.SelectedNodeViewers.ToArray();

            for (int i = 0; i < SelectedNodeViewers.Count; i++)
            {
                System.Diagnostics.Debug.Print("SelectedNodeViewers_CollectionChanged | selection " + i+" : " + SelectedNodeViewers.ElementAt(i));
            }

			this.changingSelectedNodeViewers = false;
		}

		private void This_DragOver(object sender, DragEventArgs e)
		{
		}

		private void This_Drop(object sender, DragEventArgs e)
        {
            Node node = ((Tuple<Node>)e.Data.GetData(typeof(Tuple<Node>))).Item1;
			Point mousePos = e.GetPosition(this.cnvBackground);
			Node newNode = node.Clone();
            newNode.PosX = mousePos.X - this.cnvBackground.ActualWidth / 2;
			newNode.PosY = mousePos.Y - this.cnvBackground.ActualHeight / 2;
            this.AddNode(newNode, true, true);
            scvCanvas.Focus();
        }

        public void onDeleteCommand()
        {
            RemoveNodes(
                this.SelectedNodeViewers
                    .Select(v => v.Node)
                    .ToArray()
                , true);

            SelectedNodeViewer = null;
        }


        public bool CanUndo()
        {
            return undoStack.Count > 0;
        }


        public void Undo()
        {
            //If we have nothing to Undo, we can't
            if (undoStack.Empty())
            {
                return;
            }

            //First undo the Action.
            UITransaction toUndo = undoStack.Pop();
            toUndo.Undo(this);

            //Then push to Redo stack if we want to apply it.
            redoStack.Push(toUndo);
        }


        public bool CanRedo()
        {
            return redoStack.Count > 0;
        }


        public void Redo()
        {
            //If we have nothing to redo, we can't
            if (redoStack.Empty())
            {
                return;
            }

            //First Redo the Action.
            UITransaction toRedo = redoStack.Pop();
            toRedo.Redo(this);

            //Then push to Undo stack if we want to undo it.
            undoStack.Push(toRedo);
        }


        public void onNodeViewerSelectAdded(UcNodeViewer nodeViewer)
        {
            this.changingSelectedNodeViewers = true;
            nodeViewer.IsSelected = true;
            this.changingSelectedNodeViewers = false;
        }

        public void onNodeViewerSelectRemoved(UcNodeViewer nodeViewer)
        {
            this.changingSelectedNodeViewers = true;
            nodeViewer.IsSelected = false;
            SelectedNodeViewers.Remove(nodeViewer);
            this.changingSelectedNodeViewers = false;
        }

        public void onNodeViewerDoubleClick(UcNodeViewer nodeViewer)
        {
            nodeViewer.IsSelected = true;
            List<UcNodeViewer> connectedNodeList = new List<UcNodeViewer>();
            connectedNodeList.Add(nodeViewer);
            int i = 0;
            while (i < connectedNodeList.Count)
            {
                foreach (Connection connection in Tree.Connections)
                {
                    if (connection.InputNode.Equals(connectedNodeList.ElementAt(i).Node) || connection.OutputNode.Equals(connectedNodeList.ElementAt(i).Node))
                    {
                        UcNodeViewer connectedViewer;
                        Node connectedNode = connection.InputNode.Equals(connectedNodeList.ElementAt(i).Node) ? connection.OutputNode : connection.InputNode;
                        if (connectedNode as OutputNode != null)
                        {
                            connectedViewer = outOutputViewer;
                        }
                        else
                        {
                            nodeViewers.TryGetValue(connectedNode, out connectedViewer);
                        }
                        if (!connectedNodeList.Contains(connectedViewer) && connectedViewer != null)
                        {
                            connectedNodeList.Add(connectedViewer);
                            this.changingSelectedNodeViewers = true;
                            connectedViewer.IsSelected = true;
                            this.changingSelectedNodeViewers = false;
                        }
                    }
                }
                i++;
            }
        }

        public void onNodeViewerSelected(UcNodeViewer nodeViewer)
        {
            scvCanvas.Focus();
            if (changingSelectedNodeViewers && !SelectedNodeViewers.Contains(nodeViewer))
            {
                SelectedNodeViewers.Add(nodeViewer);
            }
            else
            {
                SelectedNodeViewer = nodeViewer;
            }
        }

        private void snapToGrid(UcNodeViewer nodeViewer)
        {
            double left = Canvas.GetLeft(nodeViewer);
            double deltaX = ((left % gridSize) > (gridSize / 2)) ? (left % gridSize) - gridSize : (left % gridSize);
            nodeViewer.Node.PosX = nodeViewer.Node.PosX - deltaX;
            double top = Canvas.GetTop(nodeViewer);
            double deltaY = ((top % gridSize) > (gridSize / 2)) ? (top % gridSize) - gridSize : (top % gridSize);
            nodeViewer.Node.PosY = nodeViewer.Node.PosY - deltaY;
        }


        private void This_Loaded(object sender, RoutedEventArgs e)
		{
			this.scvCanvas.ScrollToHorizontalOffset(GetOffset(this.vbBackground.ActualWidth, this.scvCanvas.ViewportWidth));
			this.scvCanvas.ScrollToVerticalOffset(GetOffset(this.vbBackground.ActualHeight, this.scvCanvas.ViewportHeight));
		}

		private void This_MouseLeave(object sender, MouseEventArgs e)
		{
			this.StopDragging(null, true);
		}

		private void This_MouseMove(object sender, MouseEventArgs e)
		{
			if (e.LeftButton == MouseButtonState.Pressed)
			{
				Point mousePos = e.GetPosition(this.cnvBackground);
				if (this.dragStart == null)
				{
					this.dragStart = mousePos;
				}

				if (this.movingNodeViewer != null)
				{
					if (this.mousePosOnViewer == null)
					{
						this.mousePosOnViewer = Mouse.GetPosition(this.movingNodeViewer);
					}

					Node node = (Node)this.movingNodeViewer.DataContext;
					double dx = mousePos.X - this.mousePosOnViewer.Value.X + (this.movingNodeViewer.ActualWidth - this.cnvBackground.ActualWidth) / 2 - node.PosX;
					double dy = mousePos.Y - this.mousePosOnViewer.Value.Y + (this.movingNodeViewer.ActualHeight - this.cnvBackground.ActualHeight) / 2 - node.PosY;
					foreach (var nodeViewer in this.SelectedNodeViewers)
					{
						node = (Node)nodeViewer.DataContext;
						node.PosX += dx;
						node.PosY += dy;
					}
				}
				else if (this.ConnectingIOHandle != null)
				{
					this.linPreviewConnection.Visibility = Visibility.Visible;

					Point pos = this.GetCanvasElementPosition(this.ConnectingIOHandle, true);
					this.linPreviewConnection.X1 = pos.X;
					this.linPreviewConnection.Y1 = pos.Y;
					this.linPreviewConnection.X2 = mousePos.X;
					this.linPreviewConnection.Y2 = mousePos.Y;
				}
                else if ((Keyboard.Modifiers & ModifierKeys.Alt) > 0) {
                    if(lastScrollPosition == null)
                    {
                        lastScrollPosition = Mouse.GetPosition(scvCanvas);
                    }
                    double dx = Mouse.GetPosition(scvCanvas).X - lastScrollPosition.Value.X;
                    double dy = Mouse.GetPosition(scvCanvas).Y - lastScrollPosition.Value.Y;
                    scvCanvas.ScrollToHorizontalOffset(scvCanvas.HorizontalOffset - dx);
                    scvCanvas.ScrollToVerticalOffset(scvCanvas.VerticalOffset - dy);
                    lastScrollPosition = Mouse.GetPosition(scvCanvas);
                }
				else {
					this.bdrSelectionBorder.Visibility = Visibility.Visible;
					double dx = mousePos.X - this.dragStart.Value.X;
					double dy = mousePos.Y - this.dragStart.Value.Y;
					double x = this.dragStart.Value.X;
					if (dx < 0)
					{
						x += dx;
					}
					double y = this.dragStart.Value.Y;
					if (dy < 0)
					{
						y += dy;
					}

					Canvas.SetLeft(this.bdrSelectionBorder, x);
					Canvas.SetTop(this.bdrSelectionBorder, y);
					this.bdrSelectionBorder.Width = Math.Abs(dx);
					this.bdrSelectionBorder.Height = Math.Abs(dy);

					this.MarkNodesInSelectionArea(false);
				}
			}
		}

		private void This_PreviewMouseLeftButtonUp(object sender, MouseButtonEventArgs e)
		{
			this.StopDragging(e, true);
		}

		private void TreeConnections_CollectionChanged(object sender, NotifyCollectionChangedEventArgs e)
		{
		}

		private void TreeNodes_CollectionChanged(object sender, NotifyCollectionChangedEventArgs e)
		{
		}

		private void UcNodeViewer_StartedMoving(object sender, EventArgs e)
		{
            if (ConnectingIOHandle == null)
            {
                this.movingNodeViewer = (UcNodeViewer)sender;
            }
		}


        private void Canvas_MouseWheel(object sender, MouseWheelEventArgs e)
        {
            if (Keyboard.IsKeyDown(Key.LeftAlt) || Keyboard.IsKeyDown(Key.RightAlt))
            {
                if (scaletransform == null)
                {
                    scaletransform = new ScaleTransform(1, 1);
                    cnvBackground.RenderTransform = scaletransform;
                    scaletransform.CenterX = cnvBackground.Width / 2;
                    scaletransform.CenterY = cnvBackground.Height / 2;
                }

                if (e.Delta > 0)
                {
                    scaletransform.ScaleX *= scaleRate;
                    scaletransform.ScaleY *= scaleRate;
                }
                else
                {
                    scaletransform.ScaleX /= scaleRate;
                    scaletransform.ScaleY /= scaleRate;
                }

                e.Handled = true;
            }
            if (Keyboard.IsKeyDown(Key.LeftShift) || Keyboard.IsKeyDown(Key.RightShift))
            {
                if (e.Delta < 0)
                {
                    scvCanvas.LineRight();
                }
                else
                {
                    scvCanvas.LineLeft();
                }
                e.Handled = true;
            }
            }

        #endregion event handlers

        #region test area / not sorted yet


        #endregion test area / not sorted yet
    }
}