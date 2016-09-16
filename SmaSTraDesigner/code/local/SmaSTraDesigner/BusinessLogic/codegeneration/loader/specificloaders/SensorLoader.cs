using Newtonsoft.Json.Linq;
using SmaSTraDesigner.BusinessLogic.utils;

namespace SmaSTraDesigner.BusinessLogic.codegeneration.loader
{
    class SensorLoader : AbstractNodeLoader
    {
        public SensorLoader(ClassManager cManager) 
            : base(ClassManager.NodeType.Sensor, cManager)
        {}

        public override NodeClass loadFromJson(string name, JObject root)
        {
            string displayName = ReadDisplayName(root).EmptyDefault(name);
            string description = ReadDescription(root).EmptyDefault("No Description");
            DataType output = ReadOutput(root);
            Node baseNode = new DataSource()
            {
                Name = displayName
            };

            return new NodeClass(nodeType, name, baseNode, output)
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

            return root;
        }

    }
}
