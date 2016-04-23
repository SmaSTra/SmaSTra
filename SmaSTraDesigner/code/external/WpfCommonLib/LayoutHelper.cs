namespace Common
{
	using System;
	using System.Collections.Generic;
	using System.Linq;
	using System.Windows;
	using System.Windows.Controls;
	using System.Windows.Media;
	using System.Windows.Media.Media3D;

	/// <summary>
	/// Collection of static methods used to navigate through the logical and visual trees,
	/// and find specific elements there.
	/// </summary>
	public static class LayoutHelper
	{
		#region static methods

		/// <summary>
		/// Finds all logical children of the given parent DependencyObject with the given
		/// type T using the LogicalTreeHelper class's static methods.
		/// </summary>
		/// <typeparam name="T">Type to look for.</typeparam>
		/// <param name="parent">The parent.</param>
		/// <param name="includeParent">if set to <c>true</c> the given parent element is included in the search.</param>
		/// <returns>See summary.</returns>
		public static List<T> FindAllLogicalChildren<T>(DependencyObject parent, bool includeParent = false)
			where T : DependencyObject
		{
			List<T> result = new List<T>();
			if (parent != null)
			{
				if (includeParent)
				{
					T castParent = parent as T;
					if (castParent != null)
					{
						result.Add((T)parent);
					}
				}

				List<object> children;
				if (parent is Frame)
				{
					children = new List<object>();
					children.Add(((Frame)parent).Content);
				}
				else
				{
					children = LogicalTreeHelper.GetChildren(parent).Cast<object>().ToList();
				}

				if (children != null && children.Count() > 0)
				{
					foreach (object child in children)
					{
						if (child is T)
						{
							result.Add((T)child);
						}

						if (child is DependencyObject)
						{
							result.AddRange(FindAllLogicalChildren<T>((DependencyObject)child));
						}
					}
				}
			}

			return result;
		}

		/// <summary>
		/// Returns all visual children of the given parent DependencyObject that
		/// are of the given type.
		/// Returns an empty list if nothing was found.
		/// </summary>
		/// <typeparam name="T">Type to look for.</typeparam>
		/// <param name="parent">The parent from which to start.</param>
		/// <returns>See summary.</returns>
		public static List<T> FindAllVisualChildren<T>(DependencyObject parent, bool includeParent = false)
			where T : DependencyObject
		{
			List<T> result = new List<T>();
			if (parent != null)
			{
				if (includeParent)
				{
					T castParent = parent as T;
					if (castParent != null)
					{
						result.Add((T)parent);
					}
				}

				int count = VisualTreeHelper.GetChildrenCount(parent);

				for (int i = 0; i < count; i++)
				{
					DependencyObject child = VisualTreeHelper.GetChild(parent, i);
					T correctlyTyped = child as T;
					if (correctlyTyped != null)
					{
						result.Add(correctlyTyped);
					}

					result.AddRange(FindAllVisualChildren<T>(child));
				}
			}

			return result;
		}

		/// <summary>
		/// Finds the first child of type T of the given parent DependencyObject
		/// in the Logical Tree.
		/// </summary>
		/// <typeparam name="T">Wanted type.</typeparam>
		/// <param name="parent">Parent object</param>
		/// <returns>First occurrence of a object of type T</returns>
		public static T FindLogicalChild<T>(DependencyObject parent, bool includeParent = false)
			where T : DependencyObject
		{
			if (parent != null)
			{
				if (includeParent)
				{
					T castParent = parent as T;
					if (castParent != null)
					{
						return castParent;
					}
				}

				IEnumerable<object> children = LogicalTreeHelper.GetChildren(parent).Cast<object>().ToList();
				if (children != null && children.Count() > 0)
				{
					foreach (object child in children)
					{
						if (child is T)
						{
							return (T)child;
						}
					}

					foreach (object child in children)
					{
						if (child is DependencyObject)
						{
							T result = FindLogicalChild<T>((DependencyObject)child);
							if (result != null)
							{
								return result;
							}
						}
					}
				}
			}

			return null;
		}

		/// <summary>
		/// Finds the first parent of type T by walking upwards the logical tree.
		/// </summary>
		/// <typeparam name="T">Type of the object to look for. Must be a descendend of DependecyObject</typeparam>
		/// <param name="current">The DepencencyObject from which to start the search.</param>
		/// <param name="ignoreCurrent">Specifies if the given current element is ignored in type comparison.</param>
		/// <returns>First occurence of type T.</returns>
		public static T FindLogicalParent<T>(DependencyObject current, bool ignoreCurrent = false)
			where T : DependencyObject
		{
			return FindLogicalParentWhere<T>(current, null, ignoreCurrent);
		}

		// TODO: (PS) Comment this.
		public static T FindLogicalParentWhere<T>(DependencyObject current, Func<T, bool> conditionFunction, bool ignoreCurrent = false)
			where T : DependencyObject
		{
			if (current != null)
			{
				T correctlyTyped = current as T;
				if (!ignoreCurrent && correctlyTyped != null && (conditionFunction == null || conditionFunction(correctlyTyped)))
				{
					return correctlyTyped;
				}
				else
				{
					return FindLogicalParentWhere<T>(LogicalTreeHelper.GetParent(current), conditionFunction);
				}
			}
			else
			{
				return null;
			}
		}

		/// <summary>
		/// Finds the first child of type T of the given parent DependencyObject
		/// in the Visual Tree.
		/// </summary>
		/// <typeparam name="T">Wanted type.</typeparam>
		/// <param name="parent">Parent object</param>
		/// <returns>First occurrence of a object of type T</returns>
		public static T FindVisualChild<T>(DependencyObject parent, bool includeParent = false)
			where T : DependencyObject
		{
			return FindVisualChildWhere<T>(parent, null, includeParent);
		}

		/// <summary>
		/// Finds the first child of type T of the given parent DependencyObject
		/// in the Visual Tree where the given conditionFunction returns true.
		/// The parent object is not considered.
		/// </summary>
		/// <typeparam name="T">Wanted type.</typeparam>
		/// <param name="parent">Parent object</param>
		/// <param name="conditionFunction"></param>
		/// <returns>First occurrence of a object of type T</returns>
		public static T FindVisualChildWhere<T>(DependencyObject parent, Func<T, bool> conditionFunction, bool includeParent = false)
			where T : DependencyObject
		{
			if (parent != null)
			{
				if (includeParent)
				{
					T castParent = parent as T;
					if (castParent != null && (conditionFunction == null || conditionFunction(castParent)))
					{
						return castParent;
					}
				}

				int childrenCount = VisualTreeHelper.GetChildrenCount(parent);
				if (childrenCount > 0)
				{
					DependencyObject[] children = new DependencyObject[childrenCount];
					for (int i = 0; i < childrenCount; i++)
					{
						DependencyObject child = VisualTreeHelper.GetChild(parent, i);
						children[i] = child;

						if (child is T && (conditionFunction == null || conditionFunction((T)child)))
						{
							return (T)child;
						}
					}

					foreach (DependencyObject child in children)
					{
						T result = FindVisualChildWhere<T>(child, conditionFunction);
						if (result != null)
						{
							return result;
						}
					}
				}
			}

			return null;
		}

		/// <summary>
		/// Finds the DependencyObject parent of the given one.
		/// </summary>
		/// <typeparam name="T">Type of which the parent element is supposed to be.</typeparam>
		/// <param name="current">DependencyObject child</param>
		/// <param name="ignoreCurrent">Specifies if the given current element is ignored in type comparison.</param>
		/// <returns>DependencyObject parent of the given one</returns>
		public static T FindVisualParent<T>(DependencyObject current, bool ignoreCurrent = false)
			where T : DependencyObject
		{
			return FindVisualParentWhere<T>(current, null, ignoreCurrent);
		}

		/// <summary>
		/// Finds the visual parent of the given current DependencyObject of the given type,
		/// that fullfills the given condition ('conditionFunction').
		/// </summary>
		/// <typeparam name="T">The type to look for.</typeparam>
		/// <param name="current">The element from which to start looking.</param>
		/// <param name="conditionFunction">The function containing the condition that is supposed to be fullfilled for the element being looked for.
		/// If null, no condition is used.</param>
		/// <param name="ignoreCurrent">Specifies whether the current element should be ignored even if it fulfills the type and the given condition.</param>
		/// <returns>null if nothing was found. See summary for otherwise.</returns>
		public static T FindVisualParentWhere<T>(DependencyObject current, Func<T, bool> conditionFunction, bool ignoreCurrent = false)
			where T : DependencyObject
		{
			if (current != null)
			{
				T correctlyTyped = current as T;
				if (!ignoreCurrent && correctlyTyped != null && (conditionFunction == null || conditionFunction(correctlyTyped)))
				{
					return correctlyTyped;
				}
				else
				{
					return FindVisualParentWhere<T>(VisualTreeHelper.GetParent(current), conditionFunction);
				}
			}
			else
			{
				return null;
			}
		}

		// TODO: (PS) Comment this.
		public static List<DependencyObject> GetVisualChildren(DependencyObject parent)
		{
			List<DependencyObject> result = new List<DependencyObject>();
			if (parent != null && (parent is Visual || parent is Visual3D))
			{
				int childrenCount = VisualTreeHelper.GetChildrenCount(parent);
				for (int i = 0; i < childrenCount; i++)
				{
					result.Add(VisualTreeHelper.GetChild(parent, i));
				}
			}

			return result;
		}

		#endregion static methods
	}
}