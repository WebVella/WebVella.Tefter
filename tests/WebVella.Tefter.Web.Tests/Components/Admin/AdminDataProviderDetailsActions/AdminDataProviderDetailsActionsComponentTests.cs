namespace WebVella.Tefter.Web.Tests.Components;
public class AdminDataProviderDetailsActionsComponentTests : BaseTest
{

	[Fact]
	public async Task RendersCorrectly()
	{
		using (await locker.LockAsync())
		{
			//Given

			// Act
			var cut = Context.RenderComponent<TfAdminDataProviderDetailsActions>();

			// Assert
			cut.Nodes.Length.Should().Be(0);
		}
	}
}
