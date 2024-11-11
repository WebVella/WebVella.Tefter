namespace WebVella.Tefter.Web.Tests.Components;
public class LoadingPaneComponentTests : BaseTest
{

	[Fact]
	public async Task RendersCorrectly()
	{
		using (await locker.LockAsync())
		{
			//Given

			// Act
			var cut = Context.RenderComponent<TfLoadingPane>();

			// Assert
			cut.WaitForElements(".loading", 1, TimeSpan.FromSeconds(2));
		}
	}
}
