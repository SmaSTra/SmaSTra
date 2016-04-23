namespace Common.WpfControls
{
	using System;
	using System.Collections.Generic;
	using System.Windows;
	using System.Windows.Controls;
	using System.Windows.Controls.Primitives;
	using System.Windows.Input;

	using Common.WpfControls.DataGridExpanderColumnHelperClasses;

	/// <summary>
	/// A Column for the DataGrid or UcGdmDataGrid control, that enables the user to
	/// manually expand/collapse the details for each row.
	/// 
	/// Prefix: exc
	/// </summary>
	public class DataGridExpanderColumn : DataGridTemplateColumn
	{
		#region static attributes

		/// <summary>
		/// Registration of RememberExpandedRows DependencyProperty.
		/// </summary>
		public static readonly DependencyProperty RememberExpandedRowsProperty = 
			DependencyProperty.Register(
				"RememberExpandedRows", typeof(bool), typeof(DataGridExpanderColumn),
				new UIPropertyMetadata(
					true,
					OnRememberExpandedRowsChanged));

		#endregion static attributes

		#region attributes

		/// <summary>
		/// A Hashset for the items that have previously been expanded.
		/// Is used to reexpand items when the DataGrid's ItemsSource.
		/// </summary>
		private HashSet<object> expandedItems = new HashSet<object>();

		/// <summary>
		/// Specifies whether a left mouse button click on the whole row triggers
		/// the row's details to expand/collapse.
		/// </summary>
		private bool expandOnClickOnRow = true;

		/// <summary>
		/// Specifies whether the DataGridOwner property is set to a valid
		/// value.
		/// </summary>
		private bool ownerNotFound = true;

		#endregion attributes

		#region events

		/// <summary>
		/// Occurs when an item is collapsed.
		/// </summary>
		public event EventHandler<ExpansionStatusChangedEventArgs> ItemCollapsed;

		/// <summary>
		/// Occurs when an item is expanded.
		/// </summary>
		public event EventHandler<ExpansionStatusChangedEventArgs> ItemExpanded;

		#endregion events

		#region constructors

		/// <summary>
		/// Initializes a new instance of the <see cref="DataGridExpanderColumn"/> class.
		/// </summary>
		public DataGridExpanderColumn()
			: base()
		{
			// Initialize this column.
			this.IsReadOnly = true;
			this.CanUserReorder = false;
			this.CanUserResize = false;
			this.CanUserSort = false;

			// Create a template for the column's cells
			FrameworkElementFactory factory = new FrameworkElementFactory(typeof(TextBlock));
			factory.SetValue(TextBlock.TextProperty, "+");
			factory.SetValue(TextBlock.HorizontalAlignmentProperty, HorizontalAlignment.Stretch);
			factory.SetValue(TextBlock.VerticalAlignmentProperty, VerticalAlignment.Stretch);
			factory.SetValue(TextBlock.TextAlignmentProperty, TextAlignment.Center);
			factory.SetValue(TextBlock.IsHitTestVisibleProperty, true);
			factory.AddHandler(TextBlock.LoadedEvent, new RoutedEventHandler(this.ExpansionTrigger_Loaded));
			factory.AddHandler(TextBlock.MouseLeftButtonUpEvent, new MouseButtonEventHandler(this.ExpansionTrigger_MouseLeftButtonUp));

			// Apply the template.
			this.CellTemplate = new DataTemplate()
			{
				VisualTree = factory
			};
			this.CellEditingTemplate = this.CellTemplate;
		}

		#endregion constructors

		#region properties

		/// <summary>
		/// Gets or sets a Functions which specifies whether an item is supposed
		/// to be expandable or not.
		/// The object argument is the item.
		/// </summary>
		public Func<object, bool> ExpandableCondition
		{
			get;
			set;
		}

		/// <summary>
		/// Gets or sets a value indicating whether a left mouse button click on the whole row triggers
		/// the row's details to expand/collapse.
		/// </summary>
		public bool ExpandOnClickOnRow
		{
			get { return this.expandOnClickOnRow; }
			set
			{
				bool oldValue = this.expandOnClickOnRow;
				this.expandOnClickOnRow = value;

				if (this.DataGridOwner != null && value != oldValue)
				{
					if (this.ExpandOnClickOnRow)
					{
						this.DataGridOwner.MouseLeftButtonUp += new MouseButtonEventHandler(DataGridOwner_MouseLeftButtonUp);
					}
					else
					{
						this.DataGridOwner.MouseLeftButtonUp -= new MouseButtonEventHandler(DataGridOwner_MouseLeftButtonUp);
					}
				}
			}
		}

		/// <summary>
		/// Gets or sets whether the rows that were expanded once shoult automatically
		/// expanded in case the DataGrid's ItemsSource is updated or something else
		/// happens that causes the Rows to reload.
		/// </summary>
		public bool RememberExpandedRows
		{
			get { return (bool)GetValue(RememberExpandedRowsProperty); }
			set { SetValue(RememberExpandedRowsProperty, value); }
		}

		/// <summary>
		/// Gets or sets the type of items the user can expand.
		/// </summary>
		public Type TypeToExpand
		{
			get;
			set;
		}

		#endregion properties

		#region methods

		/// <summary>
		/// Collapses all items.
		/// </summary>
		public void CollapseAll()
		{
			if (this.DataGridOwner != null)
			{
				foreach (object item in this.DataGridOwner.Items)
				{
					this.CollapseItem(item);
				}
			}
		}

		/// <summary>
		/// Collapses the given item.
		/// </summary>
		/// <param name="item">The item.</param>
		public void CollapseItem(object item)
		{
			if (this.DataGridOwner != null)
			{
				TextBlock expander = LayoutHelper.FindVisualChild<TextBlock>(this.GetCellContent(item));

				expander.Text = "+";
				this.DataGridOwner.SetDetailsVisibilityForItem(item, Visibility.Collapsed);
				if (this.RememberExpandedRows && this.expandedItems.Contains(item))
				{
					this.expandedItems.Remove(item);
				}

				this.OnItemCollapsed(item);
			}
		}

		/// <summary>
		/// Expands all items.
		/// </summary>
		public void ExpandAll()
		{
			if (this.DataGridOwner != null)
			{
				foreach (object item in this.DataGridOwner.Items)
				{
					this.ExpandItem(item);
				}
			}
		}

		/// <summary>
		/// Expands the given item.
		/// </summary>
		/// <param name="item">The item.</param>
		public void ExpandItem(object item)
		{
			if (this.DataGridOwner != null && this.IsItemExpandable(item))
			{
				TextBlock expander = LayoutHelper.FindVisualChild<TextBlock>(this.GetCellContent(item));

				expander.Text = "-";
				this.DataGridOwner.SetDetailsVisibilityForItem(item, Visibility.Visible);
				if (this.RememberExpandedRows)
				{
					this.expandedItems.Add(item);
				}

				this.OnItemExpanded(item);
			}
		}

		/// <summary>
		/// Refreshes the attributes of each row that define whether that row is expandable or not.
		/// </summary>
		public void RefreshExpandableAttributes()
		{
			if (this.DataGridOwner != null)
			{
				foreach (var item in this.DataGridOwner.Items)
				{
					FrameworkElement content = this.GetCellContent(item);
					if (content != null)
					{
						TextBlock expansionTrigger = LayoutHelper.FindVisualChild<TextBlock>(content);
						if (expansionTrigger != null)
						{
							this.RefreshExpandableAttribute(expansionTrigger);
						}
					}
				}
			}
		}

		/// <summary>
		/// Adds the DataGrid's event handlers.
		/// </summary>
		private void AddDataGridEventHandlers()
		{
			if (this.DataGridOwner != null)
			{
				if (this.ExpandOnClickOnRow)
				{
					this.DataGridOwner.MouseLeftButtonUp += new MouseButtonEventHandler(DataGridOwner_MouseLeftButtonUp);
				}
				this.DataGridOwner.Unloaded += new RoutedEventHandler(DataGridOwner_Unloaded);
			}
		}

		/// <summary>
		/// Determines whether the given item is expandable or not.
		/// </summary>
		private bool IsItemExpandable(object item)
		{
			if (item == null)
			{
				return false;
			}

			Type type = item.GetType();
			if (this.TypeToExpand != null && this.ExpandableCondition != null)
			{
				return (type == this.TypeToExpand || type.IsSubclassOf(this.TypeToExpand)) && this.ExpandableCondition(item);
			}
			else if (this.TypeToExpand != null)
			{
				return type == this.TypeToExpand || type.IsSubclassOf(this.TypeToExpand);
			}
			else if (this.ExpandableCondition != null)
			{
				return this.ExpandableCondition(item);
			}
			else
			{
				return true;
			}
		}

		/// <summary>
		/// Raises the <see cref="E:ItemCollapsed"/> event.
		/// </summary>
		/// <param name="item">The item.</param>
		private void OnItemCollapsed(object item)
		{
			if (this.ItemCollapsed != null)
			{
				this.ItemCollapsed(this, new ExpansionStatusChangedEventArgs(ChangeType.Collapsed, item, this.DataGridOwner));
			}
		}

		/// <summary>
		/// Raises the <see cref="E:ItemExpanded"/> event.
		/// </summary>
		/// <param name="item">The item.</param>
		private void OnItemExpanded(object item)
		{
			if (this.ItemExpanded != null)
			{
				this.ItemExpanded(this, new	ExpansionStatusChangedEventArgs(ChangeType.Expanded, item, this.DataGridOwner));
			}
		}

		/// <summary>
		/// Refreshes the attribute of a row that defines whether that row is expandable or not.
		/// </summary>
		/// <param name="expansionTrigger">The expansion trigger of that row.</param>
		private void RefreshExpandableAttribute(TextBlock expansionTrigger)
		{
			object item = expansionTrigger.DataContext;

			if (this.IsItemExpandable(item))
			{
				if (this.RememberExpandedRows && this.expandedItems.Contains(item))
				{
					this.DataGridOwner.SetDetailsVisibilityForItem(item, Visibility.Visible);
					expansionTrigger.Text = "-";
				}
				else
				{
					this.DataGridOwner.SetDetailsVisibilityForItem(item, Visibility.Collapsed);
				}

				expansionTrigger.Visibility = Visibility.Visible;
			}
			else
			{
				if (this.DataGridOwner.GetDetailsVisibilityForItem(item) == Visibility.Visible)
				{
					this.CollapseItem(item);
				}

				expansionTrigger.Visibility = Visibility.Collapsed;
			}
		}

		/// <summary>
		/// Removes the DataGrid's event handlers.
		/// </summary>
		private void RemoveDataGridEventHandlers()
		{
			if (this.DataGridOwner != null)
			{
				if (this.ExpandOnClickOnRow)
				{
					this.DataGridOwner.MouseLeftButtonUp -= new MouseButtonEventHandler(DataGridOwner_MouseLeftButtonUp);
				}
				this.DataGridOwner.Unloaded -= new RoutedEventHandler(DataGridOwner_Unloaded);
			}
		}

		/// <summary>
		/// Toggles the expansion of the selected item.
		/// </summary>
		private void ToggleExpansionOfSelected()
		{
			if (this.DataGridOwner != null)
			{
				object item = this.DataGridOwner.SelectedItem;
				if (item != null)
				{
					if (this.DataGridOwner.GetDetailsVisibilityForItem(item) == Visibility.Visible)
					{
						this.CollapseItem(item);
					}
					else
					{
						this.ExpandItem(item);
					}
				}
			}
		}

		#endregion methods

		#region overrides

		/// <summary>
		/// Override that forces the Column to allways be in ReadOnly mode.
		/// </summary>
		protected override bool OnCoerceIsReadOnly(bool baseValue)
		{
			return true;
		}

		#endregion overrides

		#region static event handlers

		/// <summary>
		/// Property Changed Callback method of the RememberExpanded Dependency Property.
		/// </summary>
		/// <param name="sender">The instance of the class that had the RememberExpanded property changed.</param>
		/// <param name="e">The <see cref="System.Windows.DependencyPropertyChangedEventArgs"/> instance containing the event data.</param>
		public static void OnRememberExpandedRowsChanged(object sender, DependencyPropertyChangedEventArgs e)
		{
			DataGridExpanderColumn subject = (DataGridExpanderColumn)sender;
			if ((bool)e.NewValue)
			{
				// If RememberExpandedRows is set to true
				// store all currently expanded items.
				if (subject.DataGridOwner != null)
				{
					foreach (object item in subject.DataGridOwner.Items)
					{
						if (subject.DataGridOwner.GetDetailsVisibilityForItem(item) == Visibility.Visible)
						{
							subject.expandedItems.Add(item);
						}
					}
				}
			}
			else
			{
				subject.expandedItems.Clear();
			}
		}

		#endregion static event handlers

		#region event handlers

		/// <summary>
		/// Handles the MouseLeftButtonUp event of the DataGridOwner control.
		/// </summary>
		/// <param name="sender">The source of the event.</param>
		/// <param name="e">The <see cref="System.Windows.Input.MouseButtonEventArgs"/> instance containing the event data.</param>
		private void DataGridOwner_MouseLeftButtonUp(object sender, MouseButtonEventArgs e)
		{
			// If expansion on click on row is enabled this event handler is called when
			// a user clicks on a row.

			// Make sure that the click was inside the selected row.
			if (this.DataGridOwner != null)
			{
				DataGridRow row = LayoutHelper.FindVisualParent<DataGridRow>(e.OriginalSource as UIElement);
				if (row != null && row.Item == this.DataGridOwner.SelectedItem &&
					LayoutHelper.FindVisualParent<DataGridDetailsPresenter>(e.OriginalSource as UIElement) == null)
				{
					// toggle expansion.
					this.ToggleExpansionOfSelected();
				}
			}
		}

		/// <summary>
		/// Handles the Unloaded event of the DataGridOwner control.
		/// </summary>
		/// <param name="sender">The source of the event.</param>
		/// <param name="e">The <see cref="System.Windows.RoutedEventArgs"/> instance containing the event data.</param>
		private void DataGridOwner_Unloaded(object sender, RoutedEventArgs e)
		{
			this.RemoveDataGridEventHandlers();
			this.ownerNotFound = true;
		}

		/// <summary>
		/// Handles the Loaded event of the TextBlock uset as expansion trigger.
		/// </summary>
		/// <param name="sender">The source of the event.</param>
		/// <param name="e">The <see cref="System.Windows.RoutedEventArgs"/> instance containing the event data.</param>
		private void ExpansionTrigger_Loaded(object sender, RoutedEventArgs e)
		{
			TextBlock subject = (TextBlock)sender;
			if (this.DataGridOwner != null && subject.DataContext != null)
			{
				if (this.ownerNotFound)
				{
					this.AddDataGridEventHandlers();

					this.ownerNotFound = false;
				}

				this.RefreshExpandableAttribute(subject);
			}
		}

		/// <summary>
		/// Handles the MouseLeftButtonUp event of the TextBlock uset as expansion trigger.
		/// </summary>
		/// <param name="sender">The source of the event.</param>
		/// <param name="e">The <see cref="System.Windows.Input.MouseButtonEventArgs"/> instance containing the event data.</param>
		private void ExpansionTrigger_MouseLeftButtonUp(object sender, MouseButtonEventArgs e)
		{
			if (!this.ExpandOnClickOnRow)
			{
				this.ToggleExpansionOfSelected();
			}
		}

		#endregion event handlers
	}
}