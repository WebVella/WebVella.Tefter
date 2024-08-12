namespace WebVella.Tefter.Tests;

public partial class SpaceManagerTests : BaseTest
{
	[Fact]
	public async Task CRUD_Space()
	{
		using (await locker.LockAsync())
		{
			IDatabaseService dbService = ServiceProvider.GetRequiredService<IDatabaseService>();
			ITfSpaceManager spaceManager = ServiceProvider.GetRequiredService<ITfSpaceManager>();

			using (var scope = dbService.CreateTransactionScope(Constants.DB_OPERATION_LOCK_KEY))
			{
				var space1 = new TfSpace
				{
					Id = Guid.NewGuid(),
					Name = "Space1",
					Color = 10,
					Icon = "icon1",
					IsPrivate = false,
					Position = 0
				};
				var result = spaceManager.CreateSpace(space1);
				result.IsSuccess.Should().BeTrue();
				result.Value.Should().NotBeNull();
				result.Value.Id.Should().Be(space1.Id);
				result.Value.Name.Should().Be(space1.Name);
				result.Value.Position.Should().Be(1);
				result.Value.IsPrivate.Should().Be(space1.IsPrivate);
				result.Value.Icon.Should().Be(space1.Icon);
				result.Value.Color.Should().Be(space1.Color);

				var space2 = new TfSpace
				{
					Id = Guid.NewGuid(),
					Name = "Space2",
					Color = 12,
					Icon = "icon2",
					IsPrivate = false,
					Position = 0
				};
				result = spaceManager.CreateSpace(space2);
				result.IsSuccess.Should().BeTrue();
				result.Value.Should().NotBeNull();
				result.Value.Id.Should().Be(space2.Id);
				result.Value.Name.Should().Be(space2.Name);
				result.Value.Position.Should().Be(2);
				result.Value.IsPrivate.Should().Be(space2.IsPrivate);
				result.Value.Icon.Should().Be(space2.Icon);
				result.Value.Color.Should().Be(space2.Color);

				result = spaceManager.MoveSpaceDown(space1.Id);
				result.IsSuccess.Should().BeTrue();

				space1 = spaceManager.GetSpace(space1.Id).Value;
				space2 = spaceManager.GetSpace(space2.Id).Value;
				space1.Position.Should().Be(2);
				space2.Position.Should().Be(1);

				result = spaceManager.MoveSpaceUp(space1.Id);
				result.IsSuccess.Should().BeTrue();

				space1 = spaceManager.GetSpace(space1.Id).Value;
				space2 = spaceManager.GetSpace(space2.Id).Value;
				space1.Position.Should().Be(1);
				space2.Position.Should().Be(2);

				space1.Name = "updated name";
				space1.Icon = "updated icon";
				space1.Color = 100;
				space1.IsPrivate = true;

				result = spaceManager.UpdateSpace(space1);
				result.IsSuccess.Should().BeTrue();
				result.Value.Should().NotBeNull();
				result.Value.Id.Should().Be(space1.Id);
				result.Value.Name.Should().Be(space1.Name);
				result.Value.Position.Should().Be(1);
				result.Value.IsPrivate.Should().Be(space1.IsPrivate);
				result.Value.Icon.Should().Be(space1.Icon);
				result.Value.Color.Should().Be(space1.Color);

				result = spaceManager.DeleteSpace(space1.Id);
				result.IsSuccess.Should().BeTrue();
				space2 = spaceManager.GetSpace(space2.Id).Value;
				space2.Position.Should().Be(1);



			}
		}
	}
}
