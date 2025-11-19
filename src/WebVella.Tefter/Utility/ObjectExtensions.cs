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
	public static object? GetPropertyByName(this object target, string propName)
	{
		if (target == null) throw new ArgumentNullException(nameof(target));
		if (string.IsNullOrWhiteSpace(propName)) throw new ArgumentException("Property name required", nameof(propName));

		var type = target.GetType();
		// BindingFlags.IgnoreCase lets you ignore case differences
		var propInfo = type.GetProperty(propName,
			BindingFlags.Public | BindingFlags.Instance | BindingFlags.IgnoreCase);

		if (propInfo == null)
			throw new ArgumentException($"Property '{propName}' not found on {type.FullName}");

		return propInfo.GetValue(target);
	}	
}
