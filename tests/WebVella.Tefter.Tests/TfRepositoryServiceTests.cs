namespace WebVella.Tefter.Tests;

public partial class TfRepositoryServiceTests : BaseTest
{

	[Fact]
	public async Task RepositoryService_CRUD()
	{
		using (await locker.LockAsync())
		{
			ITfDatabaseService dbService = ServiceProvider.GetRequiredService<ITfDatabaseService>();
			ITfRepositoryService repoSercice = ServiceProvider.GetRequiredService<ITfRepositoryService>();

			using (var scope = dbService.CreateTransactionScope(Constants.DB_OPERATION_LOCK_KEY))
			{
				var files = repoSercice.GetFiles();
			}
		}
	}

	public static byte[] ReadFully(Stream input)
	{
		byte[] buffer = new byte[16 * 1024];
		using (MemoryStream ms = new MemoryStream())
		{
			int read;
			while ((read = input.Read(buffer, 0, buffer.Length)) > 0)
			{
				ms.Write(buffer, 0, read);
			}
			return ms.ToArray();
		}
	}
}