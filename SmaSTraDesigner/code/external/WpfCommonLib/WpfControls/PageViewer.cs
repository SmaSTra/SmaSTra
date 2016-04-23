namespace Common.WpfControls
{
	using System;
	using System.Collections.Generic;
	using System.Collections.ObjectModel;
	using System.Collections.Specialized;
	using System.Linq;
	using System.Windows;
	using System.Windows.Controls;
	using System.Windows.Controls.Primitives;
	using System.Windows.Data;
	using System.Windows.Input;

	using Common.ExtensionMethods;

	// TODO: (PS) Comment this.
	[TemplatePart(Type = typeof(Selector), Name = PageViewer.PartNames.ItemsHost)]
	[TemplatePart(Type = typeof(ContentControl), Name = PageViewer.PartNames.ContentHost)]
	public class PageViewer : Control
	{
		#region static constructor

		/// <summary>
		/// Initializes the <see cref="PageViewer"/> class.
		/// </summary>
		static PageViewer()
		{
			DefaultStyleKeyProperty.OverrideMetadata(typeof(PageViewer), new FrameworkPropertyMetadata(typeof(PageViewer)));
			IsSettingSelectionProperty = IsSettingSelectionPropertyKey.DependencyProperty;
		}

		#endregion static constructor

		#region dependency properties

		/// <summary>
		/// Registration of Content Dependency Property.
		/// </summary>
		private static readonly DependencyProperty ContentProperty = 
			DependencyProperty.Register("Content", typeof(object), typeof(PageViewer));

		/// <summary>
		/// Registration of IsSettingSelection Dependency Property.
		/// </summary>
		public static readonly DependencyProperty IsSettingSelectionProperty;

		/// <summary>
		/// Registration of IsSettingSelection Dependency Property Key.
		/// </summary>
		private static readonly DependencyPropertyKey IsSettingSelectionPropertyKey = 
			DependencyProperty.RegisterReadOnly("IsSettingSelection", typeof(bool), typeof(PageViewer), new FrameworkPropertyMetadata(false));

		/// <summary>
		/// Registration of Items Dependency Property.
		/// </summary>
		public static readonly DependencyProperty ItemsProperty = 
			DependencyProperty.Register(
				"Items", typeof(ObservableCollection<PageViewerItem>), typeof(PageViewer),
				new FrameworkPropertyMetadata(
					null,
					OnItemsChanged,
					CoerceItems));

		/// <summary>
		/// Registration of SelectedIndex Dependency Property.
		/// </summary>
		public static readonly DependencyProperty SelectedIndexProperty = 
			DependencyProperty.Register(
				"SelectedIndex", typeof(int), typeof(PageViewer),
				new FrameworkPropertyMetadata(
					-1,
					null,
					CoerceSelectedIndex));

		/// <summary>
		/// Registration of SelectedItem Dependency Property.
		/// </summary>
		public static readonly DependencyProperty SelectedItemProperty = 
			DependencyProperty.Register(
				"SelectedItem", typeof(PageViewerItem), typeof(PageViewer),
				new FrameworkPropertyMetadata(
					null,
					OnSelectedItemChanged));

		#endregion dependency properties

		#region dependency property callbacks

		/// <summary>
		/// Coerce Callback method of the Items Dependency Property.
		/// </summary>
		/// <param name="sender">The instance of the class of which the Items property is about to change.</param>
		/// <param name="value">The proposed new value for the Items property.</param>
		/// <returns>The actual new value for the Items property.</returns>
		private static object CoerceItems(DependencyObject sender, object value)
		{
			if (value as ObservableCollection<PageViewerItem> == null)
			{
				return new ObservableCollection<PageViewerItem>();
			}

			return value;
		}

		/// <summary>
		/// Coerce Callback method of the SelectedIndex Dependency Property.
		/// </summary>
		/// <param name="sender">The instance of the class of which the SelectedIndex property is about to change.</param>
		/// <param name="value">The proposed new value for the SelectedIndex property.</param>
		/// <returns>The actual new value for the SelectedIndex property.</returns>
		private static object CoerceSelectedIndex(DependencyObject sender, object value)
		{
			PageViewer subject = (PageViewer)sender;
			int newValue = (int)value;
			if (newValue < -1 || newValue > subject.Items.Count - 1)
			{
				return -1;
			}

			return value;
		}

		/// <summary>
		/// Property Changed Callback method of the Items Dependency Property.
		/// </summary>
		/// <param name="sender">The instance of the class that had the Items property changed.</param>
		/// <param name="e">The <see cref="System.Windows.DependencyPropertyChangedEventArgs"/> instance containing the event data.</param>
		private static void OnItemsChanged(DependencyObject sender, DependencyPropertyChangedEventArgs e)
		{
			PageViewer subject = (PageViewer)sender;
			ObservableCollection<PageViewerItem> newValue = (ObservableCollection<PageViewerItem>)e.NewValue;
			ObservableCollection<PageViewerItem> oldValue = (ObservableCollection<PageViewerItem>)e.OldValue;

			if (oldValue != null)
			{
				oldValue.CollectionChanged -= subject.Items_CollectionChanged;
				subject.SetOldItemsOwner(oldValue);
			}

			if (newValue != null)
			{
				newValue.CollectionChanged += subject.Items_CollectionChanged;
				subject.SetNewItemsOwner(newValue);
			}
		}

		/// <summary>
		/// Property Changed Callback method of the SelectedItem Dependency Property.
		/// </summary>
		/// <param name="sender">The instance of the class that had the SelectedItem property changed.</param>
		/// <param name="e">The <see cref="System.Windows.DependencyPropertyChangedEventArgs"/> instance containing the event data.</param>
		private static void OnSelectedItemChanged(DependencyObject sender, DependencyPropertyChangedEventArgs e)
		{
			PageViewer subject = (PageViewer)sender;
			PageViewerItem newValue = (PageViewerItem)e.NewValue;
			PageViewerItem oldValue = (PageViewerItem)e.OldValue;

			subject.IsSettingSelection = true;

			if (oldValue != null)
			{
				oldValue.FreeContent();
				oldValue.IsSelected = false;
			}

			if (newValue != null)
			{
				newValue.InitializeContent();
				newValue.IsSelected = true;
			}

			subject.IsSettingSelection = false;
		}

		#endregion dependency property callbacks

		#region fields

		private ContentControl contentHost = null;
		private Selector itemsHost = null;

		#endregion fields

		#region constructors

		/// <summary>
		/// Initializes a new instance of the <see cref="PageViewer"/> class.
		/// </summary>
		public PageViewer()
		{
			BindingOperations.SetBinding(this, ContentProperty, new Binding("SelectedItem.Content")
			{
				Source = this
			});

			this.Items = new ObservableCollection<PageViewerItem>();
			this.Unloaded += this.This_Unloaded;
			this.Dispatcher.ShutdownStarted += this.Dispatcher_ShutdownStarted;
		}

		#endregion constructors

		#region properties

		/// <summary>
		/// Gets or sets the value of the Content property.
		/// TODO: (PS) Comment this.
		/// This is a Dependency Property.
		/// </summary>
		public object Content
		{
			get { return (object)this.GetValue(ContentProperty); }
			private set { this.SetValue(ContentProperty, value); }
		}

		/// <summary>
		/// Gets the value of the IsSettingSelection property.
		/// This is a Dependency Property.
		/// </summary>
		public bool IsSettingSelection
		{
			get { return (bool)this.GetValue(IsSettingSelectionProperty); }
			private set { this.SetValue(IsSettingSelectionPropertyKey, value); }
		}

		/// <summary>
		/// Gets or sets the value of the Items property.
		/// TODO: (PS) Comment this.
		/// This is a Dependency Property.
		/// </summary>
		public ObservableCollection<PageViewerItem> Items
		{
			get { return (ObservableCollection<PageViewerItem>)this.GetValue(ItemsProperty); }
			set { this.SetValue(ItemsProperty, value); }
		}

		/// <summary>
		/// Gets or sets the value of the SelectedIndex property.
		/// TODO: (PS) Comment this.
		/// This is a Dependency Property.
		/// </summary>
		public int SelectedIndex
		{
			get { return (int)this.GetValue(SelectedIndexProperty); }
			set { this.SetValue(SelectedIndexProperty, value); }
		}

		/// <summary>
		/// Gets or sets the value of the SelectedItem property.
		/// TODO: (PS) Comment this.
		/// This is a Dependency Property.
		/// </summary>
		public PageViewerItem SelectedItem
		{
			get { return (PageViewerItem)this.GetValue(SelectedItemProperty); }
			set { this.SetValue(SelectedItemProperty, value); }
		}

		#endregion properties

		#region overrideable methods

		/// <summary>
		/// When overridden in a derived class, is invoked whenever application code or internal processes call <see cref="M:System.Windows.FrameworkElement.ApplyTemplate"/>.
		/// </summary>
		public override void OnApplyTemplate()
		{
			base.OnApplyTemplate();
			if (this.Template != null)
			{
				try
				{
					this.itemsHost = (Selector)this.Template.FindName(PartNames.ItemsHost, this);
					BindingOperations.SetBinding(this.itemsHost, Selector.ItemsSourceProperty, new Binding("Items")
					{
						Source = this,
						Mode = BindingMode.TwoWay
					});
					BindingOperations.SetBinding(this.itemsHost, Selector.SelectedItemProperty, new Binding("SelectedItem")
					{
						Source = this,
						Mode = BindingMode.TwoWay
					});
					BindingOperations.SetBinding(this.itemsHost, Selector.SelectedIndexProperty, new Binding("SelectedIndex")
					{
						Source = this,
						Mode = BindingMode.TwoWay
					});

					ICommand[] ignoeredCommands = new ICommand[]
						{
							ComponentCommands.MoveLeft,
							ComponentCommands.MoveRight,
							ComponentCommands.MoveUp,
							ComponentCommands.MoveDown,
							ComponentCommands.MoveToHome,
							ComponentCommands.MoveToEnd,
							ComponentCommands.MoveToPageUp,
							ComponentCommands.MoveToPageDown
						};
					this.itemsHost.CommandBindings.AddRange(ignoeredCommands.Select(command => new CommandBinding(command, null, (sender, e) => e.CanExecute = false)));

					this.contentHost = (ContentControl)this.Template.FindName(PartNames.ContentHost, this);
					BindingOperations.SetBinding(this.contentHost, ContentControl.ContentProperty, new Binding("Content")
					{
						Source = this,
						Mode = BindingMode.TwoWay
					});
				}
				catch (Exception ex)
				{
					throw new InvalidOperationException("Template could not be applied", ex);
				}
			}
		}

		/// <summary>
		/// Raises the <see cref="E:System.Windows.FrameworkElement.Initialized"/> event. This method is invoked whenever <see cref="P:System.Windows.FrameworkElement.IsInitialized"/> is set to true internally.
		/// </summary>
		/// <param name="e">The <see cref="T:System.Windows.RoutedEventArgs"/> that contains the event data.</param>
		protected override void OnInitialized(System.EventArgs e)
		{
			base.OnInitialized(e);

			if (!System.ComponentModel.DesignerProperties.GetIsInDesignMode(this))
			{
				this.CommandBindings.Add(new CommandBinding(Commands.MoveForward, this.OnMoveForwardExecuted, this.OnMoveCanExecute));
				this.CommandBindings.Add(new CommandBinding(Commands.MoveBack, this.OnMoveBackExecuted, this.OnMoveCanExecute));

				this.Loaded += this.This_Loaded;
			}
		}

		#endregion overrideable methods

		#region methods

		private void OnClosing()
		{
			this.Content = null;
		}

		private void OnMoveBackExecuted(object sender, ExecutedRoutedEventArgs e)
		{
			e.Handled = true;
			if (this.SelectedIndex <= 0)
			{
				this.SelectedIndex = this.Items.Count - 1;
			}
			else
			{
				this.SelectedIndex--;
			}
		}

		private void OnMoveCanExecute(object sender, CanExecuteRoutedEventArgs e)
		{
			e.CanExecute = this.Items.Count > 1;
		}

		private void OnMoveForwardExecuted(object sender, ExecutedRoutedEventArgs e)
		{
			e.Handled = true;
			this.SelectedIndex = (this.SelectedIndex + 1) % this.Items.Count;
		}

		private void SetNewItemsOwner(IEnumerable<PageViewerItem> items)
		{
			foreach (PageViewerItem item in items)
			{
				item.Owner = this;
			}
		}

		private void SetOldItemsOwner(IEnumerable<PageViewerItem> items)
		{
			foreach (PageViewerItem item in items)
			{
				if (item.Owner == this)
				{
					item.Owner = null;
				}
			}
		}

		#endregion methods

		#region event handlers

		private void Dispatcher_ShutdownStarted(object sender, EventArgs e)
		{
			this.OnClosing();
		}

		/// <summary>
		/// Handles the CollectionChanged event of the Items collection property.
		/// </summary>
		/// <param name="sender">The source of the event.</param>
		/// <param name="e">The <see cref="System.Collections.Specialized.NotifyCollectionChangedEventArgs"/> instance containing the event data.</param>
		private void Items_CollectionChanged(object sender, NotifyCollectionChangedEventArgs e)
		{
			if (e.OldItems != null)
			{
				this.SetOldItemsOwner(e.OldItems.Cast<PageViewerItem>());
			}

			if (e.NewItems != null)
			{
				this.SetNewItemsOwner(e.NewItems.Cast<PageViewerItem>());
			}
		}

		/// <summary>
		/// Handles the Loaded event of this control.
		/// </summary>
		/// <param name="sender">The source of the event.</param>
		/// <param name="e">The <see cref="System.Windows.RoutedEventArgs"/> instance containing the event data.</param>
		private void This_Loaded(object sender, RoutedEventArgs e)
		{
			if (e.OriginalSource == sender)
			{
				if (this.SelectedIndex < 0)
				{
					this.SelectedIndex = 0;
				}

				this.Loaded -= this.This_Loaded;
			}
		}

		private void This_Unloaded(object sender, RoutedEventArgs e)
		{
			if (e.OriginalSource == sender)
			{
				this.OnClosing();
			}
		}

		#endregion event handlers

		#region nested types

		public static class Commands
		{
			#region constants

			public static readonly RoutedUICommand MoveBack = new RoutedUICommand("Move to the previous tab.", "MoveBack", typeof(Commands), new InputGestureCollection()
				{
					new KeyGesture(Key.Tab, ModifierKeys.Control | ModifierKeys.Shift)
				});
			public static readonly RoutedUICommand MoveForward = new RoutedUICommand("Move to the next tab.", "MoveForward", typeof(Commands), new InputGestureCollection()
				{
					new KeyGesture(Key.Tab, ModifierKeys.Control)
				});

			#endregion constants
		}

		public static class PartNames
		{
			#region constants

			public const string ContentHost = "PART_ContentHost";
			public const string ItemsHost = "PART_ItemsHost";

			#endregion constants
		}

		#endregion nested types
	}
}