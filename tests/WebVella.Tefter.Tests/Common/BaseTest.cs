using Bogus;
using FluentAssertions.Common;
using WebVella.Tefter.Jobs;

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

	public async static Task<(TfDataProvider, TfSpaceData)> CreateTestStructureAndData(
		ServiceProvider serviceProvider,
		ITfDatabaseService dbService)
	{
		ITfService tfService = serviceProvider.GetService<ITfService>();
		ITfMetaService tfMetaService = serviceProvider.GetService<ITfMetaService>();
		TfDataProviderSynchronizeJob backgroundSync = serviceProvider.GetRequiredService<TfDataProviderSynchronizeJob>();

		var providerTypes = tfMetaService.GetDataProviderTypes();
		var providerType = providerTypes
			.Single(x => x.AddonId == new Guid("90b7de99-4f7f-4a31-bcf9-9be988739d2d"));

		TfDataProviderModel providerModel = new TfDataProviderModel
		{
			Id = Guid.NewGuid(),
			Name = "test data provider",
			ProviderType = providerType,
			SettingsJson = null
		};
		var provider = tfService.CreateDataProvider(providerModel);
		provider.Should().BeOfType<TfDataProvider>();

		List<Tuple<string, TfDatabaseColumnType, string>> columns = new List<Tuple<string, TfDatabaseColumnType, string>>();
		columns.Add(new Tuple<string, TfDatabaseColumnType, string>($"dp{provider.Index}_guid_column", TfDatabaseColumnType.Guid, "GUID"));
		columns.Add(new Tuple<string, TfDatabaseColumnType, string>($"dp{provider.Index}_short_text_column", TfDatabaseColumnType.ShortText, "SHORT_TEXT"));
		columns.Add(new Tuple<string, TfDatabaseColumnType, string>($"dp{provider.Index}_text_column", TfDatabaseColumnType.Text, "TEXT"));
		columns.Add(new Tuple<string, TfDatabaseColumnType, string>($"dp{provider.Index}_date_column", TfDatabaseColumnType.DateOnly, "DATE"));
		columns.Add(new Tuple<string, TfDatabaseColumnType, string>($"dp{provider.Index}_datetime_column", TfDatabaseColumnType.DateTime, "DATETIME"));
		columns.Add(new Tuple<string, TfDatabaseColumnType, string>($"dp{provider.Index}_short_int_column", TfDatabaseColumnType.ShortInteger, "SHORT_INTEGER"));
		columns.Add(new Tuple<string, TfDatabaseColumnType, string>($"dp{provider.Index}_int_column", TfDatabaseColumnType.Integer, "INTEGER"));
		columns.Add(new Tuple<string, TfDatabaseColumnType, string>($"dp{provider.Index}_long_int_column", TfDatabaseColumnType.LongInteger, "LONG_INTEGER"));
		columns.Add(new Tuple<string, TfDatabaseColumnType, string>($"dp{provider.Index}_number_column", TfDatabaseColumnType.Number, "NUMBER"));

		foreach (var column in columns)
			CreateProviderColumn(tfService, provider, column.Item1, column.Item2, column.Item3);

		//get provider with new columns
		provider = tfService.GetDataProvider(provider.Id);

		TfDataProviderJoinKey joinKey =
					new TfDataProviderJoinKey
					{
						Id = Guid.NewGuid(),
						Description = "will be used for integer shared column",
						DataProviderId = provider.Id,
						DbName = "join_key_int",
						Columns = new() { provider.Columns.Single(x => x.DbType == TfDatabaseColumnType.Integer) }
					};

		provider = tfService.CreateDataProviderJoinKey(joinKey);
		provider.Should().NotBeNull();

		joinKey =
					new TfDataProviderJoinKey
					{
						Id = Guid.NewGuid(),
						Description = "will be used for short text shared column",
						DataProviderId = provider.Id,
						DbName = "join_key_text",
						Columns = new() { provider.Columns.Single(x => x.DbType == TfDatabaseColumnType.ShortText) }

					};

		provider = tfService.CreateDataProviderJoinKey(joinKey);
		provider.Should().NotBeNull();

		TfSharedColumn sharedColumn1 = new TfSharedColumn
		{
			Id = Guid.NewGuid(),
			DbName = "sc_join_key_text",
			DbType = TfDatabaseColumnType.ShortText,
			IncludeInTableSearch = false,
			JoinKeyDbName = "join_key_text"
		};
		tfService.CreateSharedColumn(sharedColumn1);

		TfSharedColumn sharedColumn2 = new TfSharedColumn
		{
			Id = Guid.NewGuid(),
			DbName = "sc_join_key_int",
			DbType = TfDatabaseColumnType.Integer,
			IncludeInTableSearch = false,
			JoinKeyDbName = "join_key_int"
		};
		tfService.CreateSharedColumn(sharedColumn2);

		//get provider with new shared columns
		provider = tfService.GetDataProvider(provider.Id);

		var createResult = tfService.CreateSynchronizationTask(provider.Id, new TfSynchronizationPolicy());

		await backgroundSync.StartManualProcessTasks();

		//insert data for join keys
		DataTable dt = dbService.ExecuteSqlQueryCommand($"SELECT * FROM dp{provider.Index}");
		foreach (DataRow dr in dt.Rows)
		{
			var textJoinKeyId = (Guid)dr["tf_jk_join_key_text_id"];
			int insertResult = dbService.ExecuteSqlNonQueryCommand($"INSERT INTO tf_shared_column_short_text_value(join_key_id,shared_column_id,value) " +
				$"VALUES ('{textJoinKeyId}','{sharedColumn1.Id}', @value) ", new NpgsqlParameter("value", new Faker("en").Lorem.Sentence()));
			insertResult.Should().Be(1);

			var intJoinKeyId = (Guid)dr["tf_jk_join_key_int_id"];
			insertResult = dbService.ExecuteSqlNonQueryCommand($"INSERT INTO tf_shared_column_integer_value(join_key_id,shared_column_id,value) " +
				$"VALUES ('{textJoinKeyId}','{sharedColumn2.Id}', @value) ", new NpgsqlParameter("value", new Faker("en").Random.Int()));
			insertResult.Should().Be(1);
		}

		var space = new TfSpace
		{
			Id = Guid.NewGuid(),
			Name = "TestSpace",
			Color = 10,
			FluentIconName = "icon1",
			IsPrivate = false,
			Position = 0
		};
		tfService.CreateSpace(space);

		var spaceColumns = columns.Select(x => x.Item1).ToList();
		spaceColumns.Add(sharedColumn1.DbName);
		spaceColumns.Add(sharedColumn2.DbName); //this one will be used for sort to check sort join

		var spaceData = new TfSpaceData
		{
			Id = Guid.NewGuid(),
			DataProviderId = providerModel.Id,
			Name = "TestSpaceData",
			SpaceId = space.Id,
			Columns = spaceColumns
		};

		List<TfFilterBase> filters = new List<TfFilterBase>();

		//spaceData.Filters.Add(new TfFilterNumeric("sc_join_key_int", TfFilterNumericComparisonMethod.Greater, 5));

		var result = tfService.CreateSpaceData(spaceData);
		provider = tfService.GetDataProvider(provider.Id);
		spaceData = tfService.GetSpaceData(spaceData.Id);

		return (provider, spaceData);
	}

	private static void CreateProviderColumn(
		ITfService tfService,
		TfDataProvider provider,
		string dbName,
		TfDatabaseColumnType dbType,
		string sourceType)
	{
		var column = new TfDataProviderColumn
		{
			Id = Guid.NewGuid(),
			AutoDefaultValue = false,
			DefaultValue = null,
			DataProviderId = provider.Id,
			DbName = dbName,
			DbType = dbType,
			SourceName = dbName,
			SourceType = sourceType,
			IncludeInTableSearch = true,
			IsNullable = true,
			IsSearchable = true,
			IsSortable = false,
			IsUnique = false,
			PreferredSearchType = TfDataProviderColumnSearchType.Contains
		};

		tfService.CreateDataProviderColumn(column);
	}
}

