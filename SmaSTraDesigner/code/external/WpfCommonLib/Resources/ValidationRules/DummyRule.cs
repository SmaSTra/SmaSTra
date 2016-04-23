namespace Common.Resources.ValidationRules
{
	using System;
	using System.ComponentModel;
	using System.Globalization;
	using System.Windows;
	using System.Windows.Controls;
	using System.Windows.Data;

	/// <summary>
	/// A ValidationRule Dummy that can be used to define the validation of some
	/// control elsewhere and add some ValidationError using Validation.MarkInvalid(...).
	/// </summary>
	public class DummyRule : ValidationRule
	{
		#region static methods

		/// <summary>
		/// Clears the given FrameworkElement from all ValidationErrors created using the
		/// DummyRule's MarkInvalid(...) method and removes the binding to the DummyErrorProperty.
		/// </summary>
		/// <param name="frameworkElement">The framework element the errors are removed from.</param>
		/// <exception cref="ArgumentNullException" />
		public static void ClearInvalid(FrameworkElement frameworkElement)
		{
			if (frameworkElement == null)
			{
				throw new ArgumentNullException("Argument 'frameworkElement' must not be null.");
			}

			BindingExpression bindex = BindingOperations.GetBindingExpression(frameworkElement, DummyRule.DummyErrorProperty);
			if (bindex != null)
			{
				Validation.ClearInvalid(bindex);

				BindingOperations.ClearBinding(frameworkElement, DummyRule.DummyErrorProperty);
			}
		}

		/// <summary>
		/// Specifies whether the given FrameworkElement is currently marked as invalid
		/// using the DummyErrorProperty.
		/// </summary>
		/// <param name="frameworkElement">The framework element.</param>
		/// <returns>
		/// <c>true</c> if [is marked invalid] [the specified framework element]; otherwise, <c>false</c>.
		/// </returns>
		/// <exception cref="ArgumentNullException" />
		public static bool IsMarkedInvalid(FrameworkElement frameworkElement)
		{
			if (frameworkElement == null)
			{
				throw new ArgumentNullException("Argument 'frameworkElement' must not be null.");
			}

			BindingExpression bindex = BindingOperations.GetBindingExpression(frameworkElement, DummyRule.DummyErrorProperty);
			return bindex != null && bindex.HasError;
		}

		/// <summary>
		/// Marks the specified FrameworkElement as invalid using the DummyRule
		/// in combination with the DummyErrorProperty to fabricate a ValidationError.
		/// </summary>
		/// <param name="frameworkElement">The framework element that is supposed to have a validation error.</param>
		/// <param name="errorContent">Content of the error that is added to the framework element.</param>
		/// <exception cref="ArgumentNullException" />
		public static void MarkInvalid(FrameworkElement frameworkElement, object errorContent)
		{
			if (frameworkElement == null)
			{
				throw new ArgumentNullException("Argument 'frameworkElement' must not be null.");
			}

			DummyRule.MarkInvalid(frameworkElement, errorContent, false);
		}

		/// <summary>
		/// Marks the specified FrameworkElement as invalid using the DummyRule
		/// in combination with the DummyErrorProperty to fabricate a ValidationError.
		/// </summary>
		/// <param name="frameworkElement">The framework element that is supposed to have a validation error.</param>
		/// <param name="errorContent">Content of the error that is added to the framework element.</param>
		/// <param name="newMark">if set to <c>true</c> a new error is simply added else
		/// any existing error is cleared before adding.</param>
		/// <exception cref="ArgumentNullException" />
		public static void MarkInvalid(FrameworkElement frameworkElement, object errorContent, bool newMark)
		{
			if (frameworkElement == null)
			{
				throw new ArgumentNullException("Argument 'frameworkElement' must not be null.");
			}

			if (newMark)
			{
				DummyRule.ClearInvalid(frameworkElement);
			}

			if (!BindingOperations.IsDataBound(frameworkElement, DummyRule.DummyErrorProperty))
			{
				DummyRule rule = new DummyRule(false, errorContent);
				Binding binding = new Binding("ErrorContent")
				{
					Source = rule
				};
				binding.ValidationRules.Add(rule);
				BindingOperations.SetBinding(frameworkElement, DummyRule.DummyErrorProperty, binding);

				BindingExpression bindex = BindingOperations.GetBindingExpression(frameworkElement, DummyRule.DummyErrorProperty);

				Validation.MarkInvalid(bindex, new ValidationError(rule, binding, errorContent, null));

				bindex.UpdateSource();
			}
		}

		#endregion static methods

		#region dependency properties

		/// <summary>
		/// Register DummyErrorProperty
		/// </summary>
		public static readonly DependencyProperty DummyErrorProperty = 
			DependencyProperty.RegisterAttached(
				"DummyError", typeof(object), typeof(DummyRule));

		#endregion dependency properties

		#region constructors

		/// <summary>
		/// Initializes a new instance of the <see cref="DummyRule"/> class.
		/// </summary>
		public DummyRule()
		{
		}

		/// <summary>
		/// Initializes a new instance of the <see cref="DummyRule"/> class.
		/// </summary>
		/// <param name="returnsValid">Specifies whether this validation rule should return a valid result or not.</param>
		public DummyRule(bool returnsValid)
		{
			this.ReturnsValid = returnsValid;
		}

		// TODO: (PS) Comment this.
		public DummyRule(Func<DummyRule, object, CultureInfo, ValidationResult> validationFunction)
		{
			this.ValidationFunction = validationFunction;
		}

		/// <summary>
		/// Initializes a new instance of the <see cref="DummyRule"/> class.
		/// </summary>
		/// <param name="returnsValid">Specifies whether this validation rule should return a valid result or not.</param>
		/// <param name="errorContent">The content of the error that is thrown should this rule be set
		/// to return an invalid result.</param>
		public DummyRule(bool returnsValid, object errorContent)
			: this(returnsValid)
		{
			this.ErrorContent = errorContent;
		}

		#endregion constructors

		#region properties

		/// <summary>
		/// Gets or sets the content of the error that is thrown should this rule be set
		/// to return an invalid result.
		/// </summary>
		public object ErrorContent
		{
			get;
			set;
		}

		/// <summary>
		/// Gets or sets a value indicating whether this validation rule should return a valid result or not.
		/// </summary>
		[DefaultValue(true)]
		public bool ReturnsValid
		{
			get;
			set;
		}

		// TODO: (PS) Comment this.
		public Func<DummyRule, object, CultureInfo, ValidationResult> ValidationFunction
		{
			get;
			set;
		}

		#endregion properties

		#region overrideable methods

		/// <summary>
		/// Validation override.
		/// </summary>
		/// <param name="value">The value from the binding target to check.</param>
		/// <param name="cultureInfo">The culture to use in this rule.</param>
		/// <returns>
		/// A <see cref="T:System.Windows.Controls.ValidationResult"/> object.
		/// </returns>
		public override ValidationResult Validate(object value, CultureInfo cultureInfo)
		{
			if (this.ValidationFunction != null)
			{
				return this.ValidationFunction(this, value, cultureInfo);
			}
			else if (this.ReturnsValid)
			{
				return ValidationResult.ValidResult;
			}
			else
			{
				return new ValidationResult(false, this.ErrorContent);
			}
		}

		#endregion overrideable methods

		#region methods

		/// <summary>
		/// Tells this rule to return an invalid result with the given error content only from now on when
		/// Validate(...) is called.
		/// </summary>
		/// <param name="errorContent">Content with error.</param>
		public void ReturnInvalid(object errorContent)
		{
			this.ErrorContent = errorContent;
		}

		/// <summary>
		/// Tells this rule to return ValidationResult.ValidResult only from now on when
		/// Validate(...) is called.
		/// </summary>
		public void ReturnValid()
		{
			this.ReturnsValid = true;
		}

		#endregion methods
	}
}