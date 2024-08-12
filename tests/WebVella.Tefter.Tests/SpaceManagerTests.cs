namespace WebVella.Tefter.Tests;

public partial class SpaceManagerTests : BaseTest
{
	[Fact]
	public async Task CRUD_ID()
	{
		using (await locker.LockAsync())
		{
			IDatabaseService dbService = ServiceProvider.GetRequiredService<IDatabaseService>();
			ITfSpaceManager spaceManager = ServiceProvider.GetRequiredService<ITfSpaceManager>();

			using (var scope = dbService.CreateTransactionScope(Constants.DB_OPERATION_LOCK_KEY))
			{
				var space = new TfSpace
				{
					Id = Guid.NewGuid(),
					Name = "Space1",
					Color = 10,
					Icon = "icon1",
					IsPrivate = false,
					Position = 0
				};
				var result = spaceManager.CreateSpace(space);
				result.IsSuccess.Should().BeTrue();
				result.Value.Should().NotBeNull();
				result.Value.Id.Should().Be(space.Id);
				result.Value.Name.Should().Be(space.Name);
				result.Value.Position.Should().Be(1);
				result.Value.IsPrivate.Should().Be(space.IsPrivate);
				result.Value.Icon.Should().Be(space.Icon);
				result.Value.Color.Should().Be(space.Color);


			}
		}
	}
}
