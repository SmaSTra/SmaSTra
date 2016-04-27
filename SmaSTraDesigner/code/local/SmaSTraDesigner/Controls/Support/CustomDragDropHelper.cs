namespace SmaSTraDesigner.Controls.Support
{
	using System;
	using System.Windows.Controls;
	using System.Windows.Input;

	/// <summary>
	/// Helper class for the custom drag & drop function in the tree designer (not the standard proviced by WPF).
	/// </summary>
	public class CustomDragDropHelper
	{
		#region fields

		/// <summary>
		/// Callback for when something is being dragged.
		/// </summary>
		private Action<MouseEventArgs> dragCallback;

		/// <summary>
		/// Keeps track of whether the owner was entered in with the left mouse button already pressed.
		/// </summary>
		private bool enteredWithButtonPressed = false;

		/// <summary>
		/// The owning control that is supposed to provide the custom drag & drop functionality.
		/// </summary>
		private Control owner;

		#endregion fields

		#region constructors

		/// <summary>
		/// Initializes a new instance of the <see cref="CustomDragDropHelper"/> class.
		/// </summary>
		/// <param name="owner">The owning control that is supposed to provide the custom drag & drop functionality.</param>
		/// <param name="dragCallback">Callback for when something is being dragged.</param>
		/// <exception cref="System.ArgumentNullException">
		/// owner
		/// or
		/// dragCallback
		/// </exception>
		public CustomDragDropHelper(Control owner, Action<MouseEventArgs> dragCallback)
		{
			if (owner == null)
			{
				throw new ArgumentNullException("owner");
			}
			if (dragCallback == null)
			{
				throw new ArgumentNullException("dragCallback");
			}

			this.owner = owner;
			this.dragCallback = dragCallback;

			owner.MouseEnter += this.Parent_MouseEnter;
			owner.PreviewMouseLeftButtonUp += this.Parent_PreviewMouseLeftButtonUp;
			owner.MouseLeave += this.Parent_MouseLeave;
			owner.MouseMove += this.Parent_MouseMove;
		}

		#endregion constructors

		#region event handlers

		/// <summary>
		/// Handles the MouseEnter event of the Parent control.
		/// </summary>
		/// <param name="sender">The source of the event.</param>
		/// <param name="e">The <see cref="MouseEventArgs"/> instance containing the event data.</param>
		private void Parent_MouseEnter(object sender, MouseEventArgs e)
		{
			if (e.LeftButton == MouseButtonState.Pressed)
			{
				this.enteredWithButtonPressed = true;
			}
		}

		/// <summary>
		/// Handles the MouseLeave event of the Parent control.
		/// </summary>
		/// <param name="sender">The source of the event.</param>
		/// <param name="e">The <see cref="MouseEventArgs"/> instance containing the event data.</param>
		private void Parent_MouseLeave(object sender, MouseEventArgs e)
		{
			this.enteredWithButtonPressed = false;
		}

		/// <summary>
		/// Handles the MouseMove event of the Parent control.
		/// </summary>
		/// <param name="sender">The source of the event.</param>
		/// <param name="e">The <see cref="MouseEventArgs"/> instance containing the event data.</param>
		private void Parent_MouseMove(object sender, MouseEventArgs e)
		{
			if (e.LeftButton == MouseButtonState.Pressed && !this.enteredWithButtonPressed)
			{
				this.dragCallback(e);
			}
		}

		/// <summary>
		/// Handles the PreviewMouseLeftButtonUp event of the Parent control.
		/// </summary>
		/// <param name="sender">The source of the event.</param>
		/// <param name="e">The <see cref="MouseButtonEventArgs"/> instance containing the event data.</param>
		private void Parent_PreviewMouseLeftButtonUp(object sender, MouseButtonEventArgs e)
		{
			this.enteredWithButtonPressed = false;
		}

		#endregion event handlers
	}
}