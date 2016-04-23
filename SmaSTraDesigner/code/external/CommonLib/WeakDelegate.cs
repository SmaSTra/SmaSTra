namespace Common
{
	using System;
	using System.Reflection;

	/// <summary>
	/// Delegate that uses a weak reference to store the invocation target instance.
	/// </summary>
	public class WeakDelegate
	{
		#if DEBUG

		private ulong id = idCounter++;
		private static ulong idCounter = 0;

		#endif

		#region fields

		/// <summary>
		/// Hashcode for this delegate.
		/// </summary>
		private int hashCode;

		/// <summary>
		/// Stores the target instance as a strong reference in case it is supposed to be locked.
		/// </summary>
		private object lockedTarget = null;

		/// <summary>
		/// Mutex for locking and unlocking the target reference.
		/// </summary>
		private object monitor = new object();

		/// <summary>
		/// Weak reference to the invokation target.
		/// </summary>
		private WeakReference weakTargetReference = null;

		#endregion fields

		#region constructors

		/// <summary>
		/// Initializes a new instance of the <see cref="WeakDelegate"/> class.
		/// </summary>
		/// <param name="del">The delegate to wrap.</param>
		public WeakDelegate(Delegate del)
		{
			if (del == null)
			{
				throw new ArgumentNullException("del");
			}

			this.Method = del.Method;
			this.Target = del.Target;
			this.hashCode = HashCodeOperations.Combine(del.Target, del.Method);
		}

		#endregion constructors

		#region properties

		/// <summary>
		/// Gets a value indicating whether the target reference is alive.
		/// </summary>
		public bool IsTargetAlive
		{
			get { return this.weakTargetReference.IsAlive; }
		}

		/// <summary>
		/// Gets a value indicating whether the target reference is locked.
		/// </summary>
		public bool IsTargetLocked
		{
			get { return this.lockedTarget != null; }
		}

		/// <summary>
		/// Strength of the lock on the target reference.
		/// </summary>
		public int LockStrength
		{
			get;
			private set;
		}

		/// <summary>
		/// Gets the delegate method.
		/// </summary>
		public MethodInfo Method
		{
			get;
			private set;
		}

		/// <summary>
		/// Gets the weak reference to the invokation target.
		/// (Getter returns null if reference is no longer alive.)
		/// </summary>
		public object Target
		{
			get
			{
				if (this.weakTargetReference == null || !this.weakTargetReference.IsAlive)
				{
					return null;
				}
				else
				{
					return (object)this.weakTargetReference.Target;
				}
			}
			private set
			{
				if (value != null)
				{
					this.weakTargetReference = new WeakReference(value);
				}
				else
				{
					this.weakTargetReference = null;
				}
			}
		}

		#endregion properties

		#region overrideable methods

		/// <summary>
		/// Determines whether the specified <see cref="System.Object"/> is equal to this instance.
		/// </summary>
		/// <param name="obj">The <see cref="System.Object"/> to compare with this instance.</param>
		/// <returns>
		/// 	<c>true</c> if the specified <see cref="System.Object"/> is equal to this instance; otherwise, <c>false</c>.
		/// </returns>
		public override bool Equals(object obj)
		{
			WeakDelegate other = obj as WeakDelegate;
			Delegate del = obj as Delegate;

			if (this.Method.IsStatic)
			{
				if (other != null)
				{
					return other.Method == this.Method;
				}
				else if (del != null)
				{
					return del.Method == this.Method;
				}
				else
				{
					return false;
				}
			}
			else
			{
				if (this.LockTarget())
				{
					bool result;
					if (other != null)
					{
						result = other.LockTarget() &&
							object.Equals(other.Target, this.Target) &&
							other.Method == this.Method;

						other.UnlockTarget();
					}
					else if (del != null)
					{
						result = object.Equals(del.Target, this.Target) &&
							del.Method == this.Method;
					}
					else
					{
						result = false;
					}

					this.UnlockTarget();

					return result;
				}

				return false;
			}
		}

		/// <summary>
		/// Returns a hash code for this instance.
		/// </summary>
		/// <returns>
		/// A hash code for this instance, suitable for use in hashing algorithms and data structures like a hash table. 
		/// </returns>
		public override int GetHashCode()
		{
			return this.hashCode;
		}

		#endregion overrideable methods

		#region methods

		/// <summary>
		/// Dynamically invokes the delegate method with the specified parameters.
		/// </summary>
		/// <param name="parameters">The parameters.</param>
		/// <returns>The result of the delegate method.</returns>
		public object DynamicInvoke(params object[] parameters)
		{
			return this.InvokeMethod(parameters);
		}

		/// <summary>
		/// Dynamically invokes the delegate method with the specified parameters.
		/// </summary>
		/// <param name="targetDestroyed">Specifies whether the invokation target was allready distroyed.</param>
		/// <param name="parameters">The parameters.</param>
		/// <returns>The result of the delegate method.</returns>
		public object DynamicInvoke(out bool targetDestroyed, params object[] parameters)
		{
			object target = this.Target;
			targetDestroyed = !this.Method.IsStatic && target == null;

			return this.InvokeMethod(parameters);
		}

		/// <summary>
		/// Locks the invokation target reference, preventing it from being destroyed.
		/// </summary>
		/// <returns>A value specifying whether the lock action was successful.</returns>
		public bool LockTarget()
		{
			if (this.Method.IsStatic)
			{
				return true;
			}
			else
			{
				lock (this.monitor)
				{
					bool result = (this.lockedTarget = this.Target) != null;
					if (result)
					{
						this.LockStrength++;
					}

					return result;
				}
			}
		}

		/// <summary>
		/// Unlocks the target reference making it collectable by the garbage collection.
		/// </summary>
		public void UnlockTarget()
		{
			if (!this.Method.IsStatic)
			{
				lock (this.monitor)
				{
					if (--this.LockStrength <= 0)
					{
						this.LockStrength = 0;
						this.lockedTarget = null;
					}
				}
			}
		}

		/// <summary>
		/// Invokes the delegate method with the specified parameters.
		/// </summary>
		/// <param name="parameters">The parameters.</param>
		/// <returns>The result of the delegate method.</returns>
		protected object InvokeMethod(object[] parameters)
		{
			object target = this.Target;
			if (target != null)
			{
				return this.Method.Invoke(target, parameters);
			}

			return null;
		}

		#endregion methods
	}
}