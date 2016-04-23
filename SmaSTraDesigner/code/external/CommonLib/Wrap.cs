namespace Common
{
	// TODO: (PS) Comment this.
	public class Wrap<T>
	{
		#region static methods

		public static implicit operator Wrap<T>(T value)
		{
			return new Wrap<T>(value);
		}

		public static implicit operator T(Wrap<T> value)
		{
			return value.Value;
		}

		#endregion static methods

		#region constructors

		public Wrap()
			: this(default(T))
		{
		}

		public Wrap(T value)
		{
			this.Value = value;
		}

		#endregion constructors

		#region properties

		public T Value
		{
			get;
			set;
		}

		#endregion properties

		#region overrideable methods

		public override bool Equals(object obj)
		{
			if (obj is T)
			{
				return object.Equals(this.Value, (T)obj);
			}
			else
			{
				Wrap<T> other = obj as Wrap<T>;

				return other != null && (object.ReferenceEquals(this, other) || object.Equals(this.Value, other.Value));
			}
		}

		public override int GetHashCode()
		{
			return HashCodeOperations.Combine(typeof(Wrap<T>), this.Value);
		}

		#endregion overrideable methods
	}

	public class Wrap : Wrap<object>
	{
		#region constructors

		public Wrap()
			: this(null)
		{
		}

		public Wrap(object value)
			: base(value)
		{
		}

		#endregion constructors
	}
}