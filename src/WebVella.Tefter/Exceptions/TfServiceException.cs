namespace WebVella.Tefter.Exceptions;

public class TfValidationException : TfException
{
	public TfValidationException() : base() { }

	public TfValidationException(string message) : base(message) { }

	public TfValidationException(string message, Exception innerException) : base(message, innerException) { }

	public TfValidationException(string message, System.Collections.IDictionary data) : base(message, data) { }

	public TfValidationException(string message, Dictionary<string, List<string>> data) : base(message, data) { }

	public TfValidationException(string key, string message) : base()
	{
		AddValidationError(key, message);
	}

	public void AddValidationError(string key, string value)
	{
		UpsertDataList(key, value);
	}

}
