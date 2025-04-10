﻿using Bogus;
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
			.Single(x => x.Id == new Guid("90b7de99-4f7f-4a31-bcf9-9be988739d2d"));

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
		columns.Add(new Tuple<string, TfDatabaseColumnType, string>("guid_column", TfDatabaseColumnType.Guid, "GUID"));
		columns.Add(new Tuple<string, TfDatabaseColumnType, string>("short_text_column", TfDatabaseColumnType.ShortText, "SHORT_TEXT"));
		columns.Add(new Tuple<string, TfDatabaseColumnType, string>("text_column", TfDatabaseColumnType.Text, "TEXT"));
		columns.Add(new Tuple<string, TfDatabaseColumnType, string>("date_column", TfDatabaseColumnType.Date, "DATE"));
		columns.Add(new Tuple<string, TfDatabaseColumnType, string>("datetime_column", TfDatabaseColumnType.DateTime, "DATETIME"));
		columns.Add(new Tuple<string, TfDatabaseColumnType, string>("short_int_column", TfDatabaseColumnType.ShortInteger, "SHORT_INTEGER"));
		columns.Add(new Tuple<string, TfDatabaseColumnType, string>("int_column", TfDatabaseColumnType.Integer, "INTEGER"));
		columns.Add(new Tuple<string, TfDatabaseColumnType, string>("long_int_column", TfDatabaseColumnType.LongInteger, "LONG_INTEGER"));
		columns.Add(new Tuple<string, TfDatabaseColumnType, string>("number_column", TfDatabaseColumnType.Number, "NUMBER"));

		foreach (var column in columns)
			CreateProviderColumn(tfService, provider, column.Item1, column.Item2, column.Item3);

		//get provider with new columns
		provider = tfService.GetDataProvider(provider.Id);

		TfDataProviderSharedKey sharedKey =
					new TfDataProviderSharedKey
					{
						Id = Guid.NewGuid(),
						Description = "will be used for integer shared column",
						DataProviderId = provider.Id,
						DbName = "shared_key_int",
						Columns = new() { provider.Columns.Single(x => x.DbType == TfDatabaseColumnType.Integer) }
					};

		provider = tfService.CreateDataProviderSharedKey(sharedKey);
		provider.Should().NotBeNull();

		sharedKey =
					new TfDataProviderSharedKey
					{
						Id = Guid.NewGuid(),
						Description = "will be used for short text shared column",
						DataProviderId = provider.Id,
						DbName = "shared_key_text",
						Columns = new() { provider.Columns.Single(x => x.DbType == TfDatabaseColumnType.ShortText) }

					};

		provider = tfService.CreateDataProviderSharedKey(sharedKey);
		provider.Should().NotBeNull();

		TfSharedColumn sharedColumn1 = new TfSharedColumn
		{
			Id = Guid.NewGuid(),
			DbName = "sk_shared_key_text",
			DbType = TfDatabaseColumnType.ShortText,
			IncludeInTableSearch = false,
			SharedKeyDbName = "shared_key_text"
		};
		tfService.CreateSharedColumn(sharedColumn1);

		TfSharedColumn sharedColumn2 = new TfSharedColumn
		{
			Id = Guid.NewGuid(),
			DbName = "sk_shared_key_int",
			DbType = TfDatabaseColumnType.Integer,
			IncludeInTableSearch = false,
			SharedKeyDbName = "shared_key_int"
		};
		tfService.CreateSharedColumn(sharedColumn2);

		//get provider with new shared columns
		provider = tfService.GetDataProvider(provider.Id);

		var createResult = tfService.CreateSynchronizationTask(provider.Id, new TfSynchronizationPolicy());

		await backgroundSync.StartManualProcessTasks();

		//insert data for shared keys
		DataTable dt = dbService.ExecuteSqlQueryCommand($"SELECT * FROM dp{provider.Index}");
		foreach (DataRow dr in dt.Rows)
		{
			var textSharedKeyId = (Guid)dr["tf_sk_shared_key_text_id"];
			int insertResult = dbService.ExecuteSqlNonQueryCommand($"INSERT INTO shared_column_short_text_value(shared_key_id,shared_column_id,value) " +
				$"VALUES ('{textSharedKeyId}','{sharedColumn1.Id}', @value) ", new NpgsqlParameter("value", new Faker("en").Lorem.Sentence()));
			insertResult.Should().Be(1);

			var intSharedKeyId = (Guid)dr["tf_sk_shared_key_int_id"];
			insertResult = dbService.ExecuteSqlNonQueryCommand($"INSERT INTO shared_column_integer_value(shared_key_id,shared_column_id,value) " +
				$"VALUES ('{textSharedKeyId}','{sharedColumn2.Id}', @value) ", new NpgsqlParameter("value", new Faker("en").Random.Int()));
			insertResult.Should().Be(1);
		}

		var space = new TfSpace
		{
			Id = Guid.NewGuid(),
			Name = "TestSpace",
			Color = 10,
			Icon = "icon1",
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

		//spaceData.Filters.Add(new TfFilterNumeric("sk_shared_key_int", TfFilterNumericComparisonMethod.Greater, 5));

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

