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
        /// Constructor for multiple nodes.
        /// </summary>
        /// <param name="nodes">multiple nodes.</param>
        public UITransactionDeleteNodes(Node[] nodes)
        {
            this.nodes = nodes.ToArray();
        }


        /// <summary>
        /// Constructor for single node.
        /// </summary>
        /// <param name="node">node.</param>
        public UITransactionDeleteNodes(Node node)
            : this(new Node[] { node })
        {}


        public void Execute(UcTreeDesigner designer)
        {
            nodes.ForEach(n => designer.RemoveNode(n));
        }


        public void Undo(UcTreeDesigner designer)
        {
            nodes.ForEach(n => designer.AddNode(n));
        }
    }
}
