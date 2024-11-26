namespace WebVella.Tefter.Web.Tests.Components;

public class HomeComponentTests : BaseTest
{

	[Fact]
	public void RendersCorrectly()
	{
		//Given
		var Context = GetTestContext();
		// Act
		var cut = Context.RenderComponent<TfHome>();

		// Assert
		cut.Find(".tf-layout__body__main");

		Context.DisposeComponents();

	}

}
