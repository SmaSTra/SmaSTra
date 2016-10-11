using System.Collections.Generic;
using System.Linq;
using SmaSTraDesigner.BusinessLogic.nodes;

namespace SmaSTraDesigner.BusinessLogic.classhandler.nodeclasses
{
    public class CombinedNodeClass : AbstractNodeClass
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
        public string OutputNodeUuid { private set; get; }

        #endregion Properties

        #region Constructor

        public CombinedNodeClass(string name,
                string displayName, string description, string creator,
                List<SimpleSubNode> subElements, List<SimpleConnection> connections,
                DataType outputType, string outputNodeUuid, bool userCreated, string nodePath,
                DataType[] inputTypes = null)
                    : base(ClassManager.NodeType.Combined, name, displayName, description, creator, outputType, "", null, null, null, null, inputTypes, userCreated, nodePath)
        {
            this.SubElements = subElements ?? new List<SimpleSubNode>();
            this.Connections = connections ?? new List<SimpleConnection>();
            this.OutputNodeUuid = outputNodeUuid;
        }

        #endregion Constructor

        public override Node GenerateNode()
        {
            return new CombinedNode(this);
        }
    }

    #region SubClasses
    public class SimpleSubNode
    {

        public string Name { get; private set; }
        public string Uuid { get; private set; }
        public string Type { get; }
        public double PosX { get; }
        public double PosY { get; }

        public IOData[] Data { get; }

        public DataConfigElement[] Configuration { get; }



        public SimpleSubNode(Node node, double centerX, double centerY)
        {
            this.Name = node.Name;
            this.Uuid = node.NodeUUID;
            this.Type = node.Class.Name;
            this.PosX = node.PosX - centerX;
            this.PosY = node.PosY - centerY;

            this.Data = node.InputIOData.Select(d => (IOData) d.Clone()).ToArray();
            this.Configuration = node.Configuration.Select(c => c.Clone()).ToArray();
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
            if(string.IsNullOrEmpty(Name))
            {
                Name = Type;
            }

            //If no UUID -> Give it a new one!
            if(string.IsNullOrEmpty(Uuid))
            {
                Uuid = System.Guid.NewGuid().ToString();
            }

            var node = classManager.GetNewNodeForType(Type);
            if (node == null) return null;

            //Set the node property if present:
            node.Name = Name;
            node.ForceUUID(Uuid);
            node.PosX = PosX;
            node.PosY = PosY;

            //Apply the IOData:
            for(var i = 0; i < node.InputIOData.Count; i++)
            {
                node.InputIOData[i].Value = Data[i].Value;
            }

            //Apply the Configuration:
            for (var i = 0; i < node.Configuration.Count; i++)
            {
                node.Configuration[i].Value = Configuration[i].Value;
            }

            return node;
        }
        
    }


    public class SimpleConnection
    {
        public SimpleConnection(string node1, string node2, int position)
        {
            this.FirstNode = node1;
            this.SecondNode = node2;
            this.Position = position;
        }


        public string FirstNode { private set; get; }
        public string SecondNode { private set; get; }
        public int Position { private set; get; }
    }

    #endregion SubClasses

}
