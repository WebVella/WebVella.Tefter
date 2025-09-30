namespace WebVella.Tefter.UI.Tests.Components;
public class LoadingPaneComponentTests : BaseTest
{

	[Fact]
	public void RendersCorrectly()
	{
		//Given
		var Context = GetTestContext();
		// Act
		var cut = Context.RenderComponent<TucLoadingPane>();

		// Assert
		cut.WaitForElements(".loading", 1, TimeSpan.FromSeconds(2));

		Context.DisposeComponents();
	}
}
