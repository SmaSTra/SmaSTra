using System.IO;

namespace SmaSTraDesigner.BusinessLogic.utils
{
    class DirCopy
    {

        public static void DirectoryCopy(
            Node node, string destDirName)
        {
            string source = GetPathForNode(node.Class);
            DirectoryCopy(source, destDirName, true);
        }


        /// Copying all files from a given source directory to a destination directory, is capable of copying subdirs too. Deletes File "metadata.json" from dest in the end.
        /// Copied from MSDN
        /// </summary>
        /// <param name="sourceDirName">The source directory</param>
        /// <param name="destDirName">The target directory</param>
        /// <param name="copySubDirs">whether substrings should be copied</param>
        public  static void DirectoryCopy(
            string sourceDirName, string destDirName, bool copySubDirs)
        {
            DirectoryInfo dir = new DirectoryInfo(sourceDirName);
            DirectoryInfo[] dirs = dir.GetDirectories();

            // If the source directory does not exist, throw an exception.
            if (!dir.Exists)
            {
                throw new DirectoryNotFoundException(
                    "Source directory does not exist or could not be found: "
                    + sourceDirName);
            }

            // If the destination directory does not exist, create it.
            if (!Directory.Exists(destDirName))
            {
                Directory.CreateDirectory(destDirName);
            }

            // Get the file contents of the directory to copy.
            FileInfo[] files = dir.GetFiles();

            foreach (FileInfo file in files)
            {
                //Do not copy the Metadata files:
                if (file.Name.EndsWith("metadata.json")) continue;

                // Create the path to the new copy of the file.
                string temppath = Path.Combine(destDirName, file.Name);

                // Copy the file.
                file.CopyTo(temppath, true);
            }

            // If copySubDirs is true, copy the subdirectories.
            if (copySubDirs)
            {

                foreach (DirectoryInfo subdir in dirs)
                {
                    // Create the subdirectory.
                    string temppath = Path.Combine(destDirName, subdir.Name);

                    // Copy the subdirectories.
                    DirectoryCopy(subdir.FullName, temppath, copySubDirs);
                }
            }
        }

        /// <summary>
        /// Removing the metadata.json and adding the SmaSTraBase.aar 
        /// </summary>
        /// <param name="sourceDirName">location of the generated-folder</param>
        /// <param name="destDirName">destination directory, where the java-generation has taken place</param>
        public static void FinalizeCopy(string sourceDirName, string destDirName)
        {
            DirectoryInfo dir = new DirectoryInfo(sourceDirName);
            string libsDirName = destDirName + "\\libs\\";

            // If the source directory does not exist, throw an exception.
            if (!dir.Exists)
            {
                throw new DirectoryNotFoundException(
                    "Source directory does not exist or could not be found: "
                    + sourceDirName);
            }
            // If the destination directory does not exist, create it.
            if (!Directory.Exists(destDirName))
            {
                Directory.CreateDirectory(destDirName);
            }
            // If the libs directory does not yet exist, create it.
            if (!Directory.Exists(libsDirName))
            {
                Directory.CreateDirectory(libsDirName);
            }


            FileInfo[] aar = dir.GetFiles("SmaSTraBase.aar"); //aar-file is the first element

            // Create the path to the new copy of the file.
            string libspath = Path.Combine(libsDirName, aar[0].Name);

            // Copy the file.
            aar[0].CopyTo(libspath, true);

            string temppath = Path.Combine(destDirName, "metadata.json");

            File.Delete(temppath);
        }


        public static string GetPathForNode(AbstractNodeClass nodeClass)
        {
            string path = Path.Combine("generated", nodeClass.Name);
            if (Directory.Exists(path))
            {
                return path;
            }

            return Path.Combine("created", nodeClass.Name);
        }

    }
}
