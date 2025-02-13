namespace WebVella.Tefter.Exceptions;

public class TfDboServiceException : TfException
{
	public TfDboServiceException() : base() { }

	public TfDboServiceException(string message) : base(message) { }

	public TfDboServiceException(string message, Exception innerException) : base(message, innerException) { }

	public TfDboServiceException(string message, System.Collections.IDictionary data) : base(message, data) { }

	public TfDboServiceException(string message, Dictionary<string, List<string>> data) : base(message, data) { }
}
