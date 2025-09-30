namespace WebVella.Tefter.UI.Tests.Components;

public class SpaceViewDetailsComponentTests : BaseTest
{

	[Fact]
	public void RendersCorrectly()
	{
		//Given
		var Context = GetTestContext();
		// Act
		var cut = Context.RenderComponent<TfSpaceViewDetails>();

		// Assert
		cut.Find(".tf-layout__body__main");

		Context.DisposeComponents();
	}

}
