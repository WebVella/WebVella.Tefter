namespace WebVella.Tefter.Web.Tests.Components;
public class PageHeaderComponentTests : BaseTest
{

	[Fact]
	public void RendersCorrectly()
	{
		//Given
		var Context = GetTestContext();
		// Act
		var cut = Context.RenderComponent<TfPageHeader>();

		// Assert
		cut.Find(".page-header");

		Context.DisposeComponents();
	}
}
