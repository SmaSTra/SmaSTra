namespace Common
{
	using System;
	using System.Collections.Generic;
	using System.ComponentModel;
	using System.Threading;

	using Common.Collections;

	#if MACRO

	using CodeGeneration;
	using Common.ExtensionMethods;
	using ICSharpCode.NRefactory.Ast;
	using MacroLib;
	using MacroLib.Attributes;
	using System.Linq;
	using System.Runtime.InteropServices;

	#endif

	public partial class ThreadingHelper : IDisposable
	{
		#if MACRO

		[Execute]
		void ExecuteMacro(MacroOutput output, CodeConstruct sourceFileCode, string sourceFilePath, string defaultNamespace, int orderNo)
		{
			string typeName = sourceFileCode.CompilationUnit.Down().OfType<TypeDeclaration>().First().Name;

			NamespaceDeclaration ns = (NamespaceDeclaration)output.CodeConstruct.CompilationUnit.PushChild(sourceFileCode.Clone().CompilationUnit
				.GetPartialClassArchitecture(typeName));
			ns.AddUsings(sourceFileCode.CompilationUnit.GetUsings());

			TypeDeclaration generatedType = output.CodeConstruct.CompilationUnit.Down()
				.OfType<TypeDeclaration>().First(t => t.Name == typeName);

			List<string> genericTypeNameList = new List<string>();
			List<string> paramNameList = new List<string>();
			for (int i = 2; i <= MAX_PARAMS; i++)
			{
				genericTypeNameList.Add("T" + i);
				paramNameList.Add("param" + i);
			}

			for (int paramCount = 2; paramCount <= MAX_PARAMS; paramCount++)
			{
				IEnumerable<string> genericTypeNames = genericTypeNameList.Where((name, index) => index <= paramCount - 2);
				string[] paramNames = paramNameList.Where((name, index) => index <= paramCount - 2).ToArray();

				MakeMethodWithAdditionalGenericTypes(generatedType, sourceFileCode, genericTypeNames, paramNames, typeName, "InvokeActionInUnmonitoredWorkerThread", true);
				MakeMethodWithAdditionalGenericTypes(generatedType, sourceFileCode, genericTypeNames, paramNames, typeName, "InvokeActionInWorkerThread", false);
			}
		}

		void MakeMethodWithAdditionalGenericTypes(TypeDeclaration generatedType, CodeConstruct sourceFileCode, IEnumerable<string> genericTypeNames, string[] paramNames,
			string typeName, string methodName, bool staticMethod)
		{
			TypeDeclaration type = sourceFileCode.Clone().CompilationUnit.Down()
				.OfType<TypeDeclaration>().First(t => t.Name == typeName);

			MethodDeclaration method = type.Children.OfType<MethodDeclaration>().First(m => m.Name == methodName && m.Templates.Count != 0 &&
				((staticMethod && m.Modifier.HasFlag(Modifiers.Static)) || !(staticMethod || m.Modifier.HasFlag(Modifiers.Static))));
			method.Templates[0].Name = "T1";
			method.Templates.AddRange(genericTypeNames.Select(name => new TemplateDefinition(name, null)));
			method.Parameters[0].TypeReference.GenericTypes[0].Type = "T1";
			method.Parameters[0].TypeReference.GenericTypes.AddRange(genericTypeNames.Select(name => new TypeReference(name)));
			method.Parameters[1].TypeReference.Type = "T1";
			method.Parameters[1].ParameterName = "param1";
			method.Parameters.AddRange(genericTypeNames.Select((name, index) => new ParameterDeclarationExpression(new TypeReference(name), paramNames[index++])));

			InvocationExpression invocation = method.Body.Children.OfType<LocalVariableDeclaration>().First().Variables.First().Initializer
				.Cast<ObjectCreateExpression>().Parameters[0].Cast<AnonymousMethodExpression>().Body.Children.Cast<ExpressionStatement>().First()
				.Expression.Cast<InvocationExpression>();
			invocation.Arguments[0].Cast<IdentifierExpression>().Identifier = "param1";
			invocation.Arguments.AddRange(paramNames.Select(name => new IdentifierExpression(name)));

			generatedType.AddChild(method);
		}

		const int MAX_PARAMS = 8;

		[DllImport("user32.dll", CharSet = CharSet.Unicode)]
		public static extern int MessageBox(IntPtr hWnd, String text, String caption, uint type);

		public static void MessageBox(object text = null)
		{
			MessageBox(IntPtr.Zero, text != null ? text.ToString() : String.Empty, null, 0);
		}

		#endif

		#region static methods

		public static Thread InvokeActionInUnmonitoredWorkerThread(ThreadStart method)
		{
			if (method == null)
			{
				throw new ArgumentNullException("method");
			}

			Thread thread = new Thread(method);
			thread.Start();

			return thread;
		}

		public static Thread InvokeActionInUnmonitoredWorkerThread<T>(Action<T> method, T param)
		{
			if (method == null)
			{
				throw new ArgumentNullException("method");
			}

			Thread thread = new Thread(delegate()
			{
				method(param);
			});
			thread.Start();

			return thread;
		}

		public static Thread InvokeActionInUnmonitoredWorkerThread(Delegate method, params object[] parameters)
		{
			if (method == null)
			{
				throw new ArgumentNullException("method");
			}

			Thread thread = new Thread(parameter =>
			{
				method.Method.Invoke(method.Target, parameters);
			});
			thread.Start();

			return thread;
		}

		#endregion static methods

		#region fields

		private ObservableCollectionWrap<Thread> runningThreads = new ObservableCollectionWrap<Thread>(new HashSet<Thread>());

		#endregion fields

		#region constructors

		public ThreadingHelper()
		{
			this.runningThreads.PropertyChanged += this.runningThreads_PropertyChanged;
		}

		~ThreadingHelper()
		{
			this.Dispose();
		}

		#endregion constructors

		#region events

		/// <summary>
		/// Occurs when all threads have finished or were killed.
		/// </summary>
		public event EventHandler<EventArgs> AllThreadsDead;

		#endregion events

		#region properties

		/// <summary>
		/// Gets or sets a value indicating whether this <see cref="ThreadingHelper"/> is disposed.
		/// </summary>
		/// <value><c>true</c> if disposed; otherwise, <c>false</c>.</value>
		public bool IsDisposed
		{
			get;
			protected set;
		}

		#endregion properties

		#region methods

		/// <summary>
		/// Performs application-defined tasks associated with freeing, releasing, or resetting unmanaged resources.
		/// </summary>
		public void Dispose()
		{
			if (!this.IsDisposed)
			{
				this.IsDisposed = true;

				lock (this.runningThreads)
				{
					if (this.runningThreads.Count != 0)
					{
						this.KillAllThreads();
					}
				}
			}
		}

		public void InvokeActionInWorkerThread(Action method)
		{
			if (method == null)
			{
				throw new ArgumentNullException("method");
			}

			Thread thread = new Thread(delegate(object threadParameter)
			{
				method();
				this.FinishUpThread((Thread)threadParameter);
			});
			this.StartThread(thread);
		}

		public void InvokeActionInWorkerThread<T>(Action<T> method, T param)
		{
			if (method == null)
			{
				throw new ArgumentNullException("method");
			}

			Thread thread = new Thread(delegate(object threadParameter)
			{
				method(param);
				this.FinishUpThread((Thread)threadParameter);
			});
			this.StartThread(thread);
		}

		public void InvokeActionInWorkerThread(Delegate method, params object[] parameters)
		{
			if (method == null)
			{
				throw new ArgumentNullException("method");
			}

			this.StartThread(new Thread(delegate(object threadParameter)
			{
				method.Method.Invoke(method.Target, parameters);
				this.FinishUpThread((Thread)threadParameter);
			}));
		}

		public void KillAllThreads()
		{
			foreach (Thread thread in this.runningThreads)
			{
				thread.Abort();
			}

			lock (this.runningThreads)
			{
				this.runningThreads.Clear();
			}
		}

		public void KillThread(Thread thread)
		{
			thread.Abort();
			lock (this.runningThreads)
			{
				this.runningThreads.Remove(thread);
			}
		}

		private void FinishUpThread(Thread thread)
		{
			lock (this.runningThreads)
			{
				this.runningThreads.Remove(thread);
			}
		}

		/// <summary>
		/// Raises the <see cref="E:AllThreadsDead"/> event.
		/// </summary>
		private void OnAllThreadsDead()
		{
			if (this.AllThreadsDead != null)
			{
				this.AllThreadsDead(this, null);
			}
		}

		private void StartThread(Thread thread)
		{
			lock (this.runningThreads)
			{
				this.runningThreads.Add(thread);
			}
			thread.Start(thread);
		}

		#endregion methods

		#region event handlers

		private void runningThreads_PropertyChanged(object sender, PropertyChangedEventArgs e)
		{
			if (e.PropertyName == "Count" && this.runningThreads.Count == 0)
			{
				this.OnAllThreadsDead();
			}
		}

		#endregion event handlers
	}
}