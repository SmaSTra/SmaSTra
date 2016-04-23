namespace Common.Collections.TreeWalk.HelperClasses
{
	using System;
	using System.Collections;
	using System.Collections.Generic;

	/// <summary>
	/// Base class for enumerators used in a tree walk enumeration.
	/// </summary>
	/// <typeparam name="TItems">The type of the nodes in the tree.</typeparam>
	/// <typeparam name="TSelectorReturn">The type of the selector function's return argument.</typeparam>
	public abstract class TreeWalkEnumeratorBase<TItems, TSelectorReturn> : IEnumerator<TItems>
		where TItems : class
	{
		#region fields

		/// <summary>
		/// Specifies whether this instance has been initialized, meaning that
		/// the first item in the enumeration has been set as Current.
		/// </summary>
		protected bool initialized = false;

		#endregion fields

		#region constructors

		/// <summary>
		/// Initializes a new instance of the <see cref="TreeWalkEnumeratorBase&lt;TItems, TSelectorReturn&gt;"/> class.
		/// </summary>
		/// <param name="initialNode">The initial tree node to start from.</param>
		/// <param name="selectorFunction">The selector function that returns the next node(s) to select for each current node.</param>
		public TreeWalkEnumeratorBase(TItems initialNode, Func<TItems, TSelectorReturn> selectorFunction)
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

		/// <summary>
		/// Releases unmanaged resources and performs other cleanup operations before the
		/// <see cref="TreeWalkEnumeratorBase&lt;TItems, TSelectorReturn&gt;"/> is reclaimed by garbage collection.
		/// </summary>
		~TreeWalkEnumeratorBase()
		{
			this.Dispose();
		}

		#endregion constructors

		#region properties

		/// <summary>
		/// Gets the current node in the tree.
		/// </summary>
		public TItems Current
		{
			get;
			private set;
		}

		/// <summary>
		/// Gets the initial node to start from.
		/// </summary>
		public TItems InitialNode
		{
			get;
			protected set;
		}

		/// <summary>
		/// Gets the selector function that returns the next node(s) to select for each current node.
		/// </summary>
		public Func<TItems, TSelectorReturn> SelectorFunction
		{
			get;
			protected set;
		}

		/// <summary>
		/// Gets the current node in the tree.
		/// </summary>
		object IEnumerator.Current
		{
			get
			{
				return this.Current;
			}
		}

		#endregion properties

		#region overrideable methods

		/// <summary>
		/// When overridden in derived class, is supposed to return the next node in the tree.
		/// </summary>
		/// <param name="current">The current node.</param>
		/// <returns>See summary.</returns>
		protected abstract TItems GetNext(TItems current);

		/// <summary>
		/// Performs application-defined tasks associated with freeing, releasing, or resetting unmanaged resources.
		/// </summary>
		public virtual void Dispose()
		{
			this.InitialNode = null;
			this.SelectorFunction = null;
			this.Current = null;
		}

		#endregion overrideable methods

		#region methods

		/// <summary>
		/// Advances the enumerator to the next element of the collection.
		/// </summary>
		/// <returns>
		/// true if the enumerator was successfully advanced to the next element; false if the enumerator has passed the end of the collection.
		/// </returns>
		/// <exception cref="T:System.InvalidOperationException">
		/// The collection was modified after the enumerator was created.
		/// </exception>
		public bool MoveNext()
		{
			if (this.InitialNode == null)
			{
				throw new InvalidOperationException("The enumerator is allready disposed.");
			}

			if (this.initialized)
			{
				if (this.Current != null)
				{
					return (this.Current = this.GetNext(this.Current)) != null;
				}
				else
				{
					return false;
				}
			}
			else
			{
				this.Current = this.InitialNode;
				this.initialized = true;

				return true;
			}
		}

		/// <summary>
		/// Sets the enumerator to its initial position, which is before the first element in the collection.
		/// </summary>
		/// <exception cref="T:System.InvalidOperationException">
		/// The collection was modified after the enumerator was created.
		/// </exception>
		public void Reset()
		{
			if (this.InitialNode == null)
			{
				throw new InvalidOperationException("The enumerator is allready disposed.");
			}

			this.Current = this.InitialNode;
		}

		#endregion methods
	}
}