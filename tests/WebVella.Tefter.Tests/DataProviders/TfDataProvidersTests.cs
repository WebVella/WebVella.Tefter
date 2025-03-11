using WebVella.Tefter.Exceptions;
using WebVella.Tefter.Models;
using WebVella.Tefter.Services;

namespace WebVella.Tefter.Tests.DataProviders;

public partial class TfDataProvidersTests : BaseTest
{
	#region <--- Providers --->

	//[Fact]
	//public async Task _Provider_Lists()
	//{
	//	using (await locker.LockAsync())
	//	{
	//		ITfService tfService = ServiceProvider.GetService<ITfService>();
	//		ITfDatabaseService dbService = ServiceProvider.GetRequiredService<ITfDatabaseService>();

	//		using (var scope = dbService.CreateTransactionScope())
	//		{
	//			var providerTypes = tfService.GetDataProviderTypes();
	//			var providerType = providerTypes
	//				.Single(x => x.Id == new Guid("90b7de99-4f7f-4a31-bcf9-9be988739d2d"));

	//			var provider = tfService.GetDataProvider(new Guid("b90d2c29-5600-401d-821e-b7d85c676ee5"));

	//			var dataTable = tfService.QueryDataProvider(provider, search: null, page: null, pageSize: null);
	//			var dt = dataTable.ToDataTable();

	//		}
	//	}
	//}

