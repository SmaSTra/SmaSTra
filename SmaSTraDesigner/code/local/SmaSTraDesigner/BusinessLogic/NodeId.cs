using Common;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SmaSTraDesigner.BusinessLogic
{
	public struct NodeId
	{
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

		public NodeId(string className, ulong instanceId)
		{
			this.ClassName = className;
			this.InstanceId = instanceId;
		}

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
	}
}
