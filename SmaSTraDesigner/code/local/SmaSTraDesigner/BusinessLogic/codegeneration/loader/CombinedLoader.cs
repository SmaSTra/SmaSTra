using Newtonsoft.Json.Linq;
using SmaSTraDesigner.BusinessLogic.classhandler;
using SmaSTraDesigner.BusinessLogic.nodes;
using SmaSTraDesigner.BusinessLogic.utils;
using System.Collections.Generic;
using System.Linq;

namespace SmaSTraDesigner.BusinessLogic.codegeneration.loader
{
    class CombinedLoader : AbstractNodeLoader
    {

        #region consts

        /// <summary>
        /// Name of the connections if this is a Combined node.
        /// </summary>
        private const string JSON_PROP_CONNECTIONS = "connections";

        /// <summary>
        /// Name of the path to the sub-Elements if this is a CombinedNode.
        /// </summary>
        private const string JSON_PROP_SUB_ELEMENTS = "subElements";

        /// <summary>
        /// The path to the Properties of the Sub-Elements.
        /// </summary>
        private const string JSON_PROP_SUB_ELEMENTS_PROPERTIES = "Properties";

        /// <summary>
        /// Name of the path for the Output Node in a Combined Node.
        /// </summary>
        private const string JSON_PROP_OUTPUT_NODE_NAME = "outputNodeName";

        #endregion consts


        public CombinedLoader(ClassManager cManager) 
            : base(ClassManager.NodeType.Combined, cManager)
        { }


        public override NodeClass loadFromJson(string name, JObject root)
        {
            string displayName = ReadDisplayName(root).EmptyDefault(name);
            string description = ReadDescription(root).EmptyDefault("No Description");
            DataType output = ReadOutput(root);
            DataType[] inputs = ReadInputs(root);
            List<SimpleSubNode> subNodes = ReadSubNodes(root);
            List<SimpleConnection> connection = ReadConnections(root);
            string outputNodeID = ReadOutputNodeID(root);
                
            Node baseNode = new CombinedNode()
            {
                Name = displayName
            };

            return new CombinedNodeClass(nodeType, name, baseNode, subNodes, connection, output, outputNodeID, inputs)
            {
                DisplayName = displayName,
                Description = description
            };
        }



        private List<SimpleSubNode> ReadSubNodes(JObject root)
        {
            return root
                .GetValueAsJArray(JSON_PROP_SUB_ELEMENTS, new JArray())
                .ToJObj().NonNull()
                .Select(n => n.GetValueAsJObject(JSON_PROP_SUB_ELEMENTS_PROPERTIES)).NonNull()
                .Select(o => o.ToStringString())
                .Select(m => new SimpleSubNode(m))
                .ToList();
        }

        private List<SimpleConnection> ReadConnections(JObject root)
        {
            return root
                .GetValueAsJArray(JSON_PROP_CONNECTIONS)
                .Collect(ReadConnection)
                .NonNull()
                .ToList();
        }

        private SimpleConnection ReadConnection(JToken tObj)
        {
            JObject obj = tObj as JObject;
            if (obj == null) return null;

            string firstNode = obj.GetValueAsString("firstNode");
            string secondNode = obj.GetValueAsString("secondNode");
            int position = obj.GetValueAsInt("position");
            return new SimpleConnection(firstNode, secondNode, position);
        }

        /// <summary>
        /// Reads the Output node ID.
        /// </summary>
        /// <param name="root">To read from</param>
        /// <returns>The output node ID</returns>
        private string ReadOutputNodeID(JObject root)
        {
            return root.GetValueAsString(JSON_PROP_OUTPUT_NODE_NAME, "");
        }

    }
}
