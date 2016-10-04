using SmaSTraDesigner.BusinessLogic.classhandler.nodeclasses.extras;
using SmaSTraDesigner.BusinessLogic.utils;
using System.IO;
using System.Linq;

namespace SmaSTraDesigner.BusinessLogic.codegeneration.javacodegenerator.exporters
{
    class DefaultCodeExporter : AbstractCodeExporter
    {

        public override bool Supports(string directory)
        {
            //The default exporter is ALWAYS supported!
            return true;
        }


        public override void save(string directory, string javaClass, CodeExtension codeExtension)
        {
            this.CopyNodes(directory, codeExtension.GetAllUsedNodes());
            this.CopyLibs(directory);
            this.CopyJavaFile(directory, javaClass, codeExtension);

            //TODO create text:
            string readMeText = 
                "Please do the following steps to use " + codeExtension.ClassName + ":\n" +
                "  1: Add the Classes in the folders to your IDE. \n" +
                "  2: Add the Libs in the 'libs' folder to your IDE and include it to the Project.\n" +
                "  3: Include the permissions below to your Manifest.\n" + 
                "  4: Create a new " + codeExtension.ClassName + " in your code.\n" +
                "  5: Populate all Setters in the class according to what they need to be set.\n" +
                "  6: Call .start(); on " + codeExtension.ClassName + " to start it.\n" +
                "\n\n Permission Needed:\n" +
                codeExtension.GetExtras()
                    .Select(e => e as NeedsPermission)
                    .NonNull()
                    .Select(p => p.Permission)
                    .StringJoin("\n")
                ;


            File.WriteAllText(Path.Combine(directory, "README.txt"), readMeText);
        }

    }
}
