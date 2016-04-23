namespace Common.ExtensionMethods
{
	using System;
	using System.Collections.Generic;
	using System.Linq;
	using System.Windows.Data;

	// TODO: (PS) Comment this.
	public static class MultiBindingExtensions
	{
		#region extension methods

		public static MultiBinding AddBindings(this MultiBinding subject, IEnumerable<BindingBase> bindings)
		{
			if (subject == null)
			{
				throw new ArgumentNullException("subject");
			}
			if (bindings == null)
			{
				throw new ArgumentNullException("bindings");
			}

			foreach (var binding in bindings)
			{
				subject.Bindings.Add(binding);
			}

			return subject;
		}

		public static MultiBinding AddBindings(this MultiBinding subject, params BindingBase[] bindings)
		{
			return AddBindings(subject, (IEnumerable<BindingBase>)bindings);
		}

		#endregion extension methods

		#region static methods

		internal static MultiBinding Clone(MultiBinding subject)
		{
			MultiBinding clone = new MultiBinding()
			{
				BindingGroupName = subject.BindingGroupName,
				Converter = subject.Converter,
				ConverterCulture = subject.ConverterCulture,
				ConverterParameter = subject.ConverterParameter,
				FallbackValue = subject.FallbackValue,
				Mode = subject.Mode,
				NotifyOnSourceUpdated = subject.NotifyOnSourceUpdated,
				NotifyOnTargetUpdated = subject.NotifyOnTargetUpdated,
				NotifyOnValidationError = subject.NotifyOnValidationError,
				StringFormat = subject.StringFormat,
				TargetNullValue = subject.TargetNullValue,
				UpdateSourceExceptionFilter = subject.UpdateSourceExceptionFilter,
				UpdateSourceTrigger = subject.UpdateSourceTrigger,
				ValidatesOnDataErrors = subject.ValidatesOnDataErrors,
				ValidatesOnExceptions = subject.ValidatesOnExceptions
			};
			clone.Bindings.AddRange(subject.Bindings.Select(binding => binding.Clone()));
			clone.ValidationRules.AddRange(subject.ValidationRules);

			return clone;
		}

		#endregion static methods
	}
}