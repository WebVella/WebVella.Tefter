using Bogus;
using System;
using WebVella.Tefter.Exceptions;
using WebVella.Tefter.Models;
using WebVella.Tefter.Web.Addons;

namespace WebVella.Tefter.Tests.Services;

public partial class TfServiceTest : BaseTest
{
	[Fact]
	public async Task Space_CRUD()
	{
		using (await locker.LockAsync())
		{
			ITfDatabaseService dbService = ServiceProvider.GetRequiredService<ITfDatabaseService>();
			ITfService tfService = ServiceProvider.GetService<ITfService>();

			using (var scope = dbService.CreateTransactionScope(TfConstants.DB_OPERATION_LOCK_KEY))
			{
				var role = tfService
					.CreateRoleBuilder()
					.WithName("UnitTester")
					.Build();

				role = await tfService.SaveRoleAsync(role);
				role.Should().NotBeNull();

				var space1 = new TfSpace
				{
					Id = Guid.NewGuid(),
					Name = "Space1",
					Color = 10,
					FluentIconName = "icon1",
					IsPrivate = false,
					Position = 0,
					Roles = new List<TfRole> { role }
				};
				var result = tfService.CreateSpace(space1);
				result.Should().NotBeNull();
				result.Id.Should().Be(space1.Id);
				result.Name.Should().Be(space1.Name);
				result.Position.Should().Be(1);
				result.IsPrivate.Should().Be(space1.IsPrivate);
				result.FluentIconName.Should().Be(space1.FluentIconName);
				result.Color.Should().Be(space1.Color);
				result.Roles.Count.Should().Be(1);

				var space2 = new TfSpace
				{
					Id = Guid.NewGuid(),
					Name = "Space2",
					Color = 12,
					FluentIconName = "icon2",
					IsPrivate = false,
					Position = 0,
					Roles = new List<TfRole> { role }
				};
				result = tfService.CreateSpace(space2);
				result.Should().NotBeNull();
				result.Id.Should().Be(space2.Id);
				result.Name.Should().Be(space2.Name);
				result.Position.Should().Be(2);
				result.IsPrivate.Should().Be(space2.IsPrivate);
				result.FluentIconName.Should().Be(space2.FluentIconName);
				result.Color.Should().Be(space2.Color);
				result.Roles.Count.Should().Be(1);

				tfService.MoveSpaceDown(space1.Id);
				space1 = tfService.GetSpace(space1.Id);
				space2 = tfService.GetSpace(space2.Id);
				space1.Position.Should().Be(2);
				space2.Position.Should().Be(1);

				tfService.MoveSpaceUp(space1.Id);
				space1 = tfService.GetSpace(space1.Id);
				space2 = tfService.GetSpace(space2.Id);
				space1.Position.Should().Be(1);
				space2.Position.Should().Be(2);

				space1.Name = "updated name";
				space1.FluentIconName = "updated icon";
				space1.Color = 100;
				space1.IsPrivate = true;

				result = tfService.UpdateSpace(space1);
				result.Should().NotBeNull();
				result.Id.Should().Be(space1.Id);
				result.Name.Should().Be(space1.Name);
				result.Position.Should().Be(1);
				result.IsPrivate.Should().Be(space1.IsPrivate);
				result.FluentIconName.Should().Be(space1.FluentIconName);
				result.Color.Should().Be(space1.Color);

				tfService.DeleteSpace(space1.Id);
				space2 = tfService.GetSpace(space2.Id);
				space2.Position.Should().Be(1);
			}
		}
	}

	[Fact]
	public async Task Space_ListPerUser()
	{
		using (await locker.LockAsync())
		{
			ITfDatabaseService dbService = ServiceProvider.GetRequiredService<ITfDatabaseService>();
			ITfService tfService = ServiceProvider.GetService<ITfService>();

			using (var scope = dbService.CreateTransactionScope(TfConstants.DB_OPERATION_LOCK_KEY))
			{
				var role = tfService
					.CreateRoleBuilder()
					.WithName("UnitTester")
					.Build();

				role = await tfService.SaveRoleAsync(role);
				role.Should().NotBeNull();


				var user = tfService
					.CreateUserBuilder()
					.WithEmail("test@test.com")
					.WithPassword("password")
					.WithFirstName("firstname")
					.WithLastName("lastname")
					.CreatedOn(DateTime.Now)
					.Enabled(true)
					.WithRoles(role)
					.Build();

				user = await tfService.SaveUserAsync(user);
				user.Should().NotBeNull();

				var space1 = new TfSpace
				{
					Id = Guid.NewGuid(),
					Name = "Space1",
					Color = 10,
					FluentIconName = "icon1",
					IsPrivate = false,
					Position = 0,
					Roles = new List<TfRole> { role }
				};
				var result = tfService.CreateSpace(space1);
				result.Should().NotBeNull();
				result.Id.Should().Be(space1.Id);
				result.Name.Should().Be(space1.Name);
				result.Position.Should().Be(1);
				result.IsPrivate.Should().Be(space1.IsPrivate);
				result.FluentIconName.Should().Be(space1.FluentIconName);
				result.Color.Should().Be(space1.Color);
				result.Roles.Count.Should().Be(1);

				var space2 = new TfSpace
				{
					Id = Guid.NewGuid(),
					Name = "Space2",
					Color = 10,
					FluentIconName = "icon1",
					IsPrivate = false,
					Position = 0,
					Roles = new List<TfRole> { }
				};
				result = tfService.CreateSpace(space2);
				result.Should().NotBeNull();
				result.Id.Should().Be(space2.Id);
				result.Name.Should().Be(space2.Name);
				result.Position.Should().Be(2);
				result.IsPrivate.Should().Be(space2.IsPrivate);
				result.FluentIconName.Should().Be(space2.FluentIconName);
				result.Color.Should().Be(space2.Color);
				result.Roles.Count.Should().Be(0);

				var userSpaces = tfService.GetSpacesListForUser(user.Id);
				userSpaces.Should().NotBeNull();
				userSpaces.Count.Should().Be(1);
				userSpaces[0].Id.Should().Be(space1.Id);

				tfService.RemoveUsersRoleAsync(new List<TfUser> { user }, role).Wait();

				userSpaces = tfService.GetSpacesListForUser(user.Id);
				userSpaces.Should().NotBeNull();
				userSpaces.Count.Should().Be(0);

				var adminRole = tfService.GetRole(TfConstants.ADMIN_ROLE_ID);
				tfService.AddUsersRole(new List<TfUser> { user }, adminRole);

				userSpaces = tfService.GetSpacesListForUser(user.Id);
				userSpaces.Should().NotBeNull();
				userSpaces.Count.Should().Be(2);
			}
		}
	}

