using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SmaSTraDesigner.BusinessLogic
{
	public class NodeClassCategory
	{
		// TODO: (PS) Comment this.
		public string Name
		{
			get;
			private set;
		}

		public NodeClassCategory(string name)
		{
			if (String.IsNullOrWhiteSpace(name))
			{
				throw new ArgumentException("String argument 'name' must not be null or empty (incl. whitespace).", "name");
			}

			this.Name = name;
		}

		public override bool Equals(object obj)
		{
			DataType other = obj as DataType;
			if (other == null)
			{
				return false;
			}

			return String.Equals(this.Name, other.Name, StringComparison.InvariantCultureIgnoreCase);
		}

		public override int GetHashCode()
		{
			return this.Name.GetHashCode();
		}
	}
}
