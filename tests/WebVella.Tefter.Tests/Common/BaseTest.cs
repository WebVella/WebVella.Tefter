namespace WebVella.Tefter.Tests.Common;

public class BaseTest
{
    protected static readonly AsyncLock locker = new AsyncLock();

    public TestContext Context { get; }

    public ServiceProvider ServiceProvider { get; }

    public BaseTest()
    {
        Context = new TestContext();
		Context.Services.AddTefterDI();
        Context.Services.AddSingleton<ILogger, DebugLogger>();
        ServiceProvider = Context.Services.BuildServiceProvider();
    }

    protected DataTable ExecuteSqlQueryCommand(string sql, params NpgsqlParameter[] parameters)
    {
        return ExecuteSqlQueryCommand(sql, new List<NpgsqlParameter>(parameters));
    }

    protected DataTable ExecuteSqlQueryCommand(string sql, List<NpgsqlParameter> parameters)
    {
        IDatabaseService dbService = Context.Services.GetService<IDatabaseService>();
        using (var dbCon = dbService.CreateConnection())
        {
            NpgsqlCommand cmd = dbCon.CreateCommand(sql, CommandType.Text, parameters);
            DataTable dataTable = new DataTable();
            new NpgsqlDataAdapter(cmd).Fill(dataTable);
            return dataTable;
        }
    }

}