	[Fact]
	public async Task Space_AddAndRemoveToRole()
	{
		using (await locker.LockAsync())
		{
			ITfDatabaseService dbService = ServiceProvider.GetRequiredService<ITfDatabaseService>();
			ITfService tfService = ServiceProvider.GetService<ITfService>();

			using (var scope = dbService.CreateTransactionScope(TfConstants.DB_OPERATION_LOCK_KEY))
			{
				var role = tfService
					.CreateRoleBuilder()
					.WithName("UnitTester")
					.Build();

				role = await tfService.SaveRoleAsync(role);
				role.Should().NotBeNull();

				var space1 = new TfSpace
				{
					Id = Guid.NewGuid(),
					Name = "Space1",
					Color = 10,
					FluentIconName = "icon1",
					IsPrivate = false,
					Position = 0,
					Roles = new List<TfRole> { }
				};
				var result = tfService.CreateSpace(space1);
				result.Should().NotBeNull();
				result.Id.Should().Be(space1.Id);
				result.Name.Should().Be(space1.Name);
				result.Position.Should().Be(1);
				result.IsPrivate.Should().Be(space1.IsPrivate);
				result.FluentIconName.Should().Be(space1.FluentIconName);
				result.Color.Should().Be(space1.Color);
				result.Roles.Count.Should().Be(0);

				var space2 = new TfSpace
				{
					Id = Guid.NewGuid(),
					Name = "Space2",
					Color = 10,
					FluentIconName = "icon1",
					IsPrivate = false,
					Position = 0,
					Roles = new List<TfRole> { }
				};
				result = tfService.CreateSpace(space2);
				result.Should().NotBeNull();
				result.Id.Should().Be(space2.Id);
				result.Name.Should().Be(space2.Name);
				result.Position.Should().Be(2);
				result.IsPrivate.Should().Be(space2.IsPrivate);
				result.FluentIconName.Should().Be(space2.FluentIconName);
				result.Color.Should().Be(space2.Color);
				result.Roles.Count.Should().Be(0);


				tfService.AddSpacesRole(new List<TfSpace> { space1, space2 }, role);

				space1 = tfService.GetSpace(space1.Id);
				space1.Roles.Count().Should().Be(1);

				space2 = tfService.GetSpace(space2.Id);
				space2.Roles.Count().Should().Be(1);

				tfService.RemoveSpacesRole(new List<TfSpace> { space1, space2 }, role);

				space1 = tfService.GetSpace(space1.Id);
				space1.Roles.Count().Should().Be(0);

				space2 = tfService.GetSpace(space2.Id);
				space2.Roles.Count().Should().Be(0);
			}
		}
	}


