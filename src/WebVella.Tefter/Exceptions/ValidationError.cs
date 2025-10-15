namespace WebVella.Tefter.Exceptions;

public class ValidationError : Error
{
	public string PropertyName { get; init; }

	public string? ErrorCode { get; init; }
	public long Index { get; init; } = -1;

	public ValidationError(string propertyName, string message, string? errorCode = null, long index = -1)
		: base()
	{
		PropertyName = propertyName;
		Message = message;
		ErrorCode = errorCode;
		Index = index;
	}

}
