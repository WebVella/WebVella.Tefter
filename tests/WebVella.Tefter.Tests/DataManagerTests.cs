using Bogus;
using WebVella.Tefter.Api;
using WebVella.Tefter.Models;

namespace WebVella.Tefter.Tests;

public partial class DataManagerTests : BaseTest
{

	[Fact]
	public async Task SpaceData_DebugInsertUpdateTableTest()
	{
		using (await locker.LockAsync())
		{
			var faker = new Faker("en");
			IDatabaseService dbService = ServiceProvider.GetRequiredService<IDatabaseService>();
			IDataManager dataManager = ServiceProvider.GetRequiredService<IDataManager>();
			ITfDataProviderManager providerManager = ServiceProvider.GetRequiredService<ITfDataProviderManager>();

			using (var scope = dbService.CreateTransactionScope(Constants.DB_OPERATION_LOCK_KEY))
			{
				var (provider, spaceData) = CreateTestStructureAndData(dbService);
				//var result = dataManager.QuerySpaceData(spaceData.Id);
				var result = dataManager.QueryDataProvider(provider);
				result.IsSuccess.Should().BeTrue();

				dataManager.DeleteAllProviderRows(provider);

				var newTable = result.Value.NewTable();

				for (var i = 0; i < 10; i++)
				{
					var newRow = newTable.NewRow();

					newRow["guid_column"] = Guid.NewGuid();
					newRow["short_text_column"] = i + faker.Lorem.Sentence();
					newRow["text_column"] =  i + faker.Lorem.Lines();
					newRow["date_column"] = faker.Date.PastDateOnly(); 
					newRow["datetime_column"] = faker.Date.Future();
					newRow["short_int_column"] = faker.Random.Short(0, 100);
					newRow["int_column"] = faker.Random.Number(100, 1000);
					newRow["long_int_column"] = faker.Random.Long(1000, 10000);
					newRow["number_column"] = faker.Random.Decimal(100000, 1000000);

					newRow["sk_shared_key_text"] = "this is shared key text test " + i;
					newRow["sk_shared_key_int"] =  i;

					newTable.Rows.Add(newRow);
				}

				result = dataManager.SaveDataTable(newTable);
				result.IsSuccess.Should().BeTrue();

				//result = dataManager.QuerySpaceData(spaceData.Id);
				result = dataManager.QueryDataProvider(provider);
				result.IsSuccess.Should().BeTrue();

				var tableToUpdate = result.Value;

				for (var i = 0; i < 10; i++)
				{
					var row = tableToUpdate.Rows[i];

					row["guid_column"] = Guid.NewGuid();
					row["short_text_column"] = i + faker.Lorem.Sentence() + "upd";
					row["text_column"] = i + faker.Lorem.Lines() + "upd";
					row["date_column"] = faker.Date.PastDateOnly();
					row["datetime_column"] = faker.Date.Future();
					row["short_int_column"] = faker.Random.Short(0, 100);
					row["int_column"] = faker.Random.Number(100, 1000);
					row["long_int_column"] = faker.Random.Long(1000, 10000);
					row["number_column"] = faker.Random.Decimal(100000, 1000000);

					row["sk_shared_key_text"] = "this is shared key text test " + i + "update";
					row["sk_shared_key_int"] = i + i;
				}

				result = dataManager.SaveDataTable(tableToUpdate);
				result.IsSuccess.Should().BeTrue();

				//result = dataManager.QuerySpaceData(spaceData.Id);
				result = dataManager.QueryDataProvider(provider);
				result.IsSuccess.Should().BeTrue();

				tableToUpdate = result.Value;

				for (var i = 0; i < 10; i++)
				{
					var row = tableToUpdate.Rows[i];

					row["guid_column"] = null;
					row["short_text_column"] = null;
					row["text_column"] = null;
					row["date_column"] = null;
					row["datetime_column"] = null;
					row["short_int_column"] = null;
					row["int_column"] = null;
					row["long_int_column"] = null;
					row["number_column"] = null;

					row["sk_shared_key_text"] = null;
					row["sk_shared_key_int"] = null;
				}

				result = dataManager.SaveDataTable(tableToUpdate);
				result.IsSuccess.Should().BeTrue();

				//result = dataManager.QuerySpaceData(spaceData.Id);
				result = dataManager.QueryDataProvider(provider);
				result.IsSuccess.Should().BeTrue();
			}
		}
	}

