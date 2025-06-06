﻿//namespace WebVella.Tefter.Tests;

//using WebVella.Tefter.Assets.Models;
//using WebVella.Tefter.Assets.Services;


//public partial class AssetsTests : BaseTest
//{
//	[Fact]
//	public async Task Assests_Folder_CRUD()
//	{
//		using (await locker.LockAsync())
//		{
//			ITfDatabaseService dbService = ServiceProvider.GetRequiredService<ITfDatabaseService>();
//			IAssetsService assetService = ServiceProvider.GetRequiredService<IAssetsService>();
//			IIdentityManager identityManager = ServiceProvider.GetRequiredService<IIdentityManager>();
//			ITfDataManager dataManager = ServiceProvider.GetRequiredService<ITfDataManager>();


//			using (var scope = dbService.CreateTransactionScope(Constants.DB_OPERATION_LOCK_KEY))
//			{
//				var user = identityManager.GetUser("rumen@webvella.com").Value;

//				Folder folder = new Folder
//				{
//					Id = Guid.NewGuid(),
//					Name = "Test Folder",
//					JoinKey = "talk_join_key",
//					CountSharedColumnName = ""
//				};

//				var folderResult = assetService.CreateFolder(folder);
//				folderResult.IsSuccess.Should().BeTrue();

//				folder = folderResult.Value;
//				folder.Should().NotBeNull();

//				folder.Name = "Test folder 1";
//				folderResult = assetService.UpdateFolder(folder);
//				folderResult.IsSuccess.Should().BeTrue();

//				folder.Name.Should().Be("Test folder 1");

//				folderResult = assetService.DeleteFolder(folder.Id);
//				folderResult.IsSuccess.Should().BeTrue();
//			}
//		}
//	}

//	[Fact]
//	public async Task Assets_Asset_CRUD()
//	{
//		using (await locker.LockAsync())
//		{
//			ITfDatabaseService dbService = ServiceProvider.GetRequiredService<ITfDatabaseService>();
//			IAssetsService assetService = ServiceProvider.GetRequiredService<IAssetsService>();
//			IIdentityManager identityManager = ServiceProvider.GetRequiredService<IIdentityManager>();
//			ITfDataManager dataManager = ServiceProvider.GetRequiredService<ITfDataManager>();


//			using (var scope = dbService.CreateTransactionScope(Constants.DB_OPERATION_LOCK_KEY))
//			{
//				var (provider, spaceData) = await SpaceEnvUtility.CreateTestStructureAndData(ServiceProvider, dbService);
//				var dataTable = dataManager.QueryDataProvider(provider).Value;

//				List<Guid> rowIds = new List<Guid>();
//				for (int i = 0; i < 5; i++)
//					rowIds.Add((Guid)dataTable.Rows[i]["tf_id"]);

//				var user = identityManager.GetUser("rumen@webvella.com").Value;

//				Folder folder = new Folder
//				{
//					Id = Guid.NewGuid(),
//					Name = "Test Folder",
//					JoinKey = "join_key_text",
//					CountSharedColumnName = ""
//				};
//				var folderCreatedResult = assetService.CreateFolder(folder);

//				Guid skId = dataManager.GetId("join_key_value", "1").Value;

//				CreateLinkAssetModel asset = new CreateLinkAssetModel
//				{
//					FolderId = folder.Id,
//					Label = "Test link",
//					Url = "http://google.com",
//					CreatedBy = user.Id,
//					RowIds = rowIds.ToList(),
//					DataProviderId = provider.Id,
//				};

//				var id1 = assetService.CreateLinkAsset(asset).Value;

//				var asset1 = assetService.GetAsset(id1).Value;
//				asset1.Should().NotBeNull();

//				var assets = assetService.GetAssets(folder.Id, null).Value;
//				assets.Count.Should().Be(1);

//				var relSKId = assets[0].RelatedSK.Keys.First();

//				assets = assetService.GetAssets(folder.Id, relSKId).Value;
//				assets.Count.Should().Be(1);

//				relSKId = Guid.NewGuid();

//				assets = assetService.GetAssets(folder.Id, relSKId).Value;
//				assets.Count.Should().Be(0);


//				//create file asset
//				CreateFileAssetModel fileAssetModel = new CreateFileAssetModel
//				{
//					FolderId = folder.Id,
//					CreatedBy = user.Id,
//					RowIds = rowIds.ToList(),
//					DataProviderId = provider.Id,
//					LocalPath = ToApplicationPath("appsettings.json")
//				};

//				var fileAssetResult = assetService.CreateFileAsset(fileAssetModel);
//				fileAssetResult.IsSuccess.Should().BeTrue();
//			}
//		}
//	}

//	public static string ToApplicationPath(string fileName)
//	{
//		var exePath = Path.GetDirectoryName(System.Reflection.Assembly.GetExecutingAssembly().Location);
//		Regex appPathMatcher = new Regex(@"(?<!fil)[A-Za-z]:\\+[\S\s]*?(?=\\+bin)");
//		var appRoot = appPathMatcher.Match(exePath).Value;
//		return Path.Combine(appRoot, fileName);
//	}
//}
