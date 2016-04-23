namespace Common
{
	using System;

	#if MACRO

	using CodeGeneration;
	using Common.ExtensionMethods;
	using ICSharpCode.NRefactory.Ast;
	using MacroLib;
	using MacroLib.Attributes;
	using System.Collections.Generic;
	using System.Linq;
	using System.Runtime.InteropServices;

	public class WeakDelegateMacroContent
	{
		#region constants

		const int MAX_PARAMS = 8;

		#endregion constants

		#region static methods

		[DllImport("user32.dll", CharSet = CharSet.Unicode)]
		public static extern int MessageBox(IntPtr hWnd, String text, String caption, uint type);

		public static void MessageBox(object text = null)
		{
			MessageBox(IntPtr.Zero, text != null ? text.ToString() : String.Empty, null, 0);
		}

		#endregion static methods

		#region methods

		void AddGenericTypes(TypeReference typeReference, IEnumerable<string> genericTypeNames)
		{
			var genericTypes = typeReference.GenericTypes.ToArray();
			typeReference.GenericTypes.Clear();
			typeReference.GenericTypes.AddRange(genericTypeNames.Select(name => new TypeReference(name)).Concat(genericTypes));
		}

		[Execute]
		void ExecuteMacro(MacroOutput output, CodeConstruct sourceFileCode, string sourceFilePath, string defaultNamespace, int orderNo)
		{
			NamespaceDeclaration ns = output.CodeConstruct.CompilationUnit.PushChild(
				new NamespaceDeclaration(sourceFileCode.CompilationUnit.Down().OfType<NamespaceDeclaration>().First().Name));
			ns.AddUsing("System");

			List<string> genericTypeNameList = new List<string>();
			List<string> paramNameList = new List<string>();
			for (int i = 1; i <= MAX_PARAMS; i++)
			{
				genericTypeNameList.Add("T" + i);
				paramNameList.Add("param" + i);
			}

			for (int paramCount = 1; paramCount <= MAX_PARAMS; paramCount++)
			{
				Func<string, int, bool> where = (name, index) => index < paramCount;
				IEnumerable<string> genericTypeNames = genericTypeNameList.Where(where);
				string[] paramNames = paramNameList.Where(where).ToArray();

				CodeConstruct sourceFileCodeClone = sourceFileCode.Clone();
				ns.AddChild(MakeGenericCopy(sourceFileCodeClone, "WeakAction", genericTypeNames, paramNames));
				ns.AddChild(MakeGenericCopy(sourceFileCodeClone, "WeakFunc", genericTypeNames, paramNames));
			}
		}

		TypeDeclaration MakeGenericCopy(CodeConstruct sourceFileCodeClone, string typeName, IEnumerable<string> genericTypeNames, string[] paramNames)
		{
			TypeDeclaration result = sourceFileCodeClone.CompilationUnit.Down().OfType<TypeDeclaration>().First(t => t.Name == typeName);

			var templates = result.Templates.ToArray();
			result.Templates.Clear();
			result.Templates.AddRange(genericTypeNames.Select(name => new TemplateDefinition(name, null)).Concat(templates));

			AddGenericTypes(result.Children.OfType<ConstructorDeclaration>().First().Parameters.First().TypeReference, genericTypeNames);

			var method = result.Down().OfType<MethodDeclaration>().First(m => m.Name == "Invoke");
			method.Parameters.AddRange(genericTypeNames.Select((name, index) => new ParameterDeclarationExpression(new TypeReference(name), paramNames[index])));
			InvocationExpression invocation;
			if (method.Body.Children[0] is ReturnStatement)
			{
				invocation = method.Body.Children.First().Cast<ReturnStatement>().Expression.Cast<CastExpression>().Expression.Cast<InvocationExpression>();
			}
			else
			{
				invocation = method.Body.Children.First().Cast<ExpressionStatement>().Expression.Cast<InvocationExpression>();
			}

			invocation.Arguments.Clear();
			invocation.Arguments.Add(new ArrayCreateExpression(new TypeReference("object[]", new int[] { }),
				new CollectionInitializerExpression().Manipulate(ex => ex.CreateExpressions.AddRange(paramNames.Select(name => new IdentifierExpression(name))))));

			var op = result.Children.OfType<OperatorDeclaration>().First(o => o.IsConversionOperator && o.ConversionType == ConversionType.Implicit);
			AddGenericTypes(op.TypeReference, genericTypeNames);
			AddGenericTypes(op.Parameters.First().TypeReference, genericTypeNames);

			var getInstanceMethod = result.Down().OfType<MethodDeclaration>().First(m => m.Name == "GetInstance");

			AddGenericTypes(getInstanceMethod.TypeReference, genericTypeNames);
			AddGenericTypes(getInstanceMethod.Parameters.First().TypeReference, genericTypeNames);

			var returnedExpression = getInstanceMethod.Body.Children.First().Cast<ReturnStatement>().Expression.Cast<CastExpression>();
			AddGenericTypes(returnedExpression.CastTo, genericTypeNames);
			AddGenericTypes(returnedExpression.Expression.Cast<InvocationExpression>().Arguments.First().Cast<ObjectCreateExpression>().CreateType, genericTypeNames);

			return result;
		}

		#endregion methods
	}

	#endif

	// TODO: (PS) Comment this.
	public class WeakAction : WeakDelegate
	{
		#region static methods

		public static implicit operator Action(WeakAction subject)
		{
			return subject.Invoke;
		}

		public static WeakAction GetInstance(Action action)
		{
			return (WeakAction)DistinctInstanceProvider<WeakDelegate>.Instance.GetDistinctInstance(new WeakAction(action));
		}

		#endregion static methods

		#region constructors

		protected WeakAction(Action action)
			: base(action)
		{
		}

		#endregion constructors

		#region methods

		public void Invoke()
		{
			this.InvokeMethod(null);
		}

		#endregion methods
	}

	// TODO: (PS) Comment this.
	public class WeakFunc<TResult> : WeakDelegate
	{
		#region static methods

		public static implicit operator Func<TResult>(WeakFunc<TResult> subject)
		{
			return subject.Invoke;
		}

		public static WeakFunc<TResult> GetInstance(Func<TResult> function)
		{
			return (WeakFunc<TResult>)DistinctInstanceProvider<WeakDelegate>.Instance.GetDistinctInstance(new WeakFunc<TResult>(function));
		}

		#endregion static methods

		#region constructors

		protected WeakFunc(Func<TResult> function)
			: base(function)
		{
		}

		#endregion constructors

		#region methods

		public TResult Invoke()
		{
			return (TResult)this.InvokeMethod(null);
		}

		#endregion methods
	}
}