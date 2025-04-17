using WebVella.Tefter.Models;

namespace WebVella.Tefter.Tests.Services;

public partial class TfServiceTest : BaseTest
{
	[Fact]
	public async Task DataProviderSharedKey_CRUD()
	{
		using (await locker.LockAsync())
		{
			ITfService tfService = ServiceProvider.GetService<ITfService>();
			ITfMetaService tfMetaService = ServiceProvider.GetService<ITfMetaService>();
			ITfDatabaseService dbService = ServiceProvider.GetRequiredService<ITfDatabaseService>();
			ITfDatabaseManager dbManager = ServiceProvider.GetRequiredService<ITfDatabaseManager>();

			using (var scope = dbService.CreateTransactionScope())
			{
				var provider = CreateSharedKeysStructure(tfService, tfMetaService);

				TfDataProviderSharedKey sharedKey =
					new TfDataProviderSharedKey
					{
						Id = Guid.NewGuid(),
						Description = "testing1",
						DataProviderId = provider.Id,
						DbName = "testing1",
						Columns = new() { provider.Columns[0] }

					};

				var task = Task.Run(() => { provider = tfService.CreateDataProviderSharedKey(sharedKey); });
				var exception = Record.ExceptionAsync(async () => await task).Result;
				exception.Should().BeNull();

				provider.SharedKeys.Count().Should().Be(1);

				var tables = dbManager.GetDatabaseBuilder().Build();
				var table = tables.SingleOrDefault(t => t.Name == $"dp{provider.Index}");
				table.Should().NotBeNull();
				table.Columns.Any(x => x.Name == $"tf_sk_{sharedKey.DbName}_id").Should().BeTrue();
				table.Columns.Any(x => x.Name == $"tf_sk_{sharedKey.DbName}_version").Should().BeTrue();

				var sharedKey1Created = provider.SharedKeys.Single(x => x.Id == sharedKey.Id);

				sharedKey1Created.DataProviderId.Should().Be(sharedKey.DataProviderId);
				sharedKey1Created.DbName.Should().Be(sharedKey.DbName);
				sharedKey1Created.Description.Should().Be(sharedKey.Description);
				sharedKey1Created.Version.Should().Be(1);
				sharedKey1Created.Columns[0].Id.Should().Be(provider.Columns[0].Id);

				var sharedKey2 = new TfDataProviderSharedKey
				{
					Id = Guid.NewGuid(),
					Description = "testing2",
					DataProviderId = provider.Id,
					DbName = "testing2",
					Columns = new() { provider.Columns[1] }
				};

				task = Task.Run(() => { provider = tfService.CreateDataProviderSharedKey(sharedKey2); });
				exception = Record.ExceptionAsync(async () => await task).Result;
				exception.Should().BeNull();

				provider.SharedKeys.Count().Should().Be(2);

				var sharedKey2Created = provider.SharedKeys.Single(x => x.Id == sharedKey2.Id);

				sharedKey2Created.DataProviderId.Should().Be(sharedKey2.DataProviderId);
				sharedKey2Created.DbName.Should().Be(sharedKey2.DbName);
				sharedKey2Created.Description.Should().Be(sharedKey2.Description);
				sharedKey2Created.Version.Should().Be(1);
				sharedKey2Created.Columns[0].Id.Should().Be(provider.Columns[1].Id);

				sharedKey2Created.Columns.Add(provider.Columns[0]);

				task = Task.Run(() => { provider = tfService.UpdateDataProviderSharedKey(sharedKey2Created); });
				exception = Record.ExceptionAsync(async () => await task).Result;
				exception.Should().BeNull();

				provider.SharedKeys.Count().Should().Be(2);

				tables = dbManager.GetDatabaseBuilder().Build();
				table = tables.SingleOrDefault(t => t.Name == $"dp{provider.Index}");
				table.Should().NotBeNull();
				table.Columns.Any(x => x.Name == $"tf_sk_{sharedKey2Created.DbName}_id").Should().BeTrue();
				table.Columns.Any(x => x.Name == $"tf_sk_{sharedKey2Created.DbName}_version").Should().BeTrue();



				var sharedKey2Update = provider.SharedKeys.Single(x => x.Id == sharedKey2Created.Id);
				sharedKey2Update.DataProviderId.Should().Be(sharedKey2.DataProviderId);
				sharedKey2Update.DbName.Should().Be(sharedKey2.DbName);
				sharedKey2Update.Description.Should().Be(sharedKey2.Description);
				sharedKey2Update.Version.Should().Be(2);
				sharedKey2Update.Columns.Count().Should().Be(2);

				task = Task.Run(() => { provider = tfService.DeleteDataProviderSharedKey(sharedKey2Created.Id); });
				exception = Record.ExceptionAsync(async () => await task).Result;
				exception.Should().BeNull();
				provider.SharedKeys.Count().Should().Be(1);

				tables = dbManager.GetDatabaseBuilder().Build();
				table = tables.SingleOrDefault(t => t.Name == $"dp{provider.Index}");
				table.Should().NotBeNull();
				table.Columns.Any(x => x.Name == $"tf_sk_{sharedKey2Created.DbName}_id").Should().BeFalse();
				table.Columns.Any(x => x.Name == $"tf_sk_{sharedKey2Created.DbName}_version").Should().BeFalse();
			}
		}
	}

	[Fact]
	public async Task DataProviderSharedKey_WithDuplicateColumns()
	{
		using (await locker.LockAsync())
		{
			ITfService tfService = ServiceProvider.GetService<ITfService>();
			ITfMetaService tfMetaService = ServiceProvider.GetService<ITfMetaService>();
			ITfDatabaseService dbService = ServiceProvider.GetRequiredService<ITfDatabaseService>();
			ITfDatabaseManager dbManager = ServiceProvider.GetRequiredService<ITfDatabaseManager>();

			using (var scope = dbService.CreateTransactionScope())
			{
				var provider = CreateSharedKeysStructure(tfService, tfMetaService);

				TfDataProviderSharedKey sharedKey =
					new TfDataProviderSharedKey
					{
						Id = Guid.NewGuid(),
						Description = "testing1",
						DataProviderId = provider.Id,
						DbName = "testing1",
						Columns = new() { provider.Columns[0], provider.Columns[1] }

					};

				var task = Task.Run(() => { tfService.CreateDataProviderSharedKey(sharedKey); });
				var exception = Record.ExceptionAsync(async () => await task).Result;
				exception.Should().BeNull();
			}
		}
	}

	private TfDataProvider CreateSharedKeysStructure(
		ITfService tfService,
		ITfMetaService tfMetaService)
	{
		var provider = CreateProviderInternal(tfService, tfMetaService);

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

		return tfService.CreateDataProviderColumn(column2);
	}
}
