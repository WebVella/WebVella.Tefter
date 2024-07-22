namespace WebVella.Tefter.Tests;

public partial class DataManagerTests : BaseTest
{
	[Fact]
	public async Task CRUD_ID()
	{
		using (await locker.LockAsync())
		{
			IDatabaseService dbService = ServiceProvider.GetRequiredService<IDatabaseService>();
			IIdentityManager identityManager = ServiceProvider.GetRequiredService<IIdentityManager>();
			IDataManager dataManager = ServiceProvider.GetRequiredService<IDataManager>();

			using (var scope = dbService.CreateTransactionScope(Constants.DB_OPERATION_LOCK_KEY))
			{
				var idResult = dataManager.GetId("test");
				idResult.Should().NotBeNull();
				idResult.IsSuccess.Should().BeTrue();

				var idResult2 = dataManager.GetId("test");
				idResult2.Should().NotBeNull();
				idResult2.IsSuccess.Should().BeTrue();

				idResult.Value.Should().Be(idResult2.Value);

				var id = Guid.NewGuid();


				var idResult6 = dataManager.GetId(id);
				idResult6.Should().NotBeNull();
				idResult6.IsSuccess.Should().BeTrue();
				idResult6.Value.Should().Be(id);
			}
		}
	}
}
