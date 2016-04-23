namespace Common.WpfControls
{
	using System;
	using System.Windows;
	using System.Windows.Controls;
	using System.Windows.Data;

	using Common.Resources.Converters;

	// TODO: (PS) Comment this.
	[TemplatePart(Type = typeof(Button), Name = DialogWindow.PartNames.OkButton)]
	[TemplatePart(Type = typeof(Button), Name = DialogWindow.PartNames.CancelButton)]
	[TemplatePart(Type = typeof(Button), Name = DialogWindow.PartNames.ApplyButton)]
	public class DialogWindow : Window
	{
		#region static constructor

		static DialogWindow()
		{
			ContentProperty.OverrideMetadata(typeof(DialogWindow), new PropertyMetadata(OnContentChanged));
		}

		#endregion static constructor

		#region dependency properties

		/// <summary>
		/// Registration of CanApply Dependency Property.
		/// </summary>
		public static readonly DependencyProperty CanApplyProperty = 
			DependencyProperty.Register("CanApply", typeof(bool), typeof(DialogWindow));

		/// <summary>
		/// Registration of ShowApplyButton Dependency Property.
		/// </summary>
		public static readonly DependencyProperty ShowApplyButtonProperty = 
			DependencyProperty.Register("ShowApplyButton", typeof(bool), typeof(DialogWindow));

		#endregion dependency properties

		#region dependency property callbacks

		/// <summary>
		/// Property Changed Callback method of the Content Dependency Property.
		/// </summary>
		/// <param name="sender">The instance of the class that had the Content property changed.</param>
		/// <param name="e">The <see cref="System.Windows.DependencyPropertyChangedEventArgs"/> instance containing the event data.</param>
		private static void OnContentChanged(DependencyObject sender, DependencyPropertyChangedEventArgs e)
		{
			DialogWindow subject = (DialogWindow)sender;

			DependencyObject oldValue = e.OldValue as DependencyObject;
			if (oldValue != null)
			{
				oldValue.SetDialogWindow(null);
			}
			DependencyObject newValue = e.NewValue as DependencyObject;
			if (newValue != null)
			{
				newValue.SetDialogWindow(subject);
			}
		}

		#endregion dependency property callbacks

		#region fields

		private Button btnApply = null;
		private Button btnCancel = null;
		private Button btnOk = null;

		#endregion fields

		#region events

		/// <summary>
		/// TODO: (PS) Comment this.
		/// </summary>
		public event EventHandler Applied;

		#endregion events

		#region properties

		/// <summary>
		/// Gets or sets the value of the CanApply property.
		/// TODO: (PS) Comment this.
		/// This is a Dependency Property.
		/// </summary>
		public bool CanApply
		{
			get { return (bool)this.GetValue(CanApplyProperty); }
			set { this.SetValue(CanApplyProperty, value); }
		}

		/// <summary>
		/// Gets or sets the value of the ShowApplyButton property.
		/// TODO: (PS) Comment this.
		/// This is a Dependency Property.
		/// </summary>
		public bool ShowApplyButton
		{
			get { return (bool)this.GetValue(ShowApplyButtonProperty); }
			set { this.SetValue(ShowApplyButtonProperty, value); }
		}

		#endregion properties

		#region overrideable methods

		public override void OnApplyTemplate()
		{
			base.OnApplyTemplate();
			if (!System.ComponentModel.DesignerProperties.GetIsInDesignMode(this) && this.Template != null)
			{
				try
				{
					this.btnOk = (Button)this.Template.FindName(PartNames.OkButton, this);
					this.btnOk.IsDefault = true;
					this.btnOk.Click += this.btnOk_Click;
					this.btnCancel = (Button)this.Template.FindName(PartNames.CancelButton, this);
					this.btnCancel.IsCancel = true;
					this.btnApply = (Button)this.Template.FindName(PartNames.ApplyButton, this);
					this.btnApply.Click += this.btnApply_Click;
					BindingOperations.SetBinding(this.btnApply, Button.VisibilityProperty, new Binding("ShowApplyButton")
					{
						Source = this,
						Converter = Singleton<VisibilityAsBooleanConverter>.Instance
					});
					BindingOperations.SetBinding(this.btnApply, Button.IsEnabledProperty, new Binding("CanApply")
					{
						Source = this
					});
				}
				catch (Exception ex)
				{
					throw new InvalidOperationException("Template could not be applied.", ex);
				}
			}
		}

		#endregion overrideable methods

		#region methods

		/// <summary>
		/// Raises the <see cref="E:Applied"/> event.
		/// </summary>
		private void OnApplied()
		{
			if (this.Applied != null)
			{
				this.Applied(this, null);
			}
		}

		#endregion methods

		#region event handlers

		private void btnApply_Click(object sender, RoutedEventArgs e)
		{
			this.OnApplied();
		}

		private void btnOk_Click(object sender, RoutedEventArgs e)
		{
			this.DialogResult = true;
		}

		#endregion event handlers

		#region nested types

		public static class PartNames
		{
			#region constants

			public const string ApplyButton = "PART_ApplyButton";
			public const string CancelButton = "PART_CancelButton";
			public const string OkButton = "PART_OkButton";

			#endregion constants
		}

		#endregion nested types
	}

	public static class DialogWindowHelper
	{
		#region extension methods

		/// <summary>
		/// Property getter method of DialogWindow Dependency Property.
		/// TODO: (PS) Comment this.
		/// </summary>
		/// <param name="subject">The subject.</param>
		/// <returns>The current value of the DialogWindow property.</returns>
		public static DialogWindow GetDialogWindow(this DependencyObject subject)
		{
			return (DialogWindow)subject.GetValue(DialogWindowProperty);
		}

		/// <summary>
		/// Property setter method of DialogWindow Dependency Property.
		/// TODO: (PS) Comment this.
		/// </summary>
		/// <param name="subject">The subject.</param>
		/// <param name="value">The supposed value of the DialogWindow property.</param>
		public static void SetDialogWindow(this DependencyObject subject, DialogWindow value)
		{
			subject.SetValue(DialogWindowProperty, value);
		}

		#endregion extension methods

		#region dependency properties

		/// <summary>
		/// Registration of DialogWindow Dependency Property.
		/// </summary>
		public static readonly DependencyProperty DialogWindowProperty = 
			DependencyProperty.RegisterAttached("DialogWindow", typeof(DialogWindow), typeof(DialogWindowHelper), new FrameworkPropertyMetadata(null));

		#endregion dependency properties
	}
}