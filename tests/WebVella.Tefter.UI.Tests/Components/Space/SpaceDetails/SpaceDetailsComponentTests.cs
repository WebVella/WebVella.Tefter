namespace WebVella.Tefter.UI.Tests.Components;

public class SpaceDetailsComponentTests : BaseTest
{

	[Fact]
	public void RendersCorrectly()
	{
		//Given
		var Context = GetTestContext();
		// Act
		var cut = Context.RenderComponent<TfSpaceDetails>();

		// Assert
		cut.Find(".tf-layout__body__main");

		Context.DisposeComponents();
	}

}