	[Fact]
	public async Task SpaceData_DebugTest()
	{
		using (await locker.LockAsync())
		{
			IDatabaseService dbService = ServiceProvider.GetRequiredService<IDatabaseService>();
			IDataManager dataManager = ServiceProvider.GetRequiredService<IDataManager>();

			using (var scope = dbService.CreateTransactionScope(Constants.DB_OPERATION_LOCK_KEY))
			{
				var (provider, spaceData) = CreateTestStructureAndData(dbService);
				var result = dataManager.QuerySpaceData(spaceData.Id,
					noRows: true,
					search: "10",
					page: 1,
					pageSize: 5,
					additionalFilters: new List<TfFilterBase>
					{
						new TfFilterOr(new[]
							{
								(TfFilterBase)new TfFilterText("short_text_column", TfFilterTextComparisonMethod.Contains, "b"),
								(TfFilterBase)new TfFilterText("short_text_column", TfFilterTextComparisonMethod.Fts, "a"),
								(TfFilterBase)new TfFilterText("sk_shared_key_text", TfFilterTextComparisonMethod.Contains, "a"),
								(TfFilterBase)new TfFilterNumeric("sk_shared_key_int", TfFilterNumericComparisonMethod.Equal, 5 )
							})

					},
					sortOverrides: new List<TfSort> {
						new TfSort {
							DbName ="missing_column",
							Direction=TfSortDirection.DESC} ,
						new TfSort {
							DbName ="guid_column",
							Direction=TfSortDirection.DESC} ,
						new TfSort {
							DbName ="sk_shared_key_int",
							Direction=TfSortDirection.ASC}
					});

			}
		}
	}

	private (TfDataProvider, TfSpaceData) CreateTestStructureAndData(
		IDatabaseService dbService)
	{
		ITfSpaceManager spaceManager = ServiceProvider.GetRequiredService<ITfSpaceManager>();
		IDataManager dataManager = ServiceProvider.GetRequiredService<IDataManager>();
		ITfDataProviderManager providerManager = ServiceProvider.GetRequiredService<ITfDataProviderManager>();
		TfDataProviderSynchronizeJob backgroundSync = ServiceProvider.GetRequiredService<TfDataProviderSynchronizeJob>();
		ITfSharedColumnsManager sharedColumnManager = ServiceProvider.GetRequiredService<ITfSharedColumnsManager>();

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
			AddonId = null,
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
			AddonId = null,
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


		//testing update value
		//foreach (DataRow dr in dt.Rows)
		//{
		//	foreach(var column in provider.Columns)
		//	{
		//		Guid id = (Guid)dr["tf_id"];
		//		var value = dr[column.DbName];
		//		var updateResult = dataManager.UpdateValue(provider, id, column.DbName, value);
		//		updateResult.IsSuccess.Should().BeTrue();	
		//	}
			
		//}

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

	private void CreateProviderColumn(
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


	[Fact]
	public async Task CRUD_ID()
	{
		using (await locker.LockAsync())
		{
			IDatabaseService dbService = ServiceProvider.GetRequiredService<IDatabaseService>();
			IIdentityManager identityManager = ServiceProvider.GetRequiredService<IIdentityManager>();
			IDataManager dataManager = ServiceProvider.GetRequiredService<IDataManager>();

			using (var scope = dbService.CreateTransactionScope(Constants.DB_OPERATION_LOCK_KEY))
			{
				var idResult = dataManager.GetId("test");
				idResult.Should().NotBeNull();
				idResult.IsSuccess.Should().BeTrue();

				var idResult2 = dataManager.GetId("test");
				idResult2.Should().NotBeNull();
				idResult2.IsSuccess.Should().BeTrue();

				idResult.Value.Should().Be(idResult2.Value);

				var id = Guid.NewGuid();


				var idResult6 = dataManager.GetId(id);
				idResult6.Should().NotBeNull();
				idResult6.IsSuccess.Should().BeTrue();
				idResult6.Value.Should().Be(id);
			}
		}
	}

}