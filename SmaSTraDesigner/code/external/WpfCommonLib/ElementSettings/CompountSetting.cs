namespace Common.ElementSettings
{
	using System;
	using System.Collections.Generic;
	using System.Collections.ObjectModel;
	using System.Collections.Specialized;
	using System.Linq;
	using System.Windows.Data;

	using Common.ExtensionMethods;
	using Common.Resources.Converters;

	// TODO: (PS) Comment this.
	public class CompountSetting : SettingBase
	{
		#region fields

		private bool initialized = false;
		private ObservableCollection<Setting> settings = new ObservableCollection<Setting>();

		#endregion fields

		#region constructors

		public CompountSetting()
		{
			this.settings.CollectionChanged += this.Settings_CollectionChanged;
		}

		public CompountSetting(IEnumerable<Setting> settings)
			: this()
		{
			this.settings.AddRange(settings);
		}

		public CompountSetting(params Setting[] settings)
			: this(settings.AsEnumerable())
		{
		}

		#endregion constructors

		#region overrideable methods

		protected override void InitializeOverride()
		{
			foreach (var setting in this.settings)
			{
				setting.Initialize();
			}
			this.initialized = true;
		}

		protected override void ReadValueOverride()
		{
			foreach (var setting in this.settings)
			{
				setting.ReadValue();
			}
		}

		#endregion overrideable methods

		#region methods

		public void AddSetting(Setting setting)
		{
			if (this.initialized)
			{
				throw new InvalidOperationException("Settings can no longer be added once a compount setting is initialized.");
			}

			this.settings.Add(setting);
		}

		#endregion methods

		#region event handlers

		private void Settings_CollectionChanged(object sender, NotifyCollectionChangedEventArgs e)
		{
			if (e.Action == NotifyCollectionChangedAction.Add || e.Action == NotifyCollectionChangedAction.Replace)
			{
				foreach (Setting setting in e.NewItems)
				{
					BindingOperations.SetBinding(setting, SettingBase.OwnerProperty, new Binding("Owner")
					{
						Source = this
					});
					BindingOperations.SetBinding(setting, Setting.NameProperty, new MultiBinding()
					{
						Converter = new LambdaConverter()
						{
							MultiConvertMethod = (values, targetType, parameter, culture) =>
							{
								string name = (string)values[0];
								BindingBase binding = (BindingBase)values[1];

								return String.Format("{0} {1}", name, Setting.GetBindingIdentifyerString(binding));
							}
						}
					}.AddBindings(
						new Binding("Name") { Source = this },
						new Binding("Binding") { Source = setting }));
				}
			}
		}

		#endregion event handlers
	}
}