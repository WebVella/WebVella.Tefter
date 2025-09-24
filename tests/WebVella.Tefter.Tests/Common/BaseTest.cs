using Bogus;
using FluentAssertions.Common;
using Microsoft.Extensions.DependencyInjection;
using System;
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

		const string systemUserEmail = "unittestuser@test.bg";
		ITfService tfService = ServiceProvider.GetService<ITfService>();
		var user = tfService.GetUser(systemUserEmail);
		if(user == null)
		{
			var adminRole = tfService.GetRole(TfConstants.ADMIN_ROLE_ID);

			user = tfService
				.CreateUserBuilder()
				.WithEmail(systemUserEmail)
				.WithPassword("Password")
				.WithId(Guid.NewGuid())
				.WithFirstName("Unit")
				.WithLastName("Test")
				.WithRoles(new TfRole[] { adminRole })
				.Enabled(true)
				.Build();

			user = tfService.CreateUser(user);
			Assert.True(user!= null);
		}
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

	public async static Task<(TfDataProvider, TfDataSet)> CreateTestStructureAndData(
		ServiceProvider serviceProvider,
		ITfDatabaseService dbService)
	{
		ITfService tfService = serviceProvider.GetService<ITfService>();
		ITfMetaService tfMetaService = serviceProvider.GetService<ITfMetaService>();
		TfDataProviderSynchronizeJob backgroundSync = serviceProvider.GetRequiredService<TfDataProviderSynchronizeJob>();

		var providerTypes = tfMetaService.GetDataProviderTypes();
		var providerType = providerTypes
			.Single(x => x.AddonId == new Guid("90b7de99-4f7f-4a31-bcf9-9be988739d2d"));

		TfCreateDataProvider providerModel = new TfCreateDataProvider
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
		{
			CreateProviderColumn(tfService, provider, column.Item1, column.Item2, column.Item3,sourceName: column.Item1);
		}

		//create columns with no source and unique to test unique generation
		CreateProviderColumn(tfService, provider, $"dp{provider.Index}_guid_column_unique", TfDatabaseColumnType.Guid, "GUID", sourceName:null,isUnique:true);
		CreateProviderColumn(tfService, provider, $"dp{provider.Index}_short_text_column_unique", TfDatabaseColumnType.ShortText, "SHORT_TEXT", sourceName: null, isUnique: true);
		CreateProviderColumn(tfService, provider, $"dp{provider.Index}_text_column_unique", TfDatabaseColumnType.Text, "TEXT", sourceName: null, isUnique: true);
		CreateProviderColumn(tfService, provider, $"dp{provider.Index}_date_column_unique", TfDatabaseColumnType.DateOnly, "DATE", sourceName: null, isUnique: true);
		CreateProviderColumn(tfService, provider, $"dp{provider.Index}_datetime_column_unique", TfDatabaseColumnType.DateTime, "DATETIME", sourceName: null, isUnique: true);
		CreateProviderColumn(tfService, provider, $"dp{provider.Index}_short_int_column_unique", TfDatabaseColumnType.ShortInteger, "SHORT_INTEGER", sourceName: null, isUnique: true);
		CreateProviderColumn(tfService, provider, $"dp{provider.Index}_int_column_unique", TfDatabaseColumnType.Integer, "INTEGER", sourceName: null, isUnique: true);
		CreateProviderColumn(tfService, provider, $"dp{provider.Index}_long_int_column_unique", TfDatabaseColumnType.LongInteger, "LONG_INTEGER", sourceName: null, isUnique: true);
		CreateProviderColumn(tfService, provider, $"dp{provider.Index}_number_column_unique", TfDatabaseColumnType.Number, "NUMBER", sourceName: null, isUnique: true);


		//get provider with new columns
		provider = tfService.GetDataProvider(provider.Id);

		//create data identity
		TfDataIdentity dataIdentityModel1 = new TfDataIdentity
		{
			DataIdentity = "test_data_identity_1",
			Label = "Test Data Identity 1",
		};
		TfDataIdentity dataIdentity1 = tfService.CreateDataIdentity(dataIdentityModel1);
		dataIdentity1.Should().NotBeNull();

		TfDataIdentity dataIdentityModel2 = new TfDataIdentity
		{
			DataIdentity = "test_data_identity_2",
			Label = "Test Data Identity 2",
		};
		TfDataIdentity dataIdentity2 = tfService.CreateDataIdentity(dataIdentityModel2);
		dataIdentity2.Should().NotBeNull();

		//create provider data identity link
		TfDataProviderIdentity providerDataIdentity1 =
					new TfDataProviderIdentity
					{
						Id = Guid.NewGuid(),
						DataProviderId = provider.Id,
						DataIdentity = dataIdentityModel1.DataIdentity,
						Columns = new() { $"dp{provider.Index}_text_column" } //text column
					};

		provider = tfService.CreateDataProviderIdentity(providerDataIdentity1);

		TfDataProviderIdentity providerDataIdentity2 =
					new TfDataProviderIdentity
					{
						Id = Guid.NewGuid(),
						DataProviderId = provider.Id,
						DataIdentity = dataIdentityModel2.DataIdentity,
						Columns = new() { $"dp{provider.Index}_int_column" } //int column
					};

		provider = tfService.CreateDataProviderIdentity(providerDataIdentity2);


		TfSharedColumn sharedColumn1 = new TfSharedColumn
		{
			Id = Guid.NewGuid(),
			DbName = "sc_text",
			DbType = TfDatabaseColumnType.ShortText,
			IncludeInTableSearch = false,
			DataIdentity = providerDataIdentity1.DataIdentity
		};
		tfService.CreateSharedColumn(sharedColumn1);

		TfSharedColumn sharedColumn2 = new TfSharedColumn
		{
			Id = Guid.NewGuid(),
			DbName = "sc_int",
			DbType = TfDatabaseColumnType.Integer,
			IncludeInTableSearch = false,
			DataIdentity = providerDataIdentity2.DataIdentity
		};
		tfService.CreateSharedColumn(sharedColumn2);

		TfSharedColumn sharedColumn3 = new TfSharedColumn
		{
			Id = Guid.NewGuid(),
			DbName = "sc_int_row_id",
			DbType = TfDatabaseColumnType.Integer,
			IncludeInTableSearch = false,
			DataIdentity = TfConstants.TF_ROW_ID_DATA_IDENTITY
		};
		tfService.CreateSharedColumn(sharedColumn3);

		//get provider with new shared columns
		provider = tfService.GetDataProvider(provider.Id);

		var createResult = tfService.CreateSynchronizationTask(provider.Id, new TfSynchronizationPolicy());

		await backgroundSync.StartManualProcessTasks();

		HashSet<string> textDataIdentityValues = new HashSet<string>();
		HashSet<string> intDataIdentityValues = new HashSet<string>();

		//insert data for join keys
		DataTable dt = dbService.ExecuteSqlQueryCommand($"SELECT * FROM dp{provider.Index}");

		int sharedColumnIntValueInc = 0;

		foreach (DataRow dr in dt.Rows)
		{
			var textDataIdentityValue = (string)dr[$"tf_ide_{dataIdentity1.DataIdentity}"];
			
			if (textDataIdentityValues.Contains(textDataIdentityValue))
				continue; //skip if already exists

			textDataIdentityValues.Add(textDataIdentityValue);

			int insertResult = dbService.ExecuteSqlNonQueryCommand($"INSERT INTO tf_shared_column_short_text_value(data_identity_value,shared_column_id,value) " +
				$"VALUES ('{textDataIdentityValue}','{sharedColumn1.Id}', @value) ", new NpgsqlParameter("value", new Faker("en").Lorem.Sentence()));
			insertResult.Should().Be(1);

			var intDataIdentityValue = (string)dr[$"tf_ide_{dataIdentity2.DataIdentity}"];

			if (intDataIdentityValues.Contains(intDataIdentityValue))
				continue; //skip if already exists

			intDataIdentityValues.Add(intDataIdentityValue);

			insertResult = dbService.ExecuteSqlNonQueryCommand($"INSERT INTO tf_shared_column_integer_value(data_identity_value,shared_column_id,value) " +
				$"VALUES ('{intDataIdentityValue}','{sharedColumn2.Id}', @value) ", new NpgsqlParameter("value", sharedColumnIntValueInc /*new Faker("en").Random.Int()*/));
			insertResult.Should().Be(1);

			sharedColumnIntValueInc++;
		}

		var space = new TfSpace
		{
			Id = Guid.NewGuid(),
			Name = "TestSpace",
			Color = TfColor.Amber100,
			FluentIconName = "icon1",
			IsPrivate = false,
			Position = 0
		};
		tfService.CreateSpace(space);

		var spaceColumns = columns.Select(x => x.Item1).ToList();
		

		var spaceData = new TfCreateDataSet
		{
			Id = Guid.NewGuid(),
			DataProviderId = providerModel.Id,
			Name = "TestSpaceData",
			Columns = spaceColumns
		};

		List<TfFilterBase> filters = new List<TfFilterBase>();

		//spaceData.Filters.Add(new TfFilterNumeric("sc_join_key_int", TfFilterNumericComparisonMethod.Greater, 5));

		var result = tfService.CreateDataSet(spaceData);
		provider = tfService.GetDataProvider(provider.Id);
		var createdSpaceData = tfService.GetDataSet(spaceData.Id);

		tfService.CreateDataSetIdentity(new TfDataSetIdentity
		{
			Id = Guid.NewGuid(),
			DataSetId = spaceData.Id,
			Columns = new List<string> { sharedColumn1.DbName },
			DataIdentity = sharedColumn1!.DataIdentity,
		});

		tfService.CreateDataSetIdentity(new TfDataSetIdentity
		{
			Id = Guid.NewGuid(),
			DataSetId = spaceData.Id,
			Columns = new List<string> { sharedColumn2.DbName },
			DataIdentity = sharedColumn2!.DataIdentity,
		});

		tfService.CreateDataSetIdentity(new TfDataSetIdentity
		{
			Id = Guid.NewGuid(),
			DataSetId = spaceData.Id,
			Columns = new List<string> { sharedColumn3.DbName },
			DataIdentity = sharedColumn3!.DataIdentity,
		});

		createdSpaceData = tfService.GetDataSet(spaceData.Id);

		return (provider, createdSpaceData);
	}

	private static void CreateProviderColumn(
		ITfService tfService,
		TfDataProvider provider,
		string dbName,
		TfDatabaseColumnType dbType,
		string sourceType,
		string sourceName,
		bool? isUnique = null)
	{
		var column = new TfDataProviderColumn
		{
			Id = Guid.NewGuid(),
			AutoDefaultValue = false,
			DefaultValue = null,
			DataProviderId = provider.Id,
			DbName = dbName,
			DbType = dbType,
			SourceName = sourceName,
			SourceType = sourceType,
			IncludeInTableSearch = true,
			IsNullable = true,
			IsSearchable = true,
			IsSortable = false,
			IsUnique = isUnique??false,
			PreferredSearchType = TfDataProviderColumnSearchType.Contains
		};

		tfService.CreateDataProviderColumn(column);
	}
}

