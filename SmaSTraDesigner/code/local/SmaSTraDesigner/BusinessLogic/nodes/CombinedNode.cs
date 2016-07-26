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

        #endregion properties


        #region methods

        /// <summary>
        /// Called when the Class property changed its value.
        /// </summary>
        /// <param name="oldValue">The old value.</param>
        /// <param name="newValue">The new value.</param>
        protected override void OnClassChanged(NodeClass oldValue, NodeClass newValue)
        {
            if(newValue != null && oldValue != newValue) this.inputNodes = new Node[newValue.InputTypes.Count()];
        }

        #endregion methods

    }
}
