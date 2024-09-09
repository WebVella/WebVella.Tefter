using WebVella.Tefter.Core;

namespace WebVella.Tefter.Tests;

public partial class SpaceManagerTests : BaseTest
{
	[Fact]
	public async Task CRUD_Space()
	{
		using (await locker.LockAsync())
		{
			IDatabaseService dbService = ServiceProvider.GetRequiredService<IDatabaseService>();
			ITfSpaceManager spaceManager = ServiceProvider.GetRequiredService<ITfSpaceManager>();

			using (var scope = dbService.CreateTransactionScope(Constants.DB_OPERATION_LOCK_KEY))
			{


				var space1 = new TfSpace
				{
					Id = Guid.NewGuid(),
					Name = "Space1",
					Color = 10,
					Icon = "icon1",
					IsPrivate = false,
					Position = 0
				};
				var result = spaceManager.CreateSpace(space1);
				result.IsSuccess.Should().BeTrue();
				result.Value.Should().NotBeNull();
				result.Value.Id.Should().Be(space1.Id);
				result.Value.Name.Should().Be(space1.Name);
				result.Value.Position.Should().Be(1);
				result.Value.IsPrivate.Should().Be(space1.IsPrivate);
				result.Value.Icon.Should().Be(space1.Icon);
				result.Value.Color.Should().Be(space1.Color);

				var space2 = new TfSpace
				{
					Id = Guid.NewGuid(),
					Name = "Space2",
					Color = 12,
					Icon = "icon2",
					IsPrivate = false,
					Position = 0
				};
				result = spaceManager.CreateSpace(space2);
				result.IsSuccess.Should().BeTrue();
				result.Value.Should().NotBeNull();
				result.Value.Id.Should().Be(space2.Id);
				result.Value.Name.Should().Be(space2.Name);
				result.Value.Position.Should().Be(2);
				result.Value.IsPrivate.Should().Be(space2.IsPrivate);
				result.Value.Icon.Should().Be(space2.Icon);
				result.Value.Color.Should().Be(space2.Color);

				result = spaceManager.MoveSpaceDown(space1.Id);
				result.IsSuccess.Should().BeTrue();

				space1 = spaceManager.GetSpace(space1.Id).Value;
				space2 = spaceManager.GetSpace(space2.Id).Value;
				space1.Position.Should().Be(2);
				space2.Position.Should().Be(1);

				result = spaceManager.MoveSpaceUp(space1.Id);
				result.IsSuccess.Should().BeTrue();

				space1 = spaceManager.GetSpace(space1.Id).Value;
				space2 = spaceManager.GetSpace(space2.Id).Value;
				space1.Position.Should().Be(1);
				space2.Position.Should().Be(2);

				space1.Name = "updated name";
				space1.Icon = "updated icon";
				space1.Color = 100;
				space1.IsPrivate = true;

				result = spaceManager.UpdateSpace(space1);
				result.IsSuccess.Should().BeTrue();
				result.Value.Should().NotBeNull();
				result.Value.Id.Should().Be(space1.Id);
				result.Value.Name.Should().Be(space1.Name);
				result.Value.Position.Should().Be(1);
				result.Value.IsPrivate.Should().Be(space1.IsPrivate);
				result.Value.Icon.Should().Be(space1.Icon);
				result.Value.Color.Should().Be(space1.Color);

				result = spaceManager.DeleteSpace(space1.Id);
				result.IsSuccess.Should().BeTrue();
				space2 = spaceManager.GetSpace(space2.Id).Value;
				space2.Position.Should().Be(1);
			}
		}
	}

