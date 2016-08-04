namespace SmaSTraDesigner
{
	using System;
	using System.Collections.Generic;
	using System.Configuration;
	using System.Data;
	using System.IO;
	using System.Linq;
	using System.Threading.Tasks;
	using System.Windows;

	using Common;

	using SmaSTraDesigner.BusinessLogic;

	/// <summary>
	/// Interaction logic for App.xaml
	/// </summary>
	public partial class App : Application
	{
		#region event handlers

		private void Application_DispatcherUnhandledException(object sender, System.Windows.Threading.DispatcherUnhandledExceptionEventArgs e)
		{
		}

		private void Application_Startup(object sender, StartupEventArgs e)
        {
            Singleton<ClassManager>.Instance.LoadClasses(Path.Combine(Environment.CurrentDirectory, "generated"));
            Singleton<ClassManager>.Instance.LoadClasses(Path.Combine(Environment.CurrentDirectory, "created"));
        }

		#endregion event handlers
	}
}