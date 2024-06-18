namespace WebVella.Tefter.Database;

public class DatabaseUpdateException : Exception
{
	public DatabaseUpdateResult Result { get; private set; }

    public DatabaseUpdateException(DatabaseUpdateResult result) : base()
    {
		Result = result;
    }
}
