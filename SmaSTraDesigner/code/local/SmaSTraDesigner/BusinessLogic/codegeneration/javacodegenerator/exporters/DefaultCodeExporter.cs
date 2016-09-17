using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

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
        }

    }
}
