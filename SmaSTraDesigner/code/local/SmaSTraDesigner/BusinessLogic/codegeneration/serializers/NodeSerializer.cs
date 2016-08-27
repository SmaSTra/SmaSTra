using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Dynamic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SmaSTraDesigner.BusinessLogic.serializers
{
    class NodeSerializer
    {

        public ExpandoObject serializeNode(Node node)
        {
            dynamic obj = new ExpandoObject();
            obj.NodeType = node is OutputNode ? "OutputNode" : node.Class.Name;
            obj.NodeName = node.Name;
            obj.PosX = node.PosX;
            obj.PosY = node.PosY;

            return obj;
        }


        public Node deserializeNode(dynamic obj, ClassManager classManager)
        {
            String nodeType = obj["NodeType"].ToString();
            String nodeName;
            try
            {
                nodeName = obj["NodeName"].ToString();
            }
            catch
            { // for older save files where NodeName does not exist
                nodeName = nodeType.Replace("_", " ");
            }
            double posX = Double.Parse( obj["PosX"].ToString());
            double posY = Double.Parse( obj["PosY"].ToString());

            //This is the special case when we have the output node:
            if (nodeType == "OutputNode") return new OutputNode();

            //Normal node:
            Node newNode = getNode(nodeType, classManager);
            if (newNode == null) return null;

            //Do not forget to set x,y:
            newNode = (Node) newNode.Clone();
            newNode.Name = nodeName;
            newNode.PosX = posX;
            newNode.PosY = posY;

            return newNode;
        }

        public Connection? deserializeConnection(dynamic obj, IEnumerable<Node> nodes)
        {
            String inputName = obj["input"].ToString();
            String outputName = obj["output"].ToString();
            int inputIndex = (int) obj["inputIndex"];
            
            Node input = nodes.FirstOrDefault<Node>(n => n.Name.Equals(inputName));
            Node output = nodes.FirstOrDefault<Node>(n => n.Name.Equals(outputName));
            if (input == null || output == null) return null;
            return new Connection(output, input, inputIndex);
        }


        private Node getNode(String type, ClassManager classManager)
        {
            return classManager.BaseConversions
                .Concat<Node>(classManager.BaseDataSources)
                .Concat<Node>(classManager.BaseTransformations)
                .Concat<Node>(classManager.BaseCombinedNodes)
                .FirstOrDefault(x => x.Class.Name == type);
        }


        public ExpandoObject serializeNodeConnection(Connection connection)
        {
            dynamic obj = new ExpandoObject();
            obj.input = connection.InputNode.Name;
            obj.output = connection.OutputNode.Name;
            obj.inputIndex = connection.InputIndex;

            return obj;
        }

    }
}
