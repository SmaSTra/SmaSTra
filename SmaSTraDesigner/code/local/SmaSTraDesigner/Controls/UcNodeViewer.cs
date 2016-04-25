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

	public class UcNodeViewer : UserControl
	{
		#region dependency properties

		/// <summary>
		/// Registration of IsMoving Dependency Property.
		/// </summary>
		public static readonly DependencyProperty IsMovingProperty = 
			DependencyProperty.Register("IsMoving", typeof(bool), typeof(UcNodeViewer));

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

		private bool enteredWithButtonPressed = false;

		#endregion fields

		#region constructors

		public UcNodeViewer()
		{
			this.MouseMove += this.This_MouseMove;
			this.MouseEnter += UcNodeViewer_MouseEnter;
			this.PreviewMouseLeftButtonUp += UcNodeViewer_PreviewMouseLeftButtonUp;
			this.MouseLeave += UcNodeViewer_MouseLeave;
			this.MouseLeftButtonDown += UcNodeViewer_MouseLeftButtonDown;
			this.MouseRightButtonDown += UcNodeViewer_MouseRightButtonDown;
		}

		#endregion constructors

		#region properties

		/// <summary>
		/// Gets or sets the value of the IsMoving property.
		/// TODO: (PS) Comment this.
		/// This is a Dependency Property.
		/// </summary>
		public bool IsMoving
		{
			get { return (bool)this.GetValue(IsMovingProperty); }
			set { this.SetValue(IsMovingProperty, value); }
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

		#endregion properties

		#region event handlers

		private void This_MouseMove(object sender, MouseEventArgs e)
		{
			if (e.LeftButton == MouseButtonState.Pressed && !this.enteredWithButtonPressed)
			{
				if (this.IsPreview)
				{
					if (!(this is UcOutputViewer))
					{
						DragDrop.DoDragDrop(this, new Tuple<Node>((Node)this.DataContext), DragDropEffects.All);
					}
				}
				else
				{
					this.IsMoving = true;
				}
			}
		}

		private void UcNodeViewer_MouseEnter(object sender, MouseEventArgs e)
		{
			if (e.LeftButton == MouseButtonState.Pressed)
			{
				this.enteredWithButtonPressed = true;
			}
		}

		private void UcNodeViewer_MouseLeave(object sender, MouseEventArgs e)
		{
			this.enteredWithButtonPressed = false;
		}

		private void UcNodeViewer_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
		{
			this.IsSelected = true;
			e.Handled = true;
		}

		private void UcNodeViewer_MouseRightButtonDown(object sender, MouseButtonEventArgs e)
		{
			this.IsSelected = true;
			e.Handled = true;
		}

		private void UcNodeViewer_PreviewMouseLeftButtonUp(object sender, MouseButtonEventArgs e)
		{
			this.enteredWithButtonPressed = false;
		}

		#endregion event handlers
	}
}