namespace WebVella.Tefter.Database;

public class TfDatabaseException : Exception
{
    public TfDatabaseException() : base()
    {
    }
    public TfDatabaseException(string message) : base(message)
    {
    }
    public TfDatabaseException(string message, Exception innerException) : base(message, innerException)
    {
    }
}
