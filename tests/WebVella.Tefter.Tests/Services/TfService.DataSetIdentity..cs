using WebVella.Tefter.Models;

namespace WebVella.Tefter.Tests.Services;

public partial class TfServiceTest : BaseTest
{
	[Fact]
	public async Task DataSetIdentity_CRUD()
	{
		using (await locker.LockAsync())
		{
			ITfService tfService = ServiceProvider.GetService<ITfService>();
			ITfMetaService tfMetaService = ServiceProvider.GetService<ITfMetaService>();
			ITfDatabaseService dbService = ServiceProvider.GetRequiredService<ITfDatabaseService>();
			ITfDatabaseManager dbManager = ServiceProvider.GetRequiredService<ITfDatabaseManager>();

			using (var scope = dbService.CreateTransactionScope())
			{
				var (dataset,provider,provider2) = CreateDataSetSampleStructureForIdentities(tfService, tfMetaService);
				dataset.Should().NotBeNull();
				provider.Should().NotBeNull();
				provider2.Should().NotBeNull();

				TfDataIdentity dataIdentityModel = new TfDataIdentity
				{
					DataIdentity = "test_data_identity",
					Label = "Test Data Identity",
				};

				TfDataIdentity dataIdentity = null;
				var task = Task.Run(() => { dataIdentity = tfService.CreateDataIdentity(dataIdentityModel); });
				var exception = Record.ExceptionAsync(async () => await task).Result;
				exception.Should().BeNull();
				dataIdentity.Should().NotBeNull();

				TfDataProviderIdentity dataProviderIdentityModel =
					new TfDataProviderIdentity
					{
						Id = Guid.NewGuid(),
						DataProviderId = provider.Id,
						DataIdentity = dataIdentity.DataIdentity,
						Columns = provider.Columns.Select(x=>x.DbName).ToList()
					};

				task = Task.Run(() => { provider = tfService.CreateDataProviderIdentity(dataProviderIdentityModel); });
				exception = Record.ExceptionAsync(async () => await task).Result;
				exception.Should().BeNull();

				dataProviderIdentityModel =
						new TfDataProviderIdentity
						{
							Id = Guid.NewGuid(),
							DataProviderId = provider2.Id,
							DataIdentity = dataIdentity.DataIdentity,
							Columns = provider2.Columns.Select(x => x.DbName).ToList()
						};

				task = Task.Run(() => { provider2 = tfService.CreateDataProviderIdentity(dataProviderIdentityModel); });
				exception = Record.ExceptionAsync(async () => await task).Result;
				exception.Should().BeNull();

				TfDataSetIdentity datasetIdentityModel =
					new TfDataSetIdentity
					{
						Id = Guid.NewGuid(),
						DataSetId = dataset.Id,
						DataIdentity = dataIdentity.DataIdentity,
						Columns = provider2.Columns.Select(x => x.DbName).ToList()
					};

				TfDataSetIdentity datasetIdentity = null;
				task = Task.Run(() => { datasetIdentity = tfService.CreateDataSetIdentity(datasetIdentityModel); });
				exception = Record.ExceptionAsync(async () => await task).Result;
				exception.Should().BeNull();
				datasetIdentity.Should().NotBeNull();

				task = Task.Run(() => { dataset = tfService.GetDataSet(dataset.Id); });
				exception = Record.ExceptionAsync(async () => await task).Result;
				exception.Should().BeNull();
				dataset.Should().NotBeNull();
				dataset.Identities.Count().Should().Be(1);
				dataset.Identities[0].Columns.Count.Should().Be(2);

				datasetIdentityModel.Columns = new List<string> {provider2.Columns.First().DbName };
				datasetIdentity = null;
				task = Task.Run(() => { datasetIdentity = tfService.UpdateDataSetIdentity(datasetIdentityModel); });
				exception = Record.ExceptionAsync(async () => await task).Result;
				exception.Should().BeNull();
				datasetIdentity.Should().NotBeNull();

				task = Task.Run(() => { dataset = tfService.GetDataSet(dataset.Id); });
				exception = Record.ExceptionAsync(async () => await task).Result;
				exception.Should().BeNull();
				dataset.Identities[0].Columns.Count.Should().Be(1);

				task = Task.Run(() => { tfService.DeleteDataSetIdentity(datasetIdentity.Id); });
				exception = Record.ExceptionAsync(async () => await task).Result;
				exception.Should().BeNull();

				task = Task.Run(() => { dataset = tfService.GetDataSet(dataset.Id); });
				exception = Record.ExceptionAsync(async () => await task).Result;
				exception.Should().BeNull();
				dataset.Identities.Count.Should().Be(0);
			}
		}
	}

