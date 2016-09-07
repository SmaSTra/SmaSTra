using Common;
using SmaSTraDesigner.BusinessLogic.classhandler;
using System.Collections.Generic;
using System.Linq;

namespace SmaSTraDesigner.BusinessLogic.nodes
{
    public class CombinedNode : Node
    {

        #region properties

        /// <summary>
        /// The nodes included in the Combined Nodes.
        /// </summary>
        public Node[] includedNodes{ get; private set; }

        /// <summary>
        /// The conenctions included in the Combined Node.
        /// </summary>
        public Connection[] includedConnections{ get; private set; }

        /// <summary>
        /// The output node to link.
        /// </summary>
        public Node outputNode { get; set; }

        /// <summary>
        /// The Input nodes to 
        /// </summary>
        public Node[] inputNodes { get; private set; }

        /// <summary>
        /// The dictionary from node -> Input.
        /// </summary>
        public Dictionary<Node,int> inputConnections { get; private set; }

        #endregion properties


        #region methods

        /// <summary>
        /// Called when the Class property changed its value.
        /// </summary>
        /// <param name="oldValue">The old value.</param>
        /// <param name="newValue">The new value.</param>
        protected override void OnClassChanged(NodeClass oldValue, NodeClass newValue)
        {
            if (newValue != null && oldValue != newValue)
            {
                //First update the Input nodes as Size:
                this.inputNodes = new Node[newValue.InputTypes.Count()];

                CombinedNodeClass ownClass = newValue as CombinedNodeClass;
                if(ownClass != null)
                {
                    //This means we are called from the Constructor of the Node class. We get a call with our data later on!
                    if (ownClass.Connections == null || ownClass.SubElements == null) return;

                    ClassManager classManager = Singleton<ClassManager>.Instance;
                    this.includedNodes = new Node[ownClass.SubElements.Count];
                    this.includedConnections = new Connection[ownClass.Connections.Count];
                    this.inputNodes = new Node[ownClass.Connections.Count(c => c.secondNode.StartsWith("input"))];
                    this.inputConnections = new Dictionary<Node, int>();

                    int i = 0;
                    //Generate the Nodes:
                    foreach (SimpleSubNode simpleNode in ownClass.SubElements)
                    {
                        this.includedNodes[i] = simpleNode.GetAsRealNode(classManager);
                        i++;
                    }

                    //Generate the Connections:
                    foreach(SimpleConnection connection in ownClass.Connections)
                    {
                        string first = connection.firstNode;
                        string second = connection.secondNode;
                        int index = connection.position;

                        //Search for the 2 nodes:
                        Node firstNode = this.includedNodes.FirstOrDefault(n => n.Name == first);
                        Node secondNode = this.includedNodes.FirstOrDefault(n => n.Name == second);

                        //We have an Input node:
                        if (second.StartsWith("input"))
                        {
                            this.inputConnections.Add(firstNode,index);
                            continue;
                        }

                        //We have an internal connection:
                        if(secondNode != null && firstNode != null)
                        {
                            Transformation transformation = secondNode as Transformation;
                            CombinedNode combined = secondNode as CombinedNode;

                            //We connect a Transformation:
                            if(transformation != null)
                            {
                                transformation.InputNodes[index] = secondNode;
                                continue;
                            }

                            //We connect a CombinedNode:
                            if (combined != null)
                            {
                                combined.inputNodes[index] = secondNode;
                                continue;
                            }
                        }
                    }
                }
            }
        }

        public override object Clone()
        {
            CombinedNode clonedNode = (CombinedNode)base.Clone();
            clonedNode.clazz = this.clazz;
            clonedNode.inputNodes = this.inputNodes.ToArray();
            return clonedNode;
        }

        #endregion methods

    }
}
