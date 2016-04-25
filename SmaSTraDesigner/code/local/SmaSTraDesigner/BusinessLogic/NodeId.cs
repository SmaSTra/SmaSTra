namespace SmaSTraDesigner.BusinessLogic
{
	using System;
	using System.Collections.Generic;
	using System.Linq;
	using System.Text;
	using System.Threading.Tasks;

	using Common;

	public struct NodeId
	{
		#region constructors

		public NodeId(string className, ulong instanceId)
		{
			this.ClassName = className;
			this.InstanceId = instanceId;
		}

		#endregion constructors

		#region properties

		// TODO: (PS) Comment this.
		public string ClassName
		{
			get;
			set;
		}

		// TODO: (PS) Comment this.
		public ulong InstanceId
		{
			get;
			set;
		}

		#endregion properties

		#region overrideable methods

		public override bool Equals(object obj)
		{
			NodeId? other = obj as NodeId?;

			return other != null &&
				String.Equals(other.Value.ClassName, this.ClassName) &&
				other.Value.InstanceId == this.InstanceId;
		}

		public override int GetHashCode()
		{
			return HashCodeOperations.Combine(this.ClassName, this.InstanceId);
		}

		#endregion overrideable methods
	}
}