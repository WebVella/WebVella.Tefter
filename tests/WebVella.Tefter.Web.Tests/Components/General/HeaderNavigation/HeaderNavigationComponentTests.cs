namespace WebVella.Tefter.Web.Tests.Components;
public class HeaderNavigationComponentTests : BaseTest
{

	[Fact]
	public async Task RendersCorrectly()
	{
		using (await locker.LockAsync())
		{
			//Given

			// Act
			var cut = Context.RenderComponent<TfHeaderNavigation>();

			// Assert
			cut.Nodes.Length.Should().Be(0);
		}
	}
}
