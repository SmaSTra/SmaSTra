namespace Common.ExtensionMethods
{
	using System;
	using System.Collections.Generic;
	using System.Linq;
	using System.Windows;
	using System.Windows.Controls;
	using System.Windows.Data;

	/// <summary>
	/// Extension Methods for Binding class
	/// </summary>
	public static class BindingExtensions
	{
		#region extension methods

		// TODO: (PS) Comment this.
		public static Binding AddValidationRules(this Binding subject, IEnumerable<ValidationRule> validationRules)
		{
			if (subject == null)
			{
				throw new ArgumentNullException("subject");
			}

			subject.ValidationRules.AddRange(validationRules);

			return subject;
		}

		// TODO: (PS) Comment this.
		public static Binding AddValidationRules(this Binding subject, params ValidationRule[] validationRules)
		{
			if (subject == null)
			{
				throw new ArgumentNullException("subject");
			}

			subject.AddValidationRules(validationRules.AsEnumerable());

			return subject;
		}

		/// <summary>
		/// Returns a clone of this binding but with a different value for the 'Source' property.
		/// Using this method is required if you wish to set a Source and the original binding uses
		/// ElementName or RelativeSource.
		/// </summary>
		/// <param name="subject">The binding.</param>
		/// <param name="newSource">The new value for the Source property.</param>
		/// <returns>The clone.</returns>
		public static Binding Clone(this Binding subject, object newSource)
		{
			if (subject == null)
			{
				throw new ArgumentNullException("subject");
			}

			return BindingExtensions.Clone(subject, newSource, null, null);
		}

		/// <summary>
		/// Returns a clone of this binding but with a different value for the 'RelativeSource' property.
		/// Using this method is required if you wish to set a RelativeSource and the original binding uses
		/// ElementName or Source.
		/// </summary>
		/// <param name="subject">The binding.</param>
		/// <param name="newRelativeSource">The new value for RelativeSource.</param>
		/// <returns>The clone.</returns>
		public static Binding Clone(this Binding subject, RelativeSource newRelativeSource)
		{
			if (subject == null)
			{
				throw new ArgumentNullException("subject");
			}

			return BindingExtensions.Clone(subject, null, newRelativeSource, null);
		}

		/// <summary>
		/// Returns a clone of this binding but with a different value for the 'ElementName' property.
		/// Using this method is required if you wish to set a ElementName and the original binding uses
		/// RelativeSource or Source.
		/// </summary>
		/// <param name="subject">The binding.</param>
		/// <param name="newElementName">The new value for ElementName.</param>
		/// <returns>The clone.</returns>
		public static Binding Clone(this Binding subject, string newElementName)
		{
			if (subject == null)
			{
				throw new ArgumentNullException("subject");
			}

			return BindingExtensions.Clone(subject, null, null, newElementName);
		}

		#endregion extension methods

		#region static methods

		/// <summary>
		/// Returns a clone of this binding.
		/// </summary>
		/// <param name="subject">The binding.</param>
		/// <param name="newSource">The new value for the Source property.</param>
		/// <param name="newRelativeSource">The new value for RelativeSource.</param>
		/// <param name="newElementName">The new value for ElementName.</param>
		/// <returns>The clone.</returns>
		internal static Binding Clone(Binding subject, object newSource, RelativeSource newRelativeSource, string newElementName)
		{
			if (subject == null)
			{
				throw new ArgumentNullException("subject");
			}

			Binding result = new Binding();
			if (subject.Path != null)
			{
				result.Path = new PropertyPath(subject.Path.Path);
			}
			result.BindingGroupName = subject.BindingGroupName;
			result.BindsDirectlyToSource = subject.BindsDirectlyToSource;
			result.Converter = subject.Converter;
			result.ConverterCulture = subject.ConverterCulture;
			result.ConverterParameter = subject.ConverterParameter;
			result.FallbackValue = subject.FallbackValue;
			result.IsAsync = subject.IsAsync;
			result.Mode = subject.Mode;
			result.NotifyOnSourceUpdated = subject.NotifyOnSourceUpdated;
			result.NotifyOnTargetUpdated = subject.NotifyOnTargetUpdated;
			result.NotifyOnValidationError = subject.NotifyOnValidationError;
			result.StringFormat = subject.StringFormat;
			result.TargetNullValue = subject.TargetNullValue;
			result.UpdateSourceExceptionFilter = subject.UpdateSourceExceptionFilter;
			result.UpdateSourceTrigger = subject.UpdateSourceTrigger;
			result.ValidatesOnDataErrors = subject.ValidatesOnDataErrors;
			result.ValidatesOnExceptions = subject.ValidatesOnExceptions;
			result.XPath = subject.XPath;

			if (newSource != null)
			{
				result.Source = newSource;
			}
			else if (newRelativeSource != null)
			{
				result.RelativeSource = newRelativeSource;
			}
			else if (newElementName != null)
			{
				result.ElementName = newElementName;
			}
			else if (subject.Source != null)
			{
				result.Source = subject.Source;
			}
			else if (subject.RelativeSource != null)
			{
				result.RelativeSource = subject.RelativeSource;
			}
			else if (!String.IsNullOrEmpty(subject.ElementName))
			{
				result.ElementName = subject.ElementName;
			}

			foreach (ValidationRule rule in subject.ValidationRules)
			{
				result.ValidationRules.Add(rule);
			}

			return result;
		}

		#endregion static methods
	}
}