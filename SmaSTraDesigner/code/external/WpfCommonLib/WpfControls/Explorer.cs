using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;

namespace Common.WpfControls
{
	// TODO: (PS) Comment this.
	[TemplatePart(Type = typeof(TreeView), Name = Explorer.PartNames.DirectoryTree)]
	public class Explorer : Control
	{
		private TreeView directoryTree = null;
		public override void OnApplyTemplate()
		{
			base.OnApplyTemplate();
			if (this.Template != null)
			{
				try
				{
					this.directoryTree = (TreeView)this.Template.FindName(PartNames.DirectoryTree, this);
				}
				catch (Exception ex)
				{
					throw new InvalidOperationException("Template could not be applied", ex);
				}
			}
		}

		public static class PartNames
		{
			#region constants

			public const string DirectoryTree = "PART_DirectoryTree";

			#endregion constants
		}
	}
}
