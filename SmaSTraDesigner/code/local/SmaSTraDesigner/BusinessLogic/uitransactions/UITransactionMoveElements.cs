using System.Linq;
using SmaSTraDesigner.BusinessLogic.nodes;
using SmaSTraDesigner.BusinessLogic.utils;
using SmaSTraDesigner.Controls;

namespace SmaSTraDesigner.BusinessLogic.uitransactions
{
    public class UiTransactionMoveElements : UITransaction
    {

        /// <summary>
        /// The Old positions to save.
        /// </summary>
        private readonly NodePositionSave[] _oldPos;

        /// <summary>
        /// The new Position to use.
        /// </summary>
        private readonly NodePositionSave[] _newPos;


        public UiTransactionMoveElements(NodePositionSave[] oldPos, NodePositionSave[] newPos)
        {
            this._oldPos = oldPos;
            this._newPos = newPos;
        }


        public void Redo(UcTreeDesigner designer)
        {
            _newPos?.ForEach(n => n.Reset());
        }

        public void Undo(UcTreeDesigner designer)
        {
            _oldPos?.ForEach(n => n.Reset());
        }
    }


    public class MoveElementsBuilder
    {

        /// <summary>
        /// The Nodes to read from.
        /// </summary>
        private readonly Node[] _nodes;

        /// <summary>
        /// The Old positions to use.
        /// </summary>
        private NodePositionSave[] _oldNodePos;

        /// <summary>
        /// The New node positions.
        /// </summary>
        private NodePositionSave[] _newNodePos;


        public MoveElementsBuilder(Node[] nodes)
        {
            this._nodes = nodes;
        }

        /// <summary>
        /// Saves the old states.
        /// </summary>
        public void SaveOld()
        {
            _oldNodePos = _nodes
                .Select(n => new NodePositionSave(n))
                .ToArray();
        }

        /// <summary>
        /// Saves the new States.
        /// </summary>
        public void SaveNew()
        {
            _newNodePos = _nodes
                .Select(n => new NodePositionSave(n))
                .ToArray();
        }

        /// <summary>
        /// Builds the Transaction to apply.
        /// </summary>
        /// <returns>The build transaction</returns>
        public UiTransactionMoveElements Build()
        {
            return new UiTransactionMoveElements(_oldNodePos, _newNodePos);
        }

    }


    public class NodePositionSave
    {

        /// <summary>
        /// The old node to move.
        /// </summary>
        private readonly Node _node;

        /// <summary>
        /// The old position to reset.
        /// </summary>
        private readonly double _oldX;

        /// <summary>
        /// The old position to reset.
        /// </summary>
        private readonly double _oldY;

        public NodePositionSave(Node node)
        {
            this._node = node;
            this._oldX = node.PosX;
            this._oldY = node.PosY;
        }

        public NodePositionSave(Node node, double oldX, double oldY)
        {
            this._node = node;
            this._oldX = oldX;
            this._oldY = oldY;
        }

        /// <summary>
        /// Resets the position
        /// </summary>
        public void Reset()
        {
            _node.PosX = _oldX;
            _node.PosY = _oldY;
        }

    }
}
