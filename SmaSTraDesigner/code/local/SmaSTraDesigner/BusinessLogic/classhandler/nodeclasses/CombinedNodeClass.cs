using System.Collections.Generic;
using System.Linq;
using SmaSTraDesigner.BusinessLogic.nodes;
using SmaSTraDesigner.BusinessLogic.classhandler.nodeclasses;

namespace SmaSTraDesigner.BusinessLogic.classhandler
{
    class CombinedNodeClass : AbstractNodeClass
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

        public CombinedNodeClass(string name,
                string displayName, string description,
                List<SimpleSubNode> subElements, List<SimpleConnection> connections,
                DataType outputType, string outputNodeUUID, DataType[] inputTypes = null)
                    : base(ClassManager.NodeType.Combined, name, displayName, description, outputType, "", null, null, null, null, inputTypes)
        {
            this.SubElements = subElements == null ? new List<SimpleSubNode>() : subElements;
            this.Connections = connections == null ? new List<SimpleConnection>() : connections;
            this.OutputNodeUUID = outputNodeUUID;

            this.BaseNode.Class = this;
        }

        #endregion Constructor

        protected override Node generateBaseNode()
        {
            return new CombinedNode()
            {
                Name = this.DisplayName
            };
        }
    }

    #region SubClasses
    public class SimpleSubNode
    {

        public string Name { get; private set; }
        public string Uuid { get; private set; }
        public string Type { get; private set; }
        public double PosX { get; private set; }
        public double PosY { get; private set; }

        public IOData[] Data { get; set; }

        public DataConfigElement[] Configuration { get; set; }



        public SimpleSubNode(Node node, double centerX, double centerY)
        {
            this.Name = node.Name;
            this.Uuid = node.NodeUUID;
            this.Type = node.Class.Name;
            this.PosX = node.PosX - centerX;
            this.PosY = node.PosY - centerY;

            this.Data = node.InputIOData.Select(d => (IOData) d.Clone()).ToArray();
            this.Configuration = node.Configuration.Select(c => (DataConfigElement)c.Clone()).ToArray();
        }

        public SimpleSubNode(string name, string uuid, string type, double posX, double posY, 
            IOData[] data, DataConfigElement[] configuration)
        {
            this.Name = name;
            this.Uuid = uuid;
            this.Type = type;
            this.PosX = posX;
            this.PosY = posY;
            this.Data = data;
            this.Configuration = configuration;
        }

        /// <summary>
        /// Gets this node as a real node.
        /// </summary>
        /// <param name="classManager"></param>
        /// <returns></returns>
        public Node GetAsRealNode(ClassManager classManager)
        {
            //If no name -> give it the Type name:
            if(Name == null || Name.Count() == 0)
            {
                Name = Type;
            }

            //If no UUID -> Give it a new one!
            if(Uuid == null || Uuid.Count() == 0)
            {
                Uuid = System.Guid.NewGuid().ToString();
            }

            Node node = classManager.GetNewNodeForType(Type);
            //Set the node property if present:
            if (node != null)
            {
                node.Name = Name;
                node.ForceUUID(Uuid);
                node.PosX = PosX;
                node.PosY = PosY;

                //Apply the IOData:
                for(int i = 0; i < node.InputIOData.Count(); i++)
                {
                    node.InputIOData[i].Value = Data[i].Value;
                }

                //Apply the Configuration:
                for (int i = 0; i < node.Configuration.Count(); i++)
                {
                    node.Configuration[i].Value = Configuration[i].Value;
                }
            }

            return node;
        }
        
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
