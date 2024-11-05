namespace WebVella.Tefter.Database;

public class TfDatabaseSqlProviderException : TfDatabaseException
{
    public TfDatabaseSqlProviderException() : base()
    {
    }
    public TfDatabaseSqlProviderException(string message) : base(message)
    {
    }
    public TfDatabaseSqlProviderException(string message, Exception innerException) : base(message, innerException)
    {
    }
}