	[Fact]
	public async Task SpaceData_CRUD()
	{
		using (await locker.LockAsync())
		{
			ITfDatabaseService dbService = ServiceProvider.GetRequiredService<ITfDatabaseService>();
			ITfService tfService = ServiceProvider.GetService<ITfService>();
			ITfMetaService tfMetaService = ServiceProvider.GetService<ITfMetaService>();

			using (var scope = dbService.CreateTransactionScope(TfConstants.DB_OPERATION_LOCK_KEY))
			{
				var providerTypes = tfMetaService.GetDataProviderTypes();
				var providerType = providerTypes
					.Single(x => x.Id == new Guid("90b7de99-4f7f-4a31-bcf9-9be988739d2d"));

				TfDataProviderModel providerModel = new TfDataProviderModel
				{
					Id = Guid.NewGuid(),
					Name = "test data provider",
					ProviderType = providerType,
					SettingsJson = null
				};

				var provider = tfService.CreateDataProvider(providerModel);
				provider.Should().BeOfType<TfDataProvider>();

				var space = new TfSpace
				{
					Id = Guid.NewGuid(),
					Name = "Space1",
					Color = 10,
					FluentIconName = "icon1",
					IsPrivate = false,
					Position = 0
				};
				tfService.CreateSpace(space);


				var spaceData1 = new TfSpaceData
				{
					Id = Guid.NewGuid(),
					DataProviderId = providerModel.Id,
					Name = "data1",
					SpaceId = space.Id,
				};

				var result = tfService.CreateSpaceData(spaceData1);
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

				result = tfService.CreateSpaceData(spaceData2);
				result.Should().NotBeNull();
				result.Id.Should().Be(spaceData2.Id);
				result.Name.Should().Be(spaceData2.Name);
				result.SpaceId.Should().Be(space.Id);
				result.Position.Should().Be(2);
				result.Filters.Should().NotBeNull();
				result.Filters.Count().Should().Be(0);

				tfService.MoveSpaceDataDown(spaceData1.Id);
				spaceData1 = tfService.GetSpaceData(spaceData1.Id);
				spaceData2 = tfService.GetSpaceData(spaceData2.Id);
				spaceData1.Position.Should().Be(2);
				spaceData2.Position.Should().Be(1);

				tfService.MoveSpaceDataUp(spaceData1.Id);
				spaceData1 = tfService.GetSpaceData(spaceData1.Id);
				spaceData2 = tfService.GetSpaceData(spaceData2.Id);
				spaceData1.Position.Should().Be(1);
				spaceData2.Position.Should().Be(2);

				spaceData1.Name = "updated name";
				result = tfService.UpdateSpaceData(spaceData1);
				result.Name.Should().Be(spaceData1.Name);

				tfService.DeleteSpaceData(spaceData1.Id);
				spaceData2 = tfService.GetSpaceData(spaceData2.Id);
				spaceData2.Position.Should().Be(1);

				tfService.DeleteSpace(space.Id);
			}
		}
	}

