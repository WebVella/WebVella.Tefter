namespace WebVella.Tefter.Tests;

public partial class TfDataProviderManagerTests : BaseTest
{
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
				var providerType = providerTypesResult.Value.First();

				Guid id = Guid.NewGuid();
				TfDataProviderModel model = new TfDataProviderModel
				{
					Name = "test csv data provider",
					CompositeKeyPrefix = "pre_",
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
				var providerType = providerTypesResult.Value.First();

				Guid id = Guid.NewGuid();
				TfDataProviderModel model = new TfDataProviderModel
				{
					Name = "test csv data provider",
					CompositeKeyPrefix = "pre_",
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
				var providerType = providerTypesResult.Value.First();

				Guid id = Guid.NewGuid();
				TfDataProviderModel model = new TfDataProviderModel
				{
					Name = "test",
					CompositeKeyPrefix = "pre_",
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
				var providerType = providerTypesResult.Value.First();

				Guid id = Guid.NewGuid();
				TfDataProviderModel model = new TfDataProviderModel
				{
					Name = "",
					CompositeKeyPrefix = "pre_",
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
				var providerType = providerTypesResult.Value.First();

				Guid id = Guid.NewGuid();
				TfDataProviderModel model = new TfDataProviderModel
				{
					Name = "test csv data provider",
					CompositeKeyPrefix = "pre_",
					ProviderType = providerType,
					SettingsJson = null
				};
				var providerResult = providerManager.CreateDataProvider(model);
				providerResult.IsSuccess.Should().BeTrue();
				providerResult.Value.Should().BeOfType<TfDataProvider>();

				model.Id= Guid.NewGuid();

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
				var providerType = providerTypesResult.Value.First();

				Guid id = Guid.NewGuid();
				TfDataProviderModel model = new TfDataProviderModel
				{
					Name = "test csv data provider",
					CompositeKeyPrefix = "pre_",
					ProviderType = providerType,
					SettingsJson = null
				};
				var providerResult = providerManager.CreateDataProvider(model);
				providerResult.IsSuccess.Should().BeTrue();
				providerResult.Value.Should().BeOfType<TfDataProvider>();

				model.Name = "test csv data provider 1";
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
				var providerType = providerTypesResult.Value.First();

				TfDataProviderModel model = new TfDataProviderModel
				{
					Name = "test csv data provider",
					CompositeKeyPrefix = "pre_",
					ProviderType = providerType,
					SettingsJson = null
				};
				var providerResult = providerManager.CreateDataProvider(model);
				providerResult.IsSuccess.Should().BeTrue();
				providerResult.Value.Should().BeOfType<TfDataProvider>();

				TfDataProviderModel model2 = new TfDataProviderModel
				{
					Name = "test csv data provider 2",
					CompositeKeyPrefix = "pre_",
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
				var providerType = providerTypesResult.Value.First();

				TfDataProviderModel model = new TfDataProviderModel
				{
					Name = "test csv data provider",
					CompositeKeyPrefix = "pre_",
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
				var providerType = providerTypesResult.Value.First();

				TfDataProviderModel model = new TfDataProviderModel
				{
					Name = "test csv data provider",
					CompositeKeyPrefix = "pre_",
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


	[Fact]
	public async Task Column_Text()
	{
		using (await locker.LockAsync())
		{
			ITfDataProviderManager providerManager = ServiceProvider.GetRequiredService<ITfDataProviderManager>();
			IDatabaseService dbService = ServiceProvider.GetRequiredService<IDatabaseService>();

			using (var scope = dbService.CreateTransactionScope())
			{
				var providerTypesResult = providerManager.GetProviderTypes();
				var providerType = providerTypesResult.Value.First();

				Guid id = Guid.NewGuid();
				TfDataProviderModel model = new TfDataProviderModel
				{
					Name = "test csv data provider",
					CompositeKeyPrefix = "pre_",
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
					DbName = "test_text_column",
					DbType = DatabaseColumnType.Text,
					SourceName = "testova colona",
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
			}
		}
	}


}
