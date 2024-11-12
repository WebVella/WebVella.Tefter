namespace WebVella.Tefter.Web.Tests.Components;
public class PageLoaderComponentTests : BaseTest
{

	[Fact]
	public void RendersCorrectly()
	{
		//Given
		var Context = GetTestContext();
		// Act
		var cut = Context.RenderComponent<TfPageLoader>();

		// Assert
		cut.Find(".tf-loader-bar");

		Context.DisposeComponents();
	}
}
