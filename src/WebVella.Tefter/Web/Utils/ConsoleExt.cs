namespace WebVella.Tefter.Web.Utils;
public static class ConsoleExt
{
	public static void WriteLine(string message){ 
		#if DEBUG
		Console.WriteLine(">>> TEFTER >>> " + message);
		#endif
	}
}
