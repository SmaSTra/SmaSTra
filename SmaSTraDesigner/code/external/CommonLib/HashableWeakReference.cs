namespace Common
{
	using System;

	// TODO: (PS) Comment this.
	public class HashableWeakReference : WeakReference
	{
		#region fields

		private int hashCode;
		private object monitor = new object();

		#endregion fields

		#region constructors

		public HashableWeakReference(object target)
			: base(target)
		{
			this.hashCode = target.GetHashCode();
		}

		public HashableWeakReference(object target, bool trackResurrection)
			: base(target, trackResurrection)
		{
			this.hashCode = target.GetHashCode();
		}

		#endregion constructors

		#region overrideable properties

		/// <summary>
		/// Gets or sets the object (the target) referenced by the current <see cref="T:System.WeakReference"/> object.
		/// </summary>
		/// <value></value>
		/// <returns>null if the object referenced by the current <see cref="T:System.WeakReference"/> object has been garbage collected; otherwise, a reference to the object referenced by the current <see cref="T:System.WeakReference"/> object.</returns>
		/// <exception cref="T:System.InvalidOperationException">The reference to the target object is invalid. This exception can be thrown while setting this property if the value is a null reference or if the object has been finalized during the set operation.</exception>
		public override object Target
		{
			get
			{
				return base.Target;
			}
			set
			{
				lock (this.monitor)
				{
					base.Target = value;
					if (value != null)
					{
						this.hashCode = value.GetHashCode();
					}
				}
			}
		}

		#endregion overrideable properties

		#region overrideable methods

		public override bool Equals(object obj)
		{
			lock (this.monitor)
			{
				HashableWeakReference other = obj as HashableWeakReference;
				if (this.IsAlive)
				{
					if (other != null)
					{
						return other.IsAlive && object.Equals(this.Target, other.Target);
					}
					else
					{
						return object.Equals(this.Target, obj);
					}
				}
				else
				{
					return false;
				}
			}
		}

		public override int GetHashCode()
		{
			lock (this.monitor)
			{
				return this.hashCode;
			}
		}

		#endregion overrideable methods
	}
}