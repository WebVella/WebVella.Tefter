namespace WebVella.Tefter.Web.Tests.Components;
public class AdminNavigationComponentTests : BaseTest
{

	[Fact]
	public async Task RendersCorrectly()
	{
		using (await locker.LockAsync())
		{
			//Given

			// Act
			var cut = Context.RenderComponent<TfAdminNavigation>();

			// Assert
			cut.Find(".tf-navigation");
		}
	}
}
