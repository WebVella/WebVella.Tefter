namespace WebVella.Tefter.UI.Tests.Components;
public class AdminNavigationComponentTests : BaseTest
{

	[Fact]
	public async Task RendersCorrectly()
	{
		using (await locker.LockAsync())
		{
			//Given
			var Context = GetTestContext();
			// Act
			var cut = Context.RenderComponent<TfAdminNavigation>();

			// Assert
			cut.Find(".tf-navigation");

			Context.DisposeComponents();
		}
	}
}
