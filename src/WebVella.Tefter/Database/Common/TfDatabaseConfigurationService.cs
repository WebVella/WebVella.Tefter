namespace WebVella.Tefter.Database;

public interface ITfDbConfigurationService
{
    public string ConnectionString { get;  }
}

public class TfDatabaseConfigurationService : ITfDbConfigurationService
{
    public string ConnectionString { get; private set; }

    public TfDatabaseConfigurationService(ITfConfigurationService config)
    {
        ConnectionString = config.ConnectionString;
    }
}
