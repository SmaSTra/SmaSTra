namespace Common.ExtensionMethods
{
	using System;
	using System.Windows;

	// TODO: (PS) Comment this.
	public static class PropertyMetadataExtensions
	{
		#region extension methods

		public static PropertyMetadata Clone(this PropertyMetadata subject)
		{
			if (subject == null)
			{
				throw new ArgumentNullException("subject");
			}

			FrameworkPropertyMetadata subjectAsFrameworkPropertyMetadata;
			UIPropertyMetadata subjectAsUIPropertyMetadata;
			PropertyMetadata subjectAsPropertyMetadata;
			if ((subjectAsFrameworkPropertyMetadata = subject as FrameworkPropertyMetadata) != null)
			{
				return new FrameworkPropertyMetadata()
				{
					AffectsArrange = subjectAsFrameworkPropertyMetadata.AffectsArrange,
					AffectsMeasure = subjectAsFrameworkPropertyMetadata.AffectsMeasure,
					AffectsParentArrange = subjectAsFrameworkPropertyMetadata.AffectsParentArrange,
					AffectsParentMeasure = subjectAsFrameworkPropertyMetadata.AffectsParentMeasure,
					AffectsRender = subjectAsFrameworkPropertyMetadata.AffectsRender,
					BindsTwoWayByDefault = subjectAsFrameworkPropertyMetadata.BindsTwoWayByDefault,
					CoerceValueCallback = subjectAsFrameworkPropertyMetadata.CoerceValueCallback,
					DefaultUpdateSourceTrigger = subjectAsFrameworkPropertyMetadata.DefaultUpdateSourceTrigger,
					DefaultValue = subjectAsFrameworkPropertyMetadata.DefaultValue,
					Inherits = subjectAsFrameworkPropertyMetadata.Inherits,
					IsAnimationProhibited = subjectAsFrameworkPropertyMetadata.IsAnimationProhibited,
					IsNotDataBindable = subjectAsFrameworkPropertyMetadata.IsNotDataBindable,
					Journal = subjectAsFrameworkPropertyMetadata.Journal,
					OverridesInheritanceBehavior = subjectAsFrameworkPropertyMetadata.OverridesInheritanceBehavior,
					PropertyChangedCallback = subjectAsFrameworkPropertyMetadata.PropertyChangedCallback,
					SubPropertiesDoNotAffectRender = subjectAsFrameworkPropertyMetadata.SubPropertiesDoNotAffectRender
				};
			}
			else if ((subjectAsUIPropertyMetadata = subject as UIPropertyMetadata) != null)
			{
				return new UIPropertyMetadata(subjectAsUIPropertyMetadata.DefaultValue, subjectAsUIPropertyMetadata.PropertyChangedCallback,
					subjectAsUIPropertyMetadata.CoerceValueCallback, subjectAsUIPropertyMetadata.IsAnimationProhibited);
			}
			else if ((subjectAsPropertyMetadata = subject as PropertyMetadata) != null)
			{
				return new PropertyMetadata(subjectAsPropertyMetadata.DefaultValue, subjectAsPropertyMetadata.PropertyChangedCallback,
					subjectAsPropertyMetadata.CoerceValueCallback);
			}
			else
			{
				return subject.MemberwiseClone();
			}
		}

		#endregion extension methods
	}
}