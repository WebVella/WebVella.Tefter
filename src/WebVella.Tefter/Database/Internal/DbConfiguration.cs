namespace WebVella.Tefter.Database.Internal;

internal class DbConfiguration
{
    public string ConnectionString { get; private set; }

    public DbConfiguration(IConfiguration config)
    {
        ConnectionString = config["Settings:ConnectionString"];
    }
}
