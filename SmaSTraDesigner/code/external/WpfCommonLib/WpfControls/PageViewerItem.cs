namespace Common.WpfControls
{
	using System;
	using System.Windows;

	// TODO: (PS) Comment this.
	public class PageViewerItem : DependencyObject
	{
		#region dependency properties

		/// <summary>
		/// Registration of ConstructorParameters Dependency Property.
		/// </summary>
		public static readonly DependencyProperty ConstructorParametersProperty = 
			DependencyProperty.Register("ConstructorParameters", typeof(object[]), typeof(PageViewerItem));

		/// <summary>
		/// Registration of Content Dependency Property.
		/// </summary>
		public static readonly DependencyProperty ContentProperty = 
			DependencyProperty.Register("Content", typeof(object), typeof(PageViewerItem));

		/// <summary>
		/// Registration of ContentType Dependency Property.
		/// </summary>
		public static readonly DependencyProperty ContentTypeProperty = 
			DependencyProperty.Register("ContentType", typeof(Type), typeof(PageViewerItem));

		/// <summary>
		/// Registration of IsSelected Dependency Property.
		/// </summary>
		public static readonly DependencyProperty IsSelectedProperty = 
			DependencyProperty.Register(
				"IsSelected", typeof(bool), typeof(PageViewerItem),
				new FrameworkPropertyMetadata(
					false,
					OnIsSelectedChanged));

		/// <summary>
		/// Registration of KeepContentAlive Dependency Property.
		/// </summary>
		public static readonly DependencyProperty KeepContentAliveProperty = 
			DependencyProperty.Register("KeepContentAlive", typeof(bool), typeof(PageViewerItem));

		/// <summary>
		/// Registration of Owner Dependency Property.
		/// </summary>
		internal static readonly DependencyProperty OwnerProperty = 
			DependencyProperty.Register("Owner", typeof(PageViewer), typeof(PageViewerItem));

		/// <summary>
		/// Registration of Title Dependency Property.
		/// </summary>
		public static readonly DependencyProperty TitleProperty = 
			DependencyProperty.Register("Title", typeof(string), typeof(PageViewerItem));

		#endregion dependency properties

		#region dependency property callbacks

		/// <summary>
		/// Property Changed Callback method of the IsSelected Dependency Property.
		/// </summary>
		/// <param name="sender">The instance of the class that had the IsSelected property changed.</param>
		/// <param name="e">The <see cref="System.Windows.DependencyPropertyChangedEventArgs"/> instance containing the event data.</param>
		private static void OnIsSelectedChanged(DependencyObject sender, DependencyPropertyChangedEventArgs e)
		{
			PageViewerItem subject = (PageViewerItem)sender;
			bool newValue = (bool)e.NewValue;
			bool oldValue = (bool)e.OldValue;

			if (!(subject.Owner == null || subject.Owner.IsSettingSelection))
			{
				if (newValue)
				{
					subject.Owner.SelectedItem = subject;
				}
				else if (subject.Owner.SelectedItem == subject)
				{
					subject.Owner.SelectedItem = null;
				}
			}
		}

		#endregion dependency property callbacks

		#region properties

		/// <summary>
		/// Gets or sets the value of the ConstructorParameters property.
		/// TODO: (PS) Comment this.
		/// This is a Dependency Property.
		/// </summary>
		public object[] ConstructorParameters
		{
			get { return (object[])this.GetValue(ConstructorParametersProperty); }
			set { this.SetValue(ConstructorParametersProperty, value); }
		}

		/// <summary>
		/// Gets or sets the value of the Content property.
		/// TODO: (PS) Comment this.
		/// This is a Dependency Property.
		/// </summary>
		public object Content
		{
			get { return (object)this.GetValue(ContentProperty); }
			set { this.SetValue(ContentProperty, value); }
		}

		/// <summary>
		/// Gets or sets the value of the ContentType property.
		/// TODO: (PS) Comment this.
		/// This is a Dependency Property.
		/// </summary>
		public Type ContentType
		{
			get { return (Type)this.GetValue(ContentTypeProperty); }
			set { this.SetValue(ContentTypeProperty, value); }
		}

		/// <summary>
		/// Gets or sets the value of the IsSelected property.
		/// TODO: (PS) Comment this.
		/// This is a Dependency Property.
		/// </summary>
		public bool IsSelected
		{
			get { return (bool)this.GetValue(IsSelectedProperty); }
			set { this.SetValue(IsSelectedProperty, value); }
		}

		/// <summary>
		/// Gets or sets the value of the KeepContentAlive property.
		/// TODO: (PS) Comment this.
		/// This is a Dependency Property.
		/// </summary>
		public bool KeepContentAlive
		{
			get { return (bool)this.GetValue(KeepContentAliveProperty); }
			set { this.SetValue(KeepContentAliveProperty, value); }
		}

		/// <summary>
		/// Gets or sets the value of the Owner property.
		/// TODO: (PS) Comment this.
		/// This is a Dependency Property.
		/// </summary>
		public PageViewer Owner
		{
			get { return (PageViewer)this.GetValue(OwnerProperty); }
			internal set { this.SetValue(OwnerProperty, value); }
		}

		/// <summary>
		/// Gets or sets the value of the Title property.
		/// TODO: (PS) Comment this.
		/// This is a Dependency Property.
		/// </summary>
		public string Title
		{
			get { return (string)this.GetValue(TitleProperty); }
			set { this.SetValue(TitleProperty, value); }
		}

		#endregion properties

		#region methods

		public void FreeContent()
		{
			if (!this.KeepContentAlive)
			{
				this.Content = null;
			}
		}

		public void InitializeContent(params object[] constructorParameters)
		{
			if (!this.KeepContentAlive || this.Content == null)
			{
				if (this.ContentType == null)
				{
					throw new InvalidOperationException("No content type was given.");
				}

				this.Content = Activator.CreateInstance(this.ContentType, constructorParameters);
			}
		}

		public void InitializeContent()
		{
			this.InitializeContent(this.ConstructorParameters);
		}

		#endregion methods
	}
}