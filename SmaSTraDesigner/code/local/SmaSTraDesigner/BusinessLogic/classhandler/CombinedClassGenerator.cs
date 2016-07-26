using SmaSTraDesigner.BusinessLogic.nodes;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
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
            if(node != null) this.nodes.Add(node);
        }

        /// <summary>
        /// This adds all Nodes to the node generation list.
        /// </summary>
        /// <param name="nodes">to add</param>
        public void AddNodes(IEnumerable<Node> nodes)
        {
            if(nodes != null && nodes.Any()) this.nodes.AddRange(nodes);
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
        public NodeClass GenerateClass()
        {
            Node root = GetRootNode();
            if (root == null) return null;

            List<DataType> inputs = new List<DataType>();
            foreach( Node node in nodes)
            {
                NodeClass nodeClass = node.Class;
                Node[] nodeInputs = GetInputsOfNode(node);
                for(int i = 0; i < nodeInputs.Count(); i++)
                {
                    Node subNode = nodeInputs[i];
                    if(subNode == null || !nodes.Contains(subNode)) inputs.Add(nodeClass.InputTypes[i]);
                }
            }

            //Generate the BaseNode:
            DataType output = root.Class.OutputType;
            CombinedNode baseNode = new CombinedNode();
            baseNode.Name = Name;

            //Finally generate the NodeClass
            NodeClass finalNodeClass = new NodeClass(NodeType.Combined, Name, baseNode, output, inputs.ToArray());
            finalNodeClass.Description = Description;

            return finalNodeClass;
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
