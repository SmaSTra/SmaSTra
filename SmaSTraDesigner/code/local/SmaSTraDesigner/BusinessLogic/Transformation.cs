namespace SmaSTraDesigner.BusinessLogic
{
	using System;
	using System.Collections.Generic;
	using System.Collections.ObjectModel;
	using System.Linq;
	using System.Text;
	using System.Threading.Tasks;

	public class Transformation : Node
	{
		#region properties

		// TODO: (PS) Comment this.
		public Node[] InputNodes
		{
			get;
			private set;
		}

		#endregion properties

		#region overrideable methods

		protected override void OnClassChanged(NodeClass oldValue, NodeClass newValue)
		{
			base.OnClassChanged(oldValue, newValue);

			this.InputNodes = new Node[this.Class.InputTypes.Length];
		}

		#endregion overrideable methods

		#region methods

		private bool AddInput(Node inputNode)
		{
			if (this.InputNodes == null)
			{
				for (int i = 0; i < this.InputNodes.Length; i++)
				{
					if (this.InputNodes[i] == null)
					{
						this.InputNodes[i] = inputNode;

						return true;
					}
				}
			}

			return false;
		}

		#endregion methods
	}
}