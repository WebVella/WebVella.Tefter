using System.Security.AccessControl;
using WebVella.Tefter.Api;

namespace WebVella.Tefter.Tests;

public partial class TfSharedColumnsManagerTests : BaseTest
{
	[Fact]
	public async Task SharedColumn_CRUD()
	{
		using (await locker.LockAsync())
		{
			ITfSharedColumnsManager sharedColumnManager = ServiceProvider.GetRequiredService<ITfSharedColumnsManager>();
			IDatabaseService dbService = ServiceProvider.GetRequiredService<IDatabaseService>();

			using (var scope = dbService.CreateTransactionScope())
			{
				TfSharedColumn sharedColumn = new TfSharedColumn
				{
					Id = Guid.NewGuid(),
					AddonId = null,
					DbName = "sk_test",
					DbType = DatabaseColumnType.Text,
					IncludeInTableSearch = false,
					SharedKeyDbName = "shared_key"
				};
				var result = sharedColumnManager.CreateSharedColumn(sharedColumn);
				result.IsSuccess.Should().BeTrue();

				var sharedColumnsResult = sharedColumnManager.GetSharedColumns();
				sharedColumnsResult.IsSuccess.Should().BeTrue();

				var sharedColumns = sharedColumnsResult.Value;
				sharedColumns.Count().Should().Be(1);
				sharedColumns[0].Id.Should().Be(sharedColumn.Id);
				sharedColumns[0].AddonId.Should().Be(sharedColumn.AddonId);
				sharedColumns[0].DbName.Should().Be(sharedColumn.DbName);
				sharedColumns[0].DbType.Should().Be(sharedColumn.DbType);
				sharedColumns[0].IncludeInTableSearch.Should().Be(sharedColumn.IncludeInTableSearch);
				sharedColumns[0].SharedKeyDbName.Should().Be(sharedColumn.SharedKeyDbName);

				sharedColumn.AddonId = Guid.NewGuid();
				sharedColumn.DbName = "sk_test1";
				sharedColumn.DbType = DatabaseColumnType.Integer;
				sharedColumn.IncludeInTableSearch = !sharedColumn.IncludeInTableSearch;
				sharedColumn.SharedKeyDbName = "shared_key_1";

				result = sharedColumnManager.UpdateSharedColumn(sharedColumn);
				result.IsSuccess.Should().BeTrue();

				sharedColumnsResult = sharedColumnManager.GetSharedColumns();
				sharedColumnsResult.IsSuccess.Should().BeTrue();

				sharedColumns = sharedColumnsResult.Value;
				sharedColumns.Count().Should().Be(1);
				sharedColumns[0].Id.Should().Be(sharedColumn.Id);
				sharedColumns[0].AddonId.Should().Be(sharedColumn.AddonId);
				sharedColumns[0].DbName.Should().Be(sharedColumn.DbName);
				sharedColumns[0].DbType.Should().Be(sharedColumn.DbType);
				sharedColumns[0].IncludeInTableSearch.Should().Be(sharedColumn.IncludeInTableSearch);
				sharedColumns[0].SharedKeyDbName.Should().Be(sharedColumn.SharedKeyDbName);

				result = sharedColumnManager.DeleteSharedColumn(sharedColumn.Id);
				result.IsSuccess.Should().BeTrue();

				sharedColumnsResult = sharedColumnManager.GetSharedColumns();
				sharedColumnsResult.IsSuccess.Should().BeTrue();

				sharedColumns = sharedColumnsResult.Value;
				sharedColumns.Count().Should().Be(0);
			}
		}
	}


	[Fact]
	public async Task SharedColumn_DataProviderSharedColumns()
	{
		using (await locker.LockAsync())
		{
			ITfDataProviderManager providerManager = ServiceProvider.GetRequiredService<ITfDataProviderManager>();
			ITfSharedColumnsManager sharedColumnManager = ServiceProvider.GetRequiredService<ITfSharedColumnsManager>();
			IDatabaseService dbService = ServiceProvider.GetRequiredService<IDatabaseService>();

			using (var scope = dbService.CreateTransactionScope())
			{

				var providerTypesResult = providerManager.GetProviderTypes();
				var providerType = providerTypesResult.Value
					.Single(x => x.Id == new Guid("90b7de99-4f7f-4a31-bcf9-9be988739d2d"));

				Guid id = Guid.NewGuid();
				TfDataProviderModel model = new TfDataProviderModel
				{
					Name = "test data provider",
					ProviderType = providerType,
					SettingsJson = null
				};
				var providerResult = providerManager.CreateDataProvider(model);
				providerResult.IsSuccess.Should().BeTrue();
				providerResult.Value.Should().BeOfType<TfDataProvider>();

				var provider = providerResult.Value;


				TfDataProviderColumn column = new TfDataProviderColumn
				{
					Id = Guid.NewGuid(),
					AutoDefaultValue = false,
					DefaultValue = null,
					DataProviderId = provider.Id,
					DbName = "db_column",
					DbType = DatabaseColumnType.Integer,
					SourceName = "source_column",
					SourceType = "INTEGER",
					IncludeInTableSearch = false,
					IsNullable = true,
					IsSearchable = true,
					IsSortable = true,
					IsUnique = true,
					PreferredSearchType = TfDataProviderColumnSearchType.Contains
				};

				var result = providerManager.CreateDataProviderColumn(column);
				result.IsSuccess.Should().BeTrue();

				result = providerManager.GetProvider(provider.Id);
				result.IsSuccess.Should().BeTrue();
				result.Value.Should().NotBeNull();
				provider = result.Value;


				TfDataProviderSharedKey sharedKey =
					new TfDataProviderSharedKey
					{
						Id = Guid.NewGuid(),
						Description = "testing1",
						DataProviderId = provider.Id,
						DbName = "shared_key",
						Columns = new() { provider.Columns[0] }

					};

				providerResult = providerManager.CreateDataProviderSharedKey(sharedKey);
				providerResult.IsSuccess.Should().BeTrue();


				TfSharedColumn sharedColumn = new TfSharedColumn
				{
					Id = Guid.NewGuid(),
					AddonId = null,
					DbName = "sk_test1",
					DbType = DatabaseColumnType.Text,
					IncludeInTableSearch = false,
					SharedKeyDbName = "shared_key"
				};
				result = sharedColumnManager.CreateSharedColumn(sharedColumn);
				result.IsSuccess.Should().BeTrue();

				var sharedColumnsResult = sharedColumnManager.GetSharedColumns();
				sharedColumnsResult.IsSuccess.Should().BeTrue();

				result = providerManager.GetProvider(provider.Id);
				result.IsSuccess.Should().BeTrue();
				result.Value.Should().NotBeNull();
				provider = result.Value;


				provider.SharedColumns.Count.Should().Be(1);



			}
		}
	}
}
