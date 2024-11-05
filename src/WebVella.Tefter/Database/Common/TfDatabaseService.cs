namespace WebVella.Tefter.Database;

public interface ITfDatabaseService
{
	TfDatabaseContext CreateContext();
	TfDatabaseConnection CreateConnection();
	TfDatabaseTransactionScope CreateTransactionScope(string lockKey = null);
	DataTable ExecuteSqlQueryCommand(string sql, List<NpgsqlParameter> parameters);
	DataTable ExecuteSqlQueryCommand(string sql, params NpgsqlParameter[] parameters);
	int ExecuteSqlNonQueryCommand(string sql, params NpgsqlParameter[] parameters);
	int ExecuteSqlNonQueryCommand(string sql, List<NpgsqlParameter> parameters);
	Task<DataTable> ExecuteSqlQueryCommandAsync(string sql, params NpgsqlParameter[] parameters);
	Task<DataTable> ExecuteSqlQueryCommandAsync(string sql, List<NpgsqlParameter> parameters);
	Task<int> ExecuteSqlNonQueryCommandAsync(string sql, params NpgsqlParameter[] parameters);
	Task<int> ExecuteSqlNonQueryCommandAsync(string sql, List<NpgsqlParameter> parameters);
}


public class TfDatabaseService : ITfDatabaseService
{
	private ITfTransactionRollbackNotifyService _tranRNS = null;
	public ITfDbConfigurationService Configuration { get; private set; }

	public TfDatabaseService(ITfDbConfigurationService configuration, ITfTransactionRollbackNotifyService tranRNS)
	{
		_tranRNS = tranRNS;
		Configuration = configuration;
	}

	public TfDatabaseContext CreateContext()
	{
		return TfDatabaseContext.CreateContext(Configuration);
	}

	public TfDatabaseConnection CreateConnection()
	{
		if (TfDatabaseContext.Current != null)
			return TfDatabaseContext.Current.CreateConnection();
		else
			return TfDatabaseContext.CreateContext(Configuration).CreateConnection();
	}

	public TfDatabaseTransactionScope CreateTransactionScope(string lockKey = null)
	{
		return new TfDatabaseTransactionScope(Configuration, _tranRNS, lockKey);
	}

	public DataTable ExecuteSqlQueryCommand(string sql, List<NpgsqlParameter> parameters)
	{
		return ExecuteSqlQueryCommand(sql, parameters?.ToArray());
	}

	public DataTable ExecuteSqlQueryCommand(string sql, params NpgsqlParameter[] parameters)
	{
		ProcessNpgsqlParameters(parameters);
		using (var dbCon = CreateConnection())
		{
			NpgsqlCommand cmd;
			if (parameters != null && parameters.Length > 0)
				cmd = dbCon.CreateCommand(sql, CommandType.Text, new List<NpgsqlParameter>(parameters));
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

	public async Task<DataTable> ExecuteSqlQueryCommandAsync(string sql, params NpgsqlParameter[] parameters)
	{
		return await ExecuteSqlQueryCommandAsync(sql, new List<NpgsqlParameter>(parameters));
	}

#pragma warning disable 1998
	public async Task<DataTable> ExecuteSqlQueryCommandAsync(string sql, List<NpgsqlParameter> parameters)
	{
		ProcessNpgsqlParameters(parameters);
		//we are not using postgres driver for async operation because of transaction wrapper library
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

	public async Task<int> ExecuteSqlNonQueryCommandAsync(string sql, params NpgsqlParameter[] parameters)
	{
		return await ExecuteSqlNonQueryCommandAsync(sql, new List<NpgsqlParameter>(parameters));
	}

#pragma warning disable 1998
	public async Task<int> ExecuteSqlNonQueryCommandAsync(string sql, List<NpgsqlParameter> parameters)
	{

		ProcessNpgsqlParameters(parameters);
		//we are not using postgres driver for async operation because of transaction wrapper library
		using (var dbCon = CreateConnection())
		{
			var affectedRows = dbCon.CreateCommand(sql, CommandType.Text, parameters).ExecuteNonQuery();
			return affectedRows;
		}

	}
#pragma warning restore 1998

	private void ProcessNpgsqlParameters(params NpgsqlParameter[] parameters)
	{
		if (parameters == null || parameters.Length == 0)
			return;

		ProcessNpgsqlParameters(parameters.ToList());
	}

	private void ProcessNpgsqlParameters(List<NpgsqlParameter> parameters)
	{
		//if (parameters == null)
		//	return;

		//foreach (var par in parameters)
		//{
		//	if (par.Value == DBNull.Value)
		//		continue;

		//	if (par.DbType == System.Data.DbType.DateTime)
		//	{
		//		DateTime? value = (DateTime?)par.Value;
		//		if (value.HasValue)
		//		{
		//			switch (value.Value.Kind)
		//			{
		//				case DateTimeKind.Unspecified:
		//					par.Value = DateTime.SpecifyKind(value.Value, DateTimeKind.Local);
		//					break;
		//				case DateTimeKind.Utc:
		//					par.Value = value.Value.ToLocalTime();
		//					break;
		//				default:
		//					break;
		//			}
		//		}
		//	}
		//	else if (par.DbType == System.Data.DbType.DateTime2)
		//	{
		//		DateTime? value = (DateTime?)par.Value;
		//		if (value.HasValue)
		//		{
		//			switch (value.Value.Kind)
		//			{
		//				case DateTimeKind.Unspecified:
		//					par.Value = DateTime.SpecifyKind(value.Value, DateTimeKind.Local);
		//					break;
		//				case DateTimeKind.Utc:
		//					par.Value = value.Value.ToLocalTime();
		//					break;
		//				default:
		//					break;
		//			}
		//		}
		//	}
		//	else if (par.DbType == System.Data.DbType.DateTimeOffset)
		//	{
		//		DateTimeOffset? value = (DateTimeOffset?)par.Value;
		//		if (value.HasValue)
		//			par.Value = value.Value.ToLocalTime();
		//	}
		//}
	}
}