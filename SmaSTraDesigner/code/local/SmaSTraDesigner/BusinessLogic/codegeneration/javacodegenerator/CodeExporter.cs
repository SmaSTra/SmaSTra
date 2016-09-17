using SmaSTraDesigner.BusinessLogic.codegeneration.javacodegenerator.exporters;
using SmaSTraDesigner.BusinessLogic.codegeneration.loader.specificloaders;
using SmaSTraDesigner.BusinessLogic.utils;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Windows;
using System.Xml;
using System.Xml.Linq;

namespace SmaSTraDesigner.BusinessLogic.codegeneration.javacodegenerator
{
    public class CodeExporter
    {

        /// <summary>
        /// The List of Code-Exporters supported.
        /// </summary>
        private readonly List<AbstractCodeExporter> codeExporters = new List<AbstractCodeExporter>();


        public CodeExporter()
        {
            //Here are the exporters for the IDEs supported:
            this.codeExporters.Add(new AndroidStudioCodeExporter());


            //Default exporter is always last!
            this.codeExporters.Add(new DefaultCodeExporter());
        }


        /// <summary>
        /// Saves the Data passed to the directory passed.
        /// </summary>
        /// <param name="directory">To save to</param>
        /// <param name="javaClass">To save</param>
        /// <param name="codeExtension">To use for references</param>
        public void save(string directory, string javaClass, CodeExtension codeExtension)
        {
            if(!codeExporters
                .ExecuteOnFirst(
                    e => e.Supports(directory), 
                    e => e.save(directory, javaClass, codeExtension))
               )
            {
                MessageBox.Show("Could not save Code. :(");
            }
        }

    }


    abstract class AbstractCodeExporter
    {


        /// <summary>
        /// If the Exporter supports this directory.
        /// </summary>
        /// <param name="directory">To check</param>
        /// <returns>true if supports this directory</returns>
        public abstract bool Supports(string directory);


        /// <summary>
        /// Saves the Data passed to the directory passed.
        /// </summary>
        /// <param name="directory">To save to</param>
        /// <param name="javaClass">To save</param>
        /// <param name="codeExtension">To use for references</param>
        public abstract void save(string directory, string javaClass, CodeExtension codeExtension);


        /// <summary>
        /// Copies the Node-Dependencies
        /// </summary>
        /// <param name="destination">To copy to</param>
        /// <param name="nodes">To use.</param>
        protected void CopyNodes(string destination, Node[] nodes)
        {
            nodes.ForEach(n => DirCopy.DirectoryCopy(n, destination));
        }

        /// <summary>
        /// Copies the content of the libs folder to the destination.
        /// </summary>
        /// <param name="destination"></param>
        protected void CopyLibs(string destination)
        {
            string path = Path.Combine("generated", "libs");
            DirCopy.DirectoryCopy(path, destination, true);
        }

        /// <summary>
        /// Copies the Java class dependent on the Destination path and the extensions.
        /// </summary>
        /// <param name="codeSourceDirectory">Save to</param>
        /// <param name="javaCode">To save</param>
        /// <param name="extension">To modify path.</param>
        protected void CopyJavaFile(string codeSourceDirectory, string javaCode, CodeExtension extension)
        {
            //Save the Class to the File wanted:
            string[] directoryPath = new string[] { codeSourceDirectory };
            string[] packagePathSplit = directoryPath.Concat(extension.Package.Split('.')).ToArray();
            string javaClassPath = Path.Combine(packagePathSplit);

            //Create the Directory:
            Directory.CreateDirectory(javaClassPath);
            javaClassPath = Path.Combine(javaClassPath, extension.ClassName + ".java");

            //Finally save:
            File.WriteAllText(javaClassPath, javaCode);
        }

        /// <summary>
        /// Adds the permissions passed to the manifest passed.
        /// </summary>
        /// <param name="manifestPath">To add to</param>
        /// <param name="permissions">To add</param>
        protected void AddPermissionsToManifest(string manifestPath, string[] permissions)
        {
            //No permissions -> Nothing to do!
            if (permissions.Empty()) return;

            XDocument doc = XDocument.Load(manifestPath);
            var root = doc.Root;

            XElement[] nodes = root.Elements("uses-permission").ToArray();
            foreach (string permission in permissions)
            {
                //Check if already present:
                if (nodes.Any(e => e.FirstAttribute.Value == permission)) continue;

                XName name = "{http://schemas.android.com/apk/res/android}" + "permission";
                root.Add(new XElement("uses-permission", new XAttribute(name, permission)));
            }

            doc.Save(manifestPath);
        }

        /// <summary>
        /// Adds the permissions passed to the manifest passed.
        /// </summary>
        /// <param name="manifestPath">To add to</param>
        /// <param name="permissions">To add</param>
        protected void AddPermissionsToManifestWithoutBreakingLayout(string manifestPath, string[] permissions)
        {
            //No permissions -> Nothing to do!
            if (permissions.Empty()) return;

            string manifest = File.ReadAllText(manifestPath);
            permissions = permissions.Where(p => !manifest.Contains(p)).ToArray();
            if (permissions.Empty()) return;

            string permString = string.Join("\n", permissions.Select(p => string.Format(ClassTemplates.GENERATION_TEMPLATE_PERMISSION, p)));
            if (manifest.Contains("</manifest>"))
            {
                manifest = manifest.ReplaceFirst("</manifest>", permString + "\n</manifest>");
                File.WriteAllText(manifestPath, manifest);
            }
            else
            {
                MessageBox.Show("Could not add Permissions to the Manifest. Please do this per hand.", "Manifest error");
            }
        }
    }

}
