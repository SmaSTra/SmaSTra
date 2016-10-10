using System;
using System.Collections.Generic;
using System.Linq;
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
        private readonly CombinedNode _mergedNode;

        /// <summary>
        /// The new connections that are added after Unmerge.
        /// </summary>
        private readonly Connection[] _newConnections;


        public UITransactionUnmerge(CombinedNode mergedNode, Connection[] newConnections)
        {
            this._mergedNode = mergedNode;
            this._newConnections = newConnections;
        }
       


        public void Redo(UcTreeDesigner designer)
        {
            designer.UnmergeNode(_mergedNode);
        }

        public void Undo(UcTreeDesigner designer)
        {
            //Reset the Positions:
            _mergedNode.includedNodes
                .ForEach(n => n.PosX -= _mergedNode.PosX)
                .ForEach(n => n.PosY -= _mergedNode.PosY)
                .ForEach(n => designer.RemoveNode(n));

            //Re-Apply the Inputs:
            foreach (var node in _mergedNode.includedNodes)
            {
                _newConnections
                    .Where(c => c.InputNode == node)
                    .ForEach(c => node.SetInput(c.InputIndex, c.OutputNode));
            }

            //Finally Readd the node:
            designer.AddNode(_mergedNode);

            //Reapply all connections outside of the Node:
            _newConnections
                .Where(c => !_mergedNode.includedNodes.Contains(c.InputNode)) 
                .Select(c => new Connection(_mergedNode, c.InputNode, c.InputIndex))
                .ForEach(designer.AddConnection);
            _newConnections
                .Where(c => !_mergedNode.includedNodes.Contains(c.OutputNode))
                .Select(c => new Connection(c.OutputNode, _mergedNode, c.InputIndex))
                .ForEach(designer.AddConnection);

        }
    }
}
