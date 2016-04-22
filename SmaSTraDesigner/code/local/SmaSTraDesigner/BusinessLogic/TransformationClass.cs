using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SmaSTraDesigner.BusinessLogic
{
	public class TransformationClass : NodeClass
	{
		// TODO: (PS) Comment this.
		public DataType[] InputTypes
		{
			get;
			private set;
		}

		public TransformationClass(Node baseNode, DataType[] inputTypes, DataType outputType)
			: base(baseNode, outputType)
		{
			if (inputTypes == null)
			{
				throw new ArgumentNullException("inputTypes");
			}
			if (inputTypes.Length == 0)
			{
				throw new ArgumentException("Argument array 'inputTypes' must not be empty.", "inputTypes");
			}

			this.InputTypes = inputTypes;
		}
	}
}
