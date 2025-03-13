namespace WebVella.Tefter.Utility;

public static class ValidationResultExtensions
{
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