using SmaSTraDesigner.BusinessLogic.classhandler.nodeclasses;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

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
