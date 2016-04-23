namespace Common.ExtensionMethods
{
	using System;
	using System.Linq;
	using System.Windows;
	using System.Windows.Data;

	using Common.Collections.TreeWalk;

	// TODO: (PS) Comment this.
	public static class RelativeSourceExtensions
	{
		#region extension methods

		public static DependencyObject Resolve(this RelativeSource subject, DependencyObject source)
		{
			if (subject == null)
			{
				throw new ArgumentNullException("subject");
			}
			if (source == null)
			{
				throw new ArgumentNullException("source");
			}

			switch (subject.Mode)
			{
				case RelativeSourceMode.FindAncestor:
					return new UpwardTreeWalk<DependencyObject>(source, LogicalTreeHelper.GetParent).Skip(subject.AncestorLevel - 1).First();

				case RelativeSourceMode.PreviousData:
					throw new NotImplementedException("Mode = RelativeSourceMode.PreviousData not implemented.");

				default:
				case RelativeSourceMode.Self:
					return source;

				case RelativeSourceMode.TemplatedParent:
					throw new NotImplementedException("Mode = RelativeSourceMode.TemplatedParent not implemented.");
			}
		}

		#endregion extension methods
	}
}