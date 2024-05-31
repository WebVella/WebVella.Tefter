namespace WebVella.Tefter.Database;

public class IDbManager
{
}

public class DbManager : IDbManager
{
    private readonly IDbService _dbService;

    public DbManager(IServiceProvider serviceProvider)
    {
        _dbService = serviceProvider.GetService<IDbService>();
    }

    public void ValidateTable(DbTable table)
    {
    }

    public void SaveTable(DbTable table)
    {
        ValidateTable(table);


    }


}
