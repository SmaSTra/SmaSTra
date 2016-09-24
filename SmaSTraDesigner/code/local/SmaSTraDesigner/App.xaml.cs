namespace SmaSTraDesigner
{
    using System;
    using System.IO;
    using System.Windows;

    using Common;

    using SmaSTraDesigner.BusinessLogic;
    using BusinessLogic.config;

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
            SwitchWorkspace("");
        }


        /// <summary>
        /// Switches the Workspace.
        /// </summary>
        /// <param name="newWorkspace"></param>
        public void SwitchWorkspace(string newWorkspace)
        {
            SmaSTraConfiguration.WORK_SPACE = newWorkspace;

            Singleton<SmaSTraConfiguration>.Instance.Reload();
            Singleton<ClassManager>.Instance.Reload();
            Singleton<Library>.Instance.loadLibrary();

            //TODO add reload of Transformation Tree here!
        }

		#endregion event handlers
	}
}