using Bogus;
using WebVella.Tefter.Api;

namespace WebVella.Tefter.Tests;

internal class SpaceEnvUtility
{
	public static (TfDataProvider, TfSpaceData) CreateTestStructureAndData(
		ServiceProvider serviceProvider,
		IDatabaseService dbService)
	{
		ITfSpaceManager spaceManager = serviceProvider.GetRequiredService<ITfSpaceManager>();
		IDataManager dataManager = serviceProvider.GetRequiredService<IDataManager>();
		ITfDataProviderManager providerManager = serviceProvider.GetRequiredService<ITfDataProviderManager>();
		TfDataProviderSynchronizeJob backgroundSync = serviceProvider.GetRequiredService<TfDataProviderSynchronizeJob>();
		ITfSharedColumnsManager sharedColumnManager = serviceProvider.GetRequiredService<ITfSharedColumnsManager>();

		var providerTypesResult = providerManager.GetProviderTypes();
		var providerType = providerTypesResult.Value
			.Single(x => x.Id == new Guid("90b7de99-4f7f-4a31-bcf9-9be988739d2d"));

		TfDataProviderModel providerModel = new TfDataProviderModel
		{
			Id = Guid.NewGuid(),
			Name = "test data provider",
			ProviderType = providerType,
			SettingsJson = null
		};
		var providerResult = providerManager.CreateDataProvider(providerModel);
		providerResult.IsSuccess.Should().BeTrue();
		providerResult.Value.Should().BeOfType<TfDataProvider>();

		var provider = providerResult.Value;

		List<Tuple<string, DatabaseColumnType, string>> columns = new List<Tuple<string, DatabaseColumnType, string>>();
		columns.Add(new Tuple<string, DatabaseColumnType, string>("guid_column", DatabaseColumnType.Guid, "GUID"));
		columns.Add(new Tuple<string, DatabaseColumnType, string>("short_text_column", DatabaseColumnType.ShortText, "SHORT_TEXT"));
		columns.Add(new Tuple<string, DatabaseColumnType, string>("text_column", DatabaseColumnType.Text, "TEXT"));
		columns.Add(new Tuple<string, DatabaseColumnType, string>("date_column", DatabaseColumnType.Date, "DATE"));
		columns.Add(new Tuple<string, DatabaseColumnType, string>("datetime_column", DatabaseColumnType.DateTime, "DATETIME"));
		columns.Add(new Tuple<string, DatabaseColumnType, string>("short_int_column", DatabaseColumnType.ShortInteger, "SHORT_INTEGER"));
		columns.Add(new Tuple<string, DatabaseColumnType, string>("int_column", DatabaseColumnType.Integer, "INTEGER"));
		columns.Add(new Tuple<string, DatabaseColumnType, string>("long_int_column", DatabaseColumnType.LongInteger, "LONG_INTEGER"));
		columns.Add(new Tuple<string, DatabaseColumnType, string>("number_column", DatabaseColumnType.Number, "NUMBER"));

		foreach (var column in columns)
			CreateProviderColumn(providerManager, provider, column.Item1, column.Item2, column.Item3);

		//get provider with new columns
		provider = providerManager.GetProvider(provider.Id).Value;

		TfDataProviderSharedKey sharedKey =
					new TfDataProviderSharedKey
					{
						Id = Guid.NewGuid(),
						Description = "will be used for integer shared column",
						DataProviderId = provider.Id,
						DbName = "shared_key_int",
						Columns = new() { provider.Columns.Single(x => x.DbType == DatabaseColumnType.Integer) }
					};

		providerResult = providerManager.CreateDataProviderSharedKey(sharedKey);
		providerResult.IsSuccess.Should().BeTrue();

		sharedKey =
					new TfDataProviderSharedKey
					{
						Id = Guid.NewGuid(),
						Description = "will be used for short text shared column",
						DataProviderId = provider.Id,
						DbName = "shared_key_text",
						Columns = new() { provider.Columns.Single(x => x.DbType == DatabaseColumnType.ShortText) }

					};

		providerResult = providerManager.CreateDataProviderSharedKey(sharedKey);
		providerResult.IsSuccess.Should().BeTrue();

		TfSharedColumn sharedColumn1 = new TfSharedColumn
		{
			Id = Guid.NewGuid(),
			DbName = "sk_shared_key_text",
			DbType = DatabaseColumnType.ShortText,
			IncludeInTableSearch = false,
			SharedKeyDbName = "shared_key_text"
		};
		var scResult = sharedColumnManager.CreateSharedColumn(sharedColumn1);
		scResult.IsSuccess.Should().BeTrue();

		TfSharedColumn sharedColumn2 = new TfSharedColumn
		{
			Id = Guid.NewGuid(),
			DbName = "sk_shared_key_int",
			DbType = DatabaseColumnType.Integer,
			IncludeInTableSearch = false,
			SharedKeyDbName = "shared_key_int"
		};
		scResult = sharedColumnManager.CreateSharedColumn(sharedColumn2);
		scResult.IsSuccess.Should().BeTrue();

		//get provider with new shared columns
		provider = providerManager.GetProvider(provider.Id).Value;

		var createResult = providerManager.CreateSynchronizationTask(provider.Id, new TfSynchronizationPolicy());

		backgroundSync.StartManualProcessTasks();

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
		spaceManager.CreateSpace(space).IsSuccess.Should().BeTrue();

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

		var result = spaceManager.CreateSpaceData(spaceData);
		result.IsSuccess.Should().BeTrue();
		result.Value.Should().NotBeNull();

		provider = providerManager.GetProvider(provider.Id).Value;
		spaceData = spaceManager.GetSpaceData(spaceData.Id).Value;

		return (provider, spaceData);
	}

	private static void CreateProviderColumn(
		ITfDataProviderManager providerManager,
		TfDataProvider provider,
		string dbName,
		DatabaseColumnType dbType,
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

		var providerColumnResult = providerManager.CreateDataProviderColumn(column);
		providerColumnResult.IsSuccess.Should().BeTrue();
	}
}
