namespace WebVella.Tefter.Tests;

using WebVella.Tefter.Assets.Models;
using WebVella.Tefter.Assets.Services;
using WebVella.Tefter.Utility;

public partial class AssetsTests : BaseTest
{
	[Fact]
	public async Task Assests_Folder_CRUD()
	{
		using (await locker.LockAsync())
		{
			ITfDatabaseService dbService = ServiceProvider.GetRequiredService<ITfDatabaseService>();
			IAssetsService assetService = ServiceProvider.GetRequiredService<IAssetsService>();
			ITfService tfService = ServiceProvider.GetService<ITfService>();

			using (var scope = dbService.CreateTransactionScope(TfConstants.DB_OPERATION_LOCK_KEY))
			{
				var user = tfService.GetUser("rumen@webvella.com");

				AssetsFolder folder = new AssetsFolder
				{
					Id = Guid.NewGuid(),
					Name = "Test Folder",
					DataIdentity = TfConstants.TF_ROW_ID_DATA_IDENTITY,
					CountSharedColumnName = ""
				};

				var folderResult = assetService.CreateFolder(folder);
				folderResult.Should().NotBeNull();
								
				folder.Name = "Test folder 1";
				folderResult = assetService.UpdateFolder(folder);
				folder.Name.Should().Be("Test folder 1");

				assetService.DeleteFolder(folder.Id);
			}
		}
	}

	[Fact]
	public async Task Assets_Asset_CRUD()
	{
		using (await locker.LockAsync())
		{
			ITfDatabaseService dbService = ServiceProvider.GetRequiredService<ITfDatabaseService>();
			IAssetsService assetService = ServiceProvider.GetRequiredService<IAssetsService>();
			ITfService tfService = ServiceProvider.GetService<ITfService>();

			using (var scope = dbService.CreateTransactionScope(TfConstants.DB_OPERATION_LOCK_KEY))
			{
				var (provider, spaceData) = await CreateTestStructureAndData(ServiceProvider, dbService);
				var dataTable = tfService.QueryDataProvider(provider);

				List<string> rowIdentityIds = new List<string>();
				for (int i = 0; i < 5; i++)
					rowIdentityIds.Add((string)dataTable.Rows[i]["tf_row_id"]);

				var user = tfService.GetDefaultSystemUser();
				if (user == null) throw new Exception("No default system user found");

				AssetsFolder folder = new AssetsFolder
				{
					Id = Guid.NewGuid(),
					Name = "Test Folder",
					DataIdentity = TfConstants.TF_ROW_ID_DATA_IDENTITY,
					CountSharedColumnName = ""
				};
				folder = assetService.CreateFolder(folder);

				CreateLinkAssetWithDataIdentityModel asset = new CreateLinkAssetWithDataIdentityModel
				{
					FolderId = folder.Id,
					Label = "Test link",
					Url = "http://google.com",
					CreatedBy = user.Id,
					DataIdentityValues = rowIdentityIds,
				};

				var createdAsset = assetService.CreateLinkAsset(asset);
				createdAsset.Should().NotBeNull();

				createdAsset = assetService.GetAsset(createdAsset.Id);
				createdAsset.Should().NotBeNull();

				var assets = assetService.GetAssets(folder.Id, null);
				assets.Count.Should().Be(1);

				var dataIdentityValueToSearchOn = assets[0].ConnectedDataIdentityValues.Keys.First();

				assets = assetService.GetAssets(folder.Id, dataIdentityValueToSearchOn);
				assets.Count.Should().Be(1);

				var randomDataIdentityValue = Guid.NewGuid().ToSha1();

				assets = assetService.GetAssets(folder.Id, randomDataIdentityValue);
				assets.Count.Should().Be(0);

				List<string> rowIdentityIds2 = new List<string>();
				for (int i = 5; i < 10; i++)
					rowIdentityIds2.Add((string)dataTable.Rows[i]["tf_row_id"]);

				//using tmp file because local file is moved
				var appSettingsFilePath = ToApplicationPath("appsettings.json");
				var tmpFilePath = Path.GetTempPath() + "appsettings.json";
				File.Copy(appSettingsFilePath, tmpFilePath, true);

				//create file asset
				CreateFileAssetWithDataIdentityModel fileAssetModel = new CreateFileAssetWithDataIdentityModel
				{
					FolderId = folder.Id,
					CreatedBy = user.Id,
					Label = "Test file",
					DataIdentityValues = rowIdentityIds2,
					LocalPath = tmpFilePath
				};

				var fileAsset = assetService.CreateFileAsset(fileAssetModel);
				fileAsset.Should().NotBeNull();

				dataIdentityValueToSearchOn = fileAsset.ConnectedDataIdentityValues.Keys.First();

				assets = assetService.GetAssets(folder.Id, dataIdentityValueToSearchOn);
				assets.Count.Should().Be(1);

				List<string> rowIdentityIds3 = new List<string>();
				rowIdentityIds3.Add((string)dataTable.Rows[0]["tf_row_id"]);
				rowIdentityIds3.Add((string)dataTable.Rows[5]["tf_row_id"]);

				CreateLinkAssetWithDataIdentityModel dupIdentityValueAsset = new CreateLinkAssetWithDataIdentityModel
				{
					FolderId = folder.Id,
					Label = "Test link",
					Url = "http://google.com",
					CreatedBy = user.Id,
					DataIdentityValues = rowIdentityIds3,
				};

				createdAsset = assetService.CreateLinkAsset(dupIdentityValueAsset);
				createdAsset.Should().NotBeNull();
				createdAsset.ConnectedDataIdentityValues.Count.Should().Be(2);

				assets = assetService.GetAssets(folder.Id, rowIdentityIds3[0]);
				assets.Count.Should().Be(2);

				assets = assetService.GetAssets(folder.Id, rowIdentityIds3[1]);
				assets.Count.Should().Be(2);

			}
		}
	}

	public static string ToApplicationPath(string fileName)
	{
		var exePath = Path.GetDirectoryName(System.Reflection.Assembly.GetExecutingAssembly().Location);
		Regex appPathMatcher = new Regex(@"(?<!fil)[A-Za-z]:\\+[\S\s]*?(?=\\+bin)");
		var appRoot = appPathMatcher.Match(exePath).Value;
		return Path.Combine(appRoot, fileName);
	}
}
