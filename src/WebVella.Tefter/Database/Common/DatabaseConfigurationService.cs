namespace WebVella.Tefter.Database;

public interface IDbConfigurationService
{
    public string ConnectionString { get;  }
}

public class DatabaseConfigurationService : IDbConfigurationService
{
    public string ConnectionString { get; private set; }

    public DatabaseConfigurationService(IConfiguration config)
    {
        ConnectionString = config["Tefter:ConnectionString"];
    }
}
