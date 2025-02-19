using Bogus;
using System;
using WebVella.Tefter.Exceptions;
using WebVella.Tefter.Models;
using WebVella.Tefter.Web.ViewColumns;

namespace WebVella.Tefter.Tests;

public partial class SpaceManagerTests : BaseTest
{
	[Fact]
	public async Task CRUD_Space()
	{
		using (await locker.LockAsync())
		{
			ITfDatabaseService dbService = ServiceProvider.GetRequiredService<ITfDatabaseService>();
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
				result.Should().NotBeNull();
				result.Id.Should().Be(space1.Id);
				result.Name.Should().Be(space1.Name);
				result.Position.Should().Be(1);
				result.IsPrivate.Should().Be(space1.IsPrivate);
				result.Icon.Should().Be(space1.Icon);
				result.Color.Should().Be(space1.Color);

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
				result.Should().NotBeNull();
				result.Id.Should().Be(space2.Id);
				result.Name.Should().Be(space2.Name);
				result.Position.Should().Be(2);
				result.IsPrivate.Should().Be(space2.IsPrivate);
				result.Icon.Should().Be(space2.Icon);
				result.Color.Should().Be(space2.Color);

				spaceManager.MoveSpaceDown(space1.Id);
				space1 = spaceManager.GetSpace(space1.Id);
				space2 = spaceManager.GetSpace(space2.Id);
				space1.Position.Should().Be(2);
				space2.Position.Should().Be(1);

				spaceManager.MoveSpaceUp(space1.Id);
				space1 = spaceManager.GetSpace(space1.Id);
				space2 = spaceManager.GetSpace(space2.Id);
				space1.Position.Should().Be(1);
				space2.Position.Should().Be(2);

				space1.Name = "updated name";
				space1.Icon = "updated icon";
				space1.Color = 100;
				space1.IsPrivate = true;

				result = spaceManager.UpdateSpace(space1);
				result.Should().NotBeNull();
				result.Id.Should().Be(space1.Id);
				result.Name.Should().Be(space1.Name);
				result.Position.Should().Be(1);
				result.IsPrivate.Should().Be(space1.IsPrivate);
				result.Icon.Should().Be(space1.Icon);
				result.Color.Should().Be(space1.Color);

				spaceManager.DeleteSpace(space1.Id);
				space2 = spaceManager.GetSpace(space2.Id);
				space2.Position.Should().Be(1);
			}
		}
	}

	[Fact]
	public async Task CRUD_SpaceData()
	{
		using (await locker.LockAsync())
		{
			ITfDatabaseService dbService = ServiceProvider.GetRequiredService<ITfDatabaseService>();
			ITfSpaceManager spaceManager = ServiceProvider.GetRequiredService<ITfSpaceManager>();
			ITfDataProviderManager providerManager = ServiceProvider.GetRequiredService<ITfDataProviderManager>();

			using (var scope = dbService.CreateTransactionScope(Constants.DB_OPERATION_LOCK_KEY))
			{
				var providerTypes = providerManager.GetProviderTypes();
				var providerType = providerTypes
					.Single(x => x.Id == new Guid("90b7de99-4f7f-4a31-bcf9-9be988739d2d"));

				TfDataProviderModel providerModel = new TfDataProviderModel
				{
					Id = Guid.NewGuid(),
					Name = "test data provider",
					ProviderType = providerType,
					SettingsJson = null
				};
				
				var provider = providerManager.CreateDataProvider(providerModel);
				provider.Should().BeOfType<TfDataProvider>();

				var space = new TfSpace
				{
					Id = Guid.NewGuid(),
					Name = "Space1",
					Color = 10,
					Icon = "icon1",
					IsPrivate = false,
					Position = 0
				};
				spaceManager.CreateSpace(space);


				var spaceData1 = new TfSpaceData
				{
					Id = Guid.NewGuid(),
					DataProviderId = providerModel.Id,
					Name = "data1",
					SpaceId = space.Id,
				};

				var result = spaceManager.CreateSpaceData(spaceData1);
				result.Should().NotBeNull();
				result.Id.Should().Be(spaceData1.Id);
				result.Name.Should().Be(spaceData1.Name);
				result.SpaceId.Should().Be(space.Id);
				result.Position.Should().Be(1);
				result.Filters.Should().NotBeNull();
				result.Filters.Count().Should().Be(0);


				var spaceData2 = new TfSpaceData
				{
					Id = Guid.NewGuid(),
					DataProviderId = providerModel.Id,
					Name = "data2",
					SpaceId = space.Id,
				};

				result = spaceManager.CreateSpaceData(spaceData2);
				result.Should().NotBeNull();
				result.Id.Should().Be(spaceData2.Id);
				result.Name.Should().Be(spaceData2.Name);
				result.SpaceId.Should().Be(space.Id);
				result.Position.Should().Be(2);
				result.Filters.Should().NotBeNull();
				result.Filters.Count().Should().Be(0);

				spaceManager.MoveSpaceDataDown(spaceData1.Id);
				spaceData1 = spaceManager.GetSpaceData(spaceData1.Id);
				spaceData2 = spaceManager.GetSpaceData(spaceData2.Id);
				spaceData1.Position.Should().Be(2);
				spaceData2.Position.Should().Be(1);

				spaceManager.MoveSpaceDataUp(spaceData1.Id);
				spaceData1 = spaceManager.GetSpaceData(spaceData1.Id);
				spaceData2 = spaceManager.GetSpaceData(spaceData2.Id);
				spaceData1.Position.Should().Be(1);
				spaceData2.Position.Should().Be(2);

				spaceData1.Name = "updated name";
				result = spaceManager.UpdateSpaceData(spaceData1);
				result.Name.Should().Be(spaceData1.Name);

				spaceManager.DeleteSpaceData(spaceData1.Id);
				spaceData2 = spaceManager.GetSpaceData(spaceData2.Id);
				spaceData2.Position.Should().Be(1);

				spaceManager.DeleteSpace(space.Id);
			}
		}
	}

	[Fact]
	public async Task SpaceData_TryToChangeSpaceId()
	{
		using (await locker.LockAsync())
		{
			ITfDatabaseService dbService = ServiceProvider.GetRequiredService<ITfDatabaseService>();
			ITfSpaceManager spaceManager = ServiceProvider.GetRequiredService<ITfSpaceManager>();
			ITfDataProviderManager providerManager = ServiceProvider.GetRequiredService<ITfDataProviderManager>();

			using (var scope = dbService.CreateTransactionScope(Constants.DB_OPERATION_LOCK_KEY))
			{
				var providerTypes = providerManager.GetProviderTypes();
				var providerType = providerTypes
					.Single(x => x.Id == new Guid("90b7de99-4f7f-4a31-bcf9-9be988739d2d"));

				TfDataProviderModel providerModel = new TfDataProviderModel
				{
					Id = Guid.NewGuid(),
					Name = "test data provider",
					ProviderType = providerType,
					SettingsJson = null
				};
				var provider = providerManager.CreateDataProvider(providerModel);
				provider.Should().BeOfType<TfDataProvider>();


				var space = new TfSpace
				{
					Id = Guid.NewGuid(),
					Name = "Space1",
					Color = 10,
					Icon = "icon1",
					IsPrivate = false,
					Position = 0
				};
				spaceManager.CreateSpace(space);

				var space2 = new TfSpace
				{
					Id = Guid.NewGuid(),
					Name = "Space2",
					Color = 10,
					Icon = "icon1",
					IsPrivate = false,
					Position = 0
				};
				spaceManager.CreateSpace(space2);

				var spaceData1 = new TfSpaceData
				{
					Id = Guid.NewGuid(),
					DataProviderId = providerModel.Id,
					Name = "data1",
					SpaceId = space.Id,
				};

				var result = spaceManager.CreateSpaceData(spaceData1);
				result.Should().NotBeNull();
				result.Id.Should().Be(spaceData1.Id);
				result.Name.Should().Be(spaceData1.Name);
				result.SpaceId.Should().Be(space.Id);
				result.Position.Should().Be(1);
				result.Filters.Should().NotBeNull();
				result.Filters.Count().Should().Be(0);


				spaceData1.SpaceId = space2.Id;
				var task = Task.Run(() => { result = spaceManager.UpdateSpaceData(spaceData1); });
				var exception = Record.ExceptionAsync(async () => await task).Result;
				exception.Should().NotBeNull();
				exception.Should().BeOfType(typeof(TfValidationException));
				exception.Data.Keys.Count.Should().Be(1);
				exception.Data.Contains(nameof(TfSpaceData.SpaceId)).Should().BeTrue();
			}
		}
	}

	[Fact]
	public async Task SpaceData_ColumnsManage()
	{
		using (await locker.LockAsync())
		{
			ITfDatabaseService dbService = ServiceProvider.GetRequiredService<ITfDatabaseService>();
			ITfSpaceManager spaceManager = ServiceProvider.GetRequiredService<ITfSpaceManager>();
			ITfDataProviderManager providerManager = ServiceProvider.GetRequiredService<ITfDataProviderManager>();

			using (var scope = dbService.CreateTransactionScope(Constants.DB_OPERATION_LOCK_KEY))
			{
				var providerTypes = providerManager.GetProviderTypes();
				var providerType = providerTypes
					.Single(x => x.Id == new Guid("90b7de99-4f7f-4a31-bcf9-9be988739d2d"));

				TfDataProviderModel providerModel = new TfDataProviderModel
				{
					Id = Guid.NewGuid(),
					Name = "test data provider",
					ProviderType = providerType,
					SettingsJson = null
				};
				var provider = providerManager.CreateDataProvider(providerModel);
				provider.Should().BeOfType<TfDataProvider>();

				TfDataProviderColumn column = new TfDataProviderColumn
				{
					Id = Guid.Empty,
					AutoDefaultValue = true,
					DefaultValue = null,
					DataProviderId = providerModel.Id,
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

				//empty id, but internaly we set new id
				providerManager.CreateDataProviderColumn(column);

				provider = providerManager.GetProvider(providerModel.Id);

				TfDataProviderSharedKey sharedKey =
					new TfDataProviderSharedKey
					{
						Id = Guid.NewGuid(),
						Description = "testing1",
						DataProviderId = provider.Id,
						DbName = "testing1",
						Columns = new() { provider.Columns[0] }

					};

				providerManager.CreateDataProviderSharedKey(sharedKey);

				provider = providerManager.GetProvider(providerModel.Id);
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
				spaceManager.CreateSpace(space);

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
	public async Task CRUD_Bookmark()
	{
		using (await locker.LockAsync())
		{
			IIdentityManager identityManager = ServiceProvider.GetRequiredService<IIdentityManager>();
			ITfDatabaseService dbService = ServiceProvider.GetRequiredService<ITfDatabaseService>();
			ITfSpaceManager spaceManager = ServiceProvider.GetRequiredService<ITfSpaceManager>();
			ITfDataProviderManager providerManager = ServiceProvider.GetRequiredService<ITfDataProviderManager>();

			using (var scope = dbService.CreateTransactionScope(Constants.DB_OPERATION_LOCK_KEY))
			{


				var role = identityManager
						.CreateRoleBuilder()
						.WithName("UnitTester")
						.Build();

				role = await identityManager.SaveRoleAsync(role);
				role.Should().NotBeNull();

				var user = identityManager
					.CreateUserBuilder()
					.WithEmail("test@test.com")
					.WithPassword("password")
					.WithFirstName("firstname")
					.WithLastName("lastname")
					.CreatedOn(DateTime.Now)
					.Enabled(true)
					.WithRoles(role)
					.Build();

				user = await identityManager.SaveUserAsync(user);
				user.Should().NotBeNull();

				user = await identityManager.GetUserAsync("test@test.com", "password");
				user.Should().NotBeNull();

				var providerTypes = providerManager.GetProviderTypes();
				var providerType = providerTypes
					.Single(x => x.Id == new Guid("90b7de99-4f7f-4a31-bcf9-9be988739d2d"));

				TfDataProviderModel providerModel = new TfDataProviderModel
				{
					Id = Guid.NewGuid(),
					Name = "test data provider",
					ProviderType = providerType,
					SettingsJson = null
				};
				var provider = providerManager.CreateDataProvider(providerModel);
				provider.Should().BeOfType<TfDataProvider>();

				var space = new TfSpace
				{
					Id = Guid.NewGuid(),
					Name = "Space1",
					Color = 10,
					Icon = "icon1",
					IsPrivate = false,
					Position = 0
				};

				space = spaceManager.CreateSpace(space);
				space.Should().NotBeNull();

				var spaceData = new TfSpaceData
				{
					Id = Guid.NewGuid(),
					DataProviderId = providerModel.Id,
					Name = "data1",
					SpaceId = space.Id,
				};

				spaceData = spaceManager.CreateSpaceData(spaceData);
				spaceData.Should().NotBeNull();


				TfSpaceView view = new TfSpaceView
				{
					Id = Guid.NewGuid(),
					Name = "view",
					Position = 1,
					SpaceDataId = spaceData.Id,
					SpaceId = space.Id,
					Type = TfSpaceViewType.DataGrid
				};

				var spaceView = spaceManager.CreateSpaceView(view);
				spaceView.Should().NotBeNull();
				spaceView.Id.Should().Be(view.Id);
				spaceView.Name.Should().Be(view.Name);
				spaceView.Position.Should().Be(view.Position);
				spaceView.SpaceDataId.Should().Be(view.SpaceDataId);
				spaceView.SpaceId.Should().Be(view.SpaceId);
				spaceView.Type.Should().Be(view.Type);


				var bookmarkList = spaceManager.GetBookmarksListForUser(user.Id);

				var bookmarkModel = new TfBookmark
				{
					Id = Guid.NewGuid(),
					Name = "test1",
					Description = " test with #tag1 #tAg2 #Tag3",
					SpaceViewId = view.Id,
					Url = null,
					UserId = user.Id,
					CreatedOn = DateTime.UtcNow
				};

				var bookmark = spaceManager.CreateBookmark(bookmarkModel);
				bookmark.Description = " test with #tag_1 #tAg_2 #Tag3";
				bookmark = spaceManager.UpdateBookmark(bookmark);

				spaceManager.GetBookmarksListForUser(user.Id).Count.Should().Be(1);
				spaceManager.DeleteBookmark(bookmark.Id);
				spaceManager.GetBookmarksListForUser(user.Id).Count.Should().Be(0);
			}
		}
	}


	[Fact]
	public async Task CRUD_SpaceView()
	{
		using (await locker.LockAsync())
		{
			ITfDatabaseService dbService = ServiceProvider.GetRequiredService<ITfDatabaseService>();
			ITfSpaceManager spaceManager = ServiceProvider.GetRequiredService<ITfSpaceManager>();
			ITfDataProviderManager providerManager = ServiceProvider.GetRequiredService<ITfDataProviderManager>();

			using (var scope = dbService.CreateTransactionScope(Constants.DB_OPERATION_LOCK_KEY))
			{
				var providerTypes = providerManager.GetProviderTypes();
				var providerType = providerTypes
					.Single(x => x.Id == new Guid("90b7de99-4f7f-4a31-bcf9-9be988739d2d"));

				TfDataProviderModel providerModel = new TfDataProviderModel
				{
					Id = Guid.NewGuid(),
					Name = "test data provider",
					ProviderType = providerType,
					SettingsJson = null
				};
				var provider = providerManager.CreateDataProvider(providerModel);
				provider.Should().BeOfType<TfDataProvider>();

				var space = new TfSpace
				{
					Id = Guid.NewGuid(),
					Name = "Space1",
					Color = 10,
					Icon = "icon1",
					IsPrivate = false,
					Position = 0
				};

				space = spaceManager.CreateSpace(space);
				space.Should().NotBeNull();

				var spaceData = new TfSpaceData
				{
					Id = Guid.NewGuid(),
					DataProviderId = providerModel.Id,
					Name = "data1",
					SpaceId = space.Id,
				};

				spaceData = spaceManager.CreateSpaceData(spaceData);
				spaceData.Should().NotBeNull();


				TfSpaceView view = new TfSpaceView
				{
					Id = Guid.NewGuid(),
					Name = "view",
					Position = 1,
					SpaceDataId = spaceData.Id,
					SpaceId = space.Id,
					Type = TfSpaceViewType.DataGrid
				};

				var spaceView = spaceManager.CreateSpaceView(view);
				spaceView.Should().NotBeNull();
				spaceView.Id.Should().Be(view.Id);
				spaceView.Name.Should().Be(view.Name);
				spaceView.Position.Should().Be(view.Position);
				spaceView.SpaceDataId.Should().Be(view.SpaceDataId);
				spaceView.SpaceId.Should().Be(view.SpaceId);
				spaceView.Type.Should().Be(view.Type);


				view.Name = "view1";
				view.Type = TfSpaceViewType.Chart;

				spaceView = spaceManager.UpdateSpaceView(view);
				spaceView.Should().NotBeNull();
				spaceView.Id.Should().Be(view.Id);
				spaceView.Name.Should().Be(view.Name);
				spaceView.Position.Should().Be(view.Position);
				spaceView.SpaceDataId.Should().Be(view.SpaceDataId);
				spaceView.SpaceId.Should().Be(view.SpaceId);
				spaceView.Type.Should().Be(view.Type);

				spaceManager.DeleteSpaceView(view.Id);
			}
		}
	}

	[Fact]
	public async Task CRUD_SpaceViewColumn()
	{
		using (await locker.LockAsync())
		{
			ITfDatabaseService dbService = ServiceProvider.GetRequiredService<ITfDatabaseService>();
			ITfSpaceManager spaceManager = ServiceProvider.GetRequiredService<ITfSpaceManager>();
			ITfDataProviderManager providerManager = ServiceProvider.GetRequiredService<ITfDataProviderManager>();

			using (var scope = dbService.CreateTransactionScope(Constants.DB_OPERATION_LOCK_KEY))
			{
				var providerTypes = providerManager.GetProviderTypes();
				var providerType = providerTypes
					.Single(x => x.Id == new Guid("90b7de99-4f7f-4a31-bcf9-9be988739d2d"));

				TfDataProviderModel providerModel = new TfDataProviderModel
				{
					Id = Guid.NewGuid(),
					Name = "test data provider",
					ProviderType = providerType,
					SettingsJson = null
				};
				var provider = providerManager.CreateDataProvider(providerModel);
				provider.Should().BeOfType<TfDataProvider>();

				var space = new TfSpace
				{
					Id = Guid.NewGuid(),
					Name = "Space1",
					Color = 10,
					Icon = "icon1",
					IsPrivate = false,
					Position = 0
				};

				space = spaceManager.CreateSpace(space);
				space.Should().NotBeNull();

				var spaceData = new TfSpaceData
				{
					Id = Guid.NewGuid(),
					DataProviderId = providerModel.Id,
					Name = "data1",
					SpaceId = space.Id,
				};

				spaceData = spaceManager.CreateSpaceData(spaceData);
				spaceData.Should().NotBeNull();

				TfSpaceView view = new TfSpaceView
				{
					Id = Guid.NewGuid(),
					Name = "view",
					Position = 1,
					SpaceDataId = spaceData.Id,
					SpaceId = space.Id,
					Type = TfSpaceViewType.DataGrid
				};

				var spaceView = spaceManager.CreateSpaceView(view);
				spaceView.Should().NotBeNull();

				var availableColumnTypes = spaceManager.GetAvailableSpaceViewColumnTypes();
				Type componentType = typeof(TfTextDisplayColumnComponent);

				List<TfSpaceViewColumn> createdColums = new List<TfSpaceViewColumn>();

				foreach (var availableColumnType in availableColumnTypes)
				{

					TfSpaceViewColumn column = new TfSpaceViewColumn
					{
						Id = Guid.NewGuid(),
						ColumnType = availableColumnType,
						QueryName = availableColumnType.Name
											.ToLower()
											.Replace(".", "")
											.Replace(" ", "")
											.Replace("(", "")
											.Replace(")", ""),
						Title = availableColumnType.Name,
						ComponentType = componentType,
						SpaceViewId = view.Id,
						CustomOptionsJson = "{}",
						DataMapping = new Dictionary<string, string> { { "Value", "test" } }
					};

					var createdSpaceViewColumn = spaceManager.CreateSpaceViewColumn(column);
					createdSpaceViewColumn.Should().NotBeNull();
					createdColums.Add(createdSpaceViewColumn);
				}

				var columns = spaceManager.GetSpaceViewColumnsList(view.Id);
				columns.Count.Should().Be(availableColumnTypes.Count);

				var first = createdColums[0];
				var last = createdColums[createdColums.Count - 1];

				first.Position = (short)(createdColums.Count);

				var updateResult = spaceManager.UpdateSpaceViewColumn(first);
				updateResult.Should().NotBeNull();

				columns = spaceManager.GetSpaceViewColumnsList(view.Id);
				columns.Single(x => x.Id == first.Id).Position.Should().Be((short)(createdColums.Count));
				columns.Single(x => x.Id == last.Id).Position.Should().Be((short)(createdColums.Count - 1));

				last = columns.Single(x => x.Id == first.Id);

				for (int i = 1; i < columns.Count; i++)
				{
					spaceManager.MoveSpaceViewColumnUp(last.Id);

					columns = spaceManager.GetSpaceViewColumnsList(view.Id);
					var column = columns.Single(x => x.Id == last.Id);
					column.Position.Should().Be((short)(columns.Count - i));
				}

				//test delete the entire space
				spaceManager.DeleteSpace(space.Id);
			}
		}
	}


	[Fact]
	public async Task CRUD_SpaceNode()
	{
		using (await locker.LockAsync())
		{
			ITfDatabaseService dbService = ServiceProvider.GetRequiredService<ITfDatabaseService>();
			ITfSpaceManager spaceManager = ServiceProvider.GetRequiredService<ITfSpaceManager>();

			using (var scope = dbService.CreateTransactionScope(Constants.DB_OPERATION_LOCK_KEY))
			{
				#region Create Structure

				var space = new TfSpace
				{
					Id = Guid.NewGuid(),
					Name = "Space1",
					Color = 10,
					Icon = "icon1",
					IsPrivate = false,
					Position = 0
				};
				spaceManager.CreateSpace(space);

				
				var spaceNode1_0_0 = new TfSpaceNode
				{
					Id = Guid.NewGuid(),
					ComponentOptionsJson = null,
					ComponentTypeFullName = null,
					Icon = null,
					Name = "1_0_0",
					ParentId = null,
					Position = null,
					SpaceId = space.Id,
					Type = TfSpaceNodeType.Folder
				};

				spaceManager.CreateSpaceNode(spaceNode1_0_0);

				var spaceNode1_1_0 = new TfSpaceNode
				{
					Id = Guid.NewGuid(),
					ComponentOptionsJson = "{}",
					ComponentTypeFullName = "comp_type",
					Icon = "1_1_0",
					Name = "1_1_0",
					ParentId = spaceNode1_0_0.Id,
					Position = null,
					SpaceId = space.Id,
					Type = TfSpaceNodeType.Folder
				};

				spaceManager.CreateSpaceNode(spaceNode1_1_0);

				var spaceNode1_2_0 = new TfSpaceNode
				{
					Id = Guid.NewGuid(),
					ComponentOptionsJson = "{}",
					ComponentTypeFullName = "comp_type",
					Icon = "1_2_0",
					Name = "1_2_0",
					ParentId = spaceNode1_0_0.Id,
					Position = null,
					SpaceId = space.Id,
					Type = TfSpaceNodeType.Folder
				};

				spaceManager.CreateSpaceNode(spaceNode1_2_0);
				
				var spaceNode1_3_0 = new TfSpaceNode
				{
					Id = Guid.NewGuid(),
					ComponentOptionsJson = "{}",
					ComponentTypeFullName = "comp_type",
					Icon = "1_3_0",
					Name = "1_3_0",
					ParentId = spaceNode1_0_0.Id,
					Position = null,
					SpaceId = space.Id,
					Type = TfSpaceNodeType.Folder
				};

				spaceManager.CreateSpaceNode(spaceNode1_3_0);

				var spaceNode2_0_0 = new TfSpaceNode
				{
					Id = Guid.NewGuid(),
					ComponentOptionsJson = "{}",
					ComponentTypeFullName = "comp_type",
					Icon = "2_0_0",
					Name = "2_0_0",
					ParentId = null,
					Position = null,
					SpaceId = space.Id,
					Type = TfSpaceNodeType.Folder
				};

				spaceManager.CreateSpaceNode(spaceNode2_0_0);

				var spaceNode2_1_0 = new TfSpaceNode
				{
					Id = Guid.NewGuid(),
					ComponentOptionsJson = "{}",
					ComponentTypeFullName = "comp_type",
					Icon = "2_1_0",
					Name = "2_1_0",
					ParentId = spaceNode2_0_0.Id,
					Position = null,
					SpaceId = space.Id,
					Type = TfSpaceNodeType.Folder
				};

				spaceManager.CreateSpaceNode(spaceNode2_1_0);

				var spaceNode2_2_0 = new TfSpaceNode
				{
					Id = Guid.NewGuid(),
					ComponentOptionsJson = "{}",
					ComponentTypeFullName = "comp_type",
					Icon = "2_2_0",
					Name = "2_2_0",
					ParentId = spaceNode2_0_0.Id,
					Position = null,
					SpaceId = space.Id,
					Type = TfSpaceNodeType.Folder
				};

				spaceManager.CreateSpaceNode(spaceNode2_2_0);
				
				var spaceNode2_3_0 = new TfSpaceNode
				{
					Id = Guid.NewGuid(),
					ComponentOptionsJson = "{}",
					ComponentTypeFullName = "comp_type",
					Icon = "2_3_0",
					Name = "2_3_0",
					ParentId = spaceNode2_0_0.Id,
					Position = null,
					SpaceId = space.Id,
					Type = TfSpaceNodeType.Folder
				};

				spaceManager.CreateSpaceNode(spaceNode2_3_0);

				var spaceNode3_0_0 = new TfSpaceNode
				{
					Id = Guid.NewGuid(),
					ComponentOptionsJson = "{}",
					ComponentTypeFullName = "comp_type",
					Icon = "3_0_0",
					Name = "3_0_0",
					ParentId = null,
					Position = null,
					SpaceId = space.Id,
					Type = TfSpaceNodeType.Folder
				};

				spaceManager.CreateSpaceNode(spaceNode3_0_0);
				
				var spaceNode3_1_0 = new TfSpaceNode
				{
					Id = Guid.NewGuid(),
					ComponentOptionsJson = "{}",
					ComponentTypeFullName = "comp_type",
					Icon = "3_1_0",
					Name = "3_1_0",
					ParentId = spaceNode3_0_0.Id,
					Position = null,
					SpaceId = space.Id,
					Type = TfSpaceNodeType.Folder
				};

				spaceManager.CreateSpaceNode(spaceNode3_1_0);

				var spaceNode3_2_0 = new TfSpaceNode
				{
					Id = Guid.NewGuid(),
					ComponentOptionsJson = "{}",
					ComponentTypeFullName = "comp_type",
					Icon = "3_2_0",
					Name = "3_2_0",
					ParentId = spaceNode3_0_0.Id,
					Position = null,
					SpaceId = space.Id,
					Type = TfSpaceNodeType.Folder
				};

				spaceManager.CreateSpaceNode(spaceNode3_2_0);
				
				var spaceNode3_3_0 = new TfSpaceNode
				{
					Id = Guid.NewGuid(),
					ComponentOptionsJson = "{}",
					ComponentTypeFullName = "comp_type",
					Icon = "3_3_0",
					Name = "3_3_0",
					ParentId = spaceNode3_0_0.Id,
					Position = null,
					SpaceId = space.Id,
					Type = TfSpaceNodeType.Folder
				};

				spaceManager.CreateSpaceNode(spaceNode3_3_0);

				#endregion

				#region move up/down in same parent node

				//test move up in same parent node
				spaceNode1_3_0.Position = 1;
				var nodeTree = spaceManager.UpdateSpaceNode(spaceNode1_3_0);
	
				var updatedSpaceNode1_1_0 = FindNodeById(spaceNode1_1_0.Id, nodeTree);
				updatedSpaceNode1_1_0.Position.Should().Be(2);

				var updatedSpaceNode1_2_0 = FindNodeById(spaceNode1_2_0.Id, nodeTree);
				updatedSpaceNode1_2_0.Position.Should().Be(3);

				var updatedSpaceNode1_3_0 = FindNodeById(spaceNode1_3_0.Id, nodeTree);
				updatedSpaceNode1_3_0.Position.Should().Be(1);


				//test move down in same parent node
				spaceNode1_3_0.Position = 3;
				nodeTree = spaceManager.UpdateSpaceNode(spaceNode1_3_0);

				updatedSpaceNode1_1_0 = FindNodeById(spaceNode1_1_0.Id, nodeTree);
				updatedSpaceNode1_1_0.Position.Should().Be(1);

				updatedSpaceNode1_2_0 = FindNodeById(spaceNode1_2_0.Id, nodeTree);
				updatedSpaceNode1_2_0.Position.Should().Be(2);

				updatedSpaceNode1_3_0 = FindNodeById(spaceNode1_3_0.Id, nodeTree);
				updatedSpaceNode1_3_0.Position.Should().Be(3);

				//test move up in same parent node
				spaceNode1_3_0.Position = 2;
				nodeTree = spaceManager.UpdateSpaceNode(spaceNode1_3_0);

				updatedSpaceNode1_1_0 = FindNodeById(spaceNode1_1_0.Id, nodeTree);
				updatedSpaceNode1_1_0.Position.Should().Be(1);

				updatedSpaceNode1_2_0 = FindNodeById(spaceNode1_2_0.Id, nodeTree);
				updatedSpaceNode1_2_0.Position.Should().Be(3);

				updatedSpaceNode1_3_0 = FindNodeById(spaceNode1_3_0.Id, nodeTree);
				updatedSpaceNode1_3_0.Position.Should().Be(2);


				//test move down in same parent node
				spaceNode1_3_0.Position = 3;
				nodeTree = spaceManager.UpdateSpaceNode(spaceNode1_3_0);

				updatedSpaceNode1_1_0 = FindNodeById(spaceNode1_1_0.Id, nodeTree);
				updatedSpaceNode1_1_0.Position.Should().Be(1);

				updatedSpaceNode1_2_0 = FindNodeById(spaceNode1_2_0.Id, nodeTree);
				updatedSpaceNode1_2_0.Position.Should().Be(2);

				updatedSpaceNode1_3_0 = FindNodeById(spaceNode1_3_0.Id, nodeTree);
				updatedSpaceNode1_3_0.Position.Should().Be(3);
				#endregion

				#region root node - move up/down 

				//test move up in same parent node while it is on first position
				spaceNode1_0_0.Position = 0;
				nodeTree = spaceManager.UpdateSpaceNode(spaceNode1_0_0);

				var updatedSpaceNode1_0_0 = FindNodeById(spaceNode1_0_0.Id, nodeTree);
				updatedSpaceNode1_0_0.Position.Should().Be(1);

				//test move up in same parent node on position greater than max allowed
				spaceNode1_0_0.Position = 10;
				nodeTree = spaceManager.UpdateSpaceNode(spaceNode1_0_0);

				updatedSpaceNode1_0_0 = FindNodeById(spaceNode1_0_0.Id, nodeTree);
				updatedSpaceNode1_0_0.Position.Should().Be(3);

				//return node to position 1
				spaceNode1_0_0.Position = 1;
				nodeTree = spaceManager.UpdateSpaceNode(spaceNode1_0_0);

				updatedSpaceNode1_0_0 = FindNodeById(spaceNode1_0_0.Id, nodeTree);
				updatedSpaceNode1_0_0.Position.Should().Be(1);

				//test move up in same parent node
				spaceNode1_0_0.Position = 3;
				nodeTree = spaceManager.UpdateSpaceNode(spaceNode1_0_0);

				updatedSpaceNode1_0_0 = FindNodeById(spaceNode1_0_0.Id, nodeTree);
				updatedSpaceNode1_0_0.Position.Should().Be(3);

				var updatedSpaceNode2_0_0 = FindNodeById(spaceNode2_0_0.Id, nodeTree);
				updatedSpaceNode2_0_0.Position.Should().Be(1);

				var updatedSpaceNode3_0_0 = FindNodeById(spaceNode3_0_0.Id, nodeTree);
				updatedSpaceNode3_0_0.Position.Should().Be(2);

				//test move down in same parent node
				spaceNode1_0_0.Position = 1;
				nodeTree = spaceManager.UpdateSpaceNode(spaceNode1_0_0);

				updatedSpaceNode1_0_0 = FindNodeById(spaceNode1_0_0.Id, nodeTree);
				updatedSpaceNode1_0_0.Position.Should().Be(1);

				updatedSpaceNode2_0_0 = FindNodeById(spaceNode2_0_0.Id, nodeTree);
				updatedSpaceNode2_0_0.Position.Should().Be(2);

				updatedSpaceNode3_0_0 = FindNodeById(spaceNode3_0_0.Id, nodeTree);
				updatedSpaceNode3_0_0.Position.Should().Be(3);

				//change position without changing parent (root) with invalid position
				spaceNode1_0_0.Position = null;
				nodeTree = spaceManager.UpdateSpaceNode(spaceNode1_0_0);
				
				updatedSpaceNode1_0_0 = FindNodeById(spaceNode1_0_0.Id, nodeTree);
				updatedSpaceNode1_0_0.Position.Should().Be(3);
				updatedSpaceNode1_0_0.ParentId.Should().Be(null);

				updatedSpaceNode2_0_0 = FindNodeById(spaceNode2_0_0.Id, nodeTree);
				updatedSpaceNode2_0_0.Position.Should().Be(1);

				updatedSpaceNode3_0_0 = FindNodeById(spaceNode3_0_0.Id, nodeTree);
				updatedSpaceNode3_0_0.Position.Should().Be(2);


				#endregion

				#region change root

				//test change parent node and position
				spaceNode1_0_0.Position = 2;
				spaceNode1_0_0.ParentId = spaceNode3_0_0.Id;
				nodeTree = spaceManager.UpdateSpaceNode(spaceNode1_0_0);

				updatedSpaceNode1_0_0 = FindNodeById(spaceNode1_0_0.Id, nodeTree);
				updatedSpaceNode1_0_0.Position.Should().Be(2);
				updatedSpaceNode1_0_0.ParentId.Should().Be(spaceNode3_0_0.Id);

				updatedSpaceNode2_0_0 = FindNodeById(spaceNode2_0_0.Id, nodeTree);
				updatedSpaceNode2_0_0.Position.Should().Be(1);

				updatedSpaceNode3_0_0 = FindNodeById(spaceNode3_0_0.Id, nodeTree);
				updatedSpaceNode3_0_0.Position.Should().Be(2);

				var updatedSpaceNode3_2_0 = FindNodeById(spaceNode3_2_0.Id, nodeTree);
				updatedSpaceNode3_2_0.Position.Should().Be(3);

				var updatedSpaceNode3_3_0 = FindNodeById(spaceNode3_3_0.Id, nodeTree);
				updatedSpaceNode3_3_0.Position.Should().Be(4);

				spaceNode1_0_0.Position = 1;
				spaceNode1_0_0.ParentId = null;
				nodeTree = spaceManager.UpdateSpaceNode(spaceNode1_0_0);

				updatedSpaceNode1_0_0 = FindNodeById(spaceNode1_0_0.Id, nodeTree);
				updatedSpaceNode1_0_0.Position.Should().Be(1);
				updatedSpaceNode1_0_0.ParentId.Should().Be(null);

				updatedSpaceNode2_0_0 = FindNodeById(spaceNode2_0_0.Id, nodeTree);
				updatedSpaceNode2_0_0.Position.Should().Be(2);

				updatedSpaceNode3_0_0 = FindNodeById(spaceNode3_0_0.Id, nodeTree);
				updatedSpaceNode3_0_0.Position.Should().Be(3);

				updatedSpaceNode3_2_0 = FindNodeById(spaceNode3_2_0.Id, nodeTree);
				updatedSpaceNode3_2_0.Position.Should().Be(2);

				updatedSpaceNode3_3_0 = FindNodeById(spaceNode3_3_0.Id, nodeTree);
				updatedSpaceNode3_3_0.Position.Should().Be(3);

				//change position with null + parent change
				spaceNode1_0_0.Position = null;
				spaceNode1_0_0.ParentId = spaceNode2_0_0.Id;
				
				nodeTree = spaceManager.UpdateSpaceNode(spaceNode1_0_0);

				updatedSpaceNode1_0_0 = FindNodeById(spaceNode1_0_0.Id, nodeTree);
				updatedSpaceNode1_0_0.Position.Should().Be(4); //last position
				updatedSpaceNode1_0_0.ParentId.Should().Be(spaceNode2_0_0.Id);

				updatedSpaceNode2_0_0 = FindNodeById(spaceNode2_0_0.Id, nodeTree);
				updatedSpaceNode2_0_0.Position.Should().Be(1);

				updatedSpaceNode3_0_0 = FindNodeById(spaceNode3_0_0.Id, nodeTree);
				updatedSpaceNode3_0_0.Position.Should().Be(2);

				//return to initial state
				spaceNode1_0_0.Position = 1;
				spaceNode1_0_0.ParentId = null;
				nodeTree = spaceManager.UpdateSpaceNode(spaceNode1_0_0);
				updatedSpaceNode1_0_0 = FindNodeById(spaceNode1_0_0.Id, nodeTree);
				updatedSpaceNode1_0_0.Position.Should().Be(1);
				updatedSpaceNode1_0_0.ParentId.Should().BeNull();

				//try to move node inside child nodes tree
				updatedSpaceNode1_0_0.Position = 1;
				updatedSpaceNode1_0_0.ParentId = spaceNode1_2_0.Id;
				var task = Task.Run(() => { nodeTree = spaceManager.UpdateSpaceNode(updatedSpaceNode1_0_0);  });
				var exception = Record.ExceptionAsync(async () => await task).Result;
				exception.Should().NotBeNull();

				#endregion

				#region copy node

				spaceNode1_0_0 = FindNodeById(spaceNode1_0_0.Id, nodeTree);

				var (copyNodeId, newNodeTree) = spaceManager.CopySpaceNode(spaceNode1_0_0.Id);

				var copiedNode = FindNodeById(copyNodeId, newNodeTree);
				short newPosition = (short)(spaceNode1_0_0.Position.Value + 1);
				copiedNode.Position.Value.Should().Be(newPosition);
				copiedNode.ChildNodes.Count.Should().Be(spaceNode1_0_0.ChildNodes.Count);

				spaceManager.DeleteSpaceNode(copiedNode);

				#endregion

				#region delete node

				spaceManager.DeleteSpaceNode(spaceNode1_0_0);
				spaceManager.DeleteSpaceNode(spaceNode2_0_0);
				spaceManager.DeleteSpaceNode(spaceNode3_3_0);
				spaceManager.DeleteSpaceNode(spaceNode3_0_0);

				#endregion
			}
		}
	}

	private TfSpaceNode FindNodeById(
		Guid id,
		List<TfSpaceNode> nodes)
	{
		if (nodes == null || nodes.Count == 0)
			return null;

		Queue<TfSpaceNode> queue = new Queue<TfSpaceNode>();

		foreach (var node in nodes)
		{
			if (node.Id == id)
				return node;

			queue.Enqueue(node);
		}

		while (queue.Count > 0)
		{
			var node = queue.Dequeue();
			if (node.Id == id)
				return node;

			foreach (var childNode in node.ChildNodes)
				queue.Enqueue(childNode);
		}

		return null;
	}

}
