﻿namespace WebVella.Tefter.Exceptions;

public class ValidationException : TfException
{
	public ValidationException() : base() { }

	public ValidationException(string message) : base(message) { }

	public ValidationException(string message, Exception innerException) : base(message, innerException) { }

	public ValidationException(string message, System.Collections.IDictionary data) : base(message, data) { }

	public ValidationException(string message, Dictionary<string, List<string>> data) : base(message, data) { }
}
