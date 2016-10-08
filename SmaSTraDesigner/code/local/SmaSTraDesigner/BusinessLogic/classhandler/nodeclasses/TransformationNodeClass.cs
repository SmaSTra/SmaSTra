using SmaSTraDesigner.BusinessLogic.utils;
using System;
using System.IO;
using System.Linq;

namespace SmaSTraDesigner.BusinessLogic.classhandler.nodeclasses
{
    public class TransformationNodeClass : AbstractNodeClass
    {

        public string Method { get;  }

        public bool IsStatic { get; }

        public TransformationNodeClass(string name, string displayName, string description, string creator, DataType outputType, DataType[] inputTypes,
            string mainClass, string[] needsOtherClasses, NeedsExtra[] needsExtra, ConfigElement[] config, ProxyProperty[] proxyProperties,
            bool userCreated, string nodePath,
            string methodName, bool isStatic)
            : base(ClassManager.NodeType.Transformation, name, displayName, description, creator, outputType, 
                  mainClass, needsOtherClasses, needsExtra, 
                  config, proxyProperties, inputTypes, userCreated, nodePath)
        {
            this.Method = methodName;
            this.IsStatic = isStatic;
        }


        public override Node generateNode()
        {
            return new Transformation(this);
        }
        

        protected override string ReadSourceCode()
        {
            string file = Path.Combine(NodePath, MainClass.Replace('.', Path.DirectorySeparatorChar) + ".java");
            if (!File.Exists(file))
            {
                return null;
            }

            //Found the File -> Read!
            string[] lines = File.ReadAllLines(file);
            int startLine = -1;
            int endLine = lines.Length - 1;
            int curlyBreaketsOpen = 0;

            for(int i = 0; i < lines.Length; i++)
            {
                string line = lines[i];

                //Skip empty lines:
                if (startLine < 0 && string.IsNullOrWhiteSpace(line))
                {
                    continue;
                }

                //First find the start of the Method:
                if(startLine  < 0 
                    && line.Contains("public ") 
                    && line.Contains(" " + Method) 
                    && line.Contains("static ") == IsStatic)
                {
                    startLine = i;
                    curlyBreaketsOpen += line.Count(c => c == '{');
                    curlyBreaketsOpen -= line.Count(c => c == '}');
                    continue;
                }

                //Now try to find the End:
                if (startLine >= 0)
                {
                    curlyBreaketsOpen += line.Count(c => c == '{');
                    curlyBreaketsOpen -= line.Count(c => c == '}');

                    if (curlyBreaketsOpen <= 0)
                    {
                        endLine = i;
                        break;
                    }
                }
            }

            //Did not find start:
            if(startLine < 0)
            {
                return lines.StringJoin(Environment.NewLine);
            }

            //Get only the lines wanted:
            return lines
                .Skip(startLine)
                .Take(endLine - startLine + 1)
                .StringJoin(Environment.NewLine);
        }
    }
}
