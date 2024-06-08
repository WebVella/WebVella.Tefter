namespace WebVella.Tefter.Database;

public class DbUpdateException : Exception
{
	public DbUpdateResult Result { get; private set; }

    public DbUpdateException(DbUpdateResult result) : base()
    {
		Result = result;
    }
}
