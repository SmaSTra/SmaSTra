using System;
using SmaSTraDesigner.BusinessLogic.nodes;
using SmaSTraDesigner.Controls;

namespace SmaSTraDesigner.BusinessLogic.uitransactions
{
    public class UITransactionMerge : UITransaction
    {

        /// <summary>
        /// The merged node.
        /// </summary>
        private CombinedNode mergedNode;

        /// <summary>
        /// The connections of the Merged Node.
        /// </summary>
        private Connection[] oldConnections;

        /// <summary>
        /// The new Connections to apply.
        /// </summary>
        private Connection[] newConnections;


        public UITransactionMerge(CombinedNode mergedNode, Connection[] oldConnections, Connection[] newConnections)
        {
            this.mergedNode = mergedNode;
            this.oldConnections = oldConnections;
            this.newConnections = newConnections;
        }

        public void Redo(UcTreeDesigner designer)
        {
            //TODO implement me!
        }

        public void Undo(UcTreeDesigner designer)
        {
            designer.UnmergeNode(mergedNode);
        }
    }
}
