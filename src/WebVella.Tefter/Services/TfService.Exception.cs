using WebVella.Tefter.Utility;

namespace WebVella.Tefter.Services;

public partial interface ITfService
{
}

public partial class TfService : ITfService
{
	public Exception ProcessException(
		Exception ex)
	{
		if (ex == null)
			throw new Exception("Exception is not specified.");

		if (ex is TfValidationException)
		{
			return LogValidationException((TfValidationException)ex);
		}

		if (ex is TfServiceException)
		{
			return LogServiceException((TfServiceException)ex);
		}

		return CreateAndLogServiceException(ex);

	}

	public TfValidationException LogValidationException(
		TfValidationException exception)
	{
		_logger.LogWarning(exception, "Message:{message} Data:{@exData}", exception.Message, exception.Data);
		return exception;
	}

	public TfServiceException LogServiceException(
		TfServiceException exception)
	{
		_logger.LogError(exception, "Message:{message} Data:{@exData}", exception.Message, exception.Data);
		return exception;
	}

	private TfServiceException CreateAndLogServiceException(
		Exception exception)
	{
		var serviceException = new TfServiceException("An unexpected error occurred.", exception);
		serviceException.SetStackTrace(Environment.StackTrace);
		return LogServiceException(serviceException);
	}
}
