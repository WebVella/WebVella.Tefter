namespace WebVella.Tefter.Utility;

public static class ObjectExtensions
{
	public static long? ToLong(this object value)
	{
		return value switch
		{
			null => null,
			long l => l,
			short s => (long?)s,
			int i => (long?)i,
			decimal d =>
				// choose your policy on overflow:
				d <= long.MaxValue && d >= long.MinValue ? (long?)d : throw new OverflowException(),
			_ => throw new ArgumentException("Unsupported type")
		};
	}
}
