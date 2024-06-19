namespace WebVella.Tefter.Errors;

internal class DboManagerError : Error
{
	public DboManagerError(string operation, object obj = null)
		: base("DboManager operation failed.")
	{
		Metadata.Add(operation, obj);
	}
}
