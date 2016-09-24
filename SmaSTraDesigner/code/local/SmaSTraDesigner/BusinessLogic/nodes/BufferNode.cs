using SmaSTraDesigner.BusinessLogic.classhandler.nodeclasses;

namespace SmaSTraDesigner.BusinessLogic.nodes
{
    public class BufferNode : Node
    {


        #region constructor

        public BufferNode(BufferNodeClass nodeClass)
        {
            this.Name = nodeClass.DisplayName;
            this.Class = nodeClass;
        }

        #endregion constructor

    }
}
