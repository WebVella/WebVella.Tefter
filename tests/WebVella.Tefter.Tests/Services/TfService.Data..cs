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

			using (var scope = dbService.CreateTransactionScope(TfConstants.DB_OPERATION_LOCK_KEY))
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

		using (var scope = dbService.CreateTransactionScope(TfConstants.DB_OPERATION_LOCK_KEY))
		{
			var provider = tfService.GetDataProvider(providerId);
			var spaceData = tfService.GetSpaceData(spaceDataId);

			//var result = tfService.QuerySpaceData(spaceData.Id);
			var result = tfService.QueryDataProvider(provider);

			tfService.DeleteAllProviderRows(provider);

			var newTable = result.NewTable();

			for (var i = 0; i < 10; i++)
			{
				var newRow = newTable.NewRow();

				newRow[$"dp{provider.Index}_guid_column"] = Guid.NewGuid();
				newRow[$"dp{provider.Index}_short_text_column"] = i + faker.Lorem.Sentence();
				newRow[$"dp{provider.Index}_text_column"] = i + faker.Lorem.Lines();
				newRow[$"dp{provider.Index}_date_column"] = faker.Date.PastDateOnly();
				newRow[$"dp{provider.Index}_datetime_column"] = faker.Date.Future();
				newRow[$"dp{provider.Index}_short_int_column"] = faker.Random.Short(0, 100);
				newRow[$"dp{provider.Index}_int_column"] = faker.Random.Number(100, 1000);
				newRow[$"dp{provider.Index}_long_int_column"] = faker.Random.Long(1000, 10000);
				newRow[$"dp{provider.Index}_number_column"] = faker.Random.Decimal(100000, 1000000);

				newRow["test_data_identity_1.sc_text"] = "this is join key text test " + i;
				newRow["test_data_identity_2.sc_int"] = i;

				newTable.Rows.Add(newRow);
			}

			result = tfService.SaveDataTable(newTable);

			string rowIdDataIdentityValue = result.Rows[0].GetDataIdentityValue("tf_row_id");
			string intDataIdentityValue = result.Rows[0].GetDataIdentityValue("test_data_identity_1");
			string textDataIdentityValue = result.Rows[0].GetDataIdentityValue("test_data_identity_2");

			result = tfService.QueryDataProvider(provider);

			var tableToUpdate = result;

			for (var i = 0; i < 10; i++)
			{
				var row = tableToUpdate.Rows[i];

				row[$"dp{provider.Index}_guid_column"] = Guid.NewGuid();
				row[$"dp{provider.Index}_short_text_column"] = i + faker.Lorem.Sentence() + "upd";
				row[$"dp{provider.Index}_text_column"] = i + faker.Lorem.Lines() + "upd";
				row[$"dp{provider.Index}_date_column"] = faker.Date.PastDateOnly();
				row[$"dp{provider.Index}_datetime_column"] = faker.Date.Future();
				row[$"dp{provider.Index}_short_int_column"] = faker.Random.Short(0, 100);
				row[$"dp{provider.Index}_int_column"] = faker.Random.Number(100, 1000);
				row[$"dp{provider.Index}_long_int_column"] = faker.Random.Long(1000, 10000);
				row[$"dp{provider.Index}_number_column"] = faker.Random.Decimal(100000, 1000000);

				row["test_data_identity_1.sc_text"] = "this is join key text test " + i + "update";
				row["test_data_identity_2.sc_int"] = i + i;
			}

			result = tfService.SaveDataTable(tableToUpdate);

			result = tfService.QueryDataProvider(provider);


			tableToUpdate = result;

			for (var i = 0; i < 10; i++)
			{
				var row = tableToUpdate.Rows[i];

				row[$"dp{provider.Index}_guid_column"] = null;
				row[$"dp{provider.Index}_short_text_column"] = null;
				row[$"dp{provider.Index}_text_column"] = null;
				row[$"dp{provider.Index}_date_column"] = null;
				row[$"dp{provider.Index}_datetime_column"] = null;
				row[$"dp{provider.Index}_short_int_column"] = null;
				row[$"dp{provider.Index}_int_column"] = null;
				row[$"dp{provider.Index}_long_int_column"] = null;
				row[$"dp{provider.Index}_number_column"] = null;

				row["test_data_identity_1.sc_text"] =null;
				row["test_data_identity_2.sc_int"] = null;

			}

			result = tfService.SaveDataTable(tableToUpdate);

			result = tfService.QueryDataProvider(provider);
		}
	}


	public void Data_SimpleQueryTest(Guid providerId, Guid spaceDataId)
	{
		ITfDatabaseService dbService = ServiceProvider.GetRequiredService<ITfDatabaseService>();
		ITfService tfService = ServiceProvider.GetService<ITfService>();

		using (var scope = dbService.CreateTransactionScope(TfConstants.DB_OPERATION_LOCK_KEY))
		{
			var provider = tfService.GetDataProvider(providerId);
			var spaceData = tfService.GetSpaceData(spaceDataId);

			var result = tfService.QuerySpaceData(spaceData.Id,
				page: 1,
				pageSize: 5,
				userFilters: new List<TfFilterBase>
				{
						new TfFilterOr(new[]
							{
								(TfFilterBase)new TfFilterNumeric("sc_int", TfFilterNumericComparisonMethod.Equal, "5" )
							})
				},
				userSorts: new List<TfSort> {
						new TfSort {
							ColumnName ="missing_column",
							Direction=TfSortDirection.DESC} ,
						new TfSort {
							ColumnName =$"dp{provider.Index}_guid_column",
							Direction=TfSortDirection.DESC} ,
						new TfSort {
							ColumnName ="sc_int",
							Direction=TfSortDirection.ASC}
				});

			result.Rows.Count.Should().Be(1);

			result = tfService.QuerySpaceData(spaceData.Id,
				//noRows: true,
				//search: "10",
				page: 1,
				pageSize: 5,
				userFilters: new List<TfFilterBase>
				{
						new TfFilterOr(new[]
							{
								(TfFilterBase)new TfFilterText($"dp{provider.Index}_short_text_column", TfFilterTextComparisonMethod.Contains, "b"),
								(TfFilterBase)new TfFilterText($"dp{provider.Index}_short_text_column", TfFilterTextComparisonMethod.Fts, "a"),
								(TfFilterBase)new TfFilterText("sc_text", TfFilterTextComparisonMethod.Contains, "a"),
								(TfFilterBase)new TfFilterNumeric("sc_int", TfFilterNumericComparisonMethod.Equal, "5" )
							})
				},
				userSorts: new List<TfSort> {
						new TfSort {
							ColumnName ="missing_column",
							Direction=TfSortDirection.DESC} ,
						new TfSort {
							ColumnName =$"dp{provider.Index}_guid_column",
							Direction=TfSortDirection.DESC} ,
						new TfSort {
							ColumnName ="sc_int",
							Direction=TfSortDirection.ASC}
				});

		}
	}

	public void Data_QueryOnlyTfIds(Guid providerId, Guid spaceDataId)
	{
		ITfDatabaseService dbService = ServiceProvider.GetRequiredService<ITfDatabaseService>();
		ITfService tfService = ServiceProvider.GetService<ITfService>();

		using (var scope = dbService.CreateTransactionScope(TfConstants.DB_OPERATION_LOCK_KEY))
		{
			//var (provider, spaceData) = await CreateTestStructureAndData(ServiceProvider, dbService);
			var provider = tfService.GetDataProvider(providerId);
			var spaceData = tfService.GetSpaceData(spaceDataId);

			var result = tfService.QuerySpaceData(spaceData.Id,
				noRows: false,
				returnOnlyTfIds: true,
				userSorts: new List<TfSort> {
						new TfSort {
							ColumnName ="missing_column",
							Direction=TfSortDirection.DESC} ,
						new TfSort {
							ColumnName =$"dp{provider.Index}_guid_column",
							Direction=TfSortDirection.DESC} ,
						new TfSort {
							ColumnName ="sc_join_key_int",
							Direction=TfSortDirection.ASC}
				});

			result.Rows.Count.Should().Be(100);
			result.Columns.Count.Should().Be(1);
		}
	}
}