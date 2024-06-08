namespace WebVella.Tefter.Database;

public class DbSqlProviderException : DbException
{
    public DbSqlProviderException() : base()
    {
    }
    public DbSqlProviderException(string message) : base(message)
    {
    }
    public DbSqlProviderException(string message, Exception innerException) : base(message, innerException)
    {
    }
}
