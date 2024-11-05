using Bogus;
using WebVella.Tefter.Models;

namespace WebVella.Tefter.Tests;

public partial class TfFileManagerTests : BaseTest
{

	[Fact]
	public async Task FileManager_CRUD()
	{
		using (await locker.LockAsync())
		{
			ITfDatabaseService dbService = ServiceProvider.GetRequiredService<ITfDatabaseService>();
			ITfFileManager fileManager = ServiceProvider.GetRequiredService<ITfFileManager>();

			using (var scope = dbService.CreateTransactionScope(Constants.DB_OPERATION_LOCK_KEY))
			{
				const string sampleJsonFileContent = @"{
	""Tefter"": {
		""ConnectionString"": ""Server=192.168.0.190;Port=5436;User Id=dev;Password=dev;Database=tefter_unittests;Pooling=true;MinPoolSize=1;MaxPoolSize=100;CommandTimeout=120;"",
		""CryptoPassword"": ""Encryption requires a key, which is created and managed by the data protection system"",
		""FilesRootPath"": """"
	}
}";

				byte[] sampleBytes = Encoding.UTF8.GetBytes(sampleJsonFileContent);

				string initialFilePath = "/test/test.json";

				// create
				var createResult = fileManager.CreateFile(initialFilePath, sampleBytes, true);

				createResult.IsSuccess.Should().BeTrue();

				var file = createResult.Value;
				file.Should().NotBeNull();

				//find all
				var findAllfilesResult = fileManager.FindAllFiles();

				findAllfilesResult.IsSuccess.Should().BeTrue();

				findAllfilesResult.Value.Count.Should().Be(1);

				//copy

				string copyFilePath = "/test/test1.json";

				var copyResult = fileManager.CopyFile(initialFilePath, copyFilePath);

				copyResult.IsSuccess.Should().BeTrue();

				file = copyResult.Value;

				file.Should().NotBeNull();

				var deleteResult = fileManager.DeleteFile(copyFilePath);

				deleteResult.IsSuccess.Should().BeTrue();

				//move
				string moveFilePath = "/test1/test.json";

				var moveResult = fileManager.CopyFile(initialFilePath, moveFilePath);

				moveResult.IsSuccess.Should().BeTrue();

				file = copyResult.Value;

				file.Should().NotBeNull();

				deleteResult = fileManager.DeleteFile(moveFilePath);

				deleteResult.IsSuccess.Should().BeTrue();

				deleteResult = fileManager.DeleteFile(initialFilePath);

				deleteResult.IsSuccess.Should().BeTrue();

				findAllfilesResult = fileManager.FindAllFiles();

				findAllfilesResult.IsSuccess.Should().BeTrue();

				findAllfilesResult.Value.Count.Should().Be(0);

				//tmp

				var tmpResult = fileManager.CreateTempFile("test.json", sampleBytes);

				tmpResult.IsSuccess.Should().BeTrue();

				file = tmpResult.Value;

				file.Should().NotBeNull();

				var bytes = fileManager.GetFileContentAsByteArray(file).Value;

				var json = Encoding.UTF8.GetString(bytes);

				(json == sampleJsonFileContent).Should().BeTrue();

				deleteResult = fileManager.DeleteFile(file.FilePath);

				deleteResult.IsSuccess.Should().BeTrue();

				findAllfilesResult = fileManager.FindAllFiles();

				findAllfilesResult.IsSuccess.Should().BeTrue();

				findAllfilesResult.Value.Count.Should().Be(0);


				//cleanup by deleting root folder
				Directory.Delete( fileManager.RootPath, true );
			}
		}
	}
}