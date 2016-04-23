namespace Common.CommandLine
{
	using System;
	using System.Collections.Generic;
	using System.Linq;
	using System.Reflection;

	// TODO: (PS) Comment this.
	public class Command
	{
		#region static fields

		private static Dictionary<string, Command> commands = new Dictionary<string, Command>();

		#endregion static fields

		#region static methods

		public static void ParseAndExecute(string commandText)
		{
			commandText = commandText.Trim(' ', '\t');
			if (String.IsNullOrEmpty(commandText))
			{
				return;
			}

			List<string> parts = new List<string>();
			int startIndex = 0;
			bool word = false;
			bool quote = false;
			int i;
			for (i = 0; i < commandText.Length; i++)
			{
				char c = commandText[i];
				if (word)
				{
					if (quote)
					{
						if (c == '"')
						{
							quote = false;
							word = false;
							parts.Add(commandText.Substring(startIndex, i + 1 - startIndex));
						}
					}
					else
					{
						if (c == ' ' || c == '\t')
						{
							word = false;
							parts.Add(commandText.Substring(startIndex, i - startIndex));
						}
						else if (c == '"')
						{
							quote = true;
						}
					}
				}
				else if (quote)
				{
					if (c == '"')
					{
						quote = false;
						parts.Add(commandText.Substring(startIndex, i - startIndex));
					}
				}
				else
				{
					if (c == '"')
					{
						quote = true;
						startIndex = i + 1;
					}
					else if (!(c == ' ' || c == '\t'))
					{
						word = true;
						startIndex = i;
					}
				}
			}

			if (word || quote)
			{
				parts.Add(commandText.Substring(startIndex, i - startIndex));
			}

			if (parts.Count == 0 || parts[0].Length == 0)
			{
				return;
			}

			string commandName = parts[0].ToLower();
			if (commandName.Contains('"'))
			{
				throw new Exception("Command name cannot contain '\"' character.");
			}

			Command cmd;
			if (!commands.TryGetValue(commandName, out cmd))
			{
				throw new Exception(String.Format("Unrecognized command \"{0}\".", commandName));
			}

			cmd.Execute(parts.Skip(1).ToArray());
		}

		public static void Register(Delegate callback)
		{
			Register(callback.Target, callback.Method);
		}

		public static void Register(MethodInfo callback)
		{
			Register(null, callback);
		}

		public static void Register(object callbackTarget, MethodInfo callback)
		{
			commands.Add(callback.Name.ToLower(), new Command(callbackTarget, callback));
		}

		public static void RegisterAll(Type typeWithCommandCallbacks)
		{
			if (typeWithCommandCallbacks == null)
			{
				throw new ArgumentNullException("typeWithCommandCallbacks");
			}

			bool useAllMethods = typeWithCommandCallbacks.GetCustomAttributes(typeof(CommandAttribute), false).Length != 0;
			var methods = typeWithCommandCallbacks.GetMethods().Where(m => m.IsStatic && m.ReturnType == typeof(void) &&
				(useAllMethods || m.GetCustomAttributes(typeof(CommandAttribute), false).Length != 0));
			foreach (var method in methods)
			{
				Register(method);
			}
		}

		#endregion static methods

		#region fields

		private MethodInfo callback;
		private object callbackTarget = null;

		#endregion fields

		#region constructors

		private Command(object callbackTarget, MethodInfo callback)
		{
			if (callback == null)
			{
				throw new ArgumentNullException("callback");
			}

			this.callbackTarget = callbackTarget;
			this.callback = callback;
		}

		#endregion constructors

		#region methods

		public void Execute(string[] parameters)
		{
			if (parameters == null)
			{
				throw new ArgumentNullException("parameters");
			}
			var callbackParameters = this.callback.GetParameters();
			if (parameters.Length != callbackParameters.Length)
			{
				if (callbackParameters.Length == 1 && callbackParameters[0].ParameterType == typeof(string))
				{
					if (parameters.Length >= 1)
					{
						parameters = new string[] { String.Join(" ", parameters) };
					}
					else
					{
						parameters = new string[] { String.Empty };
					}
				}
				else
				{
					throw new ArgumentException(String.Format("Wrong parameter count. Should be {0}.", callbackParameters.Length), "parameters");
				}
			}

			List<object> parsedParameters = new List<object>();
			int i = 0;
			using (var enumerator1 = parameters.AsEnumerable().GetEnumerator())
			using (var enumerator2 = callbackParameters.AsEnumerable().GetEnumerator())
			{
				while (enumerator1.MoveNext() && enumerator2.MoveNext())
				{
					var parameter = enumerator1.Current;
					var callbackParameter = enumerator2.Current;

					if (callbackParameter.ParameterType == typeof(string))
					{
						parsedParameters.Add(parameter);
					}
					else if (callbackParameter.ParameterType.IsEnum)
					{
						object parsed = Enum.GetValues(callbackParameter.ParameterType).Cast<object>()
							.FirstOrDefault(value => String.Equals(value.ToString(), parameter, StringComparison.InvariantCultureIgnoreCase));
						if (parsed != null)
						{
							parsedParameters.Add(parsed);
						}
						else
						{
							throw new Exception(String.Format("Command parameter #{0} could not be parsed.", i));
						}
					}
					else
					{
						MethodInfo parseMethod = callbackParameter.ParameterType.GetMethod("Parse", new Type[] { typeof(string) });
						if (parseMethod != null)
						{
							try
							{
								parsedParameters.Add(parseMethod.Invoke(null, new object[] { parameter }));
							}
							catch (Exception ex)
							{
								throw new Exception(String.Format("Command parameter #{0} could not be parsed.", i), ex);
							}
						}
						else
						{
							throw new Exception(String.Format("Command parameter #{0} could not be parsed.", i));
						}
					}

					i++;
				}
			}

			this.callback.Invoke(this.callbackTarget, parsedParameters.ToArray());
		}

		#endregion methods
	}
}