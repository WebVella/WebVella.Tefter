namespace WebVella.Tefter.Web.Tests.Components;
public class SidebarToggleComponentTests : BaseTest
{

	[Fact]
	public async Task RendersCorrectly()
	{
		using (await locker.LockAsync())
		{
			//Given

			// Act
			var cut = Context.RenderComponent<TfSidebarToggle>();

			// Assert
			cut.Find("fluent-button");
		}
	}

}
