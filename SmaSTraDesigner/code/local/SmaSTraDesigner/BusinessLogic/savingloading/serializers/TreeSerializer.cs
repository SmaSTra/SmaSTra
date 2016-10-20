using System.Collections.Generic;
using System.Dynamic;
using System.IO;
using System.Linq;
using Common;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using SmaSTraDesigner.BusinessLogic.classhandler;
using SmaSTraDesigner.BusinessLogic.nodes;

namespace SmaSTraDesigner.BusinessLogic.savingloading.serializers
{


    public class TreeSerilizer
    {

        

        public static void Serialize(TransformationTree transformationTree, string targetFile)
        {
            //Generate the Serializer:
            var nodeSerializer = new NodeSerializer();

            //Combine the JSON:
            dynamic json = new ExpandoObject();
            json.nodes = transformationTree.Nodes.Select(nodeSerializer.serializeNode);
            json.connections = transformationTree.Connections.Select(nodeSerializer.serializeNodeConnection);


            //Create the Json settings:
            var jsonSettings = new JsonSerializerSettings()
            {
                ConstructorHandling = ConstructorHandling.AllowNonPublicDefaultConstructor,
                ReferenceLoopHandling = ReferenceLoopHandling.Ignore,
                Formatting = Formatting.Indented
            };

            //Generate a string and write it to the file:
            string text = JsonConvert.SerializeObject(json, jsonSettings);
            File.WriteAllText(targetFile, text);
        }


        public static void Deserialize(TransformationTree transformationTree, string targetFile)
        {
            //Read the singleton ClassManager.
            var classManager = Singleton<ClassManager>.Instance;

            //First clear old tree:
            foreach (var node in new List<Node>(transformationTree.Nodes)) if(!(node is OutputNode)) transformationTree.DesignTree.RemoveNode(node);
            foreach (var connection in new List<Connection>(transformationTree.Connections)) transformationTree.DesignTree.RemoveConnection(connection);

            //Generate the Serializer + lists:
            var nodeSerializer = new NodeSerializer();
            var treeDesigner = transformationTree.DesignTree;

            var newNodes = new List<Node>();
            var newConnections = new List<Connection>();

            var json = JObject.Parse(File.ReadAllText(targetFile));

            //Seems like this is the only way this is synthactically correct....
            //Read Nodes:
            foreach (dynamic obj in json["nodes"])
            {
                Node node = nodeSerializer.deserializeNode(obj, classManager);
                if (node != null) newNodes.Add(node);
            }

            //Read connections:
            foreach (dynamic obj in json["connections"])
            {
                Connection? connection = nodeSerializer.deserializeConnection(obj, newNodes);
                if (connection.HasValue)
                {
                    //Apply the connection:
                    newConnections.Add(connection.Value);
                }
                
            }

            //Then add new ones. FIRST THE NODES!
            foreach (var node in newNodes)
            {
                if(node is OutputNode)
                {
                    transformationTree.OutputNode.PosX = node.PosX;
                    transformationTree.OutputNode.PosY = node.PosY;
                    transformationTree.OutputNode.Name = node.Name;
                }
                else treeDesigner.AddNode(node);
            }

            //Apply the Connections at last.
            newConnections.ForEach(treeDesigner.AddConnection);
        }

    }
}
