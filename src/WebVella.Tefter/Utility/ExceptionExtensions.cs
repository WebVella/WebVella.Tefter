using System.Diagnostics;

namespace WebVella.Tefter.Utility;
public static class ExceptionUtilities
{
	private static readonly FieldInfo STACK_TRACE_STRING_FI = typeof(Exception).GetField("_stackTraceString", BindingFlags.NonPublic | BindingFlags.Instance);
	
	public static void SetStackTrace(this Exception target, string stackTrace)
	{
		STACK_TRACE_STRING_FI.SetValue(target, stackTrace);
	}
}