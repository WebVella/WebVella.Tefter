namespace WebVella.Tefter.Database;

public class DbSqlGeneratorException : DbException
{
    public DbSqlGeneratorException() : base()
    {
    }
    public DbSqlGeneratorException(string message) : base(message)
    {
    }
    public DbSqlGeneratorException(string message, Exception innerException) : base(message, innerException)
    {
    }
}
