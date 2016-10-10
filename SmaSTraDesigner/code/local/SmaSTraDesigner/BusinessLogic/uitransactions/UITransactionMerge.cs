using System.Collections.Generic;
using System.Linq;
using SmaSTraDesigner.BusinessLogic.nodes;
using SmaSTraDesigner.BusinessLogic.utils;
using SmaSTraDesigner.Controls;

namespace SmaSTraDesigner.BusinessLogic.uitransactions
{
    public class UITransactionMerge : UITransaction
    {

        /// <summary>
        /// The merged node.
        /// </summary>
        private readonly CombinedNode _mergedNode;

        /// <summary>
        /// The Connections to apply on a redo.
        /// </summary>
        private readonly Connection[] _oldConnections;

        /// <summary>
        /// The list of connections to use for Redoing.
        /// </summary>
        private readonly List<Connection> _connectionsAfterUnmerge = new List<Connection>();


        public UITransactionMerge(CombinedNode mergedNode, Connection[] oldConnections)
        {
            this._mergedNode = mergedNode;
            this._oldConnections = oldConnections;
        }

        public void Redo(UcTreeDesigner designer)
        {
            //Reset the Positions:
            _mergedNode.includedNodes
                .ForEach(n => n.PosX -= _mergedNode.PosX)
                .ForEach(n => n.PosY -= _mergedNode.PosY)
                .ForEach(n => designer.RemoveNode(n));

            //Re-Apply the Inputs:
            foreach (var node in _mergedNode.includedNodes)
            {
                _connectionsAfterUnmerge
                    .Where(c => c.InputNode == node)
                    .ForEach(c => node.SetInput(c.InputIndex, c.OutputNode));
            }

            //Finally Readd the node:
            designer.AddNode(_mergedNode);

            //Readd connections from outside the Bundle:
            _oldConnections.ForEach(designer.AddConnection);
        }

        public void Undo(UcTreeDesigner designer)
        {
            //Save the internal Connections for After the unmerge.
            _connectionsAfterUnmerge.Clear();
            _mergedNode.includedNodes
                .ForEach(
                    n => n.InputNodes.ForEachNonNull((n2, i) => _connectionsAfterUnmerge.Add(new Connection(n2,n,i)))
                );

            //Then do the unmerge
            designer.UnmergeNode(_mergedNode);
        }
    }
}
