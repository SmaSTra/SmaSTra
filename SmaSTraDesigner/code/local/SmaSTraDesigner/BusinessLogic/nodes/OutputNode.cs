namespace SmaSTraDesigner.BusinessLogic
{
	/// <summary>
	/// Represents a TransformationTree's data output.
	/// </summary>
	public class OutputNode : Node
	{


        public OutputNode()
        {
            this.Name = "OutputNode";
            this.NodeUUID = System.Guid.NewGuid().ToString();

            this.InputNodes = new Node[1];
        }

	}
}