using Common;
using Newtonsoft.Json.Linq;
using SmaSTraDesigner.BusinessLogic.classhandler;
using SmaSTraDesigner.BusinessLogic.utils;
using System;
using System.Collections.Generic;
using System.Dynamic;
using System.Linq;

namespace SmaSTraDesigner.BusinessLogic.serializers
{
    class NodeSerializer
    {

        public JObject serializeNode(Node node)
        {
            JObject obj = new JObject();
            obj.Add("NodeType", node is OutputNode ? "OutputNode" : node.Class.Name);
            obj.Add("NodeName", node.Name);
            obj.Add("PosX", node.PosX);
            obj.Add("PosY", node.PosY);
            obj.Add("NodeUUID", node.NodeUUID);

            JArray inputProperties = new JArray();
            node.InputIOData
                .Select(serialize)
                .forEach(inputProperties.Add);
            obj.Add("InputIOData", inputProperties);

            return obj;
        }


        private JObject serialize(IOData data)
        {
            JObject obj = new JObject();
            obj.Add("type", data.Type.Name);
            obj.Add("value", data.Value);

            return obj;
        }


        public Node deserializeNode(JObject obj, ClassManager classManager)
        {
            if (obj == null || classManager == null) return null;

            String nodeType = obj.GetValueAsString("NodeType", null);
            String nodeName = obj.GetValueAsString("NodeName", nodeName = nodeType.Replace("_", " "));
            string uuid = obj.GetValueAsString("NodeUUID", Guid.NewGuid().ToString());

            double posX = Double.Parse( obj["PosX"].ToString());
            double posY = Double.Parse( obj["PosY"].ToString());

            Node newNode;

            //This is the special case when we have the output node:
            if (nodeType == "OutputNode")
            {
                newNode = new OutputNode();
            }
            else
            {
                //Normal node:
                newNode = classManager.GetNewNodeForType(nodeType);
                if (newNode == null) return null;

                newNode.Name = nodeName;
            }
            
            //Do not forget to set x,y:
            newNode.PosX = posX;
            newNode.PosY = posY;
            newNode.ForceUUID(uuid);

            JArray ioDataArray = obj.GetValue("InputIOData", new JArray()) as JArray;
            if(ioDataArray != null)
            {
                for(int i = 0; i < ioDataArray.Count; i++)
                {
                    IOData data = deserializeIOData(ioDataArray[i] as JObject);
                    if (data != null) newNode.InputIOData[i].Value = data.Value;
                }
            }

            return newNode;
        }

        private IOData deserializeIOData(JObject obj)
        {
            if (obj == null) return null;

            ClassManager cManager = Singleton<ClassManager>.Instance;
            string typeName = obj.GetValueAsString("type", "");
            string value = obj.GetValueAsString("value", "");

            return new IOData(cManager.AddDataType(typeName), value);
        }

        public Connection? deserializeConnection(dynamic obj, IEnumerable<Node> nodes)
        {
            String inputName = obj["input"].ToString();
            String outputName = obj["output"].ToString();
            int inputIndex = (int) obj["inputIndex"];
            
            Node input = nodes.FirstOrDefault<Node>(n => n.NodeUUID.Equals(inputName));
            Node output = nodes.FirstOrDefault<Node>(n => n.NodeUUID.Equals(outputName));
            if (input == null || output == null) return null;

            return new Connection(output, input, inputIndex);
        }


        public ExpandoObject serializeNodeConnection(Connection connection)
        {
            dynamic obj = new ExpandoObject();
            obj.input = connection.InputNode.NodeUUID;
            obj.output = connection.OutputNode.NodeUUID;
            obj.inputIndex = connection.InputIndex;

            return obj;
        }

    }
}
