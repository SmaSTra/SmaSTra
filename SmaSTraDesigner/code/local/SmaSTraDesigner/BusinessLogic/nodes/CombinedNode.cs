using Common;
using SmaSTraDesigner.BusinessLogic.classhandler;
using SmaSTraDesigner.BusinessLogic.utils;
using System;
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
        /// The output node to link.
        /// </summary>
        public Node outputNode { get; set; }

        /// <summary>
        /// The dictionary from input index -> Node.
        /// </summary>
        public Dictionary<int, Tuple<Node,int>> inputConnections { get; private set; }

        #endregion properties


        #region methods

        /// <summary>
        /// Called when the Class property changed its value.
        /// </summary>
        /// <param name="oldValue">The old value.</param>
        /// <param name="newValue">The new value.</param>
        protected override void OnClassChanged(AbstractNodeClass oldValue, AbstractNodeClass newValue)
        {
            //Do not forget to call super to set DATA.
            base.OnClassChanged(oldValue, newValue);

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
                    this.inputConnections = new Dictionary<int,Tuple<Node,int>>();

                    int i = 0;
                    //Generate the Nodes:
                    foreach (SimpleSubNode simpleNode in ownClass.SubElements)
                    {
                        Node subNode = simpleNode.GetAsRealNode(classManager);
                        if (subNode == null)
                        {
                            //We have some types that are not registered?!?
                            throw new NullNodeException("Could not load Node: " + simpleNode.Properties.FirstOrDefault(n => n.Key == "Type").Value);
                        }

                        //We have our output node:
                        if(subNode != null && subNode.NodeUUID == ownClass.OutputNodeUUID)
                        {
                            this.outputNode = subNode;
                        }

                        this.includedNodes[i] = subNode;
                        i++;
                    }

                    //Generate the Connections:
                    foreach(SimpleConnection connection in ownClass.Connections)
                    {
                        string first = connection.firstNode;
                        string second = connection.secondNode;
                        int index = connection.position;

                        //Search for the 2 nodes:
                        Node firstNode = this.includedNodes.FirstOrDefault(n => n.NodeUUID == first);
                        Node secondNode = this.includedNodes.FirstOrDefault(n => n.NodeUUID == second);

                        //We have an Input node:
                        if (second.StartsWith("input"))
                        {
                            this.inputConnections.Add(Int32.Parse(second.Substring("input".Count())), new Tuple<Node,int>(firstNode,index));
                            continue;
                        }

                        //We have an internal connection:
                        if(secondNode != null && firstNode != null)
                        {
                            Transformation transformation = firstNode as Transformation;
                            CombinedNode combined = firstNode as CombinedNode;

                            //We connect a Transformation:
                            if(transformation != null)
                            {
                                transformation.SetInput(index, secondNode);
                                continue;
                            }

                            //We connect a CombinedNode:
                            if (combined != null)
                            {
                                combined.SetInput(index, secondNode);
                                continue;
                            }
                        }
                    }

                    //After building from base -> We unify the UUIDs, so they are now unique again!
                    includedNodes.ForEach(n => n.ForceUUID(Guid.NewGuid().ToString()));
                }
            }
        }


        public override object Clone()
        {
            CombinedNode clonedNode = (CombinedNode)base.Clone();
            //We call this for initing!
            clonedNode.OnClassChanged(null, this.clazz);
            return clonedNode;
        }


        public override void SetInput(int index, Node output)
        {
            base.SetInput(index, output);

            Tuple<Node,int> input = inputConnections.GetValue(index, null);
            if(input != null)
            {
                int internalIndex = input.Item2;
                input.Item1.SetInput(internalIndex, output);
            }
        }

        #endregion methods

    }
}
