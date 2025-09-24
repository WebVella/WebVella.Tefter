using WebVella.Tefter.UI.Addons;

namespace WebVella.Tefter.Tests.Services;

public partial class TfServiceTest : BaseTest
{
	[Fact]
	public async Task Dataset_CRUD()
	{
		using (await locker.LockAsync())
		{
			ITfDatabaseService dbService = ServiceProvider.GetRequiredService<ITfDatabaseService>();
			ITfService tfService = ServiceProvider.GetService<ITfService>();
			ITfMetaService tfMetaService = ServiceProvider.GetService<ITfMetaService>();

			using (var scope = dbService.CreateTransactionScope(TfConstants.DB_OPERATION_LOCK_KEY))
			{
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

				var space = new TfSpace
				{
					Id = Guid.NewGuid(),
					Name = "Space1",
					Color = TfColor.Amber100,
					FluentIconName = "icon1",
					IsPrivate = false,
					Position = 0
				};
				tfService.CreateSpace(space);


				var datasetCreate1 = new TfCreateDataset
				{
					Id = Guid.NewGuid(),
					DataProviderId = providerModel.Id,
					Name = "data1",
				};

				var result = tfService.CreateDataset(datasetCreate1);
				result.Should().NotBeNull();
				result.Id.Should().Be(datasetCreate1.Id);
				result.Name.Should().Be(datasetCreate1.Name);
				result.Filters.Should().NotBeNull();
				result.Filters.Count().Should().Be(0);


				var datasetCreate2 = new TfCreateDataset
				{
					Id = Guid.NewGuid(),
					DataProviderId = providerModel.Id,
					Name = "data2",
				};

				result = tfService.CreateDataset(datasetCreate2);
				result.Should().NotBeNull();
				result.Id.Should().Be(datasetCreate2.Id);
				result.Name.Should().Be(datasetCreate2.Name);
				result.Filters.Should().NotBeNull();
				result.Filters.Count().Should().Be(0);

				var ds1 = tfService.GetDataset(datasetCreate1.Id);
				var ds2 = tfService.GetDataset(datasetCreate2.Id);

				result = tfService.UpdateDataset(new TfUpdateDataset
				{
					Id = ds1.Id,
					DataProviderId = ds1.DataProviderId,
					Columns = ds1.Columns,
					Name = "updated name",
					Filters = ds1.Filters,
					SortOrders = ds1.SortOrders
				});
				result.Name.Should().Be("updated name");

				tfService.DeleteDataset(ds1.Id);
				ds2 = tfService.GetDataset(ds2.Id);
				tfService.DeleteSpace(space.Id);
			}
		}
	}

	[Fact]
	public async Task Dataset_ColumnsManage()
	{
		using (await locker.LockAsync())
		{
			ITfDatabaseService dbService = ServiceProvider.GetRequiredService<ITfDatabaseService>();
			ITfService tfService = ServiceProvider.GetService<ITfService>();
			ITfMetaService tfMetaService = ServiceProvider.GetService<ITfMetaService>();

			using (var scope = dbService.CreateTransactionScope(TfConstants.DB_OPERATION_LOCK_KEY))
			{
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

				TfDataProviderColumn column = new TfDataProviderColumn
				{
					Id = Guid.Empty,
					AutoDefaultValue = true,
					DefaultValue = null,
					DataProviderId = providerModel.Id,
					DbName = $"dp{provider.Index}_textcolona",
					DbType = TfDatabaseColumnType.Text,
					SourceName = "source_column",
					SourceType = "TEXT",
					IncludeInTableSearch = true,
					IsNullable = true,
					IsSearchable = true,
					IsSortable = true,
					IsUnique = true,
					PreferredSearchType = TfDataProviderColumnSearchType.Contains
				};

				//empty id, but internaly we set new id
				tfService.CreateDataProviderColumn(column);

				provider = tfService.GetDataProvider(providerModel.Id);

				var space = new TfSpace
				{
					Id = Guid.NewGuid(),
					Name = "Space1",
					Color = TfColor.Amber100,
					FluentIconName = "icon1",
					IsPrivate = false,
					Position = 0
				};
				tfService.CreateSpace(space);

				var createDatasetModel = new TfCreateDataset
				{
					Id = Guid.NewGuid(),
					DataProviderId = providerModel.Id,
					Name = "data1",
				};
			}
		}
	}
}

