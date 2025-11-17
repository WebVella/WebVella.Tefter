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

				TfDataIdentity dataIdentityModel1 = new TfDataIdentity
				{
					DataIdentity = "test_data_identity_1",
					Label = "Test Data Identity 1",
				};

				TfDataIdentity dataIdentity1 = null;
				var task = Task.Run(() => { dataIdentity1 = tfService.CreateDataIdentity(dataIdentityModel1); });
				var exception = Record.ExceptionAsync(async () => await task).Result;
				exception.Should().BeNull();
				dataIdentity1.Should().NotBeNull();

				TfDataIdentity dataIdentityModel2 = new TfDataIdentity
				{
					DataIdentity = "test_data_identity_2",
					Label = "Test Data Identity 2",
				};

				TfDataIdentity dataIdentity2 = null;
				task = Task.Run(() => { dataIdentity2 = tfService.CreateDataIdentity(dataIdentityModel2); });
				exception = Record.ExceptionAsync(async () => await task).Result;
				exception.Should().BeNull();
				dataIdentity2.Should().NotBeNull();

				TfSharedColumn sharedColumn = new TfSharedColumn
				{
					Id = Guid.NewGuid(),
					DbName = "sc_test",
					DbType = TfDatabaseColumnType.Text,
					IncludeInTableSearch = false,
					DataIdentity = "test_data_identity_1"
				};

				task = Task.Run(() => { tfService.CreateSharedColumn(sharedColumn); });
				exception = Record.ExceptionAsync(async () => await task).Result;
				exception.Should().BeNull();

				List<TfSharedColumn> sharedColumns = null;
				task = Task.Run(() => { sharedColumns = tfService.GetSharedColumns(); });
				exception = Record.ExceptionAsync(async () => await task).Result;
				exception.Should().BeNull();
				sharedColumns.Should().NotBeNull();

				sharedColumns.Count().Should().Be(3);// +2 -> addons create default shared columns
				var checkedColumn = sharedColumns.FirstOrDefault(x => x.Id == sharedColumn.Id);
				checkedColumn.Should().NotBeNull();
				checkedColumn.DbName.Should().Be(sharedColumn.DbName);
				checkedColumn.DbType.Should().Be(sharedColumn.DbType);
				checkedColumn.IncludeInTableSearch.Should().Be(sharedColumn.IncludeInTableSearch);
				checkedColumn.DataIdentity.Should().Be(sharedColumn.DataIdentity);

				sharedColumn.DbType = TfDatabaseColumnType.Integer;
				sharedColumn.IncludeInTableSearch = !sharedColumn.IncludeInTableSearch;
				sharedColumn.DataIdentity = "test_data_identity_2";


				task = Task.Run(() => { tfService.UpdateSharedColumn(sharedColumn); });
				exception = Record.ExceptionAsync(async () => await task).Result;
				exception.Should().BeNull();

				task = Task.Run(() => { sharedColumns = tfService.GetSharedColumns(); });
				exception = Record.ExceptionAsync(async () => await task).Result;
				exception.Should().BeNull();
				sharedColumns.Should().NotBeNull();

				sharedColumns.Count().Should().Be(3);// +2 -> addons create default shared columns
				checkedColumn = sharedColumns.FirstOrDefault(x => x.Id == sharedColumn.Id);
				checkedColumn.Should().NotBeNull();
				checkedColumn.DbName.Should().Be(sharedColumn.DbName);
				checkedColumn.DbType.Should().Be(sharedColumn.DbType);
				checkedColumn.IncludeInTableSearch.Should().Be(sharedColumn.IncludeInTableSearch);
				checkedColumn.DataIdentity.Should().Be(sharedColumn.DataIdentity);

				task = Task.Run(() => { tfService.DeleteSharedColumn(sharedColumn.Id); });
				exception = Record.ExceptionAsync(async () => await task).Result;
				exception.Should().BeNull();


				task = Task.Run(() => { sharedColumns = tfService.GetSharedColumns(); });
				exception = Record.ExceptionAsync(async () => await task).Result;
				exception.Should().BeNull();
				sharedColumns.Should().NotBeNull();
				sharedColumns.Count().Should().Be(2);// +2 -> addons create default shared columns
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
			
				var providerTypes = tfMetaService.GetDataProviderTypes();
				var providerType = providerTypes
					.Single(x => x.AddonId == new Guid("90b7de99-4f7f-4a31-bcf9-9be988739d2d"));

				Guid id = Guid.NewGuid();
				TfCreateDataProvider model = new TfCreateDataProvider
				{
					Name = "test data provider",
					ProviderType = providerType,
					SettingsJson = null
				};

				TfDataProvider provider = null;
				task = Task.Run(() => { provider = tfService.CreateDataProvider(model); });
				exception = Record.ExceptionAsync(async () => await task).Result;
				exception.Should().BeNull();

				TfDataProviderColumn column = new TfDataProviderColumn
				{
					Id = Guid.NewGuid(),
					AutoDefaultValue = false,
					DefaultValue = null,
					DataProviderId = provider.Id,
					DbName = $"dp{provider.Index}_db_column",
					DbType = TfDatabaseColumnType.Integer,
					SourceName = "source_column",
					SourceType = "INTEGER",
					IncludeInTableSearch = false,
					IsNullable = true,
					IsSearchable = true,
					IsSortable = true,
					IsUnique = true,
				};

				task = Task.Run(() => { tfService.CreateDataProviderColumn(column); });
				exception = Record.ExceptionAsync(async () => await task).Result;
				exception.Should().BeNull();

				provider = tfService.GetDataProvider(provider.Id);

				TfDataProviderIdentity providerDataIdentity =
					new TfDataProviderIdentity
					{
						Id = Guid.NewGuid(),
						DataProviderId = provider.Id,
						DataIdentity = dataIdentityModel.DataIdentity,
						Columns = new() { provider.Columns[0].DbName }

					};

				task = Task.Run(() => { provider = tfService.CreateDataProviderIdentity(providerDataIdentity); });
				exception = Record.ExceptionAsync(async () => await task).Result;
				exception.Should().BeNull();

				TfSharedColumn sharedColumn = new TfSharedColumn
				{
					Id = Guid.NewGuid(),
					DbName = "sc_test1",
					DbType = TfDatabaseColumnType.Text,
					IncludeInTableSearch = false,
					DataIdentity = dataIdentityModel.DataIdentity
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
				provider.SharedColumns.Count.Should().Be(3);// +2 -> addons create default shared columns
			}
		}
	}
}
