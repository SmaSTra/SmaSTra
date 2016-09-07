﻿using Newtonsoft.Json.Linq;
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
                newNode = (Node)newNode.Clone();
                newNode.Name = nodeName;
            }
            
            //Do not forget to set x,y:
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
