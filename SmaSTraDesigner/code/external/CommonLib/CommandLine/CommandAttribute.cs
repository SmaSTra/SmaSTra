namespace Common.CommandLine
{
	using System;

	// TODO: (PS) Comment this.
	[AttributeUsage(AttributeTargets.Class | AttributeTargets.Method, AllowMultiple = false, Inherited = false)]
	public sealed class CommandAttribute : Attribute
	{
	}
}