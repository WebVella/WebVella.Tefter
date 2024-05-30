namespace WebVella.Tefter.Database.Internal;

internal interface IDbService
{
    DbContext CreateContext();
    DbConnection CreateConnection();
    DbTransactionScope CreateTransactionScope(string lockKey = null);
    DataTable ExecuteSqlQueryCommand(string sql, List<NpgsqlParameter> parameters);
    int ExecuteSqlNonQueryCommand(string sql, params NpgsqlParameter[] parameters);
    int ExecuteSqlNonQueryCommand(string sql, List<NpgsqlParameter> parameters);
    ValueTask<DataTable> ExecuteSqlQueryCommandAsync(string sql, params NpgsqlParameter[] parameters);
    ValueTask<DataTable> ExecuteSqlQueryCommandAsync(string sql, List<NpgsqlParameter> parameters);
    ValueTask<int> ExecuteSqlNonQueryCommandAsync(string sql, params NpgsqlParameter[] parameters);
    ValueTask<int> ExecuteSqlNonQueryCommandAsync(string sql, List<NpgsqlParameter> parameters);
}


internal class DbService
{
    private ITransactionRollbackNotifyService _tranRNS = null;
    public DbConfiguration Configuration { get; private set; }

    public DbService(DbConfiguration configuration, ITransactionRollbackNotifyService tranRNS)
    {
        _tranRNS = tranRNS;
        Configuration = configuration;
    }

    public DbContext CreateContext()
    {
        return DbContext.CreateContext(Configuration);
    }

    public DbConnection CreateConnection()
    {
        if (DbContext.Current != null)
            return DbContext.Current.CreateConnection();
        else
            return DbContext.CreateContext(Configuration).CreateConnection();
    }

    public DbTransactionScope CreateTransactionScope(string lockKey = null)
    {
        return new DbTransactionScope(Configuration, _tranRNS, lockKey);
    }


    public DataTable ExecuteSqlQueryCommand(string sql, List<NpgsqlParameter> parameters)
    {
        ProcessNpgsqlParameters(parameters);
        using (var dbCon = CreateConnection())
        {
            NpgsqlCommand cmd;
            if (parameters != null && parameters.Count > 0)
                cmd = dbCon.CreateCommand(sql, CommandType.Text, parameters);
            else
                cmd = dbCon.CreateCommand(sql, CommandType.Text);
            DataTable dataTable = new DataTable();
            new NpgsqlDataAdapter(cmd).Fill(dataTable);
            return dataTable;
        }
    }

    public int ExecuteSqlNonQueryCommand(string sql, params NpgsqlParameter[] parameters)
    {
        return ExecuteSqlNonQueryCommand(sql, new List<NpgsqlParameter>(parameters));
    }

    public int ExecuteSqlNonQueryCommand(string sql, List<NpgsqlParameter> parameters)
    {
        ProcessNpgsqlParameters(parameters);
        using (var dbCon = CreateConnection())
        {
            return dbCon.CreateCommand(sql, CommandType.Text, parameters).ExecuteNonQuery();
        }
    }

    public async ValueTask<DataTable> ExecuteSqlQueryCommandAsync(string sql, params NpgsqlParameter[] parameters)
    {
        return await ExecuteSqlQueryCommandAsync(sql, new List<NpgsqlParameter>(parameters));
    }

#pragma warning disable 1998
    public async ValueTask<DataTable> ExecuteSqlQueryCommandAsync(string sql, List<NpgsqlParameter> parameters)
    {
        ProcessNpgsqlParameters(parameters);
        //we are not using postsgres driver for async operation because of transaction wrapper library
        using (var dbCon = CreateConnection())
        {
            NpgsqlCommand cmd = dbCon.CreateCommand(sql, CommandType.Text, parameters);
            DataTable dataTable = new DataTable();
            var reader = cmd.ExecuteReader();
            dataTable.Load(reader);
            return dataTable;
        }
    }
#pragma warning restore 1998

    public async ValueTask<int> ExecuteSqlNonQueryCommandAsync(string sql, params NpgsqlParameter[] parameters)
    {
        return await ExecuteSqlNonQueryCommandAsync(sql, new List<NpgsqlParameter>(parameters));
    }

#pragma warning disable 1998
    public async ValueTask<int> ExecuteSqlNonQueryCommandAsync(string sql, List<NpgsqlParameter> parameters)
    {

        ProcessNpgsqlParameters(parameters);
        //we are not using postsgres driver for async operation because of transaction wrapper library
        using (var dbCon = CreateConnection())
        {
            var affectedRows = dbCon.CreateCommand(sql, CommandType.Text, parameters).ExecuteNonQuery();
            return affectedRows;
        }

    }
#pragma warning restore 1998

    private void ProcessNpgsqlParameters(params NpgsqlParameter[] parameters)
    {
        if (parameters == null)
            return;

        ProcessNpgsqlParameters(parameters.ToArray());
    }

    private void ProcessNpgsqlParameters(List<NpgsqlParameter> parameters)
    {
        if (parameters == null)
            return;

        foreach (var par in parameters)
        {
            if (par.DbType == System.Data.DbType.DateTime)
            {
                DateTime? value = (DateTime?)par.Value;
                if (value.HasValue)
                {
                    switch (value.Value.Kind)
                    {
                        case DateTimeKind.Unspecified:
                            par.Value = DateTime.SpecifyKind(value.Value, DateTimeKind.Local);
                            break;
                        case DateTimeKind.Utc:
                            par.Value = value.Value.ToLocalTime();
                            break;
                        default:
                            break;
                    }
                }
            }
            else if (par.DbType == System.Data.DbType.DateTime2)
            {
                DateTime? value = (DateTime?)par.Value;
                if (value.HasValue)
                {
                    switch (value.Value.Kind)
                    {
                        case DateTimeKind.Unspecified:
                            par.Value = DateTime.SpecifyKind(value.Value, DateTimeKind.Local);
                            break;
                        case DateTimeKind.Utc:
                            par.Value = value.Value.ToLocalTime();
                            break;
                        default:
                            break;
                    }
                }
            }
            else if (par.DbType == System.Data.DbType.DateTimeOffset)
            {
                DateTimeOffset? value = (DateTimeOffset?)par.Value;
                if (value.HasValue)
                    par.Value = value.Value.ToLocalTime();
            }
        }
    }
}