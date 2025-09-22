namespace WebVella.Tefter.Tests.Services;

public partial class TfServiceTest : BaseTest
{

	[Fact]
	public async Task Repository_CRUD()
	{
		using (await locker.LockAsync())
		{
			ITfDatabaseService dbService = ServiceProvider.GetRequiredService<ITfDatabaseService>();
			ITfService tfService = ServiceProvider.GetService<ITfService>();

			using (var scope = dbService.CreateTransactionScope(TfConstants.DB_OPERATION_LOCK_KEY))
			{
				var filesResult = tfService.GetRepositoryFiles();

				var tmpFilePath = CreateTmpFile("This is a test content1.");
				var createResult = tfService.CreateRepositoryFile("test.bin", tmpFilePath);

				var fileResult = tfService.GetRepositoryFile("test.bin");
				fileResult.Filename.Should().Be("test.bin");

				filesResult = tfService.GetRepositoryFiles();
				filesResult.Count.Should().Be(1);
				filesResult[0].Filename.Should().Be("test.bin");

				tmpFilePath = CreateTmpFile("This is a test content2.");
				createResult = tfService.CreateRepositoryFile("rumen.bin", tmpFilePath);

				filesResult = tfService.GetRepositoryFiles(filenameStartsWith: "r");
				filesResult.Count.Should().Be(1);
				filesResult[0].Filename.Should().Be("rumen.bin");

				filesResult = tfService.GetRepositoryFiles(filenameStartsWith: "t");
				filesResult.Count.Should().Be(1);
				filesResult[0].Filename.Should().Be("test.bin");

				filesResult = tfService.GetRepositoryFiles(filenameContains: "e");
				filesResult.Count.Should().Be(2);

				tmpFilePath = CreateTmpFile("This is a test content3.");

				tfService.UpdateRepositoryFile("rumen.bin", tmpFilePath);

				tfService.DeleteRepositoryFile("rumen.bin");

				fileResult = tfService.GetRepositoryFile("rumen.bin");
				fileResult.Should().BeNull();

				//cleanup by deleting blob storage folder
				Directory.Delete(tfService.BlobStoragePath, true);
			}
		}
	}

	public static string CreateTmpFile(string content)
	{
		var tmpFilePath = Path.GetTempPath() + Guid.NewGuid().ToString() + ".json";
		var fileStream = File.Open(tmpFilePath, FileMode.CreateNew, FileAccess.ReadWrite, FileShare.ReadWrite);
		var bw = new BinaryWriter(fileStream);
		bw.Write(Encoding.UTF8.GetBytes(content));
		bw.Close();
		fileStream.Close();

		return tmpFilePath;
	}
}