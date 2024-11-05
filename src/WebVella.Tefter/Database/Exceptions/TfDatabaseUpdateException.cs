namespace WebVella.Tefter.Database;

public class TfDatabaseUpdateException : Exception
{
	public TfDatabaseUpdateResult Result { get; private set; }

    public TfDatabaseUpdateException(TfDatabaseUpdateResult result) : base()
    {
		Result = result;
    }
}
