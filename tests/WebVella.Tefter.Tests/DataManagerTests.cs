using Bogus;
using DocumentFormat.OpenXml.Office2010.Excel;
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
			ITfDatabaseService dbService = ServiceProvider.GetRequiredService<ITfDatabaseService>();
			ITfDataManager dataManager = ServiceProvider.GetRequiredService<ITfDataManager>();
			ITfDataProviderManager providerManager = ServiceProvider.GetRequiredService<ITfDataProviderManager>();

			using (var scope = dbService.CreateTransactionScope(Constants.DB_OPERATION_LOCK_KEY))
			{
				var (provider, spaceData) = await SpaceEnvUtility.CreateTestStructureAndData(ServiceProvider,dbService);
				//var result = dataManager.QuerySpaceData(spaceData.Id);
				var result = dataManager.QueryDataProvider(provider);

				dataManager.DeleteAllProviderRows(provider);

				var newTable = result.NewTable();

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

				Guid? skIntValue = result.Rows[0].GetSharedKeyValue("test");
				Guid? skTextValue = result.Rows[0].GetSharedKeyValue("shared_key_text");

				//result = dataManager.QuerySpaceData(spaceData.Id);
				result = dataManager.QueryDataProvider(provider);

				var tableToUpdate = result;

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
				
				result = dataManager.QueryDataProvider(provider);


				tableToUpdate = result;

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

				result = dataManager.QueryDataProvider(provider);
			}
		}
	}

	[Fact]
	public async Task SpaceData_DebugTest()
	{
		using (await locker.LockAsync())
		{
			ITfDatabaseService dbService = ServiceProvider.GetRequiredService<ITfDatabaseService>();
			ITfDataManager dataManager = ServiceProvider.GetRequiredService<ITfDataManager>();

			using (var scope = dbService.CreateTransactionScope(Constants.DB_OPERATION_LOCK_KEY))
			{
				var (provider, spaceData) = await SpaceEnvUtility.CreateTestStructureAndData(ServiceProvider,dbService);
				var result = dataManager.QuerySpaceData(spaceData.Id,
					noRows: true,
					search: "10",
					page: 1,
					pageSize: 5,
					userFilters: new List<TfFilterBase>
					{
						new TfFilterOr(new[]
							{
								(TfFilterBase)new TfFilterText("short_text_column", TfFilterTextComparisonMethod.Contains, "b"),
								(TfFilterBase)new TfFilterText("short_text_column", TfFilterTextComparisonMethod.Fts, "a"),
								(TfFilterBase)new TfFilterText("sk_shared_key_text", TfFilterTextComparisonMethod.Contains, "a"),
								(TfFilterBase)new TfFilterNumeric("sk_shared_key_int", TfFilterNumericComparisonMethod.Equal, "5" )
							})

					},
					userSorts: new List<TfSort> {
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

	[Fact]
	public async Task SpaceData_QueryOnlyTfIds()
	{
		using (await locker.LockAsync())
		{
			ITfDatabaseService dbService = ServiceProvider.GetRequiredService<ITfDatabaseService>();
			ITfDataManager dataManager = ServiceProvider.GetRequiredService<ITfDataManager>();

			using (var scope = dbService.CreateTransactionScope(Constants.DB_OPERATION_LOCK_KEY))
			{
				var (provider, spaceData) = await SpaceEnvUtility.CreateTestStructureAndData(ServiceProvider, dbService);
				var result = dataManager.QuerySpaceData(spaceData.Id,
					noRows: false,
					//search: "10",
					//page: 1,
					//pageSize: 5,
					returnOnlyTfIds: true,
					//userFilters: new List<TfFilterBase>
					//{
					//	new TfFilterOr(new[]
					//		{
					//			(TfFilterBase)new TfFilterText("short_text_column", TfFilterTextComparisonMethod.Contains, "b"),
					//			(TfFilterBase)new TfFilterText("short_text_column", TfFilterTextComparisonMethod.Fts, "a"),
					//			(TfFilterBase)new TfFilterText("sk_shared_key_text", TfFilterTextComparisonMethod.Contains, "a"),
					//			(TfFilterBase)new TfFilterNumeric("sk_shared_key_int", TfFilterNumericComparisonMethod.Equal, "5" )
					//		})

					//},
					userSorts: new List<TfSort> {
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


	[Fact]
	public async Task ID_CRUD()
	{
		using (await locker.LockAsync())
		{
			ITfDatabaseService dbService = ServiceProvider.GetRequiredService<ITfDatabaseService>();
			IIdentityManager identityManager = ServiceProvider.GetRequiredService<IIdentityManager>();
			ITfDataManager dataManager = ServiceProvider.GetRequiredService<ITfDataManager>();

			using (var scope = dbService.CreateTransactionScope(Constants.DB_OPERATION_LOCK_KEY))
			{
				Guid? id1 = null;
				Guid? id2 = null;

				var task = Task.Run(() => { id1 = dataManager.GetId("test"); });
				var exception = Record.ExceptionAsync(async () => await task).Result;
				exception.Should().BeNull();
				id1.Should().NotBeNull();

				task = Task.Run(() => { id2 = dataManager.GetId("test"); });
				exception = Record.ExceptionAsync(async () => await task).Result;
				exception.Should().BeNull();
				id2.Should().NotBeNull();

				id1.Should().Be(id2.Value);
			}
		}
	}


	[Fact]
	public async Task ID_BuildFill()
	{
		using (await locker.LockAsync())
		{
			ITfDatabaseService dbService = ServiceProvider.GetRequiredService<ITfDatabaseService>();
			ITfDataManager dataManager = ServiceProvider.GetRequiredService<ITfDataManager>();

			using (var scope = dbService.CreateTransactionScope(Constants.DB_OPERATION_LOCK_KEY))
			{
				var faker = new Faker("en");

				Dictionary<string,Guid> idsDict = new Dictionary<string, Guid>();

				for(int i = 0; i<10000; i++)
				{
					List<string> textList = new List<string>();
					textList.Add(faker.Random.ReplaceNumbers("########################################"));
					textList.Add(faker.Random.ReplaceNumbers("########################################"));
					textList.Add(faker.Random.ReplaceNumbers("########################################"));

					var combinedKey = dataManager.CombineKey(textList);
					idsDict.Add(combinedKey, Guid.Empty);
				}

				var task = Task.Run(() => { dataManager.BulkFillIds(idsDict); });
				var exception = Record.ExceptionAsync(async () => await task).Result;
				exception.Should().BeNull();

				foreach(var guid in idsDict.Values)
				{
					guid.Should().NotBeEmpty();
				}

			}
		}
	}

}