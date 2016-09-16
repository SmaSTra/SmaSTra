using SmaSTraDesigner.BusinessLogic.utils;
using System.Collections.Generic;
using System.Linq;


namespace SmaSTraDesigner.BusinessLogic.classhandler
{
    class CombinedNodeClass : NodeClass
    {

        #region Properties

        /// <summary>
        /// The Connections of the Sub-Elements.
        /// </summary>
        public List<SimpleConnection> Connections { private set; get; }

        /// <summary>
        /// The Sub-Elements Name -> Type
        /// </summary>
        public List<SimpleSubNode> SubElements { private set; get; }

        /// <summary>
        /// The Property for the Output-node.
        /// </summary>
        public string OutputNodeUUID { private set; get; }

        #endregion Properties

        #region Constructor

        public CombinedNodeClass(ClassManager.NodeType nodeType, string name, Node baseNode,
                List<SimpleSubNode> subElements, List<SimpleConnection> connections,
                DataType outputType, string outputNodeUUID, DataType[] inputTypes = null)
                    : base(nodeType, name, baseNode, outputType, inputTypes)
        {
            this.SubElements = subElements == null ? new List<SimpleSubNode>() : subElements;
            this.Connections = connections == null ? new List<SimpleConnection>() : connections;
            this.OutputNodeUUID = outputNodeUUID;

            this.BaseNode.Class = this;
        }

        #endregion Constructor

    }

    #region SubClasses
    public class SimpleSubNode
    {

        public SimpleSubNode()
        {
            this.Properties = new Dictionary<string, string>();
        }

        public SimpleSubNode(IDictionary<string, string> properties)
        {
            Properties = properties;
        }

        public SimpleSubNode(Node node, double centerX, double centerY) : this()
        {
            Properties.Add("NAME", node.Name);
            Properties.Add("UUID", node.NodeUUID);
            Properties.Add("TYPE", node.Class.Name);
            Properties.Add("POSX", (node.PosX - centerX).ToString());
            Properties.Add("POSY", (node.PosY - centerY).ToString());
        }

        /// <summary>
        /// Gets this node as a real node.
        /// </summary>
        /// <param name="classManager"></param>
        /// <returns></returns>
        public Node GetAsRealNode(ClassManager classManager)
        {
            string type = "";
            string name = "";
            string UUID = "";
            double posX = 0;
            double posY = 0;

            Properties.TryGetValue("TYPE", out type);
            Properties.TryGetValue("NAME", out name);
            Properties.TryGetValue("UUID", out UUID);
            Properties.TryGetValueAsDouble("POSX", out posX, 0);
            Properties.TryGetValueAsDouble("POSY", out posY, 0);

            //If no name -> give it the Type name:
            if(name == null || name.Count() == 0)
            {
                name = type;
            }

            //If no UUID -> Give it a new one!
            if(UUID == null || UUID.Count() == 0)
            {
                UUID = System.Guid.NewGuid().ToString();
            }

            Node node = classManager.GetNewNodeForType(type);
            //Set the node property if present:
            if (node != null)
            {
                node.Name = name;
                node.ForceUUID(UUID);
                node.PosX = posX;
                node.PosY = posY;
            }

            return node;
        }

        public IDictionary<string, string> Properties { private set; get; }
    }


    public class SimpleConnection
    {
        public SimpleConnection(string node1, string node2, int position)
        {
            this.firstNode = node1;
            this.secondNode = node2;
            this.position = position;
        }


        public string firstNode { private set; get; }
        public string secondNode { private set; get; }
        public int position { private set; get; }
    }

    #endregion SubClasses

}
