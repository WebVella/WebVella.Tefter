using System.Runtime.CompilerServices;

namespace WebVella.Tefter.Exceptions;

public class TfServiceException : TfException
{
	public TfServiceException() : base() { }

	public TfServiceException(
		string message) : base(message) { }

	public TfServiceException(
		string message,
		Exception innerException) : base(message, innerException)
	{
	}
}
