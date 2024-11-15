namespace WebVella.Tefter.Web.Tests.Components;

public class SpaceViewPagesComponentTests : BaseTest
{

	[Fact]
	public void RendersCorrectly()
	{
		//Given
		var Context = GetTestContext();
		// Act
		var cut = Context.RenderComponent<TfSpaceViewPages>();

		// Assert
		cut.Find(".fluent-messagebar");

		Context.DisposeComponents();
	}

}
