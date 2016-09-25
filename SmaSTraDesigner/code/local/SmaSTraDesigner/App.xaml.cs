namespace SmaSTraDesigner
{
    using System.Windows;

    using Common;

    using BusinessLogic;
    using BusinessLogic.config;
    using System.IO;
    using BusinessLogic.utils;
    using Controls;
    using System.Linq;
    using System.Diagnostics;
    using System;

    /// <summary>
    /// Interaction logic for App.xaml
    /// </summary>
    public partial class App : Application
	{

        #region consts

        private const string REG_SUB_KEY = "SmaSTra";
        private const string REG_WORKSPACE_KEY = "workspace";

        #endregion consts

        #region event handlers

        private void Application_DispatcherUnhandledException(object sender, System.Windows.Threading.DispatcherUnhandledExceptionEventArgs e)
		{
		}

		private void Application_Startup(object sender, StartupEventArgs e)
        {
            //Read the last used Workspace:
            string lastWorkspace = "";
            try
            {
                Microsoft.Win32.RegistryKey key = Microsoft.Win32.Registry.CurrentUser.OpenSubKey("SmaSTra");
                if(key != null)
                {
                    lastWorkspace = key.GetValue(REG_WORKSPACE_KEY, "").ToString();
                    key.Close();
                }
            }catch (Exception exp) { Debug.Print(exp.ToString()); }

            SwitchWorkspace(lastWorkspace, null);
        }


        /// <summary>
        /// Switches the Workspace.
        /// </summary>
        /// <param name="newWorkspace">to switch to.</param>
        /// <param name="treeDesigner">The tree designer to remove remaining stuff.</param>
        public static void SwitchWorkspace(string newWorkspace, UcTreeDesigner treeDesigner)
        {
            //Be sure the new Workspace exists:
            if(!string.IsNullOrWhiteSpace(newWorkspace)) Directory.CreateDirectory(newWorkspace);


            //Save the new Workspace to the Registry:
            try{
                Microsoft.Win32.RegistryKey key = Microsoft.Win32.Registry.CurrentUser.CreateSubKey("SmaSTra");
                key.SetValue(REG_WORKSPACE_KEY, newWorkspace);
                key.Close();
            }catch(Exception exp){ Debug.Print(exp.ToString()); }


            //Clear the current Tree and the GUI:
            if (treeDesigner != null)
            {
                treeDesigner.Clear();
            }


            //Set the new Workspace:
            WorkSpace.DIR = newWorkspace;


            //Copy all the Basic stuff in:
            string newGeneratedPath = Path.Combine(WorkSpace.DIR, "generated");
            if (  !Directory.Exists(newGeneratedPath) 
                || Directory.EnumerateFileSystemEntries(newGeneratedPath).Empty())
            {
                string orgGeneratedPath = "generated";
                DirCopy.PlainCopy(orgGeneratedPath, newGeneratedPath);
            }


            //Reload the managers:
            Singleton<SmaSTraConfiguration>.Instance.Reload();
            Singleton<ClassManager>.Instance.Reload();
            Singleton<Library>.Instance.loadLibrary();


            //At last change the title of the Main-Window:
            if(App.Current != null && App.Current.MainWindow != null)
            {
                App.Current.MainWindow.Title = "SmaSTra Designer (WS: " + newWorkspace + ")";
            }
        }

		#endregion event handlers
	}
}