	[Fact]
	public async Task SpaceData_TryToChangeSpaceId()
	{
		using (await locker.LockAsync())
		{
			ITfDatabaseService dbService = ServiceProvider.GetRequiredService<ITfDatabaseService>();
			ITfService tfService = ServiceProvider.GetService<ITfService>();
			ITfMetaService tfMetaService = ServiceProvider.GetService<ITfMetaService>();

			using (var scope = dbService.CreateTransactionScope(TfConstants.DB_OPERATION_LOCK_KEY))
			{
				var providerTypes = tfMetaService.GetDataProviderTypes();
				var providerType = providerTypes
					.Single(x => x.Id == new Guid("90b7de99-4f7f-4a31-bcf9-9be988739d2d"));

				TfDataProviderModel providerModel = new TfDataProviderModel
				{
					Id = Guid.NewGuid(),
					Name = "test data provider",
					ProviderType = providerType,
					SettingsJson = null
				};
				var provider = tfService.CreateDataProvider(providerModel);
				provider.Should().BeOfType<TfDataProvider>();


				var space = new TfSpace
				{
					Id = Guid.NewGuid(),
					Name = "Space1",
					Color = 10,
					FluentIconName = "icon1",
					IsPrivate = false,
					Position = 0
				};
				tfService.CreateSpace(space);

				var space2 = new TfSpace
				{
					Id = Guid.NewGuid(),
					Name = "Space2",
					Color = 10,
					FluentIconName = "icon1",
					IsPrivate = false,
					Position = 0
				};
				tfService.CreateSpace(space2);

				var spaceData1 = new TfSpaceData
				{
					Id = Guid.NewGuid(),
					DataProviderId = providerModel.Id,
					Name = "data1",
					SpaceId = space.Id,
				};

				var result = tfService.CreateSpaceData(spaceData1);
				result.Should().NotBeNull();
				result.Id.Should().Be(spaceData1.Id);
				result.Name.Should().Be(spaceData1.Name);
				result.SpaceId.Should().Be(space.Id);
				result.Position.Should().Be(1);
				result.Filters.Should().NotBeNull();
				result.Filters.Count().Should().Be(0);


				spaceData1.SpaceId = space2.Id;
				var task = Task.Run(() => { result = tfService.UpdateSpaceData(spaceData1); });
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
			ITfService tfService = ServiceProvider.GetService<ITfService>();
			ITfMetaService tfMetaService = ServiceProvider.GetService<ITfMetaService>();

			using (var scope = dbService.CreateTransactionScope(TfConstants.DB_OPERATION_LOCK_KEY))
			{
				var providerTypes = tfMetaService.GetDataProviderTypes();
				var providerType = providerTypes
					.Single(x => x.Id == new Guid("90b7de99-4f7f-4a31-bcf9-9be988739d2d"));

				TfDataProviderModel providerModel = new TfDataProviderModel
				{
					Id = Guid.NewGuid(),
					Name = "test data provider",
					ProviderType = providerType,
					SettingsJson = null
				};
				var provider = tfService.CreateDataProvider(providerModel);
				provider.Should().BeOfType<TfDataProvider>();

				TfDataProviderColumn column = new TfDataProviderColumn
				{
					Id = Guid.Empty,
					AutoDefaultValue = true,
					DefaultValue = null,
					DataProviderId = providerModel.Id,
					DbName = $"dp{provider.Index}_textcolona",
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
				tfService.CreateDataProviderColumn(column);

				provider = tfService.GetDataProvider(providerModel.Id);

				TfDataProviderJoinKey joinKey =
					new TfDataProviderJoinKey
					{
						Id = Guid.NewGuid(),
						Description = "testing1",
						DataProviderId = provider.Id,
						DbName = "testing1",
						Columns = new() { provider.Columns[0] }

					};

				tfService.CreateDataProviderJoinKey(joinKey);

				provider = tfService.GetDataProvider(providerModel.Id);
				provider.JoinKeys.Count().Should().Be(1);


				var space = new TfSpace
				{
					Id = Guid.NewGuid(),
					Name = "Space1",
					Color = 10,
					FluentIconName = "icon1",
					IsPrivate = false,
					Position = 0
				};
				tfService.CreateSpace(space);

				var spaceData = new TfSpaceData
				{
					Id = Guid.NewGuid(),
					DataProviderId = providerModel.Id,
					Name = "data1",
					SpaceId = space.Id,
				};

				//TODO rumen to fix after change

				//var result = tfService.CreateSpaceData(spaceData);
				//result.IsSuccess.Should().BeTrue();
				//result.Value.Should().NotBeNull();
				//result.Value.Columns.Count().Should().Be(2);

				//result.Value.Columns[0].Selected = true;
				//result.Value.Columns[1].Selected = true;
				//tfService.UpdateSpaceData(spaceData);

				//spaceData = tfService.GetSpaceData(spaceData.Id).Value;
				//result.Value.Columns.Count().Should().Be(2);
				//result.Value.Columns[0].Selected.Should().BeTrue();
				//result.Value.Columns[1].Selected.Should().BeTrue();

				//result.Value.Columns[1].Selected = false;
				//tfService.UpdateSpaceData(spaceData);

				//spaceData = tfService.GetSpaceData(spaceData.Id).Value;
				//result.Value.Columns.Count().Should().Be(2);
				//result.Value.Columns[0].Selected.Should().BeTrue();
				//result.Value.Columns[1].Selected.Should().BeFalse();
			}
		}
	}

	[Fact]
	public async Task Bookmark_CRUD()
	{
		using (await locker.LockAsync())
		{
			ITfDatabaseService dbService = ServiceProvider.GetRequiredService<ITfDatabaseService>();
			ITfService tfService = ServiceProvider.GetService<ITfService>();
			ITfMetaService tfMetaService = ServiceProvider.GetService<ITfMetaService>();

			using (var scope = dbService.CreateTransactionScope(TfConstants.DB_OPERATION_LOCK_KEY))
			{


				var role = tfService
						.CreateRoleBuilder()
						.WithName("UnitTester")
						.Build();

				role = await tfService.SaveRoleAsync(role);
				role.Should().NotBeNull();

				var user = tfService
					.CreateUserBuilder()
					.WithEmail("test@test.com")
					.WithPassword("password")
					.WithFirstName("firstname")
					.WithLastName("lastname")
					.CreatedOn(DateTime.Now)
					.Enabled(true)
					.WithRoles(role)
					.Build();

				user = await tfService.SaveUserAsync(user);
				user.Should().NotBeNull();

				user = await tfService.GetUserAsync("test@test.com", "password");
				user.Should().NotBeNull();

				var providerTypes = tfMetaService.GetDataProviderTypes();
				var providerType = providerTypes
					.Single(x => x.Id == new Guid("90b7de99-4f7f-4a31-bcf9-9be988739d2d"));

				TfDataProviderModel providerModel = new TfDataProviderModel
				{
					Id = Guid.NewGuid(),
					Name = "test data provider",
					ProviderType = providerType,
					SettingsJson = null
				};
				var provider = tfService.CreateDataProvider(providerModel);
				provider.Should().BeOfType<TfDataProvider>();

				var space = new TfSpace
				{
					Id = Guid.NewGuid(),
					Name = "Space1",
					Color = 10,
					FluentIconName = "icon1",
					IsPrivate = false,
					Position = 0
				};

				space = tfService.CreateSpace(space);
				space.Should().NotBeNull();

				var spaceData = new TfSpaceData
				{
					Id = Guid.NewGuid(),
					DataProviderId = providerModel.Id,
					Name = "data1",
					SpaceId = space.Id,
				};

				spaceData = tfService.CreateSpaceData(spaceData);
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

				var spaceView = tfService.CreateSpaceView(view);
				spaceView.Should().NotBeNull();
				spaceView.Id.Should().Be(view.Id);
				spaceView.Name.Should().Be(view.Name);
				spaceView.Position.Should().Be(view.Position);
				spaceView.SpaceDataId.Should().Be(view.SpaceDataId);
				spaceView.SpaceId.Should().Be(view.SpaceId);
				spaceView.Type.Should().Be(view.Type);


				var bookmarkList = tfService.GetBookmarksListForUser(user.Id);

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

				var bookmark = tfService.CreateBookmark(bookmarkModel);
				bookmark.Description = " test with #tag_1 #tAg_2 #Tag3";
				bookmark = tfService.UpdateBookmark(bookmark);

				tfService.GetBookmarksListForUser(user.Id).Count.Should().Be(1);
				tfService.DeleteBookmark(bookmark.Id);
				tfService.GetBookmarksListForUser(user.Id).Count.Should().Be(0);
			}
		}
	}


	[Fact]
	public async Task SpaceView_CRUD()
	{
		using (await locker.LockAsync())
		{
			ITfDatabaseService dbService = ServiceProvider.GetRequiredService<ITfDatabaseService>();
			ITfService tfService = ServiceProvider.GetService<ITfService>();
			ITfMetaService tfMetaService = ServiceProvider.GetService<ITfMetaService>();

			using (var scope = dbService.CreateTransactionScope(TfConstants.DB_OPERATION_LOCK_KEY))
			{
				var providerTypes = tfMetaService.GetDataProviderTypes();
				var providerType = providerTypes
					.Single(x => x.Id == new Guid("90b7de99-4f7f-4a31-bcf9-9be988739d2d"));

				TfDataProviderModel providerModel = new TfDataProviderModel
				{
					Id = Guid.NewGuid(),
					Name = "test data provider",
					ProviderType = providerType,
					SettingsJson = null
				};
				var provider = tfService.CreateDataProvider(providerModel);
				provider.Should().BeOfType<TfDataProvider>();

				var space = new TfSpace
				{
					Id = Guid.NewGuid(),
					Name = "Space1",
					Color = 10,
					FluentIconName = "icon1",
					IsPrivate = false,
					Position = 0
				};

				space = tfService.CreateSpace(space);
				space.Should().NotBeNull();

				var spaceData = new TfSpaceData
				{
					Id = Guid.NewGuid(),
					DataProviderId = providerModel.Id,
					Name = "data1",
					SpaceId = space.Id,
				};

				spaceData = tfService.CreateSpaceData(spaceData);
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

				var spaceView = tfService.CreateSpaceView(view);
				spaceView.Should().NotBeNull();
				spaceView.Id.Should().Be(view.Id);
				spaceView.Name.Should().Be(view.Name);
				spaceView.Position.Should().Be(view.Position);
				spaceView.SpaceDataId.Should().Be(view.SpaceDataId);
				spaceView.SpaceId.Should().Be(view.SpaceId);
				spaceView.Type.Should().Be(view.Type);


				view.Name = "view1";
				view.Type = TfSpaceViewType.Chart;

				spaceView = tfService.UpdateSpaceView(view);
				spaceView.Should().NotBeNull();
				spaceView.Id.Should().Be(view.Id);
				spaceView.Name.Should().Be(view.Name);
				spaceView.Position.Should().Be(view.Position);
				spaceView.SpaceDataId.Should().Be(view.SpaceDataId);
				spaceView.SpaceId.Should().Be(view.SpaceId);
				spaceView.Type.Should().Be(view.Type);

				tfService.DeleteSpaceView(view.Id);
			}
		}
	}

	[Fact]
	public async Task SpaceViewColumn_CRUD()
	{
		using (await locker.LockAsync())
		{
			ITfDatabaseService dbService = ServiceProvider.GetRequiredService<ITfDatabaseService>();
			ITfService tfService = ServiceProvider.GetService<ITfService>();
			ITfMetaService tfMetaService = ServiceProvider.GetService<ITfMetaService>();

			using (var scope = dbService.CreateTransactionScope(TfConstants.DB_OPERATION_LOCK_KEY))
			{
				var providerTypes = tfMetaService.GetDataProviderTypes();
				var providerType = providerTypes
					.Single(x => x.Id == new Guid("90b7de99-4f7f-4a31-bcf9-9be988739d2d"));

				TfDataProviderModel providerModel = new TfDataProviderModel
				{
					Id = Guid.NewGuid(),
					Name = "test data provider",
					ProviderType = providerType,
					SettingsJson = null
				};
				var provider = tfService.CreateDataProvider(providerModel);
				provider.Should().BeOfType<TfDataProvider>();

				var space = new TfSpace
				{
					Id = Guid.NewGuid(),
					Name = "Space1",
					Color = 10,
					FluentIconName = "icon1",
					IsPrivate = false,
					Position = 0
				};

				space = tfService.CreateSpace(space);
				space.Should().NotBeNull();

				var spaceData = new TfSpaceData
				{
					Id = Guid.NewGuid(),
					DataProviderId = providerModel.Id,
					Name = "data1",
					SpaceId = space.Id,
				};

				spaceData = tfService.CreateSpaceData(spaceData);
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

				var spaceView = tfService.CreateSpaceView(view);
				spaceView.Should().NotBeNull();

				var availableColumnTypes = tfService.GetAvailableSpaceViewColumnTypes();
				List<TfSpaceViewColumn> createdColums = new List<TfSpaceViewColumn>();
				foreach (var availableColumnType in availableColumnTypes)
				{

					TfSpaceViewColumn column = new TfSpaceViewColumn
					{
						Id = Guid.NewGuid(),
						TypeId = availableColumnType.Id,
						QueryName = availableColumnType.Name
											.ToLower()
											.Replace(".", "")
											.Replace(" ", "")
											.Replace("(", "")
											.Replace(")", ""),
						Title = availableColumnType.Name,
						ComponentId = new Guid(TfTextDisplayColumnComponent.ID),
						SpaceViewId = view.Id,
						CustomOptionsJson = "{}",
						DataMapping = new Dictionary<string, string> { { "Value", "test" } }
					};

					var createdSpaceViewColumn = tfService.CreateSpaceViewColumn(column);
					createdSpaceViewColumn.Should().NotBeNull();
					createdColums.Add(createdSpaceViewColumn);
				}

				var columns = tfService.GetSpaceViewColumnsList(view.Id);
				columns.Count.Should().Be(availableColumnTypes.Count);

				var first = createdColums[0];
				var last = createdColums[createdColums.Count - 1];

				first.Position = (short)createdColums.Count;

				var updateResult = tfService.UpdateSpaceViewColumn(first);
				updateResult.Should().NotBeNull();

				columns = tfService.GetSpaceViewColumnsList(view.Id);
				columns.Single(x => x.Id == first.Id).Position.Should().Be((short)createdColums.Count);
				columns.Single(x => x.Id == last.Id).Position.Should().Be((short)(createdColums.Count - 1));

				last = columns.Single(x => x.Id == first.Id);

				for (int i = 1; i < columns.Count; i++)
				{
					tfService.MoveSpaceViewColumnUp(last.Id);

					columns = tfService.GetSpaceViewColumnsList(view.Id);
					var column = columns.Single(x => x.Id == last.Id);
					column.Position.Should().Be((short)(columns.Count - i));
				}

				//test delete the entire space
				tfService.DeleteSpace(space.Id);
			}
		}
	}


	[Fact]
	public async Task SpaceNode_CRUD()
	{
		using (await locker.LockAsync())
		{
			ITfDatabaseService dbService = ServiceProvider.GetRequiredService<ITfDatabaseService>();
			ITfService tfService = ServiceProvider.GetService<ITfService>();

			using (var scope = dbService.CreateTransactionScope(TfConstants.DB_OPERATION_LOCK_KEY))
			{
				#region Create Structure

				var space = new TfSpace
				{
					Id = Guid.NewGuid(),
					Name = "Space1",
					Color = 10,
					FluentIconName = "icon1",
					IsPrivate = false,
					Position = 0
				};
				tfService.CreateSpace(space);


				var spaceNode1_0_0 = new TfSpacePage
				{
					Id = Guid.NewGuid(),
					ComponentOptionsJson = null,
					ComponentId = null,
					FluentIconName = null,
					Name = "1_0_0",
					ParentId = null,
					Position = null,
					SpaceId = space.Id,
					Type = TfSpacePageType.Folder
				};

				tfService.CreateSpacePage(spaceNode1_0_0);

				var spaceNode1_1_0 = new TfSpacePage
				{
					Id = Guid.NewGuid(),
					ComponentOptionsJson = "{}",
					ComponentId = null,
					FluentIconName = "1_1_0",
					Name = "1_1_0",
					ParentId = spaceNode1_0_0.Id,
					Position = null,
					SpaceId = space.Id,
					Type = TfSpacePageType.Folder
				};

				tfService.CreateSpacePage(spaceNode1_1_0);

				var spaceNode1_2_0 = new TfSpacePage
				{
					Id = Guid.NewGuid(),
					ComponentOptionsJson = "{}",
					ComponentId = null,
					FluentIconName = "1_2_0",
					Name = "1_2_0",
					ParentId = spaceNode1_0_0.Id,
					Position = null,
					SpaceId = space.Id,
					Type = TfSpacePageType.Folder
				};

				tfService.CreateSpacePage(spaceNode1_2_0);

				var spaceNode1_3_0 = new TfSpacePage
				{
					Id = Guid.NewGuid(),
					ComponentOptionsJson = "{}",
					ComponentId = null,
					FluentIconName = "1_3_0",
					Name = "1_3_0",
					ParentId = spaceNode1_0_0.Id,
					Position = null,
					SpaceId = space.Id,
					Type = TfSpacePageType.Folder
				};

				tfService.CreateSpacePage(spaceNode1_3_0);

				var spaceNode2_0_0 = new TfSpacePage
				{
					Id = Guid.NewGuid(),
					ComponentOptionsJson = "{}",
					ComponentId = null,
					FluentIconName = "2_0_0",
					Name = "2_0_0",
					ParentId = null,
					Position = null,
					SpaceId = space.Id,
					Type = TfSpacePageType.Folder
				};

				tfService.CreateSpacePage(spaceNode2_0_0);

				var spaceNode2_1_0 = new TfSpacePage
				{
					Id = Guid.NewGuid(),
					ComponentOptionsJson = "{}",
					ComponentId = null,
					FluentIconName = "2_1_0",
					Name = "2_1_0",
					ParentId = spaceNode2_0_0.Id,
					Position = null,
					SpaceId = space.Id,
					Type = TfSpacePageType.Folder
				};

				tfService.CreateSpacePage(spaceNode2_1_0);

				var spaceNode2_2_0 = new TfSpacePage
				{
					Id = Guid.NewGuid(),
					ComponentOptionsJson = "{}",
					ComponentId = null,
					FluentIconName = "2_2_0",
					Name = "2_2_0",
					ParentId = spaceNode2_0_0.Id,
					Position = null,
					SpaceId = space.Id,
					Type = TfSpacePageType.Folder
				};

				tfService.CreateSpacePage(spaceNode2_2_0);

				var spaceNode2_3_0 = new TfSpacePage
				{
					Id = Guid.NewGuid(),
					ComponentOptionsJson = "{}",
					ComponentId = null,
					FluentIconName = "2_3_0",
					Name = "2_3_0",
					ParentId = spaceNode2_0_0.Id,
					Position = null,
					SpaceId = space.Id,
					Type = TfSpacePageType.Folder
				};

				tfService.CreateSpacePage(spaceNode2_3_0);

				var spaceNode3_0_0 = new TfSpacePage
				{
					Id = Guid.NewGuid(),
					ComponentOptionsJson = "{}",
					ComponentId = null,
					FluentIconName = "3_0_0",
					Name = "3_0_0",
					ParentId = null,
					Position = null,
					SpaceId = space.Id,
					Type = TfSpacePageType.Folder
				};

				tfService.CreateSpacePage(spaceNode3_0_0);

				var spaceNode3_1_0 = new TfSpacePage
				{
					Id = Guid.NewGuid(),
					ComponentOptionsJson = "{}",
					ComponentId = null,
					FluentIconName = "3_1_0",
					Name = "3_1_0",
					ParentId = spaceNode3_0_0.Id,
					Position = null,
					SpaceId = space.Id,
					Type = TfSpacePageType.Folder
				};

				tfService.CreateSpacePage(spaceNode3_1_0);

				var spaceNode3_2_0 = new TfSpacePage
				{
					Id = Guid.NewGuid(),
					ComponentOptionsJson = "{}",
					ComponentId = null,
					FluentIconName = "3_2_0",
					Name = "3_2_0",
					ParentId = spaceNode3_0_0.Id,
					Position = null,
					SpaceId = space.Id,
					Type = TfSpacePageType.Folder
				};

				tfService.CreateSpacePage(spaceNode3_2_0);

				var spaceNode3_3_0 = new TfSpacePage
				{
					Id = Guid.NewGuid(),
					ComponentOptionsJson = "{}",
					ComponentId = null,
					FluentIconName = "3_3_0",
					Name = "3_3_0",
					ParentId = spaceNode3_0_0.Id,
					Position = null,
					SpaceId = space.Id,
					Type = TfSpacePageType.Folder
				};

				tfService.CreateSpacePage(spaceNode3_3_0);

				#endregion

				#region move up/down in same parent node

				//test move up in same parent node
				spaceNode1_3_0.Position = 1;
				var nodeTree = tfService.UpdateSpacePage(spaceNode1_3_0);

				var updatedSpaceNode1_1_0 = FindNodeById(spaceNode1_1_0.Id, nodeTree);
				updatedSpaceNode1_1_0.Position.Should().Be(2);

				var updatedSpaceNode1_2_0 = FindNodeById(spaceNode1_2_0.Id, nodeTree);
				updatedSpaceNode1_2_0.Position.Should().Be(3);

				var updatedSpaceNode1_3_0 = FindNodeById(spaceNode1_3_0.Id, nodeTree);
				updatedSpaceNode1_3_0.Position.Should().Be(1);


				//test move down in same parent node
				spaceNode1_3_0.Position = 3;
				nodeTree = tfService.UpdateSpacePage(spaceNode1_3_0);

				updatedSpaceNode1_1_0 = FindNodeById(spaceNode1_1_0.Id, nodeTree);
				updatedSpaceNode1_1_0.Position.Should().Be(1);

				updatedSpaceNode1_2_0 = FindNodeById(spaceNode1_2_0.Id, nodeTree);
				updatedSpaceNode1_2_0.Position.Should().Be(2);

				updatedSpaceNode1_3_0 = FindNodeById(spaceNode1_3_0.Id, nodeTree);
				updatedSpaceNode1_3_0.Position.Should().Be(3);

				//test move up in same parent node
				spaceNode1_3_0.Position = 2;
				nodeTree = tfService.UpdateSpacePage(spaceNode1_3_0);

				updatedSpaceNode1_1_0 = FindNodeById(spaceNode1_1_0.Id, nodeTree);
				updatedSpaceNode1_1_0.Position.Should().Be(1);

				updatedSpaceNode1_2_0 = FindNodeById(spaceNode1_2_0.Id, nodeTree);
				updatedSpaceNode1_2_0.Position.Should().Be(3);

				updatedSpaceNode1_3_0 = FindNodeById(spaceNode1_3_0.Id, nodeTree);
				updatedSpaceNode1_3_0.Position.Should().Be(2);


				//test move down in same parent node
				spaceNode1_3_0.Position = 3;
				nodeTree = tfService.UpdateSpacePage(spaceNode1_3_0);

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
				nodeTree = tfService.UpdateSpacePage(spaceNode1_0_0);

				var updatedSpaceNode1_0_0 = FindNodeById(spaceNode1_0_0.Id, nodeTree);
				updatedSpaceNode1_0_0.Position.Should().Be(1);

				//test move up in same parent node on position greater than max allowed
				spaceNode1_0_0.Position = 10;
				nodeTree = tfService.UpdateSpacePage(spaceNode1_0_0);

				updatedSpaceNode1_0_0 = FindNodeById(spaceNode1_0_0.Id, nodeTree);
				updatedSpaceNode1_0_0.Position.Should().Be(3);

				//return node to position 1
				spaceNode1_0_0.Position = 1;
				nodeTree = tfService.UpdateSpacePage(spaceNode1_0_0);

				updatedSpaceNode1_0_0 = FindNodeById(spaceNode1_0_0.Id, nodeTree);
				updatedSpaceNode1_0_0.Position.Should().Be(1);

				//test move up in same parent node
				spaceNode1_0_0.Position = 3;
				nodeTree = tfService.UpdateSpacePage(spaceNode1_0_0);

				updatedSpaceNode1_0_0 = FindNodeById(spaceNode1_0_0.Id, nodeTree);
				updatedSpaceNode1_0_0.Position.Should().Be(3);

				var updatedSpaceNode2_0_0 = FindNodeById(spaceNode2_0_0.Id, nodeTree);
				updatedSpaceNode2_0_0.Position.Should().Be(1);

				var updatedSpaceNode3_0_0 = FindNodeById(spaceNode3_0_0.Id, nodeTree);
				updatedSpaceNode3_0_0.Position.Should().Be(2);

				//test move down in same parent node
				spaceNode1_0_0.Position = 1;
				nodeTree = tfService.UpdateSpacePage(spaceNode1_0_0);

				updatedSpaceNode1_0_0 = FindNodeById(spaceNode1_0_0.Id, nodeTree);
				updatedSpaceNode1_0_0.Position.Should().Be(1);

				updatedSpaceNode2_0_0 = FindNodeById(spaceNode2_0_0.Id, nodeTree);
				updatedSpaceNode2_0_0.Position.Should().Be(2);

				updatedSpaceNode3_0_0 = FindNodeById(spaceNode3_0_0.Id, nodeTree);
				updatedSpaceNode3_0_0.Position.Should().Be(3);

				//change position without changing parent (root) with invalid position
				spaceNode1_0_0.Position = null;
				nodeTree = tfService.UpdateSpacePage(spaceNode1_0_0);

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
				nodeTree = tfService.UpdateSpacePage(spaceNode1_0_0);

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
				nodeTree = tfService.UpdateSpacePage(spaceNode1_0_0);

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

				nodeTree = tfService.UpdateSpacePage(spaceNode1_0_0);

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
				nodeTree = tfService.UpdateSpacePage(spaceNode1_0_0);
				updatedSpaceNode1_0_0 = FindNodeById(spaceNode1_0_0.Id, nodeTree);
				updatedSpaceNode1_0_0.Position.Should().Be(1);
				updatedSpaceNode1_0_0.ParentId.Should().BeNull();

				//try to move node inside child nodes tree
				updatedSpaceNode1_0_0.Position = 1;
				updatedSpaceNode1_0_0.ParentId = spaceNode1_2_0.Id;
				var task = Task.Run(() => { nodeTree = tfService.UpdateSpacePage(updatedSpaceNode1_0_0); });
				var exception = Record.ExceptionAsync(async () => await task).Result;
				exception.Should().NotBeNull();

				#endregion

				#region copy node

				spaceNode1_0_0 = FindNodeById(spaceNode1_0_0.Id, nodeTree);

				var (copyNodeId, newNodeTree) = tfService.CopySpacePage(spaceNode1_0_0.Id);

				var copiedNode = FindNodeById(copyNodeId, newNodeTree);
				short newPosition = (short)(spaceNode1_0_0.Position.Value + 1);
				copiedNode.Position.Value.Should().Be(newPosition);
				copiedNode.ChildPages.Count.Should().Be(spaceNode1_0_0.ChildPages.Count);

				tfService.DeleteSpacePage(copiedNode);

				#endregion

				#region delete node

				tfService.DeleteSpacePage(spaceNode1_0_0);
				tfService.DeleteSpacePage(spaceNode2_0_0);
				tfService.DeleteSpacePage(spaceNode3_3_0);
				tfService.DeleteSpacePage(spaceNode3_0_0);

				#endregion
			}
		}
	}

	private TfSpacePage FindNodeById(
		Guid id,
		List<TfSpacePage> nodes)
	{
		if (nodes == null || nodes.Count == 0)
			return null;

		Queue<TfSpacePage> queue = new Queue<TfSpacePage>();

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

			foreach (var childNode in node.ChildPages)
				queue.Enqueue(childNode);
		}

		return null;
	}

}
