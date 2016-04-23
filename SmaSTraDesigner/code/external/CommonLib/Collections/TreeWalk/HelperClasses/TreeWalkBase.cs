namespace Common.Collections.TreeWalk.HelperClasses
{
	using System;
	using System.Collections;
	using System.Collections.Generic;

	/// <summary>
	/// Base class for all tree walk enumerable types.
	/// Puts all items in a tree-like structure in order
	/// of an upward or downward tree-walk.
	/// </summary>
	/// <typeparam name="TItems">The type of the nodes in the tree.</typeparam>
	/// <typeparam name="TSelectorReturn">The type of the selector function's return argument.</typeparam>
	public abstract class TreeWalkBase<TItems, TSelectorReturn> : IEnumerable<TItems>
		where TItems : class
	{
		#region constructors

		/// <summary>
		/// Initializes a new instance of the <see cref="TreeWalkBase&lt;TItems, TSelectorReturn&gt;"/> class.
		/// </summary>
		/// <param name="initialNode">The initial tree node to start from.</param>
		/// <param name="selectorFunction">The selector function that returns the next node(s) to select for each current node.</param>
		protected TreeWalkBase(TItems initialNode, Func<TItems, TSelectorReturn> selectorFunction)
		{
			if (initialNode == null)
			{
				throw new ArgumentNullException("initialNode");
			}
			if (selectorFunction == null)
			{
				throw new ArgumentNullException("selectorFunction");
			}

			this.InitialNode = initialNode;
			this.SelectorFunction = selectorFunction;
		}

		#endregion constructors

		#region properties

		/// <summary>
		/// Gets the initial node to start from.
		/// </summary>
		public TItems InitialNode
		{
			get;
			private set;
		}

		/// <summary>
		/// Gets the selector function that returns the next node(s) to select for each current node.
		/// </summary>
		public Func<TItems, TSelectorReturn> SelectorFunction
		{
			get;
			private set;
		}

		#endregion properties

		#region overrideable methods

		/// <summary>
		/// Is supposed to return the enumerator when overriden in derived class.
		/// </summary>
		/// <returns>See summary.</returns>
		protected abstract IEnumerator<TItems> GetEnumeratorOverride();

		#endregion overrideable methods

		#region methods

		/// <summary>
		/// Gets the enumerator.
		/// </summary>
		/// <returns>See summary.</returns>
		public IEnumerator<TItems> GetEnumerator()
		{
			if (this.InitialNode == null)
			{
				throw new InvalidOperationException("This instance is allready disposed.");
			}

			return this.GetEnumeratorOverride();
		}

		/// <summary>
		/// Returns an enumerator that iterates through a collection.
		/// </summary>
		/// <returns>
		/// An <see cref="T:System.Collections.IEnumerator"/> object that can be used to iterate through the collection.
		/// </returns>
		IEnumerator IEnumerable.GetEnumerator()
		{
			return this.GetEnumerator();
		}

		#endregion methods
	}
}