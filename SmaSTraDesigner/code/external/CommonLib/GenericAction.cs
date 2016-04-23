namespace Common
{
	using System;

	#if MACRO

	using CodeGeneration;
	using Common.Collections.TreeWalk;
	using Common.ExtensionMethods;
	using ICSharpCode.NRefactory.Ast;
	using MacroLib;
	using MacroLib.Attributes;
	using System.Collections.Generic;
	using System.Linq;
	using System.Runtime.InteropServices;

	class GenericActionMacroContent
	{
		#region constants

		const int MAX_PARAMS = 16;

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
		void Macro(MacroOutput output, CodeConstruct sourceFileCode, string sourceFilePath, string defaultNamespace, int orderNo)
		{
			NamespaceDeclaration ns = output.CodeConstruct.CompilationUnit.PushChild(new NamespaceDeclaration(defaultNamespace));
			ns.AddUsing("System");
			ns.AddUsing("System.Reflection");

			MakeGeneric(ns, sourceFileCode, "GenericAction");
		}

		void MakeGeneric(INode parent, CodeConstruct sourceFileCode, string typeName)
		{
			CodeConstruct sourceFileCodeClone;

			for (int paramCount = 1; paramCount <= MAX_PARAMS; paramCount++)
			{
				sourceFileCodeClone = sourceFileCode.Clone();

				TypeDeclaration type = new DownwardTreeWalk<INode>(sourceFileCodeClone.CompilationUnit, n => n.Children)
					.OfType<TypeDeclaration>().First(t => t.Name == typeName);

				parent.AddChild(type);

				List<string> genericTypeNames = new List<string>();
				List<string> paramNames = new List<string>();
				for (int i = 1; i <= paramCount; i++)
				{
					string templateName = "T";
					string paramName = "param";
					if (paramCount > 1)
					{
						templateName += i;
						paramName += i;
					}

					genericTypeNames.Add(templateName);
					paramNames.Add(paramName);
				}

				type.Templates.AddRange(genericTypeNames.Select(name => new TemplateDefinition(name, null)));

				MethodDeclaration invokeMethod = type.Children.OfType<MethodDeclaration>().First(method => method.Name == "Invoke");
				invokeMethod.Parameters.AddRange(genericTypeNames.Select((name, i) => new ParameterDeclarationExpression(new TypeReference(name), paramNames[i])));

				ExpressionStatement statement = invokeMethod.Body.Children.OfType<ExpressionStatement>().First();
				InvocationExpression invocation = (InvocationExpression)statement.Expression;
				invocation.Arguments.AddRange(paramNames.Select(name => new IdentifierExpression(name)));

				MethodDeclaration getInstanceMethod = type.Children.OfType<MethodDeclaration>().First(method => method.Name == "GetInstance");
				AddGenericTypes(getInstanceMethod.TypeReference, genericTypeNames);

				CastExpression cast = getInstanceMethod.Body.Children.OfType<ReturnStatement>().First().Expression.Cast<CastExpression>();
				AddGenericTypes(cast.CastTo, genericTypeNames);
				AddGenericTypes(cast.Expression.Cast<InvocationExpression>().Arguments.First().Cast<ObjectCreateExpression>().CreateType, genericTypeNames);
			}
		}

		#endregion methods
	}

	#endif

	// TODO: (PS) Comment this.
	public class GenericAction : GenericActionBase
	{
		#region static methods

		public static GenericAction GetInstance(Action<object[]> callback)
		{
			return (GenericAction)DistinctInstanceProvider<GenericActionBase>.Instance.GetDistinctInstance(new GenericAction(callback));
		}

		#endregion static methods

		#region constructors

		protected GenericAction(Action<object[]> callback)
			: base(callback)
		{
		}

		#endregion constructors

		#region methods

		public void Invoke()
		{
			this.DynamicInvoke();
		}

		#endregion methods
	}
}