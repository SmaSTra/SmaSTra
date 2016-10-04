using System.Linq;
using SmaSTraDesigner.Controls;
using SmaSTraDesigner.BusinessLogic.utils;

namespace SmaSTraDesigner.BusinessLogic.uitransactions
{
    class UITransactionDeleteNodes : UITransaction
    {

        /// <summary>
        /// The Nodes to add.
        /// </summary>
        private Node[] nodes;

        /// <summary>
        /// This are the connections Removed.
        /// </summary>
        private Connection[] connections;


        /// <summary>
        /// Constructor for multiple nodes.
        /// </summary>
        /// <param name="nodes">multiple nodes.</param>
        public UITransactionDeleteNodes(Node[] nodes, Connection[] connections)
        {
            this.nodes = nodes.ToArray();
            this.connections = connections == null ? new Connection[0] : connections.ToArray();
        }


        /// <summary>
        /// Constructor for single node.
        /// </summary>
        /// <param name="node">node.</param>
        public UITransactionDeleteNodes(Node node, Connection[] connections = null)
            : this(new Node[] { node }, connections)
        {}


        public void Redo(UcTreeDesigner designer)
        {
            nodes.ForEach(n => designer.RemoveNode(n));
        }


        public void Undo(UcTreeDesigner designer)
        {
            nodes.ForEach(n => designer.AddNode(n));
            connections.ForEach(c => designer.AddConnection(c));
        }
    }
}