	[Fact]
	public async Task CRUD_SpaceData()
	{
		using (await locker.LockAsync())
		{
			IDatabaseService dbService = ServiceProvider.GetRequiredService<IDatabaseService>();
			ITfSpaceManager spaceManager = ServiceProvider.GetRequiredService<ITfSpaceManager>();
			ITfDataProviderManager providerManager = ServiceProvider.GetRequiredService<ITfDataProviderManager>();

			using (var scope = dbService.CreateTransactionScope(Constants.DB_OPERATION_LOCK_KEY))
			{
				var providerTypesResult = providerManager.GetProviderTypes();
				var providerType = providerTypesResult.Value
					.Single(x => x.Id == new Guid("90b7de99-4f7f-4a31-bcf9-9be988739d2d"));

				TfDataProviderModel providerModel = new TfDataProviderModel
				{
					Id = Guid.NewGuid(),
					Name = "test data provider",
					ProviderType = providerType,
					SettingsJson = null
				};
				var providerResult = providerManager.CreateDataProvider(providerModel);
				providerResult.IsSuccess.Should().BeTrue();
				providerResult.Value.Should().BeOfType<TfDataProvider>();

				var space = new TfSpace
				{
					Id = Guid.NewGuid(),
					Name = "Space1",
					Color = 10,
					Icon = "icon1",
					IsPrivate = false,
					Position = 0
				};
				spaceManager.CreateSpace(space).IsSuccess.Should().BeTrue();


				var spaceData1 = new TfSpaceData
				{
					Id = Guid.NewGuid(),
					DataProviderId = providerModel.Id,
					Name = "data1",
					SpaceId = space.Id,
				};

				var result = spaceManager.CreateSpaceData(spaceData1);
				result.IsSuccess.Should().BeTrue();
				result.Value.Should().NotBeNull();
				result.Value.Id.Should().Be(spaceData1.Id);
				result.Value.Name.Should().Be(spaceData1.Name);
				result.Value.SpaceId.Should().Be(space.Id);
				result.Value.Position.Should().Be(1);
				result.Value.Filters.Should().NotBeNull();
				result.Value.Filters.Count().Should().Be(0);


				var spaceData2 = new TfSpaceData
				{
					Id = Guid.NewGuid(),
					DataProviderId = providerModel.Id,
					Name = "data2",
					SpaceId = space.Id,
				};

				result = spaceManager.CreateSpaceData(spaceData2);
				result.IsSuccess.Should().BeTrue();
				result.Value.Should().NotBeNull();
				result.Value.Id.Should().Be(spaceData2.Id);
				result.Value.Name.Should().Be(spaceData2.Name);
				result.Value.SpaceId.Should().Be(space.Id);
				result.Value.Position.Should().Be(2);
				result.Value.Filters.Should().NotBeNull();
				result.Value.Filters.Count().Should().Be(0);

				result = spaceManager.MoveSpaceDataDown(spaceData1.Id);
				result.IsSuccess.Should().BeTrue();

				spaceData1 = spaceManager.GetSpaceData(spaceData1.Id).Value;
				spaceData2 = spaceManager.GetSpaceData(spaceData2.Id).Value;
				spaceData1.Position.Should().Be(2);
				spaceData2.Position.Should().Be(1);

				result = spaceManager.MoveSpaceDataUp(spaceData1.Id);
				result.IsSuccess.Should().BeTrue();

				spaceData1 = spaceManager.GetSpaceData(spaceData1.Id).Value;
				spaceData2 = spaceManager.GetSpaceData(spaceData2.Id).Value;
				spaceData1.Position.Should().Be(1);
				spaceData2.Position.Should().Be(2);

				spaceData1.Name = "updated name";
				result = spaceManager.UpdateSpaceData(spaceData1);
				result.IsSuccess.Should().BeTrue();
				result.Value.Name.Should().Be(spaceData1.Name);

				result = spaceManager.DeleteSpaceData(spaceData1.Id);
				result.IsSuccess.Should().BeTrue();

				spaceData2 = spaceManager.GetSpaceData(spaceData2.Id).Value;
				spaceData2.Position.Should().Be(1);

				var deleteSpaceResult = spaceManager.DeleteSpace(space.Id);
				deleteSpaceResult.IsSuccess.Should().BeTrue();

			}
		}
	}

