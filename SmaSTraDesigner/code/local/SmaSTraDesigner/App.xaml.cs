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

            //We have the default Workspace -> Set it correct:
            if (string.IsNullOrWhiteSpace(lastWorkspace))
            {
                lastWorkspace = Directory.GetCurrentDirectory();
            }

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
            CopyDefault(WorkSpace.DIR);


            //Reload the managers:
            DataType.ReloadFromBaseFolder();
            Singleton<SmaSTraConfiguration>.Instance.Reload();
            Singleton<ClassManager>.Instance.Reload();
            Singleton<Library>.Instance.loadLibrary();
        }

        /// <summary>
        /// Copies the Default stuff to the new Directory.
        /// </summary>
        /// <param name="to">To copy to</param>
        private static void CopyDefault(string to)
        {
            //Copy all the Basic stuff in:
            string newBasePath = Path.Combine(to, WorkSpace.BASE_DIR);
            if (!Directory.Exists(newBasePath)
                || Directory.EnumerateFileSystemEntries(newBasePath).Empty())
            {
                DirCopy.PlainCopy(WorkSpace.BASE_DIR, newBasePath);
            }

            //Copy all Libs stuff:
            string newLibsPath = Path.Combine(to, WorkSpace.LIBS_DIR);
            if (!Directory.Exists(newLibsPath)
                || Directory.EnumerateFileSystemEntries(newLibsPath).Empty())
            {
                DirCopy.PlainCopy(WorkSpace.LIBS_DIR, newLibsPath);
            }

            //Copy all DataType stuff:
            string newDataTypesPath = Path.Combine(to, WorkSpace.DATA_TYPES_DIR);
            if (!Directory.Exists(newDataTypesPath)
                || Directory.EnumerateFileSystemEntries(newDataTypesPath).Empty())
            {
                DirCopy.PlainCopy(WorkSpace.DATA_TYPES_DIR, newDataTypesPath);
            }
        }

		#endregion event handlers
	}
}