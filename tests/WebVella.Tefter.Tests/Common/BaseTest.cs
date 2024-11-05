using FluentAssertions.Common;

namespace WebVella.Tefter.Tests.Common;

public class BaseTest
{
    protected static readonly AsyncLock locker = new AsyncLock();

    public static TestContext Context { get; }

    public static ServiceProvider ServiceProvider { get; }

	//this method will be executed once and will update
	//tefter test database up to latest migration
    static BaseTest()
    {
        Context = new TestContext();
		Context.Services.AddTefter();
		Context.Services.AddSingleton<TfDataProviderSynchronizeJob>();
		ServiceProvider = Context.Services.BuildServiceProvider();
		ServiceProvider.UseTefter();
    }

    protected DataTable ExecuteSqlQueryCommand(string sql, params NpgsqlParameter[] parameters)
    {
        return ExecuteSqlQueryCommand(sql, new List<NpgsqlParameter>(parameters));
    }

    protected DataTable ExecuteSqlQueryCommand(string sql, List<NpgsqlParameter> parameters)
    {
        ITfDatabaseService dbService = Context.Services.GetService<ITfDatabaseService>();
        using (var dbCon = dbService.CreateConnection())
        {
            NpgsqlCommand cmd = dbCon.CreateCommand(sql, CommandType.Text, parameters);
            DataTable dataTable = new DataTable();
            new NpgsqlDataAdapter(cmd).Fill(dataTable);
            return dataTable;
        }
    }
}

