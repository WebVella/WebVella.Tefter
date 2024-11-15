namespace WebVella.Tefter.Web.Tests.Components;

public class SpaceDataDetailsComponentTests : BaseTest
{

	[Fact]
	public void RendersCorrectly()
	{
		//Given
		var Context = GetTestContext();
		// Act
		var cut = Context.RenderComponent<TfSpaceDataDetails>();

		// Assert
		cut.Find(".tf-layout__body__main");

		Context.DisposeComponents();
	}

}
