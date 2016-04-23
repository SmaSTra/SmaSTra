namespace Common.Collections.TreeWalk
{
	using System;
	using System.Collections.Generic;

	using Common.Collections.TreeWalk.HelperClasses;

	#region Enumerations

	/// <summary>
	/// Type of the downward treewalk enumeration.
	/// </summary>
	public enum DownwardTreeWalkType
	{
		/// <summary>
		/// Specifies a Depth-first algorithm.
		/// See http://en.wikipedia.org/wiki/Depth-first_search for more information.
		/// </summary>
		DepthFirst = 0,
		/// <summary>
		/// Specifies a Breadth-first algorithm.
		/// See http://en.wikipedia.org/wiki/Breadth-first_search for more information.
		/// </summary>
		BreadthFirst
	}

	#endregion Enumerations

	/// <summary>
	/// An enumerable that puts all items in a tree-like structure in order
	/// of a downward tree-walk.
	/// </summary>
	/// <typeparam name="T">Type of the nodes in the tree. Must be a class type.</typeparam>
	public class DownwardTreeWalk<T> : TreeWalkBase<T, IEnumerable<T>>
		where T : class
	{
		#region constructors

		/// <summary>
		/// Initializes a new instance of the <see cref="DownwardTreeWalk&lt;T&gt;"/> class.
		/// </summary>
		/// <param name="initialNode">The initial tree node to start from.</param>
		/// <param name="selectorFunction">The selector function that is supposed to return a list of children for each node.</param>
		/// <param name="downwardTreeWalkType">Type of the downward tree walk.</param>
		public DownwardTreeWalk(T initialNode, Func<T, IEnumerable<T>> selectorFunction, DownwardTreeWalkType downwardTreeWalkType = DownwardTreeWalkType.BreadthFirst)
			: base(initialNode, selectorFunction)
		{
			this.DownwardTreeWalkType = downwardTreeWalkType;
		}

		#endregion constructors

		#region properties

		/// <summary>
		/// Gets or sets the type of the downward tree walk.
		/// </summary>
		public DownwardTreeWalkType DownwardTreeWalkType
		{
			get;
			private set;
		}

		#endregion properties

		#region overrideable methods

		/// <summary>
		/// Gets the enumerator based on the DownwardTreeWalkType.
		/// </summary>
		/// <returns>The enumerator.</returns>
		protected override IEnumerator<T> GetEnumeratorOverride()
		{
			switch (this.DownwardTreeWalkType)
			{
				default:
				case DownwardTreeWalkType.DepthFirst:
					return new DownwardDepthFirstTreeWalkEnumerator<T>(this.InitialNode, this.SelectorFunction);

				case DownwardTreeWalkType.BreadthFirst:
					return new DownwardBreadthFirstTreeWalkEnumerator<T>(this.InitialNode, this.SelectorFunction);
			}
		}

		#endregion overrideable methods
	}
}