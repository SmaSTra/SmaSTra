namespace Common.Collections.TreeWalk.HelperClasses
{
	using System;
	using System.Collections.Generic;

	/// <summary>
	/// Enumerator for a downward tree walk of type depth-first.
	/// </summary>
	/// <typeparam name="T">The type of the nodes in the tree.</typeparam>
	public class DownwardDepthFirstTreeWalkEnumerator<T> : TreeWalkEnumeratorBase<T, IEnumerable<T>>
		where T : class
	{
		#region fields

		/// <summary>
		/// Stack that holds the enumerators for the lists of children of all
		/// upward branches in the tree.
		/// </summary>
		private Stack<IEnumerator<T>> enumeratorStack = new Stack<IEnumerator<T>>();

		#endregion fields

		#region constructors

		/// <summary>
		/// Initializes a new instance of the <see cref="DownwardDepthFirstTreeWalkEnumerator&lt;T&gt;"/> class.
		/// </summary>
		/// <param name="initialNode">The initial tree node to start from.</param>
		/// <param name="selectorFunction">The selector function that is supposed to return a list of children for each node.</param>
		public DownwardDepthFirstTreeWalkEnumerator(T initialNode, Func<T, IEnumerable<T>> selectorFunction)
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
		}

		/// <summary>
		/// Gets the next node.
		/// </summary>
		/// <param name="current">The current node.</param>
		/// <returns>See summary.</returns>
		protected override T GetNext(T current)
		{
			IEnumerator<T> enumerator;
			if (this.enumeratorStack.Count == 0)
			{
				if ((enumerator = this.PushNextEnumerator(this.InitialNode)) == null)
				{
					return null;
				}
			}
			else
			{
				enumerator = this.enumeratorStack.Peek();
			}

			if (enumerator.MoveNext())
			{
				this.PushNextEnumerator(enumerator.Current);

				return enumerator.Current;
			}
			else
			{
				this.enumeratorStack.Pop().Dispose();

				if (this.enumeratorStack.Count != 0)
				{
					return this.GetNext(this.enumeratorStack.Peek().Current);
				}
				else
				{
					return null;
				}
			}
		}

		#endregion overrideable methods

		#region methods

		/// <summary>
		/// Pushes the next enumerator onto the stack and returns it.
		/// </summary>
		/// <param name="current">The current node.</param>
		/// <returns>See summary.</returns>
		private IEnumerator<T> PushNextEnumerator(T current)
		{
			IEnumerable<T> children = this.SelectorFunction(current);
			if (children != null)
			{
				IEnumerator<T> result = children.GetEnumerator();
				this.enumeratorStack.Push(result);

				return result;
			}

			return null;
		}

		#endregion methods
	}
}