	private (TfDataSet, TfDataProvider, TfDataProvider) CreateDataSetSampleStructureForIdentities(
		ITfService tfService,
		ITfMetaService tfMetaService)
	{
		#region <-- data providers --->

		var provider = CreateProviderInternal(tfService, tfMetaService, "ut1");

		TfDataProviderColumn column = new TfDataProviderColumn
		{
			Id = Guid.NewGuid(),
			AutoDefaultValue = true,
			DefaultValue = null,
			DataProviderId = provider.Id,
			DbName = $"dp{provider.Index}_db_column",
			DbType = TfDatabaseColumnType.Text,
			SourceName = "source_column",
			SourceType = "TEXT",
			IncludeInTableSearch = false,
			IsNullable = true,
			IsSearchable = true,
			IsSortable = true,
			IsUnique = true,
			PreferredSearchType = TfDataProviderColumnSearchType.Contains
		};

		tfService.CreateDataProviderColumn(column);

		TfDataProviderColumn column2 = new TfDataProviderColumn
		{
			Id = Guid.NewGuid(),
			AutoDefaultValue = true,
			DefaultValue = null,
			DataProviderId = provider.Id,
			DbName = $"dp{provider.Index}_db_column2",
			DbType = TfDatabaseColumnType.Text,
			SourceName = "source_column2",
			SourceType = "TEXT",
			IncludeInTableSearch = false,
			IsNullable = true,
			IsSearchable = true,
			IsSortable = true,
			IsUnique = true,
			PreferredSearchType = TfDataProviderColumnSearchType.Contains
		};

		provider = tfService.CreateDataProviderColumn(column2);


		var provider2 = CreateProviderInternal(tfService, tfMetaService, "ut2");

		column = new TfDataProviderColumn
		{
			Id = Guid.NewGuid(),
			AutoDefaultValue = true,
			DefaultValue = null,
			DataProviderId = provider2.Id,
			DbName = $"dp{provider2.Index}_db_column",
			DbType = TfDatabaseColumnType.Text,
			SourceName = "source_column",
			SourceType = "TEXT",
			IncludeInTableSearch = false,
			IsNullable = true,
			IsSearchable = true,
			IsSortable = true,
			IsUnique = true,
			PreferredSearchType = TfDataProviderColumnSearchType.Contains
		};

		tfService.CreateDataProviderColumn(column);

		column2 = new TfDataProviderColumn
		{
			Id = Guid.NewGuid(),
			AutoDefaultValue = true,
			DefaultValue = null,
			DataProviderId = provider2.Id,
			DbName = $"dp{provider2.Index}_db_column2",
			DbType = TfDatabaseColumnType.Text,
			SourceName = "source_column2",
			SourceType = "TEXT",
			IncludeInTableSearch = false,
			IsNullable = true,
			IsSearchable = true,
			IsSortable = true,
			IsUnique = true,
			PreferredSearchType = TfDataProviderColumnSearchType.Contains
		};

		provider2 = tfService.CreateDataProviderColumn(column2);

		#endregion

		var space = new TfSpace
		{
			Id = Guid.NewGuid(),
			Name = "Space Unit Test",
			Color = TfColor.Amber100,
			FluentIconName = "icon1",
			IsPrivate = false,
			Position = 0
		};
		tfService.CreateSpace(space);


		var dataset = new TfCreateDataSet
		{
			Id = Guid.NewGuid(),
			DataProviderId = provider.Id,
			Name = "UnitTestDataSet",
		};

		return (tfService.CreateDataSet(dataset),provider, provider2);
	}
}

