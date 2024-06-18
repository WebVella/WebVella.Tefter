namespace WebVella.Tefter.Database;

public class DatabaseSqlProviderException : DatabaseException
{
    public DatabaseSqlProviderException() : base()
    {
    }
    public DatabaseSqlProviderException(string message) : base(message)
    {
    }
    public DatabaseSqlProviderException(string message, Exception innerException) : base(message, innerException)
    {
    }
}
