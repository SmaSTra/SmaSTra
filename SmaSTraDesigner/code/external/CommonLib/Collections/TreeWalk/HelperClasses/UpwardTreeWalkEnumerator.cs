namespace Common.Collections.TreeWalk.HelperClasses
{
	using System;

	/// <summary>
	/// Enumerator for the UpwardTreeWalk enumerable.
	/// </summary>
	/// <typeparam name="T">The type of the nodes in the tree.</typeparam>
	public class UpwardTreeWalkEnumerator<T> : TreeWalkEnumeratorBase<T, T>
		where T : class
	{
		#region constructors

		/// <summary>
		/// Initializes a new instance of the <see cref="UpwardTreeWalkEnumerator&lt;T&gt;"/> class.
		/// </summary>
		/// <param name="initialNode">The initial tree node to start from.</param>
		/// <param name="selectorFunction">The selector function that is supposed to return a parent for each node.</param>
		public UpwardTreeWalkEnumerator(T initialNode, Func<T, T> selectorFunction)
			: base(initialNode, selectorFunction)
		{
		}

		#endregion constructors

		#region overrideable methods

		/// <summary>
		/// Gets the next node in the tree.
		/// </summary>
		/// <param name="current">The current node.</param>
		/// <returns>See summary.</returns>
		protected override T GetNext(T current)
		{
			return this.SelectorFunction(current);
		}

		#endregion overrideable methods
	}
}