using Bogus;
using WebVella.Tefter.Models;
using WebVella.Tefter.Tests.Applications;

namespace WebVella.Tefter.Tests.Services;

public partial class TfServiceTest : BaseTest
{

	//methods are grouped because creating test structure has very time cost
	[Fact]
	public async Task Data_MultipleTestsWithSameInitialStructure()
	{
		using (await locker.LockAsync())
		{
			ITfDatabaseService dbService = ServiceProvider.GetRequiredService<ITfDatabaseService>();

			using (var scope = dbService.CreateTransactionScope(Constants.DB_OPERATION_LOCK_KEY))
			{
				var (provider, spaceData) = await CreateTestStructureAndData(ServiceProvider, dbService);

				Data_InsertUpdateTableTest(provider.Id,spaceData.Id);

				Data_SimpleQueryTest(provider.Id, spaceData.Id);

				Data_QueryOnlyTfIds(provider.Id, spaceData.Id);
			}
		}
	}

	public void Data_InsertUpdateTableTest(Guid providerId, Guid spaceDataId)
	{

		var faker = new Faker("en");
		ITfDatabaseService dbService = ServiceProvider.GetRequiredService<ITfDatabaseService>();
		ITfService tfService = ServiceProvider.GetService<ITfService>();

		using (var scope = dbService.CreateTransactionScope(Constants.DB_OPERATION_LOCK_KEY))
		{
			//var (provider, spaceData) = await CreateTestStructureAndData(ServiceProvider, dbService);
			var provider = tfService.GetDataProvider(providerId);
			var spaceData = tfService.GetSpaceData(spaceDataId);

			//var result = tfService.QuerySpaceData(spaceData.Id);
			var result = tfService.QueryDataProvider(provider);

			tfService.DeleteAllProviderRows(provider);

			var newTable = result.NewTable();

			for (var i = 0; i < 10; i++)
			{
				var newRow = newTable.NewRow();

				newRow["guid_column"] = Guid.NewGuid();
				newRow["short_text_column"] = i + faker.Lorem.Sentence();
				newRow["text_column"] = i + faker.Lorem.Lines();
				newRow["date_column"] = faker.Date.PastDateOnly();
				newRow["datetime_column"] = faker.Date.Future();
				newRow["short_int_column"] = faker.Random.Short(0, 100);
				newRow["int_column"] = faker.Random.Number(100, 1000);
				newRow["long_int_column"] = faker.Random.Long(1000, 10000);
				newRow["number_column"] = faker.Random.Decimal(100000, 1000000);

				newRow["sk_shared_key_text"] = "this is shared key text test " + i;
				newRow["sk_shared_key_int"] = i;

				newTable.Rows.Add(newRow);
			}

			result = tfService.SaveDataTable(newTable);

			Guid? skIntValue = result.Rows[0].GetSharedKeyValue("test");
			Guid? skTextValue = result.Rows[0].GetSharedKeyValue("shared_key_text");

			//result = tfService.QuerySpaceData(spaceData.Id);
			result = tfService.QueryDataProvider(provider);

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

			result = tfService.SaveDataTable(tableToUpdate);

			result = tfService.QueryDataProvider(provider);


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

			result = tfService.SaveDataTable(tableToUpdate);

			result = tfService.QueryDataProvider(provider);
		}
	}


	public void Data_SimpleQueryTest(Guid providerId, Guid spaceDataId)
	{
		ITfDatabaseService dbService = ServiceProvider.GetRequiredService<ITfDatabaseService>();
		ITfService tfService = ServiceProvider.GetService<ITfService>();

		using (var scope = dbService.CreateTransactionScope(Constants.DB_OPERATION_LOCK_KEY))
		{
			//var (provider, spaceData) = await CreateTestStructureAndData(ServiceProvider, dbService);
			var provider = tfService.GetDataProvider(providerId);
			var spaceData = tfService.GetSpaceData(spaceDataId);

			var result = tfService.QuerySpaceData(spaceData.Id,
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

	public void Data_QueryOnlyTfIds(Guid providerId, Guid spaceDataId)
	{
		ITfDatabaseService dbService = ServiceProvider.GetRequiredService<ITfDatabaseService>();
		ITfService tfService = ServiceProvider.GetService<ITfService>();

		using (var scope = dbService.CreateTransactionScope(Constants.DB_OPERATION_LOCK_KEY))
		{
			//var (provider, spaceData) = await CreateTestStructureAndData(ServiceProvider, dbService);
			var provider = tfService.GetDataProvider(providerId);
			var spaceData = tfService.GetSpaceData(spaceDataId);

			var result = tfService.QuerySpaceData(spaceData.Id,
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