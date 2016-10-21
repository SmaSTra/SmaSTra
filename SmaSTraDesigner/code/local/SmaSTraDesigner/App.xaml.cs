using SmaSTraDesigner.BusinessLogic.classhandler;
using SmaSTraDesigner.BusinessLogic.classhandler.nodeclasses;
using SmaSTraDesigner.BusinessLogic.savingloading;

namespace SmaSTraDesigner
{
    using System.Windows;

    using Common;

    using BusinessLogic;
    using BusinessLogic.config;
    using System.IO;
    using BusinessLogic.utils;
    using Controls;
    using System.Diagnostics;
    using System;
    using Controls.Support;
    using System.Threading;

    /// <summary>
    /// Interaction logic for App.xaml
    /// </summary>
    public partial class App : Application
	{

        #region consts

        /// <summary>
        /// FEATURE TOGGLE: Handle global Exceptions!
        /// </summary>
        private const bool HandleGlobalException = true;


        private const string RegSubKey = "SmaSTra";
        private const string RegWorkspaceKey = "workspace";

        #endregion consts

        #region event handlers

        private void Application_DispatcherUnhandledException(object sender, System.Windows.Threading.DispatcherUnhandledExceptionEventArgs e)
		{
            if (HandleGlobalException)
            {
                e.Handled = true;
                var exp = e.Exception;

                //TODO: Do a new Fanxy GUI for the Error-Handling!
                new DialogErrorHandler(exp) {Topmost = true}.ShowDialog();
            }
		}

        
		private void Application_Startup(object sender, StartupEventArgs e)
        {
            SplashWindow splash = new SplashWindow();
            splash.Show();

            //Read the last used Workspace:
            string lastWorkspace = ReadWorkSpaceFromRegestry();
            SwitchWorkspace(lastWorkspace, null);
            
            MainWindow main = new MainWindow();
            Thread.Sleep(3000);
            splash.Close();
            main.Show();
        }

        /// <summary>
        /// Reads the Workspace from the Registry.
        /// If not present, takes the current working directory.
        /// </summary>
        /// <returns>The Workspace</returns>
        private string ReadWorkSpaceFromRegestry()
        {
            try
            {
                var key = Microsoft.Win32.Registry.CurrentUser.OpenSubKey(RegSubKey);
                if (key != null)
                {
                    var workSpace = key.GetValue(RegWorkspaceKey, "").ToString();
                    key.Close();
                    return workSpace;
                }
            }
            catch (Exception exp)
            {
                Debug.Print(exp.ToString());
            }

            return Directory.GetCurrentDirectory();
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
                var key = Microsoft.Win32.Registry.CurrentUser.CreateSubKey(RegSubKey);
                if (key != null)
                {
                    key.SetValue(RegWorkspaceKey, newWorkspace);
                    key.Close();
                }
            }catch(Exception exp){ Debug.Print(exp.ToString()); }


            //Clear the current Tree and the GUI:
            treeDesigner?.Clear();


            //Set the new Workspace:
            WorkSpace.DIR = newWorkspace;
            CopyDefault(WorkSpace.DIR);


            //Reload the managers:
            DataType.ReloadFromBaseFolder();
            Singleton<SmaSTraConfiguration>.Instance.Reload();
            Singleton<NodeBlacklist>.Instance.Reload();
            Singleton<ClassManager>.Instance.Reload();
            Singleton<Library>.Instance.loadLibrary();

            //Restore last state:
            if(treeDesigner != null) RegularSaver.TryLoad(treeDesigner);
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