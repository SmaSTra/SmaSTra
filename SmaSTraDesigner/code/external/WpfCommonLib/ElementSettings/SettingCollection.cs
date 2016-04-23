namespace Common.ElementSettings
{
	using System;
	using System.Collections.ObjectModel;
	using System.ComponentModel;
	using System.Linq;
	using System.Windows;

	// TODO: (PS) Comment this.
	public class SettingCollection : Collection<SettingBase>, INotifyPropertyChanged
	{
		#region fields

		/// <summary>
		/// TODO: (PS) Comment this.
		/// </summary>
		private string name = null;

		/// <summary>
		/// TODO: (PS) Comment this.
		/// </summary>
		private FrameworkElement owner = null;

		/// <summary>
		/// TODO: (PS) Comment this.
		/// </summary>
		private SettingsFile settingsFile = null;

		#endregion fields

		#region events

		/// <summary>
		/// Occurs when this instance is initialized.
		/// </summary>
		public event EventHandler Initialized;

		/// <summary>
		/// Occurs when initializing.
		/// </summary>
		public event EventHandler<EventArgs> Initializing;

		/// <summary>
		/// Occurs when a property value changes.
		/// </summary>
		public event PropertyChangedEventHandler PropertyChanged;

		#endregion events

		#region properties

		/// <summary>
		/// Gets or sets the Name property value.
		/// TODO: (PS) Comment this.
		/// </summary>
		public string Name
		{
			get
			{
				return this.name;
			}
			set
			{
				if (value != this.name)
				{
					string oldValue = this.name;
					this.name = value;
					this.OnNameChanged(oldValue, value);
				}
			}
		}

		/// <summary>
		/// Gets or sets the Owner property value.
		/// TODO: (PS) Comment this.
		/// </summary>
		public FrameworkElement Owner
		{
			get
			{
				return this.owner;
			}
			set
			{
				if (value != this.owner)
				{
					FrameworkElement oldValue = this.owner;
					this.owner = value;
					this.OnOwnerChanged(oldValue, value);
				}
			}
		}

		public bool SaveAfterEachPropertyChange
		{
			get;
			set;
		}

		/// <summary>
		/// Gets or sets the SettingsFile property value.
		/// TODO: (PS) Comment this.
		/// </summary>
		public SettingsFile SettingsFile
		{
			get
			{
				if (this.settingsFile == null)
				{
					this.settingsFile = SettingsFile.Default;
				}

				return this.settingsFile;
			}
			set
			{
				if (value != null && value != this.settingsFile)
				{
					SettingsFile oldValue = this.settingsFile;
					this.settingsFile = value;
					this.OnSettingsFileChanged(oldValue, value);
				}
			}
		}

		#endregion properties

		#region overrideable methods

		/// <summary>
		/// Called when the Name property changed its value.
		/// </summary>
		/// <param name="oldValue">The old value.</param>
		/// <param name="newValue">The new value.</param>
		protected virtual void OnNameChanged(string oldValue, string newValue)
		{
			this.OnPropertyChanged("Name");
		}

		/// <summary>
		/// Called when the Owner property changed its value.
		/// </summary>
		/// <param name="oldValue">The old value.</param>
		/// <param name="newValue">The new value.</param>
		protected virtual void OnOwnerChanged(FrameworkElement oldValue, FrameworkElement newValue)
		{
			if (newValue != null && !System.ComponentModel.DesignerProperties.GetIsInDesignMode(newValue))
			{
				newValue.Unloaded += this.Owner_Unloaded;
				newValue.Dispatcher.ShutdownStarted += this.Dispatcher_ShutdownStarted;
				newValue.Loaded += this.Owner_Loaded;

				if (this.Name == null)
				{
					this.Name = String.Join(" ", newValue.GetType().Name, newValue.Name);
				}
			}
			if (oldValue != null && !System.ComponentModel.DesignerProperties.GetIsInDesignMode(oldValue))
			{
				oldValue.Unloaded -= this.Owner_Unloaded;
				oldValue.Dispatcher.ShutdownStarted -= this.Dispatcher_ShutdownStarted;
				oldValue.Loaded -= this.Owner_Loaded;
			}

			this.OnPropertyChanged("Owner");
		}

		/// <summary>
		/// Called when the SettingsFile property changed its value.
		/// </summary>
		/// <param name="oldValue">The old value.</param>
		/// <param name="newValue">The new value.</param>
		protected virtual void OnSettingsFileChanged(SettingsFile oldValue, SettingsFile newValue)
		{
		}

		protected override void ClearItems()
		{
			SettingBase[] oldSettings = this.ToArray();

			base.ClearItems();

			foreach (SettingBase setting in oldSettings)
			{
				this.UnloadSetting(setting);
			}
		}

		protected override void InsertItem(int index, SettingBase item)
		{
			base.InsertItem(index, item);
			this.LoadSetting(item);
		}

		protected override void RemoveItem(int index)
		{
			SettingBase oldSetting = this[index];

			base.RemoveItem(index);

			this.UnloadSetting(this[index]);
		}

		protected override void SetItem(int index, SettingBase item)
		{
			SettingBase oldSetting = this[index];

			base.SetItem(index, item);

			this.UnloadSetting(oldSetting);
			this.LoadSetting(item);
		}

		#endregion overrideable methods

		#region methods

		public void Save(bool saveRightNow = false)
		{
			if (this.settingsFile != null)
			{
				this.SettingsFile.Write(saveRightNow);
			}
		}

		private void Initialize()
		{
			this.OnInitializing();

			foreach (SettingBase setting in this)
			{
				setting.Initialize();
			}

			this.OnInitialized();
		}

		private void LoadSetting(SettingBase setting)
		{
			setting.Owner = this;
		}

		/// <summary>
		/// Raises the <see cref="E:Initialized"/> event.
		/// </summary>
		private void OnInitialized()
		{
			if (this.Initialized != null)
			{
				this.Initialized(this, null);
			}
		}

		/// <summary>
		/// Raises the <see cref="E:Initializing"/> event.
		/// </summary>
		private void OnInitializing()
		{
			if (this.Initializing != null)
			{
				this.Initializing(this, null);
			}
		}

		/// <summary>
		/// Called when [property changed].
		/// </summary>
		/// <param name="propertyName">Name of the property.</param>
		private void OnPropertyChanged(string propertyName)
		{
			if (this.PropertyChanged != null)
			{
				this.PropertyChanged(this, new PropertyChangedEventArgs(propertyName));
			}
		}

		private void UnloadSetting(SettingBase setting)
		{
			setting.Owner = null;
		}

		#endregion methods

		#region event handlers

		/// <summary>
		/// Handles the ShutdownStarted event of the Dispatcher.
		/// </summary>
		/// <param name="sender">The source of the event.</param>
		/// <param name="e">The <see cref="System.EventArgs"/> instance containing the event data.</param>
		private void Dispatcher_ShutdownStarted(object sender, EventArgs e)
		{
			this.Save(true);
		}

		/// <summary>
		/// Handles the Loaded event of the Owner control.
		/// </summary>
		/// <param name="sender">The source of the event.</param>
		/// <param name="e">The <see cref="System.Windows.RoutedEventArgs"/> instance containing the event data.</param>
		private void Owner_Loaded(object sender, RoutedEventArgs e)
		{
			if (e.OriginalSource == sender)
			{
				this.Initialize();
			}
		}

		/// <summary>
		/// Handles the Unloaded event of the Owner control.
		/// </summary>
		/// <param name="sender">The source of the event.</param>
		/// <param name="e">The <see cref="System.Windows.RoutedEventArgs"/> instance containing the event data.</param>
		private void Owner_Unloaded(object sender, RoutedEventArgs e)
		{
			if (e.OriginalSource == sender)
			{
				this.Save();
				this.Owner.SetSettings(null);
			}
		}

		#endregion event handlers
	}
}