	private TfDataProvider CreateProviderInternal(
		ITfService tfService,
		ITfMetaService tfMetaService)
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
		provider.Should().NotBeNull();
		provider.Should().BeOfType<TfDataProvider>();
		return provider;
	}

	[Fact]
	public async Task TestCreateDataProvider()
	{
		using (await locker.LockAsync())
		{
			ITfService tfService = ServiceProvider.GetService<ITfService>();
			ITfMetaService tfMetaService = ServiceProvider.GetService<ITfMetaService>();
			ITfDatabaseService dbService = ServiceProvider.GetRequiredService<ITfDatabaseService>();

			using (var scope = dbService.CreateTransactionScope())
			{
				CreateProviderInternal(tfService, tfMetaService);
			}
		}
	}

	[Fact]
	public async Task TestDeleteDataProvider()
	{
		using (await locker.LockAsync())
		{
			ITfService tfService = ServiceProvider.GetService<ITfService>();
			ITfMetaService tfMetaService = ServiceProvider.GetService<ITfMetaService>();
			ITfDatabaseService dbService = ServiceProvider.GetRequiredService<ITfDatabaseService>();

			using (var scope = dbService.CreateTransactionScope())
			{
				var provider = CreateProviderInternal(tfService, tfMetaService);

				var task = Task.Run(() => { tfService.DeleteDataProvider(provider.Id); });
				var exception = Record.ExceptionAsync(async () => await task).Result;
				exception.Should().BeNull();
			}
		}
	}

	[Fact]
	public async Task TestCreateProviderWithNoProviderType()
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
					Name = "test",
					ProviderType = null,
					SettingsJson = null
				};

				TfDataProvider provider = null;
				var task = Task.Run(() => { provider = tfService.CreateDataProvider(model); });
				var exception = Record.ExceptionAsync(async () => await task).Result;
				exception.Should().NotBeNull();
				exception.Should().BeOfType(typeof(TfValidationException));
				((TfValidationException)exception).Data.Keys.Count.Should().Be(1);
				((TfValidationException)exception).Data.Contains("ProviderType").Should().BeTrue();
			}
		}
	}

	[Fact]
	public async Task TestCreateProviderWithNoName()
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
					Name = "",
					ProviderType = providerType,
					SettingsJson = null
				};

				TfDataProvider provider = null;
				var task = Task.Run(() => { provider = tfService.CreateDataProvider(model); });
				var exception = Record.ExceptionAsync(async () => await task).Result;
				exception.Should().NotBeNull();
				exception.Should().BeOfType(typeof(TfValidationException));
				((TfValidationException)exception).Data.Keys.Count.Should().Be(1);
				((TfValidationException)exception).Data.Contains("Name").Should().BeTrue();
			}
		}
	}

	[Fact]
	public async Task TestCreateProviderWithSameName()
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
				provider.Should().BeOfType<TfDataProvider>();

				model.Id = Guid.NewGuid();

				task = Task.Run(() => { provider = tfService.CreateDataProvider(model); });
				exception = Record.ExceptionAsync(async () => await task).Result;
				exception.Should().BeOfType(typeof(TfValidationException));
				((TfValidationException)exception).Data.Keys.Count.Should().Be(1);
				((TfValidationException)exception).Data.Contains("Name").Should().BeTrue();
			}
		}
	}

	[Fact]
	public async Task TestUpdateDataProvider()
	{
		using (await locker.LockAsync())
		{
			ITfService tfService = ServiceProvider.GetService<ITfService>();
			ITfMetaService tfMetaService = ServiceProvider.GetService<ITfMetaService>();
			ITfDatabaseService dbService = ServiceProvider.GetRequiredService<ITfDatabaseService>();

			using (var scope = dbService.CreateTransactionScope())
			{
				var provider = CreateProviderInternal(tfService, tfMetaService);

				Guid id = Guid.NewGuid();
				TfDataProviderModel model = new TfDataProviderModel
				{
					Id = provider.Id,
					Name = "test data provider 1",
					ProviderType = provider.ProviderType,
					SettingsJson = null
				};

				var task = Task.Run(() => { provider = tfService.UpdateDataProvider(model); });
				var exception = Record.ExceptionAsync(async () => await task).Result;
				exception.Should().BeNull();
				provider.Name.Should().Be(model.Name);
			}
		}
	}

	[Fact]
	public async Task TestUpdateDataProviderWithSameName()
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
				provider.Should().BeOfType<TfDataProvider>();

				TfDataProviderModel model2 = new TfDataProviderModel
				{
					Name = "test data provider 2",
					ProviderType = providerType,
					SettingsJson = null
				};

				task = Task.Run(() => { provider = tfService.CreateDataProvider(model2); });
				exception = Record.ExceptionAsync(async () => await task).Result;
				exception.Should().BeNull();
				provider.Should().BeOfType<TfDataProvider>();


				model.Name = model2.Name;

				task = Task.Run(() => { provider = tfService.UpdateDataProvider(model); });
				exception = Record.ExceptionAsync(async () => await task).Result;
				exception.Should().BeOfType(typeof(TfValidationException));
				((TfValidationException)exception).Data.Keys.Count.Should().Be(1);
				((TfValidationException)exception).Data.Contains("Name").Should().BeTrue();
			}
		}
	}

	[Fact]
	public async Task TestUpdateDataProviderWithNoName()
	{
		using (await locker.LockAsync())
		{
			ITfService tfService = ServiceProvider.GetService<ITfService>();
			ITfMetaService tfMetaService = ServiceProvider.GetService<ITfMetaService>();
			ITfDatabaseService dbService = ServiceProvider.GetRequiredService<ITfDatabaseService>();

			using (var scope = dbService.CreateTransactionScope())
			{
				var provider = CreateProviderInternal(tfService, tfMetaService);

				TfDataProviderModel model = new TfDataProviderModel
				{
					Id = provider.Id,
					Name = string.Empty,
					ProviderType = provider.ProviderType,
					SettingsJson = null
				};

				var task = Task.Run(() => { provider = tfService.UpdateDataProvider(model); });
				var exception = Record.ExceptionAsync(async () => await task).Result;
				exception.Should().NotBeNull();
				exception.Should().BeOfType(typeof(TfValidationException));
				((TfValidationException)exception).Data.Keys.Count.Should().Be(1);
				((TfValidationException)exception).Data.Contains("Name").Should().BeTrue();
			}
		}
	}

	[Fact]
	public async Task TestUpdateDataProviderWithNoProviderType()
	{
		using (await locker.LockAsync())
		{
			ITfService tfService = ServiceProvider.GetService<ITfService>();
			ITfMetaService tfMetaService = ServiceProvider.GetService<ITfMetaService>();
			ITfDatabaseService dbService = ServiceProvider.GetRequiredService<ITfDatabaseService>();

			using (var scope = dbService.CreateTransactionScope())
			{
				var provider = CreateProviderInternal(tfService, tfMetaService);

				TfDataProviderModel model = new TfDataProviderModel
				{
					Id = provider.Id,
					Name = provider.Name,
					ProviderType = null,
					SettingsJson = null
				};

				var task = Task.Run(() => { provider = tfService.UpdateDataProvider(model); });
				var exception = Record.ExceptionAsync(async () => await task).Result;
				exception.Should().NotBeNull();
				exception.Should().BeOfType(typeof(TfValidationException));
				((TfValidationException)exception).Data.Keys.Count.Should().Be(1);
				((TfValidationException)exception).Data.Contains("ProviderType").Should().BeTrue();
			}
		}
	}

	#endregion

	#region <--- Columns --->

	[Fact]
	public async Task Column_DbName_Invalid()
	{
		using (await locker.LockAsync())
		{
			ITfService tfService = ServiceProvider.GetService<ITfService>();
			ITfMetaService tfMetaService = ServiceProvider.GetService<ITfMetaService>();
			ITfDatabaseService dbService = ServiceProvider.GetRequiredService<ITfDatabaseService>();

			using (var scope = dbService.CreateTransactionScope())
			{
				var provider = CreateProviderInternal(tfService, tfMetaService);

				TfDataProviderColumn column = new TfDataProviderColumn
				{
					Id = Guid.NewGuid(),
					AutoDefaultValue = true,
					DefaultValue = null,
					DataProviderId = provider.Id,
					DbName = "textOwa colona",
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

				//name format invalid
				var task = Task.Run(() => { tfService.CreateDataProviderColumn(column); });
				var exception = Record.ExceptionAsync(async () => await task).Result;
				exception.Should().NotBeNull();
				exception.Should().BeOfType(typeof(TfValidationException));
				((TfValidationException)exception).Data.Keys.Count.Should().Be(1);
				((TfValidationException)exception).Data.Contains(nameof(TfDataProviderColumn.DbName)).Should().BeTrue();


				//empty name
				column.DbName = string.Empty;
				task = Task.Run(() => { tfService.CreateDataProviderColumn(column); });
				exception = Record.ExceptionAsync(async () => await task).Result;
				exception.Should().NotBeNull();
				exception.Should().BeOfType(typeof(TfValidationException));
				((TfValidationException)exception).Data.Keys.Count.Should().Be(1);
				((TfValidationException)exception).Data.Contains(nameof(TfDataProviderColumn.DbName)).Should().BeTrue();

				//start with tf_
				column.DbName = "tf_test";
				task = Task.Run(() => { tfService.CreateDataProviderColumn(column); });
				exception = Record.ExceptionAsync(async () => await task).Result;
				exception.Should().NotBeNull();
				exception.Should().BeOfType(typeof(TfValidationException));
				((TfValidationException)exception).Data.Keys.Count.Should().Be(1);
				((TfValidationException)exception).Data.Contains(nameof(TfDataProviderColumn.DbName)).Should().BeTrue();

				//too short
				column.DbName = "a";
				task = Task.Run(() => { tfService.CreateDataProviderColumn(column); });
				exception = Record.ExceptionAsync(async () => await task).Result;
				exception.Should().NotBeNull();
				exception.Should().BeOfType(typeof(TfValidationException));
				((TfValidationException)exception).Data.Keys.Count.Should().Be(1);
				((TfValidationException)exception).Data.Contains(nameof(TfDataProviderColumn.DbName)).Should().BeTrue();

				//too long
				column.DbName = "rtrtrtrtrtrtrtrtrtrtrtrtrtrtrtrtrtrtrtrtrtrtrtrtrtrtrtrtrtrtrtrtrtrtrtrtrtrtrtrtrtrtrtrtrtrtrt" +
					"rtrtrtrtrtrtrtrtrtrtrtrtrtrtrtrtrtrtrtrtrtrtrtrtrtrtrtrtrtrtrtrtrtrtrtrtrtrtrtrtrtrtrtrtrtrtrt";
				task = Task.Run(() => { tfService.CreateDataProviderColumn(column); });
				exception = Record.ExceptionAsync(async () => await task).Result;
				exception.Should().NotBeNull();
				exception.Should().BeOfType(typeof(TfValidationException));
				((TfValidationException)exception).Data.Keys.Count.Should().Be(1);
				((TfValidationException)exception).Data.Contains(nameof(TfDataProviderColumn.DbName)).Should().BeTrue();
			}
		}
	}

	[Fact]
	public async Task Column_Id_Empty()
	{
		using (await locker.LockAsync())
		{
			ITfService tfService = ServiceProvider.GetService<ITfService>();
			ITfMetaService tfMetaService = ServiceProvider.GetService<ITfMetaService>();
			ITfDatabaseService dbService = ServiceProvider.GetRequiredService<ITfDatabaseService>();

			using (var scope = dbService.CreateTransactionScope())
			{
				var provider = CreateProviderInternal(tfService, tfMetaService);

				TfDataProviderColumn column = new TfDataProviderColumn
				{
					Id = Guid.Empty,
					AutoDefaultValue = true,
					DefaultValue = null,
					DataProviderId = provider.Id,
					DbName = "textcolona",
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

				//empty id, but internally we set new id
				var task = Task.Run(() => { tfService.CreateDataProviderColumn(column); });
				var exception = Record.ExceptionAsync(async () => await task).Result;
				exception.Should().BeNull();
			}
		}
	}

	[Fact]
	public async Task Column_DataProviderId_Empty()
	{
		using (await locker.LockAsync())
		{
			ITfService tfService = ServiceProvider.GetService<ITfService>();
			ITfMetaService tfMetaService = ServiceProvider.GetService<ITfMetaService>();
			ITfDatabaseService dbService = ServiceProvider.GetRequiredService<ITfDatabaseService>();

			using (var scope = dbService.CreateTransactionScope())
			{
				var provider = CreateProviderInternal(tfService, tfMetaService);

				TfDataProviderColumn column = new TfDataProviderColumn
				{
					Id = Guid.NewGuid(),
					AutoDefaultValue = true,
					DefaultValue = null,
					DataProviderId = Guid.Empty,
					DbName = "textcolona",
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

				//empty id
				var task = Task.Run(() => { tfService.CreateDataProviderColumn(column); });
				var exception = Record.ExceptionAsync(async () => await task).Result;
				exception.Should().NotBeNull();
				exception.Should().BeOfType(typeof(TfValidationException));
				((TfValidationException)exception).Data.Keys.Count.Should().Be(1);
				((TfValidationException)exception).Data.Contains(nameof(TfDataProviderColumn.DataProviderId)).Should().BeTrue();
			}
		}
	}

	[Fact]
	public async Task Column_SourceName_Empty()
	{
		using (await locker.LockAsync())
		{
			ITfService tfService = ServiceProvider.GetService<ITfService>();
			ITfMetaService tfMetaService = ServiceProvider.GetService<ITfMetaService>();
			ITfDatabaseService dbService = ServiceProvider.GetRequiredService<ITfDatabaseService>();

			using (var scope = dbService.CreateTransactionScope())
			{
				var provider = CreateProviderInternal(tfService, tfMetaService);

				TfDataProviderColumn column = new TfDataProviderColumn
				{
					Id = Guid.NewGuid(),
					AutoDefaultValue = true,
					DefaultValue = null,
					DataProviderId = provider.Id,
					DbName = "textcolona",
					DbType = TfDatabaseColumnType.Text,
					SourceName = "",
					SourceType = "TEXT",
					IncludeInTableSearch = true,
					IsNullable = true,
					IsSearchable = true,
					IsSortable = true,
					IsUnique = true,
					PreferredSearchType = TfDataProviderColumnSearchType.Contains
				};

				var task = Task.Run(() => { tfService.CreateDataProviderColumn(column); });
				var exception = Record.ExceptionAsync(async () => await task).Result;
				exception.Should().BeNull();
				//we allow this now
			}
		}
	}

	[Fact]
	public async Task Column_SourceType_Empty()
	{
		using (await locker.LockAsync())
		{
			ITfService tfService = ServiceProvider.GetService<ITfService>();
			ITfMetaService tfMetaService = ServiceProvider.GetService<ITfMetaService>();
			ITfDatabaseService dbService = ServiceProvider.GetRequiredService<ITfDatabaseService>();

			using (var scope = dbService.CreateTransactionScope())
			{
				var provider = CreateProviderInternal(tfService, tfMetaService);

				TfDataProviderColumn column = new TfDataProviderColumn
				{
					Id = Guid.NewGuid(),
					AutoDefaultValue = true,
					DefaultValue = null,
					DataProviderId = provider.Id,
					DbName = "textcolona",
					DbType = TfDatabaseColumnType.Text,
					SourceName = "source",
					SourceType = "",
					IncludeInTableSearch = true,
					IsNullable = true,
					IsSearchable = true,
					IsSortable = true,
					IsUnique = true,
					PreferredSearchType = TfDataProviderColumnSearchType.Contains
				};

				var task = Task.Run(() => { tfService.CreateDataProviderColumn(column); });
				var exception = Record.ExceptionAsync(async () => await task).Result;
				exception.Should().NotBeNull();
				exception.Should().BeOfType(typeof(TfValidationException));
				((TfValidationException)exception).Data.Keys.Count.Should().Be(1);
				((TfValidationException)exception).Data.Contains(nameof(TfDataProviderColumn.SourceType)).Should().BeTrue();
			}
		}
	}

	[Fact]
	public async Task Column_DefaultValue()
	{
		using (await locker.LockAsync())
		{
			ITfService tfService = ServiceProvider.GetService<ITfService>();
			ITfMetaService tfMetaService = ServiceProvider.GetService<ITfMetaService>();
			ITfDatabaseService dbService = ServiceProvider.GetRequiredService<ITfDatabaseService>();

			using (var scope = dbService.CreateTransactionScope())
			{
				var provider = CreateProviderInternal(tfService, tfMetaService);

				TfDataProviderColumn column = new TfDataProviderColumn
				{
					Id = Guid.NewGuid(),
					AutoDefaultValue = false,
					DefaultValue = Guid.NewGuid().ToString(),
					DataProviderId = provider.Id,
					DbName = "text",
					DbType = TfDatabaseColumnType.Text,
					SourceName = "source_column",
					SourceType = "TEXT",
					IncludeInTableSearch = true,
					IsNullable = false,
					IsSearchable = true,
					IsSortable = true,
					IsUnique = true,
					PreferredSearchType = TfDataProviderColumnSearchType.Contains
				};

				provider = tfService.CreateDataProviderColumn(column);

				column = new TfDataProviderColumn
				{
					Id = Guid.NewGuid(),
					AutoDefaultValue = false,
					DefaultValue = string.Empty,
					DataProviderId = provider.Id,
					DbName = "short_text",
					DbType = TfDatabaseColumnType.ShortText,
					SourceName = "source_column",
					SourceType = "SHORT_TEXT",
					IncludeInTableSearch = true,
					IsNullable = false,
					IsSearchable = true,
					IsSortable = true,
					IsUnique = true,
					PreferredSearchType = TfDataProviderColumnSearchType.Equals
				};

				provider = tfService.CreateDataProviderColumn(column);

				column = new TfDataProviderColumn
				{
					Id = Guid.NewGuid(),
					AutoDefaultValue = false,
					DefaultValue = short.MaxValue.ToString(CultureInfo.InvariantCulture),
					DataProviderId = provider.Id,
					DbName = "short_int",
					DbType = TfDatabaseColumnType.ShortInteger,
					SourceName = "source_column",
					SourceType = "SHORT_INTEGER",
					IncludeInTableSearch = true,
					IsNullable = true,
					IsSearchable = false,
					IsSortable = false,
					IsUnique = false,
					PreferredSearchType = TfDataProviderColumnSearchType.Equals
				};

				provider = tfService.CreateDataProviderColumn(column);

				column = new TfDataProviderColumn
				{
					Id = Guid.NewGuid(),
					AutoDefaultValue = false,
					DefaultValue = int.MaxValue.ToString(CultureInfo.InvariantCulture),
					DataProviderId = provider.Id,
					DbName = "int",
					DbType = TfDatabaseColumnType.Integer,
					SourceName = "source_column",
					SourceType = "INTEGER",
					IncludeInTableSearch = true,
					IsNullable = true,
					IsSearchable = false,
					IsSortable = false,
					IsUnique = false,
					PreferredSearchType = TfDataProviderColumnSearchType.Equals
				};

				provider = tfService.CreateDataProviderColumn(column);

				column = new TfDataProviderColumn
				{
					Id = Guid.NewGuid(),
					AutoDefaultValue = true,
					DefaultValue = long.MaxValue.ToString(CultureInfo.InvariantCulture),
					DataProviderId = provider.Id,
					DbName = "long_int",
					DbType = TfDatabaseColumnType.LongInteger,
					SourceName = "source_column",
					SourceType = "LONG_INTEGER",
					IncludeInTableSearch = true,
					IsNullable = true,
					IsSearchable = false,
					IsSortable = false,
					IsUnique = false,
					PreferredSearchType = TfDataProviderColumnSearchType.Equals
				};

				provider = tfService.CreateDataProviderColumn(column);

				column = new TfDataProviderColumn
				{
					Id = Guid.NewGuid(),
					AutoDefaultValue = true,
					DefaultValue = decimal.MaxValue.ToString(CultureInfo.InvariantCulture),
					DataProviderId = provider.Id,
					DbName = "number",
					DbType = TfDatabaseColumnType.Number,
					SourceName = "source_column",
					SourceType = "NUMBER",
					IncludeInTableSearch = true,
					IsNullable = true,
					IsSearchable = false,
					IsSortable = false,
					IsUnique = false,
					PreferredSearchType = TfDataProviderColumnSearchType.Equals
				};

				provider = tfService.CreateDataProviderColumn(column);

				column = new TfDataProviderColumn
				{
					Id = Guid.NewGuid(),
					AutoDefaultValue = false,
					DefaultValue = "2024-06-27",
					DataProviderId = provider.Id,
					DbName = "date",
					DbType = TfDatabaseColumnType.Date,
					SourceName = "source_column",
					SourceType = "DATE",
					IncludeInTableSearch = true,
					IsNullable = true,
					IsSearchable = false,
					IsSortable = false,
					IsUnique = false,
					PreferredSearchType = TfDataProviderColumnSearchType.Equals
				};

				provider = tfService.CreateDataProviderColumn(column);

				column = new TfDataProviderColumn
				{
					Id = Guid.NewGuid(),
					AutoDefaultValue = false,
					DefaultValue = "2024-06-27 12:01",
					DataProviderId = provider.Id,
					DbName = "datetime",
					DbType = TfDatabaseColumnType.DateTime,
					SourceName = "source_column",
					SourceType = "DATETIME",
					IncludeInTableSearch = true,
					IsNullable = true,
					IsSearchable = false,
					IsSortable = false,
					IsUnique = false,
					PreferredSearchType = TfDataProviderColumnSearchType.Equals
				};

				provider = tfService.CreateDataProviderColumn(column);
			}
		}
	}

	[Fact]
	public async Task Column_CreateUpdate_TEXT()
	{
		using (await locker.LockAsync())
		{
			CreateAndUpdateColumnType(TfDatabaseColumnType.Text, "TEXT", "test def value");
		}
	}

	[Fact]
	public async Task Column_CreateUpdate_SHORT_TEXT()
	{
		using (await locker.LockAsync())
		{
			CreateAndUpdateColumnType(TfDatabaseColumnType.ShortText, "SHORT_TEXT", "test def value");
		}
	}


	[Fact]
	public async Task Column_CreateUpdate_NUMBER()
	{
		using (await locker.LockAsync())
		{
			CreateAndUpdateColumnType(TfDatabaseColumnType.Number, "NUMBER", "123.456");
		}
	}

	[Fact]
	public async Task Column_CreateUpdate_SHORT_INTEGER()
	{
		using (await locker.LockAsync())
		{
			CreateAndUpdateColumnType(TfDatabaseColumnType.ShortInteger, "SHORT_INTEGER", "1");
		}
	}

	[Fact]
	public async Task Column_CreateUpdate_INTEGER()
	{
		using (await locker.LockAsync())
		{
			CreateAndUpdateColumnType(TfDatabaseColumnType.Integer, "INTEGER", "1");
		}
	}

	[Fact]
	public async Task Column_CreateUpdate_LONG_INTEGER()
	{
		using (await locker.LockAsync())
		{
			CreateAndUpdateColumnType(TfDatabaseColumnType.LongInteger, "LONG_INTEGER", "1");
		}
	}

	[Fact]
	public async Task Column_CreateUpdate_DATE()
	{
		using (await locker.LockAsync())
		{
			CreateAndUpdateColumnType(TfDatabaseColumnType.Date, "DATE", "1975-10-25");
		}
	}

	[Fact]
	public async Task Column_CreateUpdate_DATETIME()
	{
		using (await locker.LockAsync())
		{
			CreateAndUpdateColumnType(TfDatabaseColumnType.DateTime, "DATETIME", "1975-10-25 20:25:45.123");
		}
	}

	[Fact]
	public async Task Column_CreateUpdate_BOOLEAN()
	{
		using (await locker.LockAsync())
		{
			CreateAndUpdateColumnType(TfDatabaseColumnType.Boolean, "BOOLEAN", "true");
		}
	}

	[Fact]
	public async Task Column_CreateUpdate_GUID()
	{
		using (await locker.LockAsync())
		{
			CreateAndUpdateColumnType(TfDatabaseColumnType.Guid, "GUID", Guid.NewGuid().ToString());
		}
	}


	private void CreateAndUpdateColumnType(
		TfDatabaseColumnType type,
		string sourceType,
		string defaultValue)
	{
		ITfService tfService = ServiceProvider.GetService<ITfService>();
		ITfMetaService tfMetaService = ServiceProvider.GetService<ITfMetaService>();
		ITfDatabaseService dbService = ServiceProvider.GetRequiredService<ITfDatabaseService>();

		using (var scope = dbService.CreateTransactionScope())
		{
			var provider = CreateProviderInternal(tfService, tfMetaService);

			TfDataProviderColumn column = new TfDataProviderColumn
			{
				Id = Guid.NewGuid(),
				AutoDefaultValue = false,
				DefaultValue = null,
				DataProviderId = provider.Id,
				DbName = "db_column",
				DbType = type,
				SourceName = "source_column",
				SourceType = sourceType,
				IncludeInTableSearch = false,
				IsNullable = true,
				IsSearchable = true,
				IsSortable = true,
				IsUnique = true,
				PreferredSearchType = TfDataProviderColumnSearchType.Contains
			};

			provider = tfService.CreateDataProviderColumn(column);

			var exColumn = provider.Columns.FirstOrDefault();

			exColumn.Should().NotBeNull();
			exColumn.AutoDefaultValue.Should().Be(column.AutoDefaultValue);
			exColumn.DataProviderId.Should().Be(column.DataProviderId);
			exColumn.DbName.Should().Be(column.DbName);
			exColumn.DbType.Should().Be(column.DbType);
			exColumn.SourceName.Should().Be(column.SourceName);
			exColumn.SourceType.Should().Be(column.SourceType);
			exColumn.IncludeInTableSearch.Should().Be(column.IncludeInTableSearch);
			exColumn.IsNullable.Should().Be(column.IsNullable);
			exColumn.IsSearchable.Should().Be(column.IsSearchable);
			exColumn.IsSortable.Should().Be(column.IsSortable);
			exColumn.IsUnique.Should().Be(column.IsUnique);
			exColumn.PreferredSearchType.Should().Be(column.PreferredSearchType);
			exColumn.DefaultValue.Should().Be(column.DefaultValue);

			column.DefaultValue = defaultValue;
			column.SourceName = "source_column_updated";
			column.IncludeInTableSearch = !column.IncludeInTableSearch;
			column.AutoDefaultValue = !column.AutoDefaultValue;
			column.IsNullable = !column.IsNullable;
			column.IsSearchable = !column.IsSearchable;
			column.IsSortable = !column.IsSortable;
			column.IsUnique = !column.IsUnique;
			column.PreferredSearchType = TfDataProviderColumnSearchType.Equals;

			provider = tfService.UpdateDataProviderColumn(column);

			exColumn = provider.Columns.FirstOrDefault();
			exColumn.AutoDefaultValue.Should().Be(column.AutoDefaultValue);
			exColumn.DataProviderId.Should().Be(column.DataProviderId);
			exColumn.DbName.Should().Be(column.DbName);
			exColumn.DbType.Should().Be(column.DbType);
			exColumn.SourceName.Should().Be(column.SourceName);
			exColumn.SourceType.Should().Be(column.SourceType);
			exColumn.IncludeInTableSearch.Should().Be(column.IncludeInTableSearch);
			exColumn.IsNullable.Should().Be(column.IsNullable);
			exColumn.IsSearchable.Should().Be(column.IsSearchable);
			exColumn.IsSortable.Should().Be(column.IsSortable);
			exColumn.IsUnique.Should().Be(column.IsUnique);
			exColumn.PreferredSearchType.Should().Be(column.PreferredSearchType);
			exColumn.DefaultValue.Should().Be(column.DefaultValue);
		}
	}
	#endregion

	#region <--- SharedKeys --->

	[Fact]
	public async Task SharedKey_CRUD()
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
	public async Task SharedKey_WithDuplicateColumns()
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
			DbName = "db_column",
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
			DbName = "db_column2",
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

	#endregion
}
