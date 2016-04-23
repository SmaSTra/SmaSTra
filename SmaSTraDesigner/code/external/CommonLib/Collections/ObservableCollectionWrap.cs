namespace Common.Collections
{
	using System.Collections.Generic;

	/// <summary>
	/// Wrapper class for classes that implement ICollection&lt;T&gt;.
	/// Provides CollectionChanged event for changes in the wrapped collection and
	/// PropertyChanged event for the Count property.
	/// </summary>
	/// <typeparam name="T">Type of items.</typeparam>
	public class ObservableCollectionWrap<T> : ObservableCollectionWrapBase<ICollection<T>, T>
	{
		#region constructors

		/// <summary>
		/// Initializes a new instance of the <see cref="ObservableCollectionWrap&lt;T&gt;"/> class.
		/// </summary>
		/// <param name="innerList">The list that is supposed to be wrapped.</param>
		/// <param name="canAccessInnerCollection">if set to <c>true</c> the programmer can access the inner collection using the InnerCollection property.</param>
		public ObservableCollectionWrap(ICollection<T> innerCollection, bool canAccessInnerCollection = true)
			: base(innerCollection, canAccessInnerCollection)
		{
		}

		#endregion constructors
	}
}