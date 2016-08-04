using Newtonsoft.Json;
using SmaSTraDesigner.BusinessLogic.nodes;
using System;
using System.Collections.Generic;
using System.Dynamic;
using System.IO;
using System.Json;
using System.Linq;
using static SmaSTraDesigner.BusinessLogic.ClassManager;

namespace SmaSTraDesigner.BusinessLogic.classhandler
{
    class CombinedClassGenerator
    {


        #region StaticMethods


        private static bool SubTreeContains(Node root, List<Node> nodes)
        {
            //Remove the first element:
            if (root != null) nodes.Remove(root);

            //If empty -> Everything is okay!
            if (!nodes.Any()) return true;

            //Check recursivcely:
            foreach (Node input in GetInputsOfNode(root))
            {
                if (SubTreeContains(input, nodes)) return true;
            }

            return false;
        }


        private static Node[] GetInputsOfNode(Node node)
        {
            if (node == null) return new Node[0];
            if (node is DataSource) return new Node[0];

            if (node is OutputNode)
            {
                OutputNode outNode = (OutputNode)node;
                if (outNode.InputNode != null) return new Node[] { outNode.InputNode };
                return new Node[0];
            }

            if (node is Transformation)
            {
                Transformation outNode = (Transformation)node;
                return outNode.InputNodes;
            }

            return new Node[0];
        }

        #endregion StaticMethods

        #region fields

        /// <summary>
        /// The Nodes to use for generation.
        /// </summary>
        private List<Node> nodes = new List<Node>();

        /// <summary>
        /// The Cached node class to not having to rebuild it all the time when calling the getter.
        /// </summary>
        private CombinedNodeClass cachedNodeClass = null;

        #endregion fields

        #region properties

        /// <summary>
        /// The Name to set.
        /// </summary>
        public String Name { get; private set; }

        /// <summary>
        /// The Description to set.
        /// </summary>
        public String Description { get; set; }


        #endregion properties

        #region constructors

        public CombinedClassGenerator()
        {
            this.Name = "TMP" + new Random().Next();
        }

        public CombinedClassGenerator(String name)
        {
            this.Name = name;
        }

        public CombinedClassGenerator(String name, IEnumerable<Node> nodes) : this(name)
        {
            this.AddNodes(nodes);
        }

        public CombinedClassGenerator(IEnumerable<Node> nodes) : this()
        {
            this.AddNodes(nodes);
        }

        #endregion constructors

        #region Methods

        /// <summary>
        /// This adds a Node to the node generation list.
        /// </summary>
        /// <param name="node">to add</param>
        public void AddNode(Node node)
        {
            if (node != null)
            {
                this.nodes.Add(node);
                this.cachedNodeClass = null;
            }
        }

        /// <summary>
        /// This adds all Nodes to the node generation list.
        /// </summary>
        /// <param name="nodes">to add</param>
        public void AddNodes(IEnumerable<Node> nodes)
        {
            if (nodes != null && nodes.Any())
            {
                this.nodes.AddRange(nodes);
                this.cachedNodeClass = null;
            }
        }

        /// <summary>
        /// Checks if the Nodes are connected.
        /// </summary>
        /// <returns>true if connected</returns>
        public bool IsConnected()
        {
            return GetRootNode() != null;
        }


        /// <summary>
        /// Generates the NodeClass.
        /// </summary>
        /// <returns>The node class or null if not possible.</returns>
        public CombinedNodeClass GenerateClass()
        {
            if (cachedNodeClass != null) return cachedNodeClass;

            Node root = GetRootNode();
            if (root == null) return null;

            //Generate Inputs + Sub-Hirachy.
            List<DataType> inputs = new List<DataType>();
            List<SimpleConnection> connections = new List<SimpleConnection>();
            List<SimpleSubNode> subNodes = new List<SimpleSubNode>();
            int input = 0;

            foreach ( Node node in nodes)
            {
                NodeClass nodeClass = node.Class;
                Node[] nodeInputs = GetInputsOfNode(node);

                subNodes.Add(new SimpleSubNode(node));
                for(int i = 0; i < nodeInputs.Count(); i++)
                {
                    Node subNode = nodeInputs[i];
                    if (subNode == null || !nodes.Contains(subNode))
                    {
                        inputs.Add(nodeClass.InputTypes[i]);
                        connections.Add(new SimpleConnection(node.Name, "input"+input, i));
                        input++;
                    }else{
                        connections.Add(new SimpleConnection(node.Name, subNode.Name, i));
                    }
                }
            }

            //Generate the BaseNode:
            DataType output = root.Class.OutputType;
            CombinedNode baseNode = new CombinedNode();
            baseNode.Name = Name;

            //Finally generate the NodeClass
            CombinedNodeClass finalNodeClass = new CombinedNodeClass(NodeType.Combined, Name, baseNode, subNodes, connections, output, inputs.ToArray());
            finalNodeClass.Description = Description;
            finalNodeClass.DisplayName = Name;

            this.cachedNodeClass = finalNodeClass;
            return finalNodeClass;
        }


        /// <summary>
        /// Saves the Current state to the disc.
        /// </summary>
        public bool saveToDisc()
        {
            CombinedNodeClass toSave = GenerateClass();
            if (toSave == null) return false;

            string savePath = Path.Combine(Environment.CurrentDirectory, "created");
            savePath = Path.Combine(savePath, toSave.DisplayName);

            if (Directory.Exists(savePath)) return false;
            Directory.CreateDirectory(savePath);

            string metaFile = Path.Combine(savePath, "metadata.json");

            //Combine the JSON:
            dynamic json = new ExpandoObject();
            json.description = toSave.Description;
            json.display = toSave.DisplayName;
            json.output = toSave.OutputType.Name;
            json.type = "combined";

            //Generate the inputs:
            JsonObject inputs = new JsonObject();
            for (int i = 0; i < toSave.InputTypes.Count(); i++) inputs.Add("arg" + i, new JsonPrimitive(toSave.InputTypes[i].Name));

            //Generate the Connections:
            dynamic[] connections = new dynamic[toSave.Connections.Count()];
            for (int i = 0; i < connections.Count(); i++) connections[i] = toSave.Connections.ElementAt(i);
            json.connections = connections;

            //Generate the Nodes:
            dynamic[] nodes = new dynamic[toSave.SubElements.Count()];
            for (int i = 0; i < toSave.SubElements.Count(); i++) nodes[i] = toSave.SubElements.ElementAt(i);
            json.subElements = nodes;



            //Create the Json settings:
            var jsonSettings = new JsonSerializerSettings()
            {
                ConstructorHandling = ConstructorHandling.AllowNonPublicDefaultConstructor,
                ReferenceLoopHandling = ReferenceLoopHandling.Ignore,
                Formatting = Newtonsoft.Json.Formatting.Indented
            };

            //Generate a string and write it to the file:
            string text = JsonConvert.SerializeObject(json, jsonSettings);
            File.WriteAllText(metaFile, text);

            return true;
        }


        /// <summary>
        /// Gets the Root node of the Generated class.
        /// </summary>
        /// <returns>the Root node or null</returns>
        private Node GetRootNode()
        {
            foreach (Node node in nodes) if (SubTreeContains(node, nodes.ToList())) return node;
            return null;
        }

        #endregion Methods

    }
}
