using System.Diagnostics;
using System.Reflection;
using System.Runtime.CompilerServices;

namespace Common
{
    public static class DebugHelper
	{
		[MethodImpl(MethodImplOptions.NoInlining)]
		public static MethodBase GetCurrentMethod()
		{
			return new StackTrace().GetFrame(1).GetMethod();
		}
	}
}
