namespace WebVella.Tefter.Utility;

public static class ValidationErrorExt
{
	public static void Add(this List<ValidationError> list, string propertyName, string message)
	{
		list.Add(new ValidationError(propertyName, message));
	}

	
}