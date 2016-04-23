namespace Common
{
	using System;
	using System.Linq;
	using System.Reflection;

	// TODO: (PS) Comment this.
	public class GenericActionBase
	{
		#region static methods

		public static void AddGenericEventHandler(object target, EventInfo eventInfo, Action<object[]> callback)
		{
			if (eventInfo == null)
			{
				throw new ArgumentNullException("eventInfo");
			}

			eventInfo.AddEventHandler(target, GetGenericEventHandler(eventInfo.EventHandlerType, callback));
		}

		public static Delegate GetGenericEventHandler(Type delegateType, Action<object[]> callback)
		{
			if (callback == null)
			{
				throw new ArgumentNullException("callback");
			}
			if (delegateType == null)
			{
				throw new ArgumentNullException("delegateType");
			}
			if (!typeof(Delegate).IsAssignableFrom(delegateType))
			{
				throw new ArgumentException("Given type is not a delegate.", "delegateType");
			}
			MethodInfo delegateInvoke = delegateType.GetMethod("Invoke");
			if (delegateInvoke.ReturnType != typeof(void))
			{
				throw new ArgumentException("Given delegate type must not return a value.", "delegateType");
			}

			Type[] argumentTypes = delegateInvoke.GetParameters().Select(p => p.ParameterType).ToArray();

			int argumentTypeCount = 0;
			string suffix = argumentTypes != null && (argumentTypeCount = argumentTypes.Count()) != 0 ?
				String.Concat('`', argumentTypeCount) :
				String.Empty;

			Type type = typeof(GenericActionBase).Assembly.GetType(typeof(GenericAction).FullName + suffix);
			if (type == null)
			{
				throw new InvalidOperationException("No GenericAction could be found for the given argument types.");
			}

			if (argumentTypeCount != 0)
			{
				type = type.MakeGenericType(argumentTypes);
			}

			return Delegate.CreateDelegate(delegateType, type.GetMethod("GetInstance").Invoke(null, new object[] { callback }), "Invoke");
		}

		public static void RemoveGenericEventHandler(object target, EventInfo eventInfo, Action<object[]> callback)
		{
			if (eventInfo == null)
			{
				throw new ArgumentNullException("eventInfo");
			}

			eventInfo.RemoveEventHandler(target, GetGenericEventHandler(eventInfo.EventHandlerType, callback));
		}

		#endregion static methods

		#region constructors

		protected GenericActionBase(Action<object[]> callback)
		{
			if (callback == null)
			{
				throw new ArgumentNullException("callback");
			}

			this.Callback = callback;
		}

		#endregion constructors

		#region properties

		public Action<object[]> Callback
		{
			get;
			set;
		}

		#endregion properties

		#region overrideable methods

		public override bool Equals(object obj)
		{
			GenericActionBase other = obj as GenericActionBase;

			return other != null && (object.ReferenceEquals(this, other) || (this.Callback.Equals(other.Callback)));
		}

		public override int GetHashCode()
		{
			return this.Callback.GetHashCode();
		}

		#endregion overrideable methods

		#region methods

		public void DynamicInvoke(params object[] parameters)
		{
			this.Callback(parameters);
		}

		#endregion methods
	}
}