	[Fact]
	public async Task SpaceData_TryToChangeSpaceId()
	{
		using (await locker.LockAsync())
		{
			IDatabaseService dbService = ServiceProvider.GetRequiredService<IDatabaseService>();
			ITfSpaceManager spaceManager = ServiceProvider.GetRequiredService<ITfSpaceManager>();
			ITfDataProviderManager providerManager = ServiceProvider.GetRequiredService<ITfDataProviderManager>();

			using (var scope = dbService.CreateTransactionScope(Constants.DB_OPERATION_LOCK_KEY))
			{
				var providerTypesResult = providerManager.GetProviderTypes();
				var providerType = providerTypesResult.Value
					.Single(x => x.Id == new Guid("90b7de99-4f7f-4a31-bcf9-9be988739d2d"));

				TfDataProviderModel providerModel = new TfDataProviderModel
				{
					Id = Guid.NewGuid(),
					Name = "test data provider",
					ProviderType = providerType,
					SettingsJson = null
				};
				var providerResult = providerManager.CreateDataProvider(providerModel);
				providerResult.IsSuccess.Should().BeTrue();
				providerResult.Value.Should().BeOfType<TfDataProvider>();


				var space = new TfSpace
				{
					Id = Guid.NewGuid(),
					Name = "Space1",
					Color = 10,
					Icon = "icon1",
					IsPrivate = false,
					Position = 0
				};
				spaceManager.CreateSpace(space).IsSuccess.Should().BeTrue();

				var space2 = new TfSpace
				{
					Id = Guid.NewGuid(),
					Name = "Space2",
					Color = 10,
					Icon = "icon1",
					IsPrivate = false,
					Position = 0
				};
				spaceManager.CreateSpace(space2).IsSuccess.Should().BeTrue();

				var spaceData1 = new TfSpaceData
				{
					Id = Guid.NewGuid(),
					DataProviderId = providerModel.Id,
					Name = "data1",
					SpaceId = space.Id,
				};

				var result = spaceManager.CreateSpaceData(spaceData1);
				result.IsSuccess.Should().BeTrue();
				result.Value.Should().NotBeNull();
				result.Value.Id.Should().Be(spaceData1.Id);
				result.Value.Name.Should().Be(spaceData1.Name);
				result.Value.SpaceId.Should().Be(space.Id);
				result.Value.Position.Should().Be(1);
				result.Value.Filters.Should().NotBeNull();
				result.Value.Filters.Count().Should().Be(0);


				spaceData1.SpaceId = space2.Id;
				result = spaceManager.UpdateSpaceData(spaceData1);
				result.IsSuccess.Should().BeFalse();
				result.Errors.Count().Should().Be(1);
				result.Errors[0].Should().BeOfType<ValidationError>();
				((ValidationError)result.Errors[0]).PropertyName.Should().Be("SpaceId");
				((ValidationError)result.Errors[0]).Reason.Should()
					.Be("Space cannot be changed for space data.");
			}
		}
	}

