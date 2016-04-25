namespace SmaSTraDesigner.BusinessLogic
{
	using System;
	using System.Collections.Generic;
	using System.Linq;
	using System.Text;
	using System.Threading.Tasks;

	public class DataType
	{
		#region static fields

		public static DataType Number = new DataType("Number");
		public static DataType Text = new DataType("Text");

		#endregion static fields

		#region constructors

		public DataType(string name)
		{
			if (String.IsNullOrWhiteSpace(name))
			{
				throw new ArgumentException("String argument 'name' must not be null or empty (incl. whitespace).", "name");
			}

			this.Name = name;
		}

		#endregion constructors

		#region properties

		// TODO: (PS) Comment this.
		public string Name
		{
			get;
			private set;
		}

		#endregion properties

		#region overrideable methods

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

		public override string ToString()
		{
			return String.Format("{{{0} {1}}}", this.GetType().Name, this.Name);
		}

		#endregion overrideable methods
	}
}