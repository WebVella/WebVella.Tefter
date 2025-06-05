using Bogus;

namespace WebVella.Tefter.Tests.Services;

public partial class TfServiceTest : BaseTest
{
	[Fact]
	public async Task Id_CRUD()
	{
		using (await locker.LockAsync())
		{
			ITfDatabaseService dbService = ServiceProvider.GetRequiredService<ITfDatabaseService>();
			ITfService tfService = ServiceProvider.GetService<ITfService>();

			using (var scope = dbService.CreateTransactionScope(TfConstants.DB_OPERATION_LOCK_KEY))
			{
				Guid? id1 = null;
				Guid? id2 = null;

				var task = Task.Run(() => { id1 = tfService.GetId("test"); });
				var exception = Record.ExceptionAsync(async () => await task).Result;
				exception.Should().BeNull();
				id1.Should().NotBeNull();

				task = Task.Run(() => { id2 = tfService.GetId("test"); });
				exception = Record.ExceptionAsync(async () => await task).Result;
				exception.Should().BeNull();
				id2.Should().NotBeNull();

				id1.Should().Be(id2.Value);
			}
		}
	}
}