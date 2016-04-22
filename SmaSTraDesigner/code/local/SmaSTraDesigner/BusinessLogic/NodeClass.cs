using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SmaSTraDesigner.BusinessLogic
{
	public class NodeClass
	{
		public NodeClass(Node baseNode, DataType outputType)
		{
			if (baseNode == null)
			{
				throw new ArgumentNullException("baseNode");
			}
			
			this.BaseNode = baseNode;
			baseNode.Class = this;
			this.OutputType = outputType;
		}

		// TODO: (PS) Comment this.
		public DataType OutputType
		{
			get;
			private set;
		}

		// TODO: (PS) Comment this.
		public Node BaseNode
		{
			get;
			private set;
		}
	}
}
