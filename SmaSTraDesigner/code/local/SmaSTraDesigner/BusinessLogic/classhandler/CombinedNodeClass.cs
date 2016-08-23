using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

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

        #endregion Properties

        #region Constructor

        public CombinedNodeClass(ClassManager.NodeType nodeType, string name, Node baseNode,
                List<SimpleSubNode> subElements, List<SimpleConnection> connections,
                DataType outputType, DataType[] inputTypes = null)
                    : base(nodeType, name, baseNode, outputType, inputTypes)
        {
            this.SubElements = subElements == null ? new List<SimpleSubNode>() : subElements;
            this.Connections = connections == null ? new List<SimpleConnection>() : connections;
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

        public SimpleSubNode(Dictionary<string, string> properties)
        {
            Properties = properties;
        }

        public SimpleSubNode(Node node, double centerX, double centerY) : this()
        {
            Properties.Add("NAME", node.Name);
            Properties.Add("TYPE", node.Class.Name);
            Properties.Add("POSX", (node.PosX - centerX).ToString());
            Properties.Add("POSY", (node.PosY - centerY).ToString());
        }

        public Dictionary<string, string> Properties { private set; get; }
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
