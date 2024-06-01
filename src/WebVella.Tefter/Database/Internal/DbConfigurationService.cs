namespace WebVella.Tefter.Database.Internal;

internal interface IDbConfigurationService
{
    public string ConnectionString { get;  }
}

internal class DbConfigurationService : IDbConfigurationService
{
    public string ConnectionString { get; private set; }

    public DbConfigurationService(IConfiguration config)
    {
        ConnectionString = config["ConnectionString"];
    }
}
