namespace WebVella.Tefter.Tests;

public partial class IdManagerTests : BaseTest
{
	[Fact]
	public async Task CRUD_ID()
	{
		using (await locker.LockAsync())
		{
			IDatabaseService dbService = ServiceProvider.GetRequiredService<IDatabaseService>();
			IIdentityManager identityManager = ServiceProvider.GetRequiredService<IIdentityManager>();
			IIdManager idMan = ServiceProvider.GetRequiredService<IIdManager>();

			using (var scope = dbService.CreateTransactionScope(Constants.DB_OPERATION_LOCK_KEY))
			{
				var idResult = idMan.Get("test");
				idResult.Should().NotBeNull();
				idResult.IsSuccess.Should().BeTrue();

				var idResult2 = idMan.Get("test");
				idResult2.Should().NotBeNull();
				idResult2.IsSuccess.Should().BeTrue();

				idResult.Value.Should().Be(idResult2.Value);

				var idResult3 = await idMan.GetAsync("test2");
				idResult3.Should().NotBeNull();
				idResult3.IsSuccess.Should().BeTrue();

				var idResult4 = await idMan.GetAsync("test");
				idResult4.Should().NotBeNull();
				idResult4.IsSuccess.Should().BeTrue();

				idResult3.Value.Should().NotBe(idResult4.Value);
				idResult.Value.Should().Be(idResult4.Value);
			}
		}
	}
}