	[Fact]
	public async Task SpaceData_ColumnsManage()
	{
		using (await locker.LockAsync())
		{
			IDatabaseService dbService = ServiceProvider.GetRequiredService<IDatabaseService>();
			ITfSpaceManager spaceManager = ServiceProvider.GetRequiredService<ITfSpaceManager>();
			ITfDataProviderManager providerManager = ServiceProvider.GetRequiredService<ITfDataProviderManager>();

			using (var scope = dbService.CreateTransactionScope(Constants.DB_OPERATION_LOCK_KEY))
			{
				var providerTypesResult = providerManager.GetProviderTypes();
				var providerType = providerTypesResult.Value
					.Single(x => x.Id == new Guid("90b7de99-4f7f-4a31-bcf9-9be988739d2d"));

				TfDataProviderModel providerModel = new TfDataProviderModel
				{
					Id = Guid.NewGuid(),
					Name = "test data provider",
					ProviderType = providerType,
					SettingsJson = null
				};
				var providerResult = providerManager.CreateDataProvider(providerModel);
				providerResult.IsSuccess.Should().BeTrue();
				providerResult.Value.Should().BeOfType<TfDataProvider>();

				TfDataProviderColumn column = new TfDataProviderColumn
				{
					Id = Guid.Empty,
					AutoDefaultValue = true,
					DefaultValue = null,
					DataProviderId = providerModel.Id,
					DbName = "textcolona",
					DbType = DatabaseColumnType.Text,
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
				providerManager.CreateDataProviderColumn(column).IsSuccess.Should().BeTrue();

				var provider = providerManager.GetProvider(providerModel.Id).Value;

				TfDataProviderSharedKey sharedKey =
					new TfDataProviderSharedKey
					{
						Id = Guid.NewGuid(),
						Description = "testing1",
						DataProviderId = provider.Id,
						DbName = "testing1",
						Columns = new() { provider.Columns[0] }

					};

				providerManager.CreateDataProviderSharedKey(sharedKey).IsSuccess.Should().BeTrue();

				provider = providerManager.GetProvider(providerModel.Id).Value;
				provider.SharedKeys.Count().Should().Be(1);


				var space = new TfSpace
				{
					Id = Guid.NewGuid(),
					Name = "Space1",
					Color = 10,
					Icon = "icon1",
					IsPrivate = false,
					Position = 0
				};
				spaceManager.CreateSpace(space).IsSuccess.Should().BeTrue();

				var spaceData = new TfSpaceData
				{
					Id = Guid.NewGuid(),
					DataProviderId = providerModel.Id,
					Name = "data1",
					SpaceId = space.Id,
				};

				//TODO rumen to fix after change

				//var result = spaceManager.CreateSpaceData(spaceData);
				//result.IsSuccess.Should().BeTrue();
				//result.Value.Should().NotBeNull();
				//result.Value.Columns.Count().Should().Be(2);

				//result.Value.Columns[0].Selected = true;
				//result.Value.Columns[1].Selected = true;
				//spaceManager.UpdateSpaceData(spaceData);

				//spaceData = spaceManager.GetSpaceData(spaceData.Id).Value;
				//result.Value.Columns.Count().Should().Be(2);
				//result.Value.Columns[0].Selected.Should().BeTrue();
				//result.Value.Columns[1].Selected.Should().BeTrue();

				//result.Value.Columns[1].Selected = false;
				//spaceManager.UpdateSpaceData(spaceData);

				//spaceData = spaceManager.GetSpaceData(spaceData.Id).Value;
				//result.Value.Columns.Count().Should().Be(2);
				//result.Value.Columns[0].Selected.Should().BeTrue();
				//result.Value.Columns[1].Selected.Should().BeFalse();
			}
		}
	}

	[Fact]
	public async Task CRUD_SpaceView()
	{
		using (await locker.LockAsync())
		{
			IDatabaseService dbService = ServiceProvider.GetRequiredService<IDatabaseService>();
			ITfSpaceManager spaceManager = ServiceProvider.GetRequiredService<ITfSpaceManager>();
			ITfDataProviderManager providerManager = ServiceProvider.GetRequiredService<ITfDataProviderManager>();

			using (var scope = dbService.CreateTransactionScope(Constants.DB_OPERATION_LOCK_KEY))
			{
				var providerTypesResult = providerManager.GetProviderTypes();
				var providerType = providerTypesResult.Value
					.Single(x => x.Id == new Guid("90b7de99-4f7f-4a31-bcf9-9be988739d2d"));

				TfDataProviderModel providerModel = new TfDataProviderModel
				{
					Id = Guid.NewGuid(),
					Name = "test data provider",
					ProviderType = providerType,
					SettingsJson = null
				};
				var providerResult = providerManager.CreateDataProvider(providerModel);
				providerResult.IsSuccess.Should().BeTrue();
				providerResult.Value.Should().BeOfType<TfDataProvider>();

				var space = new TfSpace
				{
					Id = Guid.NewGuid(),
					Name = "Space1",
					Color = 10,
					Icon = "icon1",
					IsPrivate = false,
					Position = 0
				};

				var spaceResult = spaceManager.CreateSpace(space);
				spaceResult.IsSuccess.Should().BeTrue();
				spaceResult.Value.Should().NotBeNull();

				var spaceData = new TfSpaceData
				{
					Id = Guid.NewGuid(),
					DataProviderId = providerModel.Id,
					Name = "data1",
					SpaceId = space.Id,
				};

				var spaceDataResult = spaceManager.CreateSpaceData(spaceData);
				spaceDataResult.IsSuccess.Should().BeTrue();
				spaceDataResult.Value.Should().NotBeNull();


				TfSpaceView view = new TfSpaceView
				{
					Id = Guid.NewGuid(),
					Name = "view",
					Position = 1,
					SpaceDataId = spaceData.Id,
					SpaceId = space.Id,
					Type = TfSpaceViewType.Report
				};

				var spaceViewResult = spaceManager.CreateSpaceView(view);
				spaceViewResult.IsSuccess.Should().BeTrue();
				spaceViewResult.Value.Should().NotBeNull();
				spaceViewResult.Value.Id.Should().Be(view.Id);
				spaceViewResult.Value.Name.Should().Be(view.Name);
				spaceViewResult.Value.Position.Should().Be(view.Position);
				spaceViewResult.Value.SpaceDataId.Should().Be(view.SpaceDataId);
				spaceViewResult.Value.SpaceId.Should().Be(view.SpaceId);
				spaceViewResult.Value.Type.Should().Be(view.Type);


				view.Name = "view1";
				view.Type = TfSpaceViewType.Chart;

				spaceViewResult = spaceManager.UpdateSpaceView(view);
				spaceViewResult.IsSuccess.Should().BeTrue();
				spaceViewResult.Value.Should().NotBeNull();
				spaceViewResult.Value.Id.Should().Be(view.Id);
				spaceViewResult.Value.Name.Should().Be(view.Name);
				spaceViewResult.Value.Position.Should().Be(view.Position);
				spaceViewResult.Value.SpaceDataId.Should().Be(view.SpaceDataId);
				spaceViewResult.Value.SpaceId.Should().Be(view.SpaceId);
				spaceViewResult.Value.Type.Should().Be(view.Type);

				spaceViewResult = spaceManager.DeleteSpaceView(view.Id);
				spaceViewResult.IsSuccess.Should().BeTrue();
			}
		}
	}

	[Fact]
	public async Task CRUD_SpaceViewColumn()
	{
		using (await locker.LockAsync())
		{
			IDatabaseService dbService = ServiceProvider.GetRequiredService<IDatabaseService>();
			ITfSpaceManager spaceManager = ServiceProvider.GetRequiredService<ITfSpaceManager>();
			ITfDataProviderManager providerManager = ServiceProvider.GetRequiredService<ITfDataProviderManager>();

			using (var scope = dbService.CreateTransactionScope(Constants.DB_OPERATION_LOCK_KEY))
			{
				var providerTypesResult = providerManager.GetProviderTypes();
				var providerType = providerTypesResult.Value
					.Single(x => x.Id == new Guid("90b7de99-4f7f-4a31-bcf9-9be988739d2d"));

				TfDataProviderModel providerModel = new TfDataProviderModel
				{
					Id = Guid.NewGuid(),
					Name = "test data provider",
					ProviderType = providerType,
					SettingsJson = null
				};
				var providerResult = providerManager.CreateDataProvider(providerModel);
				providerResult.IsSuccess.Should().BeTrue();
				providerResult.Value.Should().BeOfType<TfDataProvider>();

				var space = new TfSpace
				{
					Id = Guid.NewGuid(),
					Name = "Space1",
					Color = 10,
					Icon = "icon1",
					IsPrivate = false,
					Position = 0
				};

				var spaceResult = spaceManager.CreateSpace(space);
				spaceResult.IsSuccess.Should().BeTrue();
				spaceResult.Value.Should().NotBeNull();

				var spaceData = new TfSpaceData
				{
					Id = Guid.NewGuid(),
					DataProviderId = providerModel.Id,
					Name = "data1",
					SpaceId = space.Id,
				};

				var spaceDataResult = spaceManager.CreateSpaceData(spaceData);
				spaceDataResult.IsSuccess.Should().BeTrue();
				spaceDataResult.Value.Should().NotBeNull();


				TfSpaceView view = new TfSpaceView
				{
					Id = Guid.NewGuid(),
					Name = "view",
					Position = 1,
					SpaceDataId = spaceData.Id,
					SpaceId = space.Id,
					Type = TfSpaceViewType.Report
				};

				var spaceViewResult = spaceManager.CreateSpaceView(view);
				spaceViewResult.IsSuccess.Should().BeTrue();
				spaceViewResult.Value.Should().NotBeNull();

				var availableColumnTypes = spaceManager.GetAvailableSpaceViewColumnTypes().Value;
				Type componentType = typeof(TfGeneralViewColumnComponent);

				List<TfSpaceViewColumn> createdColums = new List<TfSpaceViewColumn>();

				foreach (var availableColumnType in availableColumnTypes)
				{
					TfSpaceViewColumn column = new TfSpaceViewColumn
					{
						Id = Guid.NewGuid(),
						ColumnType = availableColumnType,
						QueryName = availableColumnType.Name,
						Title = availableColumnType.Name,
						ComponentType = componentType,
						SelectedAddonId = null,
						SpaceViewId = view.Id,
						CustomOptionsJson = "{}",
						DataMapping = new Dictionary<string, string> { { "Value", "test" } }
					};

					var columnResult = spaceManager.CreateSpaceViewColumn(column);
					columnResult.IsSuccess.Should().BeTrue();
					columnResult.Value.Should().NotBeNull();
					createdColums.Add(columnResult.Value);
				}

				var columns = spaceManager.GetSpaceViewColumnsList(view.Id).Value;
				columns.Count.Should().Be(availableColumnTypes.Count);

				var first = createdColums[0];
				var last = createdColums[createdColums.Count-1];

				first.Position = (short)(createdColums.Count);

				var updateResult = spaceManager.UpdateSpaceViewColumn(first);
				updateResult.IsSuccess.Should().BeTrue();
				updateResult.Value.Should().NotBeNull();

				columns = spaceManager.GetSpaceViewColumnsList(view.Id).Value;
				columns.Single(x=>x.Id == first.Id).Position.Should().Be((short)(createdColums.Count));
				columns.Single(x => x.Id == last.Id).Position.Should().Be((short)(createdColums.Count-1));

				last = columns.Single(x => x.Id == first.Id);

				for (int i = 1; i < columns.Count; i++)
				{
					var upResult = spaceManager.MoveSpaceViewColumnUp(last.Id);
					updateResult.IsSuccess.Should().BeTrue();

					columns = spaceManager.GetSpaceViewColumnsList(view.Id).Value;
					var column = columns.Single(x => x.Id == last.Id);
					column.Position.Should().Be((short)(columns.Count - i));
				}
			}
		}
	}

}
