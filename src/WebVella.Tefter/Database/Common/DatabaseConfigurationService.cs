namespace WebVella.Tefter.Database;

public interface IDbConfigurationService
{
    public string ConnectionString { get;  }
}

public class DatabaseConfigurationService : IDbConfigurationService
{
    public string ConnectionString { get; private set; }

    public DatabaseConfigurationService(ITfConfigurationService config)
    {
        ConnectionString = config.ConnectionString;
    }
}
