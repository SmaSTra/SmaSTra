using Common.ExtensionMethods;
using Newtonsoft.Json.Linq;
using SmaSTraDesigner.BusinessLogic.codegeneration.loader.specificloaders;
using SmaSTraDesigner.BusinessLogic.utils;
using System.Linq;

namespace SmaSTraDesigner.BusinessLogic.codegeneration.loader
{
    class TransformationLoader : AbstractNodeLoader
    {
        public TransformationLoader(ClassManager cManager) 
            : base(ClassManager.NodeType.Transformation, cManager)
        {}

        public override NodeClass loadFromJson(string name, JObject root)
        {
            string displayName = ReadDisplayName(root).EmptyDefault(name);
            string description = ReadDescription(root).EmptyDefault("No Description");
            DataType output = ReadOutput(root);
            DataType[] inputs = ReadInputs(root);
            Node baseNode = new Transformation()
            {
                Name = displayName
            };

            return new NodeClass(nodeType, name, baseNode, output, inputs)
            {
                DisplayName = displayName,
                Description = description
            };
        }

        public override JObject classToJson(NodeClass nodeClass)
        {
            JObject root = new JObject();
            AddOwnType(root);
            AddDescription(root, nodeClass.Description);
            AddDisplayName(root, nodeClass.DisplayName);
            AddOutput(root, nodeClass.OutputType);
            AddInputs(root, nodeClass.InputTypes);

            return root;
        }


        public override string GenerateClassFromSnippet(NodeClass nodeClass, string methodCode)
        {
            string methodArgs = "";
            for (int i = 0 ; i < nodeClass.InputTypes.Count(); i++) 
            {
                if (i > 0) methodArgs += ", ";
                methodArgs += nodeClass.InputTypes[i] + " arg"+i;
            }

            return string.Format(ClassTemplates.TRANSFORMATION_TEMPLATE,
                nodeClass.Name, nodeClass.OutputType.Name.RemoveAll(" ", "_"), methodArgs, methodCode);
        }

    }
}
