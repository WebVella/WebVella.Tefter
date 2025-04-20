using WebVella.Tefter.Models;

namespace WebVella.Tefter.Tests.Services;

public partial class TfServiceTest : BaseTest
{
	[Fact]
	public async Task DataProviderJoinKey_CRUD()
	{
		using (await locker.LockAsync())
		{
			ITfService tfService = ServiceProvider.GetService<ITfService>();
			ITfMetaService tfMetaService = ServiceProvider.GetService<ITfMetaService>();
			ITfDatabaseService dbService = ServiceProvider.GetRequiredService<ITfDatabaseService>();
			ITfDatabaseManager dbManager = ServiceProvider.GetRequiredService<ITfDatabaseManager>();

			using (var scope = dbService.CreateTransactionScope())
			{
				var provider = CreateJoinKeysStructure(tfService, tfMetaService);

				TfDataProviderJoinKey joinKey =
					new TfDataProviderJoinKey
					{
						Id = Guid.NewGuid(),
						Description = "testing1",
						DataProviderId = provider.Id,
						DbName = "testing1",
						Columns = new() { provider.Columns[0] }

					};

				var task = Task.Run(() => { provider = tfService.CreateDataProviderJoinKey(joinKey); });
				var exception = Record.ExceptionAsync(async () => await task).Result;
				exception.Should().BeNull();

				provider.JoinKeys.Count().Should().Be(1);

				var tables = dbManager.GetDatabaseBuilder().Build();
				var table = tables.SingleOrDefault(t => t.Name == $"dp{provider.Index}");
				table.Should().NotBeNull();
				table.Columns.Any(x => x.Name == $"tf_jk_{joinKey.DbName}_id").Should().BeTrue();
				table.Columns.Any(x => x.Name == $"tf_jk_{joinKey.DbName}_version").Should().BeTrue();

				var joinKey1Created = provider.JoinKeys.Single(x => x.Id == joinKey.Id);

				joinKey1Created.DataProviderId.Should().Be(joinKey.DataProviderId);
				joinKey1Created.DbName.Should().Be(joinKey.DbName);
				joinKey1Created.Description.Should().Be(joinKey.Description);
				joinKey1Created.Version.Should().Be(1);
				joinKey1Created.Columns[0].Id.Should().Be(provider.Columns[0].Id);

				var joinKey2 = new TfDataProviderJoinKey
				{
					Id = Guid.NewGuid(),
					Description = "testing2",
					DataProviderId = provider.Id,
					DbName = "testing2",
					Columns = new() { provider.Columns[1] }
				};

				task = Task.Run(() => { provider = tfService.CreateDataProviderJoinKey(joinKey2); });
				exception = Record.ExceptionAsync(async () => await task).Result;
				exception.Should().BeNull();

				provider.JoinKeys.Count().Should().Be(2);

				var joinKey2Created = provider.JoinKeys.Single(x => x.Id == joinKey2.Id);

				joinKey2Created.DataProviderId.Should().Be(joinKey2.DataProviderId);
				joinKey2Created.DbName.Should().Be(joinKey2.DbName);
				joinKey2Created.Description.Should().Be(joinKey2.Description);
				joinKey2Created.Version.Should().Be(1);
				joinKey2Created.Columns[0].Id.Should().Be(provider.Columns[1].Id);

				joinKey2Created.Columns.Add(provider.Columns[0]);

				task = Task.Run(() => { provider = tfService.UpdateDataProviderJoinKey(joinKey2Created); });
				exception = Record.ExceptionAsync(async () => await task).Result;
				exception.Should().BeNull();

				provider.JoinKeys.Count().Should().Be(2);

				tables = dbManager.GetDatabaseBuilder().Build();
				table = tables.SingleOrDefault(t => t.Name == $"dp{provider.Index}");
				table.Should().NotBeNull();
				table.Columns.Any(x => x.Name == $"tf_jk_{joinKey2Created.DbName}_id").Should().BeTrue();
				table.Columns.Any(x => x.Name == $"tf_jk_{joinKey2Created.DbName}_version").Should().BeTrue();



				var joinKey2Update = provider.JoinKeys.Single(x => x.Id == joinKey2Created.Id);
				joinKey2Update.DataProviderId.Should().Be(joinKey2.DataProviderId);
				joinKey2Update.DbName.Should().Be(joinKey2.DbName);
				joinKey2Update.Description.Should().Be(joinKey2.Description);
				joinKey2Update.Version.Should().Be(2);
				joinKey2Update.Columns.Count().Should().Be(2);

				task = Task.Run(() => { provider = tfService.DeleteDataProviderJoinKey(joinKey2Created.Id); });
				exception = Record.ExceptionAsync(async () => await task).Result;
				exception.Should().BeNull();
				provider.JoinKeys.Count().Should().Be(1);

				tables = dbManager.GetDatabaseBuilder().Build();
				table = tables.SingleOrDefault(t => t.Name == $"dp{provider.Index}");
				table.Should().NotBeNull();
				table.Columns.Any(x => x.Name == $"tf_jk_{joinKey2Created.DbName}_id").Should().BeFalse();
				table.Columns.Any(x => x.Name == $"tf_jk_{joinKey2Created.DbName}_version").Should().BeFalse();
			}
		}
	}

	[Fact]
	public async Task DataProviderJoinKey_WithDuplicateColumns()
	{
		using (await locker.LockAsync())
		{
			ITfService tfService = ServiceProvider.GetService<ITfService>();
			ITfMetaService tfMetaService = ServiceProvider.GetService<ITfMetaService>();
			ITfDatabaseService dbService = ServiceProvider.GetRequiredService<ITfDatabaseService>();
			ITfDatabaseManager dbManager = ServiceProvider.GetRequiredService<ITfDatabaseManager>();

			using (var scope = dbService.CreateTransactionScope())
			{
				var provider = CreateJoinKeysStructure(tfService, tfMetaService);

				TfDataProviderJoinKey joinKey =
					new TfDataProviderJoinKey
					{
						Id = Guid.NewGuid(),
						Description = "testing1",
						DataProviderId = provider.Id,
						DbName = "testing1",
						Columns = new() { provider.Columns[0], provider.Columns[1] }

					};

				var task = Task.Run(() => { tfService.CreateDataProviderJoinKey(joinKey); });
				var exception = Record.ExceptionAsync(async () => await task).Result;
				exception.Should().BeNull();
			}
		}
	}

	private TfDataProvider CreateJoinKeysStructure(
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
