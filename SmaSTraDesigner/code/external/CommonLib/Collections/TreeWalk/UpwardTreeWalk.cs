namespace Common.Collections.TreeWalk
{
	using System;
	using System.Collections.Generic;

	using Common.Collections.TreeWalk.HelperClasses;

	/// <summary>
	/// An enumerable that puts all items in a tree-like structure in order
	/// of an upward tree-walk.
	/// </summary>
	/// <typeparam name="T">Type of the nodes in the tree. Must be a class type.</typeparam>
	public class UpwardTreeWalk<T> : TreeWalkBase<T, T>
		where T : class
	{
		#region constructors

		/// <summary>
		/// Initializes a new instance of the <see cref="UpwardTreeWalk&lt;T&gt;"/> class.
		/// </summary>
		/// <param name="initialNode">The initial tree node to start from.</param>
		/// <param name="selectorFunction">The selector function that is supposed to return a parent for each node.</param>
		public UpwardTreeWalk(T initialNode, Func<T, T> selectorFunction)
			: base(initialNode, selectorFunction)
		{
		}

		#endregion constructors

		#region overrideable methods

		/// <summary>
		/// Gets the enumerator.
		/// </summary>
		/// <returns>See summary.</returns>
		protected override IEnumerator<T> GetEnumeratorOverride()
		{
			return new UpwardTreeWalkEnumerator<T>(this.InitialNode, this.SelectorFunction);
		}

		#endregion overrideable methods
	}
}