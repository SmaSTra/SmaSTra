namespace SmaSTraDesigner.BusinessLogic
{
    using System;
    using System.Linq;

    /// <summary>
    /// Stores information about a data type used by nodes for their inpot or output data.
    /// </summary>
    public class DataType
	{
		#region constructors

		/// <summary>
		/// Constructor
		/// </summary>
		/// <param name="name">Data type name (is used as a unique identifier)</param>
		public DataType(string name)
		{
            System.Diagnostics.Debug.Print("DataType created: " + name);
			if (String.IsNullOrWhiteSpace(name))
			{
				throw new ArgumentException("String argument 'name' must not be null or empty (incl. whitespace).", "name");
			}

			this.Name = name;
		}

		#endregion constructors

		#region properties

		/// <summary>
		/// Data type name (is used as a unique identifier)
		/// </summary>
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
			return String.Format("{0} {1}", this.GetType().Name, this.Name);
		}

		#endregion overrideable methods

		#region methods

		/// <summary>
		/// Checks whether this dataType can be implicitly converted into another.
		/// 
		/// NOT IMPLEMENTED YET!
		/// Currently analogous to Equals method.
		/// </summary>
		/// <param name="other">data type to check for implicit conversion.</param>
		/// <returns></returns>
		public bool CanConvertTo(DataType other)
		{
			// TODO: (PS) Implement this.
			return this.Equals(other);
		}

		#endregion methods
	}
}