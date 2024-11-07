using System.IO;

namespace WebVella.Tefter.Tests;

public partial class TfRepositoryServiceTests : BaseTest
{

	[Fact]
	public async Task RepositoryService_CRUD()
	{
		using (await locker.LockAsync())
		{
			ITfDatabaseService dbService = ServiceProvider.GetRequiredService<ITfDatabaseService>();
			ITfRepositoryService repoService = ServiceProvider.GetRequiredService<ITfRepositoryService>();

			using (var scope = dbService.CreateTransactionScope(Constants.DB_OPERATION_LOCK_KEY))
			{
				var filesResult = repoService.GetFiles();
				filesResult.IsSuccess.Should().BeTrue();

				var tmpFilePath = CreateTmpFile();
				var createResult = repoService.CreateFile("test.bin", tmpFilePath);
				createResult.IsSuccess.Should().BeTrue();

				var file = createResult.Value;
			}
		}
	}

	public static string CreateTmpFile()
	{
		var tmpFilePath = System.IO.Path.GetTempPath() + Guid.NewGuid().ToString() + ".json";
		var fileStream = File.Open(tmpFilePath, FileMode.CreateNew, FileAccess.ReadWrite);
		var bw = new BinaryWriter(fileStream);
		bw.Write( Encoding.UTF8.GetBytes("This is a test content in tmp file."));
		bw.Close();
		fileStream.Close();

		return tmpFilePath;
	}
}