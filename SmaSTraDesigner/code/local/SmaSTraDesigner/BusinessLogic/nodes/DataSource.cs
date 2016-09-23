using SmaSTraDesigner.BusinessLogic.classhandler.nodeclasses;

namespace SmaSTraDesigner.BusinessLogic
{
	/// <summary>
	/// Node that provides data and has no dynamic input (i.e. a hardware sensor or a web service).
	/// </summary>
	public class DataSource : Node
	{


        public DataSource(DataSourceNodeClass nodeClass)
        {
            this.Name = nodeClass.DisplayName;
            this.Class = nodeClass;
        }

    }
}