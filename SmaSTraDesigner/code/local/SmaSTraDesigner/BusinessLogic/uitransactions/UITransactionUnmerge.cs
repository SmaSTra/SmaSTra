using System;
using SmaSTraDesigner.BusinessLogic.nodes;
using SmaSTraDesigner.Controls;
using SmaSTraDesigner.BusinessLogic.utils;

namespace SmaSTraDesigner.BusinessLogic.uitransactions
{
    public class UITransactionUnmerge : UITransaction
    {

        /// <summary>
        /// The merged node.
        /// </summary>
        private CombinedNode mergedNode;

        /// <summary>
        /// The connections of the Merged Node.
        /// </summary>
        private Connection[] newConnections;

        /// <summary>
        /// The connections of the Merged Node.
        /// </summary>
        private Connection[] oldConnections;


        public UITransactionUnmerge(CombinedNode mergedNode, Connection[] newConnections, Connection[] oldConnections)
        {
            this.mergedNode = mergedNode;
            this.newConnections = newConnections;
            this.oldConnections = oldConnections;
        }

        public void Redo(UcTreeDesigner designer)
        {
            designer.UnmergeNode(mergedNode);
        }

        public void Undo(UcTreeDesigner designer)
        {
            mergedNode.includedNodes.ForEach(n => designer.RemoveNode(n));
            designer.AddNode(mergedNode);

            oldConnections.ForEach(designer.AddConnection);
        }
    }
}
