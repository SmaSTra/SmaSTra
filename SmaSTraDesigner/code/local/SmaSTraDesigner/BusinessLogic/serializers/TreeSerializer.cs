using Newtonsoft.Json;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using SmaSTraDesigner.BusinessLogic.serializers;
using System.Dynamic;
using System;
using Newtonsoft.Json.Linq;
using Common;

namespace SmaSTraDesigner.BusinessLogic
{


    class TreeSerilizer
    {

        private TransformationTree tree;

        public TreeSerilizer(TransformationTree tree)
        {
            this.tree = tree;
        }


        public void Serialize(string targetFile)
        {
            //Before we start -> Check if we are connected at all:
            if (this.tree == null || tree.OutputNode.InputNode == null)
            {
                Console.WriteLine("Root node is not connected to any node! Can not save!");
                return;
            }


            //Generate the Serializer:
            var nodeSerializer = new NodeSerializer();

            //Combine the JSON:
            dynamic json = new ExpandoObject();
            json.nodes = tree.Nodes.Select(nodeSerializer.serializeNode);
            json.connections = tree.Connections.Select(nodeSerializer.serializeNodeConnection);


            //Create the Json settings:
            var jsonSettings = new JsonSerializerSettings()
            {
                ConstructorHandling = ConstructorHandling.AllowNonPublicDefaultConstructor,
                ReferenceLoopHandling = ReferenceLoopHandling.Ignore,
                Formatting = Newtonsoft.Json.Formatting.Indented
            };

            //Generate a string and write it to the file:
            string text = JsonConvert.SerializeObject(json, jsonSettings);
            File.WriteAllText(targetFile, text);
        }


        public void Deserialize(string targetFile)
        {
            //Read the singleton ClassManager.
            ClassManager classManager = Singleton<ClassManager>.Instance;

            //Generate the Serializer:
            var nodeSerializer = new NodeSerializer();
            TransformationTree tree = new TransformationTree();
            tree.Nodes.Add(new OutputNode());

            dynamic json = JObject.Parse(File.ReadAllText(targetFile));

            //Seems like this is the only way this is synthactically correct....
            //Read Nodes:
            foreach (dynamic obj in json["nodes"])
            {
                Node node = nodeSerializer.deserializeNode(obj, classManager);
                if (node != null) tree.Nodes.Add(node);
            }

            //Read connections:
            foreach (dynamic obj in json["connections"])
            {
                Connection? connection = nodeSerializer.deserializeConnection(obj, tree.Nodes);
                if (connection.HasValue) tree.Connections.Add(connection.Value);
            }

            Console.WriteLine(" Read " + tree.Nodes.Count + " Nodes and " + tree.Connections.Count + " Connections!");
        }
    }
}
