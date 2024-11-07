using System.IO;
using System.Text;

namespace WebVella.Tefter.Tests;

public partial class TfRepositoryServiceTests : BaseTest
{

	[Fact]
	public async Task RepositoryService_CRUD()
	{
		using (await locker.LockAsync())
		{
			ITfDatabaseService dbService = ServiceProvider.GetRequiredService<ITfDatabaseService>();
			ITfBlobManager blobManager = ServiceProvider.GetRequiredService<ITfBlobManager>();
			ITfRepositoryService repoService = ServiceProvider.GetRequiredService<ITfRepositoryService>();

			using (var scope = dbService.CreateTransactionScope(Constants.DB_OPERATION_LOCK_KEY))
			{
				var filesResult = repoService.GetFiles();
				filesResult.IsSuccess.Should().BeTrue();

				var tmpFilePath = CreateTmpFile("This is a test content1.");
				var createResult = repoService.CreateFile("test.bin", tmpFilePath);
				createResult.IsSuccess.Should().BeTrue();

				var fileResult = repoService.GetFile("test.bin");
				fileResult.IsSuccess.Should().BeTrue();
				fileResult.Value.Filename.Should().Be("test.bin");

				filesResult = repoService.GetFiles();
				filesResult.IsSuccess.Should().BeTrue();
				filesResult.Value.Count.Should().Be(1);
				filesResult.Value[0].Filename.Should().Be("test.bin");

				tmpFilePath = CreateTmpFile("This is a test content2.");
				createResult = repoService.CreateFile("rumen.bin", tmpFilePath);
				createResult.IsSuccess.Should().BeTrue();

				filesResult = repoService.GetFiles(filenameStartsWith:"r");
				filesResult.IsSuccess.Should().BeTrue();
				filesResult.Value.Count.Should().Be(1);
				filesResult.Value[0].Filename.Should().Be("rumen.bin");

				filesResult = repoService.GetFiles(filenameStartsWith: "t");
				filesResult.IsSuccess.Should().BeTrue();
				filesResult.Value.Count.Should().Be(1);
				filesResult.Value[0].Filename.Should().Be("test.bin");

				filesResult = repoService.GetFiles(filenameContains: "e");
				filesResult.IsSuccess.Should().BeTrue();
				filesResult.Value.Count.Should().Be(2);

				tmpFilePath = CreateTmpFile("This is a test content3.");
				
				var updateResult = repoService.UpdateFile("rumen.bin", tmpFilePath);
				updateResult.IsSuccess.Should().BeTrue();

				var deleteResult = repoService.DeleteFile("rumen.bin");
				deleteResult.IsSuccess.Should().BeTrue();

				fileResult = repoService.GetFile("rumen.bin");
				fileResult.IsSuccess.Should().BeTrue();
				fileResult.Value.Should().BeNull();

				//cleanup by deleting blob storage folder
				Directory.Delete(blobManager.BlobStoragePath, true);
			}
		}
	}

	public static string CreateTmpFile(string content )
	{
		var tmpFilePath = System.IO.Path.GetTempPath() + Guid.NewGuid().ToString() + ".json";
		var fileStream = File.Open(tmpFilePath, FileMode.CreateNew, FileAccess.ReadWrite);
		var bw = new BinaryWriter(fileStream);
		bw.Write( Encoding.UTF8.GetBytes(content));
		bw.Close();
		fileStream.Close();

		return tmpFilePath;
	}
}