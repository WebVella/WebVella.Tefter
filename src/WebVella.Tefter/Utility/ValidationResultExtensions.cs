namespace WebVella.Tefter.Utility;

public static class ValidationResultExtensions
{
	public static TfValidationException ToValidationException(this ValidationResult? validationResult)
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

	public static TfValidationException ToValidationException(this List<ValidationResult> validationResults)
	{
		var valEx = new TfValidationException();

		if (validationResults == null || validationResults.Count == 0)
		{
			return valEx;
		}

		long index = -1;
		foreach (var validationResult in validationResults)
		{
			index++;

			if (validationResult == null ||
				validationResult.IsValid ||
				validationResult.Errors?.Count == 0)
			{
				continue;
			}

			foreach (var error in validationResult.Errors)
			{
				valEx.AddValidationError(error.PropertyName, error.ErrorMessage, index);
			}
		}

		return valEx;
	}
}