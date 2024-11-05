namespace WebVella.Tefter.Database;

public class TfDatabaseBuilderException : TfDatabaseException
{
    public TfDatabaseBuilderException() : base()
    {
    }
    public TfDatabaseBuilderException(string message) : base(message)
    {
    }
    public TfDatabaseBuilderException(string message, Exception innerException) : base(message, innerException)
    {
    }
}
