namespace WebVella.Tefter.Database;

public class DbBuilderException : DbException
{
    public DbBuilderException() : base()
    {
    }
    public DbBuilderException(string message) : base(message)
    {
    }
    public DbBuilderException(string message, Exception innerException) : base(message, innerException)
    {
    }
}
