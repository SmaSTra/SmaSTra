namespace Common.ExtensionMethods
{
	using System;
	using System.Windows.Data;

	// TODO: (PS) Comment this.
	public static class BindingBaseExtensions
	{
		#region extension methods

		public static T Clone<T>(this T subject)
			where T : BindingBase
		{
			if (subject == null)
			{
				throw new ArgumentNullException("subject");
			}

			T result = null;
			Binding subjectAsBinding;
			MultiBinding subjectAsMultiBinding;
			ICloneable subjectAsICloneable;
			if ((subjectAsBinding = subject as Binding) != null)
			{
				result = BindingExtensions.Clone(subjectAsBinding, null, null, null) as T;
			}
			else if ((subjectAsMultiBinding = subject as MultiBinding) != null)
			{
				result = MultiBindingExtensions.Clone(subjectAsMultiBinding) as T;
			}
			else if ((subjectAsICloneable = subject as ICloneable) != null)
			{
				result = subjectAsICloneable.Clone() as T;
			}

			if (result != null)
			{
				return result;
			}

			throw new NotSupportedException(String.Format("The binding type {0} is not supported for cloning.", subject.GetType()));
		}

		// TODO: (PS) Comment this.
		public static object GetValueFromSource(this BindingBase subject)
		{
			if (subject == null)
			{
				throw new ArgumentNullException("subject");
			}

			DependencyPropertyProvider propertyProvider = new DependencyPropertyProvider();
			BindingBase clone = subject.Clone();
			SetMode(clone, BindingMode.OneWay);
			BindingOperations.SetBinding(propertyProvider, DependencyPropertyProvider.PropertyValueProperty, clone);
			object value = propertyProvider.PropertyValue;
			BindingOperations.ClearBinding(propertyProvider, DependencyPropertyProvider.PropertyValueProperty);

			return value;
		}

		public static void SetMode(this BindingBase subject, BindingMode mode)
		{
			if (subject == null)
			{
				throw new ArgumentNullException("subject");
			}

			Binding subjectAsBinding;
			MultiBinding subjectAsMultiBinding;
			if ((subjectAsBinding = subject as Binding) != null)
			{
				subjectAsBinding.Mode = mode;
			}
			else if ((subjectAsMultiBinding = subject as MultiBinding) != null)
			{
				foreach (var binding in subjectAsMultiBinding.Bindings)
				{
					SetMode(binding, mode);
				}
			}
			else
			{
				throw new NotSupportedException(String.Format("The binding type {0} is not supported for cloning.", subject.GetType()));
			}
		}

		// TODO: (PS) Comment this.
		public static void SetValueForSource(this BindingBase subject, object value)
		{
			if (subject == null)
			{
				throw new ArgumentNullException("subject");
			}

			DependencyPropertyProvider propertyProvider = new DependencyPropertyProvider();
			BindingBase clone = subject.Clone();
			SetMode(clone, BindingMode.OneWayToSource);
			BindingOperations.SetBinding(propertyProvider, DependencyPropertyProvider.PropertyValueProperty, clone);
			propertyProvider.PropertyValue = value;
			BindingOperations.ClearBinding(propertyProvider, DependencyPropertyProvider.PropertyValueProperty);
		}

		#endregion extension methods
	}
}