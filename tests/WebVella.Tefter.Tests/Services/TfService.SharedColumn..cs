namespace WebVella.Tefter.Tests.Services;

public partial class TfServiceTest : BaseTest
{
	[Fact]
	public async Task SharedColumn_CRUD()
	{
		using (await locker.LockAsync())
		{
			ITfService tfService = ServiceProvider.GetService<ITfService>();
			ITfDatabaseService dbService = ServiceProvider.GetRequiredService<ITfDatabaseService>();

			using (var scope = dbService.CreateTransactionScope())
			{
				TfSharedColumn sharedColumn = new TfSharedColumn
				{
					Id = Guid.NewGuid(),
					DbName = "sk_test",
					DbType = TfDatabaseColumnType.Text,
					IncludeInTableSearch = false,
					SharedKeyDbName = "shared_key"
				};

				var task = Task.Run(() => { tfService.CreateSharedColumn(sharedColumn); });
				var exception = Record.ExceptionAsync(async () => await task).Result;
				exception.Should().BeNull();

				List<TfSharedColumn> sharedColumns = null;
				task = Task.Run(() => { sharedColumns = tfService.GetSharedColumns(); });
				exception = Record.ExceptionAsync(async () => await task).Result;
				exception.Should().BeNull();
				sharedColumns.Should().NotBeNull();

				sharedColumns.Count().Should().Be(1);
				sharedColumns[0].Id.Should().Be(sharedColumn.Id);
				sharedColumns[0].DbName.Should().Be(sharedColumn.DbName);
				sharedColumns[0].DbType.Should().Be(sharedColumn.DbType);
				sharedColumns[0].IncludeInTableSearch.Should().Be(sharedColumn.IncludeInTableSearch);
				sharedColumns[0].SharedKeyDbName.Should().Be(sharedColumn.SharedKeyDbName);

				sharedColumn.DbName = "sk_test1";
				sharedColumn.DbType = TfDatabaseColumnType.Integer;
				sharedColumn.IncludeInTableSearch = !sharedColumn.IncludeInTableSearch;
				sharedColumn.SharedKeyDbName = "shared_key_1";


				task = Task.Run(() => { tfService.UpdateSharedColumn(sharedColumn); });
				exception = Record.ExceptionAsync(async () => await task).Result;
				exception.Should().BeNull();

				task = Task.Run(() => { sharedColumns = tfService.GetSharedColumns(); });
				exception = Record.ExceptionAsync(async () => await task).Result;
				exception.Should().BeNull();
				sharedColumns.Should().NotBeNull();

				sharedColumns.Count().Should().Be(1);
				sharedColumns[0].Id.Should().Be(sharedColumn.Id);
				sharedColumns[0].DbName.Should().Be(sharedColumn.DbName);
				sharedColumns[0].DbType.Should().Be(sharedColumn.DbType);
				sharedColumns[0].IncludeInTableSearch.Should().Be(sharedColumn.IncludeInTableSearch);
				sharedColumns[0].SharedKeyDbName.Should().Be(sharedColumn.SharedKeyDbName);

				task = Task.Run(() => { tfService.DeleteSharedColumn(sharedColumn.Id); });
				exception = Record.ExceptionAsync(async () => await task).Result;
				exception.Should().BeNull();


				task = Task.Run(() => { sharedColumns = tfService.GetSharedColumns(); });
				exception = Record.ExceptionAsync(async () => await task).Result;
				exception.Should().BeNull();
				sharedColumns.Should().NotBeNull();
				sharedColumns.Count().Should().Be(0);
			}
		}
	}


	[Fact]
	public async Task SharedColumn_DataProviderSharedColumns()
	{
		using (await locker.LockAsync())
		{
			ITfService tfService = ServiceProvider.GetService<ITfService>();
			ITfMetaService tfMetaService = ServiceProvider.GetService<ITfMetaService>();
			ITfDatabaseService dbService = ServiceProvider.GetRequiredService<ITfDatabaseService>();

			using (var scope = dbService.CreateTransactionScope())
			{

				var providerTypes = tfMetaService.GetDataProviderTypes();
				var providerType = providerTypes
					.Single(x => x.Id == new Guid("90b7de99-4f7f-4a31-bcf9-9be988739d2d"));

				Guid id = Guid.NewGuid();
				TfDataProviderModel model = new TfDataProviderModel
				{
					Name = "test data provider",
					ProviderType = providerType,
					SettingsJson = null
				};

				TfDataProvider provider = null;
				var task = Task.Run(() => { provider = tfService.CreateDataProvider(model); });
				var exception = Record.ExceptionAsync(async () => await task).Result;
				exception.Should().BeNull();

				TfDataProviderColumn column = new TfDataProviderColumn
				{
					Id = Guid.NewGuid(),
					AutoDefaultValue = false,
					DefaultValue = null,
					DataProviderId = provider.Id,
					DbName = "db_column",
					DbType = TfDatabaseColumnType.Integer,
					SourceName = "source_column",
					SourceType = "INTEGER",
					IncludeInTableSearch = false,
					IsNullable = true,
					IsSearchable = true,
					IsSortable = true,
					IsUnique = true,
					PreferredSearchType = TfDataProviderColumnSearchType.Contains
				};

				task = Task.Run(() => { tfService.CreateDataProviderColumn(column); });
				exception = Record.ExceptionAsync(async () => await task).Result;
				exception.Should().BeNull();

				provider = tfService.GetDataProvider(provider.Id);

				TfDataProviderSharedKey sharedKey =
					new TfDataProviderSharedKey
					{
						Id = Guid.NewGuid(),
						Description = "testing1",
						DataProviderId = provider.Id,
						DbName = "shared_key",
						Columns = new() { provider.Columns[0] }

					};

				task = Task.Run(() => { provider = tfService.CreateDataProviderSharedKey(sharedKey); });
				exception = Record.ExceptionAsync(async () => await task).Result;
				exception.Should().BeNull();

				TfSharedColumn sharedColumn = new TfSharedColumn
				{
					Id = Guid.NewGuid(),
					DbName = "sk_test1",
					DbType = TfDatabaseColumnType.Text,
					IncludeInTableSearch = false,
					SharedKeyDbName = "shared_key"
				};

				task = Task.Run(() => { tfService.CreateSharedColumn(sharedColumn); });
				exception = Record.ExceptionAsync(async () => await task).Result;
				exception.Should().BeNull();

				List<TfSharedColumn> sharedColumns = null;
				task = Task.Run(() => { sharedColumns = tfService.GetSharedColumns(); });
				exception = Record.ExceptionAsync(async () => await task).Result;
				exception.Should().BeNull();

				task = Task.Run(() => { provider = tfService.GetDataProvider(provider.Id); });
				exception = Record.ExceptionAsync(async () => await task).Result;
				exception.Should().BeNull();
				provider.SharedColumns.Count.Should().Be(1);
			}
		}
	}
}
