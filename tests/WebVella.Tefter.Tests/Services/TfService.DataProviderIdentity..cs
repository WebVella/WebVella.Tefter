using WebVella.Tefter.Models;

namespace WebVella.Tefter.Tests.Services;

public partial class TfServiceTest : BaseTest
{
	[Fact]
	public async Task DataProviderIdentity_CRUD()
	{
		using (await locker.LockAsync())
		{
			ITfService tfService = ServiceProvider.GetService<ITfService>();
			ITfMetaService tfMetaService = ServiceProvider.GetService<ITfMetaService>();
			ITfDatabaseService dbService = ServiceProvider.GetRequiredService<ITfDatabaseService>();
			ITfDatabaseManager dbManager = ServiceProvider.GetRequiredService<ITfDatabaseManager>();

			using (var scope = dbService.CreateTransactionScope())
			{
				var provider = CreateDataProviderSampleStructureForIdentities(tfService, tfMetaService);
				provider.Identities.Count().Should().Be(1); //system identity

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
						Columns = provider.Columns.Select(x => x.DbName).ToList()
					};

				task = Task.Run(() => { provider = tfService.CreateDataProviderIdentity(dataProviderIdentityModel); });
				exception = Record.ExceptionAsync(async () => await task).Result;
				exception.Should().BeNull();
				provider.Identities.Count().Should().Be(2); //include system identity

				var tables = dbManager.GetDatabaseBuilder().Build();
				var table = tables.SingleOrDefault(t => t.Name == $"dp{provider.Index}");
				table.Should().NotBeNull();
				table.Columns.Any(x => x.Name == $"tf_ide_{dataProviderIdentityModel.DataIdentity}").Should().BeTrue();

				dataProviderIdentityModel.Columns = new List<string> { provider.Columns.Last().DbName };
				task = Task.Run(() => { provider = tfService.UpdateDataProviderIdentity(dataProviderIdentityModel); });
				exception = Record.ExceptionAsync(async () => await task).Result;
				exception.Should().BeNull();
				provider.Identities.Count().Should().Be(2); //include system identity


				task = Task.Run(() => { provider = tfService.DeleteDataProviderIdentity(dataProviderIdentityModel.Id); });
				exception = Record.ExceptionAsync(async () => await task).Result;
				exception.Should().BeNull();
				provider.Identities.Count().Should().Be(1); //include system identity

				tables = dbManager.GetDatabaseBuilder().Build();
				table = tables.SingleOrDefault(t => t.Name == $"dp{provider.Index}");
				table.Should().NotBeNull();
				table.Columns.Any(x => x.Name == $"tf_ide_{dataProviderIdentityModel.DataIdentity}").Should().BeFalse();

			}
		}
	}

	[Fact]
	public async Task DataProviderIdentity_TryToCreateWithSameId()
	{
		using (await locker.LockAsync())
		{
			ITfService tfService = ServiceProvider.GetService<ITfService>();
			ITfMetaService tfMetaService = ServiceProvider.GetService<ITfMetaService>();
			ITfDatabaseService dbService = ServiceProvider.GetRequiredService<ITfDatabaseService>();
			ITfDatabaseManager dbManager = ServiceProvider.GetRequiredService<ITfDatabaseManager>();

			using (var scope = dbService.CreateTransactionScope())
			{
				var provider = CreateDataProviderSampleStructureForIdentities(tfService, tfMetaService);
				provider.Identities.Count().Should().Be(1); //system identity

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

				TfDataIdentity dataIdentityModel2 = new TfDataIdentity
				{
					DataIdentity = "test_data_identity2",
					Label = "Test Data Identity 2",
				};

				TfDataIdentity dataIdentity2 = null;
				task = Task.Run(() => { dataIdentity2 = tfService.CreateDataIdentity(dataIdentityModel2); });
				exception = Record.ExceptionAsync(async () => await task).Result;
				exception.Should().BeNull();
				dataIdentity2.Should().NotBeNull();

				TfDataProviderIdentity dataProviderIdentityModel =
					new TfDataProviderIdentity
					{
						Id = Guid.NewGuid(),
						DataProviderId = provider.Id,
						DataIdentity = dataIdentity.DataIdentity,
						Columns = provider.Columns.Select(x => x.DbName).ToList()
					};

				task = Task.Run(() => { provider = tfService.CreateDataProviderIdentity(dataProviderIdentityModel); });
				exception = Record.ExceptionAsync(async () => await task).Result;
				exception.Should().BeNull();
				provider.Identities.Count().Should().Be(2); //include system identity

				dataProviderIdentityModel.DataIdentity = dataIdentity2.DataIdentity;
				task = Task.Run(() => { provider = tfService.CreateDataProviderIdentity(dataProviderIdentityModel); });
				exception = Record.ExceptionAsync(async () => await task).Result;
				exception.Should().NotBeNull();
				exception.Should().BeOfType<TfValidationException>();
				((TfValidationException)exception).Data.Keys.Count.Should().Be(1);
				((TfValidationException)exception).Data.Contains("Id").Should().BeTrue();
			}
		}
	}

	[Fact]
	public async Task DataProviderIdentity_TryToDuplicateIdentityForDataProvider()
	{
		using (await locker.LockAsync())
		{
			ITfService tfService = ServiceProvider.GetService<ITfService>();
			ITfMetaService tfMetaService = ServiceProvider.GetService<ITfMetaService>();
			ITfDatabaseService dbService = ServiceProvider.GetRequiredService<ITfDatabaseService>();
			ITfDatabaseManager dbManager = ServiceProvider.GetRequiredService<ITfDatabaseManager>();

			using (var scope = dbService.CreateTransactionScope())
			{
				var provider = CreateDataProviderSampleStructureForIdentities(tfService, tfMetaService);
				provider.Identities.Count().Should().Be(1); //system identity

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
						Columns = provider.Columns.Select(x => x.DbName).ToList()
					};

				task = Task.Run(() => { provider = tfService.CreateDataProviderIdentity(dataProviderIdentityModel); });
				exception = Record.ExceptionAsync(async () => await task).Result;
				exception.Should().BeNull();
				provider.Identities.Count().Should().Be(2); //include system identity

				dataProviderIdentityModel.Id = Guid.NewGuid();
				task = Task.Run(() => { provider = tfService.CreateDataProviderIdentity(dataProviderIdentityModel); });
				exception = Record.ExceptionAsync(async () => await task).Result;
				exception.Should().NotBeNull();
				exception.Should().BeOfType<TfValidationException>();
				((TfValidationException)exception).Data.Keys.Count.Should().Be(1);
				((TfValidationException)exception).Data.Contains("DataIdentity").Should().BeTrue();
			}
		}
	}

	[Fact]
	public async Task DataProviderIdentity_TryToCreateWithNonExistantProvider()
	{
		using (await locker.LockAsync())
		{
			ITfService tfService = ServiceProvider.GetService<ITfService>();
			ITfMetaService tfMetaService = ServiceProvider.GetService<ITfMetaService>();
			ITfDatabaseService dbService = ServiceProvider.GetRequiredService<ITfDatabaseService>();
			ITfDatabaseManager dbManager = ServiceProvider.GetRequiredService<ITfDatabaseManager>();

			using (var scope = dbService.CreateTransactionScope())
			{
				var provider = CreateDataProviderSampleStructureForIdentities(tfService, tfMetaService);
				provider.Identities.Count().Should().Be(1); //system identity

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
						DataProviderId =Guid.NewGuid(),
						DataIdentity = dataIdentity.DataIdentity,
						Columns = provider.Columns.Select(x => x.DbName).ToList()
					};

				task = Task.Run(() => { provider = tfService.CreateDataProviderIdentity(dataProviderIdentityModel); });
				exception = Record.ExceptionAsync(async () => await task).Result;
				exception.Should().NotBeNull();
				exception.Should().BeOfType<TfValidationException>();
				((TfValidationException)exception).Data.Keys.Count.Should().Be(1);
				((TfValidationException)exception).Data.Contains("DataProviderId").Should().BeTrue();
			}
		}
	}

	[Fact]
	public async Task DataProviderIdentity_TryToCreateWithInvalidColumns()
	{
		using (await locker.LockAsync())
		{
			ITfService tfService = ServiceProvider.GetService<ITfService>();
			ITfMetaService tfMetaService = ServiceProvider.GetService<ITfMetaService>();
			ITfDatabaseService dbService = ServiceProvider.GetRequiredService<ITfDatabaseService>();
			ITfDatabaseManager dbManager = ServiceProvider.GetRequiredService<ITfDatabaseManager>();

			using (var scope = dbService.CreateTransactionScope())
			{
				var provider = CreateDataProviderSampleStructureForIdentities(tfService, tfMetaService);
				provider.Identities.Count().Should().Be(1); //system identity

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
						Columns = provider.Columns.Select(x => x.DbName).ToList()
					};

				//with empty columns
				task = Task.Run(() => { provider = tfService.CreateDataProviderIdentity(dataProviderIdentityModel with { Columns = new List<string>() }); });
				exception = Record.ExceptionAsync(async () => await task).Result;
				exception.Should().NotBeNull();
				exception.Should().BeOfType<TfValidationException>();
				((TfValidationException)exception).Data.Keys.Count.Should().Be(1);
				((TfValidationException)exception).Data.Contains("Columns").Should().BeTrue();

				//with duplicated columns
				List<string> columns = new List<string> { provider.Columns.First().DbName, provider.Columns.First().DbName };
				task = Task.Run(() => { provider = tfService.CreateDataProviderIdentity(dataProviderIdentityModel with { Columns = columns }); });
				exception = Record.ExceptionAsync(async () => await task).Result;
				exception.Should().NotBeNull();
				exception.Should().BeOfType<TfValidationException>();
				((TfValidationException)exception).Data.Keys.Count.Should().Be(1);
				((TfValidationException)exception).Data.Contains("Columns").Should().BeTrue();

				//with invalid columns
				columns = new List<string> { provider.Columns.First().DbName, "invalid_column" };
				task = Task.Run(() => { provider = tfService.CreateDataProviderIdentity(dataProviderIdentityModel with { Columns = columns }); });
				exception = Record.ExceptionAsync(async () => await task).Result;
				exception.Should().NotBeNull();
				exception.Should().BeOfType<TfValidationException>();
				((TfValidationException)exception).Data.Keys.Count.Should().Be(1);
				((TfValidationException)exception).Data.Contains("Columns").Should().BeTrue();
			}
		}
	}

	private TfDataProvider CreateDataProviderSampleStructureForIdentities(
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

		tfService.CreateDataProviderColumn(column2);

		TfDataProviderColumn column3 = new TfDataProviderColumn
		{
			Id = Guid.NewGuid(),
			AutoDefaultValue = true,
			DefaultValue = null,
			DataProviderId = provider.Id,
			DbName = $"dp{provider.Index}_db_column3",
			DbType = TfDatabaseColumnType.Integer,
			SourceName = "source_column3",
			SourceType = "INTEGER",
			IncludeInTableSearch = false,
			IsNullable = true,
			IsSearchable = true,
			IsSortable = true,
			IsUnique = true,
			PreferredSearchType = TfDataProviderColumnSearchType.Contains
		};

		return tfService.CreateDataProviderColumn(column3);
	}
}
