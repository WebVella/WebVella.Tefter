using WebVella.Tefter.Web.Services;

namespace WebVella.Tefter.Tests.DataProviders;

public partial class TfDataProviderManagerTests : BaseTest
{
	#region <--- Providers --->

	[Fact]
	public async Task TestCreateDataProvider()
	{
		using (await locker.LockAsync())
		{
			ITfDataProviderManager providerManager = ServiceProvider.GetRequiredService<ITfDataProviderManager>();
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
			}
		}
	}

	[Fact]
	public async Task TestDeleteDataProvider()
	{
		using (await locker.LockAsync())
		{
			ITfDataProviderManager providerManager = ServiceProvider.GetRequiredService<ITfDataProviderManager>();
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

				var deleteResult = providerManager.DeleteDataProvider(providerResult.Value.Id);
				deleteResult.IsSuccess.Should().BeTrue();
			}
		}
	}

	[Fact]
	public async Task TestCreateProviderWithNoProviderType()
	{
		using (await locker.LockAsync())
		{
			ITfDataProviderManager providerManager = ServiceProvider.GetRequiredService<ITfDataProviderManager>();
			IDatabaseService dbService = ServiceProvider.GetRequiredService<IDatabaseService>();

			using (var scope = dbService.CreateTransactionScope())
			{
				var providerTypesResult = providerManager.GetProviderTypes();
				var providerType = providerTypesResult.Value
					.Single(x => x.Id == new Guid("90b7de99-4f7f-4a31-bcf9-9be988739d2d"));

				Guid id = Guid.NewGuid();
				TfDataProviderModel model = new TfDataProviderModel
				{
					Name = "test",
					ProviderType = null,
					SettingsJson = null
				};

				var providerResult = providerManager.CreateDataProvider(model);
				providerResult.IsFailed.Should().BeTrue();
				providerResult.Errors.Count().Should().Be(1);
				providerResult.Errors[0].Should().BeOfType<ValidationError>();
				((ValidationError)providerResult.Errors[0]).PropertyName.Should().Be("ProviderType");
				((ValidationError)providerResult.Errors[0]).Reason.Should()
					.Be("The data provider type is required.");
			}
		}
	}

	[Fact]
	public async Task TestCreateProviderWithNoName()
	{
		using (await locker.LockAsync())
		{
			ITfDataProviderManager providerManager = ServiceProvider.GetRequiredService<ITfDataProviderManager>();
			IDatabaseService dbService = ServiceProvider.GetRequiredService<IDatabaseService>();

			using (var scope = dbService.CreateTransactionScope())
			{
				var providerTypesResult = providerManager.GetProviderTypes();
				var providerType = providerTypesResult.Value
					.Single(x => x.Id == new Guid("90b7de99-4f7f-4a31-bcf9-9be988739d2d"));

				Guid id = Guid.NewGuid();
				TfDataProviderModel model = new TfDataProviderModel
				{
					Name = "",
					ProviderType = providerType,
					SettingsJson = null
				};

				var providerResult = providerManager.CreateDataProvider(model);
				providerResult.IsFailed.Should().BeTrue();
				providerResult.Errors.Count().Should().Be(1);
				providerResult.Errors[0].Should().BeOfType<ValidationError>();
				((ValidationError)providerResult.Errors[0]).PropertyName.Should().Be("Name");
				((ValidationError)providerResult.Errors[0]).Reason.Should()
					.Be("The data provider name is required.");

			}
		}
	}

	[Fact]
	public async Task TestCreateProviderWithSameName()
	{
		using (await locker.LockAsync())
		{
			ITfDataProviderManager providerManager = ServiceProvider.GetRequiredService<ITfDataProviderManager>();
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

				model.Id = Guid.NewGuid();

				providerResult = providerManager.CreateDataProvider(model);
				providerResult.IsFailed.Should().BeTrue();
				providerResult.Errors.Count().Should().Be(1);
				providerResult.Errors[0].Should().BeOfType<ValidationError>();
				((ValidationError)providerResult.Errors[0]).PropertyName.Should().Be("Name");
				((ValidationError)providerResult.Errors[0]).Reason.Should()
					.Be("There is already existing data provider with specified name.");

			}
		}
	}

	[Fact]
	public async Task TestUpdateDataProvider()
	{
		using (await locker.LockAsync())
		{
			ITfDataProviderManager providerManager = ServiceProvider.GetRequiredService<ITfDataProviderManager>();
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

				model.Name = "test data provider 1";
				providerResult = providerManager.UpdateDataProvider(model);
				providerResult.IsSuccess.Should().BeTrue();
				providerResult.Value.Should().BeOfType<TfDataProvider>();
				providerResult.Value.Name.Should().Be(model.Name);
			}
		}
	}

	[Fact]
	public async Task TestUpdateDataProviderWithSameName()
	{
		using (await locker.LockAsync())
		{
			ITfDataProviderManager providerManager = ServiceProvider.GetRequiredService<ITfDataProviderManager>();
			IDatabaseService dbService = ServiceProvider.GetRequiredService<IDatabaseService>();

			using (var scope = dbService.CreateTransactionScope())
			{
				var providerTypesResult = providerManager.GetProviderTypes();
				var providerType = providerTypesResult.Value
					.Single(x => x.Id == new Guid("90b7de99-4f7f-4a31-bcf9-9be988739d2d"));

				TfDataProviderModel model = new TfDataProviderModel
				{
					Name = "test data provider",
					ProviderType = providerType,
					SettingsJson = null
				};
				var providerResult = providerManager.CreateDataProvider(model);
				providerResult.IsSuccess.Should().BeTrue();
				providerResult.Value.Should().BeOfType<TfDataProvider>();

				TfDataProviderModel model2 = new TfDataProviderModel
				{
					Name = "test data provider 2",
					ProviderType = providerType,
					SettingsJson = null
				};
				providerResult = providerManager.CreateDataProvider(model2);
				providerResult.IsSuccess.Should().BeTrue();
				providerResult.Value.Should().BeOfType<TfDataProvider>();


				model.Name = model2.Name;
				providerResult = providerManager.UpdateDataProvider(model);
				providerResult.IsFailed.Should().BeTrue();
				providerResult.Errors.Count().Should().Be(1);
				providerResult.Errors[0].Should().BeOfType<ValidationError>();
				((ValidationError)providerResult.Errors[0]).PropertyName.Should().Be("Name");
				((ValidationError)providerResult.Errors[0]).Reason.Should()
					.Be("There is already existing data provider with specified name.");
			}
		}
	}

	[Fact]
	public async Task TestUpdateDataProviderWithNoName()
	{
		using (await locker.LockAsync())
		{
			ITfDataProviderManager providerManager = ServiceProvider.GetRequiredService<ITfDataProviderManager>();
			IDatabaseService dbService = ServiceProvider.GetRequiredService<IDatabaseService>();

			using (var scope = dbService.CreateTransactionScope())
			{
				var providerTypesResult = providerManager.GetProviderTypes();
				var providerType = providerTypesResult.Value
					.Single(x => x.Id == new Guid("90b7de99-4f7f-4a31-bcf9-9be988739d2d"));

				TfDataProviderModel model = new TfDataProviderModel
				{
					Name = "test data provider",
					ProviderType = providerType,
					SettingsJson = null
				};
				var providerResult = providerManager.CreateDataProvider(model);
				providerResult.IsSuccess.Should().BeTrue();
				providerResult.Value.Should().BeOfType<TfDataProvider>();

				model.Id = providerResult.Value.Id;
				model.Name = "";

				providerResult = providerManager.UpdateDataProvider(model);
				providerResult.IsFailed.Should().BeTrue();
				providerResult.Errors.Count().Should().Be(1);
				providerResult.Errors[0].Should().BeOfType<ValidationError>();
				((ValidationError)providerResult.Errors[0]).PropertyName.Should().Be("Name");
				((ValidationError)providerResult.Errors[0]).Reason.Should()
					.Be("The data provider name is required.");
			}
		}
	}

	[Fact]
	public async Task TestUpdateDataProviderWithNoProviderType()
	{
		using (await locker.LockAsync())
		{
			ITfDataProviderManager providerManager = ServiceProvider.GetRequiredService<ITfDataProviderManager>();
			IDatabaseService dbService = ServiceProvider.GetRequiredService<IDatabaseService>();

			using (var scope = dbService.CreateTransactionScope())
			{
				var providerTypesResult = providerManager.GetProviderTypes();
				var providerType = providerTypesResult.Value
					.Single(x => x.Id == new Guid("90b7de99-4f7f-4a31-bcf9-9be988739d2d"));

				TfDataProviderModel model = new TfDataProviderModel
				{
					Name = "test data provider",
					ProviderType = providerType,
					SettingsJson = null
				};
				var providerResult = providerManager.CreateDataProvider(model);
				providerResult.IsSuccess.Should().BeTrue();
				providerResult.Value.Should().BeOfType<TfDataProvider>();

				model.Id = providerResult.Value.Id;
				model.ProviderType = null;

				providerResult = providerManager.UpdateDataProvider(model);
				providerResult.IsFailed.Should().BeTrue();
				providerResult.Errors.Count().Should().Be(1);
				providerResult.Errors[0].Should().BeOfType<ValidationError>();
				((ValidationError)providerResult.Errors[0]).PropertyName.Should().Be("ProviderType");
				((ValidationError)providerResult.Errors[0]).Reason.Should()
					.Be("The data provider type is required.");
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
			ITfDataProviderManager providerManager = ServiceProvider.GetRequiredService<ITfDataProviderManager>();
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
					AutoDefaultValue = true,
					DefaultValue = null,
					DataProviderId = provider.Id,
					DbName = "textOwa colona",
					DbType = DatabaseColumnType.Text,
					SourceName = "source_column",
					SourceType = "TEXT",
					IncludeInTableSearch = true,
					IsNullable = false,
					IsSearchable = true,
					IsSortable = true,
					IsUnique = true,
					PreferredSearchType = TfDataProviderColumnSearchType.Contains
				};

				//name format invalid
				var result = providerManager.CreateDataProviderColumn(column);
				result.IsSuccess.Should().BeFalse();
				((ValidationError)result.Errors[0]).PropertyName.Should().Be(nameof(TfDataProviderColumn.DbName));

				//empty name
				column.DbName = string.Empty;
				result = providerManager.CreateDataProviderColumn(column);
				result.IsSuccess.Should().BeFalse();
				((ValidationError)result.Errors[0]).PropertyName.Should().Be(nameof(TfDataProviderColumn.DbName));

				//start with tf_
				column.DbName = "tf_test";
				result = providerManager.CreateDataProviderColumn(column);
				result.IsSuccess.Should().BeFalse();
				((ValidationError)result.Errors[0]).PropertyName.Should().Be(nameof(TfDataProviderColumn.DbName));

				//too short
				column.DbName = "a";
				result = providerManager.CreateDataProviderColumn(column);
				result.IsSuccess.Should().BeFalse();
				((ValidationError)result.Errors[0]).PropertyName.Should().Be(nameof(TfDataProviderColumn.DbName));

				//too long
				column.DbName = "rtrtrtrtrtrtrtrtrtrtrtrtrtrtrtrtrtrtrtrtrtrtrtrtrtrtrtrtrtrtrtrtrtrtrtrtrtrtrtrtrtrtrtrtrtrtrt" +
					"rtrtrtrtrtrtrtrtrtrtrtrtrtrtrtrtrtrtrtrtrtrtrtrtrtrtrtrtrtrtrtrtrtrtrtrtrtrtrtrtrtrtrtrtrtrtrt";
				result = providerManager.CreateDataProviderColumn(column);
				result.IsSuccess.Should().BeFalse();
				((ValidationError)result.Errors[0]).PropertyName.Should().Be(nameof(TfDataProviderColumn.DbName));
			}
		}
	}

	[Fact]
	public async Task Column_Id_Empty()
	{
		using (await locker.LockAsync())
		{
			ITfDataProviderManager providerManager = ServiceProvider.GetRequiredService<ITfDataProviderManager>();
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
					Id = Guid.Empty,
					AutoDefaultValue = true,
					DefaultValue = null,
					DataProviderId = provider.Id,
					DbName = "textcolona",
					DbType = DatabaseColumnType.Text,
					SourceName = "source_column",
					SourceType = "TEXT",
					IncludeInTableSearch = true,
					IsNullable = false,
					IsSearchable = true,
					IsSortable = true,
					IsUnique = true,
					PreferredSearchType = TfDataProviderColumnSearchType.Contains
				};

				//empty id, but internaly we set new id
				var result = providerManager.CreateDataProviderColumn(column);
				result.IsSuccess.Should().BeTrue();
			}
		}
	}

	[Fact]
	public async Task Column_DataProviderId_Empty()
	{
		using (await locker.LockAsync())
		{
			ITfDataProviderManager providerManager = ServiceProvider.GetRequiredService<ITfDataProviderManager>();
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
					AutoDefaultValue = true,
					DefaultValue = null,
					DataProviderId = Guid.Empty,
					DbName = "textcolona",
					DbType = DatabaseColumnType.Text,
					SourceName = "source_column",
					SourceType = "TEXT",
					IncludeInTableSearch = true,
					IsNullable = false,
					IsSearchable = true,
					IsSortable = true,
					IsUnique = true,
					PreferredSearchType = TfDataProviderColumnSearchType.Contains
				};

				//empty id
				var result = providerManager.CreateDataProviderColumn(column);
				result.IsSuccess.Should().BeFalse();
				((ValidationError)result.Errors[0]).PropertyName.Should().Be(nameof(TfDataProviderColumn.DataProviderId));
			}
		}
	}

	[Fact]
	public async Task Column_SourceName_Empty()
	{
		using (await locker.LockAsync())
		{
			ITfDataProviderManager providerManager = ServiceProvider.GetRequiredService<ITfDataProviderManager>();
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
					AutoDefaultValue = true,
					DefaultValue = null,
					DataProviderId = provider.Id,
					DbName = "textcolona",
					DbType = DatabaseColumnType.Text,
					SourceName = "",
					SourceType = "TEXT",
					IncludeInTableSearch = true,
					IsNullable = false,
					IsSearchable = true,
					IsSortable = true,
					IsUnique = true,
					PreferredSearchType = TfDataProviderColumnSearchType.Contains
				};

				var result = providerManager.CreateDataProviderColumn(column);
				result.IsSuccess.Should().BeFalse();
				((ValidationError)result.Errors[0]).PropertyName.Should().Be(nameof(TfDataProviderColumn.SourceName));
			}
		}
	}

	[Fact]
	public async Task Column_SourceType_Empty()
	{
		using (await locker.LockAsync())
		{
			ITfDataProviderManager providerManager = ServiceProvider.GetRequiredService<ITfDataProviderManager>();
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
					AutoDefaultValue = true,
					DefaultValue = null,
					DataProviderId = provider.Id,
					DbName = "textcolona",
					DbType = DatabaseColumnType.Text,
					SourceName = "source",
					SourceType = "",
					IncludeInTableSearch = true,
					IsNullable = false,
					IsSearchable = true,
					IsSortable = true,
					IsUnique = true,
					PreferredSearchType = TfDataProviderColumnSearchType.Contains
				};

				var result = providerManager.CreateDataProviderColumn(column);
				result.IsSuccess.Should().BeFalse();
				((ValidationError)result.Errors[0]).PropertyName.Should().Be(nameof(TfDataProviderColumn.SourceType));
			}
		}
	}

	[Fact]
	public async Task Column_DefaultValue()
	{
		using (await locker.LockAsync())
		{
			ITfDataProviderManager providerManager = ServiceProvider.GetRequiredService<ITfDataProviderManager>();
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
					DefaultValue = Guid.NewGuid().ToString(),
					DataProviderId = provider.Id,
					DbName = "text",
					DbType = DatabaseColumnType.Text,
					SourceName = "source_column",
					SourceType = "TEXT",
					IncludeInTableSearch = true,
					IsNullable = false,
					IsSearchable = true,
					IsSortable = true,
					IsUnique = true,
					PreferredSearchType = TfDataProviderColumnSearchType.Contains
				};

				var result = providerManager.CreateDataProviderColumn(column);
				result.IsSuccess.Should().BeTrue();

				column = new TfDataProviderColumn
				{
					Id = Guid.NewGuid(),
					AutoDefaultValue = false,
					DefaultValue = string.Empty,
					DataProviderId = provider.Id,
					DbName = "short_text",
					DbType = DatabaseColumnType.ShortText,
					SourceName = "source_column",
					SourceType = "SHORT_TEXT",
					IncludeInTableSearch = true,
					IsNullable = false,
					IsSearchable = true,
					IsSortable = true,
					IsUnique = true,
					PreferredSearchType = TfDataProviderColumnSearchType.Equals
				};

				result = providerManager.CreateDataProviderColumn(column);
				result.IsSuccess.Should().BeTrue();

				column = new TfDataProviderColumn
				{
					Id = Guid.NewGuid(),
					AutoDefaultValue = false,
					DefaultValue = short.MaxValue.ToString(CultureInfo.InvariantCulture),
					DataProviderId = provider.Id,
					DbName = "short_int",
					DbType = DatabaseColumnType.ShortInteger,
					SourceName = "source_column",
					SourceType = "SHORT_INTEGER",
					IncludeInTableSearch = true,
					IsNullable = true,
					IsSearchable = false,
					IsSortable = false,
					IsUnique = false,
					PreferredSearchType = TfDataProviderColumnSearchType.Equals
				};

				result = providerManager.CreateDataProviderColumn(column);
				result.IsSuccess.Should().BeTrue();

				column = new TfDataProviderColumn
				{
					Id = Guid.NewGuid(),
					AutoDefaultValue = false,
					DefaultValue = int.MaxValue.ToString(CultureInfo.InvariantCulture),
					DataProviderId = provider.Id,
					DbName = "int",
					DbType = DatabaseColumnType.Integer,
					SourceName = "source_column",
					SourceType = "INTEGER",
					IncludeInTableSearch = true,
					IsNullable = true,
					IsSearchable = false,
					IsSortable = false,
					IsUnique = false,
					PreferredSearchType = TfDataProviderColumnSearchType.Equals
				};

				result = providerManager.CreateDataProviderColumn(column);
				result.IsSuccess.Should().BeTrue();

				column = new TfDataProviderColumn
				{
					Id = Guid.NewGuid(),
					AutoDefaultValue = true,
					DefaultValue = long.MaxValue.ToString(CultureInfo.InvariantCulture),
					DataProviderId = provider.Id,
					DbName = "long_int",
					DbType = DatabaseColumnType.LongInteger,
					SourceName = "source_column",
					SourceType = "LONG_INTEGER",
					IncludeInTableSearch = true,
					IsNullable = true,
					IsSearchable = false,
					IsSortable = false,
					IsUnique = false,
					PreferredSearchType = TfDataProviderColumnSearchType.Equals
				};

				result = providerManager.CreateDataProviderColumn(column);
				result.IsSuccess.Should().BeTrue();

				column = new TfDataProviderColumn
				{
					Id = Guid.NewGuid(),
					AutoDefaultValue = true,
					DefaultValue = decimal.MaxValue.ToString(CultureInfo.InvariantCulture),
					DataProviderId = provider.Id,
					DbName = "number",
					DbType = DatabaseColumnType.Number,
					SourceName = "source_column",
					SourceType = "NUMBER",
					IncludeInTableSearch = true,
					IsNullable = true,
					IsSearchable = false,
					IsSortable = false,
					IsUnique = false,
					PreferredSearchType = TfDataProviderColumnSearchType.Equals
				};

				result = providerManager.CreateDataProviderColumn(column);
				result.IsSuccess.Should().BeTrue();

				column = new TfDataProviderColumn
				{
					Id = Guid.NewGuid(),
					AutoDefaultValue = false,
					DefaultValue = "2024-06-27",
					DataProviderId = provider.Id,
					DbName = "date",
					DbType = DatabaseColumnType.Date,
					SourceName = "source_column",
					SourceType = "DATE",
					IncludeInTableSearch = true,
					IsNullable = true,
					IsSearchable = false,
					IsSortable = false,
					IsUnique = false,
					PreferredSearchType = TfDataProviderColumnSearchType.Equals
				};

				result = providerManager.CreateDataProviderColumn(column);
				result.IsSuccess.Should().BeTrue();

				column = new TfDataProviderColumn
				{
					Id = Guid.NewGuid(),
					AutoDefaultValue = false,
					DefaultValue = "2024-06-27 12:01",
					DataProviderId = provider.Id,
					DbName = "datetime",
					DbType = DatabaseColumnType.DateTime,
					SourceName = "source_column",
					SourceType = "DATETIME",
					IncludeInTableSearch = true,
					IsNullable = true,
					IsSearchable = false,
					IsSortable = false,
					IsUnique = false,
					PreferredSearchType = TfDataProviderColumnSearchType.Equals
				};

				result = providerManager.CreateDataProviderColumn(column);
				result.IsSuccess.Should().BeTrue();
			}
		}
	}

	[Fact]
	public async Task Column_CreateUpdate_TEXT()
	{
		using (await locker.LockAsync())
		{
			CreateAndUpdateColumnType(DatabaseColumnType.Text, "TEXT", "test def value");
		}
	}

	[Fact]
	public async Task Column_CreateUpdate_SHORT_TEXT()
	{
		using (await locker.LockAsync())
		{
			CreateAndUpdateColumnType(DatabaseColumnType.ShortText, "SHORT_TEXT", "test def value");
		}
	}


	[Fact]
	public async Task Column_CreateUpdate_NUMBER()
	{
		using (await locker.LockAsync())
		{
			CreateAndUpdateColumnType(DatabaseColumnType.Number, "NUMBER", "123.456");
		}
	}

	[Fact]
	public async Task Column_CreateUpdate_SHORT_INTEGER()
	{
		using (await locker.LockAsync())
		{
			CreateAndUpdateColumnType(DatabaseColumnType.ShortInteger, "SHORT_INTEGER", "1");
		}
	}

	[Fact]
	public async Task Column_CreateUpdate_INTEGER()
	{
		using (await locker.LockAsync())
		{
			CreateAndUpdateColumnType(DatabaseColumnType.Integer, "INTEGER", "1");
		}
	}

	[Fact]
	public async Task Column_CreateUpdate_LONG_INTEGER()
	{
		using (await locker.LockAsync())
		{
			CreateAndUpdateColumnType(DatabaseColumnType.LongInteger, "LONG_INTEGER", "1");
		}
	}

	[Fact]
	public async Task Column_CreateUpdate_DATE()
	{
		using (await locker.LockAsync())
		{
			CreateAndUpdateColumnType(DatabaseColumnType.Date, "DATE", "1975-10-25");
		}
	}

	[Fact]
	public async Task Column_CreateUpdate_DATETIME()
	{
		using (await locker.LockAsync())
		{
			CreateAndUpdateColumnType(DatabaseColumnType.DateTime, "DATETIME", "1975-10-25 20:25:45.123");
		}
	}

	[Fact]
	public async Task Column_CreateUpdate_BOOLEAN()
	{
		using (await locker.LockAsync())
		{
			CreateAndUpdateColumnType(DatabaseColumnType.Boolean, "BOOLEAN", "true");
		}
	}

	[Fact]
	public async Task Column_CreateUpdate_GUID()
	{
		using (await locker.LockAsync())
		{
			CreateAndUpdateColumnType(DatabaseColumnType.Guid, "GUID", Guid.NewGuid().ToString());
		}
	}


	private void CreateAndUpdateColumnType(
		DatabaseColumnType type,
		string sourceType,
		string defaultValue)
	{
		ITfDataProviderManager providerManager = ServiceProvider.GetRequiredService<ITfDataProviderManager>();
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
				DbType = type,
				SourceName = "source_column",
				SourceType = sourceType,
				IncludeInTableSearch = false,
				IsNullable = false,
				IsSearchable = true,
				IsSortable = true,
				IsUnique = true,
				PreferredSearchType = TfDataProviderColumnSearchType.Contains
			};

			var result = providerManager.CreateDataProviderColumn(column);
			result.IsSuccess.Should().BeTrue();

			var exColumn = result.Value.Columns.FirstOrDefault();

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

			result = providerManager.UpdateDataProviderColumn(column);
			result.IsSuccess.Should().BeTrue();

			exColumn = result.Value.Columns.FirstOrDefault();
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
	public async Task SharedKey_CreateUpdate()
	{
		using (await locker.LockAsync())
		{
			ITfDataProviderManager providerManager = ServiceProvider.GetRequiredService<ITfDataProviderManager>();
			IDatabaseService dbService = ServiceProvider.GetRequiredService<IDatabaseService>();

			using (var scope = dbService.CreateTransactionScope())
			{
				var provider = CreateSharedKeysStructure(providerManager);

				TfDataProviderSharedKey sharedKey =
					new TfDataProviderSharedKey
					{
						Id = Guid.NewGuid(),
						Description = "testing1",
						DataProviderId = provider.Id,
						DbName = "testing1",
						Columns = new() { provider.Columns[0] }

					};

				var providerResult = providerManager.CreateDataProviderSharedKey(sharedKey);
				providerResult.IsSuccess.Should().BeTrue();
			}
		}
	}

	private TfDataProvider CreateSharedKeysStructure(
		ITfDataProviderManager providerManager)
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
			AutoDefaultValue = true,
			DefaultValue = null,
			DataProviderId = provider.Id,
			DbName = "db_column",
			DbType = DatabaseColumnType.Text,
			SourceName = "source_column",
			SourceType = "TEXT",
			IncludeInTableSearch = false,
			IsNullable = false,
			IsSearchable = true,
			IsSortable = true,
			IsUnique = true,
			PreferredSearchType = TfDataProviderColumnSearchType.Contains
		};

		var result = providerManager.CreateDataProviderColumn(column);
		result.IsSuccess.Should().BeTrue();

		TfDataProviderColumn column2 = new TfDataProviderColumn
		{
			Id = Guid.NewGuid(),
			AutoDefaultValue = true,
			DefaultValue = null,
			DataProviderId = provider.Id,
			DbName = "db_column2",
			DbType = DatabaseColumnType.Text,
			SourceName = "source_column2",
			SourceType = "TEXT",
			IncludeInTableSearch = false,
			IsNullable = false,
			IsSearchable = true,
			IsSortable = true,
			IsUnique = true,
			PreferredSearchType = TfDataProviderColumnSearchType.Contains
		};

		result = providerManager.CreateDataProviderColumn(column2);
		result.IsSuccess.Should().BeTrue();

		return result.Value;
	}

	#endregion
}
