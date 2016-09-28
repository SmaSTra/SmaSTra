using SmaSTraDesigner.BusinessLogic.config;
using SmaSTraDesigner.BusinessLogic.utils;
using System.IO;
using System.IO.Compression;
using System.Linq;
using System.Windows;

namespace SmaSTraDesigner.BusinessLogic.codegeneration.javacodegenerator.exporters
{
    class EclipseExporter : AbstractCodeExporter
    {

        private const int MANIFEST_SEARCH_DEPTH = 8;
        private const string MANIFEST_FILE_NAME = "AndroidManifest.xml";
        private const string PROJECT_PROPERTIES_FILE = "project.properties";
        private const string CLASS_PATH_FILE = ".classpath";
        private const string PROJECT_FILE = ".project";


        public override bool Supports(string directory)
        {
            //Check if we have a Eclipse Folder:
            string manifestPath = SearchManifest(directory);
            if (manifestPath == null)
            {
                return false;
            }

            //We have a manifest!
            //Check for Project File, classpath file and Project file:
            DirectoryInfo eclipseProjectPath = Directory.GetParent(manifestPath);
            if(    !File.Exists(Path.Combine(eclipseProjectPath.FullName, PROJECT_PROPERTIES_FILE))
                || !File.Exists(Path.Combine(eclipseProjectPath.FullName, CLASS_PATH_FILE))
                || !File.Exists(Path.Combine(eclipseProjectPath.FullName, PROJECT_FILE))) 
            {
                return false;
            }

            string projectName = eclipseProjectPath.Name;
            string projectPath = eclipseProjectPath.Parent == null ? "" : eclipseProjectPath.Parent.FullName;

            //Ask the user if he wants to export:
            MessageBoxResult result = MessageBox.Show("Found Eclipse IDE Project '" + projectName + "' at : '" 
                    + projectPath + "'. Do you want to integrate it?", "Integrate in Eclipse", 
                    MessageBoxButton.YesNo, MessageBoxImage.Question, MessageBoxResult.No);

            return result == MessageBoxResult.Yes;
        }


        /// <summary>
        /// Searches for the Android manifest.
        /// </summary>
        /// <param name="directory">To search</param>
        /// <returns>The found path or null if none found.</returns>
        protected string SearchManifest(string directory)
        {
            //First search in Top-Directories:
            string tmpPath = directory;
            string tmpManifest;
            for(int i = 0; i < MANIFEST_SEARCH_DEPTH; i++)
            {
                tmpManifest = Path.Combine(tmpPath, MANIFEST_FILE_NAME);
                if (File.Exists(tmpManifest))
                {
                    return tmpManifest;
                }

                //Fix for not having anywhere to go upwards:
                DirectoryInfo parentInfo = Directory.GetParent(tmpPath);
                if (parentInfo == null) break;

                tmpPath = parentInfo.FullName;
            }

            return null;
        }


        public override void save(string directory, string javaClass, CodeExtension codeExtension)
        {
            //Check if we have a Android studio Folder:
            string manifestPath = SearchManifest(directory);
            if (manifestPath == null)
            {
                return;
            }

            //We have a manifest!
            //Now navigate to the Source-Path:
            DirectoryInfo eclipseProjectPath = Directory.GetParent(manifestPath);

            string projectName = eclipseProjectPath.Name;
            string projectPath = eclipseProjectPath.FullName;

            //Add the permissions:
            AddPermissionsToManifestWithoutBreakingLayout(manifestPath, codeExtension.GetNeededPermissions());

            //Copy classes:
            string srcDirPath = Path.Combine(projectPath, "src");

            CopyJavaFile(srcDirPath, javaClass, codeExtension);
            CopyNodes(srcDirPath, codeExtension.GetAllUsedNodes());

            //Copy lib:
            string libPath = Path.Combine(projectPath, "libs");
            Directory.CreateDirectory(libPath);
            CopyLibs(libPath);

            //Add Lib to Eclipse File:
            //Since Eclipse does not support AAR files, we need to copy them manually:
            DirectoryInfo libDir = new DirectoryInfo(Path.Combine(WorkSpace.DIR, WorkSpace.LIBS_DIR));
            foreach(var file in libDir.GetFiles())
            {
                //Extract the classes file from the AAR files:
                if (file.Name.EndsWith(".aar"))
                {
                    using (ZipArchive zip = ZipFile.Open(file.FullName, ZipArchiveMode.Read))
                    {
                        zip.Entries
                            .Where(e => e.Name == "classes.jar")
                            .ForEachTryIgnore(e => e.ExtractToFile(Path.Combine(libPath,file.Name)));
                    }
                }

                if (file.Name.EndsWith(".jar"))
                {
                    file.CopyTo(Path.Combine(libPath, file.Name));
                }
            }

            //TODO Add libs to project file?!? No clue where! :(

        }

    }
}
