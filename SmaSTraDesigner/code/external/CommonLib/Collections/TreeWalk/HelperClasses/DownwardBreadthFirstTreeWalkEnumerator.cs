namespace Common.Collections.TreeWalk.HelperClasses
{
	using System;
	using System.Collections.Generic;

	/// <summary>
	/// Enumerator for a downward tree walk of type breadth-first.
	/// </summary>
	/// <typeparam name="T">The type of the nodes in the tree.</typeparam>
	public class DownwardBreadthFirstTreeWalkEnumerator<T> : TreeWalkEnumeratorBase<T, IEnumerable<T>>
		where T : class
	{
		#region fields

		/// <summary>
		/// The enumerator for the current tree level.
		/// </summary>
		private IEnumerator<T> currentEnumerator;

		/// <summary>
		/// Stack that holds the enumerators for the lists of children of all
		/// upward branches in the tree.
		/// </summary>
		private Stack<IEnumerator<T>> enumeratorStack = new Stack<IEnumerator<T>>();

		#endregion fields

		#region constructors

		/// <summary>
		/// Initializes a new instance of the <see cref="DownwardBreadthFirstTreeWalkEnumerator&lt;T&gt;"/> class.
		/// </summary>
		/// <param name="initialNode">The initial tree node to start from.</param>
		/// <param name="selectorFunction">The selector function that is supposed to return a list of children for each node.</param>
		public DownwardBreadthFirstTreeWalkEnumerator(T initialNode, Func<T, IEnumerable<T>> selectorFunction)
			: base(initialNode, selectorFunction)
		{
		}

		#endregion constructors

		#region overrideable methods

		/// <summary>
		/// Performs application-defined tasks associated with freeing, releasing, or resetting unmanaged resources.
		/// </summary>
		public override void Dispose()
		{
			base.Dispose();

			if (this.enumeratorStack != null)
			{
				foreach (var enumerator in this.enumeratorStack)
				{
					enumerator.Dispose();
				}

				this.enumeratorStack = null;
			}
			if (this.currentEnumerator != null)
			{
				this.currentEnumerator.Dispose();
				this.currentEnumerator = null;
			}
		}

		/// <summary>
		/// Gets the next node.
		/// </summary>
		/// <param name="current">The current node.</param>
		/// <returns>See summary.</returns>
		protected override T GetNext(T current)
		{
			if (this.currentEnumerator == null)
			{
				this.currentEnumerator = this.GetNextEnumerator(current);
				if (this.currentEnumerator != null)
				{
					this.enumeratorStack.Push(this.currentEnumerator);

					return this.currentEnumerator.Current;
				}
				else
				{
					return null;
				}
			}
			else if (this.currentEnumerator.MoveNext())
			{
				return this.currentEnumerator.Current;
			}
			else
			{
				this.currentEnumerator.Reset();
				this.currentEnumerator = null;

				return this.GetNext(null);
			}
		}

		#endregion overrideable methods

		#region methods

		/// <summary>
		/// Gets the next enumerator.
		/// </summary>
		/// <param name="node">The node.</param>
		/// <returns>See summary.</returns>
		private IEnumerator<T> GetNextEnumerator(T node)
		{
			IEnumerator<T> enumerator;
			if (node != null)
			{
				IEnumerable<T> children = this.SelectorFunction(node);
				if (children != null && (enumerator = children.GetEnumerator()).MoveNext())
				{
					return enumerator;
				}
			}

			while (this.enumeratorStack.Count != 0)
			{
				enumerator = this.enumeratorStack.Peek();

				if (enumerator.MoveNext())
				{
					return GetNextEnumerator(enumerator.Current);
				}
				else
				{
					this.enumeratorStack.Pop().Dispose();
				}
			}

			return null;
		}

		#endregion methods
	}
}