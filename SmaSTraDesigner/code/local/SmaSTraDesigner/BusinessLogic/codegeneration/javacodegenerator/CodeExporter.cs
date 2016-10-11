using SmaSTraDesigner.BusinessLogic.classhandler.nodeclasses;
using SmaSTraDesigner.BusinessLogic.codegeneration.javacodegenerator.exporters;
using SmaSTraDesigner.BusinessLogic.config;
using SmaSTraDesigner.BusinessLogic.utils;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Windows;
using System.Xml.Linq;
using SmaSTraDesigner.BusinessLogic.classhandler.nodeclasses.extras;
using SmaSTraDesigner.BusinessLogic.nodes;

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
            this.codeExporters.Add(new EclipseExporter());
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
                return;
            }

            //Tell the user it worked:
            MessageBox.Show("Code exporting done.", "Exporter");
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
            string path = Path.Combine(WorkSpace.DIR, WorkSpace.LIBS_DIR);
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
        /// Adds the Extras passed to the Manifest Dom.
        /// </summary>
        /// <param name="manifestPath">To load / save to</param>
        /// <param name="extras">To add</param>
        protected void AddExtrasToManifest(string manifestPath, INeedsExtra[] extras) 
        {
            //No extras -> Nothing to do!
            if (extras == null || extras.Empty()) return;

            XDocument doc = XDocument.Load(manifestPath, LoadOptions.PreserveWhitespace);
            var root = doc.Root;

            //Now apply:
            extras.ForEach(e => e.ApplyToManifest(root));
            doc.Save(manifestPath);
        }

    }

}
