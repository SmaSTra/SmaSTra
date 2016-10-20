using System;
using System.Collections.Generic;
using System.Dynamic;
using System.Linq;
using Newtonsoft.Json.Linq;
using SmaSTraDesigner.BusinessLogic.classhandler;
using SmaSTraDesigner.BusinessLogic.classhandler.nodeclasses;
using SmaSTraDesigner.BusinessLogic.nodes;
using SmaSTraDesigner.BusinessLogic.utils;

namespace SmaSTraDesigner.BusinessLogic.savingloading.serializers
{
    class NodeSerializer
    {

        public JObject serializeNode(Node node)
        {
            var obj = new JObject();
            obj.Add("NodeType", node is OutputNode ? "OutputNode" : node.Class.Name);
            obj.Add("NodeName", node.Name);
            obj.Add("PosX", node.PosX);
            obj.Add("PosY", node.PosY);
            obj.Add("NodeUUID", node.NodeUUID);

            var inputProperties =
                node.InputIOData
                .Select(serialize)
                .ToJArray();
            obj.Add("InputIOData", inputProperties);

            var configOptions = 
                node.Configuration
                .Select(serialize)
                .ToJArray();
            obj.Add("Configuration", configOptions);

            return obj;
        }


        private JObject serialize(IOData data)
        {
            var obj = new JObject();
            obj.Add("type", data.Type.Name);
            obj.Add("value", data.Value);

            return obj;
        }

        private JObject serialize(DataConfigElement config)
        {
            var obj = new JObject();
            obj.Add("key", config.Key);
            obj.Add("value", config.Value);

            return obj;
        }


        public Node deserializeNode(JObject obj, ClassManager classManager)
        {
            if (obj == null || classManager == null) return null;

            var nodeType = obj.GetValueAsString("NodeType", null);
            string nodeName = obj.GetValueAsString("NodeName", nodeName = nodeType.Replace("_", " "));
            var uuid = obj.GetValueAsString("NodeUUID", Guid.NewGuid().ToString());

            var posX = double.Parse( obj["PosX"].ToString());
            var posY = double.Parse( obj["PosY"].ToString());

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

            //Set the IO Data:
            var ioDataArray = obj.GetValue("InputIOData", new JArray()) as JArray;
            if(ioDataArray != null)
            {
                for(var i = 0; i < ioDataArray.Count; i++)
                {
                    var data = deserializeIOData(ioDataArray[i] as JObject);
                    if (data != null) newNode.InputIOData[i].Value = data.Value;
                }
            }

            //Set the Configuration Data:
            var configArray = obj.GetValue("Configuration", new JArray()) as JArray;
            if (configArray != null)
            {
                for (var i = 0; i < configArray.Count; i++)
                {
                    var configObj = configArray[i] as JObject;
                    if(configObj != null) newNode.Configuration[i].Value = configObj.GetValueAsString("value", "");                    
                }
            }


            return newNode;
        }

        private IOData deserializeIOData(JObject obj)
        {
            if (obj == null) return null;

            var typeName = obj.GetValueAsString("type", "");
            var value = obj.GetValueAsString("value", "");

            return new IOData(DataType.GetCachedType(typeName), value);
        }


        public Connection? deserializeConnection(dynamic obj, IEnumerable<Node> nodes)
        {
            var inputName = obj["input"].ToString();
            var outputName = obj["output"].ToString();
            var inputIndex = (int) obj["inputIndex"];
            
            var input = nodes.FirstOrDefault<Node>(n => n.NodeUUID.Equals(inputName));
            var output = nodes.FirstOrDefault<Node>(n => n.NodeUUID.Equals(outputName));
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
