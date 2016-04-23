namespace Common.Collections
{
	using System;
	using System.Collections;
	using System.Collections.Generic;
	using System.Globalization;
	using System.Linq;
	using System.Runtime.InteropServices;

	using Common.ExtensionMethods;

	#if MACRO

	using CodeGeneration;
	using ICSharpCode.NRefactory.Ast;
	using MacroLib;
	using MacroLib.Attributes;

	class Macro
	{
		#region static methods

		[DllImport("user32.dll", CharSet = CharSet.Unicode)]
		public static extern int MessageBox(IntPtr hWnd, String text, String caption, uint type);

		public static void MessageBox(object text = null)
		{
			MessageBox(IntPtr.Zero, text != null ? text.ToString() : String.Empty, null, 0);
		}

		#endregion static methods

		#region methods

		[Execute]
		void ExecuteMacro(MacroOutput output, CodeConstruct sourceFileCode, string sourceFilePath, string defaultNamespace, int orderNo)
		{
			var types = new Type[] { typeof(sbyte), typeof(short), typeof(ushort), typeof(int), typeof(uint), typeof(long), typeof(ulong), typeof(char),
				typeof(float), typeof(double), typeof(decimal), typeof(bool) };
			output.CodeConstruct.CompilationUnit.AddUsings(sourceFileCode.CompilationUnit.GetUsings());

			var ns = sourceFileCode.Clone().CompilationUnit.Down().OfType<NamespaceDeclaration>().First();
			ns.Children.Clear();

			output.CodeConstruct.CompilationUnit.AddChild(ns);

			var partialType = GetPartialType(sourceFileCode);
			partialType.Children.Clear();
			ns.AddChild(partialType);

			foreach (var type in types)
			{
				var code = sourceFileCode.Clone();
				var typeReference = type.ToTypeReference();
				var pointerType = type.MakePointerType().ToTypeReference();

				var createMethod = GetPartialType(sourceFileCode).Down().OfType<MethodDeclaration>().First(m => m.Name == "Create" && m.Parameters.Count == 3);
				createMethod.Parameters.First().TypeReference = type.MakeArrayType().ToTypeReference();
				createMethod.Body.Children.OfType<LocalVariableDeclaration>().First(vd => vd.Variables.First().Name == "sizeofTData").Variables.First()
					.Initializer.Cast<SizeOfExpression>().TypeReference = typeReference;
				createMethod.Body.Children.OfType<UnsafeStatement>().First().Block.Children.OfType<FixedStatement>().First().PointerDeclaration
					.Cast<LocalVariableDeclaration>().TypeReference = pointerType;
				partialType.AddChild(createMethod);

				createMethod = GetPartialType(sourceFileCode).Down().OfType<MethodDeclaration>().First(m => m.Name == "Create" && m.Parameters.Count == 1);
				createMethod.Parameters.First().TypeReference = type.MakeArrayType().ToTypeReference();
				createMethod.Body.Children.OfType<LocalVariableDeclaration>().First(vd => vd.Variables.First().Name == "sizeofTData").Variables.First()
					.Initializer.Cast<SizeOfExpression>().TypeReference = typeReference;
				partialType.AddChild(createMethod);

				var modifiedType = code.CompilationUnit.Down().OfType<TypeDeclaration>().First(t => t.Name == "PointerArrayOfByte");
				modifiedType.Name = modifiedType.Name.Replace("Byte", type.Name);
				modifiedType.BaseTypes.First().GenericTypes[0] = typeReference;
				modifiedType.Down().OfType<PropertyDeclaration>().First(p => p.IsIndexer).TypeReference = typeReference;
				modifiedType.Down().OfType<PropertyDeclaration>().First(p => p.IsIndexer).GetRegion.Block.Children.First().Cast<UnsafeStatement>().Block.Children
					.First().Cast<ReturnStatement>().Expression.Cast<IndexerExpression>().TargetObject.Cast<ParenthesizedExpression>().Expression.Cast<CastExpression>()
					.CastTo = pointerType;
				modifiedType.Down().OfType<PropertyDeclaration>().First(p => p.IsIndexer).SetRegion.Block.Children.First().Cast<UnsafeStatement>().Block.Children
					.First().Cast<ExpressionStatement>().Expression.Cast<AssignmentExpression>().Left.Cast<IndexerExpression>().TargetObject.Cast<ParenthesizedExpression>()
					.Expression.Cast<CastExpression>().CastTo = pointerType;

				ns.AddChild(modifiedType);
			}
		}

		TypeDeclaration GetPartialType(CodeConstruct sourceFileCode)
		{
			return sourceFileCode.Clone().CompilationUnit.Down().OfType<TypeDeclaration>().First(t => t.Name == "PointerArray" && t.Modifier.HasFlag(Modifiers.Partial));
		}

		#endregion methods
	}

	#endif

	public unsafe class PointerArray
	{
		#region constants

		public static readonly Type[] VALID_TYPES = new Type[] { typeof(byte), typeof(sbyte), typeof(short), typeof(ushort), typeof(int), typeof(uint), typeof(long),
			typeof(ulong), typeof(char), typeof(float), typeof(double), typeof(bool), typeof(decimal) };

		#endregion constants

		#region fields

		protected object dataReference;
		protected byte* pointer;

		#endregion fields

		#region constructors

		protected PointerArray()
		{
		}

		#endregion constructors
	}

	// TODO: (PS) Comment this.
	public partial class PointerArray<T> : PointerArray, IList<T>
		where T : struct
	{
		#region constants

		private static readonly int SIZEOF_T = Marshal.SizeOf(typeof(T));

		#endregion constants

		#region static fields

		private static Func<PointerArray<T>> newInstanceFunction;

		#endregion static fields

		#region static constructor

		static PointerArray()
		{
			if (!VALID_TYPES.Contains(typeof(T)))
			{
				throw new InvalidOperationException(String.Format("Only the types {0} are valid.",
					TextHelper.GetLocalizedEnumerationString(CultureInfo.GetCultureInfo("en"), VALID_TYPES.Select(t => t.Name))));
			}

			newInstanceFunction = GetNewInstanceFunction();
		}

		#endregion static constructor

		#region static methods

		public static PointerArray<T> Create(byte[] data, int startOffset, int count)
		{
			if (data == null)
			{
				throw new ArgumentNullException("data");
			}
			if (startOffset < 0)
			{
				throw new ArgumentException("Start offset must not be < 0.", "startOffset");
			}
			if (count < 0)
			{
				throw new ArgumentException("Count must not be < 0.", "count");
			}

			int sizeofTData = sizeof(byte);
			if (startOffset + count * SIZEOF_T > data.Length * sizeofTData)
			{
				throw new IndexOutOfRangeException("The sub-array must be within the confines of the given data array");
			}

			PointerArray<T> result = PointerArray<T>.newInstanceFunction();
			result.dataReference = data;
			unsafe
			{
				fixed (byte* ptr = data)
				{
					result.pointer = (byte*)ptr + startOffset;
				}
			}

			result.Count = data.Length * sizeofTData / SIZEOF_T;

			return result;
		}

		public static PointerArray<T> Create(byte[] data)
		{
			int sizeofTData = sizeof(byte);
			int count = data.Length * sizeofTData / SIZEOF_T;

			return Create(data, 0, count);
		}

		protected static Func<PointerArray<T>> GetNewInstanceFunction()
		{
			Type type = typeof(T);
			if (type == typeof(byte))
			{
				return () => new PointerArrayOfByte() as PointerArray<T>;
			}
			else if (type == typeof(sbyte))
			{
				return () => new PointerArrayOfSByte() as PointerArray<T>;
			}
			else if (type == typeof(short))
			{
				return () => new PointerArrayOfInt16() as PointerArray<T>;
			}
			else if (type == typeof(ushort))
			{
				return () => new PointerArrayOfUInt16() as PointerArray<T>;
			}
			else if (type == typeof(int))
			{
				return () => new PointerArrayOfInt32() as PointerArray<T>;
			}
			else if (type == typeof(uint))
			{
				return () => new PointerArrayOfUInt32() as PointerArray<T>;
			}
			else if (type == typeof(long))
			{
				return () => new PointerArrayOfInt64() as PointerArray<T>;
			}
			else if (type == typeof(ulong))
			{
				return () => new PointerArrayOfUInt64() as PointerArray<T>;
			}
			else if (type == typeof(char))
			{
				return () => new PointerArrayOfChar() as PointerArray<T>;
			}
			else if (type == typeof(float))
			{
				return () => new PointerArrayOfSingle() as PointerArray<T>;
			}
			else if (type == typeof(double))
			{
				return () => new PointerArrayOfDouble() as PointerArray<T>;
			}
			else if (type == typeof(bool))
			{
				return () => new PointerArrayOfBoolean() as PointerArray<T>;
			}
			else if (type == typeof(decimal))
			{
				return () => new PointerArrayOfDecimal() as PointerArray<T>;
			}
			else
			{
				throw new Exception("This was not supposed to happen.");
			}
		}

		#endregion static methods

		#region overrideable properties

		public virtual T this[int index]
		{
			get
			{
				throw new NotSupportedException();
			}
			set
			{
				throw new NotSupportedException();
			}
		}

		#endregion overrideable properties

		#region properties

		public int Count
		{
			get;
			private set;
		}

		bool ICollection<T>.IsReadOnly
		{
			get { return true; }
		}

		#endregion properties

		#region methods

		public bool Contains(T item)
		{
			return this.IndexOf(item) >= 0;
		}

		public void CopyTo(T[] array, int arrayIndex)
		{
			if (array == null)
			{
				throw new ArgumentNullException("array");
			}
			if (arrayIndex < 0 || arrayIndex >= array.Length)
			{
				throw new ArgumentOutOfRangeException(String.Concat("Arguemnt 'arrayIndex' must be between ", 0, " and ", array.Length, '.'), "arrayIndex");
			}

			for (int i = 0; i < this.Count; i++)
			{
				array[arrayIndex + i] = this[i];
			}
		}

		public IEnumerator<T> GetEnumerator()
		{
			return new Enumerator(this);
		}

		public int IndexOf(T item)
		{
			for (int i = 0; i < this.Count; i++)
			{
				if (object.Equals(this[i], item))
				{
					return i;
				}
			}

			return -1;
		}

		void ICollection<T>.Add(T item)
		{
			throw new NotSupportedException();
		}

		void ICollection<T>.Clear()
		{
			throw new NotSupportedException();
		}

		bool ICollection<T>.Remove(T item)
		{
			throw new NotSupportedException();
		}

		IEnumerator IEnumerable.GetEnumerator()
		{
			return this.GetEnumerator();
		}

		void IList<T>.Insert(int index, T item)
		{
			throw new NotSupportedException();
		}

		void IList<T>.RemoveAt(int index)
		{
			throw new NotSupportedException();
		}

		#endregion methods

		#region nested types

		private class Enumerator : IEnumerator<T>
		{
			#region fields

			private int count;
			private T current;
			private int index = -1;
			private PointerArray<T> owner;

			#endregion fields

			#region constructors

			public Enumerator(PointerArray<T> owner)
			{
				this.owner = owner;
				this.count = owner.Count;
			}

			#endregion constructors

			#region properties

			public T Current
			{
				get { return this.current; }
			}

			object IEnumerator.Current
			{
				get { return this.current; }
			}

			#endregion properties

			#region methods

			/// <summary>
			/// Performs application-defined tasks associated with freeing, releasing, or resetting unmanaged resources.
			/// </summary>
			public void Dispose()
			{
				this.owner = null;
			}

			public bool MoveNext()
			{
				int newIndex = this.index + 1;
				bool result = newIndex < this.count;
				if (result)
				{
					this.index = newIndex;
					this.current = this.owner[newIndex];
				}

				return result;
			}

			public void Reset()
			{
				this.index = -1;
			}

			#endregion methods
		}

		#endregion nested types
	}

	// TODO: (PS) Comment this.
	public class PointerArrayOfByte : PointerArray<byte>
	{
		#region overrideable properties

		public override byte this[int index]
		{
			get
			{
				unsafe
				{
					return ((byte*)this.pointer)[index];
				}
			}
			set
			{
				unsafe
				{
					((byte*)this.pointer)[index] = value;
				}
			}
		}

		#endregion overrideable properties
	}
}