namespace WebVella.Tefter.Web.Tests.Components;

public class HomeToolbarComponentTests : BaseTest
{

	[Fact]
	public async Task RendersCorrectly()
	{
		using (await locker.LockAsync())
		{
			//Given
			var Context = GetTestContext();
			// Act
			var cut = Context.RenderComponent<TfHome>();

			// Assert
			cut.Find(".home-toolbar");

			Context.DisposeComponents();
		}
	}

}
