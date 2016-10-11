using SmaSTraDesigner.BusinessLogic.classhandler.nodeclasses;

namespace SmaSTraDesigner.BusinessLogic.nodes
{
    /// <summary>
    /// Represents a transformation that combines data inputs to another output value.
    /// </summary>
    public class Transformation : Node
	{

        public Transformation(TransformationNodeClass nodeClass)
        {
            this.Name = nodeClass.DisplayName;
            this.Class = nodeClass;
        }

    }
}