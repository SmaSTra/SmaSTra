using SmaSTraDesigner.BusinessLogic.utils;
using System.IO;
using System.Linq;
using System.Windows;

namespace SmaSTraDesigner.BusinessLogic.codegeneration.javacodegenerator.exporters
{
    class AndroidStudioCodeExporter : AbstractCodeExporter
    {

        private const int MANIFEST_SEARCH_DEPTH = 5;
        private const string MANIFEST_FILE_NAME = "AndroidManifest.xml";
        private readonly string[] DOWN_SEARCH_PATH = new string[] { "app", "src", "main"};


        public override bool Supports(string directory)
        {
            //Check if we have a Android studio Folder:
            string manifestPath = SearchManifest(directory);
            if (manifestPath == null)
            {
                return false;
            }

            //We have a manifest!
            //Now navigate to the Source-Path:
            DirectoryInfo androidStudioProjectPath = Directory.GetParent(manifestPath).Parent.Parent;
            DirectoryInfo androidStudioWorkspacePath = androidStudioProjectPath.Parent;

            //Check if we have gradle files here:
            if (    !File.Exists(Path.Combine(androidStudioProjectPath.FullName, "build.gradle"))
                 || !File.Exists(Path.Combine(androidStudioWorkspacePath.FullName, "build.gradle")) 
                )
            {
                return false;
            }

            string projectName = androidStudioProjectPath.Name;
            string workspacePath = androidStudioWorkspacePath.FullName;

            //Ask the user if he wants to export:
            MessageBoxResult result = MessageBox.Show("Found Android Studio IDE Project '" + projectName + "' at : '" 
                    + workspacePath + "'. Do you want to integrate it?", "Integrate in Android Studio", 
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

                tmpPath = Directory.GetParent(tmpPath).FullName;
            }

            //Second, search in subfolders:
            for(int i = 0; i < DOWN_SEARCH_PATH.Count(); i++)
            {
                tmpPath = directory;
                for(int j = i; j < DOWN_SEARCH_PATH.Count(); j++)
                {
                    tmpPath = Path.Combine(tmpPath, DOWN_SEARCH_PATH[j]);
                }

                tmpPath = Path.Combine(tmpPath, MANIFEST_FILE_NAME);
                if (File.Exists(tmpPath))
                {
                    return tmpPath;
                }
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
            DirectoryInfo androidStudioProjectPath = Directory.GetParent(manifestPath).Parent.Parent;
            DirectoryInfo androidStudioWorkspacePath = androidStudioProjectPath.Parent;

            string projectName = androidStudioProjectPath.Name;
            string workspacePath = androidStudioWorkspacePath.FullName;

            //Add the permissions:
            AddPermissionsToManifestWithoutBreakingLayout(manifestPath, codeExtension.GetNeededPermissions());

            //Copy classes:
            string javaDirPath = Directory.GetParent(manifestPath).FullName;
            javaDirPath = Path.Combine(javaDirPath, "java");
            Directory.CreateDirectory(javaDirPath);

            CopyJavaFile(javaDirPath, javaClass, codeExtension);
            CopyNodes(javaDirPath, codeExtension.GetAllUsedNodes());

            //Copy lib:
            string libPath = Path.Combine(androidStudioProjectPath.FullName, "libs");
            Directory.CreateDirectory(libPath);
            CopyLibs(libPath);

            //Add lib to Gradle file:
            string gradlePath = Path.Combine(androidStudioProjectPath.FullName, "build.gradle");
            if (File.Exists(gradlePath))
            {
                string gradleFile = File.ReadAllText(gradlePath);
                if(!gradleFile.Contains("dirs 'libs'"))
                {
                    //Check if we have an easy go:
                    if (gradleFile.Contains("mavenCentral()"))
                    {
                        gradleFile = gradleFile.Replace("mavenCentral()", "mavenCentral()\n  flatDir {\n    dirs 'libs'\n  }\n");
                    }
                    else
                    {
                        gradleFile +=
                            "\n" +
                            "repositories {\n" +
                            "  mavenCentral()\n" +
                            "  flatDir {\n" +
                            "    dirs 'libs'\n" +
                            "  }\n" +
                            "}\n";
                    }

                    //Add Lib dependencies:
                    gradleFile = gradleFile.ReplaceFirst("dependencies {",
                        "dependencies {\n" +
                        "    compile 'de.tu_darmstadt.smastra.base:SmaSTraBase:1.0@aar'"
                    );

                    //Finally rewrite:
                    File.WriteAllText(gradlePath, gradleFile);
                }
            }
            else
            {
                MessageBox.Show("Did not find the build.gradle, please edit it by hand!", "File not found: build.gradle");
            }
        }

    }
}
