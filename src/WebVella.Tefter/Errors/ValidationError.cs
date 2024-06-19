namespace WebVella.Tefter.Errors;

public class ValidationError : Error
{
	public string PropertyName { get; init; }

	public string Reason { get; init; }

	public string ErrorCode { get; init; }

	public ValidationError(string propertyName, string reason, string errorCode = null)
		: base()
	{
		PropertyName = propertyName;
		Reason = reason;
		ErrorCode = errorCode;
	}
}
