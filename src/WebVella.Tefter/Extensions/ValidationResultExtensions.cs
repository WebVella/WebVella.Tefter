namespace WebVella.Tefter;

public static class ValidationResultExtensions
{
	//public static Result<T> ToResult<T>(this ValidationResult validationResult)
	//{
	//	if (validationResult == null ||
	//		validationResult.IsValid ||
	//		validationResult.Errors?.Count == 0)
	//	{
	//		throw new ArgumentException(nameof(validationResult));
	//	}

	//	var result = new Result<T>();

	//	foreach (var error in validationResult.Errors)
	//	{
	//		result.WithError(
	//			new ValidationError(
	//				error.PropertyName,
	//				error.ErrorMessage,
	//				error.ErrorCode)
	//			);
	//	}

	//	return result;
	//}

	//public static Result ToResult(this ValidationResult validationResult)
	//{
	//	if (validationResult == null ||
	//		validationResult.IsValid ||
	//		validationResult.Errors?.Count == 0)
	//	{
	//		throw new ArgumentException(nameof(validationResult));
	//	}

	//	var result = new Result();

	//	foreach (var error in validationResult.Errors)
	//	{
	//		result.WithError(
	//			new ValidationError(
	//				error.PropertyName,
	//				error.ErrorMessage,
	//				error.ErrorCode)
	//			);
	//	}

	//	return result;
	//}

	public static TfValidationException ToValidationException(this ValidationResult validationResult)
	{
		var valEx = new TfValidationException();

		if (validationResult == null ||
			validationResult.IsValid ||
			validationResult.Errors?.Count == 0)
		{
			return valEx;
		}

		foreach (var error in validationResult.Errors)
		{
			valEx.AddValidationError(error.PropertyName, error.ErrorMessage);
		}

		return valEx;
	}
}