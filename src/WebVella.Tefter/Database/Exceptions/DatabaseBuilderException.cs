namespace WebVella.Tefter.Database;

public class DatabaseBuilderException : DatabaseException
{
    public DatabaseBuilderException() : base()
    {
    }
    public DatabaseBuilderException(string message) : base(message)
    {
    }
    public DatabaseBuilderException(string message, Exception innerException) : base(message, innerException)
    {
    }
}
