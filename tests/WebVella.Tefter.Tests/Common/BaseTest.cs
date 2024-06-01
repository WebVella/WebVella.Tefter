namespace WebVella.Tefter.Tests.Common;

public class BaseTest
{
    protected static readonly AsyncLock locker = new AsyncLock();

    public TestContext Context { get; }

    public ServiceProvider ServiceProvider { get; }

    public BaseTest()
    {
        Context = new TestContext();
        Context.Services.AddSingleton<ILogger, DebugLogger>();
        Context.Services.AddSingleton<IDbConfigurationService, DbConfigurationService>((Context) =>
        {
            return new DbConfigurationService(new ConfigurationBuilder()
                .AddJsonFile("appsettings.json".ToApplicationPath())
                .AddJsonFile($"appsettings.{Environment.MachineName}.json".ToApplicationPath(), true)
           .Build());
        });
        Context.Services.AddSingleton<ITransactionRollbackNotifyService, TransactionRollbackNotifyService>();
        Context.Services.AddSingleton<IDbService, DbService>();
        Context.Services.AddSingleton<IDbManager, DbManager>();

        ServiceProvider = Context.Services.BuildServiceProvider();
    }

    protected DataTable ExecuteSqlQueryCommand(string sql, params NpgsqlParameter[] parameters)
    {
        return ExecuteSqlQueryCommand(sql, new List<NpgsqlParameter>(parameters));
    }

    protected DataTable ExecuteSqlQueryCommand(string sql, List<NpgsqlParameter> parameters)
    {
        IDbService dbService = Context.Services.GetService<IDbService>();
        using (var dbCon = dbService.CreateConnection())
        {
            NpgsqlCommand cmd = dbCon.CreateCommand(sql, CommandType.Text, parameters);
            DataTable dataTable = new DataTable();
            new NpgsqlDataAdapter(cmd).Fill(dataTable);
            return dataTable;
        }
    }

}


