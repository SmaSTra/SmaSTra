namespace SmaSTraDesigner.Controls.Support
{
	using System;
	using System.Collections.Generic;
	using System.Linq;
	using System.Text;
	using System.Threading.Tasks;
	using System.Windows;
	using System.Windows.Controls;
	using System.Windows.Input;

	// TODO: (PS) Comment this.
	public class CustomDragDropHelper
	{
		#region fields

		private Action<MouseEventArgs> callback;
		private bool enteredWithButtonPressed = false;
		private Control parent;

		#endregion fields

		#region constructors

		public CustomDragDropHelper(Control parent, Action<MouseEventArgs> callback)
		{
			if (parent == null)
			{
				throw new ArgumentNullException("parent");
			}
			if (callback == null)
			{
				throw new ArgumentNullException("callback");
			}

			this.parent = parent;
			this.callback = callback;

			parent.MouseEnter += this.Parent_MouseEnter;
			parent.PreviewMouseLeftButtonUp += this.Parent_PreviewMouseLeftButtonUp;
			parent.MouseLeave += this.Parent_MouseLeave;
			parent.MouseMove += this.Parent_MouseMove;
		}

		#endregion constructors

		#region event handlers

		private void Parent_MouseEnter(object sender, MouseEventArgs e)
		{
			if (e.LeftButton == MouseButtonState.Pressed)
			{
				this.enteredWithButtonPressed = true;
			}
		}

		private void Parent_MouseLeave(object sender, MouseEventArgs e)
		{
			this.enteredWithButtonPressed = false;
		}

		private void Parent_MouseMove(object sender, MouseEventArgs e)
		{
			if (e.LeftButton == MouseButtonState.Pressed && !this.enteredWithButtonPressed)
			{
				this.callback(e);
			}
		}

		private void Parent_PreviewMouseLeftButtonUp(object sender, MouseButtonEventArgs e)
		{
			this.enteredWithButtonPressed = false;
		}

		#endregion event handlers
	}
}