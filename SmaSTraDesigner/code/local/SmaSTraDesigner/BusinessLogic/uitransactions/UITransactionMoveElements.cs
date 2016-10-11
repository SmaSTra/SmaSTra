
using SmaSTraDesigner.BusinessLogic.nodes;
using SmaSTraDesigner.Controls;
using SmaSTraDesigner.BusinessLogic.utils;

namespace SmaSTraDesigner.BusinessLogic.uitransactions
{
    public class UITransactionMoveElements : UITransaction
    {

        /// <summary>
        /// The moved nodes.
        /// </summary>
        private Node[] movedNodes;

        /// <summary>
        /// The x relatice change
        /// </summary>
        private double relateX = 0;


        /// <summary>
        /// The y relatice change
        /// </summary>
        private double relateY = 0;


        public UITransactionMoveElements(Node[] nodes, double changeX, double changeY)
        {
            this.movedNodes = nodes;
            this.relateX = changeX;
            this.relateY = changeY;
        }

        public void Redo(UcTreeDesigner designer)
        {
            movedNodes.ForEach(n => n.PosX += relateX);
            movedNodes.ForEach(n => n.PosY += relateY);
        }

        public void Undo(UcTreeDesigner designer)
        {
            movedNodes.ForEach(n => n.PosX -= relateX);
            movedNodes.ForEach(n => n.PosY -= relateY);
        }
    